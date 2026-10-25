using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RmlVentana.Logica;
using Verse;

namespace RmlVentana
{
    /** Lo que GitHub dice de la traduccion oficial en este inicio del juego. */
    public class ConsultaOficial
    {
        public string Sha;
        public DateTime Fecha;
        public string Version;

        /** La compilacion del juego para la que esta hecha («1.7.4982»), para el aviso de juego atrasado. */
        public Version VersionCompleta;

        /** El ultimo cambio de cada carpeta del repositorio («Core», «Biotech»…) hasta ese Sha. */
        public Dictionary<string, DateTime> PorCarpeta = new Dictionary<string, DateTime>();
    }

    /** Una parte de la traduccion oficial (el juego base o un DLC), comparada con su propio .tar. */
    public class ParteOficial
    {
        public string Carpeta;
        public EstadoOficial Estado;
        public DateTime? GitHub;
    }

    /** La parte del boton de la traduccion oficial que toca la red y los archivos del juego
     *  (PLAN-TRADUCCION-OFICIAL.md). Lo que decide esta en Logica/TraduccionOficial.cs.
     *
     *  Solo en 1.7 o mas: el juego corre sobre .NET 10, que trae HttpClient y ZipFile. En 1.6 nada
     *  de esto se llama, y el JIT no resuelve un metodo hasta que se llama.
     */
    public static class OficialJuego
    {
        // Las escribe la tarea de fondo y las lee la pantalla de ajustes.
        private static volatile ConsultaOficial Respuesta;
        private static volatile bool Consultando;
        private static volatile bool Fallo;

        // Lo que se lee del disco, una vez y no en cada cuadro: si cada carpeta de Data/ tiene su .tar y
        // de cuando es. Se rehace despues de instalar o deshacer.
        private static bool Leido;
        private static Dictionary<string, (bool Presente, DateTime? Fecha)> Tars = new Dictionary<string, (bool, DateTime?)>();

        /** El orden en que salieron: asi se listan en los ajustes. Lo que no esta aca va al final. */
        private static readonly string[] Orden = { "Core", "Royalty", "Ideology", "Biotech", "Anomaly", "Odyssey" };

        /** El error de la ultima instalacion, para mostrarlo en los ajustes. */
        public static string Error;

        private static AjustesRml Ajustes => RmlVentanaMod.Instancia.Ajustes;

        public static bool Disponible =>
            VersionControl.CurrentMajor > 1 || (VersionControl.CurrentMajor == 1 && VersionControl.CurrentMinor >= 7);

        public static string VersionDelJuego => VersionControl.CurrentMajor + "." + VersionControl.CurrentMinor;

        public static ConsultaOficial Consulta => Respuesta;
        public static bool EstaConsultando => Consultando;
        public static bool FalloLaConsulta => Fallo;

        public static string ArchivoDeCambios => Path.Combine(GenFilePaths.ConfigFolderPath, "RML_CambiosOficiales.tsv");

        private static string Languages(string Carpeta) => Path.Combine(GenFilePaths.OfficialModsFolderPath, Carpeta, "Languages");
        private static string Tar(string Carpeta) => Path.Combine(Languages(Carpeta), TraduccionOficial.Idioma + ".tar");
        private static string Respaldo(string Carpeta) => Path.Combine(Languages(Carpeta), TraduccionOficial.Idioma + TraduccionOficial.ExtensionDelRespaldo);
        private static string Instalada(string Carpeta) => Path.Combine(Languages(Carpeta), TraduccionOficial.Idioma);

        /** Junto a Data/ y no en la temporal de Windows: en el mismo disco, mover una carpeta es un
         *  renombre y no deja nada a medias. Fuera de Data/, el juego no la ve como un DLC. */
        private static string Temporal => Path.Combine(Directory.GetParent(GenFilePaths.OfficialModsFolderPath).FullName, "RML-temporal");

        public static string NombreDe(string Carpeta) =>
            "RML_Oficial_NombreFila".Translate(Carpeta == "Core" ? "RimWorld" : Carpeta);

        public static EstadoOficial Estado() => TraduccionOficial.Combinar(Partes().Select(x => x.Estado));

