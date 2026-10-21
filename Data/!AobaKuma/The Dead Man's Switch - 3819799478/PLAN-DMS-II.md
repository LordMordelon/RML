# Plan: The Dead Man's Switch II (1.7)

Traducir lo nuevo de The Dead Man's Switch II y poner al día lo que cambió respecto del original.
Lo pidió Angel el 20-oct-2026. Esta carpeta es la que se sigue; la del original
(`The Dead Man's Switch - 3121742525`, para 1.6) queda congelada y no se toca.

## Contexto

- **Son dos mods** con el mismo packageId: el original (Workshop 3121742525, hasta 1.6) y el II
  (3819799478, solo 1.7). Cada carpeta carga en su versión por el `Default` del yaml.
- **Una sola extracción del II:** el commit `8cb000f` (20-oct-2026), que metió todo junto: lo nuevo,
  lo conservado y lo que cambió. `bea84c4` solo devolvió la carpeta del original y puso la versión en
  los yaml. Comprobado: salvo esa línea del yaml, la carpeta del original es el «antes» de `8cb000f`
  y esta es el «después». Las cifras de abajo salen de comparar las dos, clave por clave, que es leer
  ese commit separado por tipo de cambio.
- **El extractor conserva la traducción aunque el inglés cambie**: solo actualiza el comentario
  `<!-- EN: -->`. Una traducción vieja con el inglés nuevo arriba es lo que hay que revisar.
- El extractor de Angel tiene que estar en **1.7** (Opciones → «Versión base de RimWorld»). Con 1.6
  extrae solo la raíz del mod y aparta casi todo, como pasó el 20-oct. Hay una tarea para que lo
  detecte solo: RimworldExtractor#1.

## Cifras (contadas el 20-oct-2026)

| Qué | Cuántas |
|---|---|
| Entradas en la carpeta (una lista cuenta como una) | 3.306 |
| Ya traducidas | 1.874 |
| Pendientes (`TODO`), por entrada | 1.432 |
| Pendientes, por renglón (cada elemento de lista aparte; es la cifra del extractor) | 1.792 |
| Pendientes que no existían en el original (contenido nuevo) | 1.430 |
| Traducidas con el inglés cambiado | 63 (más 9 que solo cambiaron espacios) |
| Pendientes cuyo inglés ya está traducido en otra clave | 45 |
| Reglas de gramática sin «->» | 44, en 5 archivos |
| Nombres de símbolo de gramática traducidos | 19, en 5 archivos |
| Apartadas en `UNUSED.xml` (del original, que el II ya no trae) | 407 |

Pendientes por tipo: `ThingDef` 506, `Keyed` 325 (el original no tenía ninguna traducida),
`RecipeDef` 72, `BodyDef` 63, `SitePartDef` 58, `HediffDef` 45, `QuestScriptDef` 43,
`ResearchProjectDef` 42, `ThoughtDef` 40, `PreceptDef` 24, `PawnKindDef` 21, `TaleDef` 21,
`DesignatorDropdownGroupDef` 19, `BackstoryDef` 18, `HistoryEventDef` 15, `AbilityDef` 13,
`SongDef` 10, `Patches` 10, `FactionDef` 10, y 77 más repartidas en 26 tipos.

## Etapas

| # | Etapa | Tamaño | Estado |
|---|---|---|---|
| 1 | Reparar lo dañado | chica | hecha (20-oct-2026) |
| 2 | Revisar lo que cambió en inglés | chica | hecha (20-oct-2026) |
| 3 | Glosario del mod | chica | hecha: [GLOSARIO-DMS.md](GLOSARIO-DMS.md) |
| 4 | Traducir lo nuevo | grande | en curso: puntos 1 a 4 hechos, y del 5 `SitePartDef` e `IncidentDef`; siguen `QuestScriptDef` y `TaleDef` |
| 5 | Verificar y probar en partida | chica | pendiente |

### 1. Reparar lo dañado

Va primero porque son errores que el juego ya muestra mal hoy.

- **El shock biónico quedó corrido un lugar.** (Hecho: en realidad el II *quitó* la etapa «Initial»
  del principio; el efecto era el mismo.) El II agregó una etapa al principio de
  `DMS_BionicShock`, y como sus etapas se nombran por posición, cada traducción quedó en la etapa de al
  lado: donde el juego dice «LV.2 Minor» se lee «NV.1 Inicial». Son `stages.0` a `stages.4`: se
  traducen otra vez contra su `EN:` (LV.1 Initial, LV.2 Minor, LV.3 Serious, LV.4 Catatonic,
  LV.5 Disabled), y se revisa si hay un `stages.5` en `TODO`.
