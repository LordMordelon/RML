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

## Próxima sesión (23-oct-2026)

**Objetivo:** etapa 3, la pestaña «Novedades» y el `Novedades.tsv` que la alimenta.
**Estado:** etapas 1 y 2 hechas y probadas en 1.7, en `master` de RML **sin subir** (último commit 13729ea4;
8505bb85 es de otra sesión). Angel todavía no autorizó el push.
**Antes de empezarla (Angel):** nada.
**Al empezar (Claude):**
1. Builder: `Novedades.tsv` en `-build`, con la historia de git de cada carpeta de `Data/` (ver «Cómo sabe
   cada cosa»): añadido, actualizado o corregido, con la fecha. Para «actualizado» hace falta la fecha de
   Steam de cada mod: la consulta ya está en `ActualizacionesSteam.Consultar`. Va a `output/` por `CopiaLimpia`.
2. `Logica/`: leer el TSV y quedarse con lo posterior a la última vez que se abrió la ventana. Pruebas
   primero, en `Source/RmlVentana.Tests`.
3. Ajustes: guardar la fecha de la última vez que se abrió. La ventana aparece sola también si hay
   novedades de RML desde esa fecha.
4. Ventana: pestaña «Novedades» antes de «Tus mods», y abre siempre en ella. Tabla **Mod** | **Cambio**
   (etiquetas «Añadido», «Actualizado», «Corregido», colores de la previsualización) | **Fecha**, y el
   resumen «Cambios desde el <fecha>, la última vez que abriste esta ventana». Textos en los dos `Keyed`.
5. Comprobar cada función nueva del juego contra el `Assembly-CSharp.dll` de 1.7 con `ilspycmd` (ya
   instalado), como en la etapa 2: 1.7 corre en otro motor y un método que falte no avisa al compilar.
6. Unos 8 archivos (estimado): cabe en una sesión.

**Decisiones de Angel:**
- ¿`Novedades.tsv` se commitea, como `ModList.tsv`, o solo se escribe en `output/`? Recomiendo solo en
  `output/`: sale entero de la historia de git, y commitearlo sumaría un cambio en cada push. La release ya
  clona con la historia completa.
- La primera vez que se abre (sin fecha anterior), ¿cuánto muestra? Recomiendo los últimos 30 días.

**Guías:** `frontend-design` para la pestaña nueva.
**Dónde leer:** este plan («Cómo sabe cada cosa» y esta nota); `Source/RmlVentana/`;
`Source/LoadFoldersBuilder/UltimosCambios.cs` (fecha del último commit de cada carpeta, reutilizable) y
`ActualizacionesSteam.cs`; la previsualización en https://claude.ai/artifact/WfNYPCKyZAR5RnkJAtcxZb.
