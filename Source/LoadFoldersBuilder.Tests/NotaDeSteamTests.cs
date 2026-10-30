using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFoldersBuilder.Tests;

[TestClass]
public class NotaDeSteamTests
{
    private static readonly Dictionary<string, string> Nombres = new()
    {
        ["111"] = "Mod Viejo",
        ["222"] = "Cinders of the Embergarden",
        ["333"] = "Ascension Megacorp",
    };

    private static string Armar(params string[] Ahora) => NotaDeSteam.Armar(["Mod Viejo - 111"], Ahora, Nombres);

    /** El formato que pidio Angel: «Añadidos:» y un nombre por renglon, en orden alfabetico, con el nombre de
     *  ModList.tsv. Lo que ya estaba en la subida anterior no va. */
    [TestMethod]
    public void ListaSoloLosAnadidosConSuNombre()
    {
        string Nota = Armar("Mod Viejo - 111", "Cinders - 222", "Ascension - 333");

        Assert.AreEqual("Añadidos:\nAscension Megacorp\nCinders of the Embergarden\n", Nota);
    }

    /** Sin nombre en ModList.tsv, el de la carpeta sin el numero. */
    [TestMethod]
    public void SinNombreVaElDeLaCarpeta()
    {
        Assert.AreEqual("Añadidos:\nRecien Llegado\n", Armar("Recien Llegado - 999"));
    }

    [TestMethod]
    public void SinAnadidosLoDice()
    {
        Assert.AreEqual(NotaDeSteam.SinAnadidos + "\n", Armar("Mod Viejo - 111"));
    }
}
