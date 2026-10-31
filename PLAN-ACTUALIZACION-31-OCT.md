# Plan: revisar la reextracción completa del 31-oct-2026 y traducir lo nuevo

El 31-oct-2026 se reextrajo todo RML con `RimworldExtractorLotes` (287 carpetas), después del arreglo del
extractor `4416c40`: un patch con la condición del mismo DLC dueño del def ahora sale como `DefInjected`
(Progression Robotics y Biotech). La corrida trajo además todos los cambios del extractor desde la última
extracción de cada mod. Queda **sin commitear** en `master`: 119 entradas de `git status`.

Lo pidió Angel el 31-oct: revisar, corregir y traducir las actualizaciones y el contenido nuevo. Se borra al
terminar, como los otros planes.

## Lo que ya se comprobó (31-oct)

- **No se perdió ninguna traducción.** Los dos `UNUSED.xml` borrados (VFE - Props and Decor, 200 entradas, y
  Makeshift Re-Examined, 1) quedaron vacíos porque todo volvió a usarse con el mismo valor: los `shortLabel` de
  `VFEProps.PropDef`. Las 6 que salieron de un `UNUSED.xml` sin volver (Ushankas Luciferium 2, Void universe 4)
  eran copias viejas de nodos que hoy están traducidos, y mejor.
- Ningún valor de `Patches` desapareció: lo que sale de un archivo está en otro o en `DefInjected`.
- La comprobación 1 solo marca escapes equivalentes: `&quot;` → `"` (Ascension Megacorp) y `&#32;` → espacio
  (RimIris). La 3 sale vacía.
- Los avisos «LF will be replaced by CRLF» son solo saltos de línea.

## Grupos de cambios

| Grupo | Mods | Qué hacer |
|---|---|---|
| El arreglo: de `Patches` a `DefInjected` | Better Architect Menu, Progression Scenarios, VAE Royal Animals, Buffs 'n Nerfs, Insectoids 2, Zoology, ReGrowth 2, ReSplice Core, VFE - Tribals | Mirar cada uno y commitear por mod. ReGrowth, ReSplice y Tribals no están en la lista del log de «salen como DefInjected»: averiguar por qué cambiaron |
| Vuelven del `UNUSED.xml` y comps por su clase | VFE - Props and Decor, Makeshift, VWE - Makeshift, Apex, GravTech, Mechanoid Upgrades, Custom Weapons Expanded y otros | Lo esperado: commitear por mod |
| **Condición de Combat Extended por nombre** | Cinders, Ascension Megacorp, Caravan Adventures, Custom Weapons Expanded, Zoology | **Decidido por Angel el 31-oct: por nombre** (funciona con el CE oficial y con el no oficial). Commitear por mod |
| Reglas que se van en 12 `RulePackDef` | Vanilla Social Interactions Expanded (5 líneas en cada uno) | Mirar antes de commitear: pueden ser las reglas de `[commentedabout]` del paso 3 de la revisión general (ya borrado; está en git) |
| Otros | Hospitality (5 líneas menos en `InteractionDef`, y un `UNUSED.xml` nuevo), Nice Inventory Tab CE Integration (`Keyed/Formats.xml` pasa a `wmamct.xml`), Customize Weapon, Altered Carbon 2 | Mirar uno por uno |

**No son de esta tarea:** `[KCB] Mini Expeditions` (la carpeta la creó Angel con el extractor a las 00:56) y
`Graphics Settings - 1678847247` (creada a las 01:30, durante la corrida), con sus líneas en `About.xml`,
`LoadFolders.xml`, `ModList.tsv` y `docs/`. Preguntarle a Angel antes de commitearlas.

## Lo que queda por traducir (contado el 31-oct con `traducir.py estado`)

| Mod | `TODO` | Palabras | Nota |
|---|---|---|---|
| [KCB] Mini Expeditions | 1.023 | 8.617 | Nuevo, entero. Varias sesiones: plan y glosario propios |
| Buffs 'n Nerfs | 364 | 1.821 | Tenía 0 el 24-oct: ver si es una actualización del mod o un efecto del arreglo (`Patches/17q4kw.xml` nuevo) |
| Graphics Settings | 128 | 577 | Nuevo |
| VFE - Props and Decor | 73 | 130 | |
| RimIris Beta | 24 | 448 | |
| Ascension Megacorp, DMS AncientCorps, Cinders | 20, 22 y 8 | | Algunos quedan en `TODO` a propósito: ver sus planes |
| Otros 6 mods | 1 o 2 cada uno | | |

Los 5.086 de Combat Extended son de mods que RML no traduce y quedan así (decisión de Angel del 19-oct).

## Próxima sesión (31-oct-2026)

**Objetivo:** commitear la reextracción del 31-oct, mod por mod, y empezar las traducciones nuevas.
**Estado:** `master` de RML y del extractor subidos el 31-oct (RML `f93af3e9`, extractor `4416c40`). La
reextracción está sin commitear en la carpeta.
**Antes de empezarla (Angel):** no cierres ni vuelvas a extraer nada hasta que esto se commitee: una corrida nueva
pisaría lo que está sin revisar.
**Al empezar (Claude):**
1. Los grupos de la tabla, un commit por mod, con las tres comprobaciones del diff. Cabe en una sesión.
2. Preguntarle a Angel por Mini Expeditions y Graphics Settings, y commitearlas si dice que sí.
3. Los `TODO` chicos (Props and Decor, RimIris y los de 1 o 2) con la tabla de `traducir.py`. Cabe en la misma
   sesión si sobra tiempo.
4. Buffs 'n Nerfs y Graphics Settings, en otra sesión. Mini Expeditions, con su propio plan, en varias.
**Decisiones de Angel:** ninguna pendiente para el paso 1.
**Guías:** ninguna en especial; «Antes de commitear: mirar el diff» y «Editar XML de traducción» de `RML/AGENTS.md`.
**Dónde leer:** este plan; `Data/Combat Extended - 2890901044/PLAN-CE-AJUSTES.md` (punto 1).
