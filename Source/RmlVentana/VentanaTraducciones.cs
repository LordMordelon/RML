using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RmlVentana.Logica;
using UnityEngine;
using Verse;

namespace RmlVentana
{
    /** «Traducciones [Español Latino]»: los mods activos y quien los traduce. La pestaña «Novedades»
     *  llega en la etapa 3 y va antes de esta, que hoy es la unica. */
    public class VentanaTraducciones : Window
    {
        private const float AltoFilaSimple = 38f;
        private const float AltoFilaDoble = 50f;
        private const float AnchoEstado = 130f;
        private const float AnchoTraductor = 250f;
        private const float Separacion = 12f;

        private static readonly Color Gris = Hex(0xA9B0B8);
        private static readonly Color FondoEncabezado = Hex(0x1F252B);
        private static readonly Color Verde = Hex(0x8FC77A);

        private static readonly Dictionary<Estado, (Color Fondo, Color Texto)> ColoresDeEstado = new Dictionary<Estado, (Color, Color)>
        {
            [Estado.Traducido] = (Hex(0x2C5A80), Hex(0xE8F3FC)),
            [Estado.Sobrescrito] = (Hex(0x5B3F7A), Hex(0xF1E8FB)),
            [Estado.SinTraducir] = (Hex(0x3A3F45), Hex(0xD0D4D8)),
        };

        private readonly List<Fila> Filas;
        private readonly string Resumen;
        private readonly QuickSearchWidget Buscador = new QuickSearchWidget();
        private readonly List<TabRecord> Pestanas;
        private Vector2 Desplazamiento;
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
                var Filas = Clasificador.Clasificar(LecturaDelJuego.ModsActivos(), PorRml, Ajustes.Vistos,
                    LecturaDelJuego.Id(RmlVentanaMod.Instancia.Content.ModMetaData.PackageIdNonUnique));

                Solicitudes.Limpiar(Ajustes.Solicitudes, PorRml);
                Novedades = Clasificador.DebeAparecerSola(Ajustes.NoVolverAMostrar,
                    Clasificador.EsEspanolLatino(LanguageDatabase.activeLanguage?.folderName), Ajustes.Vistos, Filas);
                return new VentanaTraducciones(Filas);
            }
            catch (Exception e)
            {
                Log.Warning("[Traducciones [Español Latino]] No se pudo armar la lista de mods: " + e.Message);
                return null;
            }
        }

        private VentanaTraducciones(List<Fila> Filas)
        {
            this.Filas = Filas;
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

            Pestanas = new List<TabRecord> { new TabRecord("RML_Ventana_PestanaTusMods".Translate(), () => { }, true) };
        }

        public override void PreClose()
        {
            base.PreClose();
            Ajustes.NoVolverAMostrar = NoVolverAMostrar;
            Ajustes.Vistos = Filas.Select(x => x.Mod.PackageId).ToList();
            RmlVentanaMod.Instancia.WriteSettings();
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect Marco = inRect;
            Marco.yMin += TabDrawer.TabHeight;
            Widgets.DrawMenuSection(Marco);
            TabDrawer.DrawTabs(Marco, Pestanas);

            Rect Area = Marco.ContractedBy(16f);
            float Y = Area.y;

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(Area.x, Y, Area.width, 34f), NombreDeRml);
            Y += 34f;

            Text.Font = GameFont.Small;
            GUI.color = Gris;
            Widgets.Label(new Rect(Area.x, Y, Area.width, 24f), Resumen);
            GUI.color = Color.white;
            Y += 32f;

            Buscador.OnGUI(new Rect(Area.x, Y, Area.width, QuickSearchWidget.WidgetHeight));
            Y += QuickSearchWidget.WidgetHeight + 10f;

            // Pie: la nota, una linea y la fila de la casilla y el boton.
            const float AltoPie = 24f + 10f + 1f + 12f + 38f;
            Rect Tabla = new Rect(Area.x, Y, Area.width, Area.yMax - AltoPie - 10f - Y);
            DibujarTabla(Tabla);

            float YPie = Area.yMax - AltoPie;
            GUI.color = Gris;
            Widgets.Label(new Rect(Area.x, YPie, Area.width, 24f), "RML_Ventana_NotaSobrescribe".Translate());
            GUI.color = Color.white;
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
                default: return "RML_Ventana_Traducido".Translate();
            }
        }

        private static string TextoDeTraductor(Fila Fila)
        {
            if (Fila.PorRml) return NombreDeRml;
            if (Fila.Otros.Count > 0) return string.Join(", ", Fila.Otros);
            if (Fila.IncluidaEnElMod) return "RML_Ventana_IncluidaEnElMod".Translate();
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
