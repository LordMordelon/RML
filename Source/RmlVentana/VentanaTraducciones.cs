using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RmlVentana.Logica;
using UnityEngine;
using Verse;

namespace RmlVentana
{
    /** «Traducciones [Español Latino]», con dos pestañas: «Novedades», lo que cambio en RML desde la
     *  ultima vez, y «Tus mods», los mods activos y quien los traduce. Abre siempre en la primera. */
    public class VentanaTraducciones : Window
    {
        private const float AltoFilaSimple = 38f;
        private const float AltoFilaDoble = 50f;
        private const float AnchoEstado = 130f;
        private const float AnchoTraductor = 250f;
        private const float Separacion = 12f;

        private static readonly Color Gris = Hex(0xA9B0B8);
        private static readonly Color FondoEncabezado = Hex(0x1F252B);
        private static readonly Color FondoFecha = Hex(0x262D35);
        private static readonly Color FranjaFecha = Hex(0x4A5560);
        private static readonly Color FranjaFechaAbierta = Hex(0x6C8EAD);
        private static readonly Color Verde = Hex(0x8FC77A);

        private static readonly Dictionary<Estado, (Color Fondo, Color Texto)> ColoresDeEstado = new Dictionary<Estado, (Color, Color)>
        {
            [Estado.Traducido] = (Hex(0x2C5A80), Hex(0xE8F3FC)),
            [Estado.Sobrescrito] = (Hex(0x5B3F7A), Hex(0xF1E8FB)),
            [Estado.SinTraducir] = (Hex(0x3A3F45), Hex(0xD0D4D8)),
            [Estado.SinTexto] = (Hex(0x2B3035), Hex(0xA9B0B8)),
        };

        private static readonly Dictionary<Cambio, (Color Fondo, Color Texto)> ColoresDeCambio = new Dictionary<Cambio, (Color, Color)>
        {
            [Cambio.Anadido] = (Hex(0x3D6B30), Hex(0xEAF6E5)),
            [Cambio.Actualizado] = (Hex(0x2C5A80), Hex(0xE8F3FC)),
            [Cambio.Corregido] = (Hex(0x7A5520), Hex(0xFBEFDC)),
        };

        private readonly List<Fila> Filas;
        private readonly string Resumen;
        private readonly List<EntradaDeNovedades> Entradas;
        private readonly List<Novedad> Pendientes;
        private readonly bool YaVistas;
        /** «Ver todo el historial»: todos los mods por la fecha de su ultimo cambio. */
        private readonly List<GrupoDeFecha> Historial;
        private readonly HashSet<string> FechasAbiertas = new HashSet<string>();
        private bool VerHistorial;
        private readonly string ResumenNovedades;
        private readonly QuickSearchWidget Buscador = new QuickSearchWidget();
        private enum Pestana { Novedades, TusMods, Ajustes }
        private Pestana Abierta = Pestana.Novedades;
        private Vector2 DesplazamientoAjustes;
        private float AltoAjustes;
        private Vector2 Desplazamiento;
        private Vector2 DesplazamientoNovedades;
        private bool NoVolverAMostrar;

        public override Vector2 InitialSize => new Vector2(900f, 720f);

        private static AjustesRml Ajustes => RmlVentanaMod.Instancia.Ajustes;
        private static string NombreDeRml => "RML_Ventana_Titulo".Translate();

