# Plan: traducir Combat Extended

Combat Extended (`CETeam.CombatExtended`, workshop 2890901044) es el mod más grande que ha
entrado a RML. Este plan lo divide en etapas que se pueden hacer de a una, en sesiones
separadas. La terminología está en [GLOSARIO-COMBAT-EXTENDED.md](GLOSARIO-COMBAT-EXTENDED.md).

## Qué hay

Extraído el 17-oct-2026: **14.200 entradas, todas en `TODO`**, con unos 10.100 textos distintos.
El mod no trae traducción al español; sí al portugués de Brasil y al francés, que sirven de
referencia para entender un texto, no para copiarlo.

No todo pesa lo mismo:

- **Unas 7.000 son del núcleo**: las ve cualquiera que juegue con Combat Extended.
- **Unas 5.600 son de compatibilidad.** Combat Extended trae 768 carpetas `ModPatches/` con
  munición y ajustes para otros mods, y cada una se carga solo si ese otro mod está activo.
- **Casi el 70 % es munición y sus recetas**, con nombres muy repetitivos. Una receta son tres
  frases fijas (`make X x500`, `Craft 200 X.`, `Making X.`): traducido el nombre de la munición,
  la receta sale sola. Por eso el glosario fija primero los sustantivos y los tipos.

## Dónde se trabaja

**En `master`, desde el 19-oct-2026.** Las etapas 0 a 5 y la primera tanda de la 6 se hicieron en la
rama `combat-extended`, que se unió a `master` cuando el núcleo quedó completo (decisión de Angel). Lo
que siga va directo en `master`, como cualquier otro mod, y cada push publica una versión de RML.

**Qué hace el juego con un `TODO`.** `DefInjectionPackage.TryAddInjection` y
`LoadedLanguage.LoadFromFile_Keyed` comparan el valor con `"TODO"` y lo saltan: en `DefInjected`
y `Keyed` el jugador ve el inglés, no la palabra. Comprobado en el `Assembly-CSharp.dll` del
juego el 17-oct-2026. Hay dos excepciones, donde «TODO» **sí** llega a la pantalla:

- **`Patches/`**: un `PatchOperationReplace` pone su valor tal cual. Son las partes de armas y
  ataques (866), que saldrían como «TODO» en el registro de combate.
- **Listas** (`<li>TODO</li>`, 114): las reglas de daño de `RulePackDef` y listas de
  `ResearchProjectDef`. Una lista se inyecta entera, sin revisar cada elemento.

Por eso van en la etapa 1b, justo después de la interfaz (decidido el 17-oct-2026): así la
partida de prueba no muestra ningún «TODO», y queda abierta la opción de publicar por etapas.

- **Para probar en partida:** `LoadFoldersBuilder -build` deja `output/` al día y el juego carga lo
  traducido. Lo que falta sale en inglés, salvo en esos dos casos.
- **Este plan y los glosarios viven en la carpeta del mod** (desde el 19-oct-2026, pedido de Angel),
  junto a `Languages/`: el del núcleo y uno aparte para la compatibilidad con otros mods. El
  extractor no los toca, porque solo borra `Languages/` y `Patches/`, y la copia de `output/` deja
  afuera los `.md` de `Data/`, así que no se publican con el mod.

La etapa 0 quedó hecha el 17-oct-2026: la extracción se movió de `RimworldExtractor/` a
`Data/Combat Extended - 2890901044` (va suelta: es el único mod de su autor en RML), `-rutas`
pasó sin avisos y `-build` la sumó al índice y a `output/`.

## Etapas

Van de lo que ve todo jugador a lo que ve solo quien tiene otros mods. Cada etapa se cierra
con las tres comprobaciones de `AGENTS.md` y un commit propio, del estilo
«Combat Extended: traducir la interfaz y las estadísticas».

Se estima que una sesión traduce unas 500 entradas.

