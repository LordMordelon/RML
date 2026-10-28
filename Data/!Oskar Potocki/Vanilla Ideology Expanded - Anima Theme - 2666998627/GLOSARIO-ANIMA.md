# Glosario: Vanilla Ideology Expanded - Anima Theme

Términos que este mod repite, con la traducción que se fijó al traducirlo el 28-oct-2026, cotejando entrada
por entrada contra la oficial de Ideology y Royalty. [RML/GLOSARIO.md](../../../GLOSARIO.md) manda sobre este.
Las dríadas, sus castas y todo lo demás de Ideology que el mod no renombra están en el glosario de Dryads:
[GLOSARIO-DRYADS.md](../Vanilla%20Ideology%20Expanded%20-%20Dryads%20-%202720631512/GLOSARIO-DRYADS.md).

Las etiquetas van en minúscula, como en la oficial, aunque el inglés del mod traiga mayúsculas («Seedmaker»,
«Anima Nexus tree»). Los términos marcados **a confirmar** son propuestas que Angel todavía no aprobó; si se
cambia uno, hay que buscarlo en todos los archivos de la carpeta.

## Cómo está hecho el mod

De sus 97 textos, 77 son el texto oficial de Ideology con «Gauranlen» cambiado por «anima nexus» (o solo
«anima»), a veces con el árbol en plural, y nada más. Esos se tradujeron con la oficial de la misma clave,
cambiando solo el nombre: las claves de Anima son las de Ideology (`Carrier.description`,
`Keyed:DryadCannotChangeMaster`…), y se buscan tal cual en `Data/Ideology/Languages/` del juego.

Lo que el mod cambia de verdad:

| Qué cambia | Inglés oficial → Anima | Dónde |
|---|---|---|
| La casta que crea vainas | gaumaker → **seedmaker** | `Gaumaker.label`, `Dryad_Gaumaker`, `GaumakerCocoon` y sus descripciones |
| El musgo pasa a arbusto | Gauranlen moss → **anima bush**, con «infused with luminous microorganisms» | `Plant_MossGauranlen`, `ConnectToTree.extraPredictedOutcomeDescriptions.1` |
| La descripción del árbol | suma tres frases copiadas del árbol ánima de Royalty | `Plant_TreeGauranlen.description` |
| Tela de las dríadas (Dryads) | gaubric → **animabric**, gaubricmaker → **animabricmaker** | sus `Patches` sobre los defs de Dryads |

## Ánima oficial (Royalty)

| Inglés | Español oficial | Notas |
|---|---|---|
| anima tree | **árbol ánima** | `Plant_TreeAnima`. «Ánima» va detrás y no cambia. No «árbol de ánima» |
| anima grass | **hierba ánima** | |
| chopped / smashed anima stump | **tocón ánima cortado** / **tocón ánima dañado** | |
| psyfocus / psycaster / meditate | **psicofoco** / **psicoemisor** / **meditar** | |
| bioluminescent micro-organisms / luminous microorganisms | **microorganismos bioluminiscentes** / **microorganismos luminosos** | Del árbol ánima y de la hierba ánima |
| psychic symbiosis / artificial structures | **simbiosis psíquica** / **estructuras artificiales** | |
| refuse to be caged or studied | **se niegan a ser enjaulados o estudiados** | |
| draw psyfocus from | **obtener psicofoco de** | La oficial pone «dibujar el psicofoco», que es un error de traducción: no se copia |

## Términos propios

«Anima nexus» reemplaza a «Gauranlen», así que se trata igual que él: un nombre que va detrás y no cambia,
como «ánima» en la oficial.

| Inglés | Español | Notas |
|---|---|---|
| anima nexus | **nexo ánima** | **A confirmar.** La otra opción era «del nexo ánima» (árbol del nexo ánima), más natural pero más larga |
| anima nexus tree | **árbol nexo ánima** | **A confirmar**, con lo anterior |
| anima nexus connection | **conexión nexo ánima** | Como «conexión gauranlen». Pensamiento: «quiere una conexión con el nexo ánima» |
| anima nexus death | **muerte del nexo ánima** | Como «muerte de gauranlen» |
| chopped / smashed anima nexus stump | **tocón nexo ánima cortado** / **tocón nexo ánima destrozado** | «Destrozado» como el gauranlen, que es el que reemplaza |
| anima nexus pod / anima pod | **vaina nexo ánima** / **vaina ánima** | El mod usa las dos para la misma vaina. Se tradujo lo que dice cada una |
| anima seed / anima seed pods | **semilla ánima** / **vainas de semilla ánima** | |
| anima pod sprout / anima nexus pod sprout | **brote de vaina ánima** / **brote de vaina nexo ánima** | |
| anima bush | **arbusto ánima** | Reemplaza al musgo gauranlen |
| anima nexus life cycle | **ciclo de vida del nexo ánima** / **ciclo vital del nexo ánima** | Como la oficial, que usa las dos |
| seedmaker | **creasemillas** | **A confirmar.** Como «creabayas». Reemplaza a «creagau» |
| seedmaker dryad / Seedmaker pod | **dríada creasemillas** / **vaina de creasemillas** | Como «vaina de creagau» |
| anima tree (dicho del nexo) | **árbol ánima** | En `Barkskin.description` y en la descripción del árbol el mod dice «anima tree» a propósito: el nexo es una clase de árbol ánima |
| animabric | **animabric** | Como «gaubric», que RML deja igual |
| animabricmaker | **creanimabric** | **A confirmar.** Como «creagaubric»; la «a» repetida se funde, como en «creadicina» |

## Correcciones a la oficial

Donde el inglés de Anima es el oficial, se corrigieron cuatro errores de la oficial en vez de copiarlos:
«tres dríadas gauranlen» por «tres dríadas creasemillas» (el inglés dice *seedmaker*), «las dríadas
dermocortezana» por «dermocortezanas», «somos árboles» con minúscula tras el punto, y «los árboles enfocan la
producción», que omitía el *easily*: «cambian de producción con facilidad».

## Lo que no se ve en el juego

Con Anima y Dryads activos, los 10 `Patches` de Anima sobre defs de Dryads (`creanimabric`, `animabric` y las
descripciones que nombran al nexo) no se ven: la traducción de Dryads va por `DefInjected`, que el juego aplica
después y le gana. Queda «creagaubric» y «árbol gauranlen» en esas descripciones. Hacerlo ganar obligaría a
duplicar esas traducciones en `Ajustes/`, que es solo para arreglos de espacio o contexto, o a cambiar el
extractor; se dejó así el 28-oct-2026.

Los 14 `Patches` de pensamientos sí se ven, aunque la oficial los traduce por `DefInjected`: el juego reconoce
cada etapa por su etiqueta (`stages.Gauranlen_connection`) y el patch cambia esa etiqueta, así que la clave
oficial ya no encuentra la etapa (`TranslationHandleUtility.GetElementIndexByHandle`, comprobado en el código
del juego).
