using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RmlVentana.Logica;

namespace RmlVentana.Tests
{
    [TestClass]
    public class TraduccionOficialTests
    {
        private static readonly DateTime DelJuego = new DateTime(2026, 10, 17, 0, 15, 0, DateTimeKind.Utc);
        private static readonly DateTime Antes = DelJuego.AddDays(-3);
        private static readonly DateTime Despues = DelJuego.AddDays(2);

        [TestMethod]
        public void LaVersionDelReadmeEsLaPrincipal()
        {
            Assert.AreEqual("1.7", TraduccionOficial.VersionDelReadme("Traducción de RimWorld versión 1.7.4982.\nMás texto 2.0"));
        }

        [TestMethod]
        public void UnReadmeSinVersionNoDaVersion()
        {
            Assert.IsNull(TraduccionOficial.VersionDelReadme("Traducción de RimWorld"));
            Assert.IsNull(TraduccionOficial.VersionDelReadme(null));
        }

        [TestMethod]
        public void SoloSeInstalanLasCarpetasQueTieneElJuego()
        {
            var Carpetas = TraduccionOficial.CarpetasAInstalar(
                new[] { "Core", "Royalty", "biotech", "Scripts", ".github" },
                new[] { "Core", "Biotech", "Odyssey" });

            CollectionAssert.AreEqual(new[] { "Core", "Biotech" }, Carpetas);
        }

        [TestMethod]
        public void CadaCarpetaTieneElPackageIdDeSuDlc()
        {
            Assert.AreEqual("ludeon.rimworld", TraduccionOficial.PackageIdDe("Core"));
            Assert.AreEqual("ludeon.rimworld.biotech", TraduccionOficial.PackageIdDe("Biotech"));
        }

        [TestMethod]
        public void HayArreglosSiGitHubEsMasNuevoQueElJuego()
        {
            Assert.AreEqual(EstadoOficial.HayArreglos, TraduccionOficial.EstadoDe(Despues, DelJuego, null, TarPresente: true, FaltaReiniciar: false));
        }

        [TestMethod]
        public void EstaAlDiaSiElJuegoEsMasNuevoQueGitHub()
        {
            Assert.AreEqual(EstadoOficial.AlDia, TraduccionOficial.EstadoDe(Antes, DelJuego, null, TarPresente: true, FaltaReiniciar: false));
        }

        [TestMethod]
        public void LoInstaladoSeComparaContraGitHub()
        {
            Assert.AreEqual(EstadoOficial.AlDia, TraduccionOficial.EstadoDe(Despues, null, Despues, TarPresente: false, FaltaReiniciar: false));
            Assert.AreEqual(EstadoOficial.HayArreglos, TraduccionOficial.EstadoDe(Despues, null, DelJuego, TarPresente: false, FaltaReiniciar: false));
        }

        [TestMethod]
        public void SiSteamDevolvioElTarLoInstaladoQuedaSinUsar()
        {
            Assert.AreEqual(EstadoOficial.SinUsar, TraduccionOficial.EstadoDe(Despues, DelJuego, Antes, TarPresente: true, FaltaReiniciar: false));
        }

        [TestMethod]
        public void DespuesDeInstalarFaltaReiniciar()
        {
            Assert.AreEqual(EstadoOficial.FaltaReiniciar, TraduccionOficial.EstadoDe(Despues, null, Despues, TarPresente: false, FaltaReiniciar: true));
        }

        [TestMethod]
        public void SinRespuestaDeGitHubNoSeSabe()
        {
            Assert.AreEqual(EstadoOficial.SinConsulta, TraduccionOficial.EstadoDe(null, DelJuego, null, TarPresente: true, FaltaReiniciar: false));
        }