| # | Etapa | Qué incluye | Entradas | Sesiones |
|---|---|---|---|---|
| 1 | Interfaz y reglas | `Keyed` (menús, opciones, equipamiento, modos de disparo), `StatDef`, `ConceptDef` (ayuda del tutorial), `TipSetDef`, `DamageDef` y las categorías chicas: `ApparelLayerDef`, `BodyPartGroupDef`, `BodyDef`, `ToolCapacityDef`, `BipodCategoryDef`, los dos `PatcherPresetDef`, `JobDef`, `WorkGiverDef`, `ThoughtDef`, `MentalStateDef`, `TraitDef`, `StuffCategoryDef`, `PawnColumnDef`, `DesignatorDropdownGroupDef` | ~1.000 | 2 |
| 1b | Lo que se ve como «TODO» | Las dos excepciones de «Dónde se trabaja»: `Patches/` (partes de armas y ataques, 866, muy repetitivo) y las listas `<li>` de `RulePackDef` (cuidar el prefijo `simbolo->`) y `ResearchProjectDef` (114). Con esto, en la partida ya no se ve ningún «TODO» | ~980 | 2 |
| 2 | Vocabulario de munición | `AmmoCategoryDef`, `AmmoSetDef` y `ThingCategoryDef` del núcleo. Lo que aparezca se suma al glosario | ~800 | 2 |
| 3 | Munición y proyectiles del núcleo | `ThingDef` del núcleo, por familias: pistola y fusil, escopeta, granadas y cohetes, artillería, arcos y ballestas, energía (láser, plasma), mecanoides | ~2.900 | 6 |
| 4 | Recetas del núcleo | `RecipeDef` del núcleo, con la plantilla del glosario (sección 6), llenadas por script con las reglas de abajo | ~2.700 | 1 |
| 5 | Resto del núcleo | `HediffDef`, lo que quede de `ResearchProjectDef` y `RulePackDef` tras la 1b, `PawnKindDef`, `LaserBeamDefCE`, `WorldObjectDef` y las frases de `Strings/NameBanks` | ~420 | 1 |
| 6 | Compatibilidad con otros mods | Lo de `ModPatches/`. Primero los mods que RML ya traduce, porque esos jugadores ya los tienen; el resto, cuando alguien lo pida | ~5.600 | según avance |

Con las etapas 1 a 5 terminadas, Combat Extended queda completo para quien lo juega solo con el
juego base y los DLC.

## Cuidados

- **Los nombres de los DLC salen de la traducción oficial**, aunque en 1.7 el juego no la traiga
  instalada y se vea en inglés. Está en GitHub, en `Ludeon/RimWorld-SpanishLatin`
  (`Anomaly/`, `Biotech/`, `Ideology/`, `Odyssey/`, `Royalty/`); se busca con
  `gh api "search/code?q=repo:Ludeon/RimWorld-SpanishLatin+<defName>"`. Así salió
  «pinchador de nervios» para el *nerve spiker* de Anomaly.
- **Combat Extended se actualiza seguido.** Para reextraer, usar el extractor con «Traducción
  rápida» sobre RML: conserva lo traducido. Después, mirar el diff con las tres comprobaciones.
- **El `UNUSED.xml` de Combat Extended no está en git.** Tiene 8 entradas de `Patches/` (verbos de
  mochilas y armaduras, el escupitajo de la dríade y las dos torretas de hombro de LTS), y las 8 siguen
  traducidas en su `Patches/`: es una copia de más, no una pérdida. Queda sin commitear en el PC y sin
  borrar (regla 3 de `AGENTS.md`). La causa está en el extractor: un xpath que entra a un elemento de
  lista (`verbs/*[...]/label`) se relee con el índice (`verbs.0.label`), y la extracción lo nombra por
  su verbo. Arreglarlo cambia el nombre de unas 427 rutas en 11 mods de RML: sesión aparte, con la
  aprobación de Angel (19-oct-2026).
- **Xpath con la clase del atributo.** Hasta el 19-oct-2026 el extractor escribía
  `/Defs/CombatExtended.AmmoDef[...]` para un `ThingDef` con `Class="CombatExtended.AmmoDef"`, que en
  el juego no encuentra nada: la granada tóxica y las cuatro flechas de viuda negra estaban traducidas y
  se veían en inglés. Arreglado en el extractor (commit `9f7d871`) y en RML (`18f347a`).
