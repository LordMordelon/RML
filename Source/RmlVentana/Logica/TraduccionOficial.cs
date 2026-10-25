using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace RmlVentana.Logica
{
    public enum EstadoOficial { SinConsulta, AlDia, HayArreglos, FaltaReiniciar, SinUsar }

    public enum Deshacer { Nada, DevolverElTar, BorrarElRespaldo }

    /** Un texto de la traduccion oficial: el ingles del comentario «EN:», si lo trae, y la traduccion. */
    public class TextoOficial
    {
        public readonly string Ingles;
        public readonly string Texto;

        public TextoOficial(string Ingles, string Texto)
        {
            this.Ingles = Ingles;
            this.Texto = Texto;
        }
    }

    /** El boton que baja la traduccion oficial de GitHub (PLAN-TRADUCCION-OFICIAL.md). El juego trae la
     *  que Ludeon empaqueto en su ultima actualizacion, y lo que los traductores arreglan despues
     *  espera en GitHub hasta la siguiente. */
    public static class TraduccionOficial
    {
        public const string Repositorio = "Ludeon/RimWorld-SpanishLatin";
        public const string Idioma = "SpanishLatin (Español(Latinoamérica))";

        /** El .tar renombrado. El juego solo mira la extension .tar, asi que con esta deja de contar. */
        public const string ExtensionDelRespaldo = ".tar.respaldo-rml";

        /** «1.7» de «versión 1.7.4982». El repositorio sigue a la ultima version del juego, y con otra
         *  version principal sus textos no corresponden. */
        public static string VersionDelReadme(string Readme)
        {
            if (Readme == null)
                return null;
            Match Numero = Regex.Match(Readme, @"\b(\d+\.\d+)\.\d+");
            return Numero.Success ? Numero.Groups[1].Value : null;
        }

        /** Las carpetas del zip que el juego tambien tiene en Data/, con el nombre de Data/. Un DLC que
         *  no esta instalado se salta, y Scripts y .github no tienen carpeta en Data/. */
        public static List<string> CarpetasAInstalar(IEnumerable<string> DelZip, IEnumerable<string> DeData)
        {
            var Resultado = new List<string>();
            var Data = DeData.ToList();
            foreach (string Carpeta in DelZip)
            {
                if (Carpeta.StartsWith("."))
                    continue;
                string EnData = Data.FirstOrDefault(x => string.Equals(x, Carpeta, StringComparison.OrdinalIgnoreCase));
                if (EnData != null && !Resultado.Contains(EnData))
                    Resultado.Add(EnData);
            }
            return Resultado;
        }

        public static string PackageIdDe(string Carpeta) =>
            string.Equals(Carpeta, "Core", StringComparison.OrdinalIgnoreCase)
                ? "ludeon.rimworld"
                : "ludeon.rimworld." + Carpeta.ToLowerInvariant();

        /** GitHub es la fecha del ultimo cambio del repositorio; DelJuego, la del empaquetado del .tar
         *  (sus archivos la traen adentro); Instalado, la del cambio que se bajo con el boton. */
        public static EstadoOficial EstadoDe(DateTime? GitHub, DateTime? DelJuego, DateTime? Instalado, bool TarPresente, bool FaltaReiniciar)
        {
            if (FaltaReiniciar)
                return EstadoOficial.FaltaReiniciar;

            // Con su .tar de vuelta, el juego no mira la carpeta (AbstractFilesystem.GetDirectory).
            if (Instalado != null && TarPresente)
                return EstadoOficial.SinUsar;

            if (GitHub == null)
                return EstadoOficial.SinConsulta;

            DateTime? Actual = TarPresente ? DelJuego : Instalado;
            return Actual == null || GitHub > Actual ? EstadoOficial.HayArreglos : EstadoOficial.AlDia;
        }

        /** Si mientras tanto Steam puso un .tar nuevo, ese es el bueno y el respaldo viejo sobra. */
        public static Deshacer QueHaceDeshacer(bool TarPresente, bool RespaldoPresente)
        {
            if (!RespaldoPresente)
                return Deshacer.Nada;
            return TarPresente ? Deshacer.BorrarElRespaldo : Deshacer.DevolverElTar;
        }

        /** Los textos de un XML de Keyed o DefInjected. Prefijo es la carpeta («DefInjected/ThingDef/»),
         *  para que la clave sea unica sin depender del archivo, que puede cambiar de nombre. Una lista
         *  va con un renglon por elemento. Un XML roto no trae nada. */
        public static Dictionary<string, TextoOficial> LeerXml(string Contenido, string Prefijo)
        {
            var Resultado = new Dictionary<string, TextoOficial>();
            var Documento = new XmlDocument();
            try
            {
                Documento.LoadXml(Contenido);
            }
            catch (XmlException)
            {
                return Resultado;
            }

            if (Documento.DocumentElement == null)
                return Resultado;

            string Ingles = "";
            foreach (XmlNode Nodo in Documento.DocumentElement.ChildNodes)
            {
                if (Nodo is XmlComment Comentario)
                {
                    string Texto = Comentario.Value.Trim();
                    Ingles = Texto.StartsWith("EN:") ? Texto.Substring(3).Trim() : "";
                    continue;
                }

                if (!(Nodo is XmlElement Elemento))
                    continue;

                var Elementos = Elemento.ChildNodes.OfType<XmlElement>().ToList();
                string Valor = Elementos.Count > 0 && Elementos.All(x => x.Name == "li")
                    ? string.Join("\n", Elementos.Select(x => x.InnerText))
                    : Elemento.InnerText;

                Resultado[Prefijo + Elemento.Name] = new TextoOficial(Ingles, Valor);
                Ingles = "";
            }

            return Resultado;
        }

        /** Lo que cambio entre la traduccion que el juego tenia y la recien bajada: primero lo de la
         *  nueva, en su orden, y al final lo que se quito. */
        public static List<TextoCambiado> Comparar(Dictionary<string, TextoOficial> Viejo, Dictionary<string, TextoOficial> Nuevo)
        {
            var Resultado = new List<TextoCambiado>();

            foreach (var Par in Nuevo)
            {
                if (!Viejo.TryGetValue(Par.Key, out TextoOficial Anterior))
                    Resultado.Add(new TextoCambiado { Tipo = TipoDeTexto.Nuevo, Ingles = Par.Value.Ingles, Antes = "", Ahora = Par.Value.Texto });
                else if (Anterior.Texto != Par.Value.Texto)
                    Resultado.Add(new TextoCambiado
                    {
                        Tipo = TipoDeTexto.Cambiado,
                        Ingles = Par.Value.Ingles != "" ? Par.Value.Ingles : Anterior.Ingles,
                        Antes = Anterior.Texto,
                        Ahora = Par.Value.Texto,
                    });
            }

            foreach (var Par in Viejo)
                if (!Nuevo.ContainsKey(Par.Key))
                    Resultado.Add(new TextoCambiado { Tipo = TipoDeTexto.Quitado, Ingles = Par.Value.Ingles, Antes = Par.Value.Texto, Ahora = "" });

            return Resultado;
        }

        /** Una linea con el formato de Cambios.tsv, para que «Ver cambios» la lea con CambiosDeMod.Leer.
         *  El salto de linea va como «\n» literal, que es como lo escriben las traducciones. */
        public static string Linea(string PackageId, TextoCambiado Texto)
        {
            string Tipo = Texto.Tipo == TipoDeTexto.Cambiado ? "c" : Texto.Tipo == TipoDeTexto.Nuevo ? "n" : "q";
            return string.Join("\t", PackageId, Tipo, Campo(Texto.Ingles), Campo(Texto.Antes), Campo(Texto.Ahora));
        }

        private static string Campo(string Texto) =>
            (Texto ?? "").Replace("\r", "").Replace("\t", " ").Replace("\n", "\\n");
    }
}
