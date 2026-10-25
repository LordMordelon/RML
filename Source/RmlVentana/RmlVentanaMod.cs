using System;
using UnityEngine;
using Verse;

namespace RmlVentana
{
    /** La entrada de RML en el juego: la ventana de mods traducidos al llegar al menu principal, y la
     *  misma ventana en su pestaña «Ajustes» desde los ajustes de mods del juego (PLAN-VENTANA.md).
     *
     *  Todo va dentro de un try: un error aca le llenaria el log a cada jugador de RML, y las
     *  traducciones cargan igual sin la DLL. Si algo falla, la ventana simplemente no aparece.
     */
    public class RmlVentanaMod : Mod
    {
        public static RmlVentanaMod Instancia { get; private set; }

        public AjustesRml Ajustes { get; }

        public RmlVentanaMod(ModContentPack content) : base(content)
        {
            Instancia = this;
            Ajustes = GetSettings<AjustesRml>();
            // Mientras carga el juego, para que este listo al abrir la ventana.
            CambiosEnElPc.Empezar(content);
        }

        public override string SettingsCategory() => "RML_Ventana_Titulo".Translate();

        /** Los ajustes van en la pestaña «Ajustes» de la ventana de RML, junto a las demas: desde la lista
         *  de mods del juego se abre la ventana en esa pestaña y se cierra la de ajustes, que quedaria
         *  vacia detras. */
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Find.WindowStack.TryRemove(typeof(RimWorld.Dialog_ModSettings), doCloseSound: false);
            if (VentanaTraducciones.Crear(out _) is VentanaTraducciones Ventana)
            {
                Ventana.AbrirEnAjustes();
                Find.WindowStack.Add(Ventana);
            }
        }
    }

    /** Corre con el juego ya cargado: la ventana aparece sola si corresponde (Clasificador.DebeAparecerSola).
     *
     *  Va en un evento largo propio y no en ExecuteWhenFinished: el juego arma la pantalla del menu en
     *  otro evento, «InitializingInterface», que corre despues de cargar los datos. Con
     *  ExecuteWhenFinished la ventana se abria antes de eso y no habia donde ponerla (Find.WindowStack
     *  era null). Un evento encolado ahora va detras de ese.
     */
    [StaticConstructorOnStartup]
    public static class AlIniciar
    {
        private const int Intentos = 3;

        static AlIniciar()
        {
            PrepararOficial();
            Encolar(Intentos);
        }

        /** Antes de armar la ventana: si se instalo o se deshizo la traduccion oficial, este inicio es el
         *  reinicio que faltaba, y sus filas ya pueden salir en Novedades. Y la consulta a GitHub sale
         *  ahora, para que la respuesta este cuando se abran los ajustes. */
        private static void PrepararOficial()
        {
            try
            {
                AjustesRml Ajustes = RmlVentanaMod.Instancia.Ajustes;
                if (Ajustes.OficialReiniciar != null)
                {
                    Ajustes.OficialReiniciar = null;
                    RmlVentanaMod.Instancia.WriteSettings();
                }
                if (OficialJuego.Disponible && Ajustes.RevisarOficial)
                    OficialJuego.Consultar();
            }
            catch (Exception e)
            {
                Log.Warning("[Traducciones [Español Latino]] No se pudo revisar la traducción oficial: " + e.Message);
            }
        }

        private static void Encolar(int Quedan)
        {
            LongEventHandler.QueueLongEvent(() => Mostrar(Quedan), null, false, null);
        }

        private static void Mostrar(int Quedan)
        {
            try
            {
                if (Find.WindowStack == null)
                {
                    if (Quedan > 1)
                        Encolar(Quedan - 1);
                    else
                        Log.Warning("[Traducciones [Español Latino]] No se abrió la ventana: la pantalla del menú no estaba lista.");
                    return;
                }

                var Ventana = VentanaTraducciones.Crear(out bool Novedades);

                // Con el modo desarrollador, por que aparecio o no: sin esto, «no aparecio» no dice
                // si fue por el idioma, porque no habia novedades o por un error.
                if (Prefs.DevMode)
                    Log.Message("[Traducciones [Español Latino]] Al iniciar: idioma " + LanguageDatabase.activeLanguage?.folderName +
                                ", novedades " + (Novedades ? "sí" : "no") + ", lista " + (Ventana == null ? "con error" : "lista") +
                                ", intento " + (Intentos - Quedan + 1) + ", cambios en el PC: " + CambiosEnElPc.Estado() + ".");

                if (Ventana != null && Novedades)
                    Find.WindowStack.Add(Ventana);
            }
            catch (Exception e)
            {
                Log.Warning("[Traducciones [Español Latino]] No se pudo mostrar la ventana: " + e);
            }
        }
    }
}
