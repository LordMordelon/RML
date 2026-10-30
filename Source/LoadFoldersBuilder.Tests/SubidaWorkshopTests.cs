using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFoldersBuilder.Tests;

[TestClass]
public class SubidaWorkshopTests
{
    /** Solo los campos que se cambian: sin title, description, previewfile ni visibility, que SteamCMD
     *  pisaria con lo que vaya en el .vdf. */
    [TestMethod]
    public void ElVdfLlevaSoloElItemLaCarpetaYLaNota()
    {
        string Vdf = SubidaWorkshop.Vdf("3541009729", @"C:\RML\output\RimWorld Mod Latino", "Añadidos:\nUn Mod\n");

        StringAssert.Contains(Vdf, "\"appid\" \"294100\"");
        StringAssert.Contains(Vdf, "\"publishedfileid\" \"3541009729\"");
        StringAssert.Contains(Vdf, @"""contentfolder"" ""C:\\RML\\output\\RimWorld Mod Latino""");
        StringAssert.Contains(Vdf, "\"changenote\" \"Añadidos:\nUn Mod\n\"");
        foreach (string Campo in new[] { "title", "description", "previewfile", "visibility" })
            Assert.IsFalse(Vdf.Contains($"\"{Campo}\""), Campo);
    }

    /** Un nombre de mod con comillas no puede cerrar el valor antes de tiempo. */
    [TestMethod]
    public void LasComillasYBarrasSeEscapan()
    {
        string Vdf = SubidaWorkshop.Vdf("1", "C:\\x", "Añadidos:\nEl \"Mod\" \\ raro\n");

        StringAssert.Contains(Vdf, "\"changenote\" \"Añadidos:\nEl \\\"Mod\\\" \\\\ raro\n\"");
    }

    /** Con publishedfileid vacio o 0, SteamCMD crea un item nuevo en vez de actualizar RML. */
    [TestMethod]
    public void SoloAceptaUnNumeroDeItemDeVerdad()
    {
        Assert.IsTrue(SubidaWorkshop.IdValido("3541009729"));
        Assert.IsTrue(SubidaWorkshop.IdValido("3541009729\r\n"));
        Assert.IsFalse(SubidaWorkshop.IdValido(null));
        Assert.IsFalse(SubidaWorkshop.IdValido(""));
        Assert.IsFalse(SubidaWorkshop.IdValido("0"));
        Assert.IsFalse(SubidaWorkshop.IdValido("35410x"));
    }

    /** Solo un «s» explicito sube: Enter solo, otra letra o el fin de la entrada no. */
    [TestMethod]
    public void SoloSubeConUnSi()
    {
        Assert.IsTrue(SubidaWorkshop.Confirma("S"));
        Assert.IsTrue(SubidaWorkshop.Confirma(" s "));
        Assert.IsTrue(SubidaWorkshop.Confirma("﻿S"));
        Assert.IsFalse(SubidaWorkshop.Confirma(""));
        Assert.IsFalse(SubidaWorkshop.Confirma("n"));
        Assert.IsFalse(SubidaWorkshop.Confirma("si no"));
        Assert.IsFalse(SubidaWorkshop.Confirma(null));
    }
}
