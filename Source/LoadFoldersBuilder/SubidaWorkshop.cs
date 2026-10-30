using System.Diagnostics;
using System.Text;

namespace LoadFoldersBuilder;

/** Sube la copia de output/ al Workshop con SteamCMD, sin abrir el juego (PLAN-STEAMCMD.md, en la carpeta
 *  de trabajo). Lo llama 03-subir-al-workshop.cmd con -workshop, despues de rearmar la copia.
 *
 *  Es una herramienta personal: corre solo en el PC de Angel, con su sesion de Steam guardada por SteamCMD.
 *  Nunca en la CI, que tendria que guardar esa sesion como secreto de un repo publico. La ruta de SteamCMD
 *  y el usuario van en variables de entorno, para que nada de la cuenta quede escrito en el repo, y la
 *  contraseña la pide SteamCMD en su propia ventana cuando la sesion vence.
 *
 *  Antes de subir avisa los mods con cambios sin commitear, muestra la nota y pide confirmar: una subida
 *  le llega a todos los suscriptores apenas termina.
 */
public static class SubidaWorkshop
{
    /** RimWorld en Steam. */
    private const string AppId = "294100";

    /** El .vdf queda en output/ y no en la carpeta del mod: CopiaLimpia no lo borra y no se sube. */
    private const string ArchivoVdf = "workshop.vdf";

    public static bool Subir(string RootPath)
    {
        string? SteamCmd = Environment.GetEnvironmentVariable("STEAMCMD");
        string? Usuario = Environment.GetEnvironmentVariable("STEAM_USUARIO");
        if (string.IsNullOrWhiteSpace(SteamCmd) || string.IsNullOrWhiteSpace(Usuario))
        {
            Console.WriteLine("\e[93mFaltan las variables STEAMCMD y STEAM_USUARIO: se sube desde el juego (ver README).\x1b[0m");
            return false;
        }
        if (!File.Exists(SteamCmd))
        {
            Console.WriteLine("\e[93mSTEAMCMD no apunta a un archivo: {0}\x1b[0m", SteamCmd);
            return false;
        }

        // Con un numero vacio o 0, SteamCMD crearia otro item en vez de actualizar RML.
        string ArchivoId = Path.Combine(RootPath, "About", "PublishedFileId.txt");
        string? Id = File.Exists(ArchivoId) ? File.ReadAllText(ArchivoId) : null;
        if (!IdValido(Id))
        {
            Console.WriteLine("\e[93mAbout/PublishedFileId.txt no tiene el numero de RML en el Workshop: no se sube nada.\x1b[0m");
            return false;
        }

        if (NotaDeSteam.Calcular(RootPath) is not { } Nota)
            return false;

        AvisarSinCommitear(RootPath);
        Console.WriteLine("\n" + Nota);
        Console.Write("Subir a Steam? (S/N): ");
        if (!Confirma(Console.ReadLine()))
        {
            Console.WriteLine("No se subio nada.");
            return true;
        }

        string RutaVdf = Path.Combine(RootPath, CopiaLimpia.Carpeta, ArchivoVdf);
        File.WriteAllText(RutaVdf, Vdf(Id!.Trim(), Path.GetFullPath(CopiaLimpia.Ruta(RootPath)), Nota), new UTF8Encoding(false));

        // Sin redirigir la consola: si la sesion vencio, SteamCMD pide contraseña y codigo en esta ventana.
        var Arranque = new ProcessStartInfo(SteamCmd) { UseShellExecute = false };
        foreach (string Argumento in new[] { "+login", Usuario.Trim(), "+workshop_build_item", RutaVdf, "+quit" })
            Arranque.ArgumentList.Add(Argumento);

        using var Proceso = Process.Start(Arranque);
        if (Proceso is null)
        {
            Console.WriteLine("\e[93mNo se pudo abrir SteamCMD.\x1b[0m");
            return false;
        }
        Proceso.WaitForExit();

        if (Proceso.ExitCode is not 0)
        {
            Console.WriteLine("\e[93mSteamCMD termino con el codigo {0}. Revisa arriba el motivo y la pagina del Workshop.\x1b[0m", Proceso.ExitCode);
            return false;
        }

        Console.WriteLine("\e[32mSubido: https://steamcommunity.com/sharedfiles/filedetails/changelog/{0}\x1b[0m", Id.Trim());
        return true;
    }

    /** El .vdf de workshop_build_item. Solo los campos que cambian: lo que no va (titulo, descripcion,
     *  portada, visibilidad, etiquetas) queda como esta en Steam. */
    public static string Vdf(string Id, string Carpeta, string Nota) =>
        "\"workshopitem\"\n{\n" +
        $"\t\"appid\" \"{AppId}\"\n" +
        $"\t\"publishedfileid\" \"{Escapar(Id)}\"\n" +
        $"\t\"contentfolder\" \"{Escapar(Carpeta)}\"\n" +
        $"\t\"changenote\" \"{Escapar(Nota)}\"\n" +
        "}\n";

    // En un .vdf, \ y " van escapados; los saltos de linea, literales.
    private static string Escapar(string Texto) => Texto.Replace("\\", "\\\\").Replace("\"", "\\\"");

    public static bool IdValido(string? Texto) =>
        Texto?.Trim() is { Length: > 0 } Id && Id.All(char.IsAsciiDigit) && Id.TrimStart('0').Length > 0;

    // Sin la marca de orden de bytes que PowerShell antepone al pasarle texto por tuberia.
    public static bool Confirma(string? Respuesta) => Respuesta?.Trim().TrimStart('﻿') is "s" or "S";

    /** Se sube lo que hay en el disco, commiteado o no: una reextraccion a medias tambien iria. */
    private static void AvisarSinCommitear(string RootPath)
    {
        var Lineas = UltimosCambios.Git(RootPath, "-c", "core.quotepath=off", "status", "--porcelain", "-uall", "--", "Data");
        var Mods = (Lineas ?? []).Where(x => x.Length > 3)
            .Select(x => NotaDeSteam.CarpetaDe(x[3..].Split(" -> ")[^1].Trim('"')))
            .OfType<string>().Distinct().Order(StringComparer.OrdinalIgnoreCase).ToList();
        if (Mods.Count == 0)
            return;

        Console.WriteLine("\n\e[93mOjo: estos mods tienen cambios sin commitear, y se suben igual:\x1b[0m");
        foreach (string Mod in Mods)
            Console.WriteLine("  - " + Mod);
    }
}
