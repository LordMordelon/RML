using System;
using System.Collections.Generic;
using System.Linq;

namespace RmlVentana.Logica
{
    /** SinTexto: se pidio y no tiene nada que traducir (SinTexto.tsv, que anota solicitud.yml). */
    public enum Estado { Traducido, Sobrescrito, SinTraducir, SinTexto }

    /** Un mod activo, como lo deja LecturaDelJuego. Sin tipos del juego, para probarlo sin abrirlo. */
    public class ModActivo
    {
        /** En minusculas y sin el sufijo _steam. */
        public string PackageId;
        public string Nombre;
        public string WorkshopId;
        public bool Oficial;
        /** Tiene una carpeta Languages/SpanishLatin en lo que el juego carga de el. */
        public bool TieneEspanol;
        /** Solo trae traducciones: ni Defs ni Assemblies. */
        public bool EsPaqueteDeTraduccion;
        /** Si es paquete: los mods a los que apunta. */
        public List<string> Traduce = new List<string>();
    }

    public class Fila
    {
        public ModActivo Mod;
        public Estado Estado;
        public bool PorRml;
        public bool IncluidaEnElMod;
        public List<string> Otros = new List<string>();
        public bool Nuevo;
    }

    /** Que mods activos traduce RML, cuales otro y cuales nadie (la tabla de PLAN-VENTANA.md). */
    public static class Clasificador
    {
        /** Una fila por mod activo, en el orden de la lista de mods. Quedan afuera Core y los DLC, RML
         *  y los paquetes de traduccion: no son mods para traducir. Salvo que RML lo traduzca: un mod de
         *  solo Patches con su propia traduccion parece un paquete y no lo es. Vistos es la lista de la
         *  ultima vez que se abrio la ventana, o null si nunca se abrio: ahi ninguno es nuevo. SinTexto son
         *  los numeros del Workshop de LeerSinTexto, y solo cambian a los que nadie traduce. */
        public static List<Fila> Clasificar(IEnumerable<ModActivo> Activos, ICollection<string> TraducidosPorRml,
            ICollection<string> Vistos, string PackageIdRml, ICollection<string> SinTexto = null)
        {
            var Lista = new List<ModActivo>(Activos);
            var Filas = new List<Fila>();

            foreach (ModActivo Mod in Lista)
            {
                if (Mod.Oficial || Mod.PackageId == PackageIdRml ||
                    (Mod.EsPaqueteDeTraduccion && !TraducidosPorRml.Contains(Mod.PackageId)))
                    continue;

                var Fila = new Fila
                {
                    Mod = Mod,
                    PorRml = TraducidosPorRml.Contains(Mod.PackageId),
                    IncluidaEnElMod = Mod.TieneEspanol,
                    Nuevo = Vistos != null && !Vistos.Contains(Mod.PackageId),
                };

                foreach (ModActivo Otro in Lista)
                {
                    if (Otro != Mod && Otro.PackageId != PackageIdRml && Otro.EsPaqueteDeTraduccion &&
                        Otro.Traduce.Contains(Mod.PackageId))
                        Fila.Otros.Add(Otro.Nombre);
                }

                bool OtraTraduccion = Fila.IncluidaEnElMod || Fila.Otros.Count > 0;
                Fila.Estado = Fila.PorRml
                    ? OtraTraduccion ? Estado.Sobrescrito : Estado.Traducido
                    : OtraTraduccion ? Estado.Traducido : Estado.SinTraducir;
                if (Fila.Estado == Estado.SinTraducir && SinTexto != null && Mod.WorkshopId != null &&
                    SinTexto.Contains(Mod.WorkshopId))
                    Fila.Estado = Estado.SinTexto;

                Filas.Add(Fila);
            }

            return Filas;
        }

        /** Los numeros del Workshop de SinTexto.tsv, que el builder copia de la rama «solicitudes» a
         *  output/. Va por numero y no por packageId: dos mods distintos pueden compartirlo (The Dead
         *  Man's Switch y su II). Sin archivo, o con una linea que no se entiende, no hay ninguno. */
        public static HashSet<string> LeerSinTexto(string Texto)
        {
            var Ids = new HashSet<string>();
            if (Texto == null)
                return Ids;

            foreach (string Linea in Texto.Split('\n'))
            {
                string Id = Linea.TrimEnd('\r').Split('\t')[0].Trim();
                if (Id.Length > 0 && Id.All(char.IsDigit))
                    Ids.Add(Id);
            }

            return Ids;
        }

        /** El idioma del juego por el nombre de su carpeta: el oficial es «SpanishLatin
         *  (Español(Latinoamérica))», y RML usa el nombre corto, que el juego acepta como legado. */
        public static bool EsEspanolLatino(string CarpetaDelIdioma)
        {
            if (CarpetaDelIdioma == null)
                return false;

            return CarpetaDelIdioma == "SpanishLatin" || CarpetaDelIdioma.StartsWith("SpanishLatin (", StringComparison.Ordinal);
        }

        /** Sola, solo si el juego esta en español latinoamericano (a un jugador en otro idioma RML no le
         *  cambia nada) y la pestaña «Novedades» tiene algo sin ver de un mod activo, o la traduccion
         *  oficial recien instalada o deshecha. Pendientes y YaVistas son los de NovedadesRml.ParaMostrar;
         *  la primera vez, eso es la ultima actualizacion de RML, y si no trajo cambios no aparece.
         *  Activos son los packageId de los mods de RML con su mod activo (todos los de su carpeta).
         *
         *  Un mod nuevo en la lista no la abre (Angel, 25-oct): al activarlo el juego se reinicia, y la
         *  ventana salia con «Novedades» vacia. Lo nuevo se marca en «Tus mods». Un cambio en un mod que
         *  no esta activo sale en la tabla, pero tampoco la abre. */
        public static bool DebeAparecerSola(bool NoVolverAMostrar, bool EnEspanolLatino, IEnumerable<Novedad> Pendientes,
            bool YaVistas, ICollection<string> Activos)
        {
            if (NoVolverAMostrar || !EnEspanolLatino)
                return false;

            return Pendientes.Any(x => x.Oficial || (!YaVistas && Activos.Contains(x.PackageId)));
        }
    }
}
