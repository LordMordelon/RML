# Plan: lo que se vio en el juego

El 27-oct-2026 Angel probó RML en el juego y mandó capturas con textos en inglés o dudosos. Este plan
junta lo que salió, lo que ya se arregló y lo que falta. Va aparte de [PLAN-REVISION-GENERAL.md](PLAN-REVISION-GENERAL.md),
que revisa los glosarios y los términos sin abrir el juego.

## 1. Nombres de mods en la lista de opciones de mods

**Regla (Angel, 27-oct):** el nombre de un mod va en inglés o en su idioma original, también en esa lista. Ya está
en la sección 9 del [GLOSARIO.md](GLOSARIO.md).

- **Hecho:** se leyó el código (`SettingsCategory`) de los mods de RML. 13 nombres salían de una clave de `Keyed`
  que RML había traducido. 10 volvieron al inglés, y los 3 de RimPsyche y Graphics Setter («Configurador gráfico», que también es su botón del menú principal) volvieron al
  español a pedido de Angel: Cinders (2), Work Modes, Void Universe, Manipulator Beam Emitter, Ancient Hydroponics y VE Framework - Outposts. Los tres nombres cortos de Vanilla
  Expanded («VE - Psycasts», «VE - Aspirations», «VE - Helixien Gas») ya estaban en inglés y se quedan.
- **Hospitality se queda «Hospitalidad»** (Angel, 27-oct): su clave se usa en 7 lugares del código, no solo como
  nombre, y un nombre que se usa en otras partes se deja en español.
- **Falta:** mirar en el juego que la lista salga entera en inglés. Los nombres escritos en el código de cada mod
  no pasan por RML.

## 2. Textos que el extractor no saca

Un texto que sigue en inglés en el juego y no está en `Data/` casi siempre es una etiqueta que el extractor no
conoce. El arreglo va en el extractor (`Prefabs.EtiquetasAgregadas`), con su prueba, y después se reextrae el mod.
Antes de agregar una etiqueta se mide en los mods de RML que siempre sea texto (regla de `RimworldExtractor/AGENTS.md`).

| Etiqueta | Dónde se vio | Mods de RML que la usan | Estado |
|---|---|---|---|
| `defaultLabel` / `defaultDesc` | Cinders: botón «Change to walking mode» de las torretas | Solo Cinders: 24 usos, todos texto | **Hecho el 27-oct** (extractor 6789080, Cinders 03c7fc8b): «Cambiar a modo caminante» |
| `namerLabels` y `traitAdjectives` (listas) | Cinders: títulos de arte «Greatsword twisted», «Dragon slayer del temor» | 6 mods cada una: 50 y 230 usos | Medido el 28-oct; lo que sigue |

**Cómo arma el juego el título** (medido el 28-oct-2026 en el código de `CompUniqueWeapon`): junta el tipo de arma,
sacado de `namerLabels` del `CompProperties_UniqueWeapon` (que es del juego: Cinders lo usa tal cual), el color y un
adjetivo de `traitAdjectives` de uno de sus rasgos (`WeaponTraitDef`). Las reglas en español son de la oficial
(`NamerUniqueWeapon`: «[weapon_type] [trait_adjective]», «[weapon_type] del [badass_concept]»...). En Cinders,
«greatsword» es un `namerLabels` y «twisted» un `traitAdjectives` del rasgo de tentáculos; «del temor» ya viene de la
oficial. El extractor no conoce ninguna de las dos etiquetas, así que RML no tiene nada. Los mods: Cinders, Alpha
Armoury, Spacer Arsenal, Vanilla Weapons Expanded, Ushankas Glittertech Expansion y otros.

- **Las claves, como la oficial:** `Bow_Great_Unique.comps.CompUniqueWeapon.namerLabels.0` («arco grande») y
  `BioferriteBurner.traitAdjectives.0` («ardiente»). El extractor ya nombra el comp por su clase.
- **Regla para traducir (va al glosario antes de la primera tanda):** el tipo de arma, en minúscula; el adjetivo, en una
  forma que no cambie con el género, porque acompaña a cualquier arma: «letal», «ardiente», «de precisión», «con
  buscador». Así lo hace la oficial.
- **Los títulos ya generados quedan guardados en la partida:** las armas nuevas saldrán en español y las viejas no.

### Más campos que el extractor no saca (28-oct)

Se compararon los 281 campos que el juego marca como traducibles (`[MustTranslate]`) con la lista del extractor:
faltan 136. La mayoría solo los usa el contenido oficial, que ya viene traducido. **En los mods de RML, 35 tienen texto.**
Los números cuentan cada aparición en el XML del mod, con las versiones repetidas (1.5, 1.6, 1.7):

