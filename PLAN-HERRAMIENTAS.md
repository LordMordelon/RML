# Plan: un comando para traducir con tabla

26-oct-2026. **Aprobado por Angel el mismo día**: las seis decisiones de abajo como se recomiendan, con la
etapa 2 para después de la etapa 1.

## En simple

Desde el 26-oct-2026 cualquier mod se puede traducir con el camino de la tabla: cada traducción se escribe a
mano en una lista y un script la copia al archivo, solo donde dice `TODO` (`AGENTS.md`, «Editar XML de
traducción»). Hoy ese camino son cuatro scripts sueltos, fuera de git, que no saben hacer dos de las partes de
un mod y que cada sesión tiene que acomodar. La propuesta es juntarlos en **un solo comando guardado en RML**,
con pruebas, que haga la tanda completa (preparar la lista, copiarla y revisarla) y que sepa hacer también las
partes que hoy van a mano.

Lo que no cambia: **el comando no traduce**. Copia lo que se escribió a mano y revisa. Sigue prohibido un
script que genere o reemplace texto en masa.

## Lo que se encontró (contado el 26-oct-2026)

**Las herramientas de hoy** están en `Traducciones RimWorld/herramientas-traduccion/`, fuera de los dos
repositorios: no tienen historial ni respaldo, y una sesión en la nube no las ve. Son `aplicar.py`,
`revisar.py`, `terminos.py`, `pendientes.py`, `corregir.py`, `fin-de-linea.py`, `terminos-rimpacts.py` y
`oficial/` (`oficial.py`, `buscar.py`, `cotejar.py` y dos cachés de 16 MB). Se usaron en DMS II, Hegemony y
Expedition, unas 3.000 entradas sin un error que llegara al juego.

**Lo que costó en las últimas sesiones**, cada vez:

| Problema | Qué pasó |
|---|---|
| Listar los `TODO` con su inglés | Cada sesión escribió un programa en el momento |
| `terminos.py` trae fijo el glosario de Hegemony | Para Expedition se armó una copia en la carpeta temporal de una sesión; la siguiente la buscó ahí. Se pierde cuando se limpia esa carpeta |
| `revisar.py` necesita una copia de antes | Hay que hacerla a mano. La de `git show` tiene otros finales de renglón que el archivo de trabajo y marca todos los renglones |
| `aplicar.py` recorta los espacios del principio | Una entrada que los necesite pide una copia del script |
| Rutas de Git Bash (`/c/...`) | Python de Windows no las entiende: falla sin escribir nada, pero hay que repetir |
| Las tres comprobaciones del diff | Se copian desde `AGENTS.md` en cada sesión |
| Edit pasa a CRLF los comentarios con LF | Para arreglarlo existe `fin-de-linea.py`, pero hay que acordarse de correrlo (memoria «saltos de línea») |
| Caché de búsqueda de RML (`rml.pkl`) | No se rehace sola: busca sobre las traducciones del día en que se armó |

**Dos partes que la tabla no puede hacer hoy**, porque `aplicar.py` busca por nombre de etiqueta:

- **Las listas** (las reglas de gramática de `RulePackDef`, sobre todo): cada elemento es un `<li>TODO</li>`,
  sin nombre propio. Hay 1.034 listas en RML. En 994, los comentarios en inglés de arriba coinciden uno a uno
  con los elementos. En 40 no coinciden: traducciones viejas sin comentario o con comentarios de varios
  renglones. Esas no se pueden emparejar con seguridad.
- **`Patches/`**: todas las etiquetas se llaman igual (`<label>TODO</label>`). Lo que las distingue es el
  `xpath` de su operación. En las 2.868 entradas de `Patches` de RML, `xpath` + campo **no se repite nunca**,
  así que sirve de nombre. Hay además 194 `<li>` dentro de `Patches`: casi todos son nombres de mods
  (`<mods><li>Royalty</li>`), que no se traducen, y 69 son reglas de gramática con su comentario en inglés
  justo encima de cada `<li>` (en `DefInjected`, en cambio, los comentarios van todos juntos arriba de la
  lista). Un `<li>` sin comentario en inglés no es una entrada y no se toca nunca.

