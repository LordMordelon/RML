using LoadFoldersBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFoldersBuilder.Tests;

[TestClass]
public class CambiosDeTextoTests
{
    private const string Keyed = "Languages/SpanishLatin/Keyed/abc123.xml";

    private static string Idioma(string Cuerpo) =>
        "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<LanguageData>\n" + Cuerpo + "\n</LanguageData>\n";

    private static List<CambiosDeTexto.Cambio> Comparar(string Antes, string Ahora, string Ruta = Keyed) =>
        CambiosDeTexto.Comparar(CambiosDeTexto.Leer(Ruta, Antes), CambiosDeTexto.Leer(Ruta, Ahora));

    [TestMethod]
    public void UnaTraduccionQueCambioSaleConSuInglesYLosDosValores()
    {
        var Cambios = Comparar(
            Idioma("  <!-- EN: Cornerstone -->\n  <VFE_Piedra>Piedra base</VFE_Piedra>"),
            Idioma("  <!-- EN: Cornerstone -->\n  <VFE_Piedra>Piedra angular</VFE_Piedra>"));

        Assert.AreEqual(1, Cambios.Count);
        Assert.AreEqual(CambiosDeTexto.Tipo.Cambiado, Cambios[0].Tipo);
        Assert.AreEqual("Cornerstone", Cambios[0].Ingles);
        Assert.AreEqual("Piedra base", Cambios[0].Antes);
        Assert.AreEqual("Piedra angular", Cambios[0].Ahora);
    }

    [TestMethod]
    public void LosTextosDeUnModAnadidoVanConSuPropiaMarca()
    {
        var Cambio = new CambiosDeTexto.Cambio(CambiosDeTexto.Tipo.Anadido, "War funds", "", "Fondos secretos", Objeto: "Keyed");

        Assert.AreEqual("autor.mod\ta\tWar funds\t\tFondos secretos\t\tKeyed", CambiosDeTexto.Fila("autor.mod", Cambio));
    }

    [TestMethod]
    public void CadaCambioDiceDeQueObjetoEs()
    {
        Assert.AreEqual("ThingDef/Gun_LMG", CambiosDeTexto.ObjetoDe("1.6/Languages/SpanishLatin/DefInjected/ThingDef|Gun_LMG.description"));
        Assert.AreEqual("RulePackDef/Charla", CambiosDeTexto.ObjetoDe("Languages/SpanishLatin/DefInjected/RulePackDef|Charla.rulePack.rulesStrings[1]"));
        Assert.AreEqual("Keyed", CambiosDeTexto.ObjetoDe("Languages/SpanishLatin/Keyed|VFE_Piedra"));
        Assert.AreEqual("ThingDef/Rifle", CambiosDeTexto.ObjetoDe("Patches|/Defs/ThingDef[defName=\"Rifle\"]/tools/li[1]/label|label"));
        Assert.AreEqual("CombatExtended.AmmoDef/Ammo_556", CambiosDeTexto.ObjetoDe("Patches|Defs/CombatExtended.AmmoDef[defName='Ammo_556']/label|label"));
        Assert.AreEqual("", CambiosDeTexto.ObjetoDe("Patches|/Defs/ThingDef[@Name=\"BaseGun\"]/label|label"));
    }

    [TestMethod]
    public void SiElInglesCambioVaTambienElDeAntes()
    {
        var Cambios = Comparar(
            Idioma("  <!-- EN: Filters the air. -->\n  <A>Filtra el aire.</A>\n  <!-- EN: Same -->\n  <B>uno</B>"),
            Idioma("  <!-- EN: Filters the air and rebreathes. -->\n  <A>Filtra el aire y lo recircula.</A>\n  <!-- EN: Same -->\n  <B>dos</B>"));

        Assert.AreEqual("Filters the air.", Cambios[0].InglesAntes);
        Assert.AreEqual("", Cambios[1].InglesAntes);
        Assert.AreEqual("autor.mod\tc\tFilters the air and rebreathes.\tFiltra el aire.\tFiltra el aire y lo recircula.\tFilters the air.\tKeyed",
            CambiosDeTexto.Fila("autor.mod", Cambios[0]));
    }