        /** Con los mods activos de este momento, o null si algo falla al leerlos. */
        public static VentanaTraducciones Crear(out bool Novedades)
        {
            Novedades = false;
            try
            {
                var PorRml = LecturaDelJuego.TraducidosPorRml(RmlVentanaMod.Instancia.Content);
                // Los mods pedidos que no tienen nada que traducir. Como Novedades.tsv, lo pone el
                // builder en output/ (Solicitudes.cs); sin el, esos mods salen como «Sin traducir».
                string ArchivoSinTexto = System.IO.Path.Combine(RmlVentanaMod.Instancia.Content.RootDir, "SinTexto.tsv");
                var SinTexto = Clasificador.LeerSinTexto(System.IO.File.Exists(ArchivoSinTexto) ? System.IO.File.ReadAllText(ArchivoSinTexto) : null);
                var Filas = Clasificador.Clasificar(LecturaDelJuego.ModsActivos(), PorRml, Ajustes.Vistos,
                    LecturaDelJuego.Id(RmlVentanaMod.Instancia.Content.ModMetaData.PackageIdNonUnique), SinTexto);

                Solicitudes.Limpiar(Ajustes.Solicitudes, PorRml);

                // Novedades.tsv lo arma el builder en la copia de output/ (Novedades.cs). Sin el,
                // la pestaña queda vacia y la ventana funciona igual.
                string Archivo = System.IO.Path.Combine(RmlVentanaMod.Instancia.Content.RootDir, "Novedades.tsv");
                var Entradas = NovedadesRml.Leer(System.IO.File.Exists(Archivo) ? System.IO.File.ReadAllText(Archivo) : null);
                var Pendientes = NovedadesRml.ParaMostrar(Entradas, Ajustes.NovedadesVistas, Ajustes.NovedadesMostradas, DateTime.Today, out bool YaVistas);
                // Lo ya visto se muestra igual, pero no hace aparecer la ventana sola.
                bool HayNuevas = Pendientes.Count > 0 && !YaVistas;

                // La traduccion oficial va arriba, y recien despues de reiniciar: antes no hizo efecto.
                if (Ajustes.OficialReiniciar == null)
                    Pendientes.InsertRange(0, Ajustes.OficialNovedades
                        .Select(x => NovedadOficial.Leer(x, OficialJuego.NombreDe(NovedadOficial.CarpetaDe(x)), OficialJuego.ArchivoDeCambios))
                        .Where(x => x != null));

                Novedades = Clasificador.DebeAparecerSola(Ajustes.NoVolverAMostrar,
                    Clasificador.EsEspanolLatino(LanguageDatabase.activeLanguage?.folderName), Ajustes.Vistos, Filas,
                    HayNovedades: HayNuevas || Pendientes.Any(x => x.Oficial));
                return new VentanaTraducciones(Filas, Entradas, Pendientes, YaVistas);
            }
            catch (Exception e)
            {
                Log.Warning("[Traducciones [Español Latino]] No se pudo armar la lista de mods: " + e.Message);
                return null;
            }
        }

        private VentanaTraducciones(List<Fila> Filas, List<EntradaDeNovedades> Entradas, List<Novedad> Pendientes, bool YaVistas)
        {
            this.Filas = Filas;
            this.Entradas = Entradas;
            this.Pendientes = Pendientes;
            this.YaVistas = YaVistas;
            Historial = NovedadesRml.PorFecha(NovedadesRml.Historial(Entradas));
            // El historial arranca con la fecha mas nueva abierta y las demas plegadas.
            if (Historial.Count > 0)
                FechasAbiertas.Add(Historial[0].Fecha);
            ResumenNovedades = YaVistas
                ? (NovedadesRml.PorUltimaActualizacion(Entradas)
                    ? "RML_Ventana_ResumenNovedadesVistasUltima".Translate().ToString()
                    : "RML_Ventana_ResumenNovedadesVistasDias".Translate(NovedadesRml.DiasLaPrimeraVez).ToString())
                : Ajustes.UltimaVez != null
                ? "RML_Ventana_ResumenNovedades".Translate(FechaLarga(Ajustes.UltimaVez)).ToString()
                : NovedadesRml.PorUltimaActualizacion(Entradas)
                    ? "RML_Ventana_ResumenNovedadesUltima".Translate().ToString()
                    : "RML_Ventana_ResumenNovedadesPrimeraVez".Translate(NovedadesRml.DiasLaPrimeraVez).ToString();
            doCloseX = true;
            closeOnClickedOutside = false;
            absorbInputAroundWindow = true;
            forcePause = true;
            onlyOneOfTypeAllowed = true;
            NoVolverAMostrar = Ajustes.NoVolverAMostrar;

            int PorEste = Filas.Count(x => x.PorRml);
            int Sobrescritos = Filas.Count(x => x.Estado == Estado.Sobrescrito);
            int PorOtros = Filas.Count(x => !x.PorRml && x.Estado == Estado.Traducido);
            int Sin = Filas.Count(x => x.Estado == Estado.SinTraducir);
            Resumen = "RML_Ventana_Resumen".Translate(Filas.Count, PorEste, Sobrescritos, PorOtros, Sin);
        }

