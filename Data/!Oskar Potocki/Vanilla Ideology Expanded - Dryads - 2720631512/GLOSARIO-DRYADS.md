# Glosario: Vanilla Ideology Expanded - Dryads

Términos que este mod repite, revisados el 28-oct-2026 entrada por entrada contra la traducción oficial de
Ideology (las carpetas `Languages/SpanishLatin (Español(Latinoamérica))` del juego).
[RML/GLOSARIO.md](../../../GLOSARIO.md) manda sobre este. Anima Theme, que renombra las cosas de este mod, tiene
el suyo: [GLOSARIO-ANIMA.md](../Vanilla%20Ideology%20Expanded%20-%20Anima%20Theme%20-%202666998627/GLOSARIO-ANIMA.md).

Las etiquetas (`label`) van en minúscula, como en la oficial. Los nombres de casta van en femenino: concuerdan
con «dríada» y «casta». Si se cambia un término, hay que buscarlo en todos los archivos de la carpeta.

## Base oficial de Ideology

| Inglés | Español oficial | Clave oficial / notas |
|---|---|---|
| Gauranlen tree | **árbol gauranlen** | `Plant_TreeGauranlen`. En minúscula |
| Gauranlen pod / seed / moss | **vaina gauranlen** / **semilla gauranlen** / **musgo gauranlen** | `Plant_PodGauranlen`, `GauranlenSeed`, `Plant_MossGauranlen` |
| Gauranlen pod sprout | **brote de vaina gauranlen** | `IncidentDef:GauranlenPodSpawn` |
| chopped / smashed gauranlen stump | **tocón gauranlen cortado** / **tocón gauranlen destrozado** | |
| Gauranlen life cycle | **ciclo vital de gauranlen** / **ciclo de vida del gauranlen** | La oficial usa los dos |
| Gauranlen connection | **conexión gauranlen** | `IssueDef:GauranlenConnection` |
| want Gauranlen connection | **quiere una conexión con gauranlen** | Pensamiento `GauranlenConnectionDesired` |
| Gauranlen death | **muerte de gauranlen** | Pensamiento `ConnectedTreeDied` |
| prune / pruning / pruning speed | **podar** / **poda** / **velocidad de poda** | |
| dryad / dryads | **dríada** / **dríadas** | |
| immature dryad | **dríada inmadura** | `Dryad_Basic` |
| dryad queen | **reina dríada** | |
| dryad caste | **casta de las dríadas**; en las descripciones, **una casta de dríadas** | `Keyed:GauranlenTreeMode`; «A dryad caste…» es «Una casta de dríadas…» en cada descripción oficial |
| morph | **transformarse** (**se está transformando**) | `Keyed:ChangingDryadIntoType`, `DryadCocoon.description` |
| carrier / clawer / barkskin | **portadora** / **garradora** / **dermocortezana** | `GauranlenTreeModeDef` |
| woodmaker / medicinemaker / berrymaker / gaumaker | **creamadera** / **creadicina** / **creabayas** / **creagau** | «creadicina» funde la «me» de «medicina»: el patrón para los nombres nuevos |
| dryad cocoon | **crisálida de dríada** | `DryadCocoon`, el capullo donde se transforma |
| gaumaker pod | **vaina de creagau** | `GaumakerCocoon`: en el código es un capullo, pero la etiqueta dice *pod* y la oficial pone «vaina» |
| dryad healing pod / healing pod | **vaina curativa de la dríada** / **vaina curativa** | |
| connector | **conector** | El rol del ritual |
| connection strength | **fuerza de conexión** | `Keyed:ConnectionStrength` |
| left claw / right claw / head | **garra izquierda** / **garra derecha** / **cabeza** | Los ataques de las dríadas oficiales |
| In general, dryads are mammal-like creatures… | **En general, las dríadas son criaturas parecidas a los mamíferos…** | El párrafo que cierra cada descripción: va idéntico al oficial |
| ambrosia / ambrosia bush | **ambrosía** / **arbusto de ambrosía** | |
| neutroamine / pack animal / deconstruct / shear | **neutroamina** / **animal de carga** / **desarmar** / **esquilar** | |
| drug-like mood boost | **estímulo del humor similar al de las drogas** | Igual que la oficial del arbusto de ambrosía |

## Términos propios

| Inglés | Español | Notas |
|---|---|---|
| awakened (dryad) / awaken dryads / awakening | **despierta** / **despertar dríadas** / **despertar** | |
| awakening pod | **vaina de despertar** + casta («vaina de despertar portadora») | Decía «crisálida» hasta el 28-oct-2026. La oficial pone «vaina» para *pod*, como en «vaina de creagau» |
| stonedigger / spitter / acidic spitter | **cavapiedra** / **escupidora** / **escupidora ácida** | «Acidic spitter» es el nombre con Alpha Memes (un `Patches`); el `DefInjected` de «escupidora» le gana igual |
| gaubricmaker / nectarmaker | **creagaubric** / **creanéctar** | |
| gaubric | **gaubric** | |
| separation sickness | **enfermedad por separación** | Etapas: debilitada, marchitándose, desvaneciéndose |
| no pain sensitivity | **sin sensibilidad al dolor** | |
| ambrosia palm | **palmera de ambrosía** | |
| dryad nest | **nido de dríada** | |
| acid spit / acidic spit | **escupitajo de ácido** / **escupitajo ácido** | |
| bound relatives / unshackled | **parientes vinculadas** / **libres de ataduras**, **liberada** | |
| stone chunks | **trozos de roca** | La etiqueta oficial es «trozos de piedra», pero la oficial también dice «trozos de roca» en las descripciones |
| mop (wonder-mop) | **mopa** | Decía «trapeador», que no se usa en todos lados |
| eventually | **con el tiempo** | No «eventualmente», que en español quiere decir «quizás» |
