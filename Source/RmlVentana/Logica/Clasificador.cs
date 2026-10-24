using System;
using System.Collections.Generic;

namespace RmlVentana.Logica
{
    public enum Estado { Traducido, Sobrescrito, SinTraducir }

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
         *  ultima vez que se abrio la ventana, o null si nunca se abrio: ahi ninguno es nuevo. */
        public static List<Fila> Clasificar(IEnumerable<ModActivo> Activos, ICollection<string> TraducidosPorRml,
            ICollection<string> Vistos, string PackageIdRml)
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

                Filas.Add(Fila);
            }

            return Filas;
        }

        /** El idioma del juego por el nombre de su carpeta: el oficial es «SpanishLatin
         *  (Español(Latinoamérica))», y RML usa el nombre corto, que el juego acepta como legado. */
        public static bool EsEspanolLatino(string CarpetaDelIdioma)
        {
            if (CarpetaDelIdioma == null)
                return false;

            return CarpetaDelIdioma == "SpanishLatin" || CarpetaDelIdioma.StartsWith("SpanishLatin (", StringComparison.Ordinal);
        }

        /** Sola, solo si hay novedades y el juego esta en español latinoamericano: a un jugador en
         *  otro idioma RML no le cambia nada. La primera vez cuenta como novedad. */
        public static bool DebeAparecerSola(bool NoVolverAMostrar, bool EnEspanolLatino, ICollection<string> Vistos,
            IEnumerable<Fila> Filas)
        {
            if (NoVolverAMostrar || !EnEspanolLatino)
                return false;

            if (Vistos == null)
                return true;

            foreach (Fila Fila in Filas)
                if (Fila.Nuevo)
                    return true;

            return false;
        }
    }
}