        public override void PreClose()
        {
            base.PreClose();
            Ajustes.NoVolverAMostrar = NoVolverAMostrar;
            Ajustes.Vistos = Filas.Select(x => x.Mod.PackageId).ToList();
            // Sin Novedades.tsv no se pisa lo anterior: lo no visto tiene que seguir saliendo.
            if (Entradas.Count > 0)
                Ajustes.NovedadesVistas = NovedadesRml.Claves(Entradas);
            // Lo nuevo de esta vez es lo que vuelve a salir hasta que haya algo mas nuevo.
            if (!YaVistas && Pendientes.Any(x => !x.Oficial))
                Ajustes.NovedadesMostradas = NovedadesRml.ClavesDe(Pendientes);
            Ajustes.UltimaVez = DateTime.Today.ToString("yyyy-MM-dd");
            if (Pendientes.Any(x => x.Oficial))
                Ajustes.OficialNovedades = new List<string>();
            RmlVentanaMod.Instancia.WriteSettings();
        }

        public override void DoWindowContents(Rect inRect)
        {
            // El titulo va arriba de las pestañas, como en los ajustes de Vanilla Expanded Framework.
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(inRect.x, inRect.y, inRect.width - 40f, 34f), NombreDeRml);
            Text.Font = GameFont.Small;

            Rect Marco = inRect;
            Marco.yMin += 34f + 12f + TabDrawer.TabHeight;
            Widgets.DrawMenuSection(Marco);
            var Pestanas = new List<TabRecord>
            {
                new TabRecord("RML_Ventana_PestanaNovedades".Translate(), () => Abierta = Pestana.Novedades, Abierta == Pestana.Novedades),
                new TabRecord("RML_Ventana_PestanaTusMods".Translate(), () => Abierta = Pestana.TusMods, Abierta == Pestana.TusMods),
            };
            // En 1.6 no hay traduccion oficial que bajar, y la pestaña quedaria vacia.
            if (OficialJuego.Disponible)
                Pestanas.Add(new TabRecord("RML_Ventana_PestanaAjustes".Translate(), () => Abierta = Pestana.Ajustes, Abierta == Pestana.Ajustes));
            TabDrawer.DrawTabs(Marco, Pestanas);

            Rect Area = Marco.ContractedBy(16f);
            float Y = Area.y;

            float AnchoResumen = Area.width;
            if (Abierta == Pestana.Novedades && Historial.Count > 0)
            {
                string Historico = "RML_Ventana_VerHistorial".Translate();
                float AnchoCasilla = Text.CalcSize(Historico).x + 40f;
                Widgets.CheckboxLabeled(new Rect(Area.xMax - AnchoCasilla, Y - 3f, AnchoCasilla, 30f), Historico, ref VerHistorial, placeCheckboxNearText: true);
                AnchoResumen -= AnchoCasilla + 12f;
            }

            GUI.color = Gris;
            string TextoResumen = Abierta == Pestana.Novedades ? (VerHistorial ? "RML_Ventana_ResumenHistorial".Translate().ToString() : ResumenNovedades)
                : Abierta == Pestana.TusMods ? Resumen : "RML_Ventana_ResumenAjustes".Translate().ToString();
            Widgets.Label(new Rect(Area.x, Y, AnchoResumen, 24f), TextoResumen.Truncate(AnchoResumen));
            GUI.color = Color.white;
            Y += 32f;

            if (Abierta == Pestana.TusMods)
            {
                Buscador.OnGUI(new Rect(Area.x, Y, Area.width, QuickSearchWidget.WidgetHeight));
                Y += QuickSearchWidget.WidgetHeight + 10f;
            }
            else if (Abierta == Pestana.Novedades && OficialJuego.Disponible && OficialJuego.Consulta != null && OficialJuego.Estado() == EstadoOficial.HayArreglos)
            {
                DibujarAvisoOficial(new Rect(Area.x, Y, Area.width, 38f));
                Y += 38f + 10f;
            }

            // Pie: la nota, una linea y la fila de la casilla y el boton.
            const float AltoPie = 24f + 10f + 1f + 12f + 38f;
            Rect Tabla = new Rect(Area.x, Y, Area.width, Area.yMax - AltoPie - 10f - Y);
            if (Abierta == Pestana.Novedades)
                DibujarNovedades(Tabla);
            else if (Abierta == Pestana.TusMods)
                DibujarTabla(Tabla);
            else
                DibujarAjustes(Tabla);

