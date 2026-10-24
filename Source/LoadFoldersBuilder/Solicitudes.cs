using System.Globalization;
using System.Net;
using System.Text;

namespace LoadFoldersBuilder;

/** Los pedidos de traduccion que llegan con el boton «Solicitar» de la ventana del juego.
 *  Solo sirven para la pagina docs/solicitudes.html y para SinTexto.tsv en output/: no
 *  intervienen en la carga.
 *
 *  Viven en la rama «solicitudes», como Revisados.tsv en «revisados»: cada pedido es un commit, y
 *  asi no llenan el historial de las traducciones. Los escribe solicitud.yml.
 *  - Pedidos.tsv: WorkshopID, packageId, nombre, usuario de GitHub y fecha. Una linea por mod y
 *    persona: si alguien lo vuelve a pedir, cambia la fecha y no suma. Asi se cuentan personas.
 *  - SinTexto.tsv: WorkshopID, packageId, nombre y fecha de los mods pedidos que no tienen nada
 *    que traducir. Salen del grafico y la ventana los muestra como «Sin texto».
 */
public static class Solicitudes
{
    public const string Rama = "solicitudes";
    public const string Pedidos = "Pedidos.tsv";
    public const string SinTexto = "SinTexto.tsv";
    public const string Pagina = "solicitudes.html";
    private const int Cuantos = 20;

    public record Pedido(string WorkshopID, string Nombre, int Personas, string Ultimo);

    /** Las lineas de un archivo de la rama, o null sin la rama. Primero la copia del remoto, que es
     *  la que escribe solicitud.yml (igual que Revisados.Leer). */
    public static List<string>? Leer(string RootPath, string Archivo)
        => UltimosCambios.Git(RootPath, "show", $"origin/{Rama}:{Archivo}")
           ?? UltimosCambios.Git(RootPath, "show", $"{Rama}:{Archivo}");

    /** Los mas pedidos, de mayor a menor, sin los que ya estan en RML ni los que no tienen texto.
     *  A igual cantidad, primero el pedido mas reciente. */
    public static List<Pedido> MasPedidos(IEnumerable<string>? Pedidos, IEnumerable<string>? SinTexto, ICollection<string> EnRml)
    {
        var Fuera = new HashSet<string>(EnRml);
        foreach (var Campos in Filas(SinTexto))
            Fuera.Add(Campos[0]);

        return Filas(Pedidos)
            .Where(Campos => Campos.Length >= 5 && !Fuera.Contains(Campos[0]))
            .GroupBy(Campos => Campos[0])
            .Select(Grupo =>
            {
                var Ultima = Grupo.MaxBy(Campos => Campos[4])!;
                return new Pedido(Grupo.Key, Ultima[2], Grupo.Select(Campos => Campos[3].ToLowerInvariant()).Distinct().Count(), Ultima[4]);
            })
            .OrderByDescending(Pedido => Pedido.Personas)
            .ThenByDescending(Pedido => Pedido.Ultimo, StringComparer.Ordinal)
            .ThenBy(Pedido => Pedido.Nombre, StringComparer.OrdinalIgnoreCase)
            .Take(Cuantos)
            .ToList();
    }

    /** Las lineas con un WorkshopID numerico, partidas por tabulador. El encabezado y una linea a
     *  medio escribir no lo tienen. */
    private static IEnumerable<string[]> Filas(IEnumerable<string>? Lineas)
        => (Lineas ?? [])
            .Select(Linea => Linea.TrimEnd('\r').Split('\t'))
            .Where(Campos => Campos[0].Length > 0 && Campos[0].All(char.IsAsciiDigit));

