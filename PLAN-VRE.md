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
| ~~16~~ | Todos | pawn | peón / peones | **Retirada**: la oficial también usa «peón» cuando el inglés dice *pawn* («Ordena a un peón activar el generador de escudo», Odyssey) y el glosario general lo admite | 15 | — |

Quedan como están, aunque el cotejo los marcó: «piel de corteza» (*barkskin*; el «dermocortezana»
oficial es un modo del árbol gauranlen, no un gen), «drenaje de hemógeno» (es un edificio, no el
gen oficial), «quemadura» (*sear*), los títulos en femenino y las piezas de nombres de genes.

## Lycanthrope, leído entero (1-nov-2026)

Los 153 textos, contra el inglés y contra los textos de Biotech que copia. Parte la tradujo Angel a
mano. Glosario: `Data/!Oskar Potocki/Vanilla Races Expanded - Lycanthrope - 3114453100/GLOSARIO-LYCANTHROPE.md`.
La traducción es buena; lo que se propone:

| # | Inglés | Hoy | Propuesta | Por qué | Dónde se ve en el juego |
|---|---|---|---|---|---|
| L1 | 1st of Septober / 1st of Aprimay | el 1 de **Septiembre** / el 1 de **Aprimayo** | el 1 de **Septobre** / el 1 de **Abrimay** | Los cuadrimestres oficiales; «Septiembre» es un mes que el juego no tiene | Descripción del gen de transformación estacional |
| L2 | endogene loss shock | choque por pérdida de endogenes | **shock de pérdida de genes de línea germinal** | Oficial: «shock de pérdida de genes» | Pestaña Salud; aviso «sufre de…» |
| L3 | gene loss shock (en la carta) | sufrirá un choque por pérdida de genes | sufrirá un **shock de pérdida de genes** | Frase oficial exacta | Carta al implantar |
| L4 | endogermination coma | coma de endogerminación | **coma endogerminativo** | Como el «coma xenogerminativo» oficial | Pestaña Salud; carta al implantar |
| L5 | endogenes | endogenes | **genes de línea germinal** | Oficial (`Endogenes`) | Aviso «no tiene … disponibles», carta, descripción del choque |
| L6 | superior dark vision | visión nocturna superior | **visión oscura superior** | Oficial: *dark vision* es «visión oscura». La descripción copia la oficial: «penalizaciones del humor», «amplía su capacidad» | Nombre del gen |
| L7 | drop pods | cápsulas de transporte | **cápsulas de escape** | Oficial; la cápsula de transporte es otra cosa | Carta de los refuerzos lobunos |
| L8 | downed | debe estar incapacitado | debe estar **derribado** | Oficial en el mismo aviso de Biotech | Aviso al forzar la implantación |
| L9 | removed (genes) | fueron removidos | fueron **eliminados** | Frase oficial; «remover» es un calco | Descripción del choque |
| L10 | Implant… / Emit… / Morph into… / Force this person… | Implantar… / Emitir… / Transformarse en… / Forzar a esta persona… | **Implanta…** / **Emite…** / **Transfórmate en…** / **Obliga a esta persona…** | Las descripciones de poderes oficiales van en imperativo | Al pasar el mouse por los botones de los poderes |
| L11 | killed pack member / sold pack member | mató a miembro de la manada / vendió miembro de la manada | mató **a un** miembro de la manada / vendió **a un** miembro de la manada | Falta el artículo | Pestaña Social, opinión |
| L12 | speech inability | incapacidad del habla | **incapacidad de hablar** | Más natural | Nombre del gen |
| L13 | Extra-capable in perception | Con una percepción extra capaz | Con una percepción **excepcional** | Calco | Descripción de dos genes de esquiva |
| L14 | …results in being hit by every swing - even the one that would miss | resulta en ser golpeado por cada golpe, incluso aquellos que fallarían | hace que **reciban cada golpe**, incluso los que fallarían | Calco | Descripción del gen sin esquiva |
| L15 | panic-fleeing light | pánico (huyendo de la luz) | **huida despavorida (de la luz)** (decide Angel) | Como la oficial *panic fleeing* | Encima del personaje |
| L16 | (descripción del xenotipo) | …hecha especialmente turbia… El niño nació pero fue abandonado…, pero logró… | …**aún más turbia porque**… El niño nació **y** fue abandonado…, **aunque** logró… | Dos «pero» seguidos y un calco | Ventana del xenotipo |

## Después

- Aprobadas las correcciones, aplicarlas con la tabla clave → viejo → nuevo, un commit por arreglo, y
  las cinco comprobaciones.
- Leer las descripciones de cada mod contra el inglés, empezando por Android, Insector y Archon,
  que son los más grandes. Conviene hacerlo en sesiones aparte, de a dos o tres mods.
- Glosario por mod a medida que se lean (como `GLOSARIO-VPE.md`).
