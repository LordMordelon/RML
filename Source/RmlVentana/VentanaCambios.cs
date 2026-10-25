using System;
using System.Collections.Generic;
using RmlVentana.Logica;
using UnityEngine;
using Verse;

namespace RmlVentana
{
    /** «Ver cambios»: los textos de un mod que cambiaron en la ultima subida de RML, con lo de antes
     *  en rojo y lo de ahora en verde, como el diff de VS Code (PLAN-VENTANA.md, etapa 5).
     *
     *  Cambios.tsv se lee recien al abrirla, y solo las lineas de este mod: con la casilla de los
     *  ajustes apagada no se abre nunca, y abierta no pesa al cargar el juego. */
    public class VentanaCambios : Window
    {
        private const float AnchoFranja = 3f;
        private const float AnchoMarca = 22f;
        private const float RellenoBloque = 4f;
        private const float RellenoTexto = 12f;

        private static readonly Color Gris = Hex(0xA9B0B8);
        private static readonly (Color Fondo, Color Texto, Color Franja) Rojo = (Hex(0x3F2224), Hex(0xF4B9B5), Hex(0xE5534B));
        private static readonly (Color Fondo, Color Texto, Color Franja) Verde = (Hex(0x1F3B27), Hex(0xB9E8B4), Hex(0x57AB5A));

        private readonly string Titulo;
        private readonly string Resumen;
        private readonly List<TextoCambiado> Textos;
        /** Los que no entraron en el archivo por el tope del builder (CambiosDeTexto.Tope). */
        private readonly int Resto;
        private Vector2 Desplazamiento;

        // El alto de cada texto depende del ancho, que se sabe recien al dibujar.
        private float AnchoMedido = -1f;
        private readonly List<float> Altos = new List<float>();

        public override Vector2 InitialSize => new Vector2(780f, 660f);

        public VentanaCambios(Novedad Novedad)
        {
            string Archivo = System.IO.Path.Combine(RmlVentanaMod.Instancia.Content.RootDir, "Cambios.tsv");
            Textos = System.IO.File.Exists(Archivo)
                ? CambiosDeMod.Leer(System.IO.File.ReadLines(Archivo), Novedad.PackageId)
                : new List<TextoCambiado>();
            Resto = Math.Max(0, Novedad.Cambiados + Novedad.Nuevos + Novedad.Quitados - Textos.Count);

            Titulo = "RML_Cambios_Titulo".Translate(Novedad.Nombre);
            var Partes = new List<string>();
            if (Novedad.Cambiados > 0) Partes.Add(Cantidad(Novedad.Cambiados, "RML_Cambios_Cambiado", "RML_Cambios_Cambiados"));
            if (Novedad.Nuevos > 0) Partes.Add(Cantidad(Novedad.Nuevos, "RML_Cambios_Nuevo", "RML_Cambios_Nuevos"));
            if (Novedad.Quitados > 0) Partes.Add(Cantidad(Novedad.Quitados, "RML_Cambios_Quitado", "RML_Cambios_Quitados"));
            Resumen = "RML_Cambios_Resumen".Translate(string.Join(", ", Partes));

            doCloseX = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
            onlyOneOfTypeAllowed = true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            float Y = inRect.y;

            Text.Font = GameFont.Medium;
            string TituloCorto = Titulo.Truncate(inRect.width - 40f);
            Widgets.Label(new Rect(inRect.x, Y, inRect.width - 40f, 34f), TituloCorto);
            if (TituloCorto != Titulo)
                TooltipHandler.TipRegion(new Rect(inRect.x, Y, inRect.width - 40f, 34f), Titulo);
            Y += 36f;

            Text.Font = GameFont.Small;
            GUI.color = Gris;
            float AltoResumen = Text.CalcHeight(Resumen, inRect.width);
            Widgets.Label(new Rect(inRect.x, Y, inRect.width, AltoResumen), Resumen);
            GUI.color = Color.white;
            Y += AltoResumen + 12f;

            Rect Lista = new Rect(inRect.x, Y, inRect.width, inRect.yMax - 38f - 12f - Y);
            Widgets.DrawMenuSection(Lista);
            DibujarLista(Lista.ContractedBy(1f));

            if (Widgets.ButtonText(new Rect(inRect.xMax - 160f, inRect.yMax - 38f, 160f, 38f), "RML_Ventana_Cerrar".Translate()))
                Close();

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }

