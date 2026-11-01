# Plan: revisión de Vanilla Races Expanded

Revisión de los 14 mods de Vanilla Races Expanded contra la traducción oficial y el glosario general,
como la de Vanilla Psycasts Expanded (`Data/Vanilla Psycasts Expanded - 2842502659/PLAN-VPE.md`).
Primera pasada del 1-nov-2026. **Estado:** Angel aceptó las propuestas el 1-nov-2026, con una
condición que vale para todo: **antes de aplicar cada una, comparar cómo lo hace el juego**
(`Herramientas/oficial/oficial.py`, y `--contiene` para frases). Si el juego lo dice de otra forma,
gana el juego; si no tiene nada parecido, gana el español natural. Así se hizo en VPE, y la
comparación retiró tres propuestas («toma 12 segundos», «artículo» y «peón»).

**Aplicadas el 1-nov-2026:** las 15 de la primera tabla y las 16 de Lycanthrope, 94 textos en 28
commits, uno por arreglo. Angel eligió la propuesta en las tres que decidía (13, 14 y L15). Lo que
cambió al comparar con el juego:

- **1:** el juego usa «leve» en las etapas de hipotermia y golpe de calor, pero «serio/seria» para
  *serious*, donde VRE dice «grave». Angel eligió «serio/seria», como el juego (1-nov-2026): se cambió en
  Android (congelación y sobrecalentamiento) e Insector (golpe de calor y estrés térmico), 5 textos.
- **2:** «dependencia de la contaminación», con artículo, como la descripción del mismo gen.
- **4:** también «del psicoemisor» más abajo en la misma descripción, y «preparándolo» (era «preparándolos»
  para «el individuo»).
- **6 y 9:** eran más textos que los contados: la etiqueta del trabajo (`label`) en los dos, y en Android
  «Repostar con neutroamina» y «Repostando con TargetA.».
- **12:** «humano basinerte» y «humanos basinertes», como dice el juego («los humanos basinertes»).
- **L2, L3, L5 y L9** van en un solo commit, porque se cruzan en los mismos textos. En el texto corrido
  va «genes de **la** línea germinal», como la oficial (`EndogenesDesc`, `ReimplantXenogerm`); en las
  etiquetas, sin artículo. L9 quedó con la frase oficial exacta: «han sido eliminados o expulsados
  recientemente».
- **L8:** la frase oficial entera: «debe ser derribado antes de poder forzar{PAWN_objective} a reimplantar».
- **L10:** «Se transforma en un xenotipo diferente» en lugar de «Transfórmate»: las descripciones de poderes
  del juego van en tercera persona («Implanta», «Emite»), no en imperativo de tú.

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

## Android, leído entero (1-nov-2026)

Los 720 textos, contra el inglés y contra los textos del juego que copia (hipotermia, golpe de calor,
pérdida de sangre, quemaduras). Glosario:
`Data/!Oskar Potocki/Vanilla Races Expanded - Android - 2975771801/GLOSARIO-ANDROID.md`. La traducción
es buena y casi toda sigue al juego; lo que se propuso (cada una ya comparada con la oficial).

**Aplicadas el 1-nov-2026**, con el visto bueno de Angel: las 21, 81 textos en 21 commits, uno por arreglo
(de 38083f7e a 2966ba01). En A16 Angel eligió «modo de suspensión». A14 fueron los 31 trasfondos.

