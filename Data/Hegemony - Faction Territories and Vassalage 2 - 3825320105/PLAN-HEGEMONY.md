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

- 30-oct-2026: **se retoma.** El autor agregó el soporte de traducción: Angel reextrajo el mod y
  apareció `Keyed/wzj8g1.xml`, la ventana entera. Angel pidió traducir todo en una sola sesión, sin
  importar el costo, con la traducción oficial como referencia. Los términos pasaron a
  [GLOSARIO-HEGEMONY.md](GLOSARIO-HEGEMONY.md), que es el que lee `traducir.py`.

## Etapas desde el 30-oct-2026

Cifras de la reextracción del 30-oct:

| Archivo | Pendientes | Palabras en inglés |
|---|---|---|
| `Keyed/wzj8g1.xml` (la ventana) | 8.868 (8.373 ingleses distintos) | 106.845 |
| `ConceptDef` (lecciones) | 1.294 | 38.140 |
| `BuildingDef`, `MainButtonDef`, `WorldObjectDef` (nuevos) | 18 | 330 |
| **Total** | **10.180** | **145.315** |

Además, 11 descripciones ya traducidas de `BuildingDef` cambiaron su inglés (y la etiqueta «nutrient
synthesis plant» pasó a «life support»): se corrigen a mano contra el `EN` nuevo.

1. Los archivos cortos y las 11 descripciones con el inglés cambiado.
2. `Keyed`, de arriba hacia abajo, en tandas de unas 3.000 palabras. Los botones y pestañas fijan su
   traducción en el glosario antes de que aparezcan en las lecciones.
3. `ConceptDef`, igual. Los nombres de la ventana se copian de lo traducido en el paso 2 (decisión 1).
4. Verificar (las tres comprobaciones, `revisar`, `terminos`, ningún `TODO`), regenerar el índice y
   commitear por partes. Sin push hasta que Angel diga.

### Avance desde el 30-oct-2026

- Paso 1 hecho y commiteado (b73184d8).
- Paso 2 en curso: el `Keyed` se traduce de arriba hacia abajo con `traducir.py tabla --archivo Keyed`; lo
  que queda en `TODO` es lo pendiente. Cuidados que salieron: la tabla pierde los espacios del final de un
  texto («Aporte detenido: »), así que al terminar hay que devolvérselos a los que el inglés tiene; el `%`
  va pegado como en el inglés (el juego le pone el número delante); los `Keyed` con «-&gt;» dan un falso
  aviso de «regla de gramática» y se aplican con `--forzar`, mirando que no haya otro aviso grave.
- `corregir` con dos renglones de la misma clave aplica solo el último: una corrección por clave y tanda.
- Paso 2 terminado el 30-oct: los 8.868 textos del `Keyed` (547e3f9d). Al final se devolvieron los espacios
  finales a 178 textos y se unificaron términos con la oficial («cápsula de escape») y entre sí («nave
  insignia», «plantilla de mecas», «vía psíquica», «trato de armas», «ícono»).
- Paso 3 terminado el 30-oct: las 1.294 entradas de `ConceptDef`. Unas 600 son lecciones de «referencia»
  cuyo texto es idéntico a uno de la ventana: se copió esa misma traducción, ya revisada, para que la
  lección y la ventana digan lo mismo. Sus títulos van como «Referencia de Hegemony: …» con el nombre del
  botón o ajuste tal como está en la ventana.
- Los términos nuevos están en la última tabla de `GLOSARIO-HEGEMONY.md`.
- Falta: probar en el juego (ventana, lecciones del Asistente de aprendizaje) y confirmar «avasallar».

## A investigar: textos que en el juego siguen en inglés (30-oct-2026)

Angel probó en el juego y mandó capturas. Idea de Angel: que lo arregle el extractor, no escribir a mano.

### 1. Nombres de etapa de los edificios («Growing patch», «Council hut», «Hut»)

- **Causa encontrada:** cada edificio tiene una lista `tierLabels` con el nombre de sus 6 etapas
  (`Common/Defs/Buildings.xml` del mod; el mod los traduce en
  `Common/Languages/English/DefInjected/FactionTerritories2.BuildingDef/Text.xml`). Son 792 en 132
  edificios. El extractor no conocía la etiqueta `tierLabels`, así que nunca llegaron a RML.
- **Hecho y commiteado (30-oct):** en el extractor, `tierLabels` agregada a `Prefabs.EtiquetasAgregadas`,
  con su prueba (`EtapasDeEdificioTests`). Con eso se reextrajo Hegemony: aparecieron las 792 entradas en
  `BuildingDef/mcqike.xml`, y el diff no dañó nada (8 textos solo cambian `&apos;` por `'`, que es lo mismo).
- Las 792 se tradujeron con tabla a mano, y Angel pidió commitearlas así. Queda una idea para el
  extractor, si vuelve a pasar: reutilizar la traducción del `label` cuando una etapa dice lo mismo que el
  nombre del edificio («farm» = `FT2_Farm.label`).

### 2. Botón «Clear» de la tienda de recursos

- La clave `Hegemony_Clear_83B12C2216` («Borrar») sí está traducida, pero el juego muestra «Clear». El
  botón usa otra clave o un texto fijo en la DLL. Buscar en `Common/Languages/English/Keyed/Text.xml` del
  mod cuál clave dice «Clear» junto al buscador de la tienda; si es texto fijo en la DLL, avisar al autor.

### 3. «Cant» al lado de la cantidad

- `Hegemony_Qty_A9BE9C3E9A` es «Cant.» y en el juego se ve «Cant» (sin punto, quizá recortado). Mirar si
  cabe «Cant.» o conviene otra forma.
