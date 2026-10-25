# Plan: RimIris Beta

Traducir RimIris Beta (Workshop 3796735623, packageId `RimIris.RimIris.Beta`), un mod de gráficos
(brillo, rayos de sol, profundidad de campo, agua, suelo mojado, noches). Lo pidió Angel el
24-oct-2026.

## Cifras (contadas el 24-oct-2026)

| Archivo | Textos | Palabras en inglés |
|---|---|---|
| `Keyed/1llu53.xml` (el panel de ajustes) | 594 | unas 5.300 |
| `DefInjected/MainButtonDef/61fhsl.xml` (el botón de la barra) | 2 | 15 |

## Estado (24-oct-2026)

Traducido entero en una sesión, con tabla y script (Angel lo aprobó el 24-oct): cada traducción se
escribió a mano en la tabla, `aplicar.py` la copió solo sobre los `TODO` y `revisar.py` no marcó
nada. Sin ningún `TODO`. Índice regenerado y commit «RimIris: traducción completa», sin push.

**Falta:** que Angel lo pruebe en el juego: abrir el botón de Iris (barra de abajo), recorrer las
secciones y la página «Iluminación por escena». Si algo sigue en inglés con todo traducido, está
escrito dentro de la DLL y no se arregla desde RML.

## Términos que se eligieron

No hay términos de gráficos en la traducción oficial salvo los de abajo; el resto son los usuales en
los menús de gráficos en español.

| Inglés | Español | De dónde |
|---|---|---|
| hue / saturation | matiz / saturación | oficial |
| cold snap / heat wave | frente frío / ola de calor | oficial |
| developer mode | modo desarrollador | oficial |
| bloom (light / ground) | resplandor (de luces / del suelo) | |
| sun glow | halo solar (el efecto); brillo del sol (el valor del juego) | |
| god rays | rayos de sol | |
| vignette, tone mapping, exposure | viñeta, mapeo de tonos, exposición | |
| depth of field, bokeh | profundidad de campo, bokeh | |
| post-processing | posprocesado | |
| shader pack / preset / add-on | paquete de shaders / preajuste / complemento | |
| built-in | integrado | |
| jelly water | agua gelatinosa | |
| tide run-up | avance de la marea | |
| scenario (rules, lighting, branch) | escena (reglas de escena, iluminación por escena, rama de escena) | «escenario» se confunde con los del juego |
| picture | imagen | |
| uniform, ACES, HDR, GPU, CPU | sin traducir | términos técnicos |

Los cuatro valores con espacio al principio o al final (`StatusAddons`, `OriginTag`, `PresetTag`,
`ProfileTag`) lo llevan como `&#32;`, porque `aplicar.py` recorta los espacios.
