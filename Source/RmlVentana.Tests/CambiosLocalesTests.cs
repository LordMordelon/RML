using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RmlVentana.Logica;
using Tipo = RmlVentana.Logica.TextosDeTraduccion.Tipo;

namespace RmlVentana.Tests
{
    /** «Ver cambios» contra la copia del PC: que filas salen en Novedades cuando el mod tiene copia
     *  (PLAN-CAMBIOS-LOCALES.md). Sin copia, todo sigue saliendo de Novedades.tsv y Cambios.tsv. */
    [TestClass]
    public class CambiosLocalesTests
    {
        private const string Archivo = "Config/RML_Cambios.tsv";
        private const string Hoy = "2026-10-25";

        private const string Tsv =
            "PackageID\tMod\tAñadido\tÚltimo cambio\tTipo\n" +
            "Autor.Nuevo\tMod nuevo\t2026-10-24\t2026-10-24\t\n" +
            "Autor.Corregido\tMod corregido\t2026-09-07\t2026-10-20\tcorregido\n" +
            "Autor.Actualizado\tMod actualizado\t2026-09-07\t2026-10-21\tactualizado\n";

        private static List<EntradaDeNovedades> Entradas => NovedadesRml.Leer(Tsv);

        private static Novedad Fila(string PackageId, Cambio Cambio, string Fecha, int Cambiados = 0) =>
            new Novedad { PackageId = PackageId, Nombre = PackageId, Cambio = Cambio, Fecha = Fecha, Cambiados = Cambiados };

        private static CambiosDeUnMod Local(string PackageId, int Cambiados, int Nuevos = 0, int Quitados = 0, string Nombre = "Nombre local") =>
            new CambiosDeUnMod { PackageId = PackageId, Nombre = Nombre, Cambiados = Cambiados, Nuevos = Nuevos, Quitados = Quitados };

        private static List<Novedad> Aplicar(List<Novedad> Pendientes, bool YaVistas, params CambiosDeUnMod[] Locales) =>
            CambiosLocales.Aplicar(Pendientes, YaVistas, Entradas, Locales.ToDictionary(x => x.PackageId), Archivo, Hoy, out _);

        [TestMethod]
        public void UnModConCopiaQueCambioSaleAunqueNovedadesNoLoDiga()
        {
            // Lo corrigieron dos veces el mismo dia: Novedades.tsv no cambio de fecha y ya estaba visto.
            var Resultado = Aplicar(new List<Novedad>(), false, Local("autor.corregido", 1, 2, 3));

            Assert.AreEqual(1, Resultado.Count);
            Assert.AreEqual(Cambio.Corregido, Resultado[0].Cambio);
            Assert.AreEqual("Mod corregido", Resultado[0].Nombre);
            Assert.AreEqual("2026-10-20", Resultado[0].Fecha);
            Assert.AreEqual(Archivo, Resultado[0].Archivo);
            Assert.AreEqual((1, 2, 3), (Resultado[0].Cambiados, Resultado[0].Nuevos, Resultado[0].Quitados));
        }

        [TestMethod]
        public void UnModConCopiaSinCambiosNoSaleAunqueElBuilderLoTenga()
        {
            var Resultado = Aplicar(new List<Novedad> { Fila("autor.actualizado", Cambio.Actualizado, "2026-10-21", 5) }, false,
                Local("autor.actualizado", 0));

            Assert.AreEqual(0, Resultado.Count);
        }

        [TestMethod]
        public void UnModSinCopiaSigueConLoDelBuilder()
        {
            var Pendientes = new List<Novedad> { Fila("autor.actualizado", Cambio.Actualizado, "2026-10-21", 5) };

            var Resultado = Aplicar(Pendientes, false, Local("autor.corregido", 0));

            Assert.AreEqual(1, Resultado.Count);
            Assert.AreEqual(5, Resultado[0].Cambiados);
            Assert.IsNull(Resultado[0].Archivo);
        }

