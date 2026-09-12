using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace LoadFoldersBuilder;

/** Las tres formas en que se publica la lista de mods. Todas salen de las mismas filas, ya
 *  ordenadas de la traduccion mas reciente a la mas antigua.
 */
public static class ModListFormatos
{
    /** ModList.tsv: texto plano, para leer desde otro programa. */
    public static string Tsv(List<FilaDeModList> Filas)
    {
        // Con encabezado: GitHub muestra el .tsv como tabla y toma la primera fila como
        // titulos, asi que sin esto el primer mod quedaba de encabezado.
        // Ultima actualizacion es la del mod en Steam; ultima traduccion, la del commit en RML.
        // Si la primera es mas nueva, la traduccion puede haber quedado atrasada.
        const string Encabezado = "WorkshopID\tMod\tCarpeta\tPackageID\tÚltima actualización\tÚltima traducción";
        return string.Join(Environment.NewLine, Filas
            .Select(Fila => $"{Fila.WorkshopID ?? "No ID"}\t{Fila.Nombre}\t{Fila.Carpeta}\t{Fila.PackageID}\t{Fila.Actualizacion}\t{Fila.Traduccion}")
            .Prepend(Encabezado));
    }

    /** ModList.md: la lista vista desde GitHub. Un .tsv no admite links; aca el nombre del mod
     *  lleva a su pagina de Steam sin sumar una columna con la URL entera.
     */
    public static string Markdown(List<FilaDeModList> Filas)
    {
        var Texto = new StringBuilder();
        Texto.AppendLine("<!-- GENERADO por Source/LoadFoldersBuilder: no editar a mano. -->");
        Texto.AppendLine();
        Texto.AppendLine("# Mods traducidos");
        Texto.AppendLine();
        Texto.AppendLine($"{Filas.Count} mods, de la traducción más reciente a la más antigua. El nombre de cada mod lleva a su página en Steam.");
        Texto.AppendLine();
        Texto.AppendLine("- **Última actualización:** cuándo el autor actualizó el mod en Steam.");
        Texto.AppendLine("- **Última traducción:** el último cambio de la traducción en RML. Si es anterior a la última actualización, la traducción puede haber quedado atrasada.");
        Texto.AppendLine();
        Texto.AppendLine("| WorkshopID | Mod | Carpeta | PackageID | Última actualización | Última traducción |");
        Texto.AppendLine("|---|---|---|---|---|---|");

        foreach (var Fila in Filas)
        {
            string Mod = Fila.Steam is { } Steam
                ? $"[{EscaparMarkdown(Fila.Nombre)}]({Steam})"
                : EscaparMarkdown(Fila.Nombre);

            Texto.AppendLine($"| {EscaparMarkdown(Fila.WorkshopID ?? "No ID")} | {Mod} | {EscaparMarkdown(Fila.Carpeta)} | {EscaparMarkdown(Fila.PackageID)} | {Fila.Actualizacion} | {Fila.Traduccion} |");
        }

        return Texto.ToString();
    }

    /** Nombres como "[sbz] Fridge" o con | romperian el link o la tabla, y un * o _ los pondria en cursiva. */
    private static string EscaparMarkdown(string Texto)
        => Regex.Replace(Texto, @"[\\`*_\[\]<>|]", Coincidencia => "\\" + Coincidencia.Value);

