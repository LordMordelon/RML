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

        /** yyyy-MM-dd de la ultima vez que se cerro la ventana, para el «Cambios desde el…». */
        public string UltimaVez;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref NoVolverAMostrar, "noVolverAMostrar");
            Scribe_Collections.Look(ref Vistos, "vistos", LookMode.Value);
            Scribe_Collections.Look(ref NovedadesVistas, "novedadesVistas", LookMode.Value);
            Scribe_Values.Look(ref UltimaVez, "ultimaVez");
            Scribe_Collections.Look(ref Solicitudes, "solicitudes", LookMode.Value, LookMode.Value);

            if (Scribe.mode == LoadSaveMode.PostLoadInit && Solicitudes == null)
                Solicitudes = new Dictionary<string, string>();
        }
    }
}
