# Plan: Vanilla Plants Expanded

Traducir Vanilla Plants Expanded (Workshop 2134308522, `VanillaExpanded.VPlantsE`) entero. Lo pidió
Angel el 1-nov-2026, con el mod recién extraído y sin nada traducido.

## Contexto

- **Extracción nueva:** la carpeta no estaba en RML; está en `Data/!Oskar Potocki/` con los demás VE.
- **Sin `UNUSED.xml`**: no había traducción anterior que conservar.
- **De qué trata:** 13 cultivos y árboles frutales nuevos, con sus frutos, un terreno de tierra
  labrada que sube la fertilidad, y las ciruelas que se secan solas en ciruelas pasas. Cada
  descripción termina con un «Efecto de juego:» en amarillo.
- **El parche de RimCuisine** (`Patches/`, solo con RimCuisine 2 Core activo) cambia cada planta por
  una variedad: «granny smith apple tree», «sakura tree», etc.
- Los términos están en [GLOSARIO-VANILLA-PLANTS.md](GLOSARIO-VANILLA-PLANTS.md). Lo que el mod copia de
  vanilla usa el texto oficial.

## Cifras (contadas el 1-nov-2026)

| Qué | Cuántas |
|---|---|
| Pendientes (`TODO`) | 146, 3299 palabras de inglés |
| En `DefInjected` | 89, en 4 archivos (lo más grande, `ThingDef` con 76) |
| En `Patches` (RimCuisine) | 53 |
| En `Keyed` (el secado de las ciruelas) | 4 |

## Etapas

| # | Etapa | Tamaño | Estado |
|---|---|---|---|
| 1 | Glosario del mod | chica | hecha: [GLOSARIO-VANILLA-PLANTS.md](GLOSARIO-VANILLA-PLANTS.md) |
| 2 | Traducir todo, con tabla (`traducir.py tabla` / `aplicar`) | chica | hecha (1-nov-2026) |
| 3 | Verificar: las cinco comprobaciones | chica | hecha |
| 4 | Confirmar términos y probar en partida | chica | falta: Angel |

### 2. Decisiones al traducir

- **Lo copiado de vanilla va con el texto oficial**: la descripción de la hierba («Crece en cualquier
  sitio donde haya un poco de luz y suelo fértil»), la de la hierba alta («Reduce considerablemente la
  velocidad de cualquiera que camine sobre ella») y la de las bayas.
- **«Opuntis»** es una errata del inglés por *opuntias*.
- **"prickly pear" cactus** queda como el cactus "nopal", que es como se llama en español; su fruto, la
  «tuna». Comillas "…" como la oficial.

### 4. Para confirmar con Angel

- Confirmados por Angel el 1-nov-2026: «chiles» (*peppers*), «arvejas» (*peas*; «guisantes» es de
  España) y «damascos» (*apricots*, en lugar de «albaricoques»; el árbol, «árbol de damasco»).
- Probar en partida el secado de las ciruelas (los cuatro `Keyed`, con la duración en `{0}`).

## Para más adelante

- **El parche de RimCuisine puede no verse.** RimWorld aplica los `Patches` y después inyecta los
  `DefInjected`, así que con RimCuisine el `DefInjected` de este mismo mod («manzano») pisa la
  traducción del parche («manzano granny smith»). Es el mismo caso de «Un `Patches` no le puede ganar
  a la traducción oficial» del `AGENTS.md` del extractor, pero dentro de un mod. Probarlo con
  RimCuisine antes de tocar nada.
