# Plan: Hegemony - Faction Territories and Vassalage 2

Traducir Hegemony (Workshop 3825320105, packageId `jaeger972.factionterritories2`). Lo pidió Angel el
24-oct-2026. La carpeta ya está extraída y sin commitear: es nueva, así que no hay nada traducido que
cuidar ni `UNUSED.xml`.

## Contexto

- **La ventana del mod no se puede traducir desde RML.** El mod no trae `Languages/`: los textos de la
  pestaña Vasallaje (Overview, People, Buildings, Deliveries, los botones y los avisos) están escritos
  dentro de su DLL, unas 9.000 cadenas. Pasa lo mismo que con los nombres de configuración de Vanilla
  Expanded: solo se podrían cambiar con Harmony, y eso queda fuera de este plan.
- **Lo que sí se traduce son sus defs:** el nombre de la pestaña, los edificios, las disposiciones de
  facción, los alcaldes, los efectos de bioma y de sitio, las cartas y, sobre todo, las 465 lecciones
  del ayudante de aprendizaje (`ConceptDef`), que explican cómo se juega.
- **Una sola carpeta de versión (`Common/`)**, que carga en 1.6 y en 1.7. Sus tres archivos de
  `Compatibility/` (AlphaMemes, Vanilla Ideology y Vanilla Traits Expanded) y el `Patches` de
  Mechanoid Crisis no dejaron nada que traducir.

## Cifras (contadas el 24-oct-2026)

| Archivo | Pendientes | Palabras en inglés |
|---|---|---|
| `ConceptDef` (lecciones: título y texto) | 930 | 26.742 |
| `FactionDispositionDef` (disposiciones de facción) | 406 | 3.699 |
| `BuildingDef` (edificios del asentamiento) | 250 | 3.180 |
| `MayorTraitEffectDef` (efectos de los rasgos del alcalde) | 135 | 1.181 |
| `SiteEffectDef` | 80 | 525 |
| `BiomeEffectDef` | 44 | 497 |
| `MayorRoleDef` (especialidades del alcalde) | 24 | 352 |
| `LetterDef`, `FactionDef`, `WorldObjectDef`, `MapGeneratorDef`, `MainButtonDef`, `JobDef` | 38 | 301 |
| **Total** | **1.907** | **36.477** |

Es cerca de diez veces VFE - Tribals. Las lecciones son el 73 % del texto.

## Decisiones de Angel (24-oct-2026)

1. **Nombres de la ventana dentro de las lecciones: se traduce todo.** Las lecciones dicen «abre
   Overview, People, Buildings o Deliveries», y en el juego esos botones siguen en inglés. Angel eligió
   traducirlos igual (opción 3 de tres: dejarlos en inglés, traducirlos con el inglés al lado, o
   traducirlos). Usar siempre la misma traducción para cada botón: van en la tabla de abajo.
2. **Términos del mod: aprobados como están en esta tabla** (no están en el glosario ni en la
   traducción oficial, salvo «alcalde»):

   | Inglés | Propuesta |
   |---|---|
   | vassal / vassalage | vasallo / vasallaje |
   | Hegemony (en los títulos de las lecciones) | Hegemony, sin traducir: es el nombre del mod |
   | mayor | alcalde (como en la traducción oficial) |
   | settlement | asentamiento (oficial) |
   | claim (territorial) | reclamo |
   | contested territory | territorio disputado |
   | town hall | ayuntamiento |
   | disposition / faction influence | disposición / influencia de facción |
   | consumer goods | bienes de consumo |
   | tribute | tributo |

3. **Tabla y script: aprobado** («aplícalo donde sea posible, pero revísalo luego para que coincida y
   no haya errores»). Se escribe una tabla clave → traducción (escrita a mano, una por una) y un script
   la copia solo sobre los `TODO`. Después se revisa. Desde el 26-oct-2026 las herramientas son
   órdenes de `RML/Herramientas/traducir.py` (cómo se usa cada una, al principio del archivo):
   - `tabla <mod> --palabras N --salida <tabla.tsv>`: la tabla de la tanda, con el inglés arriba de
     cada clave.
   - `aplicar <mod> <tabla.tsv>`: solo reemplaza `TODO` cuya clave esté en la tabla; rechaza `<`, `>`
     o `&` sin escapar, y no escribe si el XML no queda válido. Respeta los saltos de línea (CRLF y LF)
     del archivo. Guarda una copia de antes y en el mismo informe corre las tres órdenes de abajo.
   - `revisar <mod> [--contra HEAD]`: que fuera de los `TODO` no haya cambiado ni un byte, y que cada
     traducción conserve lo del inglés (marcadores, colores, saltos de línea). Los saltos del
     comentario se escriben `\n` en la traducción.
   - `terminos <mod>`: avisa cuando el inglés tiene un término del glosario del mod y el español no
     tiene su traducción acordada.

