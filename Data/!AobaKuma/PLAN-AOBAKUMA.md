# Plan: la familia de AobaKuma

Angel reextrajo el 25-oct-2026 once mods de AobaKuma y de sus coautores. Este plan reúne lo que dejó
esa extracción, lo que hay que decidir antes de traducir y el orden de trabajo. Todas las cifras se
contaron ese día sobre los archivos de `Data/`.

Los `.md` de `Data/` no llegan al juego: `CopiaLimpia` los saltea, así que este archivo puede vivir en
la carpeta del autor.

## Qué dejó la extracción

| Mod | Carpeta | Pendientes | Palabras en inglés | Estado |
|---|---|---|---|---|
| The Dead Man's Switch - Expedition | `!AobaKuma/` | 1.295 | ~13.500 | Nuevo |
| Ascension Megacorp | `!AobaKuma/` | 1.355 | ~10.700 | Nuevo |
| Cinders of the Embergarden | `Data/` (suelto) | 1.514 | ~9.000 | Nuevo |
| The Dead Man's Switch - Taktikmagier | `!AobaKuma/` | 182 | ~2.900 | Nuevo |
| Fortification Industrial - Citadel | `!AobaKuma/` | 272 | ~2.400 | Nuevo |
| The Dead Man's Switch - AncientCorps | `!AobaKuma/` | 144 (122 se traducen) | ~1.650 | Actualizado por el autor |
| Hemogen Pack - Emergency transfusion | `Data/` (suelto) | 5 | 24 | Nuevo |
| The Dead Man's Switch - MobileDragoon | `!AobaKuma/` | 3 | 3 | Perdió 3 traducciones (ver abajo) |
| The Dead Man's Switch - Synthetic | `!AobaKuma/` | 0 | — | Solo cambió la condición de CE |
| Fortified Features Framework | `Data/` (suelto) | 0 | — | Solo cambió el fin de línea de un archivo |
| Exosuit Framework | `!AobaKuma/` | 0 | — | Sin cambios |
| **Total** | | **~4.770** | **~40.200** | |

Es más que Hegemony (36.477 palabras). A mano, a unas 3.000 palabras por sesión, son **14 a 16
sesiones**: hay que repartirlo en tareas, no hacerlo de una vez.

**Las tres comprobaciones del diff** de `RML/AGENTS.md` salen vacías en todo `Data/`: ninguna
traducción cambió de valor, no se borró ningún `UNUSED.xml` y ninguna regla de gramática perdió su
`->` (las que «lo pierden» son `<li>TODO</li>`, todavía sin traducir).

## Antes de traducir: lo que hay que resolver

### 1. La condición de Combat Extended en los `Patches/` (bloquea commitear siete archivos)

El extractor reescribió los parches que dependen de Combat Extended: donde el mod decía
`PatchOperationFindMod` «Combat Extended» (por **nombre**), ahora escribe
`MayRequireAnyOf="CETeam.CombatExtended,CETeam.CombatExtended_steam"` (por **packageId**). El CE que
Angel usa en 1.7 es el no oficial, con packageId `CETeam.CombatExtended.17UnofficialVer`, que no es
ninguno de los dos: **con ese CE, estos parches de RML no se aplican**. Por nombre sí se aplicaban,
porque el no oficial también se llama «Combat Extended».

Es la «Tarea 1» que está pendiente en `RimworldExtractor/PLAN-POSICIONES.md` desde el 21-oct-2026. En
ese momento los `Patches/` se dejaron como estaban en git, y la reextracción de hoy los volvió a cambiar:

| Archivo | Qué cambió |
|---|---|
| Synthetic `Patches/n47ui1.xml` | Solo la condición. Las 40 traducciones son las mismas |
| MobileDragoon `Patches/qer29y.xml` | La condición, y 3 `chargeNoun` que volvieron a `TODO` (ver el punto 2) |
| AncientCorps `Patches/fiwktu.xml` | Solo la condición |
| Expedition `Patches/l0qlvm.xml`, Ascension `Patches/ou0l6w.xml`, Taktikmagier `Patches/6h15kf.xml` | Nuevos: ya nacen con la condición por packageId |
| DMS II `Patches/ncvt73.xml` | Ya está así en GitHub, desde el commit `4424e32f` de hoy |

