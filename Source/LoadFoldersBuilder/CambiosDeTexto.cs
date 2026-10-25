using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace LoadFoldersBuilder;

/** Cambios.tsv: que textos cambiaron en cada mod desde la ultima subida de RML al Workshop, para el
 *  boton «Ver cambios» de la ventana del juego (PLAN-VENTANA.md, etapa 5). Como Novedades.tsv, no se
 *  commitea: lo arma Novedades.Armar y va solo en la copia de output/.
 *
 *  Aca esta solo la parte que no necesita git: leer los textos de un XML de traduccion y cruzar los
 *  de antes con los de ahora. Se cruzan por carpeta y no por archivo, porque el extractor puede pasar
 *  una clave a otro archivo sin cambiarla.
 */
public static class CambiosDeTexto
{
    public const string Archivo = "Cambios.tsv";

    /** Cuantos textos por mod van al archivo: una actualizacion grande (Hegemony) traeria miles. Los
     *  totales van igual en Novedades.tsv, para el «y N mas». */
    public const int Tope = 200;

    public const string Encabezado = "PackageID\tTipo\tInglés\tAntes\tAhora\tInglés antes\tObjeto";

    /** Anadido es cada texto de un mod añadido en la subida, para su fila «Añadido» (Novedades.Armar). */
    public enum Tipo { Cambiado, Nuevo, Quitado, Anadido }

    /** Un texto de un XML: su clave unica dentro de la carpeta del mod, el ingles del comentario EN y
     *  la traduccion («TODO» si no la tiene). */
    public record Texto(string Clave, string Ingles, string Valor);

    /** InglesAntes va solo si el ingles cambio: la ventana lo resalta como a la traduccion, y vacio
     *  dice que lo que cambio fue solo la traduccion. Objeto es de que es el texto (ObjetoDe), para
     *  que la ventana lo agrupe por categoria. */
    public record Cambio(Tipo Tipo, string Ingles, string Antes, string Ahora, string InglesAntes = "", string Objeto = "");

    private static readonly Regex LiDelComentario = new(@"<li>(.*?)</li>", RegexOptions.Singleline | RegexOptions.Compiled);

    /** El tipo y el defName del def que modifica un xpath de Patches: el ultimo «Tipo[defName=…]». */
    private static readonly Regex DefDelXpath = new(@"([\w.]+)\[\s*defName\s*=\s*[""']([^""']+)[""']\s*\]", RegexOptions.Compiled);

    private static readonly Regex Separadores = new(@"[\t\r\n]+", RegexOptions.Compiled);

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

        if (Documento.Root is not { } Raiz)
            return Textos;

        if (Raiz.Name.LocalName == "Patch")
        {
            // Cada operacion con su xpath y su value; lo traducido son los hijos del value.
            foreach (XElement Valor in Raiz.Descendants("value"))
            {
                if (Valor.Parent?.Element("xpath") is not { } Xpath)
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

            Viejos.TryGetValue(Nuevo.Clave, out Texto? Viejo);
            bool TeniaTexto = Viejo is not null && Traducido(Viejo.Valor);

            if (Traducido(Nuevo.Valor))
            {
                if (!TeniaTexto)
                    Nuevos.Add(new Cambio(Tipo.Nuevo, Nuevo.Ingles, "", Nuevo.Valor, Objeto: ObjetoDe(Nuevo.Clave)));
                else if (Viejo!.Valor != Nuevo.Valor)
                    Cambiados.Add(new Cambio(Tipo.Cambiado, Nuevo.Ingles, Viejo.Valor, Nuevo.Valor,
                        Viejo.Ingles != Nuevo.Ingles && Viejo.Ingles.Length > 0 && Nuevo.Ingles.Length > 0 ? Viejo.Ingles : "",
                        ObjetoDe(Nuevo.Clave)));
            }
            else if (TeniaTexto)
            {
                Quitados.Add(new Cambio(Tipo.Quitado, Viejo!.Ingles, Viejo.Valor, "", Objeto: ObjetoDe(Viejo.Clave)));
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
        string Marca = Cambio.Tipo switch { Tipo.Cambiado => "c", Tipo.Nuevo => "n", Tipo.Anadido => "a", _ => "q" };
        return string.Join("\t", PackageId, Marca, Limpio(Cambio.Ingles), Limpio(Cambio.Antes), Limpio(Cambio.Ahora), Limpio(Cambio.InglesAntes), Limpio(Cambio.Objeto));
    }

    /** «ThingDef/Gun_LMG» de una clave de Leer: el tipo de def y su defName, «Keyed» si es un texto
     *  de la interfaz, o vacio si no se sabe (un Patches que no apunta a un defName). La carpeta de
     *  version no entra: el mismo texto en 1.6/ y 1.7/ es el mismo objeto. */
    public static string ObjetoDe(string Clave)
    {
        string[] Partes = Clave.Split('|');
        if (Partes[0] == "Patches")
        {
            Match Def = Partes.Length > 1 ? DefDelXpath.Matches(Partes[1]).LastOrDefault() ?? Match.Empty : Match.Empty;
            return Def.Success ? Def.Groups[1].Value + "/" + Def.Groups[2].Value : "";
        }

        string[] Grupo = Partes[0].Split('/');
        if (Grupo[^1] == "Keyed")
            return "Keyed";
        if (Grupo.Length >= 2 && Grupo[^2] == "DefInjected" && Partes.Length > 1)
            return Grupo[^1] + "/" + Partes[1].Split('.')[0];
        return "";
    }

    private static string Limpio(string Texto) => Separadores.Replace(Texto, " ").Trim();

    private static bool Traducido(string Valor) => Valor.Length > 0 && Valor != "TODO";

    /** Un elemento, o uno por cada <li> si es una lista: una regla de gramatica corregida no tiene que
     *  mostrar el bloque entero. El ingles de cada <li> sale del <li> del mismo numero en el comentario. */
    private static void Agregar(List<Texto> Textos, string Clave, XElement Elemento)
    {
        string Ingles = Elemento.PreviousNode is XComment Comentario ? Comentario.Value.Trim() : "";
        if (Ingles.StartsWith("EN:", StringComparison.Ordinal))
            Ingles = Ingles[3..].Trim();
        // Hay mods que dejan «TODO» como texto en ingles: mostrarlo confundiria con uno sin traducir.
        if (Ingles == "TODO")
            Ingles = "";

        var Lista = Elemento.Elements("li").ToList();
        if (Lista.Count == 0)
        {
            Textos.Add(new Texto(Clave, Ingles, Elemento.Value.Trim()));
            return;
        }

        var InglesDeLaLista = LiDelComentario.Matches(Ingles).Select(x => x.Groups[1].Value.Trim()).ToList();
        for (int i = 0; i < Lista.Count; i++)
            Textos.Add(new Texto(Clave + "[" + i + "]", i < InglesDeLaLista.Count ? InglesDeLaLista[i] : "", Lista[i].Value.Trim()));
    }

    /** «Keyed» o «DefInjected/ThingDef», con lo que haya antes (una carpeta de version, por ejemplo). */
    private static string GrupoDe(string Ruta)
    {
        string[] Partes = Ruta.Split('/');
        int i = Array.IndexOf(Partes, "DefInjected");
        if (i >= 0 && i + 1 < Partes.Length - 1)
            return string.Join("/", Partes[..(i + 2)]);
        i = Array.IndexOf(Partes, "Keyed");
        if (i >= 0)
            return string.Join("/", Partes[..(i + 1)]);
        return string.Join("/", Partes[..^1]);
    }
}
