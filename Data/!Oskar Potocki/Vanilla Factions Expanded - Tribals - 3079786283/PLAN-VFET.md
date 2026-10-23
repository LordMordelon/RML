# Plan: Vanilla Factions Expanded - Tribals

Traducir VFE - Tribals (Workshop 3079786283) entero. Lo pidió Angel el 23-oct-2026, con el mod ya
extraído y sin nada traducido.

## Contexto

- **Extracción nueva:** la carpeta no estaba en RML. La trajo la traducción rápida del extractor, que
  también regeneró el índice (`About.xml`, `LoadFolders.xml`, `ModList.tsv`, `docs/`).
- **Sin `UNUSED.xml`**: no había traducción anterior que conservar.
- **De qué trata:** se empieza como hombres salvajes sin saber hacer casi nada. Las reuniones tribales
  reparten investigación, y cada vez que se completa un nivel tecnológico se avanza con un ritual y se
  elige una piedra angular, una bonificación permanente para toda la colonia.
- Los términos están en [GLOSARIO-VFET.md](GLOSARIO-VFET.md). Lo que el mod copia de vanilla usa el
  texto oficial, sacado de los `.tar` de idioma del juego.

## Cifras (contadas el 23-oct-2026)

| Qué | Cuántas |
|---|---|
| Pendientes (`TODO`), por renglón | 538 |
| En `Keyed` (interfaz de piedras angulares, cartas, avisos) | 43 |
| En `DefInjected` | 494, en 39 archivos |
| En `Patches` (el nombre del `PartySpot`) | 1 |

Lo más grande de `DefInjected`: las reglas de la descripción de la pintura de suelo (`RulePackDef`, 142),
las piedras angulares (`CornerstoneDef`, 88), los proyectos de investigación con sus reglas
(`TribalResearchProjectDef`, 91), los rituales (`RitualBehaviorDef` 24, `RitualOutcomeEffectDef` 23) y
los objetos (`ThingDef`, 29).

## Etapas

| # | Etapa | Tamaño | Estado |
|---|---|---|---|
| 1 | Glosario del mod | chica | hecha: [GLOSARIO-VFET.md](GLOSARIO-VFET.md) |
| 2 | Traducir `DefInjected` y `Patches` | mediana | hecha (23-oct-2026) |
| 3 | Traducir `Keyed` | chica | hecha (23-oct-2026) |
| 4 | Verificar | chica | hecha: ver abajo |
| 5 | Confirmar términos y probar en partida | chica | falta: Angel |

### 2 y 3. Traducir

A mano, archivo por archivo, reemplazando solo los `TODO`. Sin scripts.

Decisiones que vale la pena saber:

- **Las frases que el mod copia de vanilla van con el texto oficial**: las de *commentedabout* de la charla,
  las tres frases de contexto de la pintura de suelo, «cantidad de participantes», «comenzó en una
  fogata», «Participantes» / «participa», «opcional» y «Cualquier punto de encuentro, como una mesa».
- **Una regla de arte cambió de símbolo**: `[Quantity_adjphrase] [subimagesingle]s` (el inglés pluraliza
  con una «s») quedó como `[Quantity_adjphrase] [subimageplural]`, que en español sí concuerda. Las demás
  decisiones de concordancia están en «Reglas de gramática» del glosario.
- **«¡Descubrimiento: {0}!»** y no «¡{0} descubierto!»: el {0} es un proyecto de investigación y puede ser
  femenino («agricultura»).
- **El {0} de los resultados de la reunión tribal es el nombre del ritual**, femenino: «¡La {0} fue
  ineficaz!».
- **Los saltos de línea reales** de algunos textos en inglés (el ethos, la ideoligión, el escenario) van
  como `\n\n`, que el juego muestra igual.

### 4. Verificar

Hecho el 23-oct-2026, todo sin problemas:

- Comparación con la copia de antes: cambiaron los 538 renglones que decían `TODO` y ninguno más, sin
  tocar etiquetas, comentarios ni codificación. Los 41 XML se leen bien.
- Los marcadores (`{0}`, `[asker_nameFull]`, `[DATE]`…) y los prefijos `símbolo->` coinciden con el inglés
  en cada valor, salvo el cambio de `[subimagesingle]` a `[subimageplural]`, que es a propósito.
- Las tres comprobaciones del diff de `RML/AGENTS.md`: vacías. La 3 se corrió también sobre esta carpeta,
  que todavía no está en git.
- `LoadFoldersBuilder -rutas`: sale con 0.

Cuidado al editar: los XML usan CRLF, salvo dentro de los comentarios de varios renglones, que usan LF. La
herramienta de edición pasa esos 12 saltos a CRLF; se devolvieron a como estaban.

### Lo que no se ve aunque esté traducido

**El nombre del `PartySpot`.** El mod lo cambia por *gathering spot* con un `Patches`, pero el juego
aplica los `Patches` antes que las traducciones oficiales, y la oficial lo llama «lugar de fiestas». En
pantalla queda «lugar de fiestas» (ver «Un Patches no le puede ganar a la traducción oficial» en
`RimworldExtractor/AGENTS.md`). El extractor no lo pasó a `DefInjected` porque la operación va dentro de
un `PatchOperationFindMod`, que trata como condicional. Se tradujo igual, como «punto de encuentro».

### 5. Pendiente de Angel

1. Confirmar el término marcado **a confirmar** en el glosario: «piedra angular» por *cornerstone*.
2. Probar en partida con el escenario «Hombres Salvajes»: la pestaña de investigación Fundamentos, una
   reunión tribal y la ventana de piedras angulares.
3. Decidir el commit y el push: nada de esto está commiteado.