    /** docs/index.html: la lista para quien llega desde Steam, publicada con GitHub Pages.
     *  Se ordena por columna y se filtra, cosa que ni el .md ni el .tsv permiten dentro de GitHub.
     *
     *  Autocontenida a proposito: sin CDN ni librerias, para que no se rompa sola con el tiempo.
     *  La tabla va entera en el HTML (se ve aunque no haya JS) y una fila por linea, asi el diff
     *  de git muestra que mods cambiaron. No lleva fecha de generacion: cambiaria en cada corrida
     *  y la CI haria un commit aunque no hubiera nada nuevo.
     */
    public static string Html(List<FilaDeModList> Filas)
    {
        var FilasHtml = new StringBuilder();
        foreach (var Fila in Filas)
        {
            string Nombre = WebUtility.HtmlEncode(Fila.Nombre);
            string Mod = Fila.Steam is { } Steam
                ? $"<a href=\"{WebUtility.HtmlEncode(Steam)}\">{Nombre}</a>"
                : Nombre;
            if (Fila.PosiblementeAtrasada)
                Mod += " <span class=\"atrasada\">posiblemente atrasada</span>";

            string Buscar = WebUtility.HtmlEncode($"{Fila.Nombre} {Fila.PackageID}".ToLowerInvariant());
            string Atrasada = Fila.PosiblementeAtrasada ? " data-atrasada" : "";

            FilasHtml.AppendLine($"<tr data-buscar=\"{Buscar}\"{Atrasada}><td data-valor=\"{Nombre}\">{Mod}</td><td class=\"pid\">{WebUtility.HtmlEncode(Fila.PackageID)}</td><td class=\"fecha\">{Fila.Actualizacion}</td><td class=\"fecha\">{Fila.Traduccion}</td></tr>");
        }

        int Atrasadas = Filas.Count(Fila => Fila.PosiblementeAtrasada);

        var Pagina = $$"""
            <!doctype html>
            <!-- GENERADO por Source/LoadFoldersBuilder: no editar a mano. -->
            <html lang="es">
            <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1">
            <title>Mods traducidos · Traducciones [Español Latino]</title>
            <meta name="description" content="Lista de los {{Filas.Count}} mods de RimWorld traducidos al español latinoamericano en Traducciones [Español Latino].">
            <style>
            :root {
              color-scheme: light dark;
              --fondo: #f6f6f4; --panel: #ffffff; --texto: #1c1c1e; --suave: #6a6a70; --borde: #e3e3e0;
              --acento: #0b62d6; --aviso-fondo: #fdf0cc; --aviso-texto: #6e4b00;
            }
            @media (prefers-color-scheme: dark) {
              :root {
                --fondo: #131315; --panel: #1b1b1e; --texto: #ececef; --suave: #9c9ca3; --borde: #2d2d32;
                --acento: #7ab0ff; --aviso-fondo: #3b2f10; --aviso-texto: #f3cd70;
              }
            }
            * { box-sizing: border-box; }
            body { margin: 0; padding: 40px 16px 56px; background: var(--fondo); color: var(--texto);
              font: 15px/1.5 system-ui, -apple-system, "Segoe UI", Roboto, sans-serif; }
            main { max-width: 980px; margin: 0 auto; }
            h1 { font-size: 1.75rem; line-height: 1.2; margin: 0 0 8px; }
            p { margin: 0 0 12px; }
            .suave { color: var(--suave); }
            a { color: var(--acento); text-decoration: none; }
            a:hover { text-decoration: underline; }
            .leyenda { margin: 0 0 24px; padding-left: 20px; color: var(--suave); }
            .leyenda strong { color: var(--texto); font-weight: 600; }
            .controles { display: flex; flex-wrap: wrap; align-items: center; gap: 10px 20px; margin: 0 0 12px; }
            .controles input[type=search] { flex: 1 1 260px; min-width: 0; padding: 9px 12px; font: inherit; color: inherit;
              background: var(--panel); border: 1px solid var(--borde); border-radius: 8px; }
            .controles label { display: flex; align-items: center; gap: 6px; cursor: pointer; white-space: nowrap; }
            .contador { margin-left: auto; color: var(--suave); font-variant-numeric: tabular-nums; white-space: nowrap; }
            .tabla { overflow-x: auto; background: var(--panel); border: 1px solid var(--borde); border-radius: 10px; }
            table { width: 100%; border-collapse: collapse; }
            th, td { padding: 9px 14px; text-align: left; vertical-align: top; border-bottom: 1px solid var(--borde); }
            tbody tr:last-child td { border-bottom: 0; }
            th { font-weight: 600; white-space: nowrap; }
            th button { all: unset; cursor: pointer; }
            th button:focus-visible { outline: 2px solid var(--acento); outline-offset: 2px; border-radius: 3px; }
            th button::after { content: " ↕"; color: var(--suave); }
            th[aria-sort=ascending] button::after { content: " ↑"; color: var(--texto); }
            th[aria-sort=descending] button::after { content: " ↓"; color: var(--texto); }
            td.pid { color: var(--suave); font-size: .85em; overflow-wrap: anywhere; }
            td.fecha { white-space: nowrap; font-variant-numeric: tabular-nums; }
            .atrasada { display: inline-block; margin-left: 4px; padding: 0 7px; border-radius: 999px; font-size: .75em;
              white-space: nowrap; background: var(--aviso-fondo); color: var(--aviso-texto); }
            .vacio { padding: 24px 14px; text-align: center; color: var(--suave); }
            footer { margin-top: 20px; }
            @media (max-width: 640px) {
              th.pid, td.pid { display: none; }
              th, td { padding: 8px 10px; }
              th { white-space: normal; }
            }
            </style>
            </head>
            <body>
            <main>
            <h1>Mods traducidos</h1>
            <p class="suave">Los {{Filas.Count}} mods que traduce al español latinoamericano
            <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=3541009729">Traducciones [Español Latino]</a>.
            El nombre de cada mod abre su página en Steam.</p>
            <ul class="leyenda">
            <li><strong>Última actualización:</strong> cuándo el autor actualizó el mod en Steam.</li>
            <li><strong>Última traducción:</strong> el último cambio de su traducción. Si es anterior a la actualización del mod, la fila lleva la etiqueta <span class="atrasada">posiblemente atrasada</span> ({{Atrasadas}} en total).</li>
            </ul>

            <div class="controles" hidden>
            <input type="search" id="buscar" placeholder="Buscar por nombre o packageId" aria-label="Buscar mod">
            <label><input type="checkbox" id="atrasadas"> Solo posiblemente atrasadas</label>
            <span class="contador" id="contador" aria-live="polite"></span>
            </div>

            <div class="tabla">
            <table>
            <thead>
            <tr>
            <th data-col="0" data-tipo="texto"><button type="button">Mod</button></th>
            <th data-col="1" data-tipo="texto" class="pid"><button type="button">PackageID</button></th>
            <th data-col="2" data-tipo="fecha"><button type="button">Última actualización</button></th>
            <th data-col="3" data-tipo="fecha" aria-sort="descending"><button type="button">Última traducción</button></th>
            </tr>
            </thead>
            <tbody>
            {{FilasHtml.ToString().TrimEnd()}}
            </tbody>
            </table>
            <p class="vacio" id="vacio" hidden>Ningún mod coincide con la búsqueda.</p>
            </div>

            <footer class="suave">
            <p>¿Encontraste un error o falta un mod? Avisa en <a href="https://github.com/LordMordelon/RML/issues">GitHub</a>
            o en los comentarios de <a href="https://steamcommunity.com/sharedfiles/filedetails/?id=3541009729">Steam</a>.</p>
            </footer>
            </main>

            <script>
            (() => {
              const cuerpo = document.querySelector("tbody");
              const filas = Array.from(cuerpo.rows);
              const buscar = document.getElementById("buscar");
              const soloAtrasadas = document.getElementById("atrasadas");
              const contador = document.getElementById("contador");
              const vacio = document.getElementById("vacio");
              const encabezados = Array.from(document.querySelectorAll("th[data-col]"));
              const valor = (fila, col) => fila.cells[col].dataset.valor ?? fila.cells[col].textContent;
              const comparar = (a, b) => a.localeCompare(b, "es", { sensitivity: "base", numeric: true });

              function filtrar() {
                const texto = buscar.value.trim().toLowerCase();
                let visibles = 0;
                for (const fila of filas) {
                  const ve = (!texto || fila.dataset.buscar.includes(texto))
                    && (!soloAtrasadas.checked || "atrasada" in fila.dataset);
                  fila.hidden = !ve;
                  if (ve) visibles++;
                }
                contador.textContent = visibles + " de " + filas.length + " mods";
                vacio.hidden = visibles > 0;
              }

              function ordenar(th) {
                const col = Number(th.dataset.col);
                const actual = th.getAttribute("aria-sort");
                // Primer clic: fechas de la mas nueva a la mas vieja, texto de la A a la Z.
                const direccion = actual
                  ? (actual === "ascending" ? "descending" : "ascending")
                  : (th.dataset.tipo === "fecha" ? "descending" : "ascending");
                encabezados.forEach(h => h.removeAttribute("aria-sort"));
                th.setAttribute("aria-sort", direccion);
                const signo = direccion === "ascending" ? 1 : -1;

                filas.sort((a, b) => {
                  const va = valor(a, col), vb = valor(b, col);
                  if (!va !== !vb) return va ? -1 : 1; // sin dato, siempre al final
                  return signo * comparar(va, vb) || comparar(valor(a, 0), valor(b, 0));
                });
                cuerpo.append(...filas);
              }

              encabezados.forEach(th => th.querySelector("button").addEventListener("click", () => ordenar(th)));
              buscar.addEventListener("input", filtrar);
              soloAtrasadas.addEventListener("change", filtrar);
              document.querySelector(".controles").hidden = false;
              filtrar();
            })();
            </script>
            </body>
            </html>
            """;

        // Mismos saltos de linea en local y en la CI, sin importar como haya quedado este
        // archivo al clonarlo: si no, git veria la pagina entera como cambiada.
        return Pagina.ReplaceLineEndings("\n");
    }
}
