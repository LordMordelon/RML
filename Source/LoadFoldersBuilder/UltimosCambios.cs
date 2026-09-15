using System.Diagnostics;
using System.Text;

namespace LoadFoldersBuilder;

/** Fecha del ultimo cambio de cada mod, sacada del historial de git.
 *  Solo sirve para la lista de mods (ModList.tsv y docs/index.html): no interviene en la carga.
 */
public static class UltimosCambios
{
    /** Devuelve, por carpeta de mod (relativa a RootPath y con /), la fecha yyyy-MM-dd del
     *  ultimo commit que la toco. Nunca tira: sin git o fuera de un repo devuelve un
     *  diccionario vacio y la lista sale sin fechas, que no es motivo para no generarla.
     */
    public static Dictionary<string, string> Calcular(string RootPath, IEnumerable<string> CarpetasDeMod)
    {
        var Carpetas = new HashSet<string>(CarpetasDeMod);
        var Fechas = new Dictionary<string, string>();

        // Una sola pasada por todo el historial y no una llamada por mod, que con 250 mods
        // tarda. Va de lo mas nuevo a lo mas viejo, asi que la primera fecha que aparece para
        // una carpeta es la de su ultimo cambio.
        //
        // Un archivo que solo cambio de lugar no es una traduccion nueva. Con --no-renames lo
        // era: 8d8dbfe paso la carpeta de idioma a SpanishLatin sin tocar una linea, los 253
        // mods quedaron con la fecha de ese commit y la pagina dejo de marcar traducciones
        // atrasadas. Lo mismo pasaba cada vez que el Agrupador mueve un mod a Data/!Autor.
        // -M100% detecta solo los renombres exactos, que git encuentra comparando hashes: tarda
        // lo mismo que --no-renames y no llega al limite de renombres, que es para los parecidos.
        var Historial = Git(RootPath, "-c", "core.quotepath=off", "log", "-M100%", "--relative",
            "--format=%x01%as", "--name-status", "--", "Data");
        if (Historial is null)
        {
            Console.WriteLine("\e[93mNo se pudo leer el historial de git: la lista de mods queda sin fecha de traduccion.\x1b[0m");
            return Fechas;
        }

        // Carpeta vieja -> carpeta actual, de los mods que cambiaron de lugar: lo anterior al
        // movimiento esta en el historial con la ruta vieja y tiene que seguir contando.
        var Alias = new Dictionary<string, string>();
        // La fecha del ultimo movimiento, para un mod que no tenga ningun otro cambio.
        var Movidos = new Dictionary<string, string>();

        string? FechaDelCommit = null;
        foreach (var Linea in Historial)
        {
            if (Linea.StartsWith(''))
            {
                FechaDelCommit = Linea[1..];
                continue;
            }

            if (FechaDelCommit is null)
                continue;

            // "M<TAB>ruta", o "R100<TAB>vieja<TAB>nueva" para un renombre exacto.
            var Campos = Linea.Split('\t');
            if (Campos is ["R100", var Vieja, var Nueva])
            {
                if (CarpetaDe(Nueva, Carpetas, Alias, out var Prefijo) is not { } Movida)
                    continue;

                Movidos.TryAdd(Movida, FechaDelCommit);

                // Si la ruta vieja no cae en ninguna carpeta, lo que se movio es la carpeta del
                // mod entera: lo que sigue a la carpeta es igual en las dos rutas.
                string Resto = Nueva[Prefijo.Length..];
                if (CarpetaDe(Vieja, Carpetas, Alias, out _) is null && Vieja.EndsWith(Resto, StringComparison.Ordinal))
                    Alias.TryAdd(Vieja[..^Resto.Length], Movida);

                continue;
            }

            if (CarpetaDe(Campos[^1], Carpetas, Alias, out _) is { } Carpeta)
                Fechas.TryAdd(Carpeta, FechaDelCommit);
        }

        foreach (var (Carpeta, Fecha) in Movidos)
            Fechas.TryAdd(Carpeta, Fecha);

        // Lo que todavia no esta commiteado cuenta como cambiado hoy. El extractor regenera la
        // lista antes del commit; sin esto quedaria con la fecha anterior, la CI la corregiria
        // despues del push y habria un commit del bot cada vez. Un mod movido y sin commitear
        // tambien sale con la fecha de hoy, porque sus archivos aparecen como nuevos; despues
        // del commit vuelve a la suya.
        var Hoy = DateTime.Now.ToString("yyyy-MM-dd");
        var Pendientes = (Git(RootPath, "-c", "core.quotepath=off", "diff", "--name-only", "--relative", "HEAD", "--", "Data") ?? [])
            .Concat(Git(RootPath, "-c", "core.quotepath=off", "ls-files", "--others", "--exclude-standard", "--", "Data") ?? []);
        foreach (var Archivo in Pendientes)
        {
            if (CarpetaDe(Archivo, Carpetas, Alias, out _) is { } Carpeta)
                Fechas[Carpeta] = Hoy;
        }

        return Fechas;
    }

    /** La carpeta de mod actual que contiene a este archivo, o null si no esta dentro de
     *  ninguna. Prefijo es el tramo de la ruta que la identifico: la carpeta misma, o la ruta
     *  que tenia antes de moverse si se la encontro por un alias.
     */
    private static string? CarpetaDe(string Archivo, HashSet<string> Carpetas, Dictionary<string, string> Alias, out string Prefijo)
    {
        for (int i = Archivo.LastIndexOf('/'); i > 0; i = Archivo.LastIndexOf('/', i - 1))
        {
            Prefijo = Archivo[..i];
            if (Carpetas.Contains(Prefijo))
                return Prefijo;
            if (Alias.TryGetValue(Prefijo, out var Actual))
                return Actual;
        }

        Prefijo = "";
        return null;
    }

    /** Corre git en Directorio y devuelve las lineas de su salida, o null si fallo. */
    private static List<string>? Git(string Directorio, params string[] Argumentos)
    {
        try
        {
            var Arranque = new ProcessStartInfo("git")
            {
                WorkingDirectory = Directorio,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8
            };

            // Por lista y no como una sola cadena: las rutas pueden tener espacios.
            foreach (var Argumento in Argumentos)
                Arranque.ArgumentList.Add(Argumento);

            using var Proceso = Process.Start(Arranque);
            if (Proceso is null)
                return null;

            // stderr no interesa, pero hay que vaciarlo: si se llena el buffer, git se queda
            // esperando y esto no termina nunca.
            Proceso.ErrorDataReceived += (_, _) => { };
            Proceso.BeginErrorReadLine();

            var Lineas = new List<string>();
            while (Proceso.StandardOutput.ReadLine() is { } Linea)
            {
                if (Linea.Length > 0)
                    Lineas.Add(Linea);
            }

            Proceso.WaitForExit();
            return Proceso.ExitCode is 0 ? Lineas : null;
        }
        catch (Exception)
        {
            // Tipicamente: git no esta en el PATH.
            return null;
        }
    }
}
