# Plan: Caravan Adventures

Traducir Caravan Adventures (Workshop 2558957509) entero. Lo pidió Angel el 22-oct-2026, con el mod
ya extraído y sin nada traducido.

## Contexto

- **Extracción nueva:** la carpeta no estaba en RML. La trajo la traducción rápida del extractor, que
  también regeneró el índice (`About.xml`, `LoadFolders.xml`, `ModList.tsv`, `docs/`).
- **Sin `UNUSED.xml`**: no había traducción anterior que conservar.
- **La historia del mod es larga**: un árbol ánima elige a un colono, le da el «don ancestral» y lo
  manda a recorrer santuarios mecanoides junto a la facción Cazadores Sacrílegos. Casi todo el texto
  son diálogos y cartas de esa historia, en `Keyed`.
- Los términos están en [GLOSARIO-CARAVAN.md](GLOSARIO-CARAVAN.md). Lo que el mod copia de vanilla
  usa el texto oficial, sacado de los `.tar` de idioma del juego.

## Cifras (contadas el 22-oct-2026)

| Qué | Cuántas |
|---|---|
| Pendientes (`TODO`), por renglón | 571 |
| En `Keyed` (diálogos, cartas, botones) | 275 |
| En `DefInjected` | 277, en 27 archivos |
| En `Patches` (etiquetas de ataque con Combat Extended) | 19 |

Por tipo de `DefInjected`: `ThingDef` 85, `BodyDef` 30, `HediffDef` 26, `RecipeDef` 21,
`ThoughtDef` 16, `PawnKindDef` 15, `AbilityDef` 12, `PawnRelationDef` 12, `WorkGiverDef` 12,
`WorldObjectDef` 10, `QuestScriptDef` 8, y 30 más repartidas en 9 tipos.

## Etapas

| # | Etapa | Tamaño | Estado |
|---|---|---|---|
| 1 | Glosario del mod | chica | hecha: [GLOSARIO-CARAVAN.md](GLOSARIO-CARAVAN.md) |
| 2 | Traducir `DefInjected` y `Patches` | mediana | hecha (22-oct-2026) |
| 3 | Traducir `Keyed` | grande | hecha (22-oct-2026) |
| 4 | Verificar | chica | hecha: ver abajo |
| 5 | Confirmar términos y probar en partida | chica | falta: Angel |

### 2 y 3. Traducir

A mano, archivo por archivo, reemplazando solo los `TODO`. Sin scripts.

Decisiones que vale la pena saber:

- **Los nombres de misión `questName->Temp` y `questDescription->Temp` quedan igual.** El mod los
  reemplaza por código con los textos de `Keyed` (`Story_Shrine1_QuestName` y parecidos); «Temp» no
  se muestra nunca.
- **Las descripciones en binario de los jefes quedan en binario.** Dicen «Seeking human
  annihilation... Processing...», y el chiste es que no se lee. Se dejan como están.
- **`{0, -10}` y `{1, -20}`** en `CABountyKillMessage` son formato de alineación de C#: no se tocan.
- Cuando el inglés usa un marcador solo para el posesivo («{1} eyes», «{1} duty»), se traduce con
  «su» y el marcador queda afuera. El juego no falla por un argumento que no se usa.
- **Sin suponer el género** de quien habla o del colono del jugador cuando el texto es un marcador:
  «[A {0} se le nota la decepción]» en vez de «decepcionada», «alma elegida» en vez de «elegido».
  La damisela sí va en femenino, porque el propio inglés la fija.

### 4. Verificar

Hecho el 22-oct-2026, todo sin problemas:

- Comparación con la copia de antes: cambiaron los 571 renglones que decían `TODO` y ninguno más,
  sin tocar etiquetas, comentarios, codificación ni saltos de línea. Los 29 XML se leen bien.
- Los marcadores `{0}`, `{1}`… están todos, salvo 7 que el inglés usaba solo para el posesivo
  («{1} eyes» → «sus ojos»): `Story_Start_Dia1_1_Neg_Option2`, `CA_Story_ReceivedGiftLetterDesc`,
  `TradeCaravan_Dia1_2`, `_Dia1_4`, `_Dia1_5`, `_Dia1_6_1` y `StoryVillage_QuestUpdate_MechsArrived`.
- Las tres comprobaciones del diff de `RML/AGENTS.md`: vacías.
- `LoadFoldersBuilder -rutas`: sale con 0.

Cuidado al editar `Keyed/mxv8gz.xml`: el comentario de `Story_Shrine1_SacrilegHunters_Dia1_1` ocupa
cuatro renglones con saltos de línea mixtos, y la herramienta de edición los pasa todos a CRLF. Se
devolvieron a como estaban.

### Lo que no se puede traducir

La ventana «Caravan Adventures Setup», la que aparece al empezar la historia (Normal / Performance,
los botones de GitHub y «Disable this window…»), tiene sus textos escritos directo en
`CaravanAdventures.dll`, sin pasar por `Keyed`. Comprobado el 22-oct-2026 buscando las frases en
todos los archivos del mod: solo están en la DLL. Desde RML no se pueden cambiar, porque RML no
modifica código. Lo más probable es que pase lo mismo con las opciones del mod. El arreglo es que el
autor los mueva a `Keyed`.

### 5. Pendiente de Angel

1. Confirmar los términos marcados **a confirmar** en el glosario: el nombre de la facción y el de la
   condición del apocalipsis.
2. Probar en partida: abrir el árbol ánima para ver el primer diálogo, y la consola de comunicaciones
   para ver los Servicios de recompensas.
3. Decidir el commit y el push: nada de esto está commiteado.
