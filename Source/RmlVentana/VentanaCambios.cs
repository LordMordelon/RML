using System;
using System.Collections.Generic;
using System.Text;
using RmlVentana.Logica;
using UnityEngine;
using Verse;

namespace RmlVentana
{
    /** «Ver cambios»: los textos de un mod que cambiaron en la ultima subida de RML, como el diff de
     *  VS Code: el renglon de antes con fondo rojo y el de ahora con fondo verde, y dentro de cada uno
     *  las palabras que cambiaron con el mismo color mas fuerte (PLAN-VENTANA.md, etapa 5).
     *
     *  Cambios.tsv se lee recien al abrirla, y solo las lineas de este mod: con la casilla de los
     *  ajustes apagada no se abre nunca, y abierta no pesa al cargar el juego.
     *
     *  Si de la traduccion solo se agrego o solo se quito, va un solo bloque; y el ingles se resalta
     *  igual cuando tambien cambio, asi que sin resaltar dice que el cambio fue solo de la traduccion
     *  (PLAN-VENTANA.md, «Ver cambios» mas claro). */
    public class VentanaCambios : Window
    {
        private const float AnchoFranja = 3f;
        private const float AnchoMarca = 22f;
        private const float RellenoBloque = 4f;
        private const float RellenoTexto = 12f;

        private static readonly Color Gris = Hex(0xA9B0B8);
        private static readonly Colores Rojo = new Colores(Hex(0x4A1F21), Hex(0x8C2F33), Hex(0xF2E6E6), Hex(0xE5534B));
        private static readonly Colores Verde = new Colores(Hex(0x233B23), Hex(0x3F6E2F), Hex(0xE8F2E4), Hex(0x57AB5A));

        private readonly string Titulo;
        private readonly string Resumen;
        private readonly List<Fila> Filas = new List<Fila>();
        /** Los que no entraron en el archivo por el tope del builder (CambiosDeTexto.Tope). */
        private readonly int Resto;
        private Vector2 Desplazamiento;

        // Los renglones dependen del ancho, que se sabe recien al dibujar.
        private float AnchoMedido = -1f;

        public override Vector2 InitialSize => new Vector2(780f, 660f);

        private class Colores
        {
            public readonly Color Fondo, Resalte, Texto, Franja;
            public Colores(Color Fondo, Color Resalte, Color Texto, Color Franja)
            {
                this.Fondo = Fondo;
                this.Resalte = Resalte;
                this.Texto = Texto;
                this.Franja = Franja;
            }
        }

        /** Un texto del archivo, ya partido en palabras, y como queda acomodado con el ancho actual. */
        private class Fila
        {
            public TextoParaVer Texto;
            public Bloque BloqueIngles, BloqueAntes, BloqueAhora;
            /** Ancho tambien: en una sola linea puede pasar el de la ventana. */
            public float Alto, Ancho;
        }

        /** Los renglones de un texto y los rectangulos de las palabras cambiadas, relativos al texto. */
        private class Bloque
        {
            public readonly List<string> Renglones = new List<string>();
            public readonly List<(Rect Caja, bool Quitado)> Resaltes = new List<(Rect, bool)>();
            /** El del renglon mas largo. */
            public float Alto, Ancho;
        }

        private static AjustesRml Ajustes => RmlVentanaMod.Instancia.Ajustes;

