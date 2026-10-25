using RmlVentana.Logica;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoadFoldersBuilder.Tests;

[TestClass]
public class CambiosDeTextoTests
{
    private const string Keyed = "Languages/SpanishLatin/Keyed/abc123.xml";

    private static string Idioma(string Cuerpo) =>
        "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<LanguageData>\n" + Cuerpo + "\n</LanguageData>\n";

    private static List<TextosDeTraduccion.Cambio> Comparar(string Antes, string Ahora, string Ruta = Keyed) =>
        TextosDeTraduccion.Comparar(TextosDeTraduccion.Leer(Ruta, Antes), TextosDeTraduccion.Leer(Ruta, Ahora));

    [TestMethod]
    public void UnaTraduccionQueCambioSaleConSuInglesYLosDosValores()
    {
        var Cambios = Comparar(
            Idioma("  <!-- EN: Cornerstone -->\n  <VFE_Piedra>Piedra base</VFE_Piedra>"),
            Idioma("  <!-- EN: Cornerstone -->\n  <VFE_Piedra>Piedra angular</VFE_Piedra>"));

        Assert.AreEqual(1, Cambios.Count);
        Assert.AreEqual(TextosDeTraduccion.Tipo.Cambiado, Cambios[0].Tipo);
        Assert.AreEqual("Cornerstone", Cambios[0].Ingles);
        Assert.AreEqual("Piedra base", Cambios[0].Antes);
        Assert.AreEqual("Piedra angular", Cambios[0].Ahora);
    }

    [TestMethod]
    public void LosTextosDeUnModAnadidoVanConSuPropiaMarca()
    {
        var Cambio = new TextosDeTraduccion.Cambio(TextosDeTraduccion.Tipo.Anadido, "War funds", "", "Fondos secretos", Objeto: "Keyed");

        Assert.AreEqual("autor.mod\ta\tWar funds\t\tFondos secretos\t\tKeyed", TextosDeTraduccion.Fila("autor.mod", Cambio));
    }

