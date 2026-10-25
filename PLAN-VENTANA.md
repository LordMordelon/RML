# Plan: ventana de mods traducidos al iniciar RimWorld

Lo pidió Angel el 23-oct-2026: una ventana que, al iniciar el juego, muestre qué mods activos traduce RML.
**Estado: aprobado por Angel el 23-oct-2026** («yapos, entonces comienza»). Se empieza por la etapa 1.

## Lo que decidió Angel

- **La ven todos** los jugadores de RML, con una casilla «No volver a mostrar» y una opción en los ajustes
  del mod para volver a activarla o abrirla a mano.
- **Aparece sola solo si hay novedades**: un mod activo que no estaba la última vez, o cambios nuevos en RML.
- **Y solo si el juego está en español latinoamericano.** Si el idioma activo es otro (inglés, castellano
  u otro), la ventana no aparece sola: RML no le cambia nada a ese jugador, así que no tiene sentido
  avisarle. Se mira el idioma activo del juego (`LanguageDatabase.activeLanguage`) al llegar al menú
  principal, aceptando el nombre corto `SpanishLatin` y el largo `SpanishLatin (Español(Latinoamérica))`.
  Si el jugador cambia el idioma a español latinoamericano durante la partida, la ventana sale en el
  siguiente inicio. El botón para abrirla a mano desde los ajustes del mod funciona en cualquier idioma.
- **Título de la ventana:** «Traducciones [Español Latino]», el nombre de RML en Steam.
- **Al pie**, junto a «No volver a mostrar», una nota: «si un mod también tiene otra traducción al español,
  esta la sobrescribe». Es cierto porque el builder pone a todos los mods de RML en el
  `forceLoadAfter` de `About.xml`: RML carga después y su traducción gana.
- **Qué muestra**, en dos pestañas, y abre siempre en la primera:
  1. **Novedades**: una tabla **Mod** | **Cambio** (etiqueta «Añadido», «Actualizado» o «Corregido») |
     **Fecha**, con lo que cambió en RML desde la última vez que se abrió.
  2. **Tus mods**: un buscador arriba (filtra por nombre del mod, del traductor o por estado) y una tabla
     con todos los mods activos, en el orden de la lista de mods, con tres columnas:
     **Mod** | **Estado** | **Traducido por**. Los mods activados desde la última vez llevan la marca «NUEVO».
     En la lista no se dice «RML»: se usa el nombre de Steam, «Traducciones [Español Latino]».

     | Estado | Traducido por | Cuándo |
     |---|---|---|
     | Traducido | Traducciones [Español Latino] | Lo traduce este mod y nadie más |
     | Sobrescrito | Traducciones [Español Latino], y debajo «en lugar de: X» | Lo traduce este mod y además el propio mod u otro paquete; gana este |
     | Traducido | el nombre del otro paquete, o «Incluida en el mod» | Lo traduce otro, y este no |
     | Sin traducir | botón «Solicitar» | Nadie lo traduce |
     | Sin texto | «No necesita traducción» | Se pidió y no tiene nada que traducir (ver etapa 4) |
     En los sin traducir, la tercera columna trae un botón **«Solicitar»** que abre en el navegador un
     issue ya escrito en el repositorio de RML (nombre del mod, número del Workshop y packageId), como el
     de «marcar como revisada». Esos issues llevan una etiqueta propia, `solicitud`, para separarlos.
     **El juego recuerda las solicitudes:** al apretar el botón se guarda el packageId del mod y la fecha
     en los ajustes de RML (el archivo de configuración del mod que RimWorld guarda en
     `Config/`), así que sobrevive a cerrar el juego. Desde entonces el botón dice «Solicitada el 18-oct»
     en verde. Sigue apretable, con una ayuda que explica que vuelve a abrir el pedido, por si el
     jugador no llegó a enviarlo: el juego sabe que se abrió el navegador, no si el issue se envió.
     Cuando el mod entra a RML, la fila pasa a «Traducido» y la solicitud guardada se borra sola.

  Previsualización: https://claude.ai/artifact/WfNYPCKyZAR5RnkJAtcxZb (datos de ejemplo).