        public VentanaCambios(Novedad Novedad)
        {
            string Archivo = Novedad.Archivo ?? System.IO.Path.Combine(RmlVentanaMod.Instancia.Content.RootDir, "Cambios.tsv");
            var Textos = System.IO.File.Exists(Archivo)
                ? CambiosDeMod.Leer(System.IO.File.ReadLines(Archivo), Novedad.PackageId)
                : new List<TextoCambiado>();
            Resto = Math.Max(0, Novedad.Cambiados + Novedad.Nuevos + Novedad.Quitados - Textos.Count);

            foreach (TextoCambiado Texto in Textos)
                Filas.Add(new Fila { Texto = CambiosDeMod.ParaVer(Texto) });

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

            // Alt+Z, como el ajuste de linea de VS Code.
            if (Event.current.type == EventType.KeyDown && Event.current.alt && Event.current.keyCode == KeyCode.Z)
            {
                AlternarAjuste();
                Event.current.Use();
            }

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

            string Casilla = "RML_Cambios_AjustarLineas".Translate();
            Rect Ajuste = new Rect(inRect.x, inRect.yMax - 34f, Text.CalcSize(Casilla).x + 40f, 30f);
            bool Ajustar = !Ajustes.CambiosEnUnaLinea;
            Widgets.CheckboxLabeled(Ajuste, Casilla, ref Ajustar, placeCheckboxNearText: true);
            TooltipHandler.TipRegion(Ajuste, "RML_Cambios_AjustarLineasAyuda".Translate());
            if (Ajustar == Ajustes.CambiosEnUnaLinea)
                AlternarAjuste();

            if (Widgets.ButtonText(new Rect(inRect.xMax - 160f, inRect.yMax - 38f, 160f, 38f), "RML_Ventana_Cerrar".Translate()))
                Close();

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Text.WordWrap = true;
            GUI.color = Color.white;
        }