Ojo: los propios mods cargan su contenido para CE con `IfModActive="CETeam.CombatExtended,…"`, así que
con el CE no oficial **tampoco el mod carga su carpeta de CE**. El autor lo dice en el `LoadFolders.xml`
de Expedition: «Combat Extended no tiene versión 1.7, así que 1.7/CE no se carga». Lo único que se pierde
con el cambio de condición son los parches que el mod condiciona por nombre (MobileDragoon tiene dos).

**Decisión de Angel (25-oct-2026): no se devuelven a git.** Los siete archivos se commitean como los
escribió el extractor, y la condición se arregla en el extractor, en su propia tarea: al reextraer, se
corrige sola sin perder lo traducido.

### 2. MobileDragoon perdió 3 traducciones

`chargeNoun` de `DMS_Apparel_MissilePod`, `DMS_Apparel_MissilePodPlus` y `DMS_Apparel_SwarmMissilePod`
decían «misil», «misil» y «cohete». El xpath pasó de `comps/li[5]` (por posición) a
`comps/li[@Class="CompProperties_ApparelReloadable"][1]` (por clase, por el cambio del 21-oct), y el
cruce no reconoció la clave vieja: quedaron en `TODO` y **no pasaron al `UNUSED.xml`**. Es una falla del
extractor, de la misma forma que las cuatro de «Escribir sobre RML» en su `AGENTS.md`: el cruce de un
`Patches` no usa `NodoAnterior` cuando el xpath cambia de posición a clase.

Como el archivo no se devuelve a git (punto 1), las tres se escriben a mano, con el valor que tenían
(`git show HEAD:` del archivo), y la falla se anota en el extractor.

### 3. Fortified Features Framework: nada que commitear

`Keyed/vlnuw8.xml` solo cambió el fin de línea (con `--ignore-cr-at-eol` el diff sale vacío). Se
devuelve a git.

### 4. Dónde van las carpetas

Cinders of the Embergarden (autor «BreadMo AOBA»), Hemogen Pack (autor «Aoba») y Fortified Features
Framework («AOBA») quedaron sueltos en `Data/`: el `Agrupador` compara el `<author>` del `About.xml`, y
esos tres no dicen «AobaKuma». **Decisión de Angel (25-oct-2026): se quedan donde están.** Igual
pertenecen a este plan.

### 5. Campos que guardan una clave: se dejan en `TODO`

En algunos campos el «inglés» es el nombre de una clave de `Keyed` (`DMSAC_PatrolSquad`,
`USAC_DebtSite_AirStrike`…). El mod lee ese valor y lo busca en `Keyed`, donde está el texto de verdad,
que sí se traduce. Si se escribiera una traducción en el campo, el mod buscaría una clave que no existe
y en pantalla saldría el nombre en crudo. Se dejan en `TODO`, que el juego no aplica.

- **AncientCorps:** 22 (los `letterLabel`/`letterText` de `WorldObjectDef`, los de
  `Fortified.HuntdownDef` y los niveles de `Fortified.ReadinessDef`). Comprobadas una por una contra
  `Keyed`.
- **Ascension Megacorp:** 22 campos con 8 claves distintas, todas comprobadas en `Keyed`.
- **Excepción:** `USAC_HostilityReset.label` (`HistoryEventDef`) dice `Hostilityresetbysystem_label`,
  pero esa clave no existe en ningún lado: es un descuido del autor y se traduce como texto
  («hostilidad restablecida por el sistema»).

### 6. Cómo se traduce: a mano o con tabla y script

Las reglas de RML piden editar a mano. Para Hegemony, por su tamaño, Angel aprobó otro camino: una
tabla clave → traducción escrita a mano, una por una, y un script que la copia solo sobre los `TODO`,
con una revisión después de cada tanda (`PLAN-HEGEMONY.md`). Hegemony está en pausa desde el
25-oct-2026: el autor todavía no le da soporte de traducción a la ventana del mod.

