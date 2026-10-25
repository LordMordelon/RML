using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RmlVentana.Logica;

namespace RmlVentana.Tests
{
    [TestClass]
    public class DiffDePalabrasTests
    {
        /** Lo cambiado entre corchetes, para comparar de un vistazo. */
        private static string Marcado(List<Trozo> Trozos)
        {
            var Texto = new StringBuilder();
            bool Dentro = false;
            foreach (Trozo Trozo in Trozos)
            {
                if (Trozo.Cambiado != Dentro)
                {
                    Texto.Append(Trozo.Cambiado ? "[" : "]");
                    Dentro = Trozo.Cambiado;
                }
                Texto.Append(Trozo.Texto);
            }
            if (Dentro) Texto.Append("]");
            return Texto.ToString();
        }

        [TestMethod]
        public void LasPalabrasCambiadasSeguidasSeMarcanJuntasConSusEspacios()
        {
            DiffDePalabras.Comparar("Instalación de cubos de basura", "Instalación de robots barredores",
                out var Antes, out var Ahora);

            Assert.AreEqual("Instalación de [cubos de basura]", Marcado(Antes));
            Assert.AreEqual("Instalación de [robots barredores]", Marcado(Ahora));
        }

        [TestMethod]
        public void UnaPalabraQuitadaSeMarcaSoloAntes()
        {
            DiffDePalabras.Comparar("pistola de meca", "pistola meca", out var Antes, out var Ahora);

            CollectionAssert.AreEqual(new[] { "de" },
                Antes.Where(x => x.Cambiado && x.Texto.Trim().Length > 0).Select(x => x.Texto).ToList());
            Assert.IsFalse(Ahora.Any(x => x.Cambiado));
        }

        [TestMethod]
        public void UnaMayusculaCambiaLaPalabra()
        {
            DiffDePalabras.Comparar("Nave infestada", "Nave Infestada", out var Antes, out var Ahora);

            Assert.AreEqual("Nave [infestada]", Marcado(Antes));
            Assert.AreEqual("Nave [Infestada]", Marcado(Ahora));
        }

        [TestMethod]
        public void LaPuntuacionVaAparteDeLaPalabra()
        {
            DiffDePalabras.Comparar("Hola, mundo.", "Hola, mundo!", out var Antes, out var Ahora);

            Assert.AreEqual("Hola, mundo[.]", Marcado(Antes));
            Assert.AreEqual("Hola, mundo[!]", Marcado(Ahora));
        }

        [TestMethod]
        public void LosTextosSeRearmanEnteros()
        {
            const string Viejo = "Una línea.\n\nOtra línea con  dos espacios.";
            const string Nuevo = "Una línea distinta.\n\nOtra línea.";
            DiffDePalabras.Comparar(Viejo, Nuevo, out var Antes, out var Ahora);

            Assert.AreEqual(Viejo, string.Concat(Antes.Select(x => x.Texto)));
            Assert.AreEqual(Nuevo, string.Concat(Ahora.Select(x => x.Texto)));
        }

        [TestMethod]
        public void UnTextoMuyLargoSeMarcaEnteroSinCompararPalabraPorPalabra()
        {
            string Largo = string.Join(" ", Enumerable.Range(0, 3000).Select(i => "p" + i));
            DiffDePalabras.Comparar(Largo, Largo + " fin", out var Antes, out var Ahora);

            Assert.IsTrue(Antes.All(x => x.Cambiado));
            Assert.IsTrue(Ahora.All(x => x.Cambiado));
        }

        /** Lo quitado entre [- ] y lo agregado entre [+ ], para el ingles, que va en un solo bloque. */
        private static string Unido(List<Trozo> Trozos)
        {
            var Texto = new StringBuilder();
            string Dentro = null;
            foreach (Trozo Trozo in Trozos)
            {
                string Marca = !Trozo.Cambiado ? null : Trozo.Quitado ? "-" : "+";
                if (Marca != Dentro)
                {
                    if (Dentro != null) Texto.Append("]");
                    if (Marca != null) Texto.Append("[" + Marca);
                    Dentro = Marca;
                }
                Texto.Append(Trozo.Texto);
            }
            if (Dentro != null) Texto.Append("]");
            return Texto.ToString();
        }

        [TestMethod]
        public void ElInglesUnidoMarcaLoAgregadoEnSuLugar()
        {
            var Trozos = DiffDePalabras.Unir("Filters the air.", "Filters the air, provides limited rebreathing and cools.");

            Assert.AreEqual("Filters the air[+, provides limited rebreathing and cools].", Unido(Trozos));
        }

        [TestMethod]
        public void ElInglesUnidoPoneLoQuitadoAntesDeLoAgregadoConUnEspacio()
        {
            var Trozos = DiffDePalabras.Unir("the big dog", "the small dog");

            Assert.AreEqual("the [-big] [+small] dog", Unido(Trozos));
        }

        [TestMethod]
        public void SeSacanLasEtiquetasDeFormato()
        {
            Assert.AreEqual("rojo y negrita", DiffDePalabras.SinEtiquetas("<color=#FF0000>rojo</color> y <b>negrita</b>"));
            Assert.AreEqual("a < b", DiffDePalabras.SinEtiquetas("a < b"));
        }
    }
}
