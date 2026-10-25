# Plan: ReGrowth 2

Poner al día y revisar la traducción de ReGrowth 2 (Workshop 2260097569). Lo pidió Angel el
25-oct-2026, con el mod ya reextraído.

## Contexto

- **Ya estaba traducido entero:** la reextracción del 25-oct-2026 no dejó ningún `TODO`. La traducción
  viene de antes de este repositorio (el commit `2acfc028`, «correccion») y la puso al día `64f45576`
  el 21-oct, con el bioma nuevo bosque de álamos temblones.
- **Lo que trajo la reextracción:** el autor corrigió el inglés de 27 textos (erratas, plurales y
  copias mal pegadas). Las traducciones quedaron como estaban, así que hay que mirar si siguen
  correspondiendo.
- **Tiene `UNUSED.xml`** con traducciones apartadas. No se toca (regla 3 de `RML/AGENTS.md`).
- Los términos están en [GLOSARIO-REGROWTH.md](GLOSARIO-REGROWTH.md).

## Cifras (contadas el 25-oct-2026)

| Qué | Cuántas |
|---|---|
| Entradas en total | 489, unas 4.800 palabras en inglés |
| En `Keyed` (opciones de los módulos del mod) | 134 |
| En `DefInjected` | 354, en 20 archivos. `ThingDef` tiene 216 |
| En `Patches` (el códice de Anomaly) | 1 |
| Textos con el inglés corregido por el autor | 27 |

## Etapas

| # | Etapa | Tamaño | Estado |
|---|---|---|---|
| 1 | Glosario del mod | chica | hecha: [GLOSARIO-REGROWTH.md](GLOSARIO-REGROWTH.md) |
| 2 | Arreglar los textos cuyo inglés cambió | chica | hecha (25-oct-2026) |
| 3 | Angel confirma los términos | chica | falta: Angel |
| 4 | Aplicar los términos confirmados | chica | falta |
| 5 | Revisar el resto contra el texto oficial | mediana | falta |
| 6 | Verificar y probar en partida | chica | falta |

### 2. Los textos cuyo inglés cambió

De los 27, 21 eran solo erratas del inglés (*reasonbly*, *leafs*, *it's*) y la traducción ya estaba
bien. Se arreglaron los que no correspondían:

- `RG_Plant_Nightguide.description`: el inglés se reescribió entero. Antes hablaba de guiar a los
  viajeros por el bosque oscuro; ahora dice que es una planta resistente de los lumibosques.
- `RG_Plant_TropicalIvy.description` y `RG_Plant_TemperateIvy.description`: el inglés ahora dice
  «warm areas» y «tropical areas» en vez de «areas of … temperatures».
- `RG_NeoAmanita.description`: «que crece en racimos» quedaba repetido en la misma oración.
- `SmartFarming.Icon.Smart.Desc`: el inglés pasó a plural («they are expected to die»).
- `ScatteredFlames.Settings.IgniteGizmo.Desc`: «botón de gizmo» queda para la etapa 4, con el resto
  de los «gizmo».

### 3. Términos para confirmar

Cada uno cambia varios textos que ya estaban traducidos, por eso no se hizo sin preguntar:

1. **rimclaw → «iguanodonte».** El nombre es de un dinosaurio real, y el animal es una iguana
   mutada. Opciones: dejarlo; «iguana del borde»; o dejar «rimclaw» como el juego deja «thrumbo».
2. **chokevine.** El juego lo deja en inglés; el mod dice «enredadera estranguladora».
3. **gizmo → «botón».** 12 textos de las opciones.
4. **ancient → «ancestral»**, como el juego: «jeep ancestral», «poste eléctrico ancestral».
5. **Pensamientos en tercera persona**, como el juego: «se bañó en aguas termales» en vez de «me
   bañé…», y «mojad{PAWN_gender ? o : a}» / «empapad{PAWN_gender ? o : a}» en vez de «húmedo»
   y «extremadamente húmedo», que no tienen género. «sandy» → «arenoso», como el oficial, en vez
   de «arena».
6. **«Predeterminado» → «Por defecto»** en las opciones.
7. **Zarzales y semilla de ánima:** «zarzales rojos», «zarzales amarillos», «zarzales tropicales»
   como el oficial, y «semilla de ánima».

### 5. Revisar el resto

Lo que el mod copia del juego va con el texto oficial. La comparación automática encontró la
descripción de los zarzales y la del arbusto de bayas, que el mod copia tal cual del inglés del
juego y en RML están traducidas de otra forma. Después, una lectura del resto archivo por archivo,
con la regla de siempre: solo el valor, sin tocar etiquetas, comentarios ni codificación.

### 6. Verificar

Las tres comprobaciones del diff de `RML/AGENTS.md`. La primera va a mostrar cada valor cambiado:
tienen que ser solo los de este plan. Después, probar en partida un mapa del bosque de álamos
temblones y abrir las opciones del mod.