        [TestMethod]
        public void UnAnadidoQueYaEstabaEnLaCopiaSaleComoCambioConLoDelPc()
        {
            // Quien baja de GitHub ya tenia el mod de una version anterior: para el no es nuevo.
            var Resultado = Aplicar(new List<Novedad> { Fila("autor.nuevo", Cambio.Anadido, "2026-10-24") }, false,
                Local("autor.nuevo", 2));

            Assert.AreEqual(1, Resultado.Count);
            Assert.AreEqual(Cambio.Corregido, Resultado[0].Cambio);
            Assert.AreEqual(2, Resultado[0].Cambiados);
            Assert.AreEqual(Archivo, Resultado[0].Archivo);
        }

        [TestMethod]
        public void ElTipoSaleDeNovedadesYSinFilaEsCorregidoDeHoy()
        {
            var Resultado = Aplicar(new List<Novedad>(), false, Local("autor.actualizado", 1), Local("autor.local", 1, Nombre: "Mod local"));

            var Actualizado = Resultado.Single(x => x.PackageId == "autor.actualizado");
            Assert.AreEqual(Cambio.Actualizado, Actualizado.Cambio);
            var SinFila = Resultado.Single(x => x.PackageId == "autor.local");
            Assert.AreEqual(Cambio.Corregido, SinFila.Cambio);
            Assert.AreEqual("Mod local", SinFila.Nombre);
            Assert.AreEqual(Hoy, SinFila.Fecha);
            // De lo mas nuevo a lo mas viejo, como el resto de la pestaña.
            Assert.AreEqual("autor.local", Resultado[0].PackageId);
        }

        [TestMethod]
        public void LoYaVistoSinNadaDelPcQuedaComoEstaba()
        {
            var Pendientes = new List<Novedad> { Fila("autor.actualizado", Cambio.Actualizado, "2026-10-21", 5) };

            var Resultado = CambiosLocales.Aplicar(Pendientes, true, Entradas,
                new Dictionary<string, CambiosDeUnMod> { ["autor.actualizado"] = Local("autor.actualizado", 0) }, Archivo, Hoy, out bool YaVistas);

            Assert.IsTrue(YaVistas);
            Assert.AreEqual(1, Resultado.Count);
            Assert.AreEqual(5, Resultado[0].Cambiados);
        }

        [TestMethod]
        public void LoDelPcSinVerReemplazaALoYaVisto()
        {
            var Pendientes = new List<Novedad> { Fila("autor.actualizado", Cambio.Actualizado, "2026-10-21", 5) };

            var Resultado = CambiosLocales.Aplicar(Pendientes, true, Entradas,
                new Dictionary<string, CambiosDeUnMod> { ["autor.corregido"] = Local("autor.corregido", 1) }, Archivo, Hoy, out bool YaVistas);

            Assert.IsFalse(YaVistas);
            Assert.AreEqual(1, Resultado.Count);
            Assert.AreEqual("autor.corregido", Resultado[0].PackageId);
        }

        [TestMethod]
        public void LoDelPcQueYaSeVioEnEstaPartidaNoVuelveASalir()
        {
            var Visto = Local("autor.corregido", 1);
            Visto.Visto = true;

            Assert.AreEqual(0, Aplicar(new List<Novedad>(), false, Visto).Count);
        }

        [TestMethod]
        public void LaCuentaSaleDeLosCambios()
        {
            var Cambios = new List<TextosDeTraduccion.Cambio>
            {
                new TextosDeTraduccion.Cambio(Tipo.Cambiado, "a", "b", "c"),
                new TextosDeTraduccion.Cambio(Tipo.Nuevo, "a", "", "c"),
                new TextosDeTraduccion.Cambio(Tipo.Nuevo, "d", "", "e"),
            };

            var Mod = CambiosLocales.Contar("autor.mod", "Mod", Cambios);

            Assert.AreEqual((1, 2, 0), (Mod.Cambiados, Mod.Nuevos, Mod.Quitados));
            Assert.AreEqual(3, Mod.Total);
        }
    }
}
