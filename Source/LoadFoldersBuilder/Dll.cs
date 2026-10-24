using System.Diagnostics;

namespace LoadFoldersBuilder;

/** La DLL de RML (Source/RmlVentana), que va en Comun/Assemblies. No se commitea: se compila aca,
 *  en -build y en -copia, asi que el extractor, los .cmd y la CI la tienen sin un paso aparte.
 */
public static class Dll
{
    public const string Proyecto = "Source/RmlVentana";

    /** Compila la DLL en Comun/Assemblies. Nunca tira: devuelve false y avisa si algo salio mal. */
    public static bool Compilar(string RootPath)
    {
        try
        {
            // --disable-build-servers: sin eso MSBuild y el compilador quedan vivos en segundo
            // plano con la salida heredada, y el extractor, que lanza el builder y lee su salida
            // hasta el final, se quedaria esperando.
            var Inicio = new ProcessStartInfo("dotnet")
            {
                WorkingDirectory = RootPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            foreach (string Argumento in new[] { "build", Proyecto, "-c", "Release", "--disable-build-servers", "-nologo", "-v", "q" })
                Inicio.ArgumentList.Add(Argumento);

            using Process Proceso = Process.Start(Inicio)!;
            var Salida = Proceso.StandardOutput.ReadToEndAsync();
            var Errores = Proceso.StandardError.ReadToEndAsync();
            Proceso.WaitForExit();

            if (Proceso.ExitCode is not 0)
            {
                Console.WriteLine("\e[93mNo se pudo compilar la DLL de {0}:\x1b[0m\n{1}{2}", Proyecto, Salida.Result, Errores.Result);
                return false;
            }

            Console.WriteLine("\e[32mDLL compilada en {0}/Assemblies\x1b[0m", Statics.CarpetaComun);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("\e[93mNo se pudo compilar la DLL de {0} ({1}).\x1b[0m", Proyecto, e.Message);
            return false;
        }
    }
}
