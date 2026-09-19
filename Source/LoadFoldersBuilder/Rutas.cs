namespace LoadFoldersBuilder;

/** Lo que tiene que cumplir el arbol de Data/ para que el mod cargue en cualquier instalacion.
 *
 *  Rutas largas: RimWorld no abre rutas de mas de 260 caracteres. Instalado desde el Workshop
 *  con Steam en su carpeta por defecto, RML arranca en una ruta base de 74, y con eso algunos
 *  archivos de DefInjected se pasaban: la carga del idioma se cortaba y el juego quedaba en
 *  pantalla negra. A quien lo tiene en Mods\ no le pasa, porque ahi la base es mas corta, asi
 *  que probando en local no se ve nunca. Se cuenta siempre contra la base del Workshop.
 *
 *  Idioma duplicado: una carpeta de mod con Languages/SpanishLatin y tambien
 *  Languages/SpanishLatin (Español(Latinoamérica)). El juego carga las dos como el mismo idioma.
 *  Es lo que deja un extractor anterior al cambio al nombre corto si se corre sobre un RML ya
 *  migrado.
 *
 *  Nombres repetidos: dos carpetas de mod con un archivo en la misma ruta interna, por ejemplo
 *  Patches/Odyssey.xml. Las carpetas de Data/ son carpetas de un solo mod de RimWorld, y el
 *  juego las recorre deduplicando por esa ruta: carga la primera y descarta el resto sin decir
 *  nada. Nueve carpetas tenian su Patches/Odyssey.xml y ocho no llegaban al juego.
 */
public static class Rutas
{
    /** Donde queda RML instalado desde el Workshop con Steam en su carpeta por defecto. */
    public const string BaseWorkshop = @"C:\Program Files (x86)\Steam\steamapps\workshop\content\294100\3541009729\";

    /** MAX_PATH cuenta el nulo final: un archivo puede llegar a 259. */
    private const int LimiteArchivo = 260;

    /** Una carpeta tiene que dejar lugar para un nombre 8.3 adentro: puede llegar a 247. */
    private const int LimiteCarpeta = 248;