## Por qué es grande

- RML no tiene código: hoy son solo traducciones. Esto le agrega una DLL.
- `LoadFolders.xml` solo carga carpetas condicionales (`IfModActive`). La DLL necesita una carpeta que se
  cargue siempre, y eso cambia el builder (`LoadFoldersBuilder`), `CopiaLimpia` y lo que se sube al Workshop.
- La CI no tiene RimWorld instalado: para compilar la DLL usa las referencias del paquete NuGet
  `Krafs.Rimworld.Ref`, el que usan los mods de la comunidad.

## Cómo sabe cada cosa

| Qué | De dónde sale |
|---|---|
| Traducido por RML | De los `IfModActive` del propio `LoadFolders.xml` de RML, que el juego ya leyó. Sin archivo extra. Las carpetas de `Ajustes/` no cuentan |
| Traducido por otro mod | El mod trae su propia carpeta `Languages/SpanishLatin` (→ «trae su traducción»), u otro mod activo tiene esa carpeta y apunta a él con `IfModActive` o en su `About.xml` (→ «traducido por X»). Es una aproximación: un paquete que no siga ninguna de las dos convenciones no se detecta |
| Sobrescrito | Las dos cosas a la vez: el mod está en RML y además tiene otra traducción según la fila anterior |
| Sin traducir | Lo que no entra en lo anterior. Se dejan afuera Core, los DLC y el propio RML |
| Novedades | Un archivo `Novedades.tsv` que escribe el builder en `-build`, con la historia de git de cada carpeta de `Data/`: **añadido** = la carpeta apareció; **actualizado** = la traducción cambió después de una actualización del mod en Steam; **corregido** = cambió sin que el mod se actualizara. La última distinción es aproximada |

## Etapas

Cada una en su propia sesión, con su commit. Ninguna se sube sin que Angel lo diga.

| # | Etapa | Qué incluye |
|---|---|---|
| 1 | Base | Proyecto `Source/RmlVentana`, carpeta `Common/` que se carga siempre en cada versión, builder y `CopiaLimpia` que la incluyen, CI que compila la DLL. Sin ventana todavía: solo comprobar que RML sigue cargando igual |
| 2 | Pestaña «Tus mods» | La ventana, la tabla, los ajustes del mod y las dos reglas para que aparezca sola (hay novedades y el juego está en español latinoamericano). Los textos en `Keyed`, en español y en inglés (el inglés, para los ajustes del mod en otro idioma) |
| 3 | Pestaña «Novedades» | `Novedades.tsv` en el builder y la segunda pestaña |
| 4 | Gráfico de solicitudes | La rama `solicitudes`, el action que cuenta los pedidos y el gráfico de los mods más pedidos (ver abajo) |
| 5 | Ver qué cambió | Botón «Ver cambios» en las filas «Actualizado» y «Corregido» de Novedades, con los textos antes y después (ver abajo) |

## Etapa 4: los mods más solicitados

Funciona como «marcar como revisada» (`revisado.yml` y la rama `revisados`), que ya está probado.

1. **El pedido:** el botón «Solicitar» abre un issue con la etiqueta `solicitud` y una línea fija,
   `- <WorkshopID> <packageId>`, que es lo único que se lee. El nombre del mod va en el título, para
   leerlo a simple vista.
2. **El action** (`solicitud.yml`), al abrirse un issue con esa etiqueta:
   - lee la línea y comprueba que el WorkshopID sean solo números; si no, comenta el formato correcto y
     deja el issue abierto;
   - si el mod ya está en RML, lo dice y cierra el issue;
   - si no, suma el pedido en `Solicitudes.tsv`, en la rama `solicitudes`, y cierra el issue con un
     comentario que dice en qué puesto quedó.
3. **La rama `solicitudes`** guarda solo `Solicitudes.tsv`: WorkshopID, packageId, nombre, cantidad de
   personas que lo pidieron y fecha del último pedido. Como `revisados`, no comparte historia con
   `master` y no se une nunca; cada pedido es un commit ahí y no en las traducciones.