En Expedition, la otra sesión hizo a mano las 37 reglas de gramática y los 5 textos de `Patches`.

**Cuánto queda por traducir en RML:** 8.907 `TODO` en 5 mods: Combat Extended 5.086, Cinders 1.514,
Ascension 1.355, Hegemony 930 (en pausa) y AncientCorps 22 (dejados a propósito, punto 5 de
`PLAN-AOBAKUMA.md`). En Combat Extended falta ver cuántos son a propósito antes de planear nada.

**Dónde puede vivir la carpeta sin efectos raros:**

- En RML, una carpeta nueva en la raíz no llega al juego: `CopiaLimpia` solo copia `About/`, los archivos de
  la raíz que nombra y las carpetas de origen (`Data/`, `Ajustes/`, `Common/`). Tampoco dispara la CI:
  `loadfolders.yml` solo mira `Data/`, `Ajustes/`, `Common/`, `About/About.xml` y `Source/`.
- En el extractor, cada push que no sea solo `.md` publica una versión nueva (`publish.yml`).

## Opciones

### Dónde

| Opción | A favor | En contra |
|---|---|---|
| **A. `RML/Herramientas/`** (recomendada) | Con historial; al lado de lo que edita; la ve la nube; no llega al juego ni dispara la CI | Un push a RML la sube junto con todo |
| B. Como hoy, fuera de git | No hay que hacer nada | Sin respaldo ni historial; la nube no la ve; las copias se desparraman |
| C. En el repo del extractor | Es el repo «de herramientas» | Cada cambio publicaría una versión del extractor; edita archivos de RML, no los genera |
| D. Un repo nuevo | Separado del todo | Un tercer repo que mantener, para unos cientos de renglones |

### Con qué

| Opción | A favor | En contra |
|---|---|---|
| **A. Python** (recomendada) | Las cuatro piezas ya existen y están probadas con 3.000 entradas; trabajan renglón por renglón y no tocan un byte fuera del `TODO`; sin compilar | La CI de RML hoy no corre Python (se arregla con un flujo chico, ver «Pruebas») |
| B. C# en `RML/Source/`, como el builder | El mismo lenguaje que el resto de RML; sus pruebas ya corren en la CI | Rehacer lo que ya funciona (el doble de trabajo); `dotnet run` compila en cada uso; las bibliotecas de XML de .NET reescriben el archivo entero, y acá hay que tocar un renglón y nada más |
| C. Dentro del extractor (volver a importar una tabla, como hacía el Excel) | Reusaría su lectura de XML | El extractor reescribe el árbol entero del mod; mezcla traducir con extraer; cada cambio publica una versión |
| D. PowerShell | Viene con Windows | Escribe en ANSI por defecto; la regla 4 de `AGENTS.md` lo descarta |

La regla 4 de `AGENTS.md` («no rehacer en Python lo que ya hacen las herramientas .NET») no aplica: ninguna
herramienta .NET llena `TODO` ni revisa traducciones renglón por renglón.

### El formato de la tabla

Se queda el de hoy, texto con `clave<TAB>traducción`, con dos agregados: un encabezado por archivo
(`## archivo: <ruta>`), para que una clave repetida en dos archivos no se confunda, y el inglés arriba de cada
renglón (`# EN: ...`), que es lo que se lee al traducir. Una planilla `.xlsx` se descarta: el extractor la sacó
en la 1.0 por el peso de sus bibliotecas, y la tabla de texto se escribe y se revisa más rápido.

## Lo que se propone

Un archivo, `RML/Herramientas/traducir.py`, con estas órdenes. El mod se nombra por el comienzo de su carpeta o
por su número del Workshop, como en `RimworldExtractorLotes`.

