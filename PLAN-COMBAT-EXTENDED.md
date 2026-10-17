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

**En la rama `combat-extended` de RML, y se publica entero al final.** Mientras falte, nada de
Combat Extended llega a `master`, a GitHub ni al Workshop. Las acciones de GitHub corren solo en
`master`, así que subir la rama como respaldo no publica nada (igual se pregunta antes).

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

- **Para probar en partida:** con la rama puesta, `LoadFoldersBuilder -build` deja `output/` al
  día y el juego carga lo traducido. Lo que falta sale en inglés, salvo en esos dos casos.
- **Para trabajar otros mods:** volver a `master` (`git switch master`) antes de abrir el
  extractor, que escribe en la rama que esté puesta. Al volver a `master` la carpeta de Combat
  Extended desaparece de `Data/` y reaparece al volver a la rama.
- **Para no quedarse atrás:** cada tanto, `git merge master` estando en la rama.
- **Al terminar:** unir la rama a `master` y subir; ahí la CI publica la release.

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

- **Combat Extended se actualiza seguido.** Para reextraer, usar el extractor con «Traducción
  rápida» sobre RML: conserva lo traducido. Después, mirar el diff con las tres comprobaciones.
- **No hay `UNUSED.xml` todavía.** Aparecerá la primera vez que una actualización saque algo;
  desde ahí aplica la regla 3 de `AGENTS.md`.
- **`Strings/NameBanks`** son `.txt`, una frase por línea. El extractor tiene un error conocido
  con las líneas en blanco en estos archivos: no dejar líneas vacías al traducirlos.
- **Núcleo y compatibilidad van mezclados en el mismo archivo.** El extractor escribe un solo
  XML por tipo de def, con las entradas de `ModPatches/` intercaladas. Las del núcleo son las
  de los `defName` que aparecen en `Defs/` o en las carpetas de DLC (`Royalty/`, `Biotech/`…)
  del mod instalado; en `AmmoCategoryDef` y `ThingCategoryDef` quedan seguidas, en un bloque.
- **`EMP` se traduce `PEM`**, aunque el resto de las siglas de munición se queden en inglés.
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
2. **Llena una entrada solo si el inglés coincide letra por letra** con la plantilla armada con la
   `label` en inglés del producto y su cantidad. Si sobra o falta algo —otra redacción, otra
   cantidad, un producto que no es munición, más de un producto— queda en `TODO` para hacerla
   a mano.
3. **El plural sale del glosario** (sección 3). Si la primera palabra del nombre no está en esa
   tabla, la entrada queda en `TODO`.
4. **Solo toca `TODO`.** Nunca reemplaza un valor que ya tenga texto.
5. **Termina con un informe:** cuántas llenó y cuántas dejó, agrupadas por motivo. Antes del
   commit se lee una muestra de cada redacción y cada cantidad, y se corren las tres
   comprobaciones de `AGENTS.md`.

El script vive en el scratchpad de la sesión, no en el repo: es de un solo uso.

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

## Avance

| # | Etapa | Estado |
|---|---|---|
| 0 | Rama y carpeta en RML | hecha (17-oct-2026) |
| 1 | Interfaz y reglas | hecha (17-oct-2026): `Keyed` (531) y el resto (485) |
| 1b | Lo que se ve como «TODO» | hecha (17-oct-2026): `Patches/` (866) y las listas `<li>` (114) |
| 2 | Vocabulario de munición | en curso: `AmmoCategoryDef` del núcleo hecho (248, 17-oct-2026), y de `ThingCategoryDef` del núcleo las 121 primeras (hasta `AmmoFlare`). Faltan las otras 139 de `ThingCategoryDef` del núcleo (casi todas calibres, que quedan igual) y las 303 de `AmmoSetDef` del núcleo |
| 3 | Munición y proyectiles | pendiente |
| 4 | Recetas | pendiente |
| 5 | Resto del núcleo | pendiente |
| 6 | Compatibilidad | pendiente |