4. **El graficador:** un comando nuevo del builder, `LoadFoldersBuilder -solicitudes`, lee
   `Solicitudes.tsv` y escribe `docs/solicitudes.html`: un gráfico de barras con los 20 más pedidos, de
   mayor a menor, con el enlace al Workshop de cada uno. Va en el builder y no en un script aparte
   (regla 4 de `AGENTS.md`). El action lo publica en GitHub Pages sin commitear, como la página diaria,
   en `https://lordmordelon.github.io/RML/solicitudes.html`, con un enlace desde la página de mods.
5. **Cuando se traduce un mod pedido**, el action lo saca del gráfico solo: al rearmarlo, se salta los
   que ya tienen carpeta en `Data/`.
6. **Un mod pedido que no tiene nada que traducir** (pasó con Simple Smoke for Combat Extended el
   23-oct-2026: la extracción dejó solo el yaml). Angel cierra el issue con la etiqueta `sin-texto`, y el
   action:
   - lo anota en `SinTexto.tsv`, en la misma rama `solicitudes` (WorkshopID, packageId, nombre, fecha);
   - lo saca del gráfico;
   - a los pedidos siguientes de ese mod les responde solo que no tiene texto para traducir y los cierra.

   El builder lee `SinTexto.tsv` de `origin/solicitudes` (como `Revisados.tsv`) y lo copia a `output/`,
   así que en la ventana del juego ese mod sale con el estado **«Sin texto»** y la columna «Traducido por»
   dice «No necesita traducción», sin botón «Solicitar». Si el mod se actualiza y agrega texto, Angel
   saca la fila a mano y vuelve a quedar «Sin traducir».

Cuidados:

- **Cuenta personas, no issues:** una misma cuenta de GitHub que pide el mismo mod diez veces suma uno.
  Así nadie infla el gráfico abriendo issues repetidos.
- **El texto del issue lo escribe cualquiera:** el action lo trata como dato, nunca lo pasa a una orden
  de la consola, y solo acepta la línea con el formato fijo.
- **Actúa con issues de cualquier usuario**, a diferencia de `revisado.yml`, que solo acepta los del dueño.
  Si aparece abuso, se puede limitar a cuentas con cierta antigüedad.
- No necesita la ventana del juego para funcionar: también se puede pedir abriendo el issue a mano. Se
  puede hacer antes o después de las etapas 2 y 3.

## Etapa 5: ver qué cambió en cada mod

**Aprobada por Angel el 24-oct-2026.**

**Decisiones de Angel (24-oct):** la pestaña «Novedades» dice que un mod cambió, pero no qué. Angel pidió
ver los textos, al menos de los corregidos y actualizados, y eligió:

- **Contra qué se compara:** contra la subida anterior de RML al Workshop, la misma que ya usa la sexta
  columna de `Novedades.tsv`. No solo el último cambio, que dejaría afuera una corrección si hubo dos
  seguidas.
- **Cómo se ve:** un botón «Ver cambios» en cada fila «Actualizado» o «Corregido», que abre una ventana
  aparte. Arriba, el resumen («3 corregidos · 12 nuevos · 1 quitado»); debajo, cada texto con su original
  en inglés, cómo estaba y cómo quedó. Los «Añadido» no llevan botón: ahí cambió todo.
- **Cuántos:** hasta 200 textos por mod, primero los corregidos, después los nuevos y los quitados, y un
  «y N más» si quedan. Así el archivo no crece de más con una actualización grande (Hegemony).
- **Colores, como el diff de VS Code:** «Antes» en rojo y «Ahora» en verde, con un fondo suave del mismo
  color detrás de cada uno. Un texto nuevo solo tiene la línea verde; uno quitado, solo la roja. El inglés
  va en gris. Los dos colores tienen que leerse bien sobre el fondo oscuro de la ventana del juego.
