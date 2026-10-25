using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RmlVentana.Logica;

namespace RmlVentana.Tests
{
    [TestClass]
    public class ClasificadorTests
    {
        private const string Rml = "lordmordelon.rml";

        private static ModActivo Mod(string Id, bool Espanol = false) =>
            new ModActivo { PackageId = Id, Nombre = Id, WorkshopId = "1", TieneEspanol = Espanol };

        private static ModActivo Paquete(string Id, params string[] Traduce) =>
            new ModActivo { PackageId = Id, Nombre = "Paquete " + Id, TieneEspanol = true, EsPaqueteDeTraduccion = true, Traduce = Traduce.ToList() };

        private static Fila De(List<Fila> Filas, string Id) => Filas.Single(x => x.Mod.PackageId == Id);

        [TestMethod]
        public void SoloRmlEsTraducido()
        {
            var Filas = Clasificador.Clasificar(new[] { Mod("a") }, new HashSet<string> { "a" }, null, Rml);

            Assert.AreEqual(Estado.Traducido, De(Filas, "a").Estado);
            Assert.IsTrue(De(Filas, "a").PorRml);
        }

        [TestMethod]
        public void RmlYLaDelPropioModEsSobrescrito()
        {
            var Filas = Clasificador.Clasificar(new[] { Mod("a", Espanol: true) }, new HashSet<string> { "a" }, null, Rml);

            Assert.AreEqual(Estado.Sobrescrito, De(Filas, "a").Estado);
            Assert.IsTrue(De(Filas, "a").IncluidaEnElMod);
        }

        [TestMethod]
        public void RmlYOtroPaqueteEsSobrescritoYNombraAlOtro()
        {
            var Filas = Clasificador.Clasificar(new[] { Mod("a"), Paquete("p", "a") }, new HashSet<string> { "a" }, null, Rml);

            Assert.AreEqual(Estado.Sobrescrito, De(Filas, "a").Estado);
            CollectionAssert.AreEqual(new[] { "Paquete p" }, De(Filas, "a").Otros);
        }

        [TestMethod]
        public void SoloOtroPaqueteEsTraducidoPeroNoPorRml()
        {
            var Filas = Clasificador.Clasificar(new[] { Mod("a"), Paquete("p", "a") }, new HashSet<string>(), null, Rml);

            Assert.AreEqual(Estado.Traducido, De(Filas, "a").Estado);
            Assert.IsFalse(De(Filas, "a").PorRml);
        }

        [TestMethod]
        public void SoloLaDelPropioModEsTraducidoIncluida()
        {
            var Filas = Clasificador.Clasificar(new[] { Mod("a", Espanol: true) }, new HashSet<string>(), null, Rml);

            Assert.AreEqual(Estado.Traducido, De(Filas, "a").Estado);
            Assert.IsTrue(De(Filas, "a").IncluidaEnElMod);
        }

        [TestMethod]
        public void NadieLoTraduceEsSinTraducir()
        {
            var Filas = Clasificador.Clasificar(new[] { Mod("a") }, new HashSet<string>(), null, Rml);

            Assert.AreEqual(Estado.SinTraducir, De(Filas, "a").Estado);
        }

        [TestMethod]
        public void SinTraducirYEnSinTextoEsSinTexto()
        {
            var Sin = new ModActivo { PackageId = "a", Nombre = "a", WorkshopId = "123" };
            var Filas = Clasificador.Clasificar(new[] { Sin }, new HashSet<string>(), null, Rml, new HashSet<string> { "123" });

            Assert.AreEqual(Estado.SinTexto, De(Filas, "a").Estado);
        }

        [TestMethod]
        public void SinTextoNoTapaUnaTraduccion()
        {
            // Si el mod agrego texto despues y alguien lo tradujo, lo que vale es la traduccion.
            var PorRml = new ModActivo { PackageId = "a", Nombre = "a", WorkshopId = "1" };
            var PorElMod = new ModActivo { PackageId = "b", Nombre = "b", WorkshopId = "2", TieneEspanol = true };
            var Filas = Clasificador.Clasificar(new[] { PorRml, PorElMod }, new HashSet<string> { "a" }, null, Rml,
                new HashSet<string> { "1", "2" });

            Assert.AreEqual(Estado.Traducido, De(Filas, "a").Estado);
            Assert.AreEqual(Estado.Traducido, De(Filas, "b").Estado);
        }