        /** Cada parte con su estado. Sin respuesta de GitHub, el juego base y lo instalado, para que
         *  «quedó sin usar» se vea igual. Si GitHub respondio sin las fechas por carpeta, todo contra la
         *  fecha del ultimo cambio, en el juego base. */
        public static List<ParteOficial> Partes()
        {
            if (!Leido)
                LeerDisco();

            ConsultaOficial GitHub = Respuesta;
            DateTime? Instalado = null;
            if (Ajustes.OficialFecha != null && DateTime.TryParse(Ajustes.OficialFecha, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.RoundtripKind, out DateTime Fecha))
                Instalado = Fecha;

            Dictionary<string, DateTime> PorCarpeta = GitHub == null ? null
                : GitHub.PorCarpeta.Count > 0 ? GitHub.PorCarpeta
                : new Dictionary<string, DateTime> { ["Core"] = GitHub.Fecha };
            IEnumerable<string> Carpetas = PorCarpeta != null
                ? PorCarpeta.Keys
                : Tars.Keys.Where(x => x == "Core" || Ajustes.OficialCarpetas.Contains(x));

            return Carpetas.Where(Tars.ContainsKey)
                .OrderBy(x => Array.IndexOf(Orden, x) < 0 ? Orden.Length : Array.IndexOf(Orden, x)).ThenBy(x => x)
                .Select(Carpeta =>
                {
                    DateTime? EnGitHub = PorCarpeta != null && PorCarpeta.TryGetValue(Carpeta, out DateTime Cuando) ? Cuando : (DateTime?)null;
                    return new ParteOficial
                    {
                        Carpeta = Carpeta,
                        GitHub = EnGitHub,
                        Estado = TraduccionOficial.EstadoDe(EnGitHub, Tars[Carpeta].Fecha, Ajustes.OficialCarpetas.Contains(Carpeta) ? Instalado : null,
                            Tars[Carpeta].Presente, Ajustes.OficialReiniciar != null, MismaVersion: GitHub == null || GitHub.Version == VersionDelJuego),
                    };
                })
                .ToList();
        }

        /** Si GitHub tiene, en alguna parte, algo mas nuevo que lo que el juego vuelve a cargar con su .tar. */
        public static bool GitHubEsMasNuevoQueElTar =>
            Respuesta != null && Respuesta.Version == VersionDelJuego &&
            Partes().Any(x => x.GitHub != null && (Tars[x.Carpeta].Fecha == null || x.GitHub > Tars[x.Carpeta].Fecha));

        /** La fecha del arreglo mas nuevo entre las partes atrasadas, para «hay arreglos, del…». */
        public static DateTime FechaDeLosArreglos =>
            Partes().Where(x => x.Estado == EstadoOficial.HayArreglos && x.GitHub != null).Select(x => x.GitHub.Value)
                .DefaultIfEmpty(Respuesta?.Fecha ?? DateTime.MinValue).Max();

        public static bool HayAlgoInstalado => Ajustes.OficialCarpetas.Count > 0;

        private static void LeerDisco()
        {
            Leido = true;
            var Nuevos = new Dictionary<string, (bool, DateTime?)>();
            foreach (string Carpeta in Directory.GetDirectories(GenFilePaths.OfficialModsFolderPath).Select(Path.GetFileName))
            {
                bool Presente = File.Exists(Tar(Carpeta));
                DateTime? Fecha = null;
                if (Presente)
                {
                    try
                    {
                        using (var Archivo = File.OpenRead(Tar(Carpeta)))
                            Fecha = LectorTar.FechaMasNueva(Archivo);
                    }
                    catch (Exception e)
                    {
                        Log.Warning("[Traducciones [Español Latino]] No se pudo leer la fecha de la traducción de " + Carpeta + ": " + e.Message);
                    }
                }
                Nuevos[Carpeta] = (Presente, Fecha);
            }
            Tars = Nuevos;
        }

        /** En segundo plano: la pantalla sigue mientras tanto y muestra «Consultando GitHub…». */
        public static void Consultar()
        {
            if (!Disponible || Consultando)
                return;
            Consultando = true;
            Fallo = false;
            List<string> Carpetas = Directory.GetDirectories(GenFilePaths.OfficialModsFolderPath).Select(Path.GetFileName).ToList();
            Task.Run(() =>
            {
                try
                {
                    ConsultaOficial Nueva = Red.Consultar(Carpetas);
                    Respuesta = Nueva ?? Respuesta;
                    Fallo = Nueva == null;
                }
                catch (Exception)
                {
                    Fallo = true;
                }
                finally
                {
                    Consultando = false;
                }
            });
        }

        private class Resultado
        {
            public readonly List<string> Instaladas = new List<string>();
            public readonly List<string> Novedades = new List<string>();
            public string Error;
        }

