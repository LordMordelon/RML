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
| 3 | Angel confirma los términos | chica | hecha (25-oct-2026) |
| 4 | Aplicar los términos confirmados | chica | hecha (25-oct-2026) |
| 5 | Revisar el resto contra el texto oficial | mediana | hecha (25-oct-2026) |
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
- `ScatteredFlames.Settings.IgniteGizmo.Desc`: «botón de gizmo» se arregló en la etapa 4, con el
  resto de los «gizmo».

### 3 y 4. Términos

Angel los decidió el 25-oct-2026; cada uno está en el glosario:

1. **rimclaw** queda «iguanodonte».
2. **chokevine** queda «enredadera estranguladora», en español, aunque el juego lo deja en inglés.
3. **gizmo → «botón»**, en las 6 opciones que lo nombraban.
4. **ancient** queda «antiguo» («jeep antiguo», «poste eléctrico antiguo»). Angel dejó la elección:
   el juego usa las dos palabras, y «antiguo» es la de lo más parecido («antiguo transporte de
   tropas», «farola antigua»).
5. **Pensamientos:** los baños siguen en primera persona («me bañé en aguas termales»), como pidió
   Angel. «húmedo» y «extremadamente húmedo» pasan a «mojad{PAWN_gender ? o : a}» y
   «empapad{PAWN_gender ? o : a}», y «arena» a «llen{PAWN_gender ? o : a} de arena»: el «arenoso»
   oficial es de un terreno del mapa, no de un pensamiento.
6. **«Predeterminado» → «Por defecto»**, en 9 opciones.
7. **«zarzales rojos», «zarzales amarillos», «zarzales tropicales»** y **«semilla de ánima»**.

### 5. Revisar el resto

Lo que el mod copia del juego va con el texto oficial: la descripción de los tres zarzales del mod
(la misma del zarzal del juego) y la del arbusto de bayas. Las patas del estegosaurio siguen como
estaban: «pata delantera» es lo que usa el juego en casi todos sus animales.

La lectura del resto encontró, y se arregló:

- `RG_Hemolope.description`: «sanguívoros» → «sanguífagos», el término oficial, y «puede ser
  tratado» → «se puede ordeñar».
- `RG_WasteBathing.stages.0.description`: «Qué guarrería...» es de España; ahora «Un asco total;
  siento la suciedad en todo el cuerpo».
- `RG_Bathe.reportString`: «relajarse» → «relajándose», como los demás trabajos del juego.
- En las opciones: «vainilla» → «Vanilla» (2), «cultivos apestados» → «plagados», y las comidas con
  sus nombres oficiales («sencilla», «lujosa»).

### 6. Verificar

Las tres comprobaciones del diff de `RML/AGENTS.md`. La primera va a mostrar cada valor cambiado:
tienen que ser solo los de este plan. Después, probar en partida un mapa del bosque de álamos
temblones y abrir las opciones del mod.
