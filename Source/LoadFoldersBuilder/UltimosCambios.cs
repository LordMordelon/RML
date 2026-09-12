using System.Diagnostics;
using System.Text;

namespace LoadFoldersBuilder;

/** Fecha del ultimo cambio de cada mod, sacada del historial de git.
 *  Solo sirve para la lista de mods (ModList.md y .tsv): no interviene en la carga.
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
        // una carpeta es la de su ultimo cambio. --no-renames porque detectar renombres sobre
        // miles de archivos es lento y no aporta: el nombre viejo no es una carpeta actual.
        var Historial = Git(RootPath, "-c", "core.quotepath=off", "log", "--no-renames", "--relative",
            "--format=%x01%as", "--name-only", "--", "Data");
        if (Historial is null)
        {
            Console.WriteLine("\e[93mNo se pudo leer el historial de git: la lista de mods queda sin fecha de traduccion.\x1b[0m");
            return Fechas;
        }

        string? FechaDelCommit = null;
        foreach (var Linea in Historial)
        {
            if (Linea.StartsWith(''))
            {
                FechaDelCommit = Linea[1..];
                continue;
            }

            if (FechaDelCommit is not null && CarpetaDe(Linea, Carpetas) is { } Carpeta)
                Fechas.TryAdd(Carpeta, FechaDelCommit);
        }

        // Lo que todavia no esta commiteado cuenta como cambiado hoy. El extractor regenera la
        // lista antes del commit; sin esto quedaria con la fecha anterior, la CI la corregiria
        // despues del push y habria un commit del bot cada vez.
        var Hoy = DateTime.Now.ToString("yyyy-MM-dd");
        var Pendientes = (Git(RootPath, "-c", "core.quotepath=off", "diff", "--name-only", "--relative", "HEAD", "--", "Data") ?? [])
            .Concat(Git(RootPath, "-c", "core.quotepath=off", "ls-files", "--others", "--exclude-standard", "--", "Data") ?? []);
        foreach (var Archivo in Pendientes)
        {
            if (CarpetaDe(Archivo, Carpetas) is { } Carpeta)
                Fechas[Carpeta] = Hoy;
        }

        return Fechas;
    }

    /** La carpeta de mod que contiene a este archivo, o null si no esta dentro de ninguna. */
    private static string? CarpetaDe(string Archivo, HashSet<string> Carpetas)
    {
        for (int i = Archivo.LastIndexOf('/'); i > 0; i = Archivo.LastIndexOf('/', i - 1))
        {
            if (Carpetas.Contains(Archivo[..i]))
                return Archivo[..i];
        }

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

            // stderr no interesa (git avisa ahi del limite de renombres), pero hay que
            // vaciarlo: si se llena el buffer, git se queda esperando y esto no termina nunca.
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
