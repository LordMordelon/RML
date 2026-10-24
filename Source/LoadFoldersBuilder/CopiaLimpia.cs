using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LoadFoldersBuilder;

/** La copia liviana del mod en output/RimWorld Mod Latino. Es lo que carga el juego —Mods\RML
 *  enlaza a esa carpeta— y lo que se sube al Workshop.
 *
 *  Lleva solo lo que RimWorld lee: About, Data, Ajustes, Common (con la DLL), LoadFolders.xml, ModList.tsv y LICENSE.
 *  De Data y Ajustes quedan afuera los UNUSED.xml, los LoadFolders.Build.yaml y los .md, que el juego no usa, y a los XML
 *  se les sacan los comentarios: el <!-- EN: ... --> con el original en ingles sirve para
 *  traducir y revisar en el repo, pero al jugador no le hace nada y era un tercio del peso.
 *
 *  Se rehace en cada -build, asi que despues de una traduccion rapida del extractor el juego
 *  ya tiene el cambio. Es una sincronizacion y no un borrar y copiar: solo escribe lo que
 *  cambio y borra lo que sobra, para no rehacer miles de archivos en cada corrida.
 */
public static class CopiaLimpia
{
    public const string Carpeta = "output";
    public const string NombreDelMod = "RimWorld Mod Latino";

    /** Lo de la raiz del repo que va en la copia, ademas de About, Data, Ajustes y Common. */
    private static readonly string[] ArchivosDeRaiz = ["LoadFolders.xml", "ModList.tsv", "LICENSE"];

    /** Lo de Data que no va: el juego no lo lee. */
    private static readonly HashSet<string> NoVan = new(StringComparer.OrdinalIgnoreCase)
        { "UNUSED.xml", Statics.BuildYamlFileName };