- **Ya no se puede apagar:** tenía una casilla en los ajustes del mod, «Mostrar los cambios de cada
  traducción», que Angel pidió sacar el 25-oct-2026 (ver «Los ajustes, como pestaña»).
- **Rendimiento:** aun encendida, no pesa al cargar el juego. El archivo se lee recién al apretar «Ver
  cambios», y solo la parte de ese mod; la ventana dibuja solo los renglones que están a la vista, como
  ya hace la pestaña «Novedades».

Cómo se arma:

- **El builder** lo arma en `CopiaLimpia`, junto a `Novedades.tsv` y con la misma base: el último commit
  anterior a la subida de RML. Para cada carpeta marcada «c», lee los XML de `Languages/` y `Patches/` en
  esa base (`git show`) y los de ahora, y los cruza clave por clave. **Corregido** = la clave tenía
  traducción y cambió; **nuevo** = no estaba o era `TODO` y ahora tiene texto; **quitado** = estaba
  traducida y ya no está (casi siempre pasó a `UNUSED.xml`). El inglés sale del comentario `<!-- EN: -->`
  de `Data/`, que la copia de `output/` no tiene. Lo deja en un archivo nuevo, solo en `output/` y sin
  commitear, como `Novedades.tsv`.
- **El juego** lo lee en `Logica/` (con pruebas, sin el juego) y lo muestra en la ventana nueva, con el
  estilo de la ventana actual. Los textos van en `Keyed`, en español y en inglés. La casilla nueva va en
  `AjustesRml` con su propio nombre (`Scribe_Values.Look(ref …, "verCambios", true)`): a quien ya tiene
  ajustes guardados le aparece encendida.
- **Sin conexión con Steam** no se sabe cuál fue la subida anterior: no hay detalle y el botón no aparece,
  igual que hoy esa pestaña cae a los últimos 30 días.

Para revisar antes de subir: las pruebas del builder y de `Logica/`, y una captura de la ventana con un
mod real cambiado (por ejemplo, uno de los que la columna marca hoy).

## Etapa 5: hecha en local (24-oct-2026)

- **El builder** (`CambiosDeTexto.cs`, con 12 pruebas en el proyecto nuevo `Source/LoadFoldersBuilder.Tests`,
  que la CI corre en `loadfolders.yml`). `Novedades.Armar` devuelve los dos archivos: la séptima columna de
  `Novedades.tsv` («Textos») trae la cuenta «cambiados,nuevos,quitados» de cada mod marcado «c», y
  `Cambios.tsv` (PackageID, tipo c/n/q, inglés, antes, ahora) trae hasta 200 por mod. Lo de antes se lee de
  la base con un solo `git cat-file --batch` (`UltimosCambios.Blobs`), no con un proceso por archivo.
- **Se cruza por carpeta, no por archivo:** la clave lleva su grupo (`Keyed`, `DefInjected/ThingDef`, o
  el xpath en `Patches/`), así que una clave que el extractor pasó a otro archivo no sale como cambio.
  Cada `<li>` de una lista va aparte, con el `<li>` del mismo número del comentario `EN:`. El mismo cambio
  en `1.6/` y `1.7/` sale una vez, y un inglés «TODO» (hay mods que lo traen así) se muestra vacío.
- **Lo de un mod que el Agrupador movió** se reconoce por el nombre de su carpeta, también para la sexta
  columna: antes un mod movido y cambiado podía no marcarse.
- **El juego:** `CambiosDeMod` (Logica, con pruebas) lee solo las líneas del mod y convierte el `\n`
  literal en salto. `VentanaCambios` dibuja solo los renglones a la vista.
- **Palabras resaltadas, como VS Code (Angel, 25-oct):** el renglón lleva el fondo suave y las palabras
  que cambiaron, el mismo color más fuerte. `DiffDePalabras` (Logica, con pruebas) compara palabra por
  palabra (subsecuencia común más larga; arriba de un millón de celdas marca el texto entero) y une los
  espacios entre dos palabras cambiadas. La ventana corta los renglones ella misma, dibuja cada uno
  entero y mide el resalte sobre el renglón hasta esa palabra, sin el margen que el juego suma a cada
  medida. Las etiquetas `<color>`, `<b>`, `<i>` y `<size>` se sacan antes: partidas se verían tal cual.