    [TestMethod]
    public void UnTodoQueSeTradujoEsNuevoYUnaTraduccionQueVolvioATodoEsQuitada()
    {
        var Cambios = Comparar(
            Idioma("  <!-- EN: chief -->\n  <A>TODO</A>\n  <!-- EN: hut -->\n  <B>choza</B>"),
            Idioma("  <!-- EN: chief -->\n  <A>jefe</A>\n  <!-- EN: hut -->\n  <B>TODO</B>"));

        Assert.AreEqual(2, Cambios.Count);
        Assert.AreEqual(CambiosDeTexto.Tipo.Nuevo, Cambios[0].Tipo);
        Assert.AreEqual("", Cambios[0].Antes);
        Assert.AreEqual("jefe", Cambios[0].Ahora);
        Assert.AreEqual(CambiosDeTexto.Tipo.Quitado, Cambios[1].Tipo);
        Assert.AreEqual("hut", Cambios[1].Ingles);
        Assert.AreEqual("choza", Cambios[1].Antes);
        Assert.AreEqual("", Cambios[1].Ahora);
    }

    [TestMethod]
    public void UnaClaveQueDesaparecioEsQuitadaYLasQueNoCambiaronNoSalen()
    {
        var Cambios = Comparar(
            Idioma("  <!-- EN: one -->\n  <A>uno</A>\n  <!-- EN: two -->\n  <B>dos</B>\n  <!-- EN: three -->\n  <C>TODO</C>"),
            Idioma("  <!-- EN: one -->\n  <A>uno</A>"));

        Assert.AreEqual(1, Cambios.Count);
        Assert.AreEqual(CambiosDeTexto.Tipo.Quitado, Cambios[0].Tipo);
        Assert.AreEqual("dos", Cambios[0].Antes);
    }

    [TestMethod]
    public void VanPrimeroLosCambiadosDespuesLosNuevosYAlFinalLosQuitados()
    {
        var Cambios = Comparar(
            Idioma("<A>quitada</A>\n<B>TODO</B>\n<C>vieja</C>"),
            Idioma("<B>nueva</B>\n<C>corregida</C>"));

        CollectionAssert.AreEqual(
            new[] { CambiosDeTexto.Tipo.Cambiado, CambiosDeTexto.Tipo.Nuevo, CambiosDeTexto.Tipo.Quitado },
            Cambios.Select(x => x.Tipo).ToArray());
    }

    [TestMethod]
    public void CadaElementoDeUnaListaSeComparaAparteConSuIngles()
    {
        const string Ruta = "Languages/SpanishLatin/DefInjected/RulePackDef/x.xml";
        string Antes = Idioma("  <!-- EN:\n    <li>r_logentry->Chatted.</li>\n    <li>r_logentry->Talked.</li>\n  -->\n" +
                              "  <Charla.rulePack.rulesStrings>\n    <li>r_logentry-&gt;Charló.</li>\n    <li>r_logentry-&gt;Hablo.</li>\n  </Charla.rulePack.rulesStrings>");
        string Ahora = Antes.Replace("Hablo.", "Habló.");

        var Cambios = Comparar(Antes, Ahora, Ruta);

        Assert.AreEqual(1, Cambios.Count);
        Assert.AreEqual("r_logentry->Talked.", Cambios[0].Ingles);
        Assert.AreEqual("r_logentry->Hablo.", Cambios[0].Antes);
        Assert.AreEqual("r_logentry->Habló.", Cambios[0].Ahora);
    }

    [TestMethod]
    public void LosPatchesSeCruzanPorSuXpath()
    {
        const string Ruta = "Patches/4ljjiq.xml";
        string Patch(string Barril, string Bayoneta) =>
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<Patch>\n  <Operation Class=\"PatchOperationFindMod\">\n" +
            "    <match Class=\"PatchOperationSequence\">\n      <operations>\n" +
            "        <li Class=\"PatchOperationReplace\">\n          <xpath>/Defs/ThingDef[defName=\"Rifle\"]/tools/li[1]/label</xpath>\n" +
            $"          <value>\n            <!-- EN: barrel -->\n            <label>{Barril}</label>\n          </value>\n        </li>\n" +
            "        <li Class=\"PatchOperationReplace\">\n          <xpath>/Defs/ThingDef[defName=\"Rifle\"]/tools/li[2]/label</xpath>\n" +
            $"          <value>\n            <!-- EN: bayonet blade -->\n            <label>{Bayoneta}</label>\n          </value>\n        </li>\n" +
            "      </operations>\n    </match>\n  </Operation>\n</Patch>\n";

        var Cambios = Comparar(Patch("cañón", "hoja"), Patch("cañón", "hoja de bayoneta"), Ruta);

        Assert.AreEqual(1, Cambios.Count);
        Assert.AreEqual("bayonet blade", Cambios[0].Ingles);
        Assert.AreEqual("hoja", Cambios[0].Antes);
        Assert.AreEqual("hoja de bayoneta", Cambios[0].Ahora);
    }

