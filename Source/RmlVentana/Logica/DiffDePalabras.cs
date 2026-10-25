using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RmlVentana.Logica
{
    /** Un pedazo de texto: una palabra, un signo o un espacio, y si es parte de lo que cambio. */
    public class Trozo
    {
        public string Texto;
        public bool Cambiado;
        /** Si lo cambiado es lo que se quito y no lo que se agrego: la ventana elige asi el color del resalte. */
        public bool Quitado;
    }

    /** Que palabras cambiaron entre dos textos, para marcarlas con un color mas fuerte dentro del
     *  renglon, como el diff de VS Code. */
    public static class DiffDePalabras
    {
        /** Arriba de esto la tabla de la comparacion pesa demasiado (con 500 palabras de cada lado ya
         *  son un millon de celdas): se marca el texto entero. */
        private const int MaximoDeCeldas = 1000000;

        private static readonly Regex Palabras = new Regex(@"\s+|[\p{L}\p{N}_]+|.", RegexOptions.Compiled);

        /** Los dos textos en trozos que, unidos, dan el texto original. Lo que no esta en el otro texto
         *  va marcado; un espacio entre dos trozos marcados tambien, para que una frase cambiada se
         *  marque de corrido y no palabra por palabra. */
        public static void Comparar(string Antes, string Ahora, out List<Trozo> TrozosAntes, out List<Trozo> TrozosAhora)
        {
            TrozosAntes = new List<Trozo>();
            TrozosAhora = new List<Trozo>();
            foreach (Trozo Trozo in Recorrer(Antes, Ahora))
            {
                if (!Trozo.Cambiado || Trozo.Quitado) TrozosAntes.Add(new Trozo { Texto = Trozo.Texto, Cambiado = Trozo.Cambiado, Quitado = Trozo.Quitado });
                if (!Trozo.Cambiado || !Trozo.Quitado) TrozosAhora.Add(new Trozo { Texto = Trozo.Texto, Cambiado = Trozo.Cambiado });
            }

            UnirEspacios(TrozosAntes);
            UnirEspacios(TrozosAhora);
        }

        /** Los dos textos en una sola lista, para el ingles, que va en un solo bloque: lo quitado y lo
         *  agregado en su lugar, cada tramo con lo quitado primero y separado de lo agregado por un
         *  espacio, como el control de cambios de un procesador de texto. */
        public static List<Trozo> Unir(string Antes, string Ahora)
        {
            var Resultado = new List<Trozo>();
            var Quitados = new List<Trozo>();
            var Agregados = new List<Trozo>();

            void Volcar()
            {
                Resultado.AddRange(Quitados);
                if (Quitados.Count > 0 && Agregados.Count > 0
                    && Quitados[Quitados.Count - 1].Texto.Trim().Length > 0 && Agregados[0].Texto.Trim().Length > 0)
                    Resultado.Add(new Trozo { Texto = " " });
                Resultado.AddRange(Agregados);
                Quitados.Clear();
                Agregados.Clear();
            }

            foreach (Trozo Trozo in Recorrer(Antes, Ahora))
            {
                if (!Trozo.Cambiado)
                {
                    Volcar();
                    Resultado.Add(Trozo);
                }
                else
                    (Trozo.Quitado ? Quitados : Agregados).Add(Trozo);
            }
            Volcar();

            UnirEspacios(Resultado);
            return Resultado;
        }

        /** Los trozos de los dos textos en el orden de la comparacion: los comunes una vez, y los de un
         *  solo lado marcados, con Quitado si son del de antes. */
        private static List<Trozo> Recorrer(string Antes, string Ahora)
        {
            List<string> A = Partir(Antes), B = Partir(Ahora);
            var Resultado = new List<Trozo>();

            if ((long)A.Count * B.Count > MaximoDeCeldas)
            {
                foreach (string Texto in A) Resultado.Add(new Trozo { Texto = Texto, Cambiado = true, Quitado = true });
                foreach (string Texto in B) Resultado.Add(new Trozo { Texto = Texto, Cambiado = true });
                return Resultado;
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
                    Resultado.Add(new Trozo { Texto = A[x++] });
                    y++;
                }
                else if (y >= B.Count || (x < A.Count && Comun[x + 1, y] >= Comun[x, y + 1]))
                    Resultado.Add(new Trozo { Texto = A[x++], Cambiado = true, Quitado = true });
                else
                    Resultado.Add(new Trozo { Texto = B[y++], Cambiado = true });
            }

            return Resultado;
        }

        /** El texto sin las etiquetas de formato de RimWorld (<color=…>, <b>, <i>, <size=…>): la ventana
         *  lo dibuja palabra por palabra, y una etiqueta partida se veria tal cual. */
        public static string SinEtiquetas(string Texto) => TextosDeTraduccion.SinEtiquetas(Texto);

        /** Un texto sin otro con que compararlo (uno nuevo o uno quitado): sus trozos, sin marcar. */
        public static List<Trozo> SinMarcar(string Texto)
        {
            var Resultado = new List<Trozo>();
            foreach (string Parte in Partir(Texto))
                Resultado.Add(new Trozo { Texto = Parte });
            return Resultado;
        }

        /** Los mismos trozos para verlos en una sola linea: cada salto se escribe «\n», como en el archivo
         *  de traduccion, y conserva su marca. */
        public static List<Trozo> EnUnaLinea(List<Trozo> Trozos)
        {
            var Resultado = new List<Trozo>();
            foreach (Trozo Trozo in Trozos)
                Resultado.Add(new Trozo { Texto = Trozo.Texto.Replace("\r", "").Replace("\n", "\\n"), Cambiado = Trozo.Cambiado, Quitado = Trozo.Quitado });
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
            // En la lista unida, solo entre dos del mismo lado: entre lo quitado y lo agregado queda sin marcar.
            for (int i = 1; i < Trozos.Count - 1; i++)
                if (!Trozos[i].Cambiado && Trozos[i - 1].Cambiado && Trozos[i + 1].Cambiado
                    && Trozos[i - 1].Quitado == Trozos[i + 1].Quitado && Trozos[i].Texto.Trim().Length == 0)
                {
                    Trozos[i].Cambiado = true;
                    Trozos[i].Quitado = Trozos[i - 1].Quitado;
                }
        }
    }
}