- **Con los datos de hoy:** 7 mods con detalle, 108 textos, 41 KB; armar la copia tarda unos 13 s. El
  resumen dice «cambiados» y no «corregidos», porque también aparece en las filas «Actualizado».

**Falta (Angel):** con el juego cerrado, rearmar la copia (`02-armar-copia-limpia.cmd`), abrir el juego y
mirar «Ver cambios» en un mod, por ejemplo The Dead Man's Switch.

## «Ver cambios» más claro (aprobado por Angel el 25-oct-2026, hecho el mismo día)

Lo pidió Angel mirando «Ver cambios» de [FSF] Advanced Bionics Expansion: la traducción solo había
ganado «proporciona una recirculación limitada del aire y», y la ventana mostraba el bloque rojo entero
sin que se hubiera quitado nada. Y el inglés salía sin resaltar, aunque el cambio venía de ahí: el autor
del mod había agregado «provides limited rebreathing and».

1. **Solo agregado o solo quitado: un bloque.** Si `DiffDePalabras.Comparar` no marca nada en el
   «antes», se muestra solo el verde con lo agregado resaltado; si no marca nada en el «ahora», solo el
   rojo con lo quitado resaltado. Si hay de los dos, los dos bloques, como ahora.
2. **El inglés también con su diff.** `CambiosDeTexto.cs` (builder) ya tiene el inglés viejo
   (`Viejo.Ingles`) y hoy guarda solo el nuevo. Agregar una sexta columna a `Cambios.tsv` con el inglés
   de antes; `CambiosDeMod.Leer` la lee si está, para que un `Cambios.tsv` viejo siga sirviendo. En la
   ventana, si el inglés cambió, se resalta igual que la traducción (palabras agregadas y quitadas); si
   no cambió, queda como ahora, y eso dice que fue una corrección de la traducción.
3. **Lo mismo en la traducción oficial** (PLAN-TRADUCCION-OFICIAL.md): `TraduccionOficial.Comparar`
   tiene el inglés viejo en `Anterior.Ingles`, y `TraduccionOficial.Linea` escribe el mismo formato.

Pruebas primero en `RmlVentana.Tests` (lectura con y sin la sexta columna, cuándo va un bloque o dos) y
en `LoadFoldersBuilder.Tests` (la columna nueva en `Cambios.tsv`).

Abajo a la izquierda, «Ajustar líneas (Alt+Z)», como en VS Code (pedido por Angel el 25-oct-2026):
desactivado, cada texto va en una línea, con los saltos escritos «\n» como en el archivo
(`DiffDePalabras.EnUnaLinea`), y la lista se desplaza hacia el costado. Se guarda en
`AjustesRml.CambiosEnUnaLinea`.

Al lado, «Agrupar por categoría» (aprobado por Angel el 25-oct-2026): los textos salen bajo un título
por categoría, con cuántos tiene, en este orden: Armas, Torretas, Construcciones, Ropa y armaduras,
Criaturas, Plantas, Objetos, Investigación, Recetas, Salud, Escenarios, Interfaz y mensajes, Otros. Para
eso `Cambios.tsv` lleva una séptima columna con el objeto de cada texto («ThingDef/Gun_LMG», «Keyed»;
`CambiosDeTexto.ObjetoDe`, y `Categorias.ObjetoDeClaveOficial` para la traducción oficial). Lo que no es
un ThingDef se reconoce por su tipo (`Categorias.PorTipo`); un ThingDef, o una clase hija como el
`AmmoDef` de Combat Extended, lo clasifica el juego mirando el def cargado
(`VentanaCambios.CategoriaDe`), así que con su mod inactivo va a «Otros». Se guarda en
`AjustesRml.CambiosPorCategoria`. Cada título se pliega y se despliega con un clic, con la flecha del juego (▶ cerrado, ▼ abierto), como las listas plegables de otros mods; arrancan abiertos.