    /** Las rutas de Data/ que instaladas desde el Workshop llegan al limite, con su largo total. */
    public static List<(string Ruta, int Largo)> Largas(string RootPath)
    {
        var Resultado = new List<(string Ruta, int Largo)>();
        var Data = new DirectoryInfo(Path.Combine(RootPath, "Data"));
        if (!Data.Exists) return Resultado;

        foreach (FileSystemInfo Entrada in Data.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
        {
            string Relativa = Path.GetRelativePath(RootPath, Entrada.FullName);
            int Largo = BaseWorkshop.Length + Relativa.Length;
            int Limite = Entrada is DirectoryInfo ? LimiteCarpeta : LimiteArchivo;

            if (Largo >= Limite)
                Resultado.Add((Relativa, Largo));
        }

        return Resultado.OrderByDescending(x => x.Largo).ThenBy(x => x.Ruta, StringComparer.Ordinal).ToList();
    }

    /** Las carpetas Languages/ que tienen el mismo idioma con dos nombres, el corto y el largo. */
    public static List<string> IdiomasDuplicados(string RootPath)
    {
        var Resultado = new List<string>();
        string Data = Path.Combine(RootPath, "Data");
        if (!Directory.Exists(Data)) return Resultado;

        foreach (string Languages in Directory.EnumerateDirectories(Data, "Languages", SearchOption.AllDirectories))
        {
            // "SpanishLatin" y "SpanishLatin (Español(Latinoamérica))" comparten la primera palabra.
            var Repetidos = Directory.EnumerateDirectories(Languages)
                .GroupBy(x => Path.GetFileName(x).Split(' ')[0], StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1);

            foreach (var Grupo in Repetidos)
                Resultado.Add(Path.GetRelativePath(RootPath, Languages) + " -> " +
                              string.Join(" | ", Grupo.Select(Path.GetFileName)));
        }

        return Resultado;
    }

    /** Los archivos que dos carpetas de mod distintas tienen en la misma ruta interna.
     *
     *  Se miran solo los que estan en una subcarpeta —Patches/, Languages/…—, que son los que
     *  el juego busca por ruta relativa. Los sueltos en la raiz de cada carpeta de mod, el
     *  LoadFolders.Build.yaml y el UNUSED.xml, se llaman igual en las 262 a proposito y no los
     *  carga nadie.
     */
    public static List<string> NombresRepetidos(string RootPath)
    {
        var Resultado = new List<string>();
        string Data = Path.Combine(RootPath, "Data");
        if (!Directory.Exists(Data)) return Resultado;

        // Una carpeta de mod es una que tiene su yaml: son las mismas que salen como <li> en el
        // LoadFolders.xml, que es la lista que el juego recorre.
        var PorRutaInterna = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (string Yaml in Directory.EnumerateFiles(Data, Statics.BuildYamlFileName, SearchOption.AllDirectories))
        {
            string Carpeta = Path.GetDirectoryName(Yaml)!;
            foreach (string Archivo in Directory.EnumerateFiles(Carpeta, "*", SearchOption.AllDirectories))
            {
                string Interna = Path.GetRelativePath(Carpeta, Archivo);
                if (!Interna.Contains(Path.DirectorySeparatorChar)) continue;

                if (!PorRutaInterna.TryGetValue(Interna, out var Lista))
                    PorRutaInterna[Interna] = Lista = new List<string>();
                Lista.Add(Path.GetRelativePath(RootPath, Archivo));
            }
        }

        foreach (var Grupo in PorRutaInterna.Where(x => x.Value.Count > 1)
                     .OrderBy(x => x.Key, StringComparer.Ordinal))
            Resultado.Add(Grupo.Key + " -> " +
                          string.Join(" | ", Grupo.Value.OrderBy(x => x, StringComparer.Ordinal)));

        return Resultado;
    }

    /** Muestra lo que encuentre. Devuelve true si no hay nada que corregir. */
    public static bool Informar(string RootPath)
    {
        var ListaLargas = Largas(RootPath);
        var ListaDuplicados = IdiomasDuplicados(RootPath);
        var ListaRepetidos = NombresRepetidos(RootPath);

        if (ListaLargas.Count > 0)
        {
            Console.WriteLine("\n\e[93m{0} rutas de Data/ pasan el limite de Windows instalado desde el Workshop " +
                              "(base de {1} caracteres; archivos hasta {2}, carpetas hasta {3}).\n" +
                              "RimWorld no las puede abrir y el juego queda en pantalla negra al cargar. Hay que acortarlas.\x1b[0m",
                ListaLargas.Count, BaseWorkshop.Length, LimiteArchivo - 1, LimiteCarpeta - 1);
            foreach (var (Ruta, Largo) in ListaLargas)
                Console.WriteLine("\t{0}  {1}", Largo, Ruta);
        }

        if (ListaDuplicados.Count > 0)
        {
            Console.WriteLine("\n\e[93m{0} carpetas tienen el mismo idioma con dos nombres, y el juego carga los dos.\n" +
                              "Suele ser un extractor viejo que escribio con el nombre largo: actualizarlo y volver a traducir ese mod.\x1b[0m",
                ListaDuplicados.Count);
            foreach (string Duplicado in ListaDuplicados)
                Console.WriteLine("\t" + Duplicado);
        }

        if (ListaRepetidos.Count > 0)
        {
            Console.WriteLine("\n\e[93m{0} archivos estan en la misma ruta interna en mas de una carpeta de Data/.\n" +
                              "Todas son carpetas del mismo mod, asi que el juego carga uno solo y descarta los demas " +
                              "sin avisar: esas traducciones no llegan a la pantalla. Hay que darles nombres distintos.\x1b[0m",
                ListaRepetidos.Count);
            foreach (string Repetido in ListaRepetidos)
                Console.WriteLine("\t" + Repetido);
        }

        return ListaLargas.Count == 0 && ListaDuplicados.Count == 0 && ListaRepetidos.Count == 0;
    }
}