            float YPie = Area.yMax - AltoPie;
            // La nota es de las traducciones de mods: en «Ajustes», que es de la oficial, no va.
            if (Abierta != Pestana.Ajustes)
            {
                GUI.color = Gris;
                Widgets.Label(new Rect(Area.x, YPie, Area.width, 24f), "RML_Ventana_NotaSobrescribe".Translate());
                GUI.color = Color.white;
            }
            YPie += 34f;
            Widgets.DrawLineHorizontal(Area.x, YPie, Area.width, Widgets.SeparatorLineColor);
            YPie += 13f;

            string Casilla = "RML_Ventana_NoVolverAMostrar".Translate();
            Widgets.CheckboxLabeled(new Rect(Area.x, YPie + 4f, Text.CalcSize(Casilla).x + 40f, 30f), Casilla, ref NoVolverAMostrar, placeCheckboxNearText: true);

            if (Widgets.ButtonText(new Rect(Area.xMax - 160f, YPie, 160f, 38f), "RML_Ventana_Cerrar".Translate()))
                Close();

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }

        private void DibujarNovedades(Rect Tabla)
        {
            Rect Encabezado = new Rect(Tabla.x, Tabla.y, Tabla.width, 28f);
            Widgets.DrawBoxSolid(Encabezado, FondoEncabezado);
            Columnas(Encabezado.ContractedBy(12f, 0f), out Rect ColMod, out Rect ColCambio, out Rect ColFecha);
            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = Gris;
            Widgets.Label(ColMod, "RML_Ventana_ColMod".Translate());
            Widgets.Label(ColCambio, "RML_Ventana_ColCambio".Translate());
            Widgets.Label(ColFecha, "RML_Ventana_ColFecha".Translate());
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect Lista = new Rect(Tabla.x, Encabezado.yMax, Tabla.width, Tabla.height - Encabezado.height);

            // Cada renglon es un titulo de fecha (Novedad null) o una fila.
            var Renglones = new List<(GrupoDeFecha Grupo, Novedad Novedad)>();
            if (VerHistorial)
                foreach (GrupoDeFecha Grupo in Historial)
                {
                    Renglones.Add((Grupo, null));
                    if (FechasAbiertas.Contains(Grupo.Fecha))
                        Renglones.AddRange(Grupo.Novedades.Select(x => ((GrupoDeFecha)null, x)));
                }
            else
                Renglones.AddRange(Pendientes.Select(x => ((GrupoDeFecha)null, x)));

            if (Renglones.Count == 0)
            {
                GUI.color = Gris;
                Widgets.Label(new Rect(Lista.x + 12f, Lista.y + 20f, Lista.width - 24f, 48f), "RML_Ventana_SinNovedades".Translate());
                GUI.color = Color.white;
                return;
            }

            Rect Vista = new Rect(0f, 0f, Lista.width - 16f, Renglones.Count * AltoFilaSimple);
            Widgets.BeginScrollView(Lista, ref DesplazamientoNovedades, Vista);

            for (int i = 0; i < Renglones.Count; i++)
            {
                Rect Renglon = new Rect(0f, i * AltoFilaSimple, Vista.width, AltoFilaSimple);
                if (Renglon.yMax < DesplazamientoNovedades.y || Renglon.y > DesplazamientoNovedades.y + Lista.height)
                    continue;

                if (Renglones[i].Novedad == null)
                    DibujarFecha(Renglon, Renglones[i].Grupo);
                else
                    // En el historial, las filas van con sangria bajo su fecha.
                    DibujarNovedad(Renglon, Renglones[i].Novedad, i, VerHistorial ? 26f : 0f);
            }

            Widgets.EndScrollView();
        }

