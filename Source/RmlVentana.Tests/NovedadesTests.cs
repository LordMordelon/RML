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
        public void ConTodoVistoSigueSaliendoLaUltimaActualizacion()
        {
            // Angel cerro la ventana y la volvio a abrir desde los ajustes: no tiene que quedar vacia.
            var Leidas = NovedadesRml.Leer(TsvConUltima);

            var Lista = NovedadesRml.ParaMostrar(Leidas, NovedadesRml.Claves(Leidas), null, Hoy, out bool YaVistas);

            Assert.IsTrue(YaVistas);
            CollectionAssert.AreEqual(new[] { "autor.nuevo", "autor.actualizado" }, Lista.Select(x => x.PackageId).ToList());
        }

        [TestMethod]
        public void ConTodoVistoVuelveASalirLoMismoQueSeMostro()
        {
            // Angel: si se actualizo un solo mod, al volver a abrir la ventana tiene que seguir ese solo.
            var Leidas = NovedadesRml.Leer(TsvConUltima);
            var Mostradas = NovedadesRml.Pendientes(Leidas, NovedadesRml.Claves(Leidas.Where(x => x.PackageId != "autor.corregido").ToList()), Hoy);

            var Lista = NovedadesRml.ParaMostrar(Leidas, NovedadesRml.Claves(Leidas), NovedadesRml.ClavesDe(Mostradas), Hoy, out bool YaVistas);

            Assert.IsTrue(YaVistas);
            CollectionAssert.AreEqual(new[] { "autor.corregido" }, Lista.Select(x => x.PackageId).ToList());
            Assert.AreEqual(Mostradas[0].Cambio, Lista[0].Cambio);
            Assert.AreEqual(Mostradas[0].Fecha, Lista[0].Fecha);
        }

        [TestMethod]
        public void ConTodoVistoVuelveASalirLoQueVinoDelPc()
        {
            // Lo del PC (CambiosLocales) sale «Corregido» aunque el builder no le ponga tipo, porque el mod se
            // añadio y se corrigio el mismo dia. Con Alpha Prefabs (25-oct) la pestaña volvia vacia.
            var Leidas = Entradas;
            var Mostradas = new List<Novedad> { new Novedad { PackageId = "autor.nuevo", Cambio = Cambio.Corregido, Fecha = "2026-10-22" } };

            var Lista = NovedadesRml.ParaMostrar(Leidas, NovedadesRml.Claves(Leidas), NovedadesRml.ClavesDe(Mostradas), Hoy, out bool YaVistas);

            Assert.IsTrue(YaVistas);
            CollectionAssert.AreEqual(new[] { "autor.nuevo" }, Lista.Select(x => x.PackageId).ToList());
            Assert.AreEqual(Cambio.Corregido, Lista[0].Cambio);
        }

        [TestMethod]
        public void UnModAnadidoLlevaDetalleConSusTextos()
        {
            // Cuarto numero de la septima columna: los textos del mod añadido en esa subida.
            var Leidas = NovedadesRml.Leer(
                "PackageID\tMod\tAñadido\tÚltimo cambio\tTipo\tÚltima actualización de RML\tTextos\n" +
                "Autor.Nuevo\tMod nuevo\t2026-10-22\t2026-10-22\t\ta\t0,0,0,340\n" +
                "Autor.Viejo\tMod viejo\t2026-09-07\t2026-10-20\tcorregido\tc\t3,1,0\n");

            var Lista = NovedadesRml.Pendientes(Leidas, null, Hoy);

            Assert.AreEqual(Cambio.Anadido, Lista[0].Cambio);
            Assert.IsTrue(Lista[0].TieneDetalle);
            Assert.AreEqual(340, Lista[0].Nuevos);
            Assert.AreEqual(3, Lista[1].Cambiados);
        }

        [TestMethod]
        public void ElHistorialTraeCadaModUnaVezConSuUltimoCambio()
        {
            var Lista = NovedadesRml.Historial(Entradas);

            CollectionAssert.AreEqual(new[] { "autor.nuevo", "autor.actualizado", "autor.corregido", "autor.viejo" },
                Lista.Select(x => x.PackageId).ToList());
            Assert.AreEqual(Cambio.Corregido, Lista[2].Cambio);
            Assert.AreEqual("2026-10-20", Lista[2].Fecha);
            Assert.AreEqual(Cambio.Anadido, Lista[3].Cambio);
        }

        [TestMethod]
        public void ElHistorialSeAgrupaPorFechaDeLaMasNueva()
        {
            var Grupos = NovedadesRml.PorFecha(new List<Novedad>
            {
                new Novedad { PackageId = "a", Fecha = "2026-10-24" },
                new Novedad { PackageId = "b", Fecha = "2026-10-25" },
                new Novedad { PackageId = "c", Fecha = "2026-10-24" },
            });

            CollectionAssert.AreEqual(new[] { "2026-10-25", "2026-10-24" }, Grupos.Select(x => x.Fecha).ToList());
            CollectionAssert.AreEqual(new[] { "a", "c" }, Grupos[1].Novedades.Select(x => x.PackageId).ToList());
        }

        [TestMethod]
        public void ConAlgoSinVerSaleSoloLoSinVer()
        {
            var Leidas = NovedadesRml.Leer(TsvConUltima);
            var Vistas = NovedadesRml.Claves(Leidas.Where(x => x.PackageId != "autor.corregido").ToList());

            var Lista = NovedadesRml.ParaMostrar(Leidas, Vistas, null, Hoy, out bool YaVistas);

            Assert.IsFalse(YaVistas);
            CollectionAssert.AreEqual(new[] { "autor.corregido" }, Lista.Select(x => x.PackageId).ToList());
        }

        [TestMethod]
        public void SinLaSextaColumnaVuelveALos30Dias()
        {
            Assert.IsFalse(NovedadesRml.PorUltimaActualizacion(Entradas));
        }

        // Con la septima: cuantos textos cambiaron, se agregaron y se quitaron en esa subida.
        private const string TsvConTextos =
            "PackageID\tMod\tAñadido\tÚltimo cambio\tTipo\tÚltima actualización de RML\tTextos\n" +
            "Autor.Nuevo\tMod nuevo\t2026-10-22\t2026-10-22\t\ta\t\n" +
            "Autor.Corregido\tMod corregido\t2026-09-07\t2026-10-20\tcorregido\tc\t3,12,1\n" +
            "Autor.Actualizado\tMod actualizado\t2026-09-07\t2026-10-21\tactualizado\tc\t\n";

        [TestMethod]
        public void LaSeptimaColumnaDiceCuantosTextosCambiaron()
        {
            var Lista = NovedadesRml.Pendientes(NovedadesRml.Leer(TsvConTextos), null, Hoy);

            Novedad Corregido = Lista.Single(x => x.PackageId == "autor.corregido");
            Assert.AreEqual(3, Corregido.Cambiados);
            Assert.AreEqual(12, Corregido.Nuevos);
            Assert.AreEqual(1, Corregido.Quitados);
            Assert.IsTrue(Corregido.TieneDetalle);
            Assert.IsFalse(Lista.Single(x => x.PackageId == "autor.actualizado").TieneDetalle);
            Assert.IsFalse(Lista.Single(x => x.PackageId == "autor.nuevo").TieneDetalle);
        }

        [TestMethod]
        public void UnModAnadidoNoLlevaDetalleAunqueTengaTextos()
        {
            var Leidas = NovedadesRml.Leer(TsvConTextos.Replace("Autor.Nuevo\tMod nuevo\t2026-10-22\t2026-10-22\t\ta\t\n",
                "Autor.Nuevo\tMod nuevo\t2026-10-22\t2026-10-23\tcorregido\ta\t0,40,0\n"));

            Assert.IsFalse(NovedadesRml.Pendientes(Leidas, null, Hoy).Single(x => x.PackageId == "autor.nuevo").TieneDetalle);
        }

        [TestMethod]
        public void ElDetalleSeLeeSoloDelModPedido()
        {
            string[] Lineas =
            {
                "PackageID\tTipo\tInglés\tAntes\tAhora",
                "Autor.Corregido\tc\tCornerstone\tPiedra base\tPiedra angular",
                "otro.mod\tc\tHut\tchoza\tcabaña",
                "autor.corregido\tn\tChief\t\tJefe\\n\\ntribal",
                "autor.corregido\tq\tOld\tviejo\t",
                "autor.corregido\tx\troto",
            };

            var Textos = CambiosDeMod.Leer(Lineas, "autor.corregido");

            Assert.AreEqual(3, Textos.Count);
            Assert.AreEqual(TipoDeTexto.Cambiado, Textos[0].Tipo);
            Assert.AreEqual("Cornerstone", Textos[0].Ingles);
            Assert.AreEqual("Piedra base", Textos[0].Antes);
            Assert.AreEqual("Piedra angular", Textos[0].Ahora);
            Assert.AreEqual(TipoDeTexto.Nuevo, Textos[1].Tipo);
            Assert.AreEqual("Jefe\n\ntribal", Textos[1].Ahora);
            Assert.AreEqual(TipoDeTexto.Quitado, Textos[2].Tipo);
        }

        [TestMethod]
        public void SinArchivoNoHayNovedades()
        {
            Assert.AreEqual(0, NovedadesRml.Leer(null).Count);
            Assert.AreEqual(0, NovedadesRml.Pendientes(NovedadesRml.Leer(""), null, Hoy).Count);
        }

        [TestMethod]
        public void LaPrimeraVezApareceSoloSiLaUltimaActualizacionTrajoCambios()
        {
            // Lo pidio Angel: la primera vez no basta con que sea la primera vez.
            var Leidas = NovedadesRml.Leer(TsvConUltima);
            var Activos = new HashSet<string>(Leidas.Select(x => x.PackageId));
            var Lista = NovedadesRml.ParaMostrar(Leidas, null, null, Hoy, out bool YaVistas);

            Assert.IsTrue(Clasificador.DebeAparecerSola(false, true, Lista, YaVistas, Activos));

            var SinCambios = NovedadesRml.Leer(TsvConUltima.Replace("\ta\n", "\t\n").Replace("\tc\n", "\t\n"));
            Lista = NovedadesRml.ParaMostrar(SinCambios, null, null, Hoy, out YaVistas);

            Assert.IsFalse(Clasificador.DebeAparecerSola(false, true, Lista, YaVistas, Activos));
        }
    }
}
