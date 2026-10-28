using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RmlVentana.Logica;

namespace RmlVentana.Tests
{
    [TestClass]
    public class CambiosDeModTests
    {
        [TestMethod]
        public void LaSextaColumnaEsElInglesDeAntesYSinEllaQuedaVacio()
        {
            var Leidos = CambiosDeMod.Leer(new[]
            {
                "autor.mod\tc\tA new one\tuno\tdos\tAn old one",
                "autor.mod\tc\tSame\ttres\tcuatro",
            }, "autor.mod");

            Assert.AreEqual("An old one", Leidos[0].InglesAntes);
            Assert.AreEqual("", Leidos[1].InglesAntes);
        }

        [TestMethod]
        public void LosTextosDeUnModAnadidoSeLeenSoloParaSuFilaAnadido()
        {
            var Lineas = new[]
            {
                "autor.mod\ta\tWar funds\t\tFondos secretos",
                "autor.mod\tc\tMediate peace\tMediar por la paz\tNegociar la paz",
            };

            var DelAnadido = CambiosDeMod.Leer(Lineas, "autor.mod", DelAnadido: true);
            var DelCambio = CambiosDeMod.Leer(Lineas, "autor.mod");

            Assert.AreEqual(1, DelAnadido.Count);
            Assert.AreEqual(TipoDeTexto.Nuevo, DelAnadido[0].Tipo);
            Assert.AreEqual("Fondos secretos", DelAnadido[0].Ahora);
            Assert.AreEqual(1, DelCambio.Count);
            Assert.AreEqual("Negociar la paz", DelCambio[0].Ahora);
        }

        [TestMethod]
        public void SoloAgregadoVaEnUnBloqueVerde()
        {
            // [FSF] Advanced Bionics: la traduccion solo gano una frase y la ventana mostraba el rojo entero.
            var Texto = CambiosDeMod.ParaVer(new TextoCambiado
            {
                Tipo = TipoDeTexto.Cambiado, Ingles = "", InglesAntes = "",
                Antes = "Filtra el aire.", Ahora = "Filtra el aire y lo recircula.",
            });

            Assert.IsNull(Texto.Antes);
            Assert.IsTrue(Texto.Ahora.Any(x => x.Cambiado));
        }

        [TestMethod]
        public void SoloQuitadoVaEnUnBloqueRojo()
        {
            var Texto = CambiosDeMod.ParaVer(new TextoCambiado
            {
                Tipo = TipoDeTexto.Cambiado, Ingles = "", InglesAntes = "",
                Antes = "pistola de meca", Ahora = "pistola meca",
            });

            Assert.IsTrue(Texto.Antes.Any(x => x.Cambiado));
            Assert.IsNull(Texto.Ahora);
        }

        [TestMethod]
        public void AgregadoYQuitadoVanEnDosBloques()
        {
            var Texto = CambiosDeMod.ParaVer(new TextoCambiado
            {
                Tipo = TipoDeTexto.Cambiado, Ingles = "", InglesAntes = "",
                Antes = "cubo de basura", Ahora = "robot barredor",
            });

            Assert.IsNotNull(Texto.Antes);
            Assert.IsNotNull(Texto.Ahora);
        }

        [TestMethod]
        public void ElInglesSeResaltaSoloSiCambio()
        {
            var Cambio = CambiosDeMod.ParaVer(new TextoCambiado
            {
                Tipo = TipoDeTexto.Cambiado, Ingles = "the small dog", InglesAntes = "the big dog", Antes = "a", Ahora = "b",
            });
            var Igual = CambiosDeMod.ParaVer(new TextoCambiado
            {
                Tipo = TipoDeTexto.Cambiado, Ingles = "the dog", InglesAntes = "", Antes = "a", Ahora = "b",
            });

            // Como la traduccion: el de antes con lo quitado y el de ahora con lo agregado, cada uno en su bloque.
            Assert.AreEqual("the big dog", string.Concat(Cambio.InglesAntes.Select(x => x.Texto)));
            Assert.IsTrue(Cambio.InglesAntes.Any(x => x.Cambiado && x.Quitado));
            Assert.AreEqual("the small dog", string.Concat(Cambio.Ingles.Select(x => x.Texto)));
            Assert.IsTrue(Cambio.Ingles.Any(x => x.Cambiado && !x.Quitado));
            Assert.IsNull(Igual.InglesAntes);
            Assert.AreEqual("the dog", string.Concat(Igual.Ingles.Select(x => x.Texto)));
            Assert.IsFalse(Igual.Ingles.Any(x => x.Cambiado));
        }

        [TestMethod]
        public void SiAlInglesSoloSeLeAgregoVaUnBloque()
        {
            var Agregado = CambiosDeMod.ParaVer(new TextoCambiado
            {
                Tipo = TipoDeTexto.Cambiado, Ingles = "Filters the air and cools.", InglesAntes = "Filters the air.", Antes = "a", Ahora = "b",
            });
            var Quitado = CambiosDeMod.ParaVer(new TextoCambiado
            {
                Tipo = TipoDeTexto.Cambiado, Ingles = "Filters the air.", InglesAntes = "Filters the air and cools.", Antes = "a", Ahora = "b",
            });

            Assert.IsNull(Agregado.InglesAntes);
            Assert.IsTrue(Agregado.Ingles.Any(x => x.Cambiado));
            Assert.IsNull(Quitado.Ingles);
            Assert.IsTrue(Quitado.InglesAntes.Any(x => x.Cambiado && x.Quitado));
        }

        [TestMethod]
        public void UnTextoNuevoVaSoloEnVerdeSinResaltar()
        {
            var Texto = CambiosDeMod.ParaVer(new TextoCambiado { Tipo = TipoDeTexto.Nuevo, Ingles = "x", InglesAntes = "", Antes = "", Ahora = "nuevo" });

            Assert.IsNull(Texto.Antes);
            Assert.IsFalse(Texto.Ahora.Any(x => x.Cambiado));
        }
    }
}
