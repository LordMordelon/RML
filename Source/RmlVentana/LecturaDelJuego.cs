using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RimWorld;
using RmlVentana.Logica;
using Verse;

namespace RmlVentana
{
    /** Lo que la ventana necesita saber de los mods activos, leido de lo que el juego ya cargo. Lo
     *  demas —que estado le toca a cada uno— lo decide Clasificador, que se prueba sin el juego. */
    public static class LecturaDelJuego
    {
        public static string Id(string PackageId) => PackageId?.ToLowerInvariant();

        /** Los mods que RML traduce: los IfModActive de sus carpetas de Data/ en el LoadFolders.xml que
         *  el juego ya leyo. Las de Ajustes/ no cuentan, y Common/ no tiene condicion. */
        public static HashSet<string> TraducidosPorRml(ModContentPack Rml)
        {
            var Resultado = new HashSet<string>();

            foreach (LoadFolder Carpeta in CarpetasDeLaVersion(Rml.ModMetaData) ?? new List<LoadFolder>())
            {
                if (Carpeta.requiredAnyOfPackageIds == null) continue;

                string Nombre = Carpeta.folderName.Replace('\\', '/');
                if (!Nombre.StartsWith("Data/")) continue;

                foreach (string PackageId in Carpeta.requiredAnyOfPackageIds)
                    Resultado.Add(Id(PackageId));
            }

            return Resultado;
        }

        /** Los mods activos, en el orden de la lista de mods. */
        public static List<ModActivo> ModsActivos()
        {
            var Paquetes = LoadedModManager.RunningModsListForReading;
            var Resultado = new List<ModActivo>();

            foreach (ModMetaData Meta in ModsConfig.ActiveModsInLoadOrder)
            {
                var Mod = new ModActivo
                {
                    PackageId = Id(Meta.PackageIdNonUnique),
                    Nombre = Meta.Name,
                    WorkshopId = WorkshopId(Meta),
                    Oficial = Meta.Official,
                };
                Resultado.Add(Mod);

                if (Mod.Oficial) continue;

                // Una carpeta que no se puede leer deja a ese mod como si no tuviera traduccion, en
                // vez de dejar a todos sin ventana.
                try
                {
                    LeerTraducciones(Meta, Mod, Paquetes.FirstOrDefault(x => x.ModMetaData == Meta));
                }
                catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
                {
                }
            }

            return Resultado;
        }

        /** Si el mod trae su traduccion y, si es un paquete de traduccion, a que mods apunta. */
        private static void LeerTraducciones(ModMetaData Meta, ModActivo Mod, ModContentPack Paquete)
        {
            var Cargadas = CarpetasCargadas(Meta, Paquete).ToList();
            var ConEspanol = Cargadas.Where(x => TieneEspanol(x.Ruta)).ToList();

            Mod.TieneEspanol = ConEspanol.Count > 0;
            Mod.EsPaqueteDeTraduccion = Mod.TieneEspanol && !Cargadas.Any(x => TieneContenidoPropio(x.Ruta));

            if (!Mod.EsPaqueteDeTraduccion) return;

            // Un paquete apunta a lo que traduce con IfModActive, o en su About.xml si su carpeta de
            // idioma no tiene condicion. Es una aproximacion: uno que no siga ninguna de las dos
            // convenciones no se detecta.
            foreach (var Carpeta in ConEspanol)
                Mod.Traduce.AddRange(Carpeta.Condiciones);

            if (ConEspanol.Any(x => x.Condiciones.Count == 0))
            {
                // Sin <modDependencies> ni <loadAfter> en el About.xml, el juego deja estas listas en null.
                Mod.Traduce.AddRange((Meta.Dependencies ?? new List<ModDependency>()).Select(x => Id(x.packageId)));
                Mod.Traduce.AddRange((Meta.LoadAfter ?? new List<string>()).Select(Id));
                Mod.Traduce.AddRange((Meta.ForceLoadAfter ?? new List<string>()).Select(Id));
            }

            Mod.Traduce.RemoveAll(x => x == null || x.StartsWith("ludeon."));
        }