- **`Strings/NameBanks`** son `.txt`, una frase por línea. El extractor tiene un error conocido
  con las líneas en blanco en estos archivos: no dejar líneas vacías al traducirlos.
- **Núcleo y compatibilidad van mezclados en el mismo archivo.** El extractor escribe un solo
  XML por tipo de def, con las entradas de `ModPatches/` intercaladas. Las del núcleo son las
  de los `defName` que aparecen en `Defs/` o en las carpetas de DLC (`Royalty/`, `Biotech/`…)
  del mod instalado; en `AmmoCategoryDef` y `ThingCategoryDef` quedan seguidas, en un bloque.
- **`EMP` se traduce `PEM`**, aunque el resto de las siglas de munición se queden en inglés.
- **Munición genérica (opción de CE).** Con esa opción, CE renombra las balas por código como
  «<juego de munición> bala (<sigla>)» (`AmmoGeneralizer.cs`), así que salen «munición para fusil de
  cargas bala ()» o «Antimaterial bala (FMJ)». Los 37 tipos sin `labelShort` dejan «()», también en
  inglés, y el orden no se arregla traduciendo. Los nombres fijos sí se traducen: `genericLabel` y
  `genericDescription` (en `modExtensions`) y `genericLabelOverride` (en el `projectile` de las balas).
  El extractor los saca desde el 19-oct-2026 (`Prefabs.EtiquetasAgregadas`), y `DefInjected` llega a
  `modExtensions` (probado en partida el 18-oct-2026 con el obús de espuma). Se traducen copiando el
  nombre con calibre sin el calibre: «obús de mortero (HE)», «fabricar obús de mortero (HE) x5»,
  «Fabrica 5 obuses de mortero (HE).»; `pistol` es «pistola». El `genericLabel` del obusero 155mm dice
  `mortar shell` en el original y se dejó «obús de mortero», como el inglés.
- **Los nombres de `GunPatcherPresetDef` quedan en inglés** (`m16a1`, `pistol`, `machine gun`…).
  El parcheo automático los usa para reconocer armas de otros mods por su nombre: traducirlos
  podría hacer que dejara de reconocerlas.

## Recetas por script

Es una excepción a «A mano y directo» de `AGENTS.md`, aprobada solo para las recetas de Combat
Extended. El riesgo es suponer que todas las recetas son iguales: hay tres redacciones distintas
(`make X x500`, `Craft 200 X.`, `Making X.`), cantidades distintas y recetas que no siguen
ninguna. Por eso el script **no adivina nada**:

1. **No deduce el nombre del texto en inglés.** Lo saca del def de la receta en el mod
   instalado: qué produce (`products`) y cuántos. El nombre en español es la `label` ya
   traducida de ese producto, de la etapa 3.
2. **Llena una receta solo si el inglés nombra al producto con sus mismas palabras**, en cualquier
   orden, y con la cantidad exacta del def. Se pensó letra por letra, pero las recetas del mod
   cambian el orden («make 90mm (HE) cannon shells x5» para el producto «90mm cannon shell (HE)») y
   así quedaban 440 recetas a mano en vez de 152; Angel aprobó el cambio el 17-oct-2026. Como el
   nombre en español sale del producto y no del texto, el orden del inglés no influye en el
   resultado. A un producto sin sustantivo («5.56x45mm NATO (FMJ)») la receta solo le puede sumar
   `cartridge` → «cartucho» o `rocket` → «cohete». Si sobra o falta algo —otra redacción, otra
   cantidad, otra palabra, un producto que no es munición, más de un producto— queda en `TODO` para
   hacerla a mano.
3. **El plural sale del glosario** (sección 3), con el sustantivo más largo de la tabla que abra el
   nombre («flecha grande de acero» → «flechas grandes de acero»). Si el nombre no empieza con uno
   de esos sustantivos, o si detrás viene un adjetivo que también habría que poner en plural
   («granada aturdidora»), la receta queda en `TODO`.
4. **Solo toca `TODO`.** Nunca reemplaza un valor que ya tenga texto.
5. **Termina con un informe:** cuántas llenó y cuántas dejó, agrupadas por motivo. Antes del
   commit se lee una muestra de cada redacción y cada cantidad, y se corren las tres
   comprobaciones de `AGENTS.md`.

