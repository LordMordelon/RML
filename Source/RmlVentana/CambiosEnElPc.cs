using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RmlVentana.Logica;
using Verse;

namespace RmlVentana
{
    /** «Ver cambios» contra la version de RML que el jugador tenia en su PC (PLAN-CAMBIOS-LOCALES.md).
     *
     *  Arranca con el juego, en segundo plano: recorre las carpetas de Data/ que el juego cargo (las de
     *  los mods activos, en esta version), compara la huella de cada una con la de su copia en
     *  Config/RML_Textos y lee solo las que cambiaron. Lo que cambio va a Config/RML_Cambios.tsv, con el
     *  formato del Cambios.tsv del builder. Si la ventana se abre antes de que termine, usa lo del
     *  builder, como antes.
     *
     *  Un mod sin copia sigue con lo del builder. Su primera copia, y la de uno que cambio, se ponen en
     *  su lugar al cerrar la ventana (AlCerrar): hasta entonces quedan al lado, con «.nueva» al final, y
     *  si el juego se cierra antes, el proximo inicio vuelve a mostrar lo mismo.
     *
     *  Nada de esto puede romper el juego: todo va en try, y un mod que no se puede leer se salta.
     */
    public static class CambiosEnElPc
    {
        private class Carpeta
        {
            /** «Data/!Autor/Mod - 123», como en el LoadFolders.xml y en Ingles.tsv. */
            public string Relativa;
            public string Ruta;
            public string PackageId;
            public string Nombre;
        }

        private class Resultado
        {
            public readonly Dictionary<string, CambiosDeUnMod> Mods = new Dictionary<string, CambiosDeUnMod>();
            /** La copia de cada mod que cambio y su version nueva, para AlCerrar. */
            public readonly List<(string Copia, string Nueva)> PorPoner = new List<(string, string)>();
            public int Leidos, Errores;
            public string Error;
        }

        private static Task<Resultado> Tarea;

        public static string ArchivoLocal => Path.Combine(GenFilePaths.ConfigFolderPath, CambiosLocales.Archivo);

        /** Los mods activos con copia, con lo que cambio en cada uno. Null mientras se calcula o si fallo. */
        public static Dictionary<string, CambiosDeUnMod> Locales =>
            Tarea != null && Tarea.Status == TaskStatus.RanToCompletion && Tarea.Result.Error == null ? Tarea.Result.Mods : null;

        /** Para el log del modo desarrollador. */
        public static string Estado()
        {
            if (Tarea == null) return "sin empezar";
            if (!Tarea.IsCompleted) return "calculando";
            if (Tarea.Status != TaskStatus.RanToCompletion) return "falló (" + Tarea.Exception?.InnerException?.Message + ")";
            Resultado R = Tarea.Result;
            if (R.Error != null) return "falló (" + R.Error + ")";
            return R.Mods.Count + " mods con copia, " + R.Mods.Values.Count(x => x.Total > 0) + " con cambios, " +
                   R.Leidos + " leídos, " + R.Errores + " con error";
        }

        /** Las carpetas se averiguan aca, en el hilo del juego; el resto va en otro hilo, para no demorar
         *  la carga. */
        public static void Empezar(ModContentPack Rml)
        {
            try
            {
                var Carpetas = CarpetasCargadas(Rml);
                string Raiz = Rml.RootDir;
                string Config = GenFilePaths.ConfigFolderPath;
                Tarea = Task.Run(() => Calcular(Raiz, Config, Carpetas));
            }
            catch (Exception e)
            {
                Log.Warning("[Traducciones [Español Latino]] No se pudo revisar qué cambió en tus mods: " + e.Message);
            }
        }

