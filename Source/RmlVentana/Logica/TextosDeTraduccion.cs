#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace RmlVentana.Logica
{
    /** Los textos de los XML de traduccion de un mod y que cambio entre dos versiones: el detalle de
     *  «Ver cambios» (PLAN-VENTANA.md, etapa 5).
     *
     *  Lo usan los dos lados, y por eso vive aca, sin tipos del juego: el builder arma con esto
     *  Cambios.tsv (Novedades.cs) e Ingles.tsv (CopiaLimpia.cs), y el juego compara contra la copia
     *  que guardo en el PC (CopiaDeTextos, PLAN-CAMBIOS-LOCALES.md). El builder lo compila con un
     *  enlace a este archivo, asi que la clave de un texto es la misma en los dos.
     *
     *  Se cruzan por carpeta y no por archivo, porque el extractor puede pasar una clave a otro
     *  archivo sin cambiarla.
     */
    public static class TextosDeTraduccion
    {
        public const string ArchivoDeCambios = "Cambios.tsv";

        public const string Encabezado = "PackageID\tTipo\tInglés\tAntes\tAhora\tInglés antes\tObjeto";

        /** El ingles de cada texto, que en output/ ya no esta: CopiaLimpia saca los comentarios. */
        public const string ArchivoDeIngles = "Ingles.tsv";

        /** Anadido es cada texto de un mod añadido en la subida, para su fila «Añadido» (Novedades.Armar). */
        public enum Tipo { Cambiado, Nuevo, Quitado, Anadido }

        /** Un texto de un XML: su clave unica dentro de la carpeta del mod, el ingles del comentario EN y
         *  la traduccion («TODO» si no la tiene). */
        public class Texto
        {
            public string Clave { get; }
            public string Ingles { get; }
            public string Valor { get; }

            public Texto(string Clave, string Ingles, string Valor)
            {
                this.Clave = Clave;
                this.Ingles = Ingles;
                this.Valor = Valor;
            }
        }

        /** InglesAntes va solo si el ingles cambio: la ventana lo resalta como a la traduccion, y vacio
         *  dice que lo que cambio fue solo la traduccion. Objeto es de que es el texto (ObjetoDe), para
         *  que la ventana lo agrupe por categoria. Dos iguales son el mismo cambio (Comparar). */
        public class Cambio : IEquatable<Cambio>
        {
            public Tipo Tipo { get; }
            public string Ingles { get; }
            public string Antes { get; }
            public string Ahora { get; }
            public string InglesAntes { get; }
            public string Objeto { get; }

            public Cambio(Tipo Tipo, string Ingles, string Antes, string Ahora, string InglesAntes = "", string Objeto = "")
            {
                this.Tipo = Tipo;
                this.Ingles = Ingles;
                this.Antes = Antes;
                this.Ahora = Ahora;
                this.InglesAntes = InglesAntes;
                this.Objeto = Objeto;
            }

            public Cambio ConTipo(Tipo Otro) => new Cambio(Otro, Ingles, Antes, Ahora, InglesAntes, Objeto);

            public bool Equals(Cambio Otro) =>
                Otro != null && Tipo == Otro.Tipo && Ingles == Otro.Ingles && Antes == Otro.Antes && Ahora == Otro.Ahora &&
                InglesAntes == Otro.InglesAntes && Objeto == Otro.Objeto;

            public override bool Equals(object Otro) => Equals(Otro as Cambio);

            public override int GetHashCode()
            {
                unchecked
                {
                    int Hash = (int)Tipo;
                    foreach (string Parte in new[] { Ingles, Antes, Ahora, InglesAntes, Objeto })
                        Hash = Hash * 31 + (Parte?.GetHashCode() ?? 0);
                    return Hash;
                }
            }
        }

        private static readonly Regex LiDelComentario = new Regex(@"<li>(.*?)</li>", RegexOptions.Singleline | RegexOptions.Compiled);

        /** El tipo y el defName del def que modifica un xpath de Patches: el ultimo «Tipo[defName=…]». */
        private static readonly Regex DefDelXpath = new Regex(@"([\w.]+)\[\s*defName\s*=\s*[""']([^""']+)[""']\s*\]", RegexOptions.Compiled);

        private static readonly Regex Separadores = new Regex(@"[\t\r\n]+", RegexOptions.Compiled);

        /** Si un archivo de la carpeta de un mod es de traduccion: lo de Languages/ y Patches/. Relativa
         *  es la ruta dentro de la carpeta del mod, con /. */
        public static bool EsDeTraduccion(string Relativa) =>
            Relativa.EndsWith(".xml", StringComparison.OrdinalIgnoreCase) &&
            (Relativa.StartsWith("Patches/", StringComparison.Ordinal) || Relativa.IndexOf("Languages/", StringComparison.Ordinal) >= 0);

        /** Los textos de un XML de Languages/ o Patches/. Ruta es la del archivo dentro de la carpeta del
         *  mod, con /: de ahi sale el grupo de la clave (Keyed, DefInjected/ThingDef...), porque dos tipos
         *  de def pueden tener la misma clave. Un XML que no se puede leer no trae nada. */
        public static List<Texto> Leer(string Ruta, string Xml)
        {
            var Textos = new List<Texto>();
            XDocument Documento;
            try
            {
                Documento = XDocument.Parse(Xml.TrimStart('﻿'));
            }
            catch (XmlException)
            {
                return Textos;
            }

            if (!(Documento.Root is XElement Raiz))
                return Textos;

            if (Raiz.Name.LocalName == "Patch")
            {
                // Cada operacion con su xpath y su value; lo traducido son los hijos del value.
                foreach (XElement Valor in Raiz.Descendants("value"))
                {
                    if (!(Valor.Parent?.Element("xpath") is XElement Xpath))
                        continue;
                    foreach (XElement Elemento in Valor.Elements())
                        Agregar(Textos, "Patches|" + Xpath.Value.Trim() + "|" + Elemento.Name.LocalName, Elemento);
                }
            }
            else
            {
                string Grupo = GrupoDe(Ruta);
                foreach (XElement Elemento in Raiz.Elements())
                    Agregar(Textos, Grupo + "|" + Elemento.Name.LocalName, Elemento);
            }

            return Textos;
        }

        /** Antes y ahora de una misma carpeta de mod. Primero los cambiados, despues los nuevos y al final
         *  los quitados, cada grupo en el orden de los archivos. */
        public static List<Cambio> Comparar(IEnumerable<Texto> Antes, IEnumerable<Texto> Ahora)
        {
            // Una clave repetida en la misma carpeta la deja el juego en el ultimo archivo que lee; aca
            // alcanza con quedarse con una sola.
            var Viejos = new Dictionary<string, Texto>();
            foreach (Texto Texto in Antes)
                Viejos[Texto.Clave] = Texto;

            var Cambiados = new List<Cambio>();
            var Nuevos = new List<Cambio>();
            var Quitados = new List<Cambio>();
            var Vistas = new HashSet<string>();

            foreach (Texto Nuevo in Ahora)
            {
                if (!Vistas.Add(Nuevo.Clave))
                    continue;

                Viejos.TryGetValue(Nuevo.Clave, out Texto Viejo);
                bool TeniaTexto = Viejo != null && Traducido(Viejo.Valor);

                if (Traducido(Nuevo.Valor))
                {
                    if (!TeniaTexto)
                        Nuevos.Add(new Cambio(Tipo.Nuevo, Nuevo.Ingles, "", Nuevo.Valor, Objeto: ObjetoDe(Nuevo.Clave)));
                    else if (Viejo.Valor != Nuevo.Valor)
                        Cambiados.Add(new Cambio(Tipo.Cambiado, Nuevo.Ingles, Viejo.Valor, Nuevo.Valor,
                            Viejo.Ingles != Nuevo.Ingles && Viejo.Ingles.Length > 0 && Nuevo.Ingles.Length > 0 ? Viejo.Ingles : "",
                            ObjetoDe(Nuevo.Clave)));
                }
                else if (TeniaTexto)
                {
                    Quitados.Add(new Cambio(Tipo.Quitado, Viejo.Ingles, Viejo.Valor, "", Objeto: ObjetoDe(Viejo.Clave)));
                }
            }

            foreach (Texto Viejo in Viejos.Values)
                if (!Vistas.Contains(Viejo.Clave) && Traducido(Viejo.Valor))
                    Quitados.Add(new Cambio(Tipo.Quitado, Viejo.Ingles, Viejo.Valor, "", Objeto: ObjetoDe(Viejo.Clave)));

            // El mismo texto en dos carpetas de version (1.6/ y 1.7/) es para el jugador un solo cambio.
            return Cambiados.Concat(Nuevos).Concat(Quitados).Distinct().ToList();
        }

        /** Una linea de Cambios.tsv. Un salto de linea o un tabulador dentro de un texto romperia la fila. */
        public static string Fila(string PackageId, Cambio Cambio)
        {
            string Marca = Cambio.Tipo == Tipo.Cambiado ? "c" : Cambio.Tipo == Tipo.Nuevo ? "n" : Cambio.Tipo == Tipo.Anadido ? "a" : "q";
            return string.Join("\t", PackageId, Marca, Limpio(Cambio.Ingles), Limpio(Cambio.Antes), Limpio(Cambio.Ahora), Limpio(Cambio.InglesAntes), Limpio(Cambio.Objeto));
        }

        private const string MarcaDeCarpeta = "#\t";

        /** Lo de un mod en Ingles.tsv: «#», la carpeta en RML («Data/!Autor/Mod - 123») y cuantos textos
         *  tiene, y despues el ingles de cada uno, en el orden de Leer sobre los archivos ordenados por su
         *  ruta. Sin la clave, que pesaba mas que el ingles: con ella el archivo eran 18 MB y asi son unos
         *  6. Va por carpeta y no por packageId, porque dos carpetas pueden compartirlo (The Dead Man's
         *  Switch, una para 1.6 y otra para 1.7). Un ingles no lleva tabuladores (Limpio), asi que no se
         *  confunde con la marca. */
        public static IEnumerable<string> LineasDeIngles(string Carpeta, IReadOnlyCollection<Texto> Textos)
        {
            yield return MarcaDeCarpeta + Carpeta + "\t" + Textos.Count;
            foreach (Texto Texto in Textos)
                yield return Limpio(Texto.Ingles);
        }

        /** El ingles de las carpetas pedidas, leyendo Ingles.tsv de a una linea: lo de los demas mods se
         *  salta. Una carpeta cuyo numero de textos no coincide con su encabezado no viene. */
        public static Dictionary<string, List<string>> LeerIngles(IEnumerable<string> Lineas, ICollection<string> Carpetas)
        {
            var Pedidas = new HashSet<string>(Carpetas);
            var Resultado = new Dictionary<string, List<string>>();
            string Actual = null;
            int Esperados = 0;
            List<string> DeLaCarpeta = null;

            void Cerrar()
            {
                if (DeLaCarpeta != null && DeLaCarpeta.Count == Esperados)
                    Resultado[Actual] = DeLaCarpeta;
                DeLaCarpeta = null;
            }

            foreach (string Cruda in Lineas)
            {
                string Linea = Cruda.TrimEnd('\r');
                if (Linea.StartsWith(MarcaDeCarpeta, StringComparison.Ordinal))
                {
                    Cerrar();
                    int Tab = Linea.LastIndexOf('\t');
                    Actual = Tab > 1 ? Linea.Substring(2, Tab - 2) : "";
                    if (Pedidas.Contains(Actual) && int.TryParse(Linea.Substring(Tab + 1), out Esperados))
                        DeLaCarpeta = new List<string>();
                    continue;
                }
                DeLaCarpeta?.Add(Linea);
            }
            Cerrar();

            return Resultado;
        }

        /** Los mismos textos con el ingles de Ingles.tsv, para los XML de output/, que no lo traen. Si la
         *  cuenta no coincide, quedan como estan: un ingles corrido de lugar seria peor que ninguno. */
        public static List<Texto> ConIngles(IEnumerable<Texto> Textos, IList<string> Ingles)
        {
            var Lista = Textos.ToList();
            if (Ingles == null || Ingles.Count != Lista.Count)
                return Lista;
            return Lista.Select((x, i) => new Texto(x.Clave, Ingles[i], x.Valor)).ToList();
        }

        /** «ThingDef/Gun_LMG» de una clave de Leer: el tipo de def y su defName, «Keyed» si es un texto
         *  de la interfaz, o vacio si no se sabe (un Patches que no apunta a un defName). La carpeta de
         *  version no entra: el mismo texto en 1.6/ y 1.7/ es el mismo objeto. */
        public static string ObjetoDe(string Clave)
        {
            string[] Partes = Clave.Split('|');
            if (Partes[0] == "Patches")
            {
                Match Def = Partes.Length > 1 ? DefDelXpath.Matches(Partes[1]).Cast<Match>().LastOrDefault() : null;
                return Def != null ? Def.Groups[1].Value + "/" + Def.Groups[2].Value : "";
            }

            string[] Grupo = Partes[0].Split('/');
            if (Grupo[Grupo.Length - 1] == "Keyed")
                return "Keyed";
            if (Grupo.Length >= 2 && Grupo[Grupo.Length - 2] == "DefInjected" && Partes.Length > 1)
                return Grupo[Grupo.Length - 1] + "/" + Partes[1].Split('.')[0];
            return "";
        }

        /** El texto en una sola linea y sin tabuladores, como va en los .tsv. */
        public static string Limpio(string Texto) => Separadores.Replace(Texto ?? "", " ").Trim();

        public static bool Traducido(string Valor) => Valor.Length > 0 && Valor != "TODO";

        /** Un elemento, o uno por cada <li> si es una lista: una regla de gramatica corregida no tiene que
         *  mostrar el bloque entero. El ingles de cada <li> sale del <li> del mismo numero en el comentario. */
        private static void Agregar(List<Texto> Textos, string Clave, XElement Elemento)
        {
            string Ingles = Elemento.PreviousNode is XComment Comentario ? Comentario.Value.Trim() : "";
            if (Ingles.StartsWith("EN:", StringComparison.Ordinal))
                Ingles = Ingles.Substring(3).Trim();
            // Hay mods que dejan «TODO» como texto en ingles: mostrarlo confundiria con uno sin traducir.
            if (Ingles == "TODO")
                Ingles = "";

            var Lista = Elemento.Elements("li").ToList();
            // Un comentario no decodifica nada: «tier&apos;s» llega asi y se veria asi en la ventana.
            if (Lista.Count == 0)
            {
                Textos.Add(new Texto(Clave, WebUtility.HtmlDecode(Ingles), Elemento.Value.Trim()));
                return;
            }

            var InglesDeLaLista = LiDelComentario.Matches(Ingles).Cast<Match>().Select(x => WebUtility.HtmlDecode(x.Groups[1].Value.Trim())).ToList();
            for (int i = 0; i < Lista.Count; i++)
                Textos.Add(new Texto(Clave + "[" + i + "]", i < InglesDeLaLista.Count ? InglesDeLaLista[i] : "", Lista[i].Value.Trim()));
        }

        /** «Keyed» o «DefInjected/ThingDef», con lo que haya antes (una carpeta de version, por ejemplo). */
        private static string GrupoDe(string Ruta)
        {
            string[] Partes = Ruta.Split('/');
            int i = Array.IndexOf(Partes, "DefInjected");
            if (i >= 0 && i + 1 < Partes.Length - 1)
                return string.Join("/", Partes.Take(i + 2));
            i = Array.IndexOf(Partes, "Keyed");
            if (i >= 0)
                return string.Join("/", Partes.Take(i + 1));
            return string.Join("/", Partes.Take(Partes.Length - 1));
        }
    }
}
