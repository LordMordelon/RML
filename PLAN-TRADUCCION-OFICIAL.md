# Plan: botón para actualizar la traducción oficial del juego

Lo pidió Angel el 25-oct-2026: un botón en el juego que haga lo que dice la
[instalación manual](https://github.com/Ludeon/RimWorld-SpanishLatin#manual) de la traducción oficial
al español latinoamericano.
**Estado: aprobado por Angel el 25-oct-2026**, con las tres recomendaciones de «Decisiones» y el
agregado de «En Novedades, con su diff». Se empieza por la etapa 0.

## Por qué

El juego trae la traducción oficial **empaquetada el día de la última actualización de RimWorld**.
Ludeon toma lo que hay en su repositorio de GitHub cada vez que publica una versión, por cualquier
motivo. Lo que los traductores arreglan después queda esperando en GitHub hasta la siguiente
actualización del juego, que puede tardar semanas o meses. El botón le trae esos arreglos al
jugador el mismo día.

Sirve a cualquiera: a quien tiene el juego en Steam, a quien lo compró sin Steam y a quien lo juega
con una versión que no se actualiza sola.

## Qué hace el botón

1. Baja de GitHub el zip del repositorio `Ludeon/RimWorld-SpanishLatin` (rama `master`).
2. Lo abre en una carpeta temporal y mira la versión que dice su README («versión 1.7.xxxx»).
   **Si no es la misma versión principal del juego (1.7), no instala nada** y lo dice. Así, cuando
   el repositorio pase a la 1.8, nadie en la 1.7 recibe textos que no corresponden.
3. Por cada carpeta del zip (`Core`, `Royalty`, `Ideology`, `Biotech`, `Anomaly`, `Odyssey` y las que
   vengan después) **que también exista en `Data/` del juego**, copia su contenido a
   `Data/<carpeta>/Languages/SpanishLatin (Español(Latinoamérica))/`. Un DLC que el jugador no
   tiene, se salta.
4. Al `.tar` del juego con ese mismo nombre **no lo borra: le cambia el nombre** a
   `SpanishLatin (Español(Latinoamérica)).tar.respaldo-rml`. La página oficial dice borrarlo; con el
   respaldo se puede deshacer.
5. Anota en los ajustes de RML la fecha y el código del cambio de GitHub que se instaló, para
   saber después si hay algo más nuevo.
6. Avisa que hay que **reiniciar el juego** para ver los textos.

Todo se arma primero en la carpeta temporal y se mueve a `Data/` solo al final. Si la descarga se
corta o algo falla a mitad de camino, el juego queda como estaba.

**«Deshacer»** borra las carpetas instaladas y le devuelve su nombre al `.tar`. Si mientras tanto
Steam ya puso un `.tar` nuevo, solo borra el respaldo viejo.

## Qué muestra

Junto al botón, una línea con el estado:

| Estado | Qué dice | Botones |
|---|---|---|
| Hay arreglos en GitHub más nuevos que lo del juego | «Hay arreglos nuevos de la traducción oficial (del 20-oct)» | Actualizar |
| Lo instalado ya es lo último | «La traducción oficial está al día» | Deshacer |
| Steam actualizó el juego y volvió a poner su `.tar` | «El juego trae su propia traducción; la que bajaste quedó sin usar» | Actualizar de nuevo (si GitHub es más nuevo) · Borrar la que bajé |
| La casilla de revisar está apagada | «La revisión está apagada» | Revisar ahora |
| El juego no es 1.7 | No aparece la sección | — |
| Sin internet o GitHub no responde | «No se pudo consultar GitHub» | Reintentar |

La fecha de GitHub sale de su API pública (`api.github.com/repos/Ludeon/RimWorld-SpanishLatin/commits/master`),
que deja 60 consultas por hora sin clave: se consulta una sola vez por inicio del juego, y nunca
con la casilla de revisar apagada (ver «Lo que decidió Angel»).

## En Novedades, con su diff

Pedido de Angel al aprobar: una actualización de la traducción oficial sale en la pestaña
«Novedades» igual que la de cualquier mod, con su «Ver cambios», y **toda la fila parpadea**.

1. **Al instalar**, antes de reemplazar nada, se comparan clave por clave los textos que el juego
   tenía (el `.tar`, o la carpeta de una instalación anterior) con los recién bajados, por cada
   carpeta: Core y cada DLC. El resultado se guarda con el mismo formato de `Cambios.tsv`
   (cambiado, nuevo, quitado; inglés, antes, ahora), pero en la carpeta de configuración de RML y no
   en la del mod, porque es de cada jugador. El inglés se pone cuando se encuentra: en los `Keyed`
   sale del inglés del juego; en los `DefInjected`, del comentario del archivo si lo trae. Si no,
   la columna va vacía.
2. **La fila aparece recién después de reiniciar**, que es cuando la actualización hace efecto.
   Hasta entonces, los ajustes dicen «Reinicia el juego para ver la traducción nueva».
3. **Al iniciar después del reinicio**, la ventana se abre sola, como con cualquier novedad. Cada
   carpeta que cambió es una fila: «RimWorld (traducción oficial)», «Biotech (traducción oficial)»,
   con la etiqueta «Actualizado», la fecha del cambio en GitHub y el botón «Ver cambios», que abre la
   misma ventana de diff palabra por palabra que los demás mods.
4. **La fila entera parpadea** (el fondo sube y baja de brillo) hasta que se cierra la ventana.
   Después queda como vista y se comporta como las demás novedades.

## Lo que decidió Angel (25-oct-2026)

1. **El botón va en los ajustes de RML** (Opciones → Ajustes de mods → Traducciones [Español Latino]),
   donde ya está el botón para abrir la ventana. Si hay arreglos nuevos, una línea en «Novedades»
   lleva a los ajustes, sin abrir una ventana nueva.
2. **Sin castellano.** Ludeon tiene otro repositorio igual para `Spanish (Español(Castellano))`, pero
   RML es para el español latinoamericano.
3. **El `.tar` se renombra**, no se borra.
4. **Una casilla para no revisar** (pedida el 25-oct-2026): «Revisar si hay arreglos de la traducción
   oficial», en los ajustes de RML y encendida por defecto. Apagada:
   - RML no consulta GitHub al iniciar ni al abrir los ajustes, y no sale la línea de «hay arreglos
     nuevos» en «Novedades».
   - La sección muestra «La revisión está apagada» con un botón «Revisar ahora», que consulta una vez
     sin encender la casilla. Si el jugador quiere actualizar, sigue pudiendo.
   - Lo ya instalado no se toca: «Deshacer» sigue a mano, y las filas de una actualización hecha
     antes de reiniciar salen igual, porque esa actualización la pidió el jugador.
   Se guarda junto a las demás opciones de RML (`AjustesRml`), como la casilla de «Ver cambios».

## Etapas

### Etapa 0: comprobaciones, antes de escribir el botón

Cada una puede cambiar el plan. Si alguna falla, se avisa a Angel antes de seguir.

1. **Cómo carga el juego una carpeta y un `.tar` con el mismo nombre.** Leerlo en el código del
   juego (`LanguageDatabase`). Con eso se sabe si de verdad hay que sacar el `.tar` del camino, y si
   el archivo chico del paso 5 molesta.
2. **Que se pueda descargar desde el juego.** La 1.7 corre sobre .NET y trae `System.Net.Http`;
   hay que probar que la conexión segura con GitHub funcione desde un mod.
3. **Que se pueda abrir el zip.** La 1.7 trae `System.IO.Compression.ZipFile`. La DLL de RML se
   compila para .NET Framework 4.7.2 y también la carga la 1.6, así que el código que usa esto solo
   puede correr en la 1.7.
4. **De dónde sale la fecha de lo que trae el juego.** La del `.tar` en el disco puede ser la de la
   descarga de Steam y no la del empaquetado. Si es así, se compara contra esa fecha, que es más
   nueva que la real: en el peor caso el botón dice «al día» cuando había algo, nunca al revés.
5. **Que se pueda leer el `.tar` para el diff** (la 1.7 trae `System.Formats.Tar`), y si los archivos
   del repositorio oficial traen el inglés en comentarios.

#### Lo que salió (25-oct-2026, sobre la 1.7.4985)

1. **Si están la carpeta y el `.tar` con el mismo nombre, el juego usa el `.tar` y no mira la
   carpeta** (`RimWorld.IO.AbstractFilesystem.GetDirectory` y `GetDirectories`, en
   `Assembly-CSharp.dll`). Por eso el `.tar` tiene que salir del camino. Con el nombre
   `.tar.respaldo-rml` deja de contar, porque el juego solo mira la extensión `.tar`.
   Esto simplifica el caso de Steam: cuando vuelve a poner su `.tar`, no quedan dos copias
   mezcladas. El juego usa la de Steam y la carpeta de RML queda sin efecto. La tabla de estados
   dice entonces «El juego trae su propia traducción; la que bajaste quedó sin usar».
   Lo instalado (fecha y código del cambio de GitHub) se guarda en los ajustes de RML y no en un
   archivo suelto dentro de la carpeta del idioma, para no meterle al juego nada que no espera.
2. **El juego corre sobre .NET 10** (`RimWorld.runtimeconfig.json`) y trae `System.Net.Http` y
   `System.IO.Compression.ZipFile`. Para bajar y abrir el zip no hace falta nada más. Falta
   comprobarlo dentro del juego, en la prueba de la etapa 4.
3. **El zip** trae `Core`, `Royalty`, `Ideology`, `Biotech`, `Anomaly` y `Odyssey`, más `Scripts` y
   `.github`, que no tienen carpeta en `Data/` y quedan afuera solos. Pesa unos 2 MB.
4. **La fecha de lo que trae el juego está dentro del `.tar`:** cada archivo lleva la de su
   empaquetado (16-oct-2026, 21:15 en la 1.7.4985). El `.tar` se lee con el `TarDirectory` del
   juego, sin agregar nada.

   El 19-oct-2026 hubo cambios en GitHub («Update to 1.7.4982 rev1280») que el juego del 21-oct
   todavía no trae. Es justo el caso para el que sirve el botón.
5. **Los archivos oficiales traen el inglés en comentarios** (`<!-- EN: beer -->`), como los de RML,
   así que «Ver cambios» muestra el original también en los `DefInjected`.

### Etapa 1: la lógica, con pruebas y sin el juego

En `Source/RmlVentana/Logica/TraduccionOficial.cs`, con pruebas en `Source/RmlVentana.Tests`
escritas primero: qué estado mostrar según las fechas y los archivos, qué carpetas del zip
corresponden a qué carpetas de `Data/`, la versión del README contra la del juego, qué hace
«Deshacer» con o sin un `.tar` nuevo de Steam, la comparación de textos viejos y nuevos, y cuándo
una actualización pasa a ser una fila de «Novedades».

**Hecha el 25-oct-2026:** `Logica/TraduccionOficial.cs` y 16 pruebas en `TraduccionOficialTests.cs`.
La comparación cubre `Keyed` y `DefInjected`. `Strings` (las listas de nombres) queda afuera: son
archivos de texto con miles de renglones, y un diff renglón por renglón no le diría nada útil al jugador.
Lo que sigue en la lógica, con el juego, es la etapa 2: armar los dos diccionarios (del `.tar` con
`TarDirectory` y de la carpeta del zip) y guardar las líneas en la carpeta de configuración de RML.

### Etapas 2 y 3: hechas el 25-oct-2026, falta la prueba en el juego

- `Source/RmlVentana/OficialJuego.cs`: la consulta a GitHub (en segundo plano, al iniciar), la
  instalación (en un evento largo, con la pantalla de carga del juego) y «Deshacer». El zip se baja
  por el código del cambio y no por `master`, así que se instala justo lo que dice la fecha guardada.
  Se arma en `RML-temporal`, al lado de `Data/` y en su mismo disco, y se mueve al final.
- `Logica/LectorTar.cs`: lee el `.tar` del juego con sus fechas, porque .NET Framework no trae
  lector de tar. Lo prueba también contra el `.tar` real de esta máquina (más de 10.000 textos).
- Los ajustes guardan lo instalado y las filas de «Novedades»; el diff va a
  `Config/RML_CambiosOficiales.tsv`, con el formato de `Cambios.tsv`.
- **Contra los datos reales** (zip de GitHub del 19-oct contra los `.tar` de la 1.7.4985): Core trae
  2 textos cambiados y 19 nuevos, Biotech 1 cambiado, y los otros DLC ninguno. Los `.tar` de los DLC
  son del 16-jul: Ludeon no los reempaquetó en esta actualización.
- Se instalan todas las carpetas, cambien o no: `Strings` no se compara, y una carpeta con cambios
  solo ahí quedaría afuera.

### Etapa 2: descarga e instalación

La descarga va en segundo plano, para que el juego no se congele, con una barra o un texto de
avance. La carpeta temporal se borra al terminar. Si no se puede escribir en la carpeta del juego
(por ejemplo, instalado en «Archivos de programa» sin permisos), se dice eso y no se toca nada.

### Etapa 3: la sección en los ajustes y las filas en «Novedades», con su parpadeo

Con el estilo que ya tiene la ventana de RML. Los textos van en `Common/Languages/SpanishLatin/Keyed`
y `Common/Languages/English/Keyed`, como los demás de la ventana.

### Etapa 4: prueba en el juego (Angel)

1. Abrir los ajustes de RML: tiene que decir si hay arreglos nuevos, con una línea por parte
   (hoy, RimWorld y Biotech con arreglos y los demás DLC al día).
2. Apretar «Actualizar» y reiniciar: la ventana se abre sola, con una fila parpadeando por cada
   carpeta que cambió, y su «Ver cambios» muestra el diff. El juego sigue en español latinoamericano.
3. Apretar «Deshacer», reiniciar y comprobar que vuelve la traducción que traía el juego.
4. En la carpeta del juego, `Data/Core/Languages/` tiene que quedar como antes de empezar.

## Jugadores sin Steam (25-oct-2026)

El botón no depende de Steam: la carpeta sale de `GenFilePaths.OfficialModsFolderPath`, y se instala
en cada carpeta de `Data/` que esté, sin mirar cómo llegó ahí. Un juego base solo, o con algunos DLC,
recibe solo lo suyo; una carpeta sin `.tar` se instala igual (en «Ver cambios», todo como nuevo); un
1.7 más viejo que el de GitHub también instala, y una clave que no tiene traducción sale en inglés.

**Una revisión por parte** (aprobada por Angel el 25-oct-2026, hecha ese día). Con carpetas de DLC
venidas de otra instalación, cada una puede tener su traducción de otra fecha, y comparando solo el
`.tar` de Core un DLC atrasado pasaba como «al día». Ahora:

- La consulta pide, además del último cambio de `master`, el de cada carpeta de `Data/`
  (`commits?sha=<código>&path=<carpeta>&per_page=1`): 2 consultas más una por carpeta. Una carpeta
  que falla o que el repositorio no tiene queda afuera; si fallan todas, se compara como antes, con
  Core contra el último cambio de `master`.
- Cada parte se compara con su propio `.tar` (`OficialJuego.Partes`), y el estado de todo es el más
  urgente de las partes (`TraduccionOficial.Combinar`): basta una atrasada para que haya arreglos.
- Los ajustes muestran una línea por parte: «RimWorld: hay arreglos, del 19 de octubre»,
  «Royalty: al día». «Actualizar» sigue instalando todas, y en «Novedades» solo salen las que
  cambiaron.
- Con los datos reales del 25-oct: RimWorld y Biotech con arreglos del 19-oct; Royalty, Ideology,
  Anomaly y Odyssey al día (su último cambio es del 16-jul, minutos antes de su `.tar`).

## Cuidados

- **RML no incluye los archivos de Ludeon:** el jugador los baja de su repositorio público al
  apretar el botón. Las traducciones de mods de RML siguen cargando después y no cambian.
- **Se tocan archivos del juego**, y es lo único de RML que lo hace. Por eso el respaldo, el
  armado en la carpeta temporal y el «Deshacer».
- **Con los mods normales**, las filas oficiales y las de los mods salen juntas en «Novedades», y la
  ventana se abre sola por cualquiera de las dos. Solo parpadean las oficiales.
- **RML gana donde se cruzan.** Cuando un mod cambia un def del juego, RML trae un `DefInjected`
  para ese def (`IO.PatchesQueElJuegoPisa`, en el extractor), y carga después que la oficial. Un
  arreglo oficial de esa misma clave sale en «Ver cambios», pero con ese mod activo se ve el de RML.
- **El extractor no se rompe** en una máquina con el botón usado: `TraduccionOficial.CargarMod` lee
  el `.tar` y también la carpeta del idioma, así que con el `.tar` renombrado lee la carpeta.
- **Verificar los archivos del juego en Steam** devuelve el `.tar`. El juego vuelve a usar el suyo,
  y la sección de los ajustes lo dice y ofrece las dos salidas.

## Costo estimado

Unas dos sesiones: la etapa 0 y la 1 en una, y la 2 y la 3 en otra.