        /** El titulo de una fecha del historial: una franja separada de la de arriba, con una linea de
         *  color a la izquierda como los bloques de «Ver cambios», la flecha, la fecha y, en la columna
         *  Cambio, cuantos mods de cada tipo con las mismas etiquetas de las filas. Un clic lo pliega o
         *  lo despliega. */
        private void DibujarFecha(Rect Renglon, GrupoDeFecha Grupo)
        {
            bool Abierta = FechasAbiertas.Contains(Grupo.Fecha);
            Rect Caja = new Rect(Renglon.x, Renglon.y + 4f, Renglon.width, Renglon.height - 4f);
            Widgets.DrawBoxSolid(Caja, FondoFecha);
            Widgets.DrawBoxSolid(new Rect(Caja.x, Caja.y, 3f, Caja.height), Abierta ? FranjaFechaAbierta : FranjaFecha);
            Widgets.DrawHighlightIfMouseover(Caja);
            Columnas(Caja.ContractedBy(12f, 0f), out Rect ColMod, out Rect ColCambio, out _);

            const float Flecha = 16f;
            GUI.color = Abierta ? Color.white : Gris;
            GUI.DrawTexture(new Rect(ColMod.x, Caja.center.y - Flecha / 2f, Flecha, Flecha), Abierta ? TexButton.Collapse : TexButton.Reveal);
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(new Rect(ColMod.x + Flecha + 10f, Caja.y, ColMod.width - Flecha - 10f, Caja.height), FechaLarga(Grupo.Fecha));

            Text.Font = GameFont.Tiny;
            float X = ColCambio.x;
            foreach (Cambio Tipo in new[] { Cambio.Anadido, Cambio.Actualizado, Cambio.Corregido })
            {
                int Cuantos = Grupo.Novedades.Count(x => x.Cambio == Tipo);
                if (Cuantos == 0)
                    continue;
                string Texto = TextoDeCambio(Tipo) + " (" + Cuantos + ")";
                float Ancho = Text.CalcSize(Texto).x + 16f;
                var (Fondo, ColorDeTexto) = ColoresDeCambio[Tipo];
                Etiqueta(new Rect(X, Caja.center.y - 11f, Ancho, 22f), Texto, Fondo, ColorDeTexto);
                X += Ancho + 6f;
            }
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            if (Widgets.ButtonInvisible(Caja))
            {
                if (Abierta)
                    FechasAbiertas.Remove(Grupo.Fecha);
                else
                    FechasAbiertas.Add(Grupo.Fecha);
            }
        }