    /** docs/solicitudes.html: un grafico de barras con los mas pedidos. Autocontenida, como
     *  docs/index.html, y con sus mismos colores. No se commitea: la arma -pagina y la publica
     *  pagina-diaria.yml, que corre tambien despues de cada pedido. */
    public static string Html(List<Pedido> Lista)
    {
        var Barras = new StringBuilder();
        int Maximo = Lista.Count > 0 ? Lista.Max(Pedido => Pedido.Personas) : 1;
        for (int i = 0; i < Lista.Count; i++)
        {
            var Pedido = Lista[i];
            string Nombre = WebUtility.HtmlEncode(Pedido.Nombre);
            string Enlace = $"https://steamcommunity.com/sharedfiles/filedetails/?id={Pedido.WorkshopID}";
            string Ancho = (100.0 * Pedido.Personas / Maximo).ToString("0.#", CultureInfo.InvariantCulture);
            string Personas = Pedido.Personas == 1 ? "1 persona" : $"{Pedido.Personas} personas";
            string Ayuda = WebUtility.HtmlEncode($"{Pedido.Nombre}: {Personas}, último pedido el {Pedido.Ultimo}");

            Barras.AppendLine($"<li title=\"{Ayuda}\"><span class=\"puesto\">{i + 1}</span><a class=\"nombre\" href=\"{Enlace}\">{Nombre}</a><span class=\"pista\"><span class=\"barra\" style=\"width:{Ancho}%\"></span></span><span class=\"cifra\">{Pedido.Personas}</span></li>");
        }

        string Contenido = Lista.Count > 0
            ? $"<ol class=\"grafico\" aria-label=\"Mods más pedidos y cuántas personas pidió cada uno\">\n{Barras.ToString().TrimEnd()}\n</ol>"
            : "<p class=\"vacio\">Todavía no hay pedidos.</p>";

        var Pagina = $$"""
            <!doctype html>
            <!-- GENERADO por Source/LoadFoldersBuilder: no editar a mano. -->
            <html lang="es">
            <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1">
            <title>Mods más pedidos · Traducciones [Español Latino]</title>
            <meta name="description" content="Los mods de RimWorld que más jugadores pidieron traducir al español latinoamericano.">
            <style>
            :root {
              color-scheme: light dark;
              --fondo: #f6f6f4; --panel: #ffffff; --texto: #1c1c1e; --suave: #6a6a70; --borde: #e3e3e0;
              --acento: #0b62d6; --pista: #eeeeeb;
            }
            @media (prefers-color-scheme: dark) {
              :root {
                --fondo: #131315; --panel: #1b1b1e; --texto: #ececef; --suave: #9c9ca3; --borde: #2d2d32;
                --acento: #7ab0ff; --pista: #242428;
              }
            }
            * { box-sizing: border-box; }
            body { margin: 0; padding: 40px 16px 56px; background: var(--fondo); color: var(--texto);
              font: 15px/1.5 system-ui, -apple-system, "Segoe UI", Roboto, sans-serif; }
            main { max-width: 960px; margin: 0 auto; }
            main > p { max-width: 80ch; }
            h1 { font-size: 1.75rem; line-height: 1.2; margin: 0 0 8px; }
            p { margin: 0 0 12px; }
            .suave { color: var(--suave); }
            a { color: var(--acento); text-decoration: none; }
            a:hover { text-decoration: underline; }
            .panel { margin: 24px 0 0; padding: 8px 0; background: var(--panel); border: 1px solid var(--borde); border-radius: 10px; }
            .grafico { list-style: none; margin: 0; padding: 0; }
            .grafico li { display: grid; grid-template-columns: 2.5ch minmax(0, 2fr) minmax(120px, 3fr) 4ch;
              align-items: center; gap: 4px 14px; padding: 7px 16px; }
            .grafico li:hover { background: var(--fondo); }
            .puesto, .cifra { font-variant-numeric: tabular-nums; }
            .puesto { color: var(--suave); text-align: right; }
            .cifra { font-weight: 600; }
            .nombre { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
            .pista { height: 14px; background: var(--pista); border-radius: 0 4px 4px 0; }
            .barra { display: block; height: 100%; min-width: 4px; background: var(--acento); border-radius: 0 4px 4px 0; }
            .vacio { padding: 24px 16px; margin: 0; text-align: center; color: var(--suave); }
            footer { margin-top: 20px; }
            @media (max-width: 640px) {
              .grafico li { grid-template-columns: 2.5ch minmax(0, 1fr) 4ch; padding: 8px 12px; }
              .nombre { grid-column: 2 / 4; }
              .pista { grid-column: 2; grid-row: 2; }
              .cifra { grid-row: 2; grid-column: 3; }
            }
            </style>
            </head>
            <body>
            <main>
            <h1>Mods más pedidos</h1>
            <p class="suave">Los mods que más jugadores pidieron traducir en
            <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=3541009729">Traducciones [Español Latino]</a>.
            Cada barra cuenta personas, no pedidos: quien pide el mismo mod dos veces suma una sola vez.
            El nombre de cada mod abre su página en Steam.</p>
            <p class="suave">Para pedir uno, abre la ventana de la traducción en el juego (Opciones → Configuración de mods)
            y aprieta «Solicitar» junto al mod. Cuando se traduce, sale de esta lista.</p>

            <div class="panel">
            {{Contenido}}
            </div>

            <footer class="suave">
            <p><a href="./">Ver los mods que ya están traducidos</a></p>
            </footer>
            </main>
            </body>
            </html>
            """;

        return Pagina.ReplaceLineEndings();
    }
}