    /** Un comentario que ocupa su linea entera, junto con su salto de linea. El (?!-->) no deja
     *  que un comentario se coma el siguiente. */
    private static readonly Regex ComentarioEnSuLinea = new(@"^[ \t]*<!--(?:(?!-->).)*-->[ \t]*\r?\n",
        RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

    /** Uno que comparte linea con otra cosa. */
    private static readonly Regex Comentario = new(@"<!--(?:(?!-->).)*-->",
        RegexOptions.Singleline | RegexOptions.Compiled);

    public static string Ruta(string RootPath) => Path.Combine(RootPath, Carpeta, NombreDelMod);

    /** Deja la copia al dia. Nunca tira: devuelve false y avisa si algo salio mal. */
    public static bool Armar(string RootPath)
    {
        string Destino = Ruta(RootPath);

        try
        {
            if (!File.Exists(Path.Combine(RootPath, "LoadFolders.xml")))
            {
                Console.WriteLine("\e[93mNo esta el LoadFolders.xml: no se armo la copia en {0}.\x1b[0m", Carpeta);
                return false;
            }

            var Esperados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var EnUso = new List<string>();
            int Escritos = 0, SinComentarios = 0, CopiadosTalCual = 0;
            long Peso = 0;

            void Poner(string Relativa, byte[] Contenido)
            {
                string Archivo = Path.Combine(Destino, Relativa);
                Esperados.Add(Archivo);
                Peso += Contenido.Length;

                if (File.Exists(Archivo) && new FileInfo(Archivo).Length == Contenido.Length &&
                    File.ReadAllBytes(Archivo).AsSpan().SequenceEqual(Contenido))
                    return;

                Directory.CreateDirectory(Path.GetDirectoryName(Archivo)!);
                try
                {
                    File.WriteAllBytes(Archivo, Contenido);
                    Escritos++;
                }
                // Con el juego abierto la DLL de Common/Assemblies queda tomada. Queda la anterior y
                // se sigue: si no, una traduccion rapida con el juego abierto no copiaba nada.
                catch (IOException) when (File.Exists(Archivo))
                {
                    EnUso.Add(Relativa);
                }
            }

            foreach (string Archivo in Directory.EnumerateFiles(Path.Combine(RootPath, "About"), "*", SearchOption.AllDirectories))
                Poner(Path.GetRelativePath(RootPath, Archivo), File.ReadAllBytes(Archivo));

            foreach (string Nombre in ArchivosDeRaiz)
            {
                string Archivo = Path.Combine(RootPath, Nombre);
                if (File.Exists(Archivo))
                    Poner(Nombre, File.ReadAllBytes(Archivo));
            }

            // Se arma aca y no en el repo: sale entero de la historia de git (ver Novedades.cs).
            if (Novedades.Armar(RootPath) is { } TextoDeNovedades)
                Poner(Novedades.Archivo, new System.Text.UTF8Encoding(false).GetBytes(TextoDeNovedades));
            else
                Console.WriteLine("\e[93mNo se pudo armar {0}: la ventana del juego queda sin novedades.\x1b[0m", Novedades.Archivo);

            // Los mods pedidos que no tienen texto, para que la ventana no ofrezca pedirlos de nuevo.
            // Sale de la rama «solicitudes» (ver Solicitudes.cs); sin ella, no va.
            if (Solicitudes.Leer(RootPath, Solicitudes.SinTexto) is { } SinTexto)
                Poner(Solicitudes.SinTexto, new System.Text.UTF8Encoding(false).GetBytes(string.Join("\n", SinTexto) + "\n"));

            foreach (string Archivo in Statics.CarpetasDeOrigen(RootPath)
                         .SelectMany(Carpeta => Directory.EnumerateFiles(Carpeta, "*", SearchOption.AllDirectories)))
            {
                // Los .md son notas de trabajo de una traduccion (el plan y el glosario de Combat
                // Extended viven en su carpeta): sirven en el repo, no en el juego
                if (NoVan.Contains(Path.GetFileName(Archivo)) ||
                    Archivo.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                    continue;

                byte[] Contenido = File.ReadAllBytes(Archivo);
                if (Archivo.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    if (SacarComentarios(Contenido) is { } Liviano)
                    {
                        if (Liviano.Length != Contenido.Length) SinComentarios++;
                        Contenido = Liviano;
                    }
                    else
                    {
                        CopiadosTalCual++;
                        Console.WriteLine("\e[93mNo se pudo leer como XML, se copia tal cual: {0}\x1b[0m",
                            Path.GetRelativePath(RootPath, Archivo));
                    }
                }

                Poner(Path.GetRelativePath(RootPath, Archivo), Contenido);
            }

            // Lo que quedo de una corrida anterior y ya no esta en el repo.
            int Borrados = 0;
            if (Directory.Exists(Destino))
            {
                foreach (string Archivo in Directory.EnumerateFiles(Destino, "*", SearchOption.AllDirectories).ToList())
                {
                    if (Esperados.Contains(Archivo)) continue;
                    File.Delete(Archivo);
                    Borrados++;
                }

                foreach (string Dir in Directory.EnumerateDirectories(Destino, "*", SearchOption.AllDirectories)
                             .OrderByDescending(x => x.Length).ToList())
                {
                    if (!Directory.EnumerateFileSystemEntries(Dir).Any())
                        Directory.Delete(Dir);
                }
            }

            // Que no falte nada: una copia incompleta se publica igual, con traducciones faltantes.
            int Hay = Directory.EnumerateFiles(Destino, "*", SearchOption.AllDirectories).Count();
            if (Hay != Esperados.Count)
            {
                Console.WriteLine("\e[93mLa copia en {0} quedo incompleta: {1} archivos de {2}.\x1b[0m", Carpeta, Hay, Esperados.Count);
                return false;
            }

            Console.WriteLine("\e[32mCopia limpia en {0}\\{1}: {2} archivos, {3:F1} MB ({4} actualizados, {5} borrados, {6} XML sin comentarios).\x1b[0m",
                Carpeta, NombreDelMod, Hay, Peso / 1048576.0, Escritos, Borrados, SinComentarios);

            // La copia sirve para jugar, pero no para subirla: por eso -copia, que corre antes de
            // subir al Workshop, sale con 1.
            if (EnUso.Count > 0)
            {
                Console.WriteLine("\e[93mQuedo la version anterior de {0} archivos porque otro programa los tiene abiertos (probablemente el juego). " +
                                  "Cierra el juego y vuelve a armar la copia:\x1b[0m", EnUso.Count);
                foreach (string Archivo in EnUso)
                    Console.WriteLine("\t" + Archivo);
                return false;
            }

            // Un XML que no se pudo aligerar igual va, con sus comentarios: no es un error.
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("\e[93mNo se pudo armar la copia en {0} ({1}).\x1b[0m", Carpeta, e.Message);
            return false;
        }
    }

    /** El mismo XML sin comentarios, con la misma codificacion y BOM. Null si el resultado no
     *  queda como XML valido, para que se copie el original y no uno roto. */
    private static byte[]? SacarComentarios(byte[] Contenido)
    {
        bool ConBom = Contenido.AsSpan().StartsWith(Encoding.UTF8.Preamble);
        string Texto = new UTF8Encoding(false).GetString(Contenido, ConBom ? 3 : 0, Contenido.Length - (ConBom ? 3 : 0));

        if (!Texto.Contains("<!--"))
            return Contenido;

        string Resultado = Comentario.Replace(ComentarioEnSuLinea.Replace(Texto, ""), "");

        try
        {
            XDocument.Parse(Resultado);
        }
        catch (Exception)
        {
            return null;
        }

        return [.. (ConBom ? Encoding.UTF8.Preamble : []), .. new UTF8Encoding(false).GetBytes(Resultado)];
    }
}