        [TestMethod]
        public void DeshacerDevuelveElTarSoloSiSteamNoPusoOtro()
        {
            Assert.AreEqual(Deshacer.DevolverElTar, TraduccionOficial.QueHaceDeshacer(TarPresente: false, RespaldoPresente: true));
            Assert.AreEqual(Deshacer.BorrarElRespaldo, TraduccionOficial.QueHaceDeshacer(TarPresente: true, RespaldoPresente: true));
            Assert.AreEqual(Deshacer.Nada, TraduccionOficial.QueHaceDeshacer(TarPresente: true, RespaldoPresente: false));
        }

        [TestMethod]
        public void SeLeeCadaClaveConSuInglesYSuTexto()
        {
            var Textos = TraduccionOficial.LeerXml(
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<LanguageData>\n" +
                "  <!-- EN: beer -->\n  <Beer.label>cerveza</Beer.label>\n" +
                "  <!-- Una nota -->\n  <Beer.description>bebida</Beer.description>\n" +
                "</LanguageData>", "DefInjected/ThingDef/");

            Assert.AreEqual("beer", Textos["DefInjected/ThingDef/Beer.label"].Ingles);
            Assert.AreEqual("cerveza", Textos["DefInjected/ThingDef/Beer.label"].Texto);
            Assert.AreEqual("", Textos["DefInjected/ThingDef/Beer.description"].Ingles);
        }

        [TestMethod]
        public void UnaListaSeLeeConUnRenglonPorElemento()
        {
            var Textos = TraduccionOficial.LeerXml(
                "<LanguageData><Regla.rulesStrings><li>r->uno</li><li>r->dos</li></Regla.rulesStrings></LanguageData>", "");

            Assert.AreEqual("r->uno\nr->dos", Textos["Regla.rulesStrings"].Texto);
        }

        [TestMethod]
        public void UnXmlRotoNoTraeNada()
        {
            Assert.AreEqual(0, TraduccionOficial.LeerXml("<LanguageData><a>", "").Count);
        }

        [TestMethod]
        public void LaComparacionSeparaCambiadosNuevosYQuitados()
        {
            var Viejo = new Dictionary<string, TextoOficial>
            {
                ["a"] = new TextoOficial("A", "uno"),
                ["b"] = new TextoOficial("B", "dos"),
                ["c"] = new TextoOficial("C", "tres"),
            };
            var Nuevo = new Dictionary<string, TextoOficial>
            {
                ["a"] = new TextoOficial("A", "uno"),
                ["b"] = new TextoOficial("B", "dos, mejor"),
                ["d"] = new TextoOficial("D", "cuatro"),
            };

            var Cambios = TraduccionOficial.Comparar(Viejo, Nuevo);

            Assert.AreEqual(3, Cambios.Count);
            Assert.IsTrue(Cambios.Any(x => x.Tipo == TipoDeTexto.Cambiado && x.Antes == "dos" && x.Ahora == "dos, mejor" && x.Ingles == "B"));
            Assert.IsTrue(Cambios.Any(x => x.Tipo == TipoDeTexto.Nuevo && x.Antes == "" && x.Ahora == "cuatro"));
            Assert.IsTrue(Cambios.Any(x => x.Tipo == TipoDeTexto.Quitado && x.Antes == "tres" && x.Ahora == ""));
        }

        [TestMethod]
        public void LasLineasSeLeenIgualQueLasDeCambiosTsv()
        {
            var Texto = new TextoCambiado { Tipo = TipoDeTexto.Cambiado, Ingles = "Two\nlines", Antes = "dos\trenglones", Ahora = "dos\nrenglones" };

            var Leidos = CambiosDeMod.Leer(new[] { TraduccionOficial.Linea("ludeon.rimworld", Texto) }, "ludeon.rimworld");

            Assert.AreEqual(1, Leidos.Count);
            Assert.AreEqual(TipoDeTexto.Cambiado, Leidos[0].Tipo);
            Assert.AreEqual("Two\nlines", Leidos[0].Ingles);
            Assert.AreEqual("dos renglones", Leidos[0].Antes);
            Assert.AreEqual("dos\nrenglones", Leidos[0].Ahora);
        }
    }
}
