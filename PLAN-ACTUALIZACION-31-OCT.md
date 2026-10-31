# Plan: revisar la reextracción completa del 31-oct-2026 y traducir lo nuevo

El 31-oct-2026 se reextrajo todo RML con `RimworldExtractorLotes` (287 carpetas), después del arreglo del
extractor `4416c40`: un patch con la condición del mismo DLC dueño del def ahora sale como `DefInjected`
(Progression Robotics y Biotech). La corrida trajo además todos los cambios del extractor desde la última
extracción de cada mod.

Lo pidió Angel el 31-oct: revisar, corregir y traducir las actualizaciones y el contenido nuevo. Se borra al
terminar, como los otros planes.

## Hecho (31-oct, subido)

- **La reextracción, commiteada mod por mod** (`77bdf65f` … `2c13d8fa`), con las comprobaciones del diff.
  Los archivos que solo cambiaban en saltos de línea no dejaron commit.
- **Tres mods perdían reglas y se devolvieron a `master`** (no se commiteó el recorte): el extractor recortó
  al largo del inglés las listas de reglas con variantes de género agregadas a mano y las apartó al
  `UNUSED.xml` — Insectoids 2 (`[insectoid_masc]`), Vanilla Social Interactions Expanded (`[commentedabout]`,
  12 `RulePackDef`) y Hospitality (`[adjwords_m]`). Lo recortado quedó respaldado en la carpeta temporal de
  la sesión. **Vuelve a pasar en la próxima reextracción** hasta que se arregle el extractor (ver abajo).
- **Inglés cambiado con el español viejo** (lo encontró Angel en RimIris): corregidos RimIris (2), DMS
  AncientCorps (8: muros, puerta, misil), Altered Carbon 2 (sleeveliner), ReSplice (centrífuga) y ReBuild
  (muro ornamentado). Los otros ~200 desde el 17-oct son erratas del inglés sin cambio de sentido.
- **`traducir.py comprobar` tiene dos comprobaciones más** (`6b3b2c62`, `AGENTS.md`): la 4, español viejo donde
  cambió el inglés, y la 5, textos que desaparecen del mod. Y la orden `ingles --desde <commit>`.
- Traducidos los `TODO` de Better Architect Menu, Insectoids 2, VGE Chapter 2, Customize Weapon y Search and
  Rescue.
- **Mini Expeditions y Graphics Settings quedan fuera de `master`** (Angel, 31-oct): las carpetas siguen en
  `Data/` sin commitear y sin línea en el índice, así que el juego no las carga.

## Pendiente

| Qué | Nota |
|---|---|
| ~~Extractor: no recortar listas de reglas más largas que el inglés~~ | Hecho el 31-oct en el extractor (`ReglasAgregadasTests`), sin subir. Reextraídos Insectoids 2, VSIE y Hospitality: sin cambios y `comprobar` vacío. Las 91 del `UNUSED.xml` de VSIE ya estaban en `master` |
| ~~VFE - Props and Decor, RimIris Beta, Ascension Megacorp~~ | Traducidos el 31-oct (`b7eba52c`, `3e22355d`, `eff8c022`), sin subir. En Ascension quedan los 15 `TODO` a propósito. Props and Decor dice «antiguo» en todo el mod donde la oficial dice «ancestral» (*ancient*), y algunos nombres completos no siguen a la oficial («gestor de mecas», «monitor de constantes vitales»): los cortos nuevos van con la oficial. Queda para la revisión general |
| Buffs 'n Nerfs | 364 `TODO`. El mod no se actualizó: son patches que el extractor antes no traía. La descripción de `PlantBlazebulb` va en un `FindMod` «Ideology» (el mismo DLC dueño) y salió en `Patches`, no en `DefInjected`: averiguar por qué antes de traducirla, porque así el juego la pisa con la oficial |
| ReSplice Core | Mezcla «pack de genes» (4, el oficial) con «paquete genético» (13): unificar al oficial |
| Graphics Settings y Mini Expeditions | Fuera de `master` hasta que Angel diga. Mini Expeditions necesita plan y glosario propios |

## Próxima sesión

**Objetivo:** ReSplice Core («pack de genes») y después Buffs 'n Nerfs, empezando por averiguar lo de
`PlantBlazebulb`.
**Estado:** el arreglo del extractor (`3fb3b98`) y las tres traducciones están commiteados el 31-oct, **sin
subir**: falta el visto bueno de Angel para el push de los dos repos.
**Al empezar (Claude):**
1. ReSplice Core: pasar los 13 «paquete genético» a «pack de genes», el oficial, y su glosario si lo nombra.
2. Buffs 'n Nerfs: por qué la descripción de `PlantBlazebulb` sale en `Patches`; después los `TODO` con la tabla.
**Decisiones de Angel:** el push; «antiguo» o «ancestral» en Props and Decor (ver arriba).
**Guías:** «Antes de commitear: mirar el diff» (ahora con cinco comprobaciones) y «Editar XML de traducción» de
`RML/AGENTS.md`.
**Dónde leer:** este plan.
