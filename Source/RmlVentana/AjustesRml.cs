using System.Collections.Generic;
using Verse;

namespace RmlVentana
{
    /** Lo que RML recuerda entre partidas, en el archivo de ajustes que el juego guarda en Config/. */
    public class AjustesRml : ModSettings
    {
        /** La casilla «No volver a mostrar». La ventana se sigue abriendo a mano desde los ajustes. */
        public bool NoVolverAMostrar;

        /** Los mods de la tabla la ultima vez que se cerro la ventana, para marcar los nuevos. Null
         *  si nunca se abrio: la primera vez aparece sola y ninguno sale como nuevo. */
        public List<string> Vistos;

        /** packageId -> fecha (yyyy-MM-dd) de cada «Solicitar». Se borra sola cuando el mod entra a RML. */
        public Dictionary<string, string> Solicitudes = new Dictionary<string, string>();

        /** Los cambios de Novedades.tsv que ya se mostraron (NovedadesRml.Claves). Null si nunca. */
        public List<string> NovedadesVistas;

        /** Lo que mostro la pestaña la ultima vez que tuvo algo nuevo (NovedadesRml.ClavesDe), para
         *  volver a mostrar lo mismo al abrirla de nuevo. */
        public List<string> NovedadesMostradas;

        /** yyyy-MM-dd de la ultima vez que se cerro la ventana, para el «Cambios desde el…». */
        public string UltimaVez;

        /** «Ver cambios» con cada texto en una sola linea en vez de ajustado al ancho (Alt+Z). */
        public bool CambiosEnUnaLinea;

        /** «Ver cambios» agrupado por categoria (armas, torretas…) en vez de en el orden del archivo. */
        public bool CambiosPorCategoria;

        /** «Revisar si hay arreglos de la traducción oficial» (PLAN-TRADUCCION-OFICIAL.md). Apagada, RML
         *  no le consulta nada a GitHub salvo con «Revisar ahora». */
        public bool RevisarOficial = true;

        /** El cambio de GitHub que se instalo con el boton: su codigo y su fecha (UTC, formato «o»).
         *  Null si no hay nada instalado. */
        public string OficialSha;
        public string OficialFecha;

        /** Las carpetas de Data/ donde se instalo («Core», «Biotech»…), para que «Deshacer» toque solo esas. */
        public List<string> OficialCarpetas = new List<string>();

        /** Las filas de Novedades de la ultima instalacion (NovedadOficial.Linea). Salen despues de
         *  reiniciar y se borran al cerrar la ventana. */
        public List<string> OficialNovedades = new List<string>();

        /** «instalar» o «deshacer» mientras falta reiniciar para que haga efecto; null si no. Lo limpia
         *  AlIniciar en el siguiente inicio del juego. */
        public string OficialReiniciar;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref RevisarOficial, "revisarOficial", true);
            Scribe_Values.Look(ref OficialSha, "oficialSha");
            Scribe_Values.Look(ref OficialFecha, "oficialFecha");
            Scribe_Collections.Look(ref OficialCarpetas, "oficialCarpetas", LookMode.Value);
            Scribe_Collections.Look(ref OficialNovedades, "oficialNovedades", LookMode.Value);
            Scribe_Values.Look(ref OficialReiniciar, "oficialReiniciar");
            Scribe_Values.Look(ref NoVolverAMostrar, "noVolverAMostrar");
            Scribe_Collections.Look(ref Vistos, "vistos", LookMode.Value);
            Scribe_Collections.Look(ref NovedadesVistas, "novedadesVistas", LookMode.Value);
            Scribe_Collections.Look(ref NovedadesMostradas, "novedadesMostradas", LookMode.Value);
            Scribe_Values.Look(ref UltimaVez, "ultimaVez");
            Scribe_Values.Look(ref CambiosEnUnaLinea, "cambiosEnUnaLinea", false);
            Scribe_Values.Look(ref CambiosPorCategoria, "cambiosPorCategoria", false);
            Scribe_Collections.Look(ref Solicitudes, "solicitudes", LookMode.Value, LookMode.Value);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (Solicitudes == null)
                    Solicitudes = new Dictionary<string, string>();
                if (OficialCarpetas == null)
                    OficialCarpetas = new List<string>();
                if (OficialNovedades == null)
                    OficialNovedades = new List<string>();
            }
        }
    }
}
