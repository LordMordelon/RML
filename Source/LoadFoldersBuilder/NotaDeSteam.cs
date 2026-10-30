using System.Text.RegularExpressions;

namespace LoadFoldersBuilder;

/** La nota de cambios para la pagina del Workshop, que se pega a mano despues de subir RML desde el juego
 *  (03-subir-al-workshop.cmd la deja en el portapapeles). No va a output/ ni se commitea.
 *
 *  Lista los mods añadidos desde la ultima subida a Steam, aunque a GitHub se haya subido varias veces en
 *  el medio: subiendo a Steam el 23 y el 25 y a GitHub el 23, 24 y 25, la nota del 25 trae lo añadido el
 *  24 y el 25. Solo los añadidos, con el formato que pidio Angel: «Añadidos:» y un nombre por renglon.
 *
 *  Lo de la subida anterior es el ultimo commit antes de su hora en Steam, y lo de ahora es Data/ tal cual
 *  esta en el disco: se sube la copia del PC, asi que cuentan tambien los commits sin push y lo sin
 *  commitear.
 */
public static class NotaDeSteam
{
    public const string Archivo = "nota-de-steam.txt";

    /** Lo que dice la nota cuando no se añadio ningun mod. */
    public const string SinAnadidos = "Actualizaciones y correcciones de las traducciones.";

    /** RML en el Workshop, como en Novedades.cs. */
    private const string IdDeRml = "3541009729";

    private static readonly Regex IdAlFinal = new(@" - (\d+)$", RegexOptions.Compiled);

    /** El texto de la nota. Las carpetas son las de mod, «Nombre - ID» sin la de autor: las de la subida
     *  anterior y las de ahora. Nombres, el nombre de cada WorkshopID en ModList.tsv. */
    public static string Armar(IReadOnlyCollection<string> CarpetasEnLaSubida, IEnumerable<string> CarpetasAhora,
        IReadOnlyDictionary<string, string> Nombres)
    {
        var Antes = new HashSet<string>(CarpetasEnLaSubida);

        string NombreDe(string Carpeta)
        {
            var Id = IdAlFinal.Match(Carpeta);
            if (Id.Success && Nombres.TryGetValue(Id.Groups[1].Value, out string? Nombre))
                return Nombre;
            return Id.Success ? Carpeta[..Id.Index] : Carpeta;
        }

        var Anadidos = CarpetasAhora.Where(x => !Antes.Contains(x)).Select(NombreDe).Distinct()
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();

        return Anadidos.Count == 0 ? SinAnadidos + "\n" : "Añadidos:\n" + string.Concat(Anadidos.Select(x => x + "\n"));
    }

    /** Arma la nota del repo y la escribe en nota-de-steam.txt. False si falta Steam o git. */
    public static bool Escribir(string RootPath)
    {
        if (!ActualizacionesSteam.Momentos([IdDeRml]).TryGetValue(IdDeRml, out var Subida))
        {
            Console.WriteLine("\e[93mNo se pudo saber de Steam cuando fue la ultima subida de RML: no hay nota de cambios.\x1b[0m");
            return false;
        }

        if (UltimosCambios.Git(RootPath, "rev-list", "-1", $"--before=@{Subida.ToUnixTimeSeconds()}", "HEAD") is not [var Base]
            || UltimosCambios.Git(RootPath, "-c", "core.quotepath=off", "ls-tree", "-r", "-d", "--name-only", Base, "--", "Data") is not { } Arbol)
        {
            Console.WriteLine("\e[93mNo se pudo leer el historial de git: no hay nota de cambios.\x1b[0m");
            return false;
        }

        // Tambien salen Data/ y las carpetas de autor, que no son de ningun mod y no molestan.
        var Antes = Arbol.Select(CarpetaDe).OfType<string>().ToHashSet();

        // Las de ahora son las que tienen su yaml, en cualquier nivel.
        var Ahora = Directory.EnumerateFiles(Path.Combine(RootPath, "Data"), Statics.BuildYamlFileName, SearchOption.AllDirectories)
            .Select(x => Path.GetFileName(Path.GetDirectoryName(x)!));

        var Nombres = new Dictionary<string, string>();
        string Lista = Path.Combine(RootPath, "ModList.tsv");
        if (File.Exists(Lista))
            foreach (string[] Campos in File.ReadLines(Lista).Skip(1).Select(x => x.Split('\t')).Where(x => x.Length >= 2))
                if (Campos[0].Length > 0 && Campos[0].All(char.IsDigit))
                    Nombres[Campos[0]] = Campos[1];

        string Nota = Armar(Antes, Ahora, Nombres);
        File.WriteAllText(Path.Combine(RootPath, Archivo), Nota.Replace("\n", "\r\n"));
        Console.WriteLine("\nNota de cambios desde la subida a Steam del {0:dd-MM-yyyy HH:mm} (hora de este PC):\n\n{1}",
            Subida.ToLocalTime(), Nota);
        return true;
    }

    /** La carpeta de mod de una ruta de Data/: «Data/<mod>» o «Data/!<autor>/<mod>». */
    private static string? CarpetaDe(string Ruta)
    {
        string[] Partes = Ruta.Split('/');
        if (Partes.Length < 2 || Partes[0] != "Data") return null;
        int Mod = Partes[1].StartsWith('!') ? 2 : 1;
        return Partes.Length > Mod ? Partes[Mod] : null;
    }
}
