using System;
using Verse;

namespace RmlVentana
{
    /** La entrada de RML en el juego. Por ahora solo deja una linea en el log, para comprobar que
     *  la DLL carga desde Comun/ en 1.6 y en 1.7; la ventana de mods traducidos llega despues
     *  (PLAN-VENTANA.md).
     *
     *  Todo va dentro de un try: un error aca le llenaria el log a cada jugador de RML, y las
     *  traducciones cargan igual sin la DLL.
     */
    public class RmlVentanaMod : Mod
    {
        public RmlVentanaMod(ModContentPack content) : base(content)
        {
            try
            {
                Log.Message("[Traducciones [Español Latino]] Cargado.");
            }
            catch (Exception)
            {
            }
        }
    }
}