- **44 reglas de gramática sin «->»** (`SendForm&gt;texto` en vez de `SendForm-&gt;texto`), en
  `QuestScriptDef/pazit9.xml`, `vcq2da.xml`, `wkm5jl.xml` y `RulePackDef/1fvcpn.xml`, `rh1oai.xml`.
  Ya estaban así en el original, antes de `8cb000f`. Antes de arreglarlas, `git log -S` para ver qué
  commit les sacó el guion, por si la causa sigue en el extractor. La comprobación 3 de
  `RML/AGENTS.md` tiene que salir vacía después.
- **19 símbolos de gramática traducidos**: `[MecaTipo]`, `[Meca]`, `[MecaAdj]`, `[MecaVerbo]`, `[Tipo]`
  y `[Número]`, donde el original dice `[MechType]`, `[Mech]` y parecidos. Un símbolo es un nombre
  interno: traducido, la regla apunta a algo que no existe y en pantalla sale el corchete. Están en
  `RulePackDef/1fvcpn.xml`, `3ituv0.xml`, `pavjzd.xml`, `pksxnq.xml` y `rh1oai.xml`. Se vuelven al
  nombre del `EN:`; lo que va después del «->» sí se traduce.

### 2. Revisar lo que cambió en inglés

63 traducciones tienen el inglés nuevo arriba y el texto viejo abajo. Se corrigen a mano contra su
`EN:`.

**Nombres que cambiaron** (los que más se ven):

| Clave | Antes | Ahora | Hoy dice |
|---|---|---|---|
| `DMS_DeadMansSwitch.discoveredLetterTitle` | About: Advanced Parts | About: Special Components | Acerca de: Piezas Avanzadas |
| `DMS_HumanoidEngineering.label` | Humanoid Engineering | Ergonomics | ingeniería humana |
| `DMS_Ronin.label` | Peacekeepers | Specialized design | pacificadores |
| `DMS_Turret_Sentry.label` | Sentry turret | Sentry gun | torreta centinela |
| `DMS_Bullet_RiotRevolver.label` | 20 gauge beanbag | 20 gauge rubber slug | bala de goma calibre 20 |
| `DMS_Bullet_RiotShotgun.label` | 4 gauge bean bag | 4 gauge rubber slug | bala de goma calibre 4 |
| `DMS_Bullet_RR.label` | Shape-charged Grenade | Airburst canister | Granada de carga hueca |
| `DMS_DeposableRocketLauncher.label` | Disposable Launcher | disposable launcher | Lanzador desechable |

Más `DMS_DeadMansSwitch.discoveredLetterText` y `DMS_Tab.generalDescription`, que cambiaron de texto,
y las cinco etapas del shock biónico de la etapa 1.

**Descripciones que cambiaron (48):**

- **HediffDef** (1): `DMS_BF_ExtendCargo`
- **RecipeDef** (7): `DMS_Make_Hyrax`, `DMS_Make_Falcon`, `DMS_Make_Noctula`, `DMS_Make_Dogge`,
  `DMS_Make_Soldat`, `DMS_Make_Grenadier`, `DMS_Make_Sergeant`
- **ResearchProjectDef** (23): `DMS_UAVBasis`, `DMS_Material`, `DMS_Plasfiber`, `DMS_Automatroid`,
  `DMS_MechBasis`, `DMS_Legion`, `DMS_Prosthetic`, `DMS_DeadMansSwitch`, `DMS_Artifuscle`,
  `DMS_EquipmentBasis`, `DMS_Mechlink`, `DMS_EquipmentAdvanced`, `DMS_EquipmentElite`,
  `DMS_EquipmentHeavy`, `DMS_BattleFrame`, `DMS_Hybrid`, `DMS_Bionics`, `DMS_OrganSurgery`,
  `DMS_Hypothesis`, `DMS_HeavyAutomatroid`, `DMS_HumanoidEngineering`, `DMS_Ronin`, `DMS_Synthroid`
- **ThingDef** (17): `DMS_Turret_Sentry`, `Neuroglue`, `DMS_BF_ExtendCargo`, `DMS_Mech_EscortLifter`,
  `DMS_Mech_ShaftWorker`, `DMS_Mech_Caretta`, `DMS_Mech_Killdozer`, `DMS_Mech_Iguana`,
  `DMS_Mech_Gecko`, `DMS_Mech_Falcon`, `DMS_Mech_Pioneer`, `DMS_Mech_Gladiator`, `DMS_Mech_Phalanx`,
  `DMS_Mech_Sergeant`, `DMS_Mech_Dogge`, `DMS_MechGestatorSmall`, `DMS_GestaltEngine`

Las de `RecipeDef` pasaron de una frase («Gestate a hyrax Automatroid.») a una descripción completa
del automatroide: conviene seguir el estilo de la descripción del `ThingDef` del mismo automatroide.

### 3. Glosario del mod

Antes de traducir en cantidad, un `GLOSARIO-DMS.md` en esta carpeta con los términos que el mod repite
y que ya tienen traducción en uso: automatroide, armazón de batalla, Industrias Nara, los nombres de
los automatroides (hirax, halcón, nóctulo, dogo…), etcétera. Se arma leyendo las 1.874 traducidas,
no se inventa. `RML/GLOSARIO.md` (terminología oficial de RimWorld) manda sobre lo que diga este.

