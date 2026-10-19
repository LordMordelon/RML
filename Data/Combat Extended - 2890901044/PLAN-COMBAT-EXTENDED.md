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
- **Este plan y el glosario viven en la carpeta del mod** (desde el 19-oct-2026, pedido de Angel),
  junto a `Languages/`. El extractor no los toca, porque solo borra `Languages/` y `Patches/`, y la
  copia de `output/` deja afuera los `.md` de `Data/`, así que no se publican con el mod.
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

- **Los nombres de los DLC salen de la traducción oficial**, aunque en 1.7 el juego no la traiga
  instalada y se vea en inglés. Está en GitHub, en `Ludeon/RimWorld-SpanishLatin`
  (`Anomaly/`, `Biotech/`, `Ideology/`, `Odyssey/`, `Royalty/`); se busca con
  `gh api "search/code?q=repo:Ludeon/RimWorld-SpanishLatin+<defName>"`. Así salió
  «pinchador de nervios» para el *nerve spiker* de Anomaly.
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
- **Munición genérica (opción de CE): pendiente en el extractor.** Con esa opción, CE renombra las
  balas por código como «<juego de munición> bala (<sigla>)» (`AmmoGeneralizer.cs`), así que salen
  «munición para fusil de cargas bala ()» o «Antimaterial bala (FMJ)». Los 37 tipos sin `labelShort`
  dejan «()», también en inglés, y el orden no se arregla traduciendo. Además, 29 `genericLabel`, 8
  `genericDescription` y 1 `genericLabelOverride` viven en `modExtensions` y el extractor no los saca:
  con la opción puesta se ven en inglés. **`DefInjected` sí llega a `modExtensions`**: probado en partida
  el 18-oct-2026 con `Shell_Firefoam.modExtensions.0.genericLabel` = «obús de mortero (espuma)», que es
  la única de las 38 puesta a mano. Falta enseñarle al extractor a extraer `genericLabel`,
  `genericDescription` y `genericLabelOverride` (en los `.xml` de `Defs/Ammo/Shell/` de 60, 81 y
  120mm y del obusero 155mm, más una pistola) y traducir las otras 37 copiando el nombre con calibre
  sin el calibre: «obús de mortero (HE)», «fabricar obús de mortero (HE) x5», «Fabrica 5 obuses de
  mortero (HE).». **Hasta entonces, una reextracción manda la de prueba a `UNUSED.xml`** y vuelve a
  verse en inglés. Sesión aparte, en el extractor (Angel, 18-oct-2026).
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
| 6 | Compatibilidad | pendiente |

## Próxima sesión (19-oct-2026)

**Objetivo:** etapa 6, primera tanda: la compatibilidad con los mods que RML ya traduce.

**Estado:** etapa 5 cerrada y revisada en partida por Angel, en la rama `combat-extended` (último commit `d51261e`,
sin subir; la rama tiene 55 commits que `master` no tiene). Después de probar se corrigieron nombres: la munición
del fusil de cargas, la torreta de cargas, el «virote de bioferrita», el «escudo simple», el «chaleco compuesto» y
las «gafas de visión nocturna» (todo en el glosario). Este plan y el glosario pasaron a la carpeta del mod. No queda
nada a medias.

**Antes de empezarla (Angel):**
1. Si Combat Extended se actualizó en Steam, reextraerlo con «Traducción rápida» en la misma rama y avisarlo al
   empezar. Ojo: la reextracción manda a `UNUSED.xml` el nombre genérico del obús de espuma (ver «Cuidados»).
2. Las traducciones oficiales de los DLC que se pusieron a mano en `Data/<DLC>/Languages/` siguen ahí mientras Steam
   no las pise; si los DLC vuelven a verse en inglés, avisar.

**Al empezar (Claude):** quedan **5.523 entradas en `TODO`**, todas de `ModPatches/` (contadas el 18-oct-2026). De
esas, unas **354 son de 24 mods que RML ya traduce**: Vanilla Factions Expanded - Pirates 62, Impact Weaponry 43,
Spacer Arsenal 41, Vanilla Weapons Expanded - Non-Lethal 33, VWE - Laser 22, VFE - Empire 22, VQE - Deadlife 16,
Integrated Implants 16, Volt Weaponry 13, VWE - Coilguns 12, VQE - Cryptoforge 10, Alpha Mechs 10, A Rimworld of
Magic 10, y 11 mods más con 8 o menos. La cuenta cruza los `defName` de cada carpeta de `ModPatches/` del mod
instalado con los `TODO`, y el cruce con RML es por nombre del mod, así que puede faltar alguno: repetirla al
empezar. Cabe en una sesión (estimación). Antes de traducir cada mod, mirar cómo RML ya traduce sus armas en su
propia carpeta de `Data/`, para que los nombres coincidan. Proponer antes de cambiar nombres ya decididos: Angel
quiere ver la propuesta primero.

**Decisiones de Angel:** si se sube ya el núcleo, uniendo la rama a `master`. Recomendación: sí, porque ya sirve a
todo el que juega Combat Extended con el juego base y los DLC, y la etapa 6 puede seguir en la rama después. El
push publica una versión nueva de RML, así que lo decide Angel.

**Pendiente aparte, en otra sesión:** el extractor tiene que sacar los nombres de la munición genérica (38 textos;
ver «Munición genérica» en «Cuidados»). No mezclarlo con la etapa 6.

**Guías:** ninguna en particular (no hay tarea de GitHub). Al cerrar la tanda, `traspaso`.

**Dónde leer:** este plan (etapa 6 y «Cuidados»), [GLOSARIO-COMBAT-EXTENDED.md](GLOSARIO-COMBAT-EXTENDED.md)
(secciones 1, 7 y 8) y las tres comprobaciones de `AGENTS.md` (en la raíz de RML).