**Decisión de Angel (25-oct-2026): el mismo camino para Expedition, Ascension y Cinders.** Anotado en
`RML/AGENTS.md`. Desde el 26-oct-2026 Angel lo autorizó para cualquier mod, así que los demás de la
familia pueden ir a mano o con tabla. Cómo se hace, por tanda:

1. Leer los ingleses pendientes de una parte del mod (los `TODO`).
2. Escribir la tabla clave → traducción en la carpeta temporal de la sesión, a mano, entrada por
   entrada, con el glosario del mod abierto.
3. Copiarla con `python -I Herramientas/traducir.py aplicar <mod> <tabla.tsv>`, que solo escribe sobre
   `TODO`.
4. Leer su informe: revisa contra la copia de antes, los términos del glosario del mod (`--glosario` para
   sumar los de la familia) y las tres comprobaciones del diff. Si una tanda deja un término nuevo, va al glosario del mod antes de la tanda siguiente.

Los campos que guardan una clave (punto 5) **no van en la tabla**: se quedan en `TODO`.

### 7. Términos que necesitan a Angel

**Decisión de Angel (25-oct-2026): aceptó todas las propuestas de los glosarios**, con dos
precisiones:

- **«the corps» de AncientCorps es «el Cuerpo»**, con mayúscula (opción A de «El Cuerpo: opciones» de
  `GLOSARIO-AC.md`). De paso se corrige «dados de baja del cuerpo» en `DMSAC_Graveyard.description`, en
  su propio commit.
- **USAC se explica en las descripciones** con la fórmula de la oficial y de las armas de la familia:
  «la Compañía Unida de Armamento Estelar o por sus siglas en inglés USAC (United Stellar Armament
  Company)», una vez por descripción, y siempre en la de la facción. El detalle está en `GLOSARIO-USAC.md`.

Quedan decididos: «escopeta de combate Talca» (cambio de valor, en su propio commit; primero fue «Tula»), los rótulos de la
terminal de Ascension (código en inglés, lo de después de `//` traducido), los apodos de sus mecas
(«M7 cobalto», «M88 rocky»), «cinder», «Flota Linterna», «la Cuna», «Fumée», «magier» y «Konzentrat».

## Reglas comunes a toda la familia

