using System.Text.RegularExpressions;
using RmlVentana.Logica;

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
 *
 *  La septima, de los marcados «c», cuenta los textos cambiados, nuevos y quitados desde esa misma
 *  subida («3,12,1»), y el detalle va en Cambios.tsv (Logica/TextosDeTraduccion.cs de RmlVentana):
 *  es el boton «Ver cambios».
 */
public static class Novedades
{
    public const string Archivo = "Novedades.tsv";

    /** Cuantos textos por mod van a Cambios.tsv: una actualizacion grande (Hegemony) traeria miles. Los
     *  totales van igual en Novedades.tsv, para el «y N mas». */
    public const int Tope = 200;

    private const string Encabezado = "PackageID\tMod\tAñadido\tÚltimo cambio\tTipo";

    /** RML en el Workshop: la misma carpeta que cuenta Rutas.BaseWorkshop. */
    private const string IdDeRml = "3541009729";

    private static readonly Regex IdAlFinal = new(@" - (\d+)$", RegexOptions.Compiled);

    /** Lo que va en Novedades.tsv y en Cambios.tsv (null sin conexion con Steam). */
    public record Archivos(string Novedades, string? Cambios);

    /** Los dos archivos, o null si no hay ModList.tsv o historial de git para armarlos. */
    public static Archivos? Armar(string RootPath)
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

        var Subida = CambiosDesdeLaUltimaSubida(RootPath, Carpetas, Historial);

        var Filas = new List<string> { Subida is null ? Encabezado : Encabezado + "\tÚltima actualización de RML\tTextos" };
        var FilasDeCambios = new List<string> { TextosDeTraduccion.Encabezado };
        foreach (string Carpeta in Carpetas.OrderBy(x => x, StringComparer.Ordinal))
        {
            if (!Historial.TryGetValue(Carpeta, out var Fechas) || Fechas.Count == 0) continue;
            if (IdAlFinal.Match(Carpeta) is not { Success: true } Id) continue;
            if (!Mods.TryGetValue(Id.Groups[1].Value, out var Mod) || Mod.PackageId.Length == 0) continue;

            string Anadido = Fechas[^1];
            string Ultimo = Fechas[0];
            string Cambio = Fechas.Count < 2 ? "" : TipoDeCambio(Fechas[1], Mod.Steam, Ultimo);

            string Fila = $"{Mod.PackageId}\t{Mod.Nombre}\t{Anadido}\t{Ultimo}\t{Cambio}";
            if (Subida is not null)
            {
                string Marca = Subida.Marcas.TryGetValue(Carpeta, out string? M) ? M : "";
                string Cuenta = "";
                // Un mod añadido despues de la subida es nuevo entero para quien la recibe, pero quien ya
                // lo vio (el mantenedor, o quien baja las versiones de GitHub) lo ve «Corregido» si cambio
                // despues: sus cambios son los del dia de su ultimo cambio, contra como estaba antes.
                List<Tocado>? Tocados = null;
                string BaseDeCarpeta = Subida.Base;
                if (Marca == "c")
                    Subida.Archivos.TryGetValue(Carpeta, out Tocados);
                else if (Marca == "a" && Fechas.Count >= 2 && AntesDelDia(RootPath, Ultimo) is { } BaseDelDia)
                {
                    BaseDeCarpeta = BaseDelDia;
                    Tocados = ArchivosTocados(RootPath, BaseDelDia, Carpeta);
                }

                if (Tocados is not null)
                {
                    var Cambios = TextosCambiados(RootPath, BaseDeCarpeta, Tocados);
                    if (Cambios.Count > 0)
                    {
                        Cuenta = string.Join(",", Cambios.Count(x => x.Tipo == TextosDeTraduccion.Tipo.Cambiado),
                            Cambios.Count(x => x.Tipo == TextosDeTraduccion.Tipo.Nuevo), Cambios.Count(x => x.Tipo == TextosDeTraduccion.Tipo.Quitado));
                        FilasDeCambios.AddRange(Cambios.Take(Tope).Select(x => TextosDeTraduccion.Fila(Mod.PackageId, x)));
                    }
                }

                // Un mod añadido lleva ademas todos sus textos, para «Ver cambios» en su fila «Añadido».
                if (Marca == "a")
                {
                    var Todos = TextosDeLaCarpeta(RootPath, Carpeta);
                    if (Todos.Count > 0)
                    {
                        Cuenta = (Cuenta.Length > 0 ? Cuenta : "0,0,0") + "," + Todos.Count;
                        FilasDeCambios.AddRange(Todos.Take(Tope).Select(x => TextosDeTraduccion.Fila(Mod.PackageId, x)));
                    }
                }
                Fila += "\t" + Marca + "\t" + Cuenta;
            }
            Filas.Add(Fila);
        }

