using System.Text.RegularExpressions;

namespace LoadFoldersBuilder;

/** Novedades.tsv: lo que alimenta la pestaña «Novedades» de la ventana del juego (Source/RmlVentana).
 *  No se commitea: va solo en la copia de output/, y la arma CopiaLimpia.
 *
 *  Una fila por mod de Data/: cuando aparecio su traduccion y cuando cambio por ultima vez, con el
 *  tipo de ese ultimo cambio. «actualizado» si llego despues de una actualizacion del mod en Steam
 *  (la del cambio anterior era mas vieja que esa actualizacion); si no, «corregido». Steam solo dice
 *  la fecha de la ultima actualizacion, asi que el tipo se sabe solo del ultimo cambio, y es
 *  aproximado: un cambio por otro motivo despues de una actualizacion tambien cuenta como
 *  «actualizado».
 *
 *  El nombre y la fecha de Steam salen de ModList.tsv, que el -build acaba de escribir. El juego
 *  cruza las filas con los mods por su packageId.
 */
public static class Novedades
{
    public const string Archivo = "Novedades.tsv";

    private const string Encabezado = "PackageID\tMod\tAñadido\tÚltimo cambio\tTipo";

    private static readonly Regex IdAlFinal = new(@" - (\d+)$", RegexOptions.Compiled);

    /** El contenido del archivo, o null si no hay ModList.tsv o historial de git para armarlo. */
    public static string? Armar(string RootPath)
    {
        string Lista = Path.Combine(RootPath, "ModList.tsv");
        string Data = Path.Combine(RootPath, "Data");
        if (!File.Exists(Lista) || !Directory.Exists(Data))
            return null;

        // WorkshopID -> (packageId, nombre, fecha de Steam).
        var Mods = new Dictionary<string, (string PackageId, string Nombre, string Steam)>();
        foreach (string Linea in File.ReadLines(Lista).Skip(1))
        {
            string[] Campos = Linea.Split('\t');
            if (Campos.Length >= 5 && Campos[0].All(char.IsDigit) && Campos[0].Length > 0)
                Mods[Campos[0]] = (Campos[3], Campos[1], Campos[4]);
        }

        // Las carpetas de mod de Data/ son las que tienen su yaml, en cualquier nivel.
        var Carpetas = Directory.EnumerateFiles(Data, Statics.BuildYamlFileName, SearchOption.AllDirectories)
            .Select(Yaml => Path.GetRelativePath(RootPath, Path.GetDirectoryName(Yaml)!).Replace('\\', '/'))
            .ToList();

        var Historial = UltimosCambios.Historial(RootPath, Carpetas);
        if (Historial.Count == 0)
            return null;

        var Filas = new List<string> { Encabezado };
        foreach (string Carpeta in Carpetas.OrderBy(x => x, StringComparer.Ordinal))
        {
            if (!Historial.TryGetValue(Carpeta, out var Fechas) || Fechas.Count == 0) continue;
            if (IdAlFinal.Match(Carpeta) is not { Success: true } Id) continue;
            if (!Mods.TryGetValue(Id.Groups[1].Value, out var Mod) || Mod.PackageId.Length == 0) continue;

            string Anadido = Fechas[^1];
            string Ultimo = Fechas[0];
            string Cambio = Fechas.Count < 2 ? "" : TipoDeCambio(Fechas[1], Mod.Steam, Ultimo);

            Filas.Add($"{Mod.PackageId}\t{Mod.Nombre}\t{Anadido}\t{Ultimo}\t{Cambio}");
        }

        return string.Join("\n", Filas) + "\n";
    }

    /** Las fechas van como yyyy-MM-dd, asi que se comparan como texto. */
    private static string TipoDeCambio(string Anterior, string Steam, string Ultimo) =>
        Steam.Length == 10 && string.CompareOrdinal(Anterior, Steam) < 0 && string.CompareOrdinal(Steam, Ultimo) <= 0
            ? "actualizado"
            : "corregido";
}
