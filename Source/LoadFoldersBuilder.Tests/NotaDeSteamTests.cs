using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cambio = LoadFoldersBuilder.NotaDeSteam.Cambio;

namespace LoadFoldersBuilder.Tests;

[TestClass]
public class NotaDeSteamTests
{
    // 28-oct-2026 03:31 en Chile (UTC-3 en octubre)
    private static readonly DateTimeOffset Subida = new(2026, 10, 28, 6, 31, 0, TimeSpan.Zero);

    private static readonly Dictionary<string, string> Nombres = new()
    {
        ["111"] = "Mod Viejo",
        ["222"] = "Mod Nuevo",
        ["333"] = "Otro Viejo",
    };

    private static string Armar(IEnumerable<Cambio> Cambios) =>
        NotaDeSteam.Armar(Subida, ["Mod Viejo - 111", "Otro Viejo - 333"], Cambios, Nombres, 283);

    private static Cambio En(string MomentoUtc, string Carpeta) => new(DateTimeOffset.Parse(MomentoUtc + "Z"), Carpeta);

    /** El ejemplo de Angel: subidas a Steam el 23 y el 25, a GitHub el 23, 24 y 25. La del 25 trae lo del 24
     *  y lo del 25, cada uno en su dia. */
    [TestMethod]
    public void CadaDiaDesdeLaSubidaVaConSusModsNuevos()
    {
        string Nota = Armar([
            En("2026-10-29T15:00:00", "Mod Viejo - 111"),
            En("2026-10-29T16:00:00", "Mod Nuevo - 222"),
            En("2026-10-30T15:00:00", "Otro Viejo - 333"),
        ]);

        StringAssert.Contains(Nota, "Cambios desde la subida anterior al Workshop (28-oct-2026, 03:31, hora de Chile)");
        StringAssert.Contains(Nota, "30-oct-2026\n1 mod actualizado");
        StringAssert.Contains(Nota, "29-oct-2026\nMods nuevos (1)\n- Mod Nuevo\n1 mod actualizado");
        Assert.IsTrue(Nota.IndexOf("30-oct-2026") < Nota.IndexOf("29-oct-2026"), "lo mas nuevo arriba");
        StringAssert.Contains(Nota, "283 mods traducidos");
    }

    /** El dia es el de Chile, no el de UTC: a las 23:00 del 24 en Chile ya es el 25 en UTC. */
    [TestMethod]
    public void ElDiaSaleEnHoraDeChile()
    {
        string Nota = Armar([En("2026-10-30T02:00:00", "Mod Nuevo - 222")]);

        StringAssert.Contains(Nota, "29-oct-2026\nMods nuevos (1)");
        Assert.IsFalse(Nota.Contains("30-oct-2026"));
    }

    /** Un mod nuevo es nuevo entero para quien baja la subida: sus cambios de los dias siguientes no lo
     *  vuelven «actualizado». */
    [TestMethod]
    public void UnModNuevoNoCuentaComoActualizadoDespues()
    {
        string Nota = Armar([
            En("2026-10-29T15:00:00", "Mod Nuevo - 222"),
            En("2026-10-30T15:00:00", "Mod Nuevo - 222"),
        ]);

        StringAssert.Contains(Nota, "29-oct-2026\nMods nuevos (1)\n- Mod Nuevo");
        Assert.IsFalse(Nota.Contains("30-oct-2026"));
        Assert.IsFalse(Nota.Contains("actualizado"));
    }

    /** Los actualizados se cuentan, sin la lista: Angel pidio solo los añadidos. Un mod tocado varias veces
     *  el mismo dia cuenta una. */
    [TestMethod]
    public void LosActualizadosSoloSeCuentan()
    {
        string Nota = Armar([
            En("2026-10-29T15:00:00", "Mod Viejo - 111"),
            En("2026-10-29T17:00:00", "Mod Viejo - 111"),
            En("2026-10-29T18:00:00", "Otro Viejo - 333"),
        ]);

        StringAssert.Contains(Nota, "29-oct-2026\n2 mods actualizados");
        Assert.IsFalse(Nota.Contains("Otro Viejo"));
    }

    /** Sin nombre en ModList.tsv, el de la carpeta sin el numero. */
    [TestMethod]
    public void SinNombreVaElDeLaCarpeta()
    {
        string Nota = Armar([En("2026-10-29T15:00:00", "Recien Llegado - 999")]);

        StringAssert.Contains(Nota, "- Recien Llegado\n");
    }

    [TestMethod]
    public void SinCambiosLoDice()
    {
        StringAssert.Contains(Armar([]), "Sin cambios en las traducciones desde la subida anterior.");
    }
}
