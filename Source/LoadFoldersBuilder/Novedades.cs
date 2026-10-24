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
 *
 *  La sexta columna marca lo que cambio desde la ultima subida de RML al Workshop («a» añadido, «c»
 *  cambiado): es lo que el juego muestra la primera vez. Al armar la copia para subir, esa subida es
 *  la anterior, asi que la columna dice que trae la version nueva. Sin conexion con Steam la columna
 *  no va, y el juego muestra los ultimos 30 dias.
 */
public static class Novedades
{
    public const string Archivo = "Novedades.tsv";

    private const string Encabezado = "PackageID\tMod\tAñadido\tÚltimo cambio\tTipo";

    /** RML en el Workshop: la misma carpeta que cuenta Rutas.BaseWorkshop. */
    private const string IdDeRml = "3541009729";

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

        var EnLaUltima = CambiosDesdeLaUltimaSubida(RootPath, Carpetas, Historial);

        var Filas = new List<string> { EnLaUltima is null ? Encabezado : Encabezado + "\tÚltima actualización de RML" };
        foreach (string Carpeta in Carpetas.OrderBy(x => x, StringComparer.Ordinal))
        {
            if (!Historial.TryGetValue(Carpeta, out var Fechas) || Fechas.Count == 0) continue;
            if (IdAlFinal.Match(Carpeta) is not { Success: true } Id) continue;
            if (!Mods.TryGetValue(Id.Groups[1].Value, out var Mod) || Mod.PackageId.Length == 0) continue;

            string Anadido = Fechas[^1];
            string Ultimo = Fechas[0];
            string Cambio = Fechas.Count < 2 ? "" : TipoDeCambio(Fechas[1], Mod.Steam, Ultimo);

            string Fila = $"{Mod.PackageId}\t{Mod.Nombre}\t{Anadido}\t{Ultimo}\t{Cambio}";
            if (EnLaUltima is not null)
                Fila += "\t" + (EnLaUltima.TryGetValue(Carpeta, out string? Marca) ? Marca : "");
            Filas.Add(Fila);
        }

        return string.Join("\n", Filas) + "\n";
    }

    /** Carpeta -> «a» o «c», de las que cambiaron desde la ultima subida de RML al Workshop: el diff
     *  entre el ultimo commit anterior a esa subida y lo que hay ahora, sin commitear incluido. Un
     *  movimiento del Agrupador (R100) no cuenta. Null si no se pudo saber. */
    private static Dictionary<string, string>? CambiosDesdeLaUltimaSubida(string RootPath, List<string> Carpetas,
        Dictionary<string, List<string>> Historial)
    {
        if (!ActualizacionesSteam.Momentos([IdDeRml]).TryGetValue(IdDeRml, out var Subida))
            return null;

        if (UltimosCambios.Git(RootPath, "rev-list", "-1", $"--before=@{Subida.ToUnixTimeSeconds()}", "HEAD") is not [var Base])
            return null;

        var Cambiados = UltimosCambios.Git(RootPath, "-c", "core.quotepath=off", "diff", "-M100%", "--name-status", "--relative", Base, "--", "Data");
        var Nuevos = UltimosCambios.Git(RootPath, "-c", "core.quotepath=off", "ls-files", "--others", "--exclude-standard", "--", "Data");
        if (Cambiados is null || Nuevos is null)
            return null;

        var Conjunto = new HashSet<string>(Carpetas);
        string DiaDeLaSubida = Subida.ToLocalTime().ToString("yyyy-MM-dd");
        var Resultado = new Dictionary<string, string>();

        foreach (string Linea in Cambiados.Concat(Nuevos.Select(x => "A\t" + x)))
        {
            string[] Campos = Linea.Split('\t');
            if (Campos[0] == "R100") continue;

            string Archivo = Campos[^1];
            for (int i = Archivo.LastIndexOf('/'); i > 0; i = Archivo.LastIndexOf('/', i - 1))
            {
                string Carpeta = Archivo[..i];
                if (!Conjunto.Contains(Carpeta)) continue;

                // Añadido si su traduccion aparecio despues de la subida anterior.
                bool Nueva = Historial.TryGetValue(Carpeta, out var Fechas) && string.CompareOrdinal(Fechas[^1], DiaDeLaSubida) >= 0;
                Resultado[Carpeta] = Nueva ? "a" : "c";
                break;
            }
        }

        return Resultado;
    }

    /** Las fechas van como yyyy-MM-dd, asi que se comparan como texto. */
    private static string TipoDeCambio(string Anterior, string Steam, string Ultimo) =>
        Steam.Length == 10 && string.CompareOrdinal(Anterior, Steam) < 0 && string.CompareOrdinal(Steam, Ultimo) <= 0
            ? "actualizado"
            : "corregido";
}
