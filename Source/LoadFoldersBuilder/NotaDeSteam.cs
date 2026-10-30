using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LoadFoldersBuilder;

/** La nota de cambios para la pagina del Workshop, que se pega a mano despues de subir RML desde el juego
 *  (03-subir-al-workshop.cmd la deja en el portapapeles). No va a output/ ni se commitea.
 *
 *  Junta todo lo que cambio desde la ultima subida a Steam, aunque a GitHub se haya subido varias veces en
 *  el medio: subiendo a Steam el 23 y el 25 y a GitHub el 23, 24 y 25, la nota del 25 trae lo del 24 y lo
 *  del 25, cada dia por separado. Por pedido de Angel solo lista los mods nuevos; los actualizados se
 *  cuentan. Los dias van en hora de Chile, como las fechas de las releases.
 *
 *  Lo de la subida anterior es el ultimo commit antes de su hora en Steam: se sube la copia del PC, asi
 *  que se cuentan tambien los commits sin push y lo sin commitear.
 */
public static class NotaDeSteam
{
    public const string Archivo = "nota-de-steam.txt";

    /** RML en el Workshop, como en Novedades.cs. */
    private const string IdDeRml = "3541009729";

    private static readonly Regex IdAlFinal = new(@" - (\d+)$", RegexOptions.Compiled);

    private static readonly CultureInfo Espanol = CultureInfo.GetCultureInfo("es-CL");

    /** Una carpeta de mod («Nombre - ID», sin la de autor) que cambio en ese momento. */
    public record Cambio(DateTimeOffset Momento, string Carpeta);

    /** Hora de Chile, con su horario de verano. «America/Santiago» lo traduce .NET a la zona de Windows. */
    private static DateTimeOffset EnChile(DateTimeOffset Momento) =>
        TimeZoneInfo.ConvertTime(Momento, TimeZoneInfo.FindSystemTimeZoneById("America/Santiago"));

    // «28-oct-2026»: el mes abreviado sin el punto que le pone la cultura.
    private static string Dia(DateTimeOffset Momento) =>
        $"{Momento.Day:00}-{Momento.ToString("MMM", Espanol).TrimEnd('.')}-{Momento.Year}";

    /** El texto de la nota. CarpetasEnLaSubida son las carpetas de mod que ya estaban en la subida anterior;
     *  Nombres, el nombre de cada WorkshopID en ModList.tsv. */
    public static string Armar(DateTimeOffset Subida, IReadOnlyCollection<string> CarpetasEnLaSubida, IEnumerable<Cambio> Cambios,
        IReadOnlyDictionary<string, string> Nombres, int Mods)
    {
        var Antes = new HashSet<string>(CarpetasEnLaSubida);
        var EnOrden = Cambios.Select(x => (Momento: EnChile(x.Momento), x.Carpeta)).OrderBy(x => x.Momento).ToList();

        // Un mod nuevo va en el dia en que aparecio y no se cuenta como actualizado despues.
        var Nuevos = new Dictionary<string, DateTimeOffset>();
        foreach (var (Momento, Carpeta) in EnOrden)
            if (!Antes.Contains(Carpeta))
                Nuevos.TryAdd(Carpeta, Momento);

        string NombreDe(string Carpeta)
        {
            var Id = IdAlFinal.Match(Carpeta);
            if (Id.Success && Nombres.TryGetValue(Id.Groups[1].Value, out string? Nombre))
                return Nombre;
            return Id.Success ? Carpeta[..Id.Index] : Carpeta;
        }

        var Texto = new StringBuilder();
        var Hora = EnChile(Subida);
        Texto.Append($"Cambios desde la subida anterior al Workshop ({Dia(Hora)}, {Hora:HH:mm}, hora de Chile)\n");

        var Dias = EnOrden.GroupBy(x => x.Momento.Date).OrderByDescending(x => x.Key).ToList();
        if (Dias.Count == 0)
            Texto.Append("\nSin cambios en las traducciones desde la subida anterior.\n");

        foreach (var DelDia in Dias)
        {
            var NuevosDelDia = Nuevos.Where(x => x.Value.Date == DelDia.Key).Select(x => NombreDe(x.Key))
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();
            int Actualizados = DelDia.Select(x => x.Carpeta).Where(Antes.Contains).Distinct().Count();
            if (NuevosDelDia.Count == 0 && Actualizados == 0)
                continue;

            Texto.Append($"\n{Dia(DelDia.First().Momento)}\n");
            if (NuevosDelDia.Count > 0)
            {
                Texto.Append($"Mods nuevos ({NuevosDelDia.Count})\n");
                foreach (string Nombre in NuevosDelDia)
                    Texto.Append($"- {Nombre}\n");
            }
            if (Actualizados > 0)
                Texto.Append(Actualizados == 1 ? "1 mod actualizado\n" : $"{Actualizados} mods actualizados\n");
        }

        Texto.Append($"\n{Mods} mods traducidos. Lista completa: https://lordmordelon.github.io/RML/\n");
        return Texto.ToString();
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
            || UltimosCambios.Git(RootPath, "-c", "core.quotepath=off", "ls-tree", "-r", "-d", "--name-only", Base, "--", "Data") is not { } Arbol
            || UltimosCambios.Git(RootPath, "-c", "core.quotepath=off", "log", "-M100%", "--format=%x01%at",
                "--name-only", $"{Base}..HEAD", "--", "Data") is not { } Historial
            || UltimosCambios.Git(RootPath, "-c", "core.quotepath=off", "status", "--porcelain", "-uall", "--", "Data") is not { } SinCommitear)
        {
            Console.WriteLine("\e[93mNo se pudo leer el historial de git: no hay nota de cambios.\x1b[0m");
            return false;
        }

