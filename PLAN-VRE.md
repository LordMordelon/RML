# Plan: revisión de Vanilla Races Expanded

Revisión de los 14 mods de Vanilla Races Expanded contra la traducción oficial y el glosario general,
como la de Vanilla Psycasts Expanded (`Data/Vanilla Psycasts Expanded - 2842502659/PLAN-VPE.md`).
Primera pasada del 1-nov-2026. **Estado:** propuesto; no se cambió ningún texto.

Ninguno de los 14 tiene `TODO`. Son 3012 textos: Android 716, Insector 526, Archon 374,
Sanguophage 313, Saurid y Lycanthrope 153, Phytokin 146, Fungoid 129, Waster 124, Highmate 113,
Genie 81, Starjack 67, Pigskin 59 y Hussar 58.

## Cómo se buscó

1. Los textos con el mismo inglés que un texto del juego: 144 traducidos distinto. Casi todos están
   bien: títulos de trasfondos en femenino («médica», donde la oficial solo tiene «médico»), piezas
   de nombres de xenotipos y genes (`symbolPack`) y partes de armas.
2. Cómo se traduce cada término de Biotech y Royalty en los 14 mods, contra el glosario general.

Falta una lectura de las descripciones mod por mod (ver «Después»).

## Correcciones propuestas

| # | Mod | Inglés | Hoy | Propuesta | Textos | Dónde se ve en el juego |
|---|---|---|---|---|---|---|
| 1 | Waster, Android, Insector, Fungoid | minor / major (etapas) | menor / mayor | **leve** / **grave** (oficial) | ~20 | Pestaña Salud: «… (leve)» |
| 2 | Insector | pollution | polución | **contaminación** (oficial; el resto de VRE ya la usa 66 veces) | 6 | La necesidad del gen en Necesidad; el gen y su estado en Salud |
| 3 | Archon | psylink | psienlace | **psicoenlace** (oficial; el glosario dice «NO psienlace») | 3 | El gen «psicoenlace natural» y su descripción |
| 4 | Sanguophage | psycasters | los psíquicos en mortisueño | los **psicoemisores** en mortisueño | 1 | Descripción del pozo de psicofluido |
| 5 | Android | psychically deaf | psíquicamente sordo | **sordera psíquica** (oficial) | 1 | El componente del androide en la ventana de creación y en su ficha |
| 6 | Android | operate on | operar en / operando en | **operar a** / **operando a** (oficial) | 2 | Menú del clic derecho y lo que hace el colono |
| 7 | Android | Scanning sickness | malestar por escaneo | **enfermedad del escaneo** (oficial) | 1 | Pestaña Salud |
| 8 | Sanguophage | piercing spine / acid spray (de un solo uso) | espina perforante / rociada de ácido | **columna punzante** / **spray ácido** (los poderes oficiales que copian) | 2 | Botón del poder |
| 9 | Sanguophage | refuel | reabastecer / reabasteciendo | **repostar** / **repostando** (oficial) | 2 | Menú del clic derecho y lo que hace el colono |
| 10 | Saurid | town guard / councilman / trader | … de la ciudad | guardia / concejal / comerciante **citadino** (oficial) | 3 | Nombre del personaje en las ciudades sáuridas |
| 11 | Pigskin | muddy (color de piel) | turbio | **fangoso** (oficial) | 4 | Nombre del gen de color de piel |
| 12 | Pigskin, Hussar | baseliner | humano base / humanos de línea base | **basinerte** (oficial, `Baseliner`) | 4 | Descripción de los genes de envejecimiento y del gen gigante |
| 13 | Highmate, Saurid | danger / crush (peinados) | danger / crush (en inglés) | **peligro** / **flechazo** (decide Angel) | 2 | Nombre del peinado al crear o editar un personaje |
| 14 | Archon | weak / high / extreme entropy capacity | capacidad de entropía débil / alta / extrema | capacidad de **calor neural** débil / alta / extrema (decide Angel: el gen cambia el calor neural, que la oficial nunca llama «entropía») | 3 | Nombre del gen |
| 15 | Phytokin | Natural Meditation focus | foco de Meditación Natural | **enfoque de meditación natural** (oficial) | 2 | Efecto del gen al pasar el mouse |
| 16 | Todos | pawn | peón / peones | **colono(s)** o **persona(s)**, como en VPE | 15 | Efectos de los genes al pasar el mouse; avisos |

Quedan como están, aunque el cotejo los marcó: «piel de corteza» (*barkskin*; el «dermocortezana»
oficial es un modo del árbol gauranlen, no un gen), «drenaje de hemógeno» (es un edificio, no el
gen oficial), «quemadura» (*sear*), los títulos en femenino y las piezas de nombres de genes.

## Después

- Aprobadas las correcciones, aplicarlas con la tabla clave → viejo → nuevo, un commit por arreglo, y
  las cinco comprobaciones.
- Leer las descripciones de cada mod contra el inglés, empezando por Android, Insector y Archon,
  que son los más grandes. Conviene hacerlo en sesiones aparte, de a dos o tres mods.
- Glosario por mod a medida que se lean (como `GLOSARIO-VPE.md`).