Al volver a abrir la ventana, «Novedades» ya no queda vacía: si se vio todo, muestra lo de la última
actualización de RML con el aviso «Ya viste estos cambios» (`NovedadesRml.ParaMostrar`), y eso no hace
aparecer la ventana sola.

### Lo mismo al volver a abrirla, y el historial (pedido por Angel el 25-oct-2026)

Si se actualizó un solo mod, al cerrar la ventana y volver a abrirla desde los ajustes tiene que seguir
ese solo. Al cerrarla con algo nuevo se guarda qué se mostró (`AjustesRml.NovedadesMostradas`,
`NovedadesRml.ClavesDe`), y con todo visto la pestaña vuelve a mostrar eso (`NovedadesRml.ParaMostrar`).

La casilla «Ver todo el historial», a la derecha del resumen, muestra todos los mods de RML por la fecha
de su último cambio (`NovedadesRml.Historial` y `PorFecha`), con un título plegable por fecha; abre con la
más nueva desplegada. «Ver cambios» sigue solo en los mods de la última subida, que son los que
tiene `Cambios.tsv`.

## Los ajustes, como pestaña (pedido por Angel el 25-oct-2026, hecho)

Los ajustes del mod eran una pantalla aparte (Opciones → Configuración de mods). Ahora son la tercera
pestaña de la ventana, «Ajustes», con la sección de la traducción oficial (`VentanaTraducciones.DibujarOficial`).
Desde la configuración de mods del juego se abre la ventana en esa pestaña y se cierra la pantalla del
juego, que quedaría vacía detrás. En 1.6 no hay traducción oficial que bajar, así que no hay pestaña y la
ventana abre en «Novedades».

Se sacaron dos opciones: «Mostrar la ventana al iniciar», que repetía la casilla «No volver a mostrar»
del pie, y «Mostrar los cambios de cada traducción»: «Ver cambios» sale siempre que hay detalle. El
aviso de arreglos de «Novedades» lleva a la pestaña en vez de abrir otra pantalla.

## Cómo se prueba

- Etapa 1: que `LoadFoldersBuilder -rutas` y `-build` sigan en verde y que la copia de `output/` tenga la
  DLL. Angel abre el juego y confirma que no hay errores nuevos en el log.
- Etapas 2 y 3: la lógica (clasificar mods, leer novedades) va separada de la ventana, para probarla con
  tests sin abrir el juego. La ventana la revisa Angel en el juego, con capturas.

## Riesgos

- **Un error en la DLL se ve en el log de todos los jugadores.** Todo lo que haga va dentro de un `try`,
  y si algo falla la ventana simplemente no aparece.
- **Rutas largas:** la carpeta `Common/` es corta y no se acerca al límite de 260 caracteres.
- **Workshop:** la primera subida con DLL la hace Angel con el `03`, como siempre.

## Etapa 1: hecha en local (23-oct-2026)

**Decisiones de Angel (23-oct):** la carpeta se llama `Common`, como en los demás mods (primero se eligió `Comun`), y va una sola DLL para 1.6 y 1.7, compilada
contra 1.6 (`Krafs.Rimworld.Ref` 1.6.4871). Si en 1.7 falla, se separa por versión.

Lo que quedó:
- `Source/RmlVentana`: .NET Framework 4.7.2, una clase `Mod` que deja «[Traducciones [Español Latino]]
  Cargado.» en el log. Compila a `Common/Assemblies/RmlVentana.dll` (ignorada por git), sin nada más al lado.
- El builder la compila (`Dll.cs`) en `-build` (si falla, solo avisa) y en `-copia` (si falla, sale con 1 y
  `release.yml` no publica). La compilación es determinista: dos compilaciones desde cero dan los mismos
  bytes, así que no dispara releases vacías.
- `LoadFolders.xml` lleva `<li>Common</li>` arriba de cada versión, sin `IfModActive`. `Common/` entra en
  `CarpetasDeOrigen`, así que va a `output/` y `-rutas` la revisa, también por nombres repetidos.
