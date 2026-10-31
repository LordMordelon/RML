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
| **Extractor: no recortar listas de reglas más largas que el inglés** | Antes de la próxima reextracción. Tarea del extractor, con prueba que falle primero. Hasta entonces, la comprobación 5 lo avisa |
| VFE - Props and Decor | 73 `TODO`, 130 palabras: los `shortLabel` nuevos del catálogo |
| RimIris Beta | 24 `TODO`, 448 palabras |
| Ascension Megacorp | 5 descripciones nuevas de munición de CE (los otros 15 `TODO` son a propósito: ver su plan) |
| Buffs 'n Nerfs | 364 `TODO`. El mod no se actualizó: son patches que el extractor antes no traía. La descripción de `PlantBlazebulb` va en un `FindMod` «Ideology» (el mismo DLC dueño) y salió en `Patches`, no en `DefInjected`: averiguar por qué antes de traducirla, porque así el juego la pisa con la oficial |
| ReSplice Core | Mezcla «pack de genes» (4, el oficial) con «paquete genético» (13): unificar al oficial |
| Graphics Settings y Mini Expeditions | Fuera de `master` hasta que Angel diga. Mini Expeditions necesita plan y glosario propios |

## Próxima sesión

**Objetivo:** el arreglo del extractor para las listas de reglas, y después los `TODO` chicos (Props and Decor,
RimIris, Ascension).
**Estado:** todo subido a `master` de RML el 31-oct (`3ce5a505`). El extractor sin cambios.
**Antes de empezarla (Angel):** no reextraer nada hasta que esté el arreglo del extractor.
**Al empezar (Claude):**
1. En `RimworldExtractor`: prueba que falle con una lista de `rulesStrings` más larga que la del inglés, y el
   arreglo para que se conserve entera. Probarlo reextrayendo Insectoids 2, VSIE y Hospitality, con
   `traducir.py comprobar` vacío.
2. Los `TODO` de Props and Decor, RimIris y Ascension con la tabla, y `comprobar` antes de cada commit.
**Decisiones de Angel:** ninguna pendiente.
**Guías:** «Antes de commitear: mirar el diff» (ahora con cinco comprobaciones) y «Editar XML de traducción» de
`RML/AGENTS.md`.
**Dónde leer:** este plan.