| Orden | Qué hace | Reemplaza a |
|---|---|---|
| `estado [mod]` | Cuántos `TODO` hay, por mod o por archivo, y cuántas palabras de inglés | conteos a mano |
| `tabla <mod> [archivo] [--palabras N]` | Escribe la tabla para llenar: encabezado por archivo, el inglés arriba de cada renglón y la clave con la traducción vacía. Corta en tandas de unas N palabras | `pendientes.py` y los programas de cada sesión |
| `aplicar <mod> <tabla>` | Guarda una copia de cada archivo, copia la tabla solo sobre `TODO` y corre `revisar`, `terminos` y `comprobar` con un solo informe | `aplicar.py` + la copia a mano + las revisiones sueltas |
| `revisar <mod> [--contra copia\|HEAD]` | Que fuera de los `TODO` no cambió ni un byte, y que cada traducción conserve lo del inglés (qué frena y qué avisa: «Avance»). Sirve también después de editar a mano | `revisar.py` |
| `terminos <mod> [--glosario ruta]...` | Lee los `GLOSARIO*.md` del mod (y los de la familia que se le pasen) y avisa cuando el inglés usa un término y el español no usa el acordado. Resume por término | `terminos.py` y sus copias |
| `comprobar` | Las tres comprobaciones del diff de `AGENTS.md`, con los mismos comandos | copiarlas cada vez |
| `prueba <mod> --desde <commit>` | La lista de nombres nuevos para mirar en el juego, agrupada (edificios, armas, investigación…) | armarla a mano |
| `lineas <copia> <archivo>` | Devuelve a LF los renglones que Edit pasó a CRLF | `fin-de-linea.py` |
| `corregir <mod> <cambios>` | Cambia valores ya traducidos por clave, comprobando el valor viejo (para correcciones de términos) | `corregir.py` |

`oficial/` (buscar en la traducción oficial y cotejar un glosario) se copia tal cual, con las cachés fuera de
git, y la de RML se rehace sola cuando hay traducciones más nuevas que ella.

**Lo que tiene que hacer bien `aplicar`**, cada punto con su prueba:

1. Escribe solo sobre un `TODO` exacto. Nunca toca un valor con texto, los comentarios ni la estructura.
2. Conserva los finales de renglón (CRLF o LF, renglón por renglón), la marca BOM y los espacios del
   principio y del final de cada traducción.
3. No guarda nada si un archivo queda como XML inválido, si hay `<`, `>` o `&` sin escapar, o si una clave
   de la tabla aparece como `TODO` en dos archivos sin encabezado que diga cuál.
4. **Listas:** empareja cada `<li>` con su comentario en inglés solo si los dos números coinciden; si no, la
   lista queda fuera de la tabla con un aviso, para hacerla a mano. Una lista se llena entera o no se toca:
   una inyección de lista reemplaza la lista entera (`AGENTS.md` del extractor), así que a medio llenar no
   queda bien. Qué hace el juego exactamente con un `<li>TODO</li>` suelto no está comprobado, y no hace
   falta para esta regla. En las reglas de gramática,
   cada elemento tiene que conservar su `simbolo->`.
5. **`Patches`:** la clave es el `xpath` + el campo, que en RML es único.
6. **Campos que guardan una clave** (punto 5 de `PLAN-AOBAKUMA.md`): si el inglés de una entrada es el
   nombre de una clave de `Keyed` del mismo mod, la tabla lo marca y `aplicar` no lo llena salvo que se le
   pida. Hoy esa revisión se hace a mano, una por una.
7. No toca nunca un `UNUSED.xml`.
8. Acepta rutas de Windows y de Git Bash. En Windows corre las comprobaciones con el `bash` de Git, no con el
   de WSL, que también se llama `bash`.