- `loadfolders.yml` corre también con cambios en `Common/**` y `Source/RmlVentana/**`.

**Falta (Angel):** abrir RimWorld 1.7 con RML y mirar en el log que salga la línea «Cargado.» y ningún error
nuevo. Si se puede, lo mismo en 1.6. Después, decir si se sube.

Angel la probó en 1.7 el 23-oct: sale la línea en el log y ningún error nuevo.

## Etapa 2: hecha en local (23-oct-2026)

- **La lógica** está en `Source/RmlVentana/Logica/`, sin tipos del juego, con pruebas en
  `Source/RmlVentana.Tests` (la CI las corre antes de regenerar). `Clasificador` decide el estado de cada
  mod y si la ventana aparece sola; `Solicitudes` arma el enlace del issue y las fechas.
- **Lo que lee del juego** (`LecturaDelJuego`): lo que traduce RML sale de los `IfModActive` de sus
  carpetas de `Data/` en el `LoadFolders.xml` ya leído. Otra traducción: una carpeta
  `Languages/SpanishLatin` (o el `.tar`) en lo que el juego carga del mod. Un mod sin `Defs` ni
  `Assemblies` que trae español es un **paquete de traducción**: apunta a lo que traduce con `IfModActive`
  o, si su carpeta no tiene condición, con el `modDependencies`, `loadAfter` y `forceLoadAfter` de su
  `About.xml`. Los paquetes no salen en la tabla, salvo que RML los traduzca.
- **La ventana** (`VentanaTraducciones`) sigue la previsualización con los controles del juego: pestaña
  «Tus mods» (la de «Novedades» llega en la etapa 3), buscador, tabla, «NUEVO», botón «Solicitar», nota y
  «No volver a mostrar». Los textos están en `Common/Languages/{SpanishLatin,English}/Keyed/`.
- **Aparece sola** (`AlIniciar`) al terminar de cargar, si el juego está en español latinoamericano y hay
  un mod nuevo desde la última vez, o si nunca se abrió. Se abre a mano desde Opciones → Configuración de
  mods → «Traducciones [Español Latino]».
- **El builder** ya no corta la copia si el juego tiene tomada la DLL: deja la anterior, copia lo demás y
  avisa. `-copia` sale con 1 en ese caso, para no subir al Workshop una copia a medias.
- Todo lo que usa del juego se comprobó contra el `Assembly-CSharp.dll` de 1.7.

- **Al iniciar** va en un evento largo propio (`LongEventHandler.QueueLongEvent`) y no en
  `ExecuteWhenFinished`: así se abría antes del evento «InitializingInterface», con `Find.WindowStack` en
  null (0470d29a). Con el modo desarrollador deja en el log por qué apareció o no (13729ea4).

Angel la probó en 1.7 el 23-oct: los estados de sus mods están bien y aparece sola al iniciar.
**Pendiente para la etapa 4:** crear la etiqueta `solicitud` en el repositorio de RML. Sin ella, GitHub
abre el issue igual, pero sin etiqueta.

## Etapa 3: hecha en local (23-oct-2026)

**Decisiones de Angel (23-oct):** `Novedades.tsv` va solo en `output/`, sin commitearse. La primera vez se
muestra **solo lo de la última actualización de RML**; después, todo lo que el jugador no vio.

- **El builder** arma `Novedades.tsv` en `CopiaLimpia` (o sea, en `-build` y `-copia`), con
  `Novedades.cs`: una fila por mod de `Data/` con su packageId, nombre, la fecha en que apareció su
  traducción, la de su último cambio y el tipo de ese cambio. Las fechas salen de
  `UltimosCambios.Historial` (la misma pasada por git que ya daba la última fecha; ahora guarda todas, sin
  contar los movimientos del Agrupador). El nombre y la fecha de Steam salen de `ModList.tsv`.
  **actualizado** = el cambio anterior es de antes de la última actualización del mod en Steam y el
  último es de después; si no, **corregido**. Steam solo da su última fecha, así que solo se clasifica el
  último cambio, y es aproximado.