| # | Inglés | Hoy | Propuesta | Oficial | Textos | Dónde se ve en el juego |
|---|---|---|---|---|---|---|
| A1 | urbworld | ingeniero de urbworld / un urbworld | ingeniero de **mundourbano** / un **mundourbano** | Sí | 2 | Trasfondo en la ficha del personaje |
| A2 | mechlink | soporte de enlace mecánico / enlaces mecánicos | soporte de **mecaenlace** / **mecaenlaces** | Sí | 2 | Componente en la ventana de creación y en la ficha |
| A3 | softscanner | escaneada por un escáner de subnúcleos | por un **flexiescáner de subnúcleos** | Sí | 1 | Salud: descripción de la enfermedad del escaneo |
| A4 | toxic wastepacks | paquetes de residuos tóxicos | **packs** de residuos tóxicos | Sí | 2 | Subrutinas «cero residuos» y «residuos extra» |
| A5 | joy (necesidad) | diversión deshabilitada / necesidad de diversión | **recreación** deshabilitada / de **recreación** | Sí | 4 | Componente; descripción del androide básico |
| A6 | outdoors (necesidad) | exteriores deshabilitados | **aire libre** deshabilitado | Sí | 1 | Componente |
| A7 | berserk | ha entrado en un estado de furia / se volverán locos | se ha vuelto **frenétic{o/a}** / se volverán **frenéticos** | Sí | 2 | Carta del despertar con ánimo bajo; componente sin colapsos |
| A8 | downed person | una persona abatida | una persona **derribada** | Sí | 1 | Ayuda del botón del polianalizador |
| A9 | baseline humans | se limita a imitar a humanos estándar | a humanos **basinertes** | Sí | 1 | Descripción de la estación de creación |
| A10 | No reachable, un-reserved non-prisoner neutrocasket… | No hay neutrocápsula no reservada para prisioneros… (dice otra cosa) | No hay **una neutrocápsula para no prisioneros accesible, sin reservar** y a temperatura segura | Sí (la de la cama) | 1 | Menú del clic derecho al rescatar |
| A11 | Assign this stand to be owned by… | Asignar este soporte para ser propiedad de… | **Asigna este soporte para que sea propiedad de** un androide específico | Sí (la de la cama) | 1 | Botón del soporte |
| A12 | {0} in the {1} / Burned off / Lost to freeze | {0} en {1} / Quemado (PEM) / Perdido por congelación | **la {0} en {1_gender ? el : la} {1}** / Quemado**(a)** (PEM), **Achicharrado(a)** (PEM) / Perdido**(a)** por congelación | Sí (quemadura y congelación) | 5 | Salud, al pasar el mouse por la herida; registro de combate |
| A13 | basic / intermediate / advanced / expert {0} | {0} básico (sale «construcción básico») | **{0} de nivel básico** (y así los otros) | No: el juego usa adjetivos que no cambian («{0} fuerte») | 4 | Componentes de habilidad en la ventana de creación |
| A14 | (trasfondos) | todo en masculino: «fue ensamblado», «fue creado» | con género, como los trasfondos oficiales: «fue ensamblad{PAWN_gender ? o : a}». De paso, 6 frases calcadas («dejado languidecer», «fue dejado oxidarse», «dejando a X soportar»…) | Sí (la forma) | ~29 | Trasfondo de las androides en la ficha |
| A15 | has already been scanned / must also be assigned | ya ha sido escaneado / debe estar asignado | escanead**{PAWN_gender ? o : a}** / asignad**{1_gender ? o : a}** | Sí (la forma) | 2 | Avisos arriba a la izquierda |
| A16 | sleep mode | modo de sueño (5) / modo sueño (1) / modo de suspensión (1) | **modo de suspensión** en los 7 (decide Angel) | No | 7 | Botón del androide, su nombre mientras duerme y la subrutina |
| A17 | About {0}s: | Sobre los {0}s: (sale «Sobre los androide básicos») | **Sobre el tipo {0}:** | No | 1 | Ventana de información del androide |
| A18 | remove component / Removing artificial body part | eliminar componente / Quitando parte del cuerpo artificial | **quitar** componente / Quitando parte **corporal** artificial | No | 2 | Lista de operaciones; lo que hace el médico |
| A19 | Avoid freezing by equipping warm clothes on your android | Evita la congelación equipando ropa abrigada en tu androide | Evita la congelación **vistiendo a tu androide con ropa abrigada** en ambientes fríos | Sí (la de la hipotermia) | 1 | Salud: descripción de la congelación |
| A20 | when neutro loss becomes severe | se vuelve grave | se vuelve **severa** (como su etapa y la hemorragia oficial) | Sí | 1 | Salud: descripción de la pérdida de neutroamina |
| A21 | Consider crafting new reactors and schedule them for reactor replacement | …y programarlos para el reemplazo del reactor (parece que se programan los reactores) | …y **programar el cambio de reactor de esos androides** | No | 1 | Alerta de poca energía |