        private void DibujarLista(Rect Lista)
        {
            if (Textos.Count == 0)
            {
                GUI.color = Gris;
                Widgets.Label(new Rect(Lista.x + RellenoTexto, Lista.y + 16f, Lista.width - 2 * RellenoTexto, 48f), "RML_Cambios_SinDetalle".Translate());
                GUI.color = Color.white;
                return;
            }

            float Ancho = Lista.width - 16f;
            Medir(Ancho);
            float AltoResto = Resto > 0 ? 44f : 0f;
            float Total = AltoResto;
            foreach (float Alto in Altos) Total += Alto;

            Rect Vista = new Rect(0f, 0f, Ancho, Total);
            Widgets.BeginScrollView(Lista, ref Desplazamiento, Vista);

            float Y = 0f;
            for (int i = 0; i < Textos.Count; i++)
            {
                Rect Renglon = new Rect(0f, Y, Ancho, Altos[i]);
                Y += Altos[i];

                // Solo lo que se ve: con 200 textos, dibujarlos todos en cada cuadro se nota.
                if (Renglon.yMax < Desplazamiento.y || Renglon.y > Desplazamiento.y + Lista.height)
                    continue;

                DibujarTexto(Renglon, Textos[i], i < Textos.Count - 1 || Resto > 0);
            }

            if (Resto > 0)
            {
                GUI.color = Gris;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(new Rect(RellenoTexto, Y, Ancho - 2 * RellenoTexto, AltoResto), "RML_Cambios_Resto".Translate(Resto));
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.color = Color.white;
            }

            Widgets.EndScrollView();
        }

        private void DibujarTexto(Rect Renglon, TextoCambiado Texto, bool ConLinea)
        {
            float Ancho = Renglon.width - 2 * RellenoTexto;
            float Y = Renglon.y + RellenoTexto;

            if (Texto.Ingles.Length > 0)
            {
                Text.Font = GameFont.Tiny;
                GUI.color = Gris;
                float Alto = Text.CalcHeight(Texto.Ingles, Ancho);
                Widgets.Label(new Rect(Renglon.x + RellenoTexto, Y, Ancho, Alto), Texto.Ingles);
                GUI.color = Color.white;
                Y += Alto + 4f;
            }

            Text.Font = GameFont.Small;
            if (Texto.Antes.Length > 0)
                Y += Bloque(new Rect(Renglon.x + RellenoTexto, Y, Ancho, 0f), "-", Texto.Antes, Rojo) + 2f;
            if (Texto.Ahora.Length > 0)
                Bloque(new Rect(Renglon.x + RellenoTexto, Y, Ancho, 0f), "+", Texto.Ahora, Verde);

            if (ConLinea)
                Widgets.DrawLineHorizontal(Renglon.x + RellenoTexto, Renglon.yMax - 1f, Ancho, Widgets.SeparatorLineColor);
        }

        /** Un renglon del diff: el fondo suave, la franja y el signo a la izquierda y el texto. Devuelve
         *  su alto. Va en la fuente que este puesta (Small). */
        private static float Bloque(Rect Donde, string Signo, string Texto, (Color Fondo, Color Texto, Color Franja) Colores)
        {
            float AnchoTexto = Donde.width - AnchoFranja - AnchoMarca - RellenoBloque;
            float Alto = Text.CalcHeight(Texto, AnchoTexto) + 2 * RellenoBloque;
            Rect Caja = new Rect(Donde.x, Donde.y, Donde.width, Alto);

            Widgets.DrawBoxSolid(Caja, Colores.Fondo);
            Widgets.DrawBoxSolid(new Rect(Caja.x, Caja.y, AnchoFranja, Caja.height), Colores.Franja);

            GUI.color = Colores.Franja;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(new Rect(Caja.x + AnchoFranja, Caja.y + RellenoBloque, AnchoMarca, Alto - 2 * RellenoBloque), Signo);
            Text.Anchor = TextAnchor.UpperLeft;

            GUI.color = Colores.Texto;
            Widgets.Label(new Rect(Caja.x + AnchoFranja + AnchoMarca, Caja.y + RellenoBloque, AnchoTexto, Alto - 2 * RellenoBloque), Texto);
            GUI.color = Color.white;
            return Alto;
        }

        /** El alto de cada texto con este ancho, con las mismas cuentas que DibujarTexto y Bloque. */
        private void Medir(float Ancho)
        {
            if (Mathf.Approximately(Ancho, AnchoMedido))
                return;

            AnchoMedido = Ancho;
            Altos.Clear();
            float AnchoInterior = Ancho - 2 * RellenoTexto;
            float AnchoBloque = AnchoInterior - AnchoFranja - AnchoMarca - RellenoBloque;

            foreach (TextoCambiado Texto in Textos)
            {
                float Alto = 2 * RellenoTexto;
                if (Texto.Ingles.Length > 0)
                {
                    Text.Font = GameFont.Tiny;
                    Alto += Text.CalcHeight(Texto.Ingles, AnchoInterior) + 4f;
                }

                Text.Font = GameFont.Small;
                if (Texto.Antes.Length > 0)
                    Alto += Text.CalcHeight(Texto.Antes, AnchoBloque) + 2 * RellenoBloque + 2f;
                if (Texto.Ahora.Length > 0)
                    Alto += Text.CalcHeight(Texto.Ahora, AnchoBloque) + 2 * RellenoBloque;
                Altos.Add(Alto);
            }

            Text.Font = GameFont.Small;
        }

        private static string Cantidad(int Numero, string Uno, string Varios) =>
            Numero == 1 ? Uno.Translate().ToString() : Varios.Translate(Numero).ToString();

        private static Color Hex(int Rgb) =>
            new Color(((Rgb >> 16) & 0xFF) / 255f, ((Rgb >> 8) & 0xFF) / 255f, (Rgb & 0xFF) / 255f);
    }
}