    [TestMethod]
    public void LaMismaClaveEnDosTiposDeDefNoSeMezcla()
    {
        var Antes = CambiosDeTexto.Leer("Languages/SpanishLatin/DefInjected/ThingDef/a.xml", Idioma("<Foo.label>espada</Foo.label>"))
            .Concat(CambiosDeTexto.Leer("Languages/SpanishLatin/DefInjected/HediffDef/a.xml", Idioma("<Foo.label>herida</Foo.label>")));
        var Ahora = CambiosDeTexto.Leer("Languages/SpanishLatin/DefInjected/ThingDef/a.xml", Idioma("<Foo.label>espada</Foo.label>"))
            .Concat(CambiosDeTexto.Leer("Languages/SpanishLatin/DefInjected/HediffDef/a.xml", Idioma("<Foo.label>herida</Foo.label>")));

        Assert.AreEqual(0, CambiosDeTexto.Comparar(Antes, Ahora).Count);
    }

    [TestMethod]
    public void UnaTraduccionQueSoloCambioDeArchivoNoEsUnCambio()
    {
        // El extractor puede pasar una clave a otro archivo del mismo tipo: se compara por carpeta.
        var Antes = CambiosDeTexto.Leer("Languages/SpanishLatin/Keyed/viejo.xml", Idioma("<A>uno</A>"));
        var Ahora = CambiosDeTexto.Leer("Languages/SpanishLatin/Keyed/nuevo.xml", Idioma("<A>uno</A>"));

        Assert.AreEqual(0, CambiosDeTexto.Comparar(Antes, Ahora).Count);
    }

    [TestMethod]
    public void ElMismoCambioEnDosCarpetasDeVersionSaleUnaVez()
    {
        string Antes = Idioma("  <!-- EN: Infested Ship -->\n  <Nave.label>Nave infestada</Nave.label>");
        string Ahora = Antes.Replace("Nave infestada", "Nave Infestada");

        var Cambios = CambiosDeTexto.Comparar(
            CambiosDeTexto.Leer("1.6/Languages/SpanishLatin/DefInjected/ScenarioDef/a.xml", Antes)
                .Concat(CambiosDeTexto.Leer("1.7/Languages/SpanishLatin/DefInjected/ScenarioDef/a.xml", Antes)),
            CambiosDeTexto.Leer("1.6/Languages/SpanishLatin/DefInjected/ScenarioDef/a.xml", Ahora)
                .Concat(CambiosDeTexto.Leer("1.7/Languages/SpanishLatin/DefInjected/ScenarioDef/a.xml", Ahora)));

        Assert.AreEqual(1, Cambios.Count);
    }

    [TestMethod]
    public void ElInglesDelComentarioSaleConSusSimbolos()
    {
        // Toxin Warcasket: «tier&apos;s» salia tal cual en «Ver cambios».
        var Textos = CambiosDeTexto.Leer(Keyed, Idioma("  <!-- EN: the tier&apos;s &quot;vacuum&quot; &amp; &lt;b&gt;more&lt;/b&gt; -->\n  <A>x</A>"));

        Assert.AreEqual("the tier's \"vacuum\" & <b>more</b>", Textos[0].Ingles);
    }

    [TestMethod]
    public void UnInglesQueEsTodoSeTomaComoSinIngles()
    {
        // The Dead Man's Switch II trae «TODO» como descripcion en ingles de una de sus criaturas.
        var Cambios = Comparar(Idioma("  <!-- EN: TODO -->\n  <A.description>TODO</A.description>"),
            Idioma("  <!-- EN: TODO -->\n  <A.description>Un organismo militar.</A.description>"));

        Assert.AreEqual("", Cambios[0].Ingles);
    }

    [TestMethod]
    public void UnXmlRotoNoTiraYNoTraeNada()
    {
        Assert.AreEqual(0, CambiosDeTexto.Leer(Keyed, "<LanguageData><A>uno</LanguageData>").Count);
    }

    [TestMethod]
    public void LaFilaNoLlevaTabuladoresNiSaltosDentroDeLosTextos()
    {
        var Cambio = new CambiosDeTexto.Cambio(CambiosDeTexto.Tipo.Cambiado, "a\tb", "uno\ndos", "tres\r\ncuatro");

        Assert.AreEqual("Autor.Mod\tc\ta b\tuno dos\ttres cuatro\t\t", CambiosDeTexto.Fila("Autor.Mod", Cambio));
    }
}