        var Cambios = new List<Cambio>();
        DateTimeOffset Momento = default;
        foreach (string Linea in Historial)
        {
            if (Linea.StartsWith('\x01'))
                Momento = DateTimeOffset.FromUnixTimeSeconds(long.Parse(Linea[1..]));
            else if (CarpetaDe(Linea) is { } Carpeta)
                Cambios.Add(new Cambio(Momento, Carpeta));
        }
        // Lo sin commitear tambien va en la subida: con la hora de ahora. «XY ruta» o «R  vieja -> nueva».
        foreach (string Linea in SinCommitear.Where(x => x.Length > 3))
            if (CarpetaDe(Linea[3..].Split(" -> ")[^1].Trim('"')) is { } Carpeta)
                Cambios.Add(new Cambio(DateTimeOffset.Now, Carpeta));

        var Antes = Arbol.Select(CarpetaDe).OfType<string>().ToHashSet();

        var Nombres = new Dictionary<string, string>();
        int Mods = 0;
        string Lista = Path.Combine(RootPath, "ModList.tsv");
        if (File.Exists(Lista))
            foreach (string[] Campos in File.ReadLines(Lista).Skip(1).Select(x => x.Split('\t')).Where(x => x.Length >= 2 && x[0].Length > 0))
            {
                Mods++;
                if (Campos[0].All(char.IsDigit))
                    Nombres[Campos[0]] = Campos[1];
            }

        // Un mod que se borro de Data/ no es un añadido: si cambio y ya no esta, no va.
        var Ahora = Directory.EnumerateFiles(Path.Combine(RootPath, "Data"), Statics.BuildYamlFileName, SearchOption.AllDirectories)
            .Select(x => Path.GetFileName(Path.GetDirectoryName(x)!)).ToHashSet();
        string Nota = Armar(Subida, Antes, Cambios.Where(x => Ahora.Contains(x.Carpeta)), Nombres, Mods);

        File.WriteAllText(Path.Combine(RootPath, Archivo), Nota.Replace("\n", "\r\n"));
        Console.WriteLine("\n" + Nota);
        return true;
    }

    /** La carpeta de mod de una ruta de Data/: «Data/<mod>/…» o «Data/!<autor>/<mod>/…». Lo suelto en Data/
     *  o en la carpeta de un autor (una nota .md de AobaKuma) tambien sale, y lo descarta Escribir: no es
     *  una carpeta con su yaml. */
    private static string? CarpetaDe(string Ruta)
    {
        string[] Partes = Ruta.Split('/');
        if (Partes.Length < 2 || Partes[0] != "Data") return null;
        int Mod = Partes[1].StartsWith('!') ? 2 : 1;
        return Partes.Length > Mod ? Partes[Mod] : null;
    }
}