**Qué manda.** De más a menos: la traducción oficial del juego, [RML/GLOSARIO.md](../../GLOSARIO.md),
los glosarios de la familia ([GLOSARIO-DMS.md](The%20Dead%20Man's%20Switch%20-%203819799478/GLOSARIO-DMS.md),
[GLOSARIO-FFF.md](../Fortified%20Features%20Framework%20-%203498575851/GLOSARIO-FFF.md)) y el del mod.
**Si un glosario contradice a la oficial, gana la oficial** (Angel, 25-oct-2026). Con esa regla se
corrigieron el 25-oct, en los glosarios: *shuttle* = «transbordador» (no «lanzadera»), *EMP* = «PEM»,
*psycast* = «psicoemisión», *neural heat* = «calor neural», *antigrain warhead* = «obús de antigrano» y
seis nombres de Biotech (ver `GLOSARIO.md`). Los textos ya traducidos con el término viejo quedan
pendientes, con su cuenta en cada fila.

**La oficial, a mano.** Desde el botón de RML de la traducción oficial, está desempaquetada en
`D:\SteamLibrary\steamapps\common\RimWorld\Data\<Core o DLC>\Languages\SpanishLatin (Español(Latinoamérica))`
(el `.tar` quedó como `.tar.respaldo-rml`). Se busca ahí con `grep`. Para ir del inglés al español, el
inglés está en `Data/<DLC>/Defs` y en `Languages/English`, con el mismo `defName`.

**Se toma de la oficial, tal cual**, cuando el inglés es idéntico al del juego:

- **Partes del cuerpo** (`BodyDef`): 84 de las 118 de Cinders y 66 de las 120 de Ascension tienen el
  mismo nombre que una parte del juego («left lung» → «pulmón izquierdo»). Están en
  `DefInjected/BodyDef/Bodies_Humanlike.xml` y `Bodies_Mechanoid.xml` de Core.
- **Rasgos de armas** (`WeaponTraitDef`, de Odyssey): «fuego rápido», «cargador extendido», «mira
  táctica», «miras de mala calidad», «mira de rastreo», «mira con telémetro», «balas de punta hueca»,
  «balas PEM», «recarga rápida», «ligero», «feo», «torpe», «ornamental». Cinders tiene 74 entradas de
  rasgos.
- **Nombres de armas únicas**: las reglas de `RulePackDef` de Cinders copian
  `RulePacks_Namers_UniqueWeapons.xml` de Odyssey (`[badass_concept]`, `[weapon_type]`): la oficial
  dice cómo armarlas.
- **Psicoemisiones**: *psylink* «psicoenlace», *psycaster* «psicoemisor», *psycast* «psicoemisión»,
  *neural heat* «calor neural», *psychic sensitivity* «sensibilidad psíquica», *psylink neuroformer*
  «neuroformador de psicoenlace».
- **Genes de cabello**: «cabello castaño oscuro», «cabello negro oscuro», etc. (`GeneDef` de Biotech).

**Lo de la familia que ya está decidido:**

- **Armas**: las reglas de [ARMAS-DMS.md](The%20Dead%20Man's%20Switch%20-%203819799478/ARMAS-DMS.md) y
  de la memoria «armas al estilo del juego»: minúscula, «fusil» para lo militar, siglas como el juego
  (LMG, HMG, SMG, DMR) explicadas en la descripción, apodos traducidos sin comillas («GAU-4 vulcano»).
  Expedition redefine armas de DMS (el «sistema de disparo rápido multipropósito tipo 85», el GAU-4):
  van con el mismo nombre que en DMS.
- **Munición de Combat Extended**: secciones 3 a 6 de
  [GLOSARIO-COMBAT-EXTENDED.md](../Combat%20Extended%20-%202890901044/GLOSARIO-COMBAT-EXTENDED.md):
  sustantivo, calibre tal cual y tipo («cartucho 5x50mm Caseless (AP)», «bala 7.62x51mm NATO (DU)»),
  y las recetas con la plantilla oficial («fabricar cartucho … x500» / «Fabrica …» / «Fabricando …»).
  En la familia se aprobó además *slug* = «posta» y *airburst* = «de estallido aéreo», que le ganan al
  glosario de CE dentro de DMS.
- **Unidades en el texto**: «23 mm», con espacio (91 usos así en la familia, ninguno pegado). En el
  nombre de una munición de CE el calibre va tal cual: «5x50mm».
- **Trasfondos** (`BackstoryDef`, unos 290 entre los mods): como la oficial, con
  `{PAWN_gender ? o : a}` para el género («fue reclutad{PAWN_gender ? o : a}») y sin pronombres
  inventados.
- **Reglas de gramática**: cada `<li>` conserva su `simbolo->` y los símbolos no se traducen. Las
  opciones de un mismo símbolo van en el mismo género si la regla que las usa pone el artículo una sola
  vez («El [garage]»).
- **Mecas y facciones**: *mech* «meca» (masculino), *automatroid* «automatroide», *drop pod* «cápsula
  de escape», *Colonial Fleet* «Flota Colonial», *huntdown* «cacería».

## Orden de trabajo

Una tarea por renglón; las grandes se parten en sesiones. Un commit por grupo de archivos, con el mod
en el mensaje.

| # | Tarea | Sesiones | Por qué en este orden |
|---|---|---|---|
| 0 | Commitear lo que ya está bien (abajo). Todo lo demás está decidido desde el 25-oct-2026 | — | Destraba todo lo demás |
| 1 | Hemogen Pack (5) y los 3 de MobileDragoon | 1, junto con la 2 | Casi todo es oficial |
| 2 | AncientCorps (122) | 1 | Ya tiene glosario y plan |
| 3 | Taktikmagier (182) | 1 | Chico; fija las psicoemisiones para el resto |
| 4 | Citadel (272) | 1 | Chico; fija las fortificaciones que usa Ascension |
| 5 | Expedition (1.295) | 5 | Depende de DMS; sus misiles aparecen en Cinders y Ascension |
| 6 | Ascension Megacorp (1.355) | 4 | Usa DMS (automatroides, M88), Citadel y munición de CE |
| 7 | Cinders (1.514) | 3 o 4 | El más independiente de DMS |

**Lo que ya se puede commitear** (paso 0): Synthetic y los `Patches` de MobileDragoon y AncientCorps,
como los escribió el extractor; el `Keyed/vlnuw8.xml` de FFF, en cambio, se devuelve a git; la parte de AncientCorps que no tiene `TODO`
(compañías movidas a `Fortified.CompanyDef`, misiones viejas al `UNUSED.xml`, archivos borrados); las
carpetas nuevas tal como salieron del extractor; este plan y los glosarios, y las correcciones de
`GLOSARIO.md`, `GLOSARIO-DMS.md` y `GLOSARIO-FFF.md`. Las carpetas nuevas con todo en `TODO` no le
cambian nada al jugador: el juego no aplica un `TODO`.

Al terminar cada mod: las tres comprobaciones del diff, que no quede un `TODO` fuera de los que se dejan a
propósito, `LoadFoldersBuilder -build` y una prueba en el juego.

## Cada mod

### The Dead Man's Switch - AncientCorps (122 por traducir)

Glosario: [GLOSARIO-AC.md](The%20Dead%20Man's%20Switch%20-%20AncientCorps%20-%203469398006/GLOSARIO-AC.md).

- **Las compañías cambiaron de clase**: `AncientCorps.CompanyDef` pasó a `Fortified.CompanyDef`. Las 27
  traducciones del archivo borrado están en el nuevo.
- **Las misiones se rehicieron**: las siete `DMSAC_OpportunitySite_*` ya no existen y el mod trae
  `DMSAC_Installation_Field`, `_Logistic`, `_Facility` y `DMSAC_Boss`. Las traducciones viejas quedaron
  en `UNUSED.xml`, como corresponde; sirven de referencia («Instalación», «Fortaleza», «Alijo»), pero no
  se copian, porque los textos son otros.
- **Lo nuevo**: el sistema DEFCON (`Fortified.ReadinessDef`), la cacería (`Fortified.HuntdownDef`), la
  red de alerta temprana, el automatroide de carga saray RGM-4, seis módulos de armas montables y las
  cuatro municiones de la escopeta KS-23K.

| Paso | Archivos | Entradas |
|---|---|---|
| 1 | `Keyed/ie46y1.xml`: cartas y alertas de la cacería, del sitio, de la hostilidad permanente y los rótulos DEFCON. Primero, porque fija el tono | 22 |
| 2 | Módulos de armas: `ThingDef` (`DMSAC_Mod_*` y sus «Usar {0_label}»), `HediffDef/so76kl.xml` y `AbilityDef/qab94b.xml`. Las tres partes nombran los mismos módulos y tienen que coincidir | 36 |
| 3 | Saray y la baliza: `DMS_Mech_Saray` (`ThingDef`, `PawnKindDef`), la receta `DMSAC_Make_Saray` y `DMSAC_BossCaller` | 9 |
| 4 | Munición de la KS-23K: `DMS_KS23K.comps.CompAmmoSwitch.ammos.*` y los cuatro proyectiles | 15 |
| 5 | Misiones: 38 reglas en 9 archivos de `QuestScriptDef` | 38 |
| 6 | `DMSAC_Defcon.label` («DEFCON») y `DMSAC_EarlyWarningNet.label` | 2 |

Prueba en el juego: una misión de instalación, el nivel DEFCON en pantalla y un saray ensamblando.

### The Dead Man's Switch - Taktikmagier (182)

Glosario: [GLOSARIO-TAKTIKMAGIER.md](The%20Dead%20Man's%20Switch%20-%20Taktikmagier%20-%203826500999/GLOSARIO-TAKTIKMAGIER.md).
Coautores: AobaKuma, Bill Doors y KV4EX. Solo 1.7.

Psicoemisores tácticos («magier»): 17 habilidades (`AbilityDef`, 691 palabras), su equipo y sus
construcciones (gárgola, torreta centinela), 14 trasfondos (`BackstoryDef`, 42 entradas), 20 reglas de
`RulePackDef` y 21 textos de `Keyed`. 8 entradas son de su carpeta de CE.

### Fortification Industrial - Citadel (272)

Glosario: [GLOSARIO-CITADEL.md](Fortification%20Industrial%20-%20Citadel%20-%202887532326/GLOSARIO-CITADEL.md).
Llega hasta 1.6; en 1.7 el juego carga la de 1.6.

Muros, troneras y búnkeres con armas (181 entradas de `ThingDef`, 1.874 palabras), investigación con
43 reglas (`generalRules` de `ResearchProjectDef`), recetas y un escenario. Las armas de los búnkeres
existen en dos versiones, `CombatExtended/` y `Vanilla/`, con los mismos nombres: se traducen igual.

### The Dead Man's Switch - Expedition (1.295)

Glosario: [GLOSARIO-EXPEDITION.md](The%20Dead%20Man's%20Switch%20-%20Expedition%20-%203689879335/GLOSARIO-EXPEDITION.md).

Misiles armados por partes (cuerpo, ojiva, guiado, carga útil), radares, defensa antiaérea, munición,
plataformas orbitales y el viaje de la gravinave. Es texto técnico y largo: las descripciones de las
partes explican cómo apunta cada una.

| Parte | Entradas | Palabras |
|---|---|---|
| `ThingDef` | 339 | 5.328 |
| `Keyed` (la interfaz de misiles y de la transferencia) | 549 | 3.836 |
| `DMSE.MissilePartDef` (partes de misil) | 138 | 2.171 |
| `DMSE.MissileBodyDef` (cuerpos de misil) | 34 | 812 |
| `ResearchProjectDef` | 22 | 400 |
| `RecipeDef` | 75 | 352 |
| Lo demás (sitios, condiciones, trabajos, terreno, escenario) | ~140 | ~600 |

- **Dos versiones distintas**: la 1.6 está congelada en la 0.35 del Workshop y la 1.7 es la de
  desarrollo. Se extrajo la 1.7. La carpeta de RML no tiene bloque `Version`, así que también carga en
  1.6: lo que no exista allá simplemente no se aplica.
- **Partes condicionales**: 102 entradas son de `1.6/CE` y `1.7/CE` (que hoy no cargan en 1.7, ver el
  punto 1) y una carpeta `1.7/Cinders` se carga con Cinders.
- Sesiones sugeridas: 1) `Keyed`; 2) partes y cuerpos de misil; 3) y 4) `ThingDef`; 5) recetas,
  investigación y lo demás.