        [TestMethod]
        public void SinTextoVaPorElNumeroDelWorkshop()
        {
            // The Dead Man's Switch y su II comparten packageId: el numero es lo que los separa.
            var Uno = new ModActivo { PackageId = "aoba.dms", Nombre = "Uno", WorkshopId = "3121742525" };
            var Dos = new ModActivo { PackageId = "aoba.dms", Nombre = "Dos", WorkshopId = "3819799478" };
            var Filas = Clasificador.Clasificar(new[] { Uno, Dos }, new HashSet<string>(), null, Rml,
                new HashSet<string> { "3121742525" });

            Assert.AreEqual(Estado.SinTexto, Filas.Single(x => x.Mod.Nombre == "Uno").Estado);
            Assert.AreEqual(Estado.SinTraducir, Filas.Single(x => x.Mod.Nombre == "Dos").Estado);
        }

        [TestMethod]
        public void LeeLosNumerosDeSinTexto()
        {
            string Tsv = "WorkshopID\tPackageID\tMod\tFecha\r\n" +
                         "3584213645\tnanoyss.simplesmokece\tSimple Smoke for Combat Extended\t2026-10-23\r\n" +
                         "no es numero\tx\ty\t2026-10-23\n" +
                         "\n";

            CollectionAssert.AreEquivalent(new[] { "3584213645" }, Clasificador.LeerSinTexto(Tsv).ToList());
            Assert.AreEqual(0, Clasificador.LeerSinTexto(null).Count);
        }

        [TestMethod]
        public void QuedanAfueraLosOficialesRmlYLosPaquetes()
        {
            var Core = new ModActivo { PackageId = "ludeon.rimworld", Nombre = "Core", Oficial = true };
            var Filas = Clasificador.Clasificar(new[] { Core, Mod(Rml), Mod("a"), Paquete("p", "a") }, new HashSet<string>(), null, Rml);

            CollectionAssert.AreEqual(new[] { "a" }, Filas.Select(x => x.Mod.PackageId).ToList());
        }

        [TestMethod]
        public void UnPaqueteQueRmlTraduceSiSaleEnLaTabla()
        {
            // Un mod de solo Patches con su propia traduccion parece un paquete de traduccion.
            var Filas = Clasificador.Clasificar(new[] { Paquete("p") }, new HashSet<string> { "p" }, null, Rml);

            Assert.AreEqual(Estado.Sobrescrito, De(Filas, "p").Estado);
        }

        [TestMethod]
        public void RespetaElOrdenDeLaListaDeMods()
        {
            var Filas = Clasificador.Clasificar(new[] { Mod("c"), Mod("a"), Mod("b") }, new HashSet<string>(), null, Rml);

            CollectionAssert.AreEqual(new[] { "c", "a", "b" }, Filas.Select(x => x.Mod.PackageId).ToList());
        }

        [TestMethod]
        public void NuevoEsElQueNoEstabaLaUltimaVez()
        {
            var Filas = Clasificador.Clasificar(new[] { Mod("a"), Mod("b") }, new HashSet<string>(), new HashSet<string> { "a" }, Rml);

            Assert.IsFalse(De(Filas, "a").Nuevo);
            Assert.IsTrue(De(Filas, "b").Nuevo);
        }

        [TestMethod]
        public void LaPrimeraVezNingunoEsNuevo()
        {
            var Filas = Clasificador.Clasificar(new[] { Mod("a") }, new HashSet<string>(), null, Rml);

            Assert.IsFalse(De(Filas, "a").Nuevo);
        }

        [TestMethod]
        [DataRow("SpanishLatin", true)]
        [DataRow("SpanishLatin (Español(Latinoamérica))", true)]
        [DataRow("Spanish (Español(Castellano))", false)]
        [DataRow("English", false)]
        [DataRow(null, false)]
        public void ReconoceElEspanolLatinoPorSusDosNombres(string Carpeta, bool Esperado)
        {
            Assert.AreEqual(Esperado, Clasificador.EsEspanolLatino(Carpeta));
        }

        private static Novedad Cambiado(string PackageId) =>
            new Novedad { PackageId = PackageId, Nombre = PackageId, Cambio = Cambio.Corregido, Fecha = "2026-10-25" };

        private static readonly HashSet<string> Activos = new HashSet<string> { "a", "b" };

