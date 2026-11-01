# Glosario: Vanilla Plants Expanded

Términos que este mod repite, con la traducción que se fijó al traducirlo el 1-nov-2026.
[RML/GLOSARIO.md](../../../GLOSARIO.md), la terminología oficial de RimWorld, manda sobre este. Lo que
el mod copia de vanilla va con el texto oficial, sacado de los `.tar` de idioma del juego
(`Herramientas/oficial/`): la hierba, la planta de heno, la plaga, el contenedor hidropónico.

Las etiquetas (`label`) van en minúscula y los frutos en plural, como en la oficial («papas», «bayas»).
Las plantas siguen el molde oficial «planta de …» («planta de papa», «planta de maíz»); los árboles
frutales, con su nombre propio en español («manzano», «naranjo»).

Los términos marcados **a confirmar** son propuestas que Angel todavía no aprobó. Si se cambia uno,
hay que buscarlo en todos los archivos de la carpeta, no solo en uno.

## De vanilla (oficiales)

| Inglés | Español | Notas |
|---|---|---|
| soil / rich soil / stony soil | **tierra** / **tierra fértil** / **suelo pedregoso** | `TerrainDef:Soil`, `SoilRich` |
| grass / tall grass | **hierba** / **hierba alta** | `Plant_Grass`, `Plant_TallGrass`. La descripción copia la oficial |
| haygrass / hay | **planta de heno** / **heno** | |
| agave | **agave** | `Plant_Agave` |
| berries | **bayas** | El molde para los frutos en plural |
| blight | **plaga** | `ThingDef:Blight` |
| toxic fallout | **lluvia tóxica** | |
| growing season | **temporada de cultivo** | `GrowSeasonHereNow` |
| hydroponics basin | **contenedor hidropónico** | |
| plant pot | **maceta** | |
| wind turbine | **aeroturbina** | |
| Plants (habilidad) | **Plantas** | «la habilidad de Plantas del colono» |
| sow / harvest | **sembrar** / **cosechar** | |
| beauty / nutrition | **belleza** / **nutrición** | |
| tile | **celda** | |
| river | **río** | |
| vegetables | **vegetales** | Como Vanilla Cooking Expanded |

## Conceptos del mod

| Inglés | Español | Notas |
|---|---|---|
| tilled soil | **tierra labrada** | El terreno nuevo (`VCE_TilledSoil`), al lado de «tierra» y «tierra fértil» |
| till / tilling / till soil | **labrar** / **labrando** / **labrar la tierra** | Los tres `WorkGiverDef`, como «alisar» / «alisando» / «alisar suelos» |
| tillage | **labranza** | |
| fruit tree | **árbol frutal** | |
| raw fruits | **frutas crudas** | La categoría `VCE_Fruit`. Con RimCuisine, **frutas de postre crudas** |
| Gameplay effect: | **Efecto de juego:** | Es lo que más usa RML en los mods VE (66 veces) |
| preserved food | **comida en conserva** | Las ciruelas pasas |

## Frutos, plantas y árboles

| Fruto (inglés) | Fruto | Planta o árbol (inglés) | Planta o árbol |
|---|---|---|---|
| apples | **manzanas** | apple tree | **manzano** |
| peaches | **duraznos** | peach tree | **duraznero** |
| cherries | **cerezas** | cherry tree | **cerezo** |
| plums / prunes | **ciruelas** / **ciruelas pasas** | plum tree | **ciruelo** |
| pears | **peras** | pear tree | **peral** |
| prickly pears | **tunas** | opuntia | **opuntia** (el cactus es el «nopal») |
| bearberries | **gayubas** | bearberry bush | **arbusto de gayuba** |
| bananas | **bananas** | banana tree | **banano** |
| oranges | **naranjas** | orange tree | **naranjo** |
| onions | **cebollas** | onion plant | **planta de cebolla** |
| pumpkins | **calabazas** | pumpkin plant | **planta de calabaza** |
| peppers | **chiles** (Angel, 1-nov-2026) | pepper plant | **planta de chile** |
| peas | **arvejas** (Angel, 1-nov-2026; NO «guisantes», que es de España) | pea plant | **planta de arveja** |
| eggplants | **berenjenas** | eggplant | **berenjena** |
| cabbages | **repollos** | cabbage plant | **planta de repollo** |
| beets | **remolachas** | beet plant | **planta de remolacha** |
| tomatoes | **tomates** | tomato plant | **planta de tomate** |

### Las variedades de RimCuisine

Con RimCuisine 2 Core activo, el parche del mod cambia cada planta por una variedad. Los nombres de
variedad son nombres propios y quedan en inglés, en minúscula como en el inglés («manzanas granny
smith», como se dice en español).

| Inglés | Español |
|---|---|
| chioggia beet(s) | **remolacha(s) chioggia** |
| cauliflower(s) | **coliflor(es)** |
| white onion(s) | **cebolla(s) blanca(s)** |
| mammoth gold pumpkin(s) | **calabaza(s) mammoth gold** |
| red beefsteak tomato(es) | **tomate(s) red beefsteak** |
| granny smith apple(s) | **manzano / manzanas granny smith** |
| sakura tree / sakuranbo | **árbol de sakura** / **sakuranbo** |
| apricot(s) | **árbol de damasco** / **damascos** (Angel, 1-nov-2026; NO «albaricoques») |
| damson plum(s) | **ciruelo damasceno** / **ciruelas damascenas** |
| gros michel banana(s) | **banano** / **bananas gros michel** |
| mandarin(s) | **mandarino** / **mandarinas** |
| black beauty aubergine(s) | **berenjena(s) black beauty** |
| forage grass / forage tall grass | **hierba forrajera** / **hierba alta forrajera** |
