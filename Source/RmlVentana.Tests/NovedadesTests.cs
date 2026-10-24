using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RmlVentana.Logica;

namespace RmlVentana.Tests
{
    [TestClass]
    public class NovedadesTests
    {
        private static readonly DateTime Hoy = new DateTime(2026, 10, 23);

        private const string Tsv =
            "PackageID\tMod\tAñadido\tÚltimo cambio\tTipo\n" +
            "Autor.Nuevo\tMod nuevo\t2026-10-22\t2026-10-22\t\n" +
            "Autor.Viejo\tMod viejo\t2026-09-07\t2026-09-07\t\n" +
            "Autor.Corregido\tMod corregido\t2026-09-07\t2026-10-20\tcorregido\n" +
            "Autor.Actualizado\tMod actualizado\t2026-09-07\t2026-10-21\tactualizado\n" +
            "linea rota\n";

        private static List<EntradaDeNovedades> Entradas => NovedadesRml.Leer(Tsv);

        [TestMethod]
        public void LeeLasFilasYSaltaElEncabezadoYLasRotas()
        {
            var Leidas = Entradas;

            Assert.AreEqual(4, Leidas.Count);
            Assert.AreEqual("autor.nuevo", Leidas[0].PackageId);
            Assert.AreEqual("Mod nuevo", Leidas[0].Nombre);
            Assert.AreEqual("corregido", Leidas[2].Tipo);
        }

        [TestMethod]
        public void LaPrimeraVezMuestraLosUltimos30Dias()
        {
            var Lista = NovedadesRml.Pendientes(Entradas, null, Hoy);

            CollectionAssert.AreEqual(new[] { "autor.nuevo", "autor.actualizado", "autor.corregido" },
                Lista.Select(x => x.PackageId).ToList());
            Assert.AreEqual(Cambio.Anadido, Lista[0].Cambio);
            Assert.AreEqual(Cambio.Actualizado, Lista[1].Cambio);
            Assert.AreEqual("2026-10-21", Lista[1].Fecha);
            Assert.AreEqual(Cambio.Corregido, Lista[2].Cambio);
        }

        [TestMethod]
        public void LoVistoNoVuelveASalir()
        {
            var Vistas = NovedadesRml.Claves(Entradas);

            Assert.AreEqual(0, NovedadesRml.Pendientes(Entradas, Vistas, Hoy).Count);
        }

        [TestMethod]
        public void UnCambioNuevoDeUnModVistoSale()
        {
            var Vistas = NovedadesRml.Claves(Entradas);
            var Despues = NovedadesRml.Leer(Tsv.Replace("2026-09-07\t2026-10-20\tcorregido", "2026-09-07\t2026-10-23\tcorregido"));

            var Lista = NovedadesRml.Pendientes(Despues, Vistas, Hoy);

            Assert.AreEqual(1, Lista.Count);
            Assert.AreEqual("autor.corregido", Lista[0].PackageId);
            Assert.AreEqual("2026-10-23", Lista[0].Fecha);
        }

        [TestMethod]
        public void UnModAnadidoYCorregidoSinVerSaleComoAnadido()
        {
            var Vistas = new List<string>();
            var Leidas = NovedadesRml.Leer("PackageID\tMod\tAñadido\tÚltimo cambio\tTipo\nA.B\tMod\t2026-10-10\t2026-10-20\tcorregido\n");

            var Lista = NovedadesRml.Pendientes(Leidas, Vistas, Hoy);

            Assert.AreEqual(Cambio.Anadido, Lista.Single().Cambio);
            Assert.AreEqual("2026-10-10", Lista.Single().Fecha);
        }

        // Con la sexta columna: «a» o «c» si el mod cambio en la ultima subida de RML al Workshop.
        private const string TsvConUltima =
            "PackageID\tMod\tAñadido\tÚltimo cambio\tTipo\tÚltima actualización de RML\n" +
            "Autor.Nuevo\tMod nuevo\t2026-10-22\t2026-10-22\t\ta\n" +
            "Autor.Corregido\tMod corregido\t2026-09-07\t2026-10-20\tcorregido\t\n" +
            "Autor.Actualizado\tMod actualizado\t2026-09-07\t2026-10-21\tactualizado\tc\n";

        [TestMethod]
        public void LaPrimeraVezMuestraSoloLaUltimaActualizacionDeRml()
        {
            var Leidas = NovedadesRml.Leer(TsvConUltima);

            var Lista = NovedadesRml.Pendientes(Leidas, null, Hoy);

            Assert.IsTrue(NovedadesRml.PorUltimaActualizacion(Leidas));
            CollectionAssert.AreEqual(new[] { "autor.nuevo", "autor.actualizado" }, Lista.Select(x => x.PackageId).ToList());
            Assert.AreEqual(Cambio.Anadido, Lista[0].Cambio);
            Assert.AreEqual(Cambio.Actualizado, Lista[1].Cambio);
        }

        [TestMethod]
        public void DespuesDeLaPrimeraVezSaleTodoLoNoVisto()
        {
            var Leidas = NovedadesRml.Leer(TsvConUltima);

            // Nada visto todavia, pero ya no es la primera vez: tambien sale el corregido.
            Assert.AreEqual(3, NovedadesRml.Pendientes(Leidas, new List<string>(), Hoy).Count);
        }

        [TestMethod]
        public void SinLaSextaColumnaVuelveALos30Dias()
        {
            Assert.IsFalse(NovedadesRml.PorUltimaActualizacion(Entradas));
        }

        [TestMethod]
        public void SinArchivoNoHayNovedades()
        {
            Assert.AreEqual(0, NovedadesRml.Leer(null).Count);
            Assert.AreEqual(0, NovedadesRml.Pendientes(NovedadesRml.Leer(""), null, Hoy).Count);
        }

        [TestMethod]
        public void LasNovedadesTambienLaHacenAparecer()
        {
            var Vistos = new HashSet<string> { "a" };
            var Filas = Clasificador.Clasificar(new[] { new ModActivo { PackageId = "a", Nombre = "a" } }, new HashSet<string>(), Vistos, "rml");

            Assert.IsFalse(Clasificador.DebeAparecerSola(false, true, Vistos, Filas, HayNovedades: false));
            Assert.IsTrue(Clasificador.DebeAparecerSola(false, true, Vistos, Filas, HayNovedades: true));
            Assert.IsFalse(Clasificador.DebeAparecerSola(false, false, Vistos, Filas, HayNovedades: true));
        }
    }
}
