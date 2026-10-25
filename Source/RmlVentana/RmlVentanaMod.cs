using System;
using UnityEngine;
using Verse;

namespace RmlVentana
{
    /** La entrada de RML en el juego: sus ajustes, con la opcion de abrir la ventana a mano, y la
     *  ventana de mods traducidos al llegar al menu principal (PLAN-VENTANA.md).
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
        }

        public override string SettingsCategory() => "RML_Ventana_Titulo".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var Lista = new Listing_Standard();
            Lista.Begin(inRect);

            bool Mostrar = !Ajustes.NoVolverAMostrar;
            Lista.CheckboxLabeled("RML_Ajustes_MostrarAlIniciar".Translate(), ref Mostrar, "RML_Ajustes_MostrarAlIniciarAyuda".Translate());
            Ajustes.NoVolverAMostrar = !Mostrar;

            Lista.CheckboxLabeled("RML_Ajustes_VerCambios".Translate(), ref Ajustes.VerCambios, "RML_Ajustes_VerCambiosAyuda".Translate());

            Lista.Gap();
            if (Lista.ButtonText("RML_Ajustes_Abrir".Translate(), widthPct: 0.4f) &&
                VentanaTraducciones.Crear(out _) is VentanaTraducciones Ventana)
                Find.WindowStack.Add(Ventana);

            Lista.End();
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
            Encolar(Intentos);
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
                                ", intento " + (Intentos - Quedan + 1) + ".");

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