### 4. Traducir lo nuevo

1.792 renglones. Primero lo que el jugador ve más, y en cada tipo primero los nombres y después las
descripciones:

1. `Keyed` (325): textos de la interfaz del mod.
2. `ThingDef` (506): armas, edificios, automatroides.
3. `ResearchProjectDef` (42) y `RecipeDef` (72).
4. `HediffDef` (45), `BodyDef` (63) y `AbilityDef` (13).
5. Misiones y sucesos: `QuestScriptDef` (43), `SitePartDef` (58), `IncidentDef`, `TaleDef` (21).
6. Ideología: `PreceptDef` (24), `HistoryEventDef` (15), `ThoughtDef` (40), `MemeDef`, `IssueDef`.
7. El resto.

Las **45 pendientes cuyo inglés ya está traducido** en otra clave (de esta carpeta, de la del original
o de `UNUSED.xml`) van primero dentro de su tipo: se copia esa traducción, revisando que el contexto
sea el mismo.

Reglas: `RML/AGENTS.md` («Editar XML de traducción» y «Reglas estrictas de XML»). A mano y
reemplazando solo el `TODO`; en las reglas de gramática, el símbolo y el «->» quedan como en el `EN:`.

### 5. Verificar y probar

- Las tres comprobaciones del diff de `RML/AGENTS.md`, vacías.
- `dotnet run --project Source/LoadFoldersBuilder -c Release -- -build` (índice y `output/`).
- Angel: una partida en 1.7 con el mod, mirando la investigación, un automatroide, el shock biónico y
  una misión del mod.

## Decisiones de Angel

1. **Los nombres que cambiaron (tabla de la etapa 2): aprobados** (Angel, 20-oct-2026): «arma
   centinela» para Sentry gun, «posta de goma» para rubber slug, «cartucho de estallido aéreo» para
   Airburst canister, «ergonomía» para Ergonomics, «diseño especializado» para Specialized design y
   «Acerca de: componentes especiales» para la carta.
2. **Script para lo repetitivo: aprobado** (Angel, 20-oct-2026), solo para las 72 recetas y las
   partes del cuerpo (`BodyDef`, 63), con una tabla que Angel revisa antes de aplicarla, como en
   Combat Extended. Todo lo demás, a mano. Al hacerlo, anotar la excepción en «Editar XML de
   traducción» de `RML/AGENTS.md`, que hoy la permite solo en Combat Extended.

## Sesiones (estimación)

- Sesión 1: etapas 1, 2 y 3, y `Keyed` de la etapa 4.
- Sesiones 2 y 3: `ThingDef` y el resto de la etapa 4 (unos 700 renglones por sesión).
- Sesión 4: lo que quede, y la etapa 5.

## Próxima sesión (20-oct-2026, después de la sesión 3)

**Objetivo:** etapa 4, punto 5: `QuestScriptDef` (140 `TODO`, en 38 archivos) y `TaleDef` (124, en 8),
a mano.

**Estado:** en la sesión 3 se tradujeron `ResearchProjectDef`, `RecipeDef` y `BodyDef` (estos dos con
tablas aprobadas por Angel), `HediffDef`, `AbilityDef`, `SitePartDef` e `IncidentDef`. Todo en RML
`master`, en local (último commit, el de esta nota); se sube cuando Angel lo diga. Quedan 641 `TODO` en
`DefInjected`, contados por renglón. Sin cambios a medias.

**Antes de empezarla (Angel):** nada.

**Al empezar (Claude):**
1. `QuestScriptDef` y después `TaleDef`, un commit por tramo de unos 40 renglones. Son casi todos reglas
   de gramática: el símbolo y el «->» quedan como en el `EN:` y solo se traduce lo de después (la
   comprobación 3 tiene que salir vacía). Los pronombres de persona se omiten (ver el glosario).
2. Para los términos que existen en el juego, comparar con la traducción oficial: los `.tar` de
   `D:\SteamLibrary\steamapps\common\RimWorld\Data\*\Languages\SpanishLatin (Español(Latinoamérica)).tar`.
   Angel pidió usarla como referencia.
3. 264 renglones con gramática pueden no caber en una sesión: si no, `TaleDef` queda para la siguiente.

**Decisiones de Angel:** ninguna prevista.

**Guías:** ninguna (no hay tarea de GitHub). Al cerrar, `traspaso`.

**Dónde leer:** este plan, `GLOSARIO-DMS.md`, `RML/AGENTS.md` y `RML/GLOSARIO.md`. Los cambios de otra
sesión que siguen sin commitear (Combat Extended, `ModList.tsv`, `docs/`, dos XML con finales de línea)
no se tocan.