        private void DibujarNovedad(Rect Renglon, Novedad Novedad, int i, float Sangria)
        {
            if (i % 2 == 1) Widgets.DrawLightHighlight(Renglon);
            if (Novedad.Oficial)
                Widgets.DrawBoxSolid(Renglon, new Color(Verde.r, Verde.g, Verde.b, Parpadeo()));
            Widgets.DrawHighlightIfMouseover(Renglon);
            Columnas(Renglon.ContractedBy(12f, 0f), out Rect Mod, out Rect Cambio, out Rect Fecha);
            Mod.xMin += Sangria;

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            string Nombre = Novedad.Nombre.Truncate(Mod.width);
            Widgets.Label(Mod, Nombre);
            if (Nombre != Novedad.Nombre)
                TooltipHandler.TipRegion(Mod, Novedad.Nombre);
            Widgets.Label(Fecha, FechaLarga(Novedad.Fecha));

            // Solo los mods con textos en Cambios.tsv (los de la ultima subida) llevan el boton.
            if (Novedad.TieneDetalle)
            {
                string Ver = "RML_Ventana_VerCambios".Translate();
                float AnchoBoton = Text.CalcSize(Ver).x + 24f;
                if (Widgets.ButtonText(new Rect(Fecha.xMax - AnchoBoton, Fecha.center.y - 14f, AnchoBoton, 28f), Ver))
                    AbrirCambios(Novedad);
            }

            Text.Font = GameFont.Tiny;
            string Texto = TextoDeCambio(Novedad.Cambio);
            var (Fondo, ColorDeTexto) = ColoresDeCambio[Novedad.Cambio];
            Etiqueta(new Rect(Cambio.x, Cambio.center.y - 11f, Text.CalcSize(Texto).x + 16f, 22f), Texto, Fondo, ColorDeTexto);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        /** El fondo de una fila de la traduccion oficial sube y baja de brillo, una vez cada ~1,6 s. */
        private static float Parpadeo() =>
            0.06f + 0.16f * (0.5f + 0.5f * (float)Math.Sin(DateTime.Now.TimeOfDay.TotalSeconds * 4.0));

        /** «Hay arreglos nuevos de la traducción oficial», con el boton que lleva a los ajustes de RML,
         *  donde se actualiza. La ventana no se queda abierta debajo. */
        private void DibujarAvisoOficial(Rect Aviso)
        {
            Widgets.DrawBoxSolid(Aviso, FondoEncabezado);
            string Ir = "RML_Oficial_IrAAjustes".Translate();
            float AnchoBoton = Text.CalcSize(Ir).x + 24f;
            Rect Boton = new Rect(Aviso.xMax - AnchoBoton - 6f, Aviso.center.y - 14f, AnchoBoton, 28f);

            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = Verde;
            Widgets.Label(new Rect(Aviso.x + 12f, Aviso.y, Boton.x - Aviso.x - 18f, Aviso.height),
                "RML_Oficial_AvisoVentana".Translate(FechaLarga(OficialJuego.FechaDeLosArreglos.ToString("yyyy-MM-dd"))));
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;

            if (Widgets.ButtonText(Boton, Ir))
                Abierta = Pestana.Ajustes;
        }

        /** Para abrirla desde los ajustes de mods del juego. En 1.6 no hay pestaña de ajustes. */
        public void AbrirEnAjustes() => Abierta = OficialJuego.Disponible ? Pestana.Ajustes : Pestana.Novedades;

        /** La pestaña «Ajustes», en la misma caja que las tablas de las otras dos y con desplazamiento:
         *  un error o el aviso de juego atrasado la pueden alargar. */
        private void DibujarAjustes(Rect Tabla)
        {
            Widgets.DrawMenuSection(Tabla);
            Rect Dentro = Tabla.ContractedBy(12f);
            Rect Vista = new Rect(0f, 0f, Dentro.width - 16f, Mathf.Max(AltoAjustes, Dentro.height));
            Widgets.BeginScrollView(Dentro, ref DesplazamientoAjustes, Vista);
            var Lista = new Listing_Standard();
            Lista.Begin(new Rect(0f, 0f, Vista.width, 100000f));
            DibujarOficial(Lista);
            AltoAjustes = Lista.CurHeight;
            Lista.End();
            Widgets.EndScrollView();
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
        }

        /** La seccion de la traduccion oficial (PLAN-TRADUCCION-OFICIAL.md): la casilla, una linea con
         *  el estado y los botones que corresponden a ese estado. */
        private static void DibujarOficial(Listing_Standard Lista)
        {
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

        private static void AbrirCambios(Novedad Novedad)
        {
            try
            {
                Find.WindowStack.Add(new VentanaCambios(Novedad));
            }
            catch (Exception e)
            {
                Log.Warning("[Traducciones [Español Latino]] No se pudo abrir la lista de cambios: " + e.Message);
            }
        }

        private static string TextoDeCambio(Cambio Cambio)
        {
            switch (Cambio)
            {
                case Logica.Cambio.Anadido: return "RML_Ventana_Anadido".Translate();
                case Logica.Cambio.Actualizado: return "RML_Ventana_Actualizado".Translate();
                default: return "RML_Ventana_Corregido".Translate();
            }
        }

        internal static string FechaLarga(string FechaIso) =>
            Solicitudes.FechaCorta(FechaIso, "RML_Ventana_Meses".Translate().ToString().Split(','), "RML_Ventana_FormatoFechaLarga".Translate());

        private void DibujarTabla(Rect Tabla)
        {
            var Visibles = Filas.Where(Coincide).ToList();

            Rect Encabezado = new Rect(Tabla.x, Tabla.y, Tabla.width, 28f);
            Widgets.DrawBoxSolid(Encabezado, FondoEncabezado);
            Columnas(Encabezado.ContractedBy(12f, 0f), out Rect ColMod, out Rect ColEstado, out Rect ColTraductor);
            Text.Anchor = TextAnchor.MiddleLeft;
            GUI.color = Gris;
            Widgets.Label(ColMod, "RML_Ventana_ColMod".Translate());
            Widgets.Label(ColEstado, "RML_Ventana_ColEstado".Translate());
            Widgets.Label(ColTraductor, "RML_Ventana_ColTraducidoPor".Translate());
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect Lista = new Rect(Tabla.x, Encabezado.yMax, Tabla.width, Tabla.height - Encabezado.height);

            if (Visibles.Count == 0)
            {
                GUI.color = Gris;
                string Vacio = Filas.Count == 0
                    ? "RML_Ventana_SinMods".Translate().ToString()
                    : "RML_Ventana_SinCoincidencias".Translate(Buscador.filter.Text).ToString();
                Widgets.Label(new Rect(Lista.x + 12f, Lista.y + 20f, Lista.width - 24f, 48f), Vacio);
                GUI.color = Color.white;
                return;
            }

            float AltoTotal = Visibles.Sum(Alto);
            Rect Vista = new Rect(0f, 0f, Lista.width - 16f, AltoTotal);
            Widgets.BeginScrollView(Lista, ref Desplazamiento, Vista);

            float Y = 0f;
            for (int i = 0; i < Visibles.Count; i++)
            {
                Rect Renglon = new Rect(0f, Y, Vista.width, Alto(Visibles[i]));
                Y += Renglon.height;

                // Solo se dibuja lo que se ve: con muchos mods, dibujar todos en cada cuadro se nota.
                if (Renglon.yMax < Desplazamiento.y || Renglon.y > Desplazamiento.y + Lista.height)
                    continue;

                DibujarFila(Renglon, Visibles[i], i);
            }

            Widgets.EndScrollView();
        }

        private void DibujarFila(Rect Renglon, Fila Fila, int Indice)
        {
            if (Indice % 2 == 1) Widgets.DrawLightHighlight(Renglon);
            Widgets.DrawHighlightIfMouseover(Renglon);

            Columnas(Renglon.ContractedBy(12f, 0f), out Rect ColMod, out Rect ColEstado, out Rect ColTraductor);

            // El nombre, cortado si no entra, y la marca «NUEVO» detras.
            Text.Font = GameFont.Tiny;
            string Nuevo = "RML_Ventana_Nuevo".Translate();
            float AnchoMarca = Fila.Nuevo ? Text.CalcSize(Nuevo).x + 10f : 0f;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            string Nombre = Fila.Mod.Nombre.Truncate(ColMod.width - (Fila.Nuevo ? AnchoMarca + 8f : 0f));
            float AnchoNombre = Text.CalcSize(Nombre).x;
            Widgets.Label(new Rect(ColMod.x, ColMod.y, ColMod.width, ColMod.height), Nombre);
            if (Nombre != Fila.Mod.Nombre)
                TooltipHandler.TipRegion(new Rect(ColMod.x, ColMod.y, AnchoNombre, ColMod.height), Fila.Mod.Nombre);

            if (Fila.Nuevo)
            {
                Text.Font = GameFont.Tiny;
                Etiqueta(new Rect(ColMod.x + AnchoNombre + 8f, ColMod.center.y - 9f, AnchoMarca, 18f), Nuevo, Hex(0xE0B95C), Hex(0x15191D));
            }

            Text.Font = GameFont.Tiny;
            string Estado = TextoDeEstado(Fila.Estado);
            var (Fondo, Texto) = ColoresDeEstado[Fila.Estado];
            Etiqueta(new Rect(ColEstado.x, ColEstado.center.y - 11f, Text.CalcSize(Estado).x + 16f, 22f), Estado, Fondo, Texto);

            if (Fila.Estado == Logica.Estado.SinTraducir)
                BotonSolicitar(ColTraductor, Fila);
            else
                DibujarTraductor(ColTraductor, Fila);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void DibujarTraductor(Rect Columna, Fila Fila)
        {
            string Traductor = TextoDeTraductor(Fila);
            string EnLugarDe = Fila.Estado == Logica.Estado.Sobrescrito ? TextoEnLugarDe(Fila) : null;

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            Rect Arriba = EnLugarDe == null ? Columna : new Rect(Columna.x, Columna.y + 4f, Columna.width, 24f);
            Widgets.Label(Arriba, Traductor.Truncate(Columna.width));

            if (EnLugarDe != null)
            {
                Text.Font = GameFont.Tiny;
                GUI.color = Gris;
                Rect Abajo = new Rect(Columna.x, Arriba.yMax - 2f, Columna.width, 18f);
                Widgets.Label(Abajo, EnLugarDe.Truncate(Columna.width));
                TooltipHandler.TipRegion(Abajo, EnLugarDe);
                GUI.color = Color.white;
            }
        }

        private static void BotonSolicitar(Rect Columna, Fila Fila)
        {
            Text.Font = GameFont.Small;
            bool Pedida = Ajustes.Solicitudes.TryGetValue(Fila.Mod.PackageId, out string Fecha);
            string Corta = Pedida ? FechaCorta(Fecha) : null;
            string Texto = Pedida ? "RML_Ventana_SolicitadaEl".Translate(Corta).ToString() : "RML_Ventana_Solicitar".Translate().ToString();

            Rect Boton = new Rect(Columna.x, Columna.center.y - 15f, Mathf.Min(Columna.width, Text.CalcSize(Texto).x + 28f), 30f);
            bool Clic = Pedida
                ? Widgets.ButtonText(Boton, Texto, true, true, Verde)
                : Widgets.ButtonText(Boton, Texto);
            TooltipHandler.TipRegion(Boton, Pedida
                ? "RML_Ventana_SolicitadaAyuda".Translate(Corta).ToString()
                : "RML_Ventana_SolicitarAyuda".Translate().ToString());

            if (!Clic) return;

            Application.OpenURL(Solicitudes.Enlace(Fila.Mod.Nombre, Fila.Mod.WorkshopId, Fila.Mod.PackageId));
            Ajustes.Solicitudes[Fila.Mod.PackageId] = DateTime.Now.ToString("yyyy-MM-dd");
            RmlVentanaMod.Instancia.WriteSettings();
        }

        private bool Coincide(Fila Fila)
        {
            if (!Buscador.filter.Active)
                return true;

            return Buscador.filter.Matches(Fila.Mod.Nombre) ||
                   Buscador.filter.Matches(TextoDeEstado(Fila.Estado)) ||
                   Buscador.filter.Matches(TextoDeTraductor(Fila)) ||
                   (Fila.Estado == Logica.Estado.Sobrescrito && Buscador.filter.Matches(TextoEnLugarDe(Fila)));
        }

        private static float Alto(Fila Fila) => Fila.Estado == Estado.Sobrescrito ? AltoFilaDoble : AltoFilaSimple;

        private static void Columnas(Rect Renglon, out Rect Mod, out Rect Estado, out Rect Traductor)
        {
            Traductor = new Rect(Renglon.xMax - AnchoTraductor, Renglon.y, AnchoTraductor, Renglon.height);
            Estado = new Rect(Traductor.x - Separacion - AnchoEstado, Renglon.y, AnchoEstado, Renglon.height);
            Mod = new Rect(Renglon.x, Renglon.y, Estado.x - Separacion - Renglon.x, Renglon.height);
        }

        /** Una etiqueta de color con el texto centrado, en la fuente que este puesta. */
        private static void Etiqueta(Rect Rect, string Texto, Color Fondo, Color ColorDeTexto)
        {
            Widgets.DrawBoxSolid(Rect, Fondo);
            TextAnchor Antes = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = ColorDeTexto;
            Widgets.Label(Rect, Texto);
            GUI.color = Color.white;
            Text.Anchor = Antes;
        }

        private static string TextoDeEstado(Estado Estado)
        {
            switch (Estado)
            {
                case Estado.Sobrescrito: return "RML_Ventana_Sobrescrito".Translate();
                case Estado.SinTraducir: return "RML_Ventana_SinTraducir".Translate();
                case Estado.SinTexto: return "RML_Ventana_SinTexto".Translate();
                default: return "RML_Ventana_Traducido".Translate();
            }
        }

        private static string TextoDeTraductor(Fila Fila)
        {
            if (Fila.PorRml) return NombreDeRml;
            if (Fila.Otros.Count > 0) return string.Join(", ", Fila.Otros);
            if (Fila.IncluidaEnElMod) return "RML_Ventana_IncluidaEnElMod".Translate();
            if (Fila.Estado == Estado.SinTexto) return "RML_Ventana_NoNecesitaTraduccion".Translate();
            return "";
        }

        private static string TextoEnLugarDe(Fila Fila)
        {
            var Otras = new List<string>(Fila.Otros);
            if (Fila.IncluidaEnElMod) Otras.Add("RML_Ventana_LaIncluidaEnElMod".Translate());
            return "RML_Ventana_EnLugarDe".Translate(string.Join(", ", Otras));
        }

        private static string FechaCorta(string FechaIso) =>
            Solicitudes.FechaCorta(FechaIso, "RML_Ventana_Meses".Translate().ToString().Split(','), "RML_Ventana_FormatoFecha".Translate());

        private static Color Hex(int Rgb) =>
            new Color(((Rgb >> 16) & 0xFF) / 255f, ((Rgb >> 8) & 0xFF) / 255f, (Rgb & 0xFF) / 255f);
    }
}
