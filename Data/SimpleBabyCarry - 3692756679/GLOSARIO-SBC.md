# Glosario de SimpleBabyCarry

Complementa a [GLOSARIO.md](../../GLOSARIO.md), que sigue mandando: si un término está en los dos,
gana el general. Acá va lo que el mod agrega: llevar bebés en brazos o en portabebés, sus estados de
ánimo y sus opciones. Se tradujo entero el 27-oct-2026.

Origen de cada fila:

- **oficial**: traducción oficial del juego (Core y Biotech), buscada con `Herramientas/oficial/`.
- **GLOSARIO**: el glosario general de RML.
- **propuesta**: sin nombre previo; se usa así hasta que Angel lo cambie acá.

Los pensamientos no saben si el colono es hombre o mujer: se escriben sin adjetivos que cambien con
el género («Me hirieron de gravedad», no «Estoy muy herido»).

## Bebés y cuidado

| Inglés | Español | Origen | Notas |
|---|---|---|---|
| baby | bebé | oficial | «el bebé»; el juego no le marca género |
| child / kid | niño | oficial | En los nombres de prenda, «kid's» va como adjetivo: ver «Prendas» |
| crib | cuna | oficial | |
| sleeping spot / sleeping place | lugar para dormir | oficial | |
| growth vat | tanque de crecimiento | oficial | |
| childcare | cuidar niños | oficial | Es el tipo de trabajo de Biotech: «(no puede cuidar niños)» |
| childcarer | cuidador | propuesta | Quien está asignado a cuidar niños. Femenino «cuidadora» con `{X_gender ? o : a}` si el texto lo recibe |
| breastfeed / nurse-feed | amamantar | oficial | «amamantando a TargetA.» (`Breastfeed.reportString`) |
| feed | alimentar | oficial | |
| baby food | comida para bebés | oficial | |
| carry | llevar | oficial | «llevando a TargetA.»; «in arms» → «en brazos» |
| hand over (baby) | entregar | propuesta | «entregando al bebé.» |
| put down | dejar | propuesta | «Dejar a {BABY}» |
| pick up | recoger | propuesta | |
| parent / mother / father | padre o madre / madre / padre | propuesta | «parents only» → «(solo los padres)» |
| sibling | hermano | RML | «my little sibling» → «mi hermanito», con `{X_gender ? o : a}` solo si el texto recibe al bebé |
| parental bond | vínculo parental | propuesta | |
| protective instinct | instinto protector | propuesta | |
| adrenaline | adrenalina | RML | |
| sore back | espalda adolorida | propuesta | Las cuatro etapas: «fatiga por cargar», «espalda algo adolorida», «espalda adolorida», «espalda muy adolorida» |

## Prendas

| Inglés | Español | Origen | Notas |
|---|---|---|---|
| baby carrier | portabebés | Angel | Invariable: «el portabebés», «los portabebés». Angel, 27-oct-2026 |
| baby sling / sling | portabebés | Angel | Todo lo que lleva al bebé es «portabebés», también el *sling*; lo distingue el material. No «fular», «cabestrillo» ni «rebozo» |
| sling or carrier | portabebés | Angel | Cuando el inglés nombra los dos, va uno solo: «Exigir portabebés para llevar bebés» |
| cloth baby sling | portabebés de tela | Angel | |
| tribal baby sling | portabebés tribal | Angel | «tribal» es oficial |
| kid's … | … infantil | propuesta | «kid's baby carrier» → «portabebés infantil», «kid's cloth baby sling» → «portabebés de tela infantil» |
| spacer carrier | portabebés espacial | Angel | «spacer» = «espacial», oficial. «(deployed)» → «(desplegado)» |
| carry apparel | prenda para cargar | propuesta | En los textos de opciones: «(sin prenda para cargar)» |
| outer utility (capa de ropa) ⚠ | accesorio exterior | propuesta | El juego llama «cinturón» a la capa *utility*; esta es una capa nueva del mod, encima de la ropa |

## Peligros

| Inglés | Español | Origen | Notas |
|---|---|---|---|
| hypothermia / hyperthermia | hipotermia / golpe de calor | oficial | El juego no usa «hipertermia» |
| vacuum | vacío | oficial | Odyssey |
| tox gas / toxic gas | gas tóxico | oficial | |
| toxic buildup | acumulación tóxica | oficial | |
| unsafe | peligroso | propuesta | «too cold» → «demasiado frío», «too hot» → «demasiado calor» |
| caravan | caravana | oficial | |
| tend | atender | RML | |
