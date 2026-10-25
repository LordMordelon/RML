using System;
using System.Collections.Generic;

namespace RmlVentana.Logica
{
    public enum TipoDeTexto { Cambiado, Nuevo, Quitado }

    /** Un texto de Cambios.tsv (Source/LoadFoldersBuilder/CambiosDeTexto.cs). Antes va vacio en uno
     *  nuevo y Ahora, en uno quitado. */
    public class TextoCambiado
    {
        public TipoDeTexto Tipo;
        public string Ingles;
        public string Antes;
        public string Ahora;
    }

    /** La ventana «Ver cambios»: los textos de un mod que cambiaron en la ultima subida de RML. */
    public static class CambiosDeMod
    {
        /** Los textos de un mod, en el orden del archivo. Recibe las lineas y no el texto entero para
         *  que el archivo se pueda leer de a una, y las de otros mods se saltan sin partirlas. */
        public static List<TextoCambiado> Leer(IEnumerable<string> Lineas, string PackageId)
        {
            var Resultado = new List<TextoCambiado>();
            string Prefijo = PackageId + "\t";

            foreach (string Linea in Lineas)
            {
                if (!Linea.StartsWith(Prefijo, StringComparison.OrdinalIgnoreCase))
                    continue;

                string[] Campos = Linea.TrimEnd('\r').Split('\t');
                if (Campos.Length < 5)
                    continue;

                TipoDeTexto Tipo;
                switch (Campos[1])
                {
                    case "c": Tipo = TipoDeTexto.Cambiado; break;
                    case "n": Tipo = TipoDeTexto.Nuevo; break;
                    case "q": Tipo = TipoDeTexto.Quitado; break;
                    default: continue;
                }

                Resultado.Add(new TextoCambiado
                {
                    Tipo = Tipo,
                    Ingles = ConSaltos(Campos[2]),
                    Antes = ConSaltos(Campos[3]),
                    Ahora = ConSaltos(Campos[4]),
                });
            }

            return Resultado;
        }

        /** Las traducciones escriben el salto de linea como «\n» literal, y el juego lo convierte al
         *  cargarlas. Aca se muestra el texto, asi que se convierte igual. */
        private static string ConSaltos(string Texto) => Texto.Replace("\\n", "\n");
    }
}