### Ascension Megacorp (1.355)

Glosario: [GLOSARIO-USAC.md](Ascension%20Megacorp%20-%203678583415/GLOSARIO-USAC.md).

La facción United Stellar Armament Company (USAC): una corporación que vende armas y mecas por una
terminal propia, presta, cobra intereses y manda a cobrar. Trae su cultura (`CultureDef` y unas 160
reglas para nombrar su ideología), títulos y permisos al estilo Royalty, trasfondos, el narrador
«Rookie Rockefeller» y munición de uranio empobrecido (DU) para CE.

| Parte | Entradas | Palabras |
|---|---|---|
| `Keyed` (la terminal: catálogo, finanzas, contrato) | 392 | 3.309 |
| `ThingDef` | 182 | 1.853 |
| `BackstoryDef` | 84 | 1.109 |
| `USAC.USACProductDef` (fichas del catálogo) | 52 | 973 |
| Reglas de `RulePackDef` y `QuestScriptDef` | ~250 | ~1.400 |
| `TipSetDef`, `BodyDef`, permisos, títulos, afecciones y lo demás | ~395 | ~2.000 |

- Todo su contenido está en `1.6/` (que también carga en 1.7); 36 entradas son de `1.6/Royalty` y 81 de
  `1.6/CE`.
- Los 22 campos con clave del punto 5.
- Sesiones sugeridas: 1) y 2) `Keyed`; 3) `ThingDef` y catálogo; 4) trasfondos, reglas y lo demás.
- **Terminado el 27-oct-2026** (cuarta sesión: `a4d2f4f6` a `a5b03047`). Quedan 15 `TODO`, todos a propósito:
  8 campos con clave de `WorldObjectDef` y 7 nombres internos del guion en `QuestScriptDef`. Las 66 partes del cuerpo iguales a las del juego salieron de la oficial (`Bodies_Humanlike.xml` y
  `Bodies_Mechanoid.xml`); los mecas dicen «pierna», como Lancer y Pikeman. Falta la prueba en el juego.