9. Si otra sesión cambió archivos en la misma carpeta, el informe de `comprobar` dice cuáles no son de esta
   tanda (hoy pasó: dos sesiones trabajaron Expedition a la vez).

## Pruebas

Antes que el código, como pide la forma de trabajo:

- **Pruebas con archivos chicos de ejemplo** (`RML/Herramientas/pruebas/`, `unittest` de Python, sin
  bibliotecas de afuera): un caso por cada punto de la lista de arriba, más que la tabla vacía que sale de
  `tabla`, una vez llenada, se aplique sin avisos.
- **Prueba contra trabajo real, una vez:** rehacer Expedition desde el commit anterior a cada tanda con las
  tablas que usaron esas sesiones, y comprobar que el resultado sea **idéntico byte a byte** a lo que quedó en
  git. Las tablas de las dos mitades de los `ThingDef` todavía están en las carpetas temporales de esas
  sesiones.
- **En GitHub:** un flujo chico y aparte, `herramientas.yml`, que corre solo esas pruebas cuando cambia
  `Herramientas/`. No toca `loadfolders.yml`, así que no regenera nada ni publica.

## Lo que se actualiza junto

- `AGENTS.md`: la estructura, «Editar XML de traducción» (con el comando en vez de los scripts sueltos) y
  cómo correr las pruebas.
- `CLAUDE.md` de la carpeta de trabajo: el comando en «Comandos».
- Los planes que nombran `herramientas-traduccion/` (`PLAN-AOBAKUMA.md`, `PLAN-HEGEMONY.md`): la ruta nueva.
  Solo si ninguna otra sesión los está editando.
- La carpeta vieja `herramientas-traduccion/` se borra recién cuando Angel confirme que ninguna sesión la está
  usando. Está fuera de git, así que borrarla no tiene vuelta atrás: su contenido ya va a estar en RML.

## Riesgos

| Riesgo | Cómo se cubre |
|---|---|
| Un error del comando daña traducciones en muchos archivos a la vez | Solo escribe sobre `TODO`, guarda copia antes, revisa byte a byte después y no guarda si algo falla; las tres comprobaciones del diff siguen antes de cada commit |
| Una lista emparejada mal (traducción en el elemento equivocado) | Solo empareja si los números coinciden; si no, a mano |
| Avisos de términos que no son errores (ruido) | Resumen por término en vez de uno por entrada; los avisos son para mirar, no bloquean |
| La nube no tiene Python o se llama `python3` | Se documentan los dos nombres; el comando no usa bibliotecas de afuera |

## Avance (26-oct-2026)

**Etapa 1 hecha en la misma sesión del plan**, sin subir: `Herramientas/traducir.py`, sus pruebas
(`Herramientas/pruebas/`, 26 en verde), `Herramientas/oficial/` y `herramientas.yml`. Rehacer las dos mitades de
los `ThingDef` de Expedition (339 entradas) con las tablas de esas sesiones da un resultado **idéntico byte a
byte** al de git, sin avisos. Las 8.907 `TODO` de RML quedan reconocidas, todas con su inglés.

**Errores del plan que salieron al construirlo y medirlo contra RML**, ya corregidos en el comando:

| Error | Cómo se vio | Arreglo |
|---|---|---|
| La revisión frenaba por cualquier marcador distinto | Sobre las 105.507 traducciones de RML frenaba 1.273 que están bien: el español saca con razón `[PAWN_pronoun]` (936) y traduce palabras entre corchetes («[Overloaded]» → «[Sobrecargado]») | Frena solo lo que se ve roto en el juego: una `{llave}` que el inglés no tiene (salvo las de género, `{X_gender ? a : b}`), un color sin cerrar, una regla sin su `simbolo->`, un cambio fuera de un `TODO`. Lo demás avisa. Con eso no frena ninguna traducción nueva correcta |
| Emparejar listas por posición | En listas ya traducidas el orden del español no siempre sigue al de los comentarios (`ArtDescriptionUtility_Global`: el 32 es «subimageany->» en inglés y «artextra_clause->» en español) | Una lista con elementos traducidos y `TODO` mezclados va a mano. Hoy las 26 listas con `TODO` de RML están enteras en `TODO` |
| Los `<li>` de `Patches` | 194 contados como traducibles, pero casi todos son nombres de mods | Un `<li>` sin comentario en inglés justo arriba no es una entrada |
| `terminos` con el glosario tal cual | En Expedition daba 253 avisos: los glosarios abrevian («large / medium / small fragments»), «EMP» encontraba «employed», «municiones» no era «munición» | Completa las abreviaturas, busca palabras enteras con su plural y saca la raíz del plural en «-es»: 56 avisos, casi todos para mirar |
| `aplicar` y `corregir` sin el mod | El comando necesita saber en qué carpeta buscar | `aplicar <mod> <tabla>` y `corregir <mod> <cambios>` |
| La tabla vieja recortaba espacios | Las entradas que los necesitaban pedían una copia del script | Se conservan, y un espacio al principio o al final que el inglés no tiene sale como aviso |

**Errores en las herramientas viejas**, corregidos en la copia de `Herramientas/oficial/`: `oficial.py` guardaba
la etiqueta de cada ataque con la clave de su arma, y «barrel» salía como oficial «LMG»; `buscar.py` no rehacía
su caché de RML y su uso nombraba una variable `EXCLUIR` que no existe. Queda una limitación conocida: no
encuentra las partes del cuerpo de la oficial («left lung»), que se siguen buscando a mano.

**Lo que salió de paso en RML**, sin tocar (para decidir aparte):

- **44 reglas de gramática rotas** en The Dead Man's Switch de 1.6 (`3121742525`): perdieron el guion del
  `simbolo->` («manufacture&gt;», «questDescription&gt;grupo de jefes»), así que el juego no las reconoce como
  reglas. Esa carpeta está congelada.
- **376 elementos de lista** en otros mods (Vanilla Social Interactions 144, Noobert 121, VIE - Memes 41…) cuyo
  `simbolo->` no es el del comentario en la misma posición. Puede ser solo otro orden, que no rompe nada: no
  está comprobado.
- **El glosario de Expedition** dice «missile body / airframe → cuerpo del misil / fuselaje», y los 13 cuerpos
  se tradujeron «cuerpo de misil …», como dice la fila de «Partes y cuerpos de misil». Sobra una de las dos.
- **`PLAN-AOBAKUMA.md` cuenta 22 campos de Ascension** que guardan una clave de `Keyed`; en los archivos hay 8
  (y el de `Hostilityresetbysystem_label`, que no es clave). AncientCorps da los 22 que dice.

**Tanda real de otro mod (criterio 3), hecha:** Cinders, 21 entradas en 4 archivos (dos listas de reglas de
gramática y dos `Patches` de Combat Extended), de `tabla` a `aplicar` sin tocar nada a mano. El informe salió
limpio y el diff solo cambia los `TODO`, con los finales de renglón iguales a la copia de antes.

**Falta para terminar la etapa 1:** subir y ver las pruebas en GitHub; cambiar la ruta en `PLAN-AOBAKUMA.md` y `PLAN-HEGEMONY.md`,
que hoy edita otra sesión; y borrar `herramientas-traduccion/` cuando Angel confirme que ninguna sesión la usa.

## Etapa 2 (opcional): informe de términos viejos

Se hace después de la etapa 1, en su propia sesión, y solo si Angel la aprueba aparte.

**Para qué.** Desde el 25-oct-2026, si un glosario contradice a la traducción oficial, gana la oficial. Los
glosarios ya se corrigieron, pero los textos traducidos antes siguen con el término viejo. La lista de
pendientes está en «Pendientes fuera de este plan» de `PLAN-AOBAKUMA.md`, contada a mano ese día: «lanzadera»
(172 usos en 40 archivos), «entropía psíquica» (26), «abrasador» (22), «psiconjuro» (14), «EMP» (15),
«calor neuronal» (7) y otros más chicos. Esa cuenta envejece con cada traducción nueva.