        /** Baja, compara e instala en un evento largo, con la pantalla de carga del juego: mientras
         *  tanto no se puede tocar nada, y al terminar los ajustes se guardan en el hilo principal. */
        public static void Instalar()
        {
            ConsultaOficial GitHub = Respuesta;
            // Un doble clic encolaria dos instalaciones seguidas.
            if (GitHub == null || Instalando)
                return;
            Instalando = true;
            Error = null;
            var Hecho = new Resultado();
            LongEventHandler.QueueLongEvent(() => Proceso(GitHub, Hecho), "RML_Oficial_Bajando", true, null,
                callback: () => Aplicar(GitHub, Hecho));
        }

        private static bool Instalando;

        private static void Proceso(ConsultaOficial GitHub, Resultado Hecho)
        {
            string Zip = Path.Combine(Path.GetTempPath(), "RML-oficial-" + GitHub.Sha.Substring(0, 8) + ".zip");
            try
            {
                Red.Bajar(GitHub.Sha, Zip, Avance => LongEventHandler.SetCurrentEventText("RML_Oficial_BajandoAvance".Translate(Avance)));

                try
                {
                    if (Directory.Exists(Temporal))
                        Directory.Delete(Temporal, true);
                    Directory.CreateDirectory(Temporal);
                }
                catch (Exception e) when (e is UnauthorizedAccessException || e is IOException)
                {
                    Hecho.Error = "RML_Oficial_ErrorPermisos".Translate(GenFilePaths.OfficialModsFolderPath);
                    return;
                }

                List<string> Carpetas;
                using (ZipArchive Archivo = ZipFile.OpenRead(Zip))
                {
                    string Raiz = TraduccionOficial.RaizDelZip(Archivo.Entries.Select(x => x.FullName));
                    ZipArchiveEntry Readme = Archivo.GetEntry(Raiz + "README.md");
                    string Version = Readme == null ? null : TraduccionOficial.VersionDelReadme(new StreamReader(Readme.Open()).ReadToEnd());
                    if (Version != VersionDelJuego)
                    {
                        Hecho.Error = "RML_Oficial_ErrorVersion".Translate(Version ?? "?", VersionDelJuego);
                        return;
                    }

                    var DelZip = Archivo.Entries.Select(x => x.FullName.Substring(Raiz.Length).Split('/'))
                        .Where(x => x.Length > 1).Select(x => x[0]).Distinct();
                    Carpetas = TraduccionOficial.CarpetasAInstalar(DelZip,
                        Directory.GetDirectories(GenFilePaths.OfficialModsFolderPath).Select(Path.GetFileName));
                    if (Carpetas.Count == 0)
                    {
                        Hecho.Error = "RML_Oficial_ErrorNada".Translate();
                        return;
                    }

                    LongEventHandler.SetCurrentEventText("RML_Oficial_Preparando".Translate());
                    foreach (ZipArchiveEntry Entrada in Archivo.Entries)
                    {
                        if (Entrada.Name.Length == 0 || !Entrada.FullName.StartsWith(Raiz))
                            continue;
                        string Relativa = Entrada.FullName.Substring(Raiz.Length);
                        string Carpeta = Relativa.Split('/')[0];
                        if (!Carpetas.Contains(Carpeta, StringComparer.OrdinalIgnoreCase) || !TraduccionOficial.RutaSegura(Relativa))
                            continue;
                        string Destino = Path.Combine(Temporal, Relativa);
                        Directory.CreateDirectory(Path.GetDirectoryName(Destino));
                        Entrada.ExtractToFile(Destino, true);
                    }
                }

                // Antes de mover nada: lo que el juego tenia contra lo recien bajado.
                var Lineas = new List<string>();
                string Fecha = GitHub.Fecha.ToString("yyyy-MM-dd");
                foreach (string Carpeta in Carpetas)
                {
                    var Cambios = TraduccionOficial.Comparar(TextosActuales(Carpeta), TextosDeCarpeta(Path.Combine(Temporal, Carpeta)));
                    if (Cambios.Count == 0)
                        continue;
                    string Id = TraduccionOficial.PackageIdDe(Carpeta);
                    Lineas.AddRange(Cambios.Select(x => TraduccionOficial.Linea(Id, x)));
                    Hecho.Novedades.Add(NovedadOficial.Linea(Carpeta, Fecha,
                        Cambios.Count(x => x.Tipo == TipoDeTexto.Cambiado), Cambios.Count(x => x.Tipo == TipoDeTexto.Nuevo),
                        Cambios.Count(x => x.Tipo == TipoDeTexto.Quitado)));
                }
                File.WriteAllLines(ArchivoDeCambios, Lineas, new UTF8Encoding(false));

                foreach (string Carpeta in Carpetas)
                {
                    if (Directory.Exists(Instalada(Carpeta)))
                        Directory.Delete(Instalada(Carpeta), true);
                    Directory.Move(Path.Combine(Temporal, Carpeta), Instalada(Carpeta));
                    if (File.Exists(Tar(Carpeta)))
                    {
                        // Un respaldo viejo junto a un .tar es de antes de que Steam pusiera el suyo: sobra.
                        if (File.Exists(Respaldo(Carpeta)))
                            File.Delete(Respaldo(Carpeta));
                        File.Move(Tar(Carpeta), Respaldo(Carpeta));
                    }
                    Hecho.Instaladas.Add(Carpeta);
                }
            }
            catch (Exception e)
            {
                Hecho.Error = (Hecho.Instaladas.Count > 0 ? "RML_Oficial_ErrorAMedias" : "RML_Oficial_ErrorGeneral").Translate(e.Message);
            }
            finally
            {
                try
                {
                    if (Directory.Exists(Temporal))
                        Directory.Delete(Temporal, true);
                    if (File.Exists(Zip))
                        File.Delete(Zip);
                }
                catch (Exception)
                {
                    // Quedan archivos de sobra fuera de Data/, que el juego no lee; se borran en la proxima.
                }
            }
        }

