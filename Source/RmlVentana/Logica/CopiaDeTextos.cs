using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Texto = RmlVentana.Logica.TextosDeTraduccion.Texto;

namespace RmlVentana.Logica
{
    /** La copia de los textos de RML que el jugador tenia en su PC, uno por carpeta de mod activo, en
     *  Config/RML_Textos/ (PLAN-CAMBIOS-LOCALES.md). «Ver cambios» compara contra ella, y no contra la
     *  ultima subida al Workshop: asi le sirve igual a quien baja RML de GitHub o lo copia a mano.
     *
     *  La primera linea es la huella de los XML del mod («#», tabulador, huella): si no cambio, no
     *  hace falta leer el mod. Despues, una linea por texto traducido: clave, traduccion e ingles.
     */
    public static class CopiaDeTextos
    {
        public const string Carpeta = "RML_Textos";

        private const string Marca = "#";

        private static readonly Regex IdAlFinal = new Regex(@" - (\d+)$", RegexOptions.Compiled);

        /** Por el numero del Workshop, que no se repite en Data/ y deja la ruta corta. Una carpeta sin
         *  numero (no deberia haberlas) va por su nombre, sin lo que Windows no acepta. */
        public static string NombreDeArchivo(string CarpetaDelMod)
        {
            Match Id = IdAlFinal.Match(CarpetaDelMod);
            if (Id.Success)
                return Id.Groups[1].Value + ".tsv";
            string Nombre = new string(CarpetaDelMod.Select(x => x == '/' || x == '\\' || x == ':' || x == '*' || x == '?' ||
                                                                 x == '"' || x == '<' || x == '>' || x == '|' ? '_' : x).ToArray());
            return Nombre.TrimStart('_') + ".tsv";
        }

        /** Una huella de los XML de traduccion de un mod: su ruta dentro de la carpeta, su tamaño y su
         *  fecha. CopiaLimpia solo reescribe lo que cambio, asi que un mod que no se toco da la misma. */
        public static string Huella(IEnumerable<(string Ruta, long Tamano, long Fecha)> Archivos)
        {
            // FNV-1a de 64 bits: estable entre ejecuciones, a diferencia de string.GetHashCode.
            ulong Hash = 14695981039346656037UL;
            foreach (var Archivo in Archivos.OrderBy(x => x.Ruta, StringComparer.Ordinal))
            {
                foreach (char Letra in Archivo.Ruta + "|" + Archivo.Tamano + "|" + Archivo.Fecha + "\n")
                {
                    Hash ^= Letra;
                    Hash *= 1099511628211UL;
                }
            }
            return Hash.ToString("x16");
        }

        /** El archivo de la copia. Lo sin traducir no va: Comparar no lo muestra, y la copia pesa menos. */
        public static string Escribir(string Huella, IEnumerable<Texto> Textos)
        {
            var Resultado = new StringBuilder();
            Resultado.Append(Marca).Append('\t').Append(Huella).Append('\n');
            foreach (Texto Texto in Textos)
            {
                string Valor = TextosDeTraduccion.Limpio(Texto.Valor);
                if (!TextosDeTraduccion.Traducido(Valor))
                    continue;
                Resultado.Append(Texto.Clave).Append('\t').Append(Valor).Append('\t')
                    .Append(TextosDeTraduccion.Limpio(Texto.Ingles)).Append('\n');
            }
            return Resultado.ToString();
        }

        /** Los textos de la copia, o null si el archivo no empieza con la huella (vacio o de otra cosa):
         *  entonces se toma como un mod sin copia. Una linea rota se salta. */
        public static List<Texto> Leer(IEnumerable<string> Lineas, out string Huella)
        {
            Huella = null;
            List<Texto> Resultado = null;
            foreach (string Cruda in Lineas)
            {
                string Linea = Cruda.TrimEnd('\r');
                if (Resultado == null)
                {
                    if (!Linea.StartsWith(Marca + "\t", StringComparison.Ordinal))
                        return null;
                    Huella = Linea.Substring(Marca.Length + 1);
                    Resultado = new List<Texto>();
                    continue;
                }

                string[] Campos = Linea.Split('\t');
                if (Campos.Length >= 2)
                    Resultado.Add(new Texto(Campos[0], Campos.Length >= 3 ? Campos[2] : "", Campos[1]));
            }
            return Resultado;
        }

        /** Lo que cambio desde la copia. Lo de ahora se compara como se guarda, en una sola linea: si
         *  no, un texto con un salto de linea saldria cambiado en cada inicio. */
        public static List<TextosDeTraduccion.Cambio> Comparar(IEnumerable<Texto> Copia, IEnumerable<Texto> Ahora) =>
            TextosDeTraduccion.Comparar(Copia,
                Ahora.Select(x => new Texto(x.Clave, TextosDeTraduccion.Limpio(x.Ingles), TextosDeTraduccion.Limpio(x.Valor))));
    }
}