**Qué hace.** Una orden más, `traducir.py viejos [mod]`, **de solo lectura**: recorre RML y dice, por término
y por mod, cuántos textos tienen todavía el término viejo y cuáles son (archivo y clave). Al final da el total.

- **La lista de términos** va en un archivo, `Herramientas/terminos-viejos.tsv`: término viejo, término
  correcto y una nota. Se arma con los pendientes de `PLAN-AOBAKUMA.md` y las filas de los glosarios que dicen
  «Decía…». Un término que se corrige en un glosario se suma ahí en el mismo commit.
- **Busca palabras enteras**, sin distinguir mayúsculas, y mira solo el texto traducido: no cuenta los
  comentarios `EN:` ni los `TODO`, y no abre los `UNUSED.xml`.
- **Los dudosos se marcan, no se cuentan como error.** «Abrasador» a veces es el adjetivo y está bien; esos
  términos llevan la marca «revisar» en la lista y el informe los muestra aparte.
- **Los mods congelados** (el DMS de 1.6) salen aparte, porque a propósito no se corrigen.

**Qué no hace.** No corrige nada. Las correcciones siguen a mano, mod por mod y mirando cada caso, con
`corregir` (que comprueba el valor viejo antes de cambiarlo), un commit por mod y las tres comprobaciones del
diff.

**Pruebas.** Con archivos chicos de ejemplo: que encuentre la palabra entera y no un pedazo («lanzaderas» sí,
otra palabra que la contenga no), que salte comentarios, `TODO` y `UNUSED.xml`, y que separe los dudosos y los
congelados. Y una comparación con la cuenta a mano de `PLAN-AOBAKUMA.md`: si las cifras no coinciden, se
explica la diferencia antes de dar el informe por bueno.

**Terminado cuando:** las pruebas pasan, el informe de todo RML queda al día en `PLAN-AOBAKUMA.md` (en lugar
de la cuenta a mano) y `AGENTS.md` dice cómo correrlo. Es media sesión, estimada.

## Decisiones de Angel

1. **Dónde:** `RML/Herramientas/` (recomendado) o dejarlo fuera de git.
2. **Con qué:** Python (recomendado) o C#.
3. **Pruebas en GitHub:** un flujo aparte que corre solo las pruebas del comando (recomendado) o solo en el PC.
4. **Sugerencias de RML en la tabla:** si un inglés idéntico ya está traducido en otro mod, `tabla` lo
   muestra como comentario (`# RML: cañón (14)`) y la traducción se sigue escribiendo a mano. Recomendado: sí.
   Nunca se copia sola.
5. **Etapa 2, el informe de términos viejos** (arriba): aprobarla aparte, para después de la etapa 1.
   Recomendado: sí, en su propia sesión, justo antes de empezar las correcciones de términos.
6. **Borrar `herramientas-traduccion/`** cuando la migración esté probada y no haya otra sesión usándola.

## Sesiones (estimación)

Una sesión para el comando, las pruebas, la prueba contra Expedition y la documentación. Si las listas o los
`Patches` dan más trabajo del esperado, se dejan para una segunda y la primera entrega todo lo demás.

## Terminado cuando

1. Las pruebas pasan en el PC y en GitHub, y cada punto de «Lo que tiene que hacer bien `aplicar`» tiene la
   suya.
2. Rehacer Expedition con las tablas guardadas da un resultado idéntico al de git.
3. Una tanda real de otro mod (Ascension o Cinders) se hace con el comando, incluidas sus listas y sus
   `Patches`.
4. `AGENTS.md`, `CLAUDE.md` y los planes nombran el comando, y la carpeta vieja se borró con el visto bueno de
   Angel.