| Campo | Usos | Mods | Qué es |
|---|---|---|---|
| `labelNoLocation` | 1072 | 7 (Apex Mechanoids, Cinders, Erin's Auraeyl, Persona Mechanoid Pawns 2...) | Con qué se hizo una herida, en la pestaña de salud («garra», «escudo») |
| `traitAdjectives`, `namerLabels` | 230, 50 | 6 cada uno | El título de un arma única (arriba) |
| `activatingString`, `activatingStringPending`, `messageCompletedString` | 44, 51, 23 | 6 | Hackear o extraer: «hacking {0}: {1}s» |
| `hackedMessage`, `notHackedInspectString`, `hackedInspectString`, `hackedLetterLabel`, `hackedLetterText` | 16, 17, 7, 2, 2 | hasta 7 (familia DMS, Void Universe, Mechanoid Upgrades) | Mensajes y cartas de hackeo |
| `reportStringOverride` | 45 | 5 | Lo que hace un colono o mecanoide: «Patrolling.» |
| `sealCommandLabel`, `sealCommandDesc`, `cannotSealLabel`, `confirmSealText`, `sealedInspectString`, `cannotEnterLabel`, `enteredLetterLabel`, `enteredLetterText` | 5 cada uno | 2 (Void Universe, DMS) | Sellar la entrada de un portal o almacén |
| `tooltipOverride`, `cooldownEndedMessage`, `labelNotMet`, `customUnlockTexts`, `messageCooldownEnded`, `messageOnDisappear`, `useMessage`, `labelFemalePlural`, `beginRitualOverride`, `letterDesc`, `graphLabelY`, `enoughPlantsLetterLabel`, `enoughPlantsLetterText`, `startMessage`, `failedMessage` | 1 a 10 | 1 a 4 | Sueltos: avisos, cartas y etiquetas de VGE, VQE, Landmarks, Integrated Implants y otros |

Quedan afuera tres que aparecen en la lista y no son texto: `enteredLetterDef` (es una referencia a otro def),
`ignoreConfigErrors` (vale `true`; Ludeon lo marcó por error) y el `explanation` de Phytokin, que es otro campo con el
mismo nombre y guarda claves de `Keyed`. Antes de agregar cada etiqueta se mira que **todos** sus valores en RML sean
texto, no solo los de muestra (en DMS, un `hackedInspectString` vale `FFF_Substation_HackedLabel`, una clave: esa
entrada se deja en `TODO`, como hace `traducir.py`).

**`labelNoLocation` va con las herramientas por su etiqueta.** La oficial la nombra así:
`Bear_Grizzly.tools.left_claw.labelNoLocation` = «garra». El extractor nombra las herramientas por posición
(`tools.0`), y pasarlas a la etiqueta es la tarea 2 de `RimworldExtractor/PLAN-POSICIONES.md`. Si se agrega antes, son
más de mil claves que esa tarea tendría que mover después. Conviene hacerlas juntas.

## 3. Mayúscula inicial

«balas de uranio 14/15»: las etiquetas van en minúscula, como en la oficial. El juego pone la mayúscula donde
corresponde, y ponerla en la traducción la deja también en medio de una frase («cargar Balas de uranio»).

- **Falta:** ver qué mod muestra ese contador. Si es el código del mod el que no pone la mayúscula, se anota como
  algo del mod y la traducción no se toca.

## 4. Cómo seguir

Angel aprobó los puntos 1 y 2 el 27-oct-2026; el 3 quedó resuelto arriba.

1. ~~`defaultLabel` y `defaultDesc`~~: hecho el 27-oct.
2. Títulos de armas: `namerLabels` y `traitAdjectives` a `Prefabs.EtiquetasAgregadas`, con una prueba que pida las
   claves como la oficial (comp por su clase, lista por índice); la regla de la forma sin género al glosario;
   reextraer los mods y traducir con la tabla, mod por mod. **Va después de `RimworldExtractor/PLAN-CLASE-DEL-DEF.md`:**
   los rasgos de Custom Weapons Expanded y Combat Extended caen hoy en una carpeta con la clase de CE, y sus
   adjetivos irían a parar ahí.
   Después, con aprobación aparte: los campos de hackeo, sellado y sueltos de la tabla de arriba (mismo camino), y
   `labelNoLocation` junto con las herramientas por su etiqueta.
3. ~~Hospitality~~: resuelto, se queda en español.
4. Probar en el juego con una partida nueva o un arma nueva, y mandar capturas de lo que siga en inglés. Cada
   caso nuevo se anota aquí con dónde se vio.

Cada mod con su commit, y las tres comprobaciones del diff antes de subir.
