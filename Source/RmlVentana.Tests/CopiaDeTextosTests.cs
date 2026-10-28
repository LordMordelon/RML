using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RmlVentana.Logica;
using Texto = RmlVentana.Logica.TextosDeTraduccion.Texto;
using Tipo = RmlVentana.Logica.TextosDeTraduccion.Tipo;

namespace RmlVentana.Tests
{
    /** La copia de los textos que el jugador tenia en su PC y lo que cambio desde entonces
     *  (PLAN-CAMBIOS-LOCALES.md, etapas 2 y 3). */
    [TestClass]
    public class CopiaDeTextosTests
    {
        private const string Keyed = "Languages/SpanishLatin/Keyed|";

        private static List<string> Lineas(string Archivo) => Archivo.Split('\n').ToList();

        private static List<Texto> Ida(string Huella, IEnumerable<Texto> Textos) =>
            CopiaDeTextos.Leer(Lineas(CopiaDeTextos.Escribir(Huella, Textos)), out _);

        [TestMethod]
        public void LoQueSeEscribeSeLeeIgualConSuHuella()
        {
            string Archivo = CopiaDeTextos.Escribir("abc", new[]
            {
                new Texto(Keyed + "A", "War funds", "Fondos"),
                new Texto("Patches|/Defs/ThingDef[defName=\"Rifle\"]/label|label", "rifle", "fusil"),
            });

            var Textos = CopiaDeTextos.Leer(Lineas(Archivo), out string Huella);

            Assert.AreEqual("abc", Huella);
            Assert.AreEqual(2, Textos.Count);
            Assert.AreEqual(Keyed + "A", Textos[0].Clave);
            Assert.AreEqual("War funds", Textos[0].Ingles);
            Assert.AreEqual("Fondos", Textos[0].Valor);
            Assert.AreEqual("fusil", Textos[1].Valor);
        }

        [TestMethod]
        public void LoSinTraducirNoSeGuarda()
        {
            var Textos = Ida("h", new[] { new Texto(Keyed + "A", "one", "TODO"), new Texto(Keyed + "B", "two", "dos") });

            Assert.AreEqual(1, Textos.Count);
            Assert.AreEqual(Keyed + "B", Textos[0].Clave);
        }

        [TestMethod]
        public void LosTabuladoresYSaltosNoRompenLaLinea()
        {
            var Textos = Ida("h", new[] { new Texto(Keyed + "A", "a\tb", "uno\ndos") });

            Assert.AreEqual("a b", Textos[0].Ingles);
            Assert.AreEqual("uno dos", Textos[0].Valor);
        }

        [TestMethod]
        public void UnArchivoSinEncabezadoNoEsUnaCopia()
        {
            Assert.IsNull(CopiaDeTextos.Leer(new[] { "Languages/SpanishLatin/Keyed|A\tFondos\tWar funds" }, out string Huella));
            Assert.IsNull(Huella);
            Assert.IsNull(CopiaDeTextos.Leer(new string[0], out _));
        }

        [TestMethod]
        public void UnaLineaRotaSeSaltaYLasDemasQuedan()
        {
            var Textos = CopiaDeTextos.Leer(new[] { "#\th", "sin tabulador", Keyed + "A\tFondos\tWar funds\r", "" }, out _);

            Assert.AreEqual(1, Textos.Count);
            Assert.AreEqual("War funds", Textos[0].Ingles);
        }

        [TestMethod]
        public void LaHuellaNoDependeDelOrdenYCambiaConElTamanoLaFechaOLaRuta()
        {
            var A = ("Languages/SpanishLatin/Keyed/a.xml", 100L, 5L);
            var B = ("Patches/b.xml", 200L, 6L);

            string Huella = CopiaDeTextos.Huella(new[] { A, B });

            Assert.AreEqual(Huella, CopiaDeTextos.Huella(new[] { B, A }));
            Assert.AreNotEqual(Huella, CopiaDeTextos.Huella(new[] { A, ("Patches/b.xml", 201L, 6L) }));
            Assert.AreNotEqual(Huella, CopiaDeTextos.Huella(new[] { A, ("Patches/b.xml", 200L, 7L) }));
            Assert.AreNotEqual(Huella, CopiaDeTextos.Huella(new[] { A, ("Patches/c.xml", 200L, 6L) }));
            Assert.AreNotEqual(Huella, CopiaDeTextos.Huella(new[] { A }));
        }

        [TestMethod]
        public void ElArchivoDeLaCopiaSeLlamaPorElNumeroDelWorkshop()
        {
            Assert.AreEqual("3070780021.tsv", CopiaDeTextos.NombreDeArchivo("Data/!Sarg Bjornson/Alpha Prefabs - 3070780021"));
            Assert.AreEqual("Data_Mod sin numero.tsv", CopiaDeTextos.NombreDeArchivo("Data/Mod sin numero"));
        }

