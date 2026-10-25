using System;
using System.Collections.Generic;

namespace RmlVentana.Logica
{
    public enum TipoDeTexto { Cambiado, Nuevo, Quitado }

    /** Un texto de Cambios.tsv (TextosDeTraduccion.Fila), el del builder o el del PC (CambiosEnElPc). Antes va vacio en uno
     *  nuevo y Ahora, en uno quitado. */
    public class TextoCambiado
    {
        public TipoDeTexto Tipo;
        public string Ingles;
        public string Antes;
        public string Ahora;
        /** El ingles de antes, solo si cambio (sexta columna). Vacio si no, o si el archivo es anterior. */
        public string InglesAntes = "";
        /** De que es el texto, «ThingDef/Gun_LMG» o «Keyed» (septima columna), para agruparlo (Categorias). */
        public string Objeto = "";
    }

    /** Un texto listo para «Ver cambios»: el ingles en un bloque, con lo quitado y lo agregado si
     *  cambio, y la traduccion en uno o dos. Antes es null si no va el bloque rojo, y Ahora si no va
     *  el verde. */
    public class TextoParaVer
    {
        public List<Trozo> Ingles;
        public List<Trozo> Antes, Ahora;
    }

    /** La ventana «Ver cambios»: los textos de un mod que cambiaron en la ultima subida de RML. */
    public static class CambiosDeMod
    {
        /** Los textos de un mod, en el orden del archivo. Recibe las lineas y no el texto entero para
         *  que el archivo se pueda leer de a una, y las de otros mods se saltan sin partirlas.
         *
         *  Un mod añadido en la subida trae ademas todos sus textos, marcados «a»: son los de su fila
         *  «Añadido» (DelAnadido), y salen como nuevos. Los c, n y q son los de su fila «Corregido»,
         *  la de quien ya lo habia visto. */
        public static List<TextoCambiado> Leer(IEnumerable<string> Lineas, string PackageId, bool DelAnadido = false)
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

                if ((Campos[1] == "a") != DelAnadido)
                    continue;

                TipoDeTexto Tipo;
                switch (Campos[1])
                {
                    case "c": Tipo = TipoDeTexto.Cambiado; break;
                    case "n": case "a": Tipo = TipoDeTexto.Nuevo; break;
                    case "q": Tipo = TipoDeTexto.Quitado; break;
                    default: continue;
                }

                Resultado.Add(new TextoCambiado
                {
                    Tipo = Tipo,
                    Ingles = ConSaltos(Campos[2]),
                    Antes = ConSaltos(Campos[3]),
                    Ahora = ConSaltos(Campos[4]),
                    InglesAntes = Campos.Length > 5 ? ConSaltos(Campos[5]) : "",
                    Objeto = Campos.Length > 6 ? Campos[6] : "",
                });
            }

            return Resultado;
        }

        /** Como se muestra un texto. Si de la traduccion solo se agrego o solo se quito, va un bloque:
         *  el otro repetiria el texto entero sin nada marcado, y parecia que todo habia cambiado. */
        public static TextoParaVer ParaVer(TextoCambiado Texto)
        {
            var Resultado = new TextoParaVer();
            string Ingles = DiffDePalabras.SinEtiquetas(Texto.Ingles), InglesAntes = DiffDePalabras.SinEtiquetas(Texto.InglesAntes ?? "");
            Resultado.Ingles = InglesAntes.Length > 0 && InglesAntes != Ingles
                ? DiffDePalabras.Unir(InglesAntes, Ingles)
                : DiffDePalabras.SinMarcar(Ingles);

            string Antes = DiffDePalabras.SinEtiquetas(Texto.Antes), Ahora = DiffDePalabras.SinEtiquetas(Texto.Ahora);
            if (Antes.Length == 0 || Ahora.Length == 0)
            {
                Resultado.Antes = Antes.Length > 0 ? DiffDePalabras.SinMarcar(Antes) : null;
                Resultado.Ahora = Ahora.Length > 0 ? DiffDePalabras.SinMarcar(Ahora) : null;
                return Resultado;
            }

            DiffDePalabras.Comparar(Antes, Ahora, out Resultado.Antes, out Resultado.Ahora);
            bool Quitado = Resultado.Antes.Exists(x => x.Cambiado), Agregado = Resultado.Ahora.Exists(x => x.Cambiado);
            if (Agregado && !Quitado) Resultado.Antes = null;
            if (Quitado && !Agregado) Resultado.Ahora = null;
            return Resultado;
        }

        /** Las traducciones escriben el salto de linea como «\n» literal, y el juego lo convierte al
         *  cargarlas. Aca se muestra el texto, asi que se convierte igual. */
        private static string ConSaltos(string Texto) => Texto.Replace("\\n", "\n");
    }
}
