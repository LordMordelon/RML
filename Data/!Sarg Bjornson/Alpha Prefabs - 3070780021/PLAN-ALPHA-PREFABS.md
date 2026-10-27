# Plan de Alpha Prefabs

Alpha Prefabs (3070780021, de Sarg Bjornson) vende construcciones enteras por plata desde un
catálogo. Extraído el 25-oct-2026: 902 textos, unas 9.000 palabras. Casi todo son los nombres y
descripciones de los prefabricados (`AlphaPrefabs.PrefabDef`, 729 textos, 125 de ellos nombres de
variantes como «Facing West»).

Los términos están en [GLOSARIO-ALPHA-PREFABS.md](GLOSARIO-ALPHA-PREFABS.md).

## Decisiones de Angel

- **25-oct-2026: tabla + script**, como en Hegemony y por el tamaño. Cada traducción se escribe a
  mano en una tabla clave → texto, y un script la copia solo sobre los `TODO` (desde el 26-oct-2026,
  `Herramientas/traducir.py aplicar`, que además revisa). Después la revisión compara con la copia de la extracción: mismos bytes fuera de los
  `TODO` y los mismos marcadores (`{0}`, `\n`, colores) en cada traducción.
- Los 19 textos de `Patches/` se editan a mano: todos tienen la misma clave (`label`) y la tabla no
  los distingue.

## Criterios

- Los nombres de prefabricados van en minúscula, como las etiquetas del juego.
- Las descripciones nombran los muebles y edificios como los llama el juego o la traducción de su
  mod en RML. Si el mod que los trae no está traducido, se traduce el nombre de forma literal.
- Los nombres de mods (Rimatomics, Dub's Bad Hygiene, VFE - Architect) no se traducen.
- Las bromas del autor se adaptan (Monty Python, Los Sims), sin perder la referencia.

## Estilo (pedido por Angel el 25-oct-2026)

- **Descripciones al estilo del juego**: frases que fluyen, «además de» o «y» en vez de «más»,
  números chicos con letras, adjetivo delante cuando suena natural («un pequeño establo»). El tamaño
  de los cuartos va como «su espacio es apretado / ajustado / espacioso», con las palabras del juego.
- **Títulos en amarillo: descartado.** Se probó pintar los títulos de la ficha («Investigación
  necesaria:», etc.) con `#E5E54C`, pero el recuadro «Nombre: descripción» del catálogo lo arma la
  DLL con el nombre del prefabricado y no se puede pintar sin meter el color en el nombre. Angel
  desistió de hacerlo por traducción (25-oct-2026) y se sacaron los colores.
- **Espacio antes de «Mods necesarios»**: error de la DLL (`Window_Prefab`), que agrega un renglón
  con un espacio y después pega ese texto sin salto. `AP_ModsNeeded` empieza con `\n` para que quede
  en su renglón. Con el modo sin bloqueo por investigación queda un renglón vacío arriba.

## Para confirmar con Angel

1. **killbox → «zona de muerte»**. Otras opciones: «embudo de muerte», o dejar «killbox».
2. **pillbox → «fortín»**.
3. **undeploy → «replegar»** (el botón que vuelve a guardar el constructor en una caja).
4. Los salones del trono usan los títulos oficiales: «count» es **arconte** y «duke» es
   **dominus**. Los títulos de VFE - Empire siguen su traducción en RML: archiconde, marqués,
   archiduque, magister y déspota.

## Estado

- [x] Glosario y plan.
- [x] Tabla de las claves de `DefInjected` y `Keyed` (883 textos), aplicada y revisada: 0 problemas
  y 0 `TODO`.
- [x] `Patches/` a mano (19 textos).
- [x] Las tres comprobaciones del diff de `RML/AGENTS.md`: vacías.
- [x] Probando en el juego, Angel vio dos campos en inglés que el extractor no sacaba: `shortLabel`
  (el nombre de la cuadrícula del catálogo) y `labelForDefaultVariation` (la primera opción de
  «Pedir ahora con...»). Se sumaron al extractor, se reextrajo y se tradujeron los 361 textos
  nuevos. Los cortos van abreviados («lab.», «fáb.», «peq.») porque la cuadrícula es angosta.
- [ ] Prueba en el juego: abrir el catálogo y mirar que los textos largos entren.