    [TestMethod]
    public void CadaCambioDiceDeQueObjetoEs()
    {
        Assert.AreEqual("ThingDef/Gun_LMG", TextosDeTraduccion.ObjetoDe("1.6/Languages/SpanishLatin/DefInjected/ThingDef|Gun_LMG.description"));
        Assert.AreEqual("RulePackDef/Charla", TextosDeTraduccion.ObjetoDe("Languages/SpanishLatin/DefInjected/RulePackDef|Charla.rulePack.rulesStrings[1]"));
        Assert.AreEqual("Keyed", TextosDeTraduccion.ObjetoDe("Languages/SpanishLatin/Keyed|VFE_Piedra"));
        Assert.AreEqual("ThingDef/Rifle", TextosDeTraduccion.ObjetoDe("Patches|/Defs/ThingDef[defName=\"Rifle\"]/tools/li[1]/label|label"));
        Assert.AreEqual("CombatExtended.AmmoDef/Ammo_556", TextosDeTraduccion.ObjetoDe("Patches|Defs/CombatExtended.AmmoDef[defName='Ammo_556']/label|label"));
        Assert.AreEqual("", TextosDeTraduccion.ObjetoDe("Patches|/Defs/ThingDef[@Name=\"BaseGun\"]/label|label"));
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
            TextosDeTraduccion.Fila("autor.mod", Cambios[0]));
    }

    [TestMethod]
    public void UnTodoQueSeTradujoEsNuevoYUnaTraduccionQueVolvioATodoEsQuitada()
    {
        var Cambios = Comparar(
            Idioma("  <!-- EN: chief -->\n  <A>TODO</A>\n  <!-- EN: hut -->\n  <B>choza</B>"),
            Idioma("  <!-- EN: chief -->\n  <A>jefe</A>\n  <!-- EN: hut -->\n  <B>TODO</B>"));

        Assert.AreEqual(2, Cambios.Count);
        Assert.AreEqual(TextosDeTraduccion.Tipo.Nuevo, Cambios[0].Tipo);
        Assert.AreEqual("", Cambios[0].Antes);
        Assert.AreEqual("jefe", Cambios[0].Ahora);
        Assert.AreEqual(TextosDeTraduccion.Tipo.Quitado, Cambios[1].Tipo);
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
        Assert.AreEqual(TextosDeTraduccion.Tipo.Quitado, Cambios[0].Tipo);
        Assert.AreEqual("dos", Cambios[0].Antes);
    }

    [TestMethod]
    public void VanPrimeroLosCambiadosDespuesLosNuevosYAlFinalLosQuitados()
    {
        var Cambios = Comparar(
            Idioma("<A>quitada</A>\n<B>TODO</B>\n<C>vieja</C>"),
            Idioma("<B>nueva</B>\n<C>corregida</C>"));

        CollectionAssert.AreEqual(
            new[] { TextosDeTraduccion.Tipo.Cambiado, TextosDeTraduccion.Tipo.Nuevo, TextosDeTraduccion.Tipo.Quitado },
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
        var Antes = TextosDeTraduccion.Leer("Languages/SpanishLatin/DefInjected/ThingDef/a.xml", Idioma("<Foo.label>espada</Foo.label>"))
            .Concat(TextosDeTraduccion.Leer("Languages/SpanishLatin/DefInjected/HediffDef/a.xml", Idioma("<Foo.label>herida</Foo.label>")));
        var Ahora = TextosDeTraduccion.Leer("Languages/SpanishLatin/DefInjected/ThingDef/a.xml", Idioma("<Foo.label>espada</Foo.label>"))
            .Concat(TextosDeTraduccion.Leer("Languages/SpanishLatin/DefInjected/HediffDef/a.xml", Idioma("<Foo.label>herida</Foo.label>")));

        Assert.AreEqual(0, TextosDeTraduccion.Comparar(Antes, Ahora).Count);
    }

    [TestMethod]
    public void UnaTraduccionQueSoloCambioDeArchivoNoEsUnCambio()
    {
        // El extractor puede pasar una clave a otro archivo del mismo tipo: se compara por carpeta.
        var Antes = TextosDeTraduccion.Leer("Languages/SpanishLatin/Keyed/viejo.xml", Idioma("<A>uno</A>"));
        var Ahora = TextosDeTraduccion.Leer("Languages/SpanishLatin/Keyed/nuevo.xml", Idioma("<A>uno</A>"));

        Assert.AreEqual(0, TextosDeTraduccion.Comparar(Antes, Ahora).Count);
    }

    [TestMethod]
    public void ElMismoCambioEnDosCarpetasDeVersionSaleUnaVez()
    {
        string Antes = Idioma("  <!-- EN: Infested Ship -->\n  <Nave.label>Nave infestada</Nave.label>");
        string Ahora = Antes.Replace("Nave infestada", "Nave Infestada");

        var Cambios = TextosDeTraduccion.Comparar(
            TextosDeTraduccion.Leer("1.6/Languages/SpanishLatin/DefInjected/ScenarioDef/a.xml", Antes)
                .Concat(TextosDeTraduccion.Leer("1.7/Languages/SpanishLatin/DefInjected/ScenarioDef/a.xml", Antes)),
            TextosDeTraduccion.Leer("1.6/Languages/SpanishLatin/DefInjected/ScenarioDef/a.xml", Ahora)
                .Concat(TextosDeTraduccion.Leer("1.7/Languages/SpanishLatin/DefInjected/ScenarioDef/a.xml", Ahora)));

        Assert.AreEqual(1, Cambios.Count);
    }

    [TestMethod]
    public void ElInglesDelComentarioSaleConSusSimbolos()
    {
        // Toxin Warcasket: «tier&apos;s» salia tal cual en «Ver cambios».
        var Textos = TextosDeTraduccion.Leer(Keyed, Idioma("  <!-- EN: the tier&apos;s &quot;vacuum&quot; &amp; &lt;b&gt;more&lt;/b&gt; -->\n  <A>x</A>"));

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
        Assert.AreEqual(0, TextosDeTraduccion.Leer(Keyed, "<LanguageData><A>uno</LanguageData>").Count);
    }

    [TestMethod]
    public void LaFilaNoLlevaTabuladoresNiSaltosDentroDeLosTextos()
    {
        var Cambio = new TextosDeTraduccion.Cambio(TextosDeTraduccion.Tipo.Cambiado, "a\tb", "uno\ndos", "tres\r\ncuatro");

        Assert.AreEqual("Autor.Mod\tc\ta b\tuno dos\ttres cuatro\t\t", TextosDeTraduccion.Fila("Autor.Mod", Cambio));
    }

    [TestMethod]
    public void ElInglesVaDespuesDeSuCarpetaUnoPorLineaEnElOrdenDeLosTextos()
    {
        var Textos = TextosDeTraduccion.Leer(Keyed, Idioma("  <!-- EN: War\nfunds -->\n  <A>Fondos</A>\n  <B>sin comentario</B>"));

        CollectionAssert.AreEqual(new[] { "#\tData/!Autor/Mod - 123\t2", "War funds", "" },
            TextosDeTraduccion.LineasDeIngles("Data/!Autor/Mod - 123", Textos).ToList());
    }

    [TestMethod]
    public void ElInglesSeLeeSoloDeLasCarpetasPedidasYCompletaLosTextosDeOutput()
    {
        string Xml = Idioma("  <!-- EN: War funds -->\n  <A>Fondos</A>\n  <B>otro</B>");
        var Lineas = TextosDeTraduccion.LineasDeIngles("Data/Otro - 2", TextosDeTraduccion.Leer(Keyed, Idioma("<A>x</A>")))
            .Concat(TextosDeTraduccion.LineasDeIngles("Data/Uno - 1", TextosDeTraduccion.Leer(Keyed, Xml)))
            .Select(x => x + "\r");

        var Ingles = TextosDeTraduccion.LeerIngles(Lineas, ["Data/Uno - 1"]);

        Assert.AreEqual(1, Ingles.Count);
        // En output/ los XML no traen el comentario: el ingles sale de Ingles.tsv.
        var Textos = TextosDeTraduccion.ConIngles(TextosDeTraduccion.Leer(Keyed, Idioma("<A>Fondos</A>\n<B>otro</B>")), Ingles["Data/Uno - 1"]);
        Assert.AreEqual("War funds", Textos[0].Ingles);
        Assert.AreEqual("", Textos[1].Ingles);
    }

    [TestMethod]
    public void SiLaCuentaNoCoincideNoSeUsaElIngles()
    {
        // Un ingles corrido de lugar quedaria al lado de otro texto: mejor ninguno.
        var Ingles = TextosDeTraduccion.LeerIngles(["#\tData/Uno - 1\t3", "one", "two"], ["Data/Uno - 1"]);
        Assert.AreEqual(0, Ingles.Count);

        var Textos = TextosDeTraduccion.ConIngles(TextosDeTraduccion.Leer(Keyed, Idioma("<A>uno</A>")), ["one", "two"]);
        Assert.AreEqual("", Textos[0].Ingles);
    }
}
