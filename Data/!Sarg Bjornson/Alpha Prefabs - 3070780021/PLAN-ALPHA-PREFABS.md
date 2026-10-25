# Plan de Alpha Prefabs

Alpha Prefabs (3070780021, de Sarg Bjornson) vende construcciones enteras por plata desde un
catálogo. Extraído el 25-oct-2026: 902 textos, unas 9.000 palabras. Casi todo son los nombres y
descripciones de los prefabricados (`AlphaPrefabs.PrefabDef`, 729 textos, 125 de ellos nombres de
variantes como «Facing West»).

Los términos están en [GLOSARIO-ALPHA-PREFABS.md](GLOSARIO-ALPHA-PREFABS.md).

## Decisiones de Angel

- **25-oct-2026: tabla + script**, como en Hegemony y por el tamaño. Cada traducción se escribe a
  mano en una tabla clave → texto, y `herramientas-traduccion/aplicar.py` la copia solo sobre los
  `TODO`. Después `revisar.py` compara con la copia de la extracción: mismos bytes fuera de los
  `TODO` y los mismos marcadores (`{0}`, `\n`, colores) en cada traducción.
- Los 19 textos de `Patches/` se editan a mano: todos tienen la misma clave (`label`) y la tabla no
  los distingue.

## Criterios

- Los nombres de prefabricados van en minúscula, como las etiquetas del juego.
- Las descripciones nombran los muebles y edificios como los llama el juego o la traducción de su
  mod en RML. Si el mod que los trae no está traducido, se traduce el nombre de forma literal.
- Los nombres de mods (Rimatomics, Dub's Bad Hygiene, VFE - Architect) no se traducen.
- Las bromas del autor se adaptan (Monty Python, Los Sims), sin perder la referencia.

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
- [ ] Prueba en el juego: abrir el catálogo y mirar que los textos largos entren.