        [TestMethod]
        public void ApareceSolaConUnCambioSinVerEnUnModActivo()
        {
            Assert.IsTrue(Clasificador.DebeAparecerSola(false, true, new[] { Cambiado("a") }, false, Activos));
        }

        [TestMethod]
        public void UnModNuevoEnLaListaNoLaHaceAparecer()
        {
            // Angel, 25-oct: activo un mod, el juego se reinicio y la ventana salio con «Novedades» vacia.
            // El mod nuevo se marca en «Tus mods», pero no la abre.
            Assert.IsFalse(Clasificador.DebeAparecerSola(false, true, new Novedad[0], false, Activos));
        }

        [TestMethod]
        public void UnCambioEnUnModQueNoEstaActivoNoLaHaceAparecer()
        {
            // Angel, 25-oct: sale en la tabla, pero no la abre.
            Assert.IsFalse(Clasificador.DebeAparecerSola(false, true, new[] { Cambiado("otro") }, false, Activos));
            Assert.IsTrue(Clasificador.DebeAparecerSola(false, true, new[] { Cambiado("otro"), Cambiado("b") }, false, Activos));
        }

        [TestMethod]
        public void LoYaVistoNoLaHaceAparecer()
        {
            Assert.IsFalse(Clasificador.DebeAparecerSola(false, true, new[] { Cambiado("a") }, true, Activos));
        }

        [TestMethod]
        public void LaTraduccionOficialLaHaceAparecerSiempre()
        {
            // Despues de instalarla o deshacerla y reiniciar. No es de ningun mod.
            var Oficial = new Novedad { PackageId = "", Nombre = "Core", Oficial = true, Fecha = "2026-10-25" };

            Assert.IsTrue(Clasificador.DebeAparecerSola(false, true, new[] { Oficial }, true, Activos));
        }

        [TestMethod]
        public void NoApareceEnOtroIdiomaNiConNoVolverAMostrar()
        {
            Assert.IsFalse(Clasificador.DebeAparecerSola(false, false, new[] { Cambiado("a") }, false, Activos));
            Assert.IsFalse(Clasificador.DebeAparecerSola(true, true, new[] { Cambiado("a") }, false, Activos));
        }
    }

    [TestClass]
    public class SolicitudesTests
    {
        [TestMethod]
        public void ElEnlaceLlevaLaLineaQueLeeElAction()
        {
            string Enlace = Solicitudes.Enlace("Rimefeller & Co #2", "1321849735", "dubwise.rimefeller");

            StringAssert.StartsWith(Enlace, "https://github.com/LordMordelon/RML/issues/new?");
            var Partes = new Uri(Enlace).Query.TrimStart('?').Split('&')
                .Select(x => x.Split('='))
                .ToDictionary(x => x[0], x => Uri.UnescapeDataString(x[1]));

            Assert.AreEqual("solicitud", Partes["labels"]);
            StringAssert.Contains(Partes["title"], "Rimefeller & Co #2");
            StringAssert.Contains(Partes["body"], "\n- 1321849735 dubwise.rimefeller\n");
        }

        [TestMethod]
        public void SeBorranLasDeModsQueYaTraduceRml()
        {
            var Guardadas = new Dictionary<string, string> { ["a"] = "2026-10-18", ["b"] = "2026-10-20" };

            Solicitudes.Limpiar(Guardadas, new HashSet<string> { "a" });

            CollectionAssert.AreEqual(new[] { "b" }, Guardadas.Keys.ToList());
        }

        [TestMethod]
        public void FechaCortaConLosMesesDelIdioma()
        {
            string[] Meses = "ene,feb,mar,abr,may,jun,jul,ago,sep,oct,nov,dic".Split(',');

            Assert.AreEqual("18-oct", Solicitudes.FechaCorta("2026-10-18", Meses, "{0}-{1}"));
            Assert.AreEqual("Oct 3", Solicitudes.FechaCorta("2026-10-03", "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec".Split(','), "{1} {0}"));
            Assert.AreEqual("", Solicitudes.FechaCorta("no es fecha", Meses, "{0}-{1}"));
            Assert.AreEqual("18-oct-2026", Solicitudes.FechaCorta("2026-10-18", Meses, "{0}-{1}-{2}"));
            // El mes tambien como numero (Angel prefiere 25-10-2026), con el formato de dos cifras.
            Assert.AreEqual("05-10-2026", Solicitudes.FechaCorta("2026-10-05", Meses, "{0:00}-{3:00}-{2}"));
        }
    }
}