        return new Archivos(string.Join("\n", Filas) + "\n", Subida is null ? null : string.Join("\n", FilasDeCambios) + "\n");
    }

    /** Un archivo de una carpeta de mod que cambio desde la subida: su ruta en Base (null si es nuevo)
     *  y la de ahora (null si se borro), relativas al repo, y su ruta dentro de la carpeta del mod. */
    private record Tocado(string? Antes, string? Ahora, string Relativa);

    private record UltimaSubida(string Base, Dictionary<string, string> Marcas, Dictionary<string, List<Tocado>> Archivos);

    /** Los textos de una carpeta que cambiaron desde Base, mirando solo los XML de traduccion que git
     *  dice que se tocaron: lo de antes sale de Base y lo de ahora, del disco. */
    private static List<TextosDeTraduccion.Cambio> TextosCambiados(string RootPath, string Base, List<Tocado> Tocados)
    {
        var DeTraduccion = Tocados.Where(x => TextosDeTraduccion.EsDeTraduccion(x.Relativa)).OrderBy(x => x.Relativa, StringComparer.Ordinal).ToList();
        var Viejos = DeTraduccion.Where(x => x.Antes is not null).ToList();
        var Contenidos = UltimosCambios.Blobs(RootPath, Viejos.Select(x => Base + ":" + x.Antes).ToList());
        if (Contenidos is null)
            return [];

        var Antes = new List<TextosDeTraduccion.Texto>();
        for (int i = 0; i < Viejos.Count; i++)
            if (Contenidos[i] is { } Texto)
                Antes.AddRange(TextosDeTraduccion.Leer(Viejos[i].Relativa, Texto));

        var Ahora = new List<TextosDeTraduccion.Texto>();
        foreach (Tocado Tocado in DeTraduccion.Where(x => x.Ahora is not null))
        {
            string Archivo = Path.Combine(RootPath, Tocado.Ahora!);
            if (File.Exists(Archivo))
                Ahora.AddRange(TextosDeTraduccion.Leer(Tocado.Relativa, File.ReadAllText(Archivo)));
        }

        return TextosDeTraduccion.Comparar(Antes, Ahora);
    }

    /** Lo que cambio desde la ultima subida de RML al Workshop: el diff entre el ultimo commit anterior
     *  a esa subida y lo que hay ahora, sin commitear incluido. Marca cada carpeta con «a» o «c» y
     *  guarda sus archivos tocados. Un movimiento del Agrupador (R100) no cuenta. Null si no se pudo
     *  saber. */
    private static UltimaSubida? CambiosDesdeLaUltimaSubida(string RootPath, List<string> Carpetas,
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
        // Por su nombre, para lo que estaba en la carpeta antes de que el Agrupador la moviera a
        // Data/!Autor: «Nombre - ID» no se repite en Data/.
        var PorNombre = new Dictionary<string, string>();
        foreach (string Carpeta in Carpetas)
            PorNombre.TryAdd(Carpeta[(Carpeta.LastIndexOf('/') + 1)..], Carpeta);

        string DiaDeLaSubida = Subida.ToLocalTime().ToString("yyyy-MM-dd");
        var Marcas = new Dictionary<string, string>();
        var Archivos = new Dictionary<string, List<Tocado>>();

        foreach (string Linea in Cambiados.Concat(Nuevos.Select(x => "A\t" + x)))
        {
            string[] Campos = Linea.Split('\t');
            if (Campos[0] == "R100") continue;

            string Archivo = Campos[^1];
            if (CarpetaDe(Archivo, Conjunto, PorNombre, out string Prefijo) is not { } Carpeta)
                continue;

            // Añadido si su traduccion aparecio despues de la subida anterior.
            bool Nueva = Historial.TryGetValue(Carpeta, out var Fechas) && string.CompareOrdinal(Fechas[^1], DiaDeLaSubida) >= 0;
            Marcas[Carpeta] = Nueva ? "a" : "c";

            bool Agregado = Campos[0] == "A", Borrado = Campos[0] == "D";
            if (!Archivos.TryGetValue(Carpeta, out var Lista))
                Archivos[Carpeta] = Lista = new List<Tocado>();
            Lista.Add(new Tocado(Agregado ? null : Archivo, Borrado ? null : Archivo, Archivo[(Prefijo.Length + 1)..]));
        }

        return new UltimaSubida(Base, Marcas, Archivos);
    }

    /** Todos los textos traducidos de una carpeta de mod, como añadidos. */
    private static List<TextosDeTraduccion.Cambio> TextosDeLaCarpeta(string RootPath, string Carpeta)
    {
        string Raiz = Path.Combine(RootPath, Carpeta);
        var Ahora = new List<TextosDeTraduccion.Texto>();
        foreach (string Archivo in Directory.EnumerateFiles(Raiz, "*.xml", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.Ordinal))
        {
            string Relativa = Path.GetRelativePath(Raiz, Archivo).Replace('\\', '/');
            if (TextosDeTraduccion.EsDeTraduccion(Relativa))
                Ahora.AddRange(TextosDeTraduccion.Leer(Relativa, File.ReadAllText(Archivo)));
        }
        return TextosDeTraduccion.Comparar([], Ahora).Select(x => x.ConTipo(TextosDeTraduccion.Tipo.Anadido)).ToList();
    }

    /** El ultimo commit antes de ese dia (yyyy-MM-dd), o null si no hay. */
    private static string? AntesDelDia(string RootPath, string Dia) =>
        UltimosCambios.Git(RootPath, "rev-list", "-1", $"--before={Dia} 00:00:00", "HEAD") is [var Commit] ? Commit : null;

    /** Los archivos de una carpeta de mod que cambiaron desde Base, sin commitear incluido. Sin mirar
     *  movimientos: es para un mod añadido hace poco, que el Agrupador no movio desde entonces. */
    private static List<Tocado>? ArchivosTocados(string RootPath, string Base, string Carpeta)
    {
        var Cambiados = UltimosCambios.Git(RootPath, "-c", "core.quotepath=off", "diff", "--name-status", "--no-renames", "--relative", Base, "--", Carpeta);
        var Nuevos = UltimosCambios.Git(RootPath, "-c", "core.quotepath=off", "ls-files", "--others", "--exclude-standard", "--", Carpeta);
        if (Cambiados is null || Nuevos is null)
            return null;

        var Lista = new List<Tocado>();
        foreach (string Linea in Cambiados.Concat(Nuevos.Select(x => "A\t" + x)))
        {
            string[] Campos = Linea.Split('\t');
            string Archivo = Campos[^1];
            if (!Archivo.StartsWith(Carpeta + "/", StringComparison.Ordinal))
                continue;
            bool Agregado = Campos[0] == "A", Borrado = Campos[0] == "D";
            Lista.Add(new Tocado(Agregado ? null : Archivo, Borrado ? null : Archivo, Archivo[(Carpeta.Length + 1)..]));
        }
        return Lista;
    }

    /** La carpeta de mod actual de un archivo, por su ruta o por el nombre de la carpeta, y el tramo
     *  de la ruta que la identifico. */
    private static string? CarpetaDe(string Archivo, HashSet<string> Carpetas, Dictionary<string, string> PorNombre, out string Prefijo)
    {
        for (int i = Archivo.LastIndexOf('/'); i > 0; i = Archivo.LastIndexOf('/', i - 1))
        {
            Prefijo = Archivo[..i];
            if (Carpetas.Contains(Prefijo))
                return Prefijo;
            if (PorNombre.TryGetValue(Prefijo[(Prefijo.LastIndexOf('/') + 1)..], out string? Movida))
                return Movida;
        }

        Prefijo = "";
        return null;
    }

    /** Las fechas van como yyyy-MM-dd, asi que se comparan como texto. */
    private static string TipoDeCambio(string Anterior, string Steam, string Ultimo) =>
        Steam.Length == 10 && string.CompareOrdinal(Anterior, Steam) < 0 && string.CompareOrdinal(Steam, Ultimo) <= 0
            ? "actualizado"
            : "corregido";
}