        /** Con la ventana cerrada, lo que se mostro pasa a ser lo que el jugador tiene. */
        public static void AlCerrar()
        {
            Resultado R = Tarea != null && Tarea.Status == TaskStatus.RanToCompletion ? Tarea.Result : null;
            if (R == null || R.Error != null)
                return;

            foreach (var (Copia, Nueva) in R.PorPoner)
            {
                try
                {
                    if (File.Exists(Nueva))
                        Reemplazar(Nueva, Copia);
                }
                catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
                {
                }
            }
            R.PorPoner.Clear();
            foreach (CambiosDeUnMod Mod in R.Mods.Values)
                Mod.Visto = true;
        }

        /** Las carpetas de Data/ que el juego cargo, con el mod que piden. Las de Ajustes/ y Common/ no:
         *  la ventana no las muestra. */
        private static List<Carpeta> CarpetasCargadas(ModContentPack Rml)
        {
            var Cargadas = new HashSet<string>(Rml.foldersToLoadDescendingOrder.Select(Normal), StringComparer.OrdinalIgnoreCase);
            var Resultado = new List<Carpeta>();
            foreach (LoadFolder Carpeta in LecturaDelJuego.CarpetasDeLaVersion(Rml.ModMetaData) ?? new List<LoadFolder>())
            {
                string Relativa = Carpeta.folderName.Replace('\\', '/');
                if (!Relativa.StartsWith("Data/") || Carpeta.requiredAnyOfPackageIds == null || Carpeta.requiredAnyOfPackageIds.Count == 0)
                    continue;

                string Ruta = Path.Combine(Rml.RootDir, Carpeta.folderName);
                if (!Cargadas.Contains(Normal(Ruta)))
                    continue;

                string Nombre = Relativa.Substring(Relativa.LastIndexOf('/') + 1);
                int Guion = Nombre.LastIndexOf(" - ", StringComparison.Ordinal);
                Resultado.Add(new Carpeta
                {
                    Relativa = Relativa,
                    Ruta = Ruta,
                    PackageId = LecturaDelJuego.Id(Carpeta.requiredAnyOfPackageIds[0]),
                    Nombre = Guion > 0 ? Nombre.Substring(0, Guion) : Nombre,
                });
            }
            return Resultado;
        }

        private static string Normal(string Ruta) => Path.GetFullPath(Ruta).TrimEnd('\\', '/');