El script vive en el scratchpad de la sesión, no en el repo: es de un solo uso.

Resultado (17-oct-2026): 754 recetas del núcleo por script (2.262 entradas) y 152 a mano (456), con
una tabla que Angel revisó antes de escribir. Las decisiones están en el glosario (sección 6).

## Partes de armas por script

Segunda excepción a «A mano y directo», aprobada por Angel el 17-oct-2026 solo para las
etiquetas de `Patches/` de Combat Extended: 834 etiquetas con 140 textos distintos («head» se
repetía 212 veces). Se hizo así:

1. Se armó a mano una tabla inglés → español con los 136 textos que no cambian de sentido, con
   la traducción oficial del juego primero y lo que RML ya usaba después. Angel la revisó y
   corrigió antes de correr nada.
2. El script llenó una etiqueta solo si era `<label>TODO</label>` y el comentario `EN:` justo
   arriba coincidía letra por letra con una fila de la tabla. No tocó descripciones, `<li>` ni
   `jobString`. Informe: 825 llenadas, 41 dejadas.
3. Lo que cambia de sentido según el arma o el animal («blade»), las descripciones, las recetas
   y las líneas de genes se hicieron a mano.
4. Al final, cada línea cambiada del diff era un `TODO` reemplazado, y las tres comprobaciones
   salieron vacías.

Como con las recetas, el script quedó en el scratchpad y no se extiende a otros mods ni a otras
partes de Combat Extended sin volver a preguntar.

## Munición por script

Tercera excepción, aprobada por Angel el 17-oct-2026 para los nombres y las descripciones de la
munición y los proyectiles de la etapa 3. Se estrenó con pistola y fusil (`Defs/Ammo/Pistols` y
`Defs/Ammo/Rifle` del mod instalado, que es de donde sale qué `defName` es de cada familia):

1. **Nombres.** El calibre sale de las `label` de `AmmoSetDef` ya traducidas en la etapa 2, más
   una tabla a mano con las formas cortas que usan los proyectiles («5.56mm NATO»). Quedan igual,
   salvo `Caseless` → «sin vaina» y dos erratas del original («Smith Wessen» → «S&amp;W»). Solo se
   llena si el inglés es `<calibre> (<tipo>)`, o `<calibre> bullet|cartridge (<tipo>)`, con un tipo
   conocido; si no, queda en `TODO`. Pistola y fusil: 735 llenados, 3 a mano.
2. **Descripciones.** Se traducen a mano en una tabla con un texto por fila, que Angel revisa, y el
   script pone cada uno donde el `EN:` coincide letra por letra. Pistola y fusil: 60 textos para
   324 descripciones.
3. Solo toca `TODO`, y antes del commit van las tres comprobaciones de `AGENTS.md`.

Para las familias que siguen vale lo mismo, pero con una tabla de descripciones nueva que Angel
revisa antes de escribir.

Escopeta y alto calibre (`Defs/Ammo/Shotgun` y `Defs/Ammo/HighCaliber`) sumaron reglas de nombres,
que Angel aprobó y están en el glosario (secciones 3, 4 y 9): el cartucho es `cartucho 12 Gauge
(postas|bala|saco de perdigones|PEM)`, y los proyectiles de escopeta, que no traen calibre, van con
una tabla fija («posta», «bala de escopeta», «saco de perdigones», «bala de escopeta PEM»). Tipos
nuevos: `EMP` → «PEM», `Smoke` → «humo», `HE Time-Fuzed` → «HE de espoleta de tiempo». 344 nombres
por script, ninguno a mano, y 21 textos para 168 descripciones.

Granadas, cohetes y artillería (`Defs/Ammo/Grenade`, `Rocket` y `Shell`) sumaron sustantivos: `cannon
shell` → «obús de cañón», `Howitzer shell` → «obús de obusero», `mortar shell` → «obús de mortero»,
`recoilless cartridge|rocket` → «cartucho|cohete … sin retroceso», y `grenade`, `rocket` y `missile`
del glosario. La designación queda en inglés después del sustantivo («granada RPG-7 (Frag)», «misil
BGM-71 TOW (HEAT)»), con cinco erratas corregidas («40x53mmVOG25» → «40x53mm VOG-25», «SPG9» →
«SPG-9»…). Los tipos escritos enteros concuerdan con el sustantivo («granada (termobárica)»). Angel
eligió dejar «flechette» en inglés. 379 nombres por script, 2 a mano, y 47 textos para 177
descripciones.

