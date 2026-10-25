using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RmlVentana.Logica
{
    /** Un pedazo de texto: una palabra, un signo o un espacio, y si es parte de lo que cambio. */
    public class Trozo
    {
        public string Texto;
        public bool Cambiado;
    }

    /** Que palabras cambiaron entre dos textos, para marcarlas con un color mas fuerte dentro del
     *  renglon, como el diff de VS Code. */
    public static class DiffDePalabras
    {
        /** Arriba de esto la tabla de la comparacion pesa demasiado (con 500 palabras de cada lado ya
         *  son un millon de celdas): se marca el texto entero. */
        private const int MaximoDeCeldas = 1000000;

        private static readonly Regex Palabras = new Regex(@"\s+|[\p{L}\p{N}_]+|.", RegexOptions.Compiled);

        private static readonly Regex Etiquetas = new Regex(@"</?(color|b|i|size)(=[^>]*)?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /** Los dos textos en trozos que, unidos, dan el texto original. Lo que no esta en el otro texto
         *  va marcado; un espacio entre dos trozos marcados tambien, para que una frase cambiada se
         *  marque de corrido y no palabra por palabra. */
        public static void Comparar(string Antes, string Ahora, out List<Trozo> TrozosAntes, out List<Trozo> TrozosAhora)
        {
            List<string> A = Partir(Antes), B = Partir(Ahora);
            TrozosAntes = new List<Trozo>();
            TrozosAhora = new List<Trozo>();

            if ((long)A.Count * B.Count > MaximoDeCeldas)
            {
                foreach (string Texto in A) TrozosAntes.Add(new Trozo { Texto = Texto, Cambiado = true });
                foreach (string Texto in B) TrozosAhora.Add(new Trozo { Texto = Texto, Cambiado = true });
                return;
            }

            // Largo de la subsecuencia comun mas larga desde cada par de posiciones.
            var Comun = new int[A.Count + 1, B.Count + 1];
            for (int i = A.Count - 1; i >= 0; i--)
                for (int j = B.Count - 1; j >= 0; j--)
                    Comun[i, j] = A[i] == B[j] ? Comun[i + 1, j + 1] + 1 : System.Math.Max(Comun[i + 1, j], Comun[i, j + 1]);

            int x = 0, y = 0;
            while (x < A.Count || y < B.Count)
            {
                if (x < A.Count && y < B.Count && A[x] == B[y])
                {
                    TrozosAntes.Add(new Trozo { Texto = A[x++] });
                    TrozosAhora.Add(new Trozo { Texto = B[y++] });
                }
                else if (y >= B.Count || (x < A.Count && Comun[x + 1, y] >= Comun[x, y + 1]))
                    TrozosAntes.Add(new Trozo { Texto = A[x++], Cambiado = true });
                else
                    TrozosAhora.Add(new Trozo { Texto = B[y++], Cambiado = true });
            }

            UnirEspacios(TrozosAntes);
            UnirEspacios(TrozosAhora);
        }

        /** El texto sin las etiquetas de formato de RimWorld (<color=…>, <b>, <i>, <size=…>): la ventana
         *  lo dibuja palabra por palabra, y una etiqueta partida se veria tal cual. */
        public static string SinEtiquetas(string Texto) => Etiquetas.Replace(Texto, "");

        /** Un texto sin otro con que compararlo (uno nuevo o uno quitado): sus trozos, sin marcar. */
        public static List<Trozo> SinMarcar(string Texto)
        {
            var Resultado = new List<Trozo>();
            foreach (string Parte in Partir(Texto))
                Resultado.Add(new Trozo { Texto = Parte });
            return Resultado;
        }

        private static List<string> Partir(string Texto)
        {
            var Resultado = new List<string>();
            foreach (Match Palabra in Palabras.Matches(Texto))
                Resultado.Add(Palabra.Value);
            return Resultado;
        }

        private static void UnirEspacios(List<Trozo> Trozos)
        {
            for (int i = 1; i < Trozos.Count - 1; i++)
                if (!Trozos[i].Cambiado && Trozos[i - 1].Cambiado && Trozos[i + 1].Cambiado && Trozos[i].Texto.Trim().Length == 0)
                    Trozos[i].Cambiado = true;
        }
    }
}