- **El juego** (`NovedadesRml`, con pruebas) muestra lo que no se vio: guarda al cerrar una clave por
  fecha de cada mod (`NovedadesVistas`) y la fecha de cierre (`UltimaVez`, para «Cambios desde el…»). Un mod
  que el jugador no vio añadido sale como «Añadido» aunque después haya cambiado. Con algo sin ver, la
  ventana también aparece sola.
- **La ventana** tiene la pestaña «Novedades» primero y abre siempre en ella: **Mod** | **Cambio** |
  **Fecha**, con los colores de la previsualización. El buscador queda solo en «Tus mods».
- **La última actualización** es la del Workshop, no la de GitHub (que publica una versión en cada push,
  nueve el 22-oct). El builder pide a Steam el momento de la última subida de RML (3541009729), toma el
  último commit anterior y marca en una sexta columna lo que cambió desde ahí, sin commitear incluido: «a»
  añadido, «c» cambiado. Al armar la copia para subir, esa subida es la anterior. Sin conexión con Steam no
  va la columna y el juego muestra los últimos 30 días. Hoy marca 7 mods (con 30 días eran 116).
- **Cuándo aparece sola (Angel, 24-oct):** la primera vez, solo si la última actualización de RML trajo
  cambios; si no, no se abre. Después, si hay cambios sin ver (de todas las actualizaciones que el jugador
  se haya saltado: 3 de una y 5 de otra suman 8) o un mod nuevo en su lista.

Angel la probó en 1.7 el 24-oct: aparece sola la primera vez con la última actualización y guarda lo visto.
Pidió dejar el nombre en Configuración de mods como está, aunque se corte («Traducciones [Español La...»).

## Etapa 4: hecha en local (24-oct-2026)

Angel subió las etapas 1 a 3 antes de empezarla (24-oct, `b0ae6370`). Cambios respecto de lo planeado:

- **Un solo archivo de pedidos**, `Pedidos.tsv` (WorkshopID, packageId, nombre, usuario, fecha), con una
  línea por mod y persona. No hay `Solicitudes.tsv`: la cantidad de personas se cuenta al leerlo.
- **El nombre sale de Steam**, no del título del issue, que lo escribe cualquiera. Steam además
  confirma que el número es un mod de RimWorld; si no lo es, el pedido no se cuenta.
- **No hay comando `-solicitudes`**: `-pagina` y `-build` arman `docs/solicitudes.html` junto con la
  lista (`Solicitudes.cs`). No se commitea, y `pages.yml` pasó a llamar a `pagina-diaria.yml` para
  que publicar la lista no borre el gráfico.
- **El issue se reconoce por el título** «Solicitud:», no por la etiqueta: GitHub ignora la etiqueta
  del enlace si quien abre el issue no puede etiquetar. El flujo la pone después.

Lo que quedó: `solicitud.yml`, `Solicitudes.cs`, el enlace desde la lista de mods, `SinTexto.tsv` en
`output/` (lo trae también `release.yml`) y el estado `SinTexto` en `Clasificador`, con 4 pruebas. El
script del flujo se probó en local con GitHub y Steam simulados: pedido nuevo, repetido, de otra
persona, ya traducido, sin la línea, número que Steam no conoce, `sin-texto` y pedido de un mod sin
texto. La página se revisó con capturas en ancho de escritorio y de celular.

## Subida y probada en GitHub (24-oct-2026)

Angel autorizó subirla y probarla. Están creadas las etiquetas `solicitud` y `sin-texto`. El pedido de
prueba fue el issue #6, Harmony (2009463077), que de verdad no tiene texto: se anotó con el puesto 1,
salió en `solicitudes.html` y, con la etiqueta `sin-texto`, pasó a `SinTexto.tsv`, se cerró y salió del
gráfico. Las cuatro corridas quedaron en verde.

**Falta (Angel):** probar en el juego, con la próxima copia de `output/`, que Harmony sale «Sin texto» |
«No necesita traducción», sin botón «Solicitar».