        private void DibujarLista(Rect Lista)
        {
            if (Filas.Count == 0)
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
            float AnchoContenido = Ancho;
            foreach (Fila Fila in Filas)
            {
                Total += Fila.Alto;
                AnchoContenido = Mathf.Max(AnchoContenido, Fila.Ancho);
            }

            // En una linea, la lista se desplaza tambien hacia el costado, hasta el texto mas largo.
            Rect Vista = new Rect(0f, 0f, AnchoContenido, Total);
            Widgets.BeginScrollView(Lista, ref Desplazamiento, Vista);

            float Y = 0f;
            for (int i = 0; i < Filas.Count; i++)
            {
                Rect Renglon = new Rect(0f, Y, AnchoContenido, Filas[i].Alto);
                Y += Filas[i].Alto;

                // Solo lo que se ve: con 200 textos, dibujarlos todos en cada cuadro se nota.
                if (Renglon.yMax < Desplazamiento.y || Renglon.y > Desplazamiento.y + Lista.height)
                    continue;

                DibujarFila(Renglon, Filas[i], i < Filas.Count - 1 || Resto > 0);
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

        private static void DibujarFila(Rect Renglon, Fila Fila, bool ConLinea)
        {
            float Ancho = Renglon.width - 2 * RellenoTexto;
            float Y = Renglon.y + RellenoTexto;

            if (Fila.BloqueIngles != null)
            {
                // El ingles va sin caja: si cambio, lo quitado y lo agregado se resaltan en su lugar.
                Text.Font = GameFont.Tiny;
                DibujarTexto(Renglon.x + RellenoTexto, Y, Renglon.xMax - RellenoTexto, Fila.BloqueIngles, Gris);
                Y += Fila.BloqueIngles.Alto + 4f;
            }

            Text.Font = GameFont.Small;
            if (Fila.BloqueAntes != null)
            {
                DibujarBloque(new Rect(Renglon.x + RellenoTexto, Y, Ancho, Fila.BloqueAntes.Alto), "-", Fila.BloqueAntes, Rojo);
                Y += Fila.BloqueAntes.Alto + 2f;
            }
            if (Fila.BloqueAhora != null)
                DibujarBloque(new Rect(Renglon.x + RellenoTexto, Y, Ancho, Fila.BloqueAhora.Alto), "+", Fila.BloqueAhora, Verde);

            if (ConLinea)
                Widgets.DrawLineHorizontal(Renglon.x + RellenoTexto, Renglon.yMax - 1f, Ancho, Widgets.SeparatorLineColor);
        }

        /** Un renglon del diff: el fondo suave, la franja y el signo a la izquierda, las palabras
         *  cambiadas resaltadas y el texto encima. Va en la fuente Small, con la que se midio. */
        private static void DibujarBloque(Rect Caja, string Signo, Bloque Bloque, Colores Colores)
        {
            Widgets.DrawBoxSolid(Caja, Colores.Fondo);
            Widgets.DrawBoxSolid(new Rect(Caja.x, Caja.y, AnchoFranja, Caja.height), Colores.Franja);

            float X = Caja.x + AnchoFranja + AnchoMarca, Y = Caja.y + RellenoBloque;
            GUI.color = Colores.Franja;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(new Rect(Caja.x + AnchoFranja, Y, AnchoMarca, Text.LineHeight), Signo);
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;

            DibujarTexto(X, Y, Caja.xMax, Bloque, Colores.Texto);
        }

        /** Los renglones de un bloque desde X, Y, con sus resaltes debajo: rojos los quitados y verdes
         *  los agregados. Va en la fuente con la que se midio. */
        private static void DibujarTexto(float X, float Y, float Hasta, Bloque Bloque, Color ColorTexto)
        {
            foreach (var Resalte in Bloque.Resaltes)
                Widgets.DrawBoxSolid(new Rect(X + Resalte.Caja.x, Y + Resalte.Caja.y, Resalte.Caja.width, Resalte.Caja.height),
                    Resalte.Quitado ? Rojo.Resalte : Verde.Resalte);

            // Sin ajuste de linea: los renglones ya vienen cortados, y el juego no tiene que volver a
            // cortarlos distinto de como se calcularon los resaltes.
            Text.WordWrap = false;
            GUI.color = ColorTexto;
            float Alto = Text.LineHeight;
            for (int i = 0; i < Bloque.Renglones.Count; i++)
                Widgets.Label(new Rect(X - MargenIzquierdo(), Y + i * Alto, Hasta - X + MargenIzquierdo(), Alto), Bloque.Renglones[i]);
            GUI.color = Color.white;
            Text.WordWrap = true;
        }

        /** Entre ajustar las lineas al ancho y dejar cada texto en una. Queda guardado para la proxima. */
        private void AlternarAjuste()
        {
            Ajustes.CambiosEnUnaLinea = !Ajustes.CambiosEnUnaLinea;
            RmlVentanaMod.Instancia.WriteSettings();
            AnchoMedido = -1f;
            Desplazamiento.x = 0f;
        }

        /** Los renglones de cada texto con este ancho, o en una sola linea si asi se eligio. */
        private void Medir(float Ancho)
        {
            if (Mathf.Approximately(Ancho, AnchoMedido))
                return;

            AnchoMedido = Ancho;
            bool UnaLinea = Ajustes.CambiosEnUnaLinea;
            float AnchoInterior = UnaLinea ? float.MaxValue : Ancho - 2 * RellenoTexto;
            float AnchoBloque = UnaLinea ? float.MaxValue : AnchoInterior - AnchoFranja - AnchoMarca - RellenoBloque;
            List<Trozo> Como(List<Trozo> Trozos) => UnaLinea ? DiffDePalabras.EnUnaLinea(Trozos) : Trozos;

            foreach (Fila Fila in Filas)
            {
                Fila.Alto = 2 * RellenoTexto;
                Fila.Ancho = 0f;
                Text.Font = GameFont.Tiny;
                Fila.BloqueIngles = Fila.Texto.Ingles.Count == 0 ? null : Acomodar(Como(Fila.Texto.Ingles), AnchoInterior);
                if (Fila.BloqueIngles != null)
                {
                    Fila.Alto += Fila.BloqueIngles.Alto + 4f;
                    Fila.Ancho = Fila.BloqueIngles.Ancho;
                }

                Text.Font = GameFont.Small;
                Fila.BloqueAntes = Fila.Texto.Antes == null ? null : ConRelleno(Acomodar(Como(Fila.Texto.Antes), AnchoBloque));
                Fila.BloqueAhora = Fila.Texto.Ahora == null ? null : ConRelleno(Acomodar(Como(Fila.Texto.Ahora), AnchoBloque));
                foreach (Bloque Bloque in new[] { Fila.BloqueAntes, Fila.BloqueAhora })
                    if (Bloque != null)
                        Fila.Ancho = Mathf.Max(Fila.Ancho, Bloque.Ancho + AnchoFranja + AnchoMarca + RellenoBloque);
                if (Fila.BloqueAntes != null) Fila.Alto += Fila.BloqueAntes.Alto + 2f;
                if (Fila.BloqueAhora != null) Fila.Alto += Fila.BloqueAhora.Alto;
                Fila.Ancho += 2 * RellenoTexto;
            }

            Text.Font = GameFont.Small;
        }

        /** Corta los trozos en renglones de este ancho, palabra por palabra como el juego, y anota
         *  donde cae cada trozo cambiado. Cada posicion se mide sobre el renglon entero hasta ahi, asi
         *  que el resalte cae justo debajo de la palabra que dibuja el juego. */
        private static Bloque Acomodar(List<Trozo> Trozos, float Ancho)
        {
            var Bloque = new Bloque();
            var Renglon = new List<Trozo>();

            void Cerrar()
            {
                while (Renglon.Count > 0 && Renglon[Renglon.Count - 1].Texto.Trim().Length == 0)
                    Renglon.RemoveAt(Renglon.Count - 1);

                float Y = Bloque.Renglones.Count * Text.LineHeight;
                var Texto = new StringBuilder();
                foreach (Trozo Trozo in Renglon)
                {
                    // Se mide solo alrededor de lo cambiado: en una linea, el renglon es el texto entero.
                    float Desde = Trozo.Cambiado ? AnchoDe(Texto.ToString()) : 0f;
                    Texto.Append(Trozo.Texto);
                    if (Trozo.Cambiado)
                        Bloque.Resaltes.Add((new Rect(Desde, Y, AnchoDe(Texto.ToString()) - Desde, Text.LineHeight), Trozo.Quitado));
                }
                Bloque.Renglones.Add(Texto.ToString());
                Bloque.Ancho = Mathf.Max(Bloque.Ancho, AnchoDe(Texto.ToString()));
                Renglon.Clear();
            }

            string Hasta = "";
            foreach (Trozo Trozo in Trozos)
            {
                bool Espacio = Trozo.Texto.Trim().Length == 0;
                if (Espacio && Trozo.Texto.Contains("\n"))
                {
                    // Un salto (o varios) termina el renglon; los espacios de despues no cuentan.
                    int Saltos = Trozo.Texto.Split('\n').Length - 1;
                    for (int i = 0; i < Saltos; i++) Cerrar();
                    Hasta = "";
                    continue;
                }

                if (Espacio && Renglon.Count == 0)
                    continue;

                if (!Espacio && Renglon.Count > 0 && Ancho < float.MaxValue && AnchoDe(Hasta + Trozo.Texto) > Ancho)
                {
                    Cerrar();
                    Hasta = "";
                }

                Renglon.Add(Trozo);
                Hasta += Trozo.Texto;
            }

            if (Renglon.Count > 0 || Bloque.Renglones.Count == 0)
                Cerrar();

            Bloque.Alto = Bloque.Renglones.Count * Text.LineHeight;
            return Bloque;
        }

        /** Un bloque de la traduccion, que va en su caja de color con un margen arriba y abajo. */
        private static Bloque ConRelleno(Bloque Bloque)
        {
            Bloque.Alto += 2 * RellenoBloque;
            return Bloque;
        }

        /** El ancho del texto en la fuente puesta, sin el margen que el juego le suma a cada medida. */
        private static float AnchoDe(string Texto) =>
            Texto.Length == 0 ? 0f : Text.CalcSize(Texto).x - Margen();

        /** El margen de la etiqueta (izquierdo mas derecho). Cada medida lo incluye una vez, asi que
         *  dos veces «a» menos «aa» deja solo el margen. */
        private static float Margen() => 2 * Text.CalcSize("a").x - Text.CalcSize("aa").x;

        private static float MargenIzquierdo() => Margen() / 2f;

        private static string Cantidad(int Numero, string Uno, string Varios) =>
            Numero == 1 ? Uno.Translate().ToString() : Varios.Translate(Numero).ToString();

        private static Color Hex(int Rgb) =>
            new Color(((Rgb >> 16) & 0xFF) / 255f, ((Rgb >> 8) & 0xFF) / 255f, (Rgb & 0xFF) / 255f);
    }
}