### Cinders of the Embergarden (1.514)

Glosario: [GLOSARIO-CINDERS.md](../Cinders%20of%20the%20Embergarden%20-%203421959154/GLOSARIO-CINDERS.md).
Autores: BreadMo y AOBA. Carga en 1.5, 1.6 y 1.7.

Un xenotipo (*Cinder*), sus facciones, el «polímero amorfo», la «erosión» (munición y agentes), mecas
caminantes con tentáculos, un culto y la narradora Fumée.

| Parte | Entradas | Palabras |
|---|---|---|
| `ThingDef` | 462 | 3.796 |
| `BackstoryDef` | 162 | 1.727 |
| `WeaponTraitDef` (casi todo oficial) | 74 | 688 |
| Reglas de `RulePackDef` (nombres de armas y de culto) | 149 | ~690 |
| `RecipeDef` | 114 | 621 |
| `HediffDef`, `BodyDef` (84 oficiales), `GeneDef`, `AbilityDef`, investigación y lo demás | ~550 | ~2.300 |

- 263 entradas son de las carpetas `CE/` (la munición «Erosion»), 35 de `Biotech/` y 22 de `Har/` (Humanoid
  Alien Races).
- Sesiones sugeridas: 1) lo oficial (partes del cuerpo, rasgos, genes) y `ThingDef` hasta la mitad;
  2) el resto de `ThingDef` y recetas; 3) trasfondos; 4) reglas, afecciones y lo demás.