        private static void Aplicar(ConsultaOficial GitHub, Resultado Hecho)
        {
            Instalando = false;
            Error = Hecho.Error;
            if (Hecho.Instaladas.Count > 0)
            {
                Ajustes.OficialSha = GitHub.Sha;
                Ajustes.OficialFecha = GitHub.Fecha.ToString("o");
                Ajustes.OficialCarpetas = Ajustes.OficialCarpetas.Union(Hecho.Instaladas).ToList();
                Ajustes.OficialNovedades = Hecho.Novedades;
                Ajustes.OficialReiniciar = "instalar";
                RmlVentanaMod.Instancia.WriteSettings();
                Messages.Message("RML_Oficial_Listo".Translate(), MessageTypeDefOf.TaskCompletion, false);
            }
            Leido = false;
        }

        /** Borra lo instalado y devuelve el .tar, o borra el respaldo viejo si Steam ya puso otro. */
        public static void Deshacer()
        {
            Error = null;
            try
            {
                foreach (string Carpeta in Ajustes.OficialCarpetas)
                {
                    // Viene del archivo de ajustes: un nombre raro no puede llevar el borrado a otra carpeta.
                    if (string.IsNullOrEmpty(Carpeta) || Carpeta.IndexOfAny(new[] { '/', '\\', ':' }) >= 0 || Carpeta.Contains(".."))
                        continue;
                    if (Directory.Exists(Instalada(Carpeta)))
                        Directory.Delete(Instalada(Carpeta), true);
                    switch (TraduccionOficial.QueHaceDeshacer(File.Exists(Tar(Carpeta)), File.Exists(Respaldo(Carpeta))))
                    {
                        case Logica.Deshacer.DevolverElTar: File.Move(Respaldo(Carpeta), Tar(Carpeta)); break;
                        case Logica.Deshacer.BorrarElRespaldo: File.Delete(Respaldo(Carpeta)); break;
                    }
                }
                Ajustes.OficialCarpetas = new List<string>();
                Ajustes.OficialNovedades = new List<string>();
                Ajustes.OficialSha = null;
                Ajustes.OficialFecha = null;
                Ajustes.OficialReiniciar = "deshacer";
            }
            catch (Exception e)
            {
                Error = "RML_Oficial_ErrorGeneral".Translate(e.Message);
            }
            RmlVentanaMod.Instancia.WriteSettings();
            Leido = false;
        }

        /** Lo que el juego carga hoy: su .tar si esta, si no la carpeta. */
        private static Dictionary<string, TextoOficial> TextosActuales(string Carpeta)
        {
            if (File.Exists(Tar(Carpeta)))
            {
                var Textos = new Dictionary<string, TextoOficial>();
                using (var Archivo = File.OpenRead(Tar(Carpeta)))
                    foreach (EntradaTar Entrada in LectorTar.Leer(Archivo))
                        Sumar(Textos, Entrada.Nombre, Encoding.UTF8.GetString(Entrada.Datos));
                return Textos;
            }
            return TextosDeCarpeta(Instalada(Carpeta));
        }