**Términos que salieron al traducir** (para usar igual en las lecciones): psycaster → psicoemisor;
psylink → psicoenlace; bandwidth → banda ancha; tech points → puntos de tecnología; technology stage
→ etapa tecnológica; grand construction → gran construcción; landmark → punto de interés (oficial);
goodwill → buena voluntad; outlander → forastero; realm → reino; tile → celda; grav site → sitio de
gravedad; Spacer → espacial; masterwork / poor → magistral / mediocre (oficial); chemfuel →
quimbustible (oficial); mech → meca; troops → tropas; travelling party → grupo en viaje; skirmish →
escaramuza. Los nombres de edificios, rasgos y memes son los de los archivos ya traducidos: copiarlos
de ahí, no traducirlos de nuevo. Los rasgos y memes usan los nombres oficiales o los de RML (Vanilla
Traits Expanded y Vanilla Ideology Expanded - Memes).

## Etapas

### 1. Glosario del mod
Con las decisiones de arriba, dejar los términos en una tabla al final de este plan, más los que
salgan al traducir (edificios, disposiciones, rasgos). Antes de cada término, buscar en `GLOSARIO.md`
y en la traducción oficial (los `.tar` del juego).

### 2. Lo corto primero (1 sesión)
Todos los archivos menos `ConceptDef`: 977 pendientes, unas 9.700 palabras. Así los nombres de
edificios, disposiciones y especialidades quedan fijos antes de que aparezcan en las lecciones.

### 3. Las lecciones (4 sesiones)
`ConceptDef`, en tandas de unas 115 lecciones (título y texto) por sesión. Los nombres de edificios y
disposiciones se copian de lo ya traducido en la etapa 2.

### 4. Verificar y probar (dentro de la última sesión)
- Las tres comprobaciones del diff de `AGENTS.md` (la 1 y la 2 no deberían mostrar nada, porque la
  carpeta es nueva).
- Que no quede ningún `TODO` y que los saltos de línea de los comentarios sigan igual que en la copia
  de la extracción.
- Regenerar el índice con `LoadFoldersBuilder -build`.
- Angel abre el juego con el mod y mira la pestaña Vasallaje y un par de lecciones.
- Commit «Hegemony: traducción completa». **Sin push hasta que Angel diga.**

## Sesiones (estimación)

Con la tabla y el script: la etapa 2 y el plan, una sesión; las lecciones, unas 3 o 4 más. Cada
sesión deja anotado acá hasta dónde llegó.

## Avance

- 24-oct-2026, sesión 1: plan, decisiones y **etapa 2 terminada**. Los 12 archivos cortos están
  traducidos (977 entradas, ninguna `TODO`), con `revisar.py` y `terminos.py` sin problemas, y en un
  commit local sin push. Falta `ConceptDef`: 930 entradas (465 lecciones), de arriba hacia abajo.
- 25-oct-2026: **en pausa.** El autor todavía no le da soporte de traducción a la ventana del mod
  (sus textos están escritos en la DLL, sin `Keyed` ni `.Translate()`). Angel le pidió ese soporte en
  Steam. Si lo agrega, se reextrae el mod, se traduce la interfaz y después se siguen las lecciones.

## Próxima sesión

Lecciones de `ConceptDef/uq1759.xml`, en tandas de unas 115 lecciones. Por tanda: leer los ingleses
pendientes con `traducir.py tabla`, escribir la tabla en la carpeta temporal y copiarla con
`traducir.py aplicar`, que en el mismo informe revisa y busca los términos. Al llegar a los botones de la ventana (Overview, People, Buildings, Deliveries y los
demás), fijar su traducción y anotarla en la tabla de términos de este plan antes de seguir.