        /** Las carpetas que el juego carga de un mod, cada una con los mods que pide (IfModActive). */
        private static IEnumerable<(string Ruta, List<string> Condiciones)> CarpetasCargadas(ModMetaData Meta, ModContentPack Paquete)
        {
            List<LoadFolder> Lista = CarpetasDeLaVersion(Meta);
            if (Lista != null)
            {
                foreach (LoadFolder Carpeta in Lista.Where(x => x.ShouldLoad))
                {
                    var Condiciones = (Carpeta.requiredAnyOfPackageIds ?? new List<string>())
                        .Concat(Carpeta.requiredAllOfPackageIds ?? new List<string>())
                        .Select(Id).ToList();
                    yield return (Path.Combine(Meta.RootDir.FullName, Carpeta.folderName), Condiciones);
                }
                yield break;
            }

            // Sin LoadFolders.xml el juego carga la raiz, Common/ y la carpeta de la version.
            foreach (string Ruta in Paquete?.foldersToLoadDescendingOrder ?? new List<string> { Meta.RootDir.FullName })
                yield return (Ruta, new List<string>());
        }

        /** La lista del LoadFolders.xml que el juego usa: la de esta version, o la mas cercana hacia
         *  atras, o la «default». Como ModContentPack.InitLoadFolders. Null si el mod no tiene. */
        internal static List<LoadFolder> CarpetasDeLaVersion(ModMetaData Meta)
        {
            if (Meta?.loadFolders == null || Meta.loadFolders.DefinedVersions().Count == 0)
                return null;

            List<LoadFolder> Lista = Meta.LoadFoldersForVersion(VersionControl.CurrentVersionStringWithoutBuild);
            if (Lista != null && Lista.Count > 0)
                return Lista;

            // Ordenadas como version y no como texto: «1.10» va despues de «1.9».
            string Anterior = null;
            System.Version Mejor = null;
            foreach (string Definida in Meta.loadFolders.DefinedVersions())
            {
                if (VersionControl.TryParseVersionString(Definida, out var Version) && Version <= VersionControl.CurrentVersion &&
                    (Mejor == null || Version > Mejor))
                {
                    Mejor = Version;
                    Anterior = Definida;
                }
            }

            return (Anterior == null ? null : Meta.LoadFoldersForVersion(Anterior)) ?? Meta.LoadFoldersForVersion("default");
        }

        /** Languages/SpanishLatin, con cualquiera de sus dos nombres, como carpeta o como .tar. */
        private static bool TieneEspanol(string Ruta)
        {
            string Languages = Path.Combine(Ruta, "Languages");
            if (!Directory.Exists(Languages))
                return false;

            return Directory.GetDirectories(Languages).Any(x => Clasificador.EsEspanolLatino(Path.GetFileName(x))) ||
                   Directory.GetFiles(Languages, "*.tar").Any(x => Clasificador.EsEspanolLatino(Path.GetFileNameWithoutExtension(x)));
        }

        /** Un paquete de traduccion no trae Defs ni Assemblies propios. */
        private static bool TieneContenidoPropio(string Ruta) =>
            Directory.Exists(Path.Combine(Ruta, "Defs")) || Directory.Exists(Path.Combine(Ruta, "Assemblies"));

        /** El numero del Workshop: el de About/PublishedFileId.txt o, instalado desde Steam, el nombre de
         *  su carpeta. «0» si es un mod local sin publicar. */
        private static string WorkshopId(ModMetaData Meta)
        {
            try
            {
                string Archivo = Path.Combine(Meta.RootDir.FullName, "About", "PublishedFileId.txt");
                if (File.Exists(Archivo))
                {
                    string Texto = File.ReadAllText(Archivo).Trim();
                    if (Texto.Length > 0 && Texto.All(char.IsDigit))
                        return Texto;
                }

                string Carpeta = Meta.RootDir.Name;
                if (Carpeta.Length > 0 && Carpeta.All(char.IsDigit))
                    return Carpeta;
            }
            catch (IOException)
            {
            }

            return "0";
        }
    }
}