        private static Dictionary<string, TextoOficial> TextosDeCarpeta(string Carpeta)
        {
            var Textos = new Dictionary<string, TextoOficial>();
            if (!Directory.Exists(Carpeta))
                return Textos;
            foreach (string Archivo in Directory.GetFiles(Carpeta, "*.xml", SearchOption.AllDirectories))
                Sumar(Textos, Archivo.Substring(Carpeta.Length + 1), File.ReadAllText(Archivo));
            return Textos;
        }

        private static void Sumar(Dictionary<string, TextoOficial> Textos, string Relativa, string Contenido)
        {
            string Prefijo = TraduccionOficial.PrefijoDe(Relativa);
            if (Prefijo == null)
                return;
            foreach (var Par in TraduccionOficial.LeerXml(Contenido, Prefijo))
                Textos[Par.Key] = Par.Value;
        }
    }

    /** Aparte, para que HttpClient no aparezca en ningun metodo que la 1.6 llegue a compilar. */
    internal static class Red
    {
        private static readonly HttpClient Cliente = Crear();

        private static HttpClient Crear()
        {
            var Nuevo = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
            // GitHub rechaza las consultas sin User-Agent.
            Nuevo.DefaultRequestHeaders.UserAgent.ParseAdd("RML-RimWorld-Mod-Latino");
            return Nuevo;
        }

        /** El ultimo cambio de master, la version de su README y el ultimo cambio de cada carpeta de
         *  Data/ que el repositorio tenga. Null si GitHub responde algo raro al primero; una carpeta que
         *  falla solo queda afuera. Son 2 consultas mas una por carpeta, dentro de las 60 por hora. */
        public static ConsultaOficial Consultar(List<string> Carpetas)
        {
            string Api = "https://api.github.com/repos/" + TraduccionOficial.Repositorio + "/commits";
            string Json = Cliente.GetStringAsync(Api + "/master").Result;
            if (!TraduccionOficial.LeerCommit(Json, out string Sha, out DateTime Fecha))
                return null;
            string Readme = Cliente.GetStringAsync("https://raw.githubusercontent.com/" + TraduccionOficial.Repositorio + "/" + Sha + "/README.md").Result;
            var Consulta = new ConsultaOficial
            {
                Sha = Sha,
                Fecha = Fecha,
                Version = TraduccionOficial.VersionDelReadme(Readme),
                VersionCompleta = TraduccionOficial.VersionCompletaDelReadme(Readme),
            };

            foreach (string Carpeta in Carpetas)
            {
                try
                {
                    string DeLaCarpeta = Cliente.GetStringAsync(Api + "?sha=" + Sha + "&per_page=1&path=" + Uri.EscapeDataString(Carpeta)).Result;
                    // Una carpeta que el repositorio no tiene responde con una lista vacia.
                    if (TraduccionOficial.LeerCommit(DeLaCarpeta, out _, out DateTime Cuando))
                        Consulta.PorCarpeta[Carpeta] = Cuando;
                }
                catch (Exception)
                {
                }
            }
            return Consulta;
        }

        /** El zip de ese cambio exacto y no el de master: si alguien sube algo mientras tanto, se instala
         *  lo que dice la fecha guardada. Avance recibe los MB bajados. */
        public static void Bajar(string Sha, string Destino, Action<string> Avance)
        {
            using (HttpResponseMessage Respuesta = Cliente.GetAsync("https://codeload.github.com/" + TraduccionOficial.Repositorio + "/zip/" + Sha,
                       HttpCompletionOption.ResponseHeadersRead).Result)
            {
                Respuesta.EnsureSuccessStatusCode();
                using (Stream Origen = Respuesta.Content.ReadAsStreamAsync().Result)
                using (FileStream Archivo = File.Create(Destino))
                {
                    var Buffer = new byte[81920];
                    long Total = 0, Ultimo = -1;
                    int n;
                    while ((n = Origen.Read(Buffer, 0, Buffer.Length)) > 0)
                    {
                        Archivo.Write(Buffer, 0, n);
                        Total += n;
                        long Megas = Total / (1024 * 1024);
                        if (Megas != Ultimo)
                        {
                            Ultimo = Megas;
                            Avance(Megas.ToString());
                        }
                    }
                }
            }
        }
    }
}
