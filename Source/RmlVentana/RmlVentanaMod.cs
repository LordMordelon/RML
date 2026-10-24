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

            Lista.Gap();
            if (Lista.ButtonText("RML_Ajustes_Abrir".Translate(), widthPct: 0.4f) &&
                VentanaTraducciones.Crear(out _) is VentanaTraducciones Ventana)
                Find.WindowStack.Add(Ventana);

            Lista.End();
        }
    }

    /** Corre con el juego ya cargado: la ventana aparece sola si corresponde (Clasificador.DebeAparecerSola). */
    [StaticConstructorOnStartup]
    public static class AlIniciar
    {
        static AlIniciar()
        {
            LongEventHandler.ExecuteWhenFinished(() =>
            {
                try
                {
                    if (VentanaTraducciones.Crear(out bool Novedades) is VentanaTraducciones Ventana && Novedades)
                        Find.WindowStack.Add(Ventana);
                }
                catch (Exception e)
                {
                    Log.Warning("[Traducciones [Español Latino]] No se pudo mostrar la ventana: " + e.Message);
                }
            });
        }
    }
}