La munición avanzada, la genérica, la medieval, la neolítica y la de `Other` traían nombres demasiado
distintos para una regla, así que los nombres también fueron con una tabla de un texto por fila (230
textos para 269 nombres), igual que las descripciones (68 textos para 166). Angel cambió cuatro reglas
antes de escribir, y están en el glosario (sección 4): la bala de carga es «cargada» («bala 10x18mm
cargada»), «Conc.» e «Ion» van sin paréntesis («6x24mm de carga concentrada», «cartucho de carga para
fusil iónico»), y el material de flechas y virotes también («virote de acero», «flecha grande
incendiaria»). Por eso las categorías «flecha de fuego» y «virote de fuego» de la etapa 2 pasaron a
«incendiaria» e «incendiario».

## Compatibilidad por script

Cuarta excepción, aprobada por Angel el 19-oct-2026 para la primera tanda de la etapa 6: las 451
entradas de `ModPatches/` de los 24 mods que RML ya traduce. Se hizo así:

1. **Qué entradas.** Se cruzaron los `defName` de cada carpeta de `ModPatches/` del mod instalado
   con los `TODO`, y la carpeta con RML **por packageId**, sacado del `LoadFolders.xml` de Combat
   Extended (`IfModActive`) y de `ModList.tsv`. La cuenta anterior, por nombre, perdía Ushankas
   Biological Warfare (140) y contaba Impact Weaponry, cuyo parche es para el original
   (`detvisor.impactweaponry`) y no para el «Reloaded» que traduce RML.
2. **Los nombres** salen de la carpeta del mod en RML; lo demás, de las reglas del núcleo. Todo en
   una tabla de una fila por clave, que Angel revisó (cambió la munición del *warbolter* a «cohete de
   sarcotraje»). Los términos están en [GLOSARIO-CE-COMPATIBILIDAD.md](GLOSARIO-CE-COMPATIBILIDAD.md).
3. El script reemplazó una línea solo si era `<clave>TODO</clave>` en el archivo y la línea donde
   la encontró el cruce. Informe: 451 llenadas, ninguna dejada. Cada línea quitada del diff era un
   `TODO` y las tres comprobaciones salieron vacías.

Como las otras, el script quedó en el scratchpad y no se extiende a la tanda siguiente sin volver a
preguntar.

La munición de `Modded` (Halo y Warhammer 40k) volvió a ir por regla, con una tabla de 123 nombres y 18
descripciones para 199 entradas. Angel aprobó tres reglas nuevas, que están en el glosario (secciones 3 y
4): «cartucho .75 Cal Bolter (estándar)» y «proyectil .75 Cal Bolter (Kraken)» para el cartucho y el disparo,
«antipersonal» y «cartucho Soellkraft 8 Gauge (postas)». Lo que no era munición (equipo, torretas, granadas,
residuos, psicho: 196 entradas) se hizo a mano, con una lista revisada antes por Angel, que cambió tres términos
(«mochila radio», «granada de contusión» y «psicho» por *psycho*).

## Avance