- **Sesión 1 hecha el 27-oct-2026** (`09221554` a `106f7039`): `BodyDef`, `WeaponTraitDef`, `GeneDef`,
  `HairDef` y su categoría, y `ThingDef` hasta pasar la mitad (247 de 462). Lo que vino de la oficial
  (partes del cuerpo, rasgos de Odyssey, genes de pelo de Biotech, torretas y textos de análisis de Core
  y Anomaly) quedó tal cual. Términos nuevos en el glosario del mod.
- **Sesión 2 hecha el 27-oct-2026** (`b51eeeba` a `a3d227a6`): el resto de `ThingDef` y las 114 recetas.
  Quedan en `TODO` 8 descripciones de `ThingDef` cuyo inglés es literalmente «TODO» (armas montadas de los
  mecas: el autor no las escribió); se dejan así, porque el juego mostraría lo mismo. Las recetas de munición
  se escribieron a mano, fila por fila, con la plantilla oficial (en este mod no rige la excepción de
  scripts de Combat Extended).
- **Sesión 3 hecha el 27-oct-2026** (`15858dc7` a `cb3e0d22`): los 162 trasfondos y las 66 afecciones
  (`HediffDef`). En los trasfondos, *Defense Council* va con mayúscula («Consejo de Defensa») porque ahí es el
  gobierno de los refugios; los títulos entre comillas del inglés (los oficios del largo sueño) van entre «».
  Términos nuevos en el glosario del mod.
