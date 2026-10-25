using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace RmlVentana.Logica
{
    public enum EstadoOficial { SinConsulta, AlDia, HayArreglos, FaltaReiniciar, SinUsar, OtraVersion }

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

        /** 1.7.4982 de «versión 1.7.4982»: la compilacion del juego para la que esta hecha la traduccion. */
        public static Version VersionCompletaDelReadme(string Readme)
        {
            if (Readme == null)
                return null;
            Match Numero = Regex.Match(Readme, @"\b(\d+)\.(\d+)\.(\d+)");
            return Numero.Success
                ? new Version(int.Parse(Numero.Groups[1].Value), int.Parse(Numero.Groups[2].Value), int.Parse(Numero.Groups[3].Value))
                : null;
        }

        /** Un aviso aproximado para quien no recibe las actualizaciones solo: Ludeon no publica cual es la
         *  ultima version, pero la traduccion oficial dice para cual esta hecha, y no puede ser de una
         *  que no salio. Si el juego es anterior, salio al menos esa. Sirve solo hacia un lado: el
         *  README a veces va atrasado, y un juego mas nuevo no dice nada. La revision no cuenta. */
        public static bool JuegoAtrasado(Version Juego, Version DelReadme) =>
            DelReadme != null && DelReadme > new Version(Juego.Major, Juego.Minor, Math.Max(0, Juego.Build));

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
         *  (sus archivos la traen adentro); Instalado, la del cambio que se bajo con el boton.
         *  MismaVersion, si el README de GitHub es de la version principal del juego. */
        public static EstadoOficial EstadoDe(DateTime? GitHub, DateTime? DelJuego, DateTime? Instalado, bool TarPresente, bool FaltaReiniciar,
                                             bool MismaVersion = true)
        {
            if (FaltaReiniciar)
                return EstadoOficial.FaltaReiniciar;

            // Con su .tar de vuelta, el juego no mira la carpeta (AbstractFilesystem.GetDirectory).
            if (Instalado != null && TarPresente)
                return EstadoOficial.SinUsar;

            if (GitHub == null)
                return EstadoOficial.SinConsulta;

            if (!MismaVersion)
                return EstadoOficial.OtraVersion;

            DateTime? Actual = TarPresente ? DelJuego : Instalado;
            return Actual == null || GitHub > Actual ? EstadoOficial.HayArreglos : EstadoOficial.AlDia;
        }

        /** Del mas urgente al menos: lo que pide hacer algo va antes de lo que solo informa. */
        private static readonly EstadoOficial[] Urgencia =
        {
            EstadoOficial.FaltaReiniciar, EstadoOficial.SinUsar, EstadoOficial.SinConsulta,
            EstadoOficial.OtraVersion, EstadoOficial.HayArreglos, EstadoOficial.AlDia,
        };

        /** El estado de todo a partir del de cada parte (Core y cada DLC), que se compara con su propio
         *  .tar: un DLC copiado de otra instalacion puede traer una traduccion mas vieja que la del
         *  juego base. Basta una parte atrasada para que haya arreglos. */
        public static EstadoOficial Combinar(IEnumerable<EstadoOficial> Partes)
        {
            var Lista = Partes.ToList();
            foreach (EstadoOficial Estado in Urgencia)
                if (Lista.Contains(Estado))
                    return Estado;
            return EstadoOficial.SinConsulta;
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
                // Los archivos oficiales empiezan con la marca de orden de bytes, y LoadXml no la acepta.
                Documento.LoadXml(Contenido.TrimStart('﻿'));
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
                    // Un comentario no decodifica nada: «&apos;» llega asi y se veria asi en la ventana.
                    Ingles = Texto.StartsWith("EN:") ? System.Net.WebUtility.HtmlDecode(Texto.Substring(3).Trim()) : "";
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
                    Resultado.Add(new TextoCambiado { Tipo = TipoDeTexto.Nuevo, Ingles = Par.Value.Ingles, Antes = "", Ahora = Par.Value.Texto, Objeto = Categorias.ObjetoDeClaveOficial(Par.Key) });
                else if (Anterior.Texto != Par.Value.Texto)
                    Resultado.Add(new TextoCambiado
                    {
                        Tipo = TipoDeTexto.Cambiado,
                        Objeto = Categorias.ObjetoDeClaveOficial(Par.Key),
                        Ingles = Par.Value.Ingles != "" ? Par.Value.Ingles : Anterior.Ingles,
                        Antes = Anterior.Texto,
                        Ahora = Par.Value.Texto,
                        InglesAntes = Par.Value.Ingles != "" && Anterior.Ingles != "" && Anterior.Ingles != Par.Value.Ingles ? Anterior.Ingles : "",
                    });
            }

            foreach (var Par in Viejo)
                if (!Nuevo.ContainsKey(Par.Key))
                    Resultado.Add(new TextoCambiado { Tipo = TipoDeTexto.Quitado, Ingles = Par.Value.Ingles, Antes = Par.Value.Texto, Ahora = "", Objeto = Categorias.ObjetoDeClaveOficial(Par.Key) });

            return Resultado;
        }

        /** Una linea con el formato de Cambios.tsv, para que «Ver cambios» la lea con CambiosDeMod.Leer.
         *  El salto de linea va como «\n» literal, que es como lo escriben las traducciones. */
        public static string Linea(string PackageId, TextoCambiado Texto)
        {
            string Tipo = Texto.Tipo == TipoDeTexto.Cambiado ? "c" : Texto.Tipo == TipoDeTexto.Nuevo ? "n" : "q";
            return string.Join("\t", PackageId, Tipo, Campo(Texto.Ingles), Campo(Texto.Antes), Campo(Texto.Ahora), Campo(Texto.InglesAntes), Campo(Texto.Objeto));
        }

        private static string Campo(string Texto) =>
            (Texto ?? "").Replace("\r", "").Replace("\t", " ").Replace("\n", "\\n");

        /** El prefijo de las claves de un archivo de la carpeta del idioma, o null si no se compara. Los
         *  Keyed son claves sueltas, sin importar la subcarpeta; los DefInjected van por tipo de def. */
        public static string PrefijoDe(string RutaRelativa)
        {
            string[] Partes = RutaRelativa.Replace('\\', '/').Split('/');
            if (Partes.Length < 2 || !Partes[Partes.Length - 1].EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                return null;
            if (Partes[0] == "Keyed")
                return "Keyed/";
            if (Partes[0] == "DefInjected" && Partes.Length >= 3)
                return "DefInjected/" + Partes[1] + "/";
            return null;
        }

        /** El codigo y la fecha del ultimo cambio, de la respuesta de api.github.com/repos/…/commits/master.
         *  Sin un lector de JSON en .NET Framework, va por expresiones: el primer «sha» es el del cambio
         *  (los de «parents» vienen despues) y la fecha es la del «committer», la de cuando entro a master. */
        public static bool LeerCommit(string Json, out string Sha, out DateTime Fecha)
        {
            Sha = null;
            Fecha = default;
            if (Json == null)
                return false;

            Match Codigo = Regex.Match(Json, "\"sha\"\\s*:\\s*\"([0-9a-f]{40})\"");
            Match Cuando = Regex.Match(Json, "\"committer\"\\s*:\\s*\\{[^{}]*?\"date\"\\s*:\\s*\"([^\"]+)\"");
            if (!Codigo.Success || !Cuando.Success ||
                !DateTime.TryParse(Cuando.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal, out Fecha))
                return false;

            Sha = Codigo.Groups[1].Value;
            return true;
        }

        /** La carpeta que envuelve todo el zip de GitHub («RimWorld-SpanishLatin-<codigo>/»), o "" si no hay. */
        public static string RaizDelZip(IEnumerable<string> Entradas)
        {
            string Raiz = null;
            foreach (string Entrada in Entradas)
            {
                int Barra = Entrada.IndexOf('/');
                string Primera = Barra < 0 ? null : Entrada.Substring(0, Barra + 1);
                if (Primera == null || (Raiz != null && Raiz != Primera))
                    return "";
                Raiz = Primera;
            }
            return Raiz ?? "";
        }

        /** Que una ruta del zip no se salga de la carpeta donde se extrae. */
        public static bool RutaSegura(string Ruta)
        {
            string Normal = Ruta.Replace('\\', '/');
            return !Normal.StartsWith("/") && !Normal.Contains(":") && !Normal.Split('/').Contains("..");
        }
    }

    /** Una fila de la traduccion oficial en «Novedades», guardada en los ajustes entre la instalacion y
     *  el reinicio: «carpeta|yyyy-MM-dd|cambiados|nuevos|quitados». */
    public static class NovedadOficial
    {
        public static string Linea(string Carpeta, string Fecha, int Cambiados, int Nuevos, int Quitados) =>
            string.Join("|", Carpeta, Fecha, Cambiados, Nuevos, Quitados);

        public static string CarpetaDe(string Linea) => Linea?.Split('|')[0];

        /** Archivo es el de los textos cambiados, que va en la configuracion y no en la carpeta de RML. */
        public static Novedad Leer(string Linea, string Nombre, string Archivo)
        {
            string[] Campos = Linea?.Split('|');
            if (Campos == null || Campos.Length < 5 ||
                !int.TryParse(Campos[2], out int Cambiados) || !int.TryParse(Campos[3], out int Nuevos) || !int.TryParse(Campos[4], out int Quitados))
                return null;

            return new Novedad
            {
                PackageId = TraduccionOficial.PackageIdDe(Campos[0]),
                Nombre = Nombre,
                Cambio = Cambio.Actualizado,
                Fecha = Campos[1],
                Cambiados = Cambiados,
                Nuevos = Nuevos,
                Quitados = Quitados,
                Oficial = true,
                Archivo = Archivo,
            };
        }
    }
}