Quedan como están: «subnúcleo de IA» (*persona subcore*; el oficial *persona core* es «núcleo de IA»),
la categoría «androide» en el almacén (las oficiales también son una palabra: «biónico», «natural») y
el texto de relleno «Es un trozo» (*stub*), que el mod dice que nunca se ve.

## Después

- Leer las descripciones de cada mod contra el inglés, empezando por Android, Insector y Archon,
  que son los más grandes. Conviene hacerlo en sesiones aparte, de a dos o tres mods.
- Glosario por mod a medida que se lean (como `GLOSARIO-VPE.md`).
- Los trasfondos de los otros mods de VRE pueden estar todos en masculino, como estaban los de Android (A14):
  revisarlos con su mod.

## Pendiente aparte: Vanilla Backstories Expanded y los títulos femeninos

Angel quiere revisar si el extractor saca los títulos de mujer de los trasfondos (pedido del 1-nov-2026). Lo que
se vio ese día, sin tocar nada:

- El mod (`Data/!Oskar Potocki/Vanilla Backstories Expanded - 2861806869`) trae 174 trasfondos, y en inglés
  **ninguno** tiene `titleFemale` ni `titleShortFemale`. RML tampoco tiene ninguno: el extractor saca solo los
  campos que el XML del mod trae, así que a una colona le sale el título en masculino. Las descripciones sí usan
  `{PAWN_gender ? … : …}` (147 veces).
- El juego hace lo mismo en inglés (5 `titleFemale` en 784 trasfondos), y aun así la traducción oficial le agrega
  `titleFemale` a 770: el juego acepta inyectar ese campo aunque el inglés no lo traiga.
- Lo que hay que decidir: que el extractor agregue siempre `titleFemale` y `titleShortFemale` a cada `BackstoryDef`
  (es un cambio en el extractor, y vale para todos los mods de RML con trasfondos), o agregarlos a mano solo en
  este mod. Antes, probar en el juego que una traducción con `titleFemale` sobre un trasfondo que no lo trae se ve.

## Próxima sesión (1-nov-2026)

**Objetivo:** leer las descripciones de Vanilla Races Expanded - Insector contra el inglés y el juego, y armar su glosario.
**Estado:** Android leído entero, con glosario y sus 21 correcciones aplicadas (de 38083f7e a 2966ba01, subidas el 1-nov-2026).
Lycanthrope también está leído; los otros 12 mods, no. *serious* ya es «serio/seria».
**Antes de empezarla (Angel):** nada. Si quieres, probar en el juego los
trasfondos de una androide (el género) y el botón «Modo de suspensión».
**Al empezar (Claude):**
1. `git -C RML pull` (otra sesión deja archivos sin commitear: el `rebase.autoStash` los guarda y los devuelve).
2. Insector: 526 textos. Leerlos contra el `EN`, armar `GLOSARIO-INSECTOR.md` en su carpeta y proponer en una tabla
   en el chat, con la columna «Dónde se ve en el juego» y si hay oficial o no. Mirar los trasfondos (género).
3. Comparar cada propuesta con el juego antes de aplicarla (`Herramientas/oficial/oficial.py`, `--contiene`). Aplicar
   con `traducir.py corregir`, **un solo commit para todo Insector** (Angel, 1-nov-2026; cada arreglo en una línea
   del cuerpo), y `traducir.py comprobar` antes del commit. La comprobación
   1 y la 5 muestran a propósito los textos cambiados: tienen que ser solo esos.
4. Archon (374) va en la siguiente. Vanilla Backstories Expanded, cuando Angel lo pida (ver arriba).
**Decisiones de Angel:** ninguna pendiente.
**Guías:** ninguna skill de tarea; reglas en `RML/AGENTS.md` («Editar XML de traducción», «Antes de commitear»).
**Dónde leer:** este plan; `GLOSARIO.md`; `Data/!Oskar Potocki/Vanilla Races Expanded - Lycanthrope - 3114453100/GLOSARIO-LYCANTHROPE.md`
como modelo de glosario.