        /** Corre fuera del hilo del juego: solo archivos, nada del juego. */
        private static Resultado Calcular(string Raiz, string Config, List<Carpeta> Carpetas)
        {
            var R = new Resultado();
            try
            {
                string Copias = Path.Combine(Config, CopiaDeTextos.Carpeta);
                Directory.CreateDirectory(Copias);

                // Primero las huellas: lo que no cambio no se lee.
                var PorLeer = new List<(Carpeta Carpeta, List<(string Ruta, string Relativa)> Archivos, string Huella, string Copia)>();
                foreach (Carpeta Carpeta in Carpetas)
                {
                    try
                    {
                        var Archivos = Directory.EnumerateFiles(Carpeta.Ruta, "*.xml", SearchOption.AllDirectories)
                            .Select(x => (Ruta: x, Relativa: x.Substring(Carpeta.Ruta.Length).Replace('\\', '/').TrimStart('/')))
                            .Where(x => TextosDeTraduccion.EsDeTraduccion(x.Relativa))
                            .OrderBy(x => x.Relativa, StringComparer.Ordinal)
                            .ToList();
                        string Huella = CopiaDeTextos.Huella(Archivos.Select(x =>
                        {
                            var Info = new FileInfo(x.Ruta);
                            return (x.Relativa, Info.Length, Info.LastWriteTimeUtc.Ticks);
                        }));
                        string Copia = Path.Combine(Copias, CopiaDeTextos.NombreDeArchivo(Carpeta.Relativa));

                        if (HuellaDe(Copia) == Huella)
                            R.Mods[Carpeta.PackageId] = CambiosLocales.Contar(Carpeta.PackageId, Carpeta.Nombre, new List<TextosDeTraduccion.Cambio>());
                        else
                            PorLeer.Add((Carpeta, Archivos, Huella, Copia));
                    }
                    catch (Exception)
                    {
                        R.Errores++;
                    }
                }

                // El ingles de los que hay que leer, en una sola pasada por Ingles.tsv.
                string ArchivoDeIngles = Path.Combine(Raiz, TextosDeTraduccion.ArchivoDeIngles);
                var Ingles = PorLeer.Count > 0 && File.Exists(ArchivoDeIngles)
                    ? TextosDeTraduccion.LeerIngles(File.ReadLines(ArchivoDeIngles), PorLeer.Select(x => x.Carpeta.Relativa).ToList())
                    : new Dictionary<string, List<string>>();

                var Lineas = new StringBuilder(TextosDeTraduccion.Encabezado + "\n");
                foreach (var (Carpeta, Archivos, Huella, Copia) in PorLeer)
                {
                    try
                    {
                        Ingles.TryGetValue(Carpeta.Relativa, out var DeLaCarpeta);
                        var Ahora = TextosDeTraduccion.ConIngles(
                            Archivos.SelectMany(x => TextosDeTraduccion.Leer(x.Relativa, File.ReadAllText(x.Ruta))), DeLaCarpeta);
                        string Nueva = CopiaDeTextos.Escribir(Huella, Ahora);
                        R.Leidos++;

                        var Vieja = File.Exists(Copia) ? CopiaDeTextos.Leer(File.ReadLines(Copia), out _) : null;
                        var Cambios = Vieja == null ? null : CopiaDeTextos.Comparar(Vieja, Ahora);

                        // Sin nada que mostrar (otra fecha, el mismo texto): la copia se pone al dia ahora.
                        if (Cambios != null && Cambios.Count == 0)
                        {
                            Escribir(Copia, Nueva);
                            R.Mods[Carpeta.PackageId] = CambiosLocales.Contar(Carpeta.PackageId, Carpeta.Nombre, Cambios);
                            continue;
                        }

                        // La primera copia tambien espera a que se cierre la ventana: puesta ahora, en el
                        // proximo inicio el mod ya tendria copia, saldria sin cambios y se perderia la
                        // fila del builder que el jugador todavia no vio.
                        Escribir(Copia + ".nueva", Nueva);
                        R.PorPoner.Add((Copia, Copia + ".nueva"));
                        if (Cambios == null)
                            continue;
                        R.Mods[Carpeta.PackageId] = CambiosLocales.Contar(Carpeta.PackageId, Carpeta.Nombre, Cambios);
                        foreach (var Cambio in Cambios)
                            Lineas.Append(TextosDeTraduccion.Fila(Carpeta.PackageId, Cambio)).Append('\n');
                    }
                    catch (Exception)
                    {
                        R.Errores++;
                    }
                }

                Escribir(Path.Combine(Config, CambiosLocales.Archivo), Lineas.ToString());
            }
            catch (Exception e)
            {
                R.Error = e.Message;
            }
            return R;
        }

        /** La huella guardada en la primera linea de una copia, o null si no hay copia. */
        private static string HuellaDe(string Copia)
        {
            if (!File.Exists(Copia))
                return null;
            using (var Lector = new StreamReader(Copia, Encoding.UTF8))
            {
                string Primera = Lector.ReadLine();
                return Primera == null ? null : CopiaDeTextos.Leer(new[] { Primera }, out string Huella) == null ? null : Huella;
            }
        }

        /** Primero a un archivo aparte y despues en su lugar: si el juego se cierra a la mitad, no queda
         *  una copia cortada que haria salir textos como nuevos. */
        private static void Escribir(string Archivo, string Texto)
        {
            string Temporal = Archivo + ".tmp";
            File.WriteAllText(Temporal, Texto, new UTF8Encoding(false));
            Reemplazar(Temporal, Archivo);
        }

        private static void Reemplazar(string Desde, string Hacia)
        {
            if (File.Exists(Hacia))
                File.Delete(Hacia);
            File.Move(Desde, Hacia);
        }
    }
}
