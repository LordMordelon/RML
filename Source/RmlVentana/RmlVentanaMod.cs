using System;
using System.Collections.Generic;
using RmlVentana.Logica;
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

            if (OficialJuego.Disponible)
                DibujarOficial(Lista);

            Lista.End();
        }

        /** La seccion de la traduccion oficial (PLAN-TRADUCCION-OFICIAL.md): la casilla, una linea con
         *  el estado y los botones que corresponden a ese estado. */
        private void DibujarOficial(Listing_Standard Lista)
        {
            Lista.GapLine();
            Text.Font = GameFont.Medium;
            Lista.Label("RML_Oficial_Titulo".Translate());
            Text.Font = GameFont.Small;
            GUI.color = new Color(0.66f, 0.69f, 0.72f);
            Lista.Label("RML_Oficial_Explicacion".Translate());
            GUI.color = Color.white;
            Lista.Gap(6f);

            bool Antes = Ajustes.RevisarOficial;
            Lista.CheckboxLabeled("RML_Oficial_Revisar".Translate(), ref Ajustes.RevisarOficial, "RML_Oficial_RevisarAyuda".Translate());
            if (Ajustes.RevisarOficial && !Antes)
                OficialJuego.Consultar();

            EstadoOficial Estado = OficialJuego.Estado();
            ConsultaOficial GitHub = OficialJuego.Consulta;
            string Fecha = GitHub == null ? "" : VentanaTraducciones.FechaLarga(GitHub.Fecha.ToString("yyyy-MM-dd"));
            string Texto;
            var Botones = new List<(string Texto, Action Accion)>();

            if (OficialJuego.EstaConsultando)
                Texto = "RML_Oficial_Consultando".Translate();
            else
                switch (Estado)
                {
                    case EstadoOficial.FaltaReiniciar:
                        Texto = (Ajustes.OficialReiniciar == "deshacer" ? "RML_Oficial_ReiniciarDeshacer" : "RML_Oficial_Reiniciar").Translate();
                        Botones.Add(("RML_Oficial_ReiniciarAhora".Translate(), GenCommandLine.Restart));
                        break;
                    case EstadoOficial.SinUsar:
                        Texto = "RML_Oficial_SinUsar".Translate();
                        if (OficialJuego.GitHubEsMasNuevoQueElTar)
                            Botones.Add(("RML_Oficial_ActualizarDeNuevo".Translate(), OficialJuego.Instalar));
                        break;
                    case EstadoOficial.OtraVersion:
                        Texto = "RML_Oficial_OtraVersion".Translate(GitHub.Version ?? "?", OficialJuego.VersionDelJuego);
                        break;
                    case EstadoOficial.HayArreglos:
                        Texto = "RML_Oficial_HayArreglos".Translate(VentanaTraducciones.FechaLarga(OficialJuego.FechaDeLosArreglos.ToString("yyyy-MM-dd")));
                        Botones.Add(("RML_Oficial_Actualizar".Translate(), OficialJuego.Instalar));
                        break;
                    case EstadoOficial.AlDia:
                        Texto = "RML_Oficial_AlDia".Translate(Fecha);
                        break;
                    default:
                        if (OficialJuego.FalloLaConsulta)
                        {
                            Texto = "RML_Oficial_SinRespuesta".Translate();
                            Botones.Add(("RML_Oficial_Reintentar".Translate(), OficialJuego.Consultar));
                        }
                        else if (!Ajustes.RevisarOficial)
                        {
                            Texto = "RML_Oficial_Apagada".Translate();
                            Botones.Add(("RML_Oficial_RevisarAhora".Translate(), OficialJuego.Consultar));
                        }
                        else
                        {
                            // Con la casilla encendida la consulta sale al iniciar; esto es por si no salio.
                            OficialJuego.Consultar();
                            Texto = "RML_Oficial_Consultando".Translate();
                        }
                        break;
                }

            if (OficialJuego.HayAlgoInstalado)
                Botones.Add(((Estado == EstadoOficial.SinUsar ? "RML_Oficial_Borrar" : "RML_Oficial_Deshacer").Translate(), OficialJuego.Deshacer));

            Lista.Label(Texto);

            // Una linea por parte: un DLC copiado de otra instalacion puede estar atrasado aunque el
            // juego base este al dia.
            if (!OficialJuego.EstaConsultando && GitHub != null && Estado != EstadoOficial.FaltaReiniciar && Estado != EstadoOficial.OtraVersion)
            {
                GUI.color = new Color(0.66f, 0.69f, 0.72f);
                foreach (ParteOficial Parte in OficialJuego.Partes())
                {
                    // La sangria va aca: el juego recorta los espacios del principio de un texto Keyed.
                    const string Sangria = "    ";
                    string Nombre = Parte.Carpeta == "Core" ? "RimWorld" : Parte.Carpeta;
                    if (Parte.Estado == EstadoOficial.HayArreglos)
                        Lista.Label(Sangria + "RML_Oficial_ParteArreglos".Translate(Nombre, VentanaTraducciones.FechaLarga(Parte.GitHub.Value.ToString("yyyy-MM-dd"))));
                    else if (Parte.Estado == EstadoOficial.SinUsar)
                        Lista.Label(Sangria + "RML_Oficial_ParteSinUsar".Translate(Nombre));
                    else if (Parte.Estado == EstadoOficial.AlDia)
                        Lista.Label(Sangria + "RML_Oficial_ParteAlDia".Translate(Nombre));
                }
                GUI.color = Color.white;
            }

            // Aproximado (TraduccionOficial.JuegoAtrasado): sirve a quien baja las actualizaciones a mano.
            if (GitHub != null && TraduccionOficial.JuegoAtrasado(RimWorld.VersionControl.CurrentVersion, GitHub.VersionCompleta))
            {
                GUI.color = new Color(0.95f, 0.8f, 0.45f);
                Lista.Label("RML_Oficial_JuegoAtrasado".Translate(RimWorld.VersionControl.CurrentVersion.ToString(3), GitHub.VersionCompleta.ToString()));
                GUI.color = Color.white;
            }

            if (OficialJuego.Error != null)
            {
                GUI.color = new Color(1f, 0.55f, 0.5f);
                Lista.Label(OficialJuego.Error);
                GUI.color = Color.white;
            }
            Lista.Gap(6f);
            foreach (var Boton in Botones)
                if (Lista.ButtonText(Boton.Texto, widthPct: 0.4f))
                    Boton.Accion();
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