        // Etapa 3: la copia contra lo de ahora.

        [TestMethod]
        public void ContraLaCopiaSalenElCambiadoElNuevoYElQuitado()
        {
            var Copia = Ida("h", new[]
            {
                new Texto(Keyed + "A", "Cornerstone", "Piedra base"),
                new Texto(Keyed + "B", "hut", "choza"),
                new Texto(Keyed + "C", "same", "igual"),
            });
            var Ahora = new[]
            {
                new Texto(Keyed + "A", "Cornerstone", "Piedra angular"),
                new Texto(Keyed + "C", "same", "igual"),
                new Texto(Keyed + "D", "chief", "jefe"),
            };

            var Cambios = CopiaDeTextos.Comparar(Copia, Ahora);

            CollectionAssert.AreEqual(new[] { Tipo.Cambiado, Tipo.Nuevo, Tipo.Quitado }, Cambios.Select(x => x.Tipo).ToArray());
            Assert.AreEqual("Piedra base", Cambios[0].Antes);
            Assert.AreEqual("Piedra angular", Cambios[0].Ahora);
            Assert.AreEqual("jefe", Cambios[1].Ahora);
            Assert.AreEqual("choza", Cambios[2].Antes);
            Assert.AreEqual("Keyed", Cambios[0].Objeto);
        }

        [TestMethod]
        public void SiCambioElInglesVaTambienElDeLaCopia()
        {
            var Copia = Ida("h", new[] { new Texto(Keyed + "A", "Filters the air.", "Filtra el aire.") });
            var Ahora = new[] { new Texto(Keyed + "A", "Filters the air and rebreathes.", "Filtra el aire y lo recircula.") };

            var Cambios = CopiaDeTextos.Comparar(Copia, Ahora);

            Assert.AreEqual("Filters the air.", Cambios[0].InglesAntes);
            Assert.AreEqual("Filters the air and rebreathes.", Cambios[0].Ingles);
        }

        [TestMethod]
        public void ElInglesQueCambioSinLaTraduccionSigueMarcadoCuandoLaTraduccionCambia()
        {
            // Animals Logic, 27-oct-2026: se reextrajo (cambio el ingles, no la traduccion), el juego se
            // abrio y la copia se puso al dia sola; despues cambio la traduccion y el ingles salio sin marcar.
            var Primera = Ida("h1", new[] { new Texto(Keyed + "A", "Filters the air.", "Filtra el aire.") });
            var Reextraido = new[] { new Texto(Keyed + "A", "Filters the air and rebreathes.", "Filtra el aire.") };
            Assert.AreEqual(0, CopiaDeTextos.Comparar(Primera, Reextraido).Count);

            var Segunda = CopiaDeTextos.Leer(Lineas(CopiaDeTextos.Escribir("h2", Reextraido, Primera)), out _);
            var Traducido = new[] { new Texto(Keyed + "A", "Filters the air and rebreathes.", "Filtra el aire y lo recircula.") };
            var Cambios = CopiaDeTextos.Comparar(Segunda, Traducido);

            Assert.AreEqual("Filters the air.", Cambios[0].InglesAntes);
        }

        [TestMethod]
        public void ConLaTraduccionCambiadaLaCopiaGuardaElInglesNuevo()
        {
            var Primera = Ida("h1", new[] { new Texto(Keyed + "A", "Filters the air.", "Filtra el aire.") });
            var Ahora = new[] { new Texto(Keyed + "A", "Filters the air and rebreathes.", "Filtra el aire y lo recircula.") };

            var Segunda = CopiaDeTextos.Leer(Lineas(CopiaDeTextos.Escribir("h2", Ahora, Primera)), out _);

            Assert.AreEqual("Filters the air and rebreathes.", Segunda[0].Ingles);
        }

        [TestMethod]
        public void UnSaltoDeLineaDentroDelTextoNoEsUnCambio()
        {
            // La copia lo guarda en una linea: lo de ahora se compara igual, o cada texto con un salto
            // saldria cambiado en cada inicio.
            var Copia = Ida("h", new[] { new Texto(Keyed + "A", "x", "uno\ndos") });

            Assert.AreEqual(0, CopiaDeTextos.Comparar(Copia, new[] { new Texto(Keyed + "A", "x", "uno\ndos") }).Count);
        }

        [TestMethod]
        public void SinCambiosNoSaleNada()
        {
            var Ahora = new[] { new Texto(Keyed + "A", "one", "uno"), new Texto(Keyed + "B", "two", "TODO") };

            Assert.AreEqual(0, CopiaDeTextos.Comparar(Ida("h", Ahora), Ahora).Count);
        }
    }
}
