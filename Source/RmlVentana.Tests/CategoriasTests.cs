using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RmlVentana.Logica;

namespace RmlVentana.Tests
{
    [TestClass]
    public class CategoriasTests
    {
        [TestMethod]
        public void ElObjetoSeParteEnTipoYDefName()
        {
            Assert.IsTrue(Categorias.Partir("ThingDef/Gun_LMG", out string Tipo, out string DefName));
            Assert.AreEqual("ThingDef", Tipo);
            Assert.AreEqual("Gun_LMG", DefName);

            Assert.IsTrue(Categorias.Partir("Keyed", out Tipo, out DefName));
            Assert.AreEqual("Keyed", Tipo);
            Assert.IsNull(DefName);

            Assert.IsFalse(Categorias.Partir("", out _, out _));
        }

        [TestMethod]
        public void SinMirarElDefSeSabenLosTiposQueNoSonObjetos()
        {
            Assert.AreEqual(Categoria.Interfaz, Categorias.PorTipo("Keyed"));
            Assert.AreEqual(Categoria.Investigacion, Categorias.PorTipo("ResearchProjectDef"));
            Assert.AreEqual(Categoria.Recetas, Categorias.PorTipo("RecipeDef"));
            Assert.AreEqual(Categoria.Salud, Categorias.PorTipo("HediffDef"));
            Assert.AreEqual(Categoria.Escenarios, Categorias.PorTipo("ScenarioDef"));
            Assert.AreEqual(Categoria.Criaturas, Categorias.PorTipo("PawnKindDef"));
            // Un ThingDef puede ser arma, torreta, construccion…: eso lo dice el juego, no el tipo.
            Assert.AreEqual(Categoria.Otros, Categorias.PorTipo("ThingDef"));
            Assert.AreEqual(Categoria.Otros, Categorias.PorTipo(null));
        }

        [TestMethod]
        public void LaClaveDeLaTraduccionOficialDaElMismoObjeto()
        {
            Assert.AreEqual("ThingDef/Beer", Categorias.ObjetoDeClaveOficial("DefInjected/ThingDef/Beer.label"));
            Assert.AreEqual("Keyed", Categorias.ObjetoDeClaveOficial("Keyed/ClickToSeeMore"));
        }

        [TestMethod]
        public void SeAgrupanEnElOrdenDeLasCategoriasYSinCambiarElOrdenDentro()
        {
            var Textos = new[] { ("a", Categoria.Otros), ("b", Categoria.Armas), ("c", Categoria.Interfaz), ("d", Categoria.Armas) };

            var Grupos = Categorias.Agrupar(Textos, x => x.Item2);

            CollectionAssert.AreEqual(new[] { Categoria.Armas, Categoria.Interfaz, Categoria.Otros }, Grupos.Select(x => x.Categoria).ToList());
            CollectionAssert.AreEqual(new[] { "b", "d" }, Grupos[0].Textos.Select(x => x.Item1).ToList());
        }

        [TestMethod]
        public void LaSeptimaColumnaEsElObjeto()
        {
            var Leidos = CambiosDeMod.Leer(new[]
            {
                "autor.mod\tc\tA\tuno\tdos\t\tThingDef/Gun_LMG",
                "autor.mod\tc\tB\ttres\tcuatro",
            }, "autor.mod");

            Assert.AreEqual("ThingDef/Gun_LMG", Leidos[0].Objeto);
            Assert.AreEqual("", Leidos[1].Objeto);
        }

        [TestMethod]
        public void LaTraduccionOficialGuardaElObjetoEnSuLinea()
        {
            var Viejo = new Dictionary<string, TextoOficial> { ["DefInjected/ThingDef/Beer.label"] = new TextoOficial("beer", "cerveza") };
            var Nuevo = new Dictionary<string, TextoOficial> { ["DefInjected/ThingDef/Beer.label"] = new TextoOficial("beer", "cerveza fría") };

            var Cambio = TraduccionOficial.Comparar(Viejo, Nuevo).Single();
            var Leido = CambiosDeMod.Leer(new[] { TraduccionOficial.Linea("ludeon.rimworld", Cambio) }, "ludeon.rimworld").Single();

            Assert.AreEqual("ThingDef/Beer", Leido.Objeto);
        }
    }
}