| # | Etapa | Estado |
|---|---|---|
| 0 | Rama y carpeta en RML | hecha (17-oct-2026) |
| 1 | Interfaz y reglas | hecha (17-oct-2026): `Keyed` (531) y el resto (485) |
| 1b | Lo que se ve como «TODO» | hecha (17-oct-2026): `Patches/` (866) y las listas `<li>` (114) |
| 2 | Vocabulario de munición | hecha (17-oct-2026): `AmmoCategoryDef` (248), `ThingCategoryDef` (260) y `AmmoSetDef` (306) del núcleo. Antes de cerrarla se revisaron contra la traducción oficial los nombres de DLC que usa («bengala disruptora», «necrovita», «bioferrita», «gas tóxico», «ojiva antigrano»): coinciden |
| 3 | Munición y proyectiles | hecha (17-oct-2026). Pistola y fusil (17-oct-2026), 738 nombres por script y 324 descripciones con una tabla de 60 textos que Angel revisó; escopeta y alto calibre hechos (17-oct-2026), 344 nombres y 168 descripciones con una tabla de 21 textos; granadas, cohetes y artillería hechos (17-oct-2026), 381 nombres y 177 descripciones con una tabla de 47 textos; avanzada, genérica, medieval, neolítica y `Other` hechas (17-oct-2026), 269 nombres y 166 descripciones con dos tablas (230 y 68 textos); `Modded` (199) y el resto de `ThingDef` del núcleo (196, a mano) |
| 4 | Recetas | hecha (17-oct-2026): 754 recetas por script y 152 a mano, 2.718 entradas |
| 5 | Resto del núcleo | hecha (18-oct-2026): `HediffDef` (147), `ResearchProjectDef` (18), `LaserBeamDefCE` (11), `WorldObjectDef` (4) y `PawnKindDef` (3), todo a mano, más los 132 gritos de combate de `Strings/NameBanks` con el tono que eligió Angel. Combat Extended queda completo para quien juega solo con el juego base y los DLC |
| 6 | Compatibilidad | primera tanda hecha (19-oct-2026): los 24 mods que RML ya traduce, 451 entradas por script con una tabla que Angel revisó. Quedan 5.086 `TODO`, todos de mods que RML no traduce |
| 6b | Munición genérica | hecha (19-oct-2026): el extractor saca los tres campos, y se tradujeron a mano las 41 entradas del núcleo y de VWE Non-Lethal. Las 14 de mods que RML no traduce quedan en `TODO` con el resto de la etapa 6 |

## Próxima sesión (19-oct-2026)

**Objetivo:** en el extractor, que un xpath que entra a un elemento de lista se relea con el mismo
nombre con el que se extrae (`verbs.Verb_ShootCE.label` y no `verbs.0.label`), sin dejar duplicados.

**Estado:** munición genérica hecha (etapa 6b) y xpath de `AmmoDef` arreglados. Extractor: commits
`7ff33bd` y `9f7d871` en la rama `1.0-beta`. RML: `ecb4fca`, `c174f9b`, `bcb543d`, `18f347a` y el de
esta nota, en `master`. Todo **sin subir** hasta que Angel lo apruebe. En el PC queda sin commitear el
`UNUSED.xml` de Combat Extended con las 8 copias (ver «Cuidados»). Quedan 5.086 `TODO`, todos de mods
que RML no traduce (contados el 19-oct-2026).

**Antes de empezarla (Angel):**
1. Con la opción de munición genérica de Combat Extended puesta, mirar en partida un obús de mortero y
   su receta, una pistola .45 ACP y la granada tóxica, y avisar lo que se vea raro.

**Al empezar (Claude):** en `RimworldExtractor/`, rama `1.0-beta`. Es un cambio grande: plan primero y
«Aprobado» de Angel. El arreglo es que `Extractor.GetRootDefNode` nombre los elementos de lista con
`MatchTranslationHandle`, como `FindExtractableNodes`. Cuidado con el paso intermedio: las 427 rutas
`verbs/li[N]` y `comps/li[N]` que RML ya tiene (11 mods) se releerían por la vía evaluada con el verbo y
por la literal con el índice, y la del índice quedaría para siempre en `UNUSED`. La relectura literal
tiene que quedar solo para los xpath que no encontraron objetivo. Pruebas primero, y después reextraer
esos 11 mods y comprobar que no sale ningún `UNUSED` nuevo. Puede no caber en una sesión (estimación).

**Decisiones de Angel:** si se hace este arreglo (recomendación: sí, en su sesión) y si se sigue con la
etapa 6 para mods que RML no traduce (recomendación: no por ahora).

**Guías:** ninguna en particular (no hay tarea de GitHub). Al cerrar, `traspaso`.

**Dónde leer:** este plan («Cuidados»), el `AGENTS.md` del extractor («Escribir sobre RML»: la
relectura de `Patches`, `LeerPatchesLiteral` y `DesarmarXpath`).