- **Sesión 4 hecha el 27-oct-2026** (`e9e5abb5` a `65a8cbda`): todo lo que quedaba. **Cinders está terminado**:
  solo quedan los 8 `TODO` de `ThingDef` cuyo inglés es «TODO». En las reglas de nombres de armas únicas la
  oficial separa los conceptos femeninos con otro símbolo (`badass_conceptF`), y `aplicar` no deja cambiar el
  símbolo de una regla: van sin artículo («fusil de venganza») y con adjetivos que no cambian de género. La Flota
  Linterna numera sus grupos al final y sin ordinal («División de Asalto 123»), y sus bases son todas «nave».
  `LoadFoldersBuilder -copia` pasó. Falta la prueba en el juego.

### Hemogen Pack - Emergency transfusion (5)

Sin glosario: «pack de hemógeno» y «hemorragia» son oficiales («blood loss» es `BloodLoss`,
«hemorragia»). Cuatro textos de `Keyed` y un `JobDef`.

## Pendientes fuera de este plan

- **Términos viejos en textos ya traducidos**, por la regla de la oficial: «mundo del borde» por «mundoborde» (13 usos en
  5 archivos: DMS de 1.6, VRE - Sanguophage, Progression Scenarios y Persona Mechanoid Pawns 2), «lanzadera» (172 usos en 40
  archivos), «entropía psíquica» (26), «abrasador» (22, a revisar: algunos son el adjetivo),
  «psiconjuro» (14), «EMP» (14 en DMS II y 1 en FFF), «calor neuronal» (7), «sanguófago» (6),
  «ensamblador de genes» (6), «clúster mecanoide» (4), «letargo mortal» (2), «ojiva de antigrano» (2),
  «barredoide» y «bomba de descontaminación» (1 cada uno). Se corrigen a mano, mod por mod, sin tocar
  los `UNUSED.xml`, como los de «cápsula de escape» y «garra craneal» que ya estaban pendientes.
- **Otros glosarios de RML** con posibles choques con la oficial, a revisar en su contexto: Combat
  Extended («penetración de armadura» / oficial «penetración de blindaje», «cadencia de fuego» /
  «cadencia de tiro», «alto el fuego» / «cese al fuego», «poder de detención» / «potencia de parada»),
  RimPacts («ideología» / «ideoligión», «charlas de paz» / «diálogos de paz», «lanzadera»), Caravan
  Adventures («concejal» / «concejal citadino») y Alpha Prefabs («magister» / «magistrado»). Se
  encontraron cotejando cada fila contra la oficial; la mayoría de los demás avisos eran palabras sueltas
  sin relación.
- **El extractor**: la condición de CE (punto 1) y el cruce de `Patches` que cambia de posición a clase
  (punto 2).
- **El chorro del lanzallamas en DMS:** corregido en DMS II el 25-oct (`c1996d28`, Angel dijo que sí):
  `Bullet_DMS_Flamethrower_Bioferrite` es «chorro de quimbustible infundido», como en Citadel, y
  `_Prometheum` dice «prometio». El inglés del mod repite *jellied prometheum stream* en el de bioferrita:
  es un descuido del autor, y la munición que dispara es el quimbustible infundido de CE. El DMS de 1.6
  (`7y4coj.xml`) sigue con «prometeo», porque está congelado.

## Estado al 27-oct-2026: traducción terminada

Los siete pasos del orden de trabajo están hechos: Cinders se terminó en su cuarta sesión. Lo único que queda en
 en la familia es lo que se deja a propósito: los campos que guardan una clave (AncientCorps y Ascension, punto
5) y las 8 descripciones de Cinders cuyo inglés es «TODO». En  hay 29 commits sin subir.

**Lo que falta (Angel):**
1. Decir «subir RML» para que llegue a GitHub y al Workshop.
2. La prueba en el juego de cada mod (en la consola de RimWorld, que no salgan errores de traducción, y mirar los
   nombres de armas únicas, las facciones de Cinders y la narradora Fumée).
3. Si quiere cambiar algún nombre propuesto, está en el glosario de cada mod con «propuesta».

**Lo siguiente, en otras sesiones:** «Pendientes fuera de este plan» (arriba): los términos viejos en textos ya
traducidos y la condición de CE en el extractor.
