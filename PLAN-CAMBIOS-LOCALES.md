# Plan: «Ver cambios» contra la versión que había en el PC

**Estado: borrador del 25-oct-2026, falta el «Aprobado» de Angel.** Es la etapa 6 de la ventana
([PLAN-VENTANA.md](PLAN-VENTANA.md)). No se escribe código hasta la aprobación, que se anota acá.

## Qué se quiere

Hoy «Ver cambios» compara cada mod contra **la última subida de RML al Workshop**: el builder arma
`Cambios.tsv` con ese diff y la ventana lo muestra. Sirve bien a quien juega con el Workshop, pero no
a los demás:

- **Quien baja RML de GitHub** recibe una versión con cada push, mucho más seguido que el Workshop, y
  «Ver cambios» le muestra cambios que ya tenía de una versión anterior de GitHub.
- **Quien actualiza a mano** (copiando la carpeta, o el mantenedor probando en su PC) tampoco ve lo
  que cambió para él: ve lo que cambió para el Workshop.
- **Un mod añadido que después se corrige** necesitó un caso aparte en el builder (`AntesDelDia`), y
  aun así es una aproximación por día.

Angel pidió (25-oct-2026) que compare **contra la versión que el jugador tenía en su PC**, sin
importar de dónde la bajó ni cómo actualizó.

## Hoy y con el plan

| Situación | Hoy | Con el plan |
|---|---|---|
| Juega con el Workshop | «Ver cambios» muestra lo que cambió desde la última subida al Workshop, que es lo que tenía | Lo mismo: lo que tenía era la versión del Workshop |
| Baja RML de GitHub en cada versión | Muestra lo que cambió desde el Workshop, aunque ya lo tuviera de una versión anterior de GitHub | Solo lo que cambió desde la versión que tenía en su PC |
| Actualiza a mano o prueba en su PC (Angel) | Lo mismo que el Workshop; un mod añadido y corregido después necesita el caso aparte por día | Exactamente lo que cambió desde la última vez que abrió la ventana |
| Pasan dos subidas sin que juegue | Solo ve lo de la última subida: lo de la anterior ya no está en `Cambios.tsv` | Ve todo lo que cambió desde la última vez, las dos subidas juntas |
| Recién instala RML | Lo de la última actualización de RML | Lo mismo la primera vez; desde ahí, contra su PC |
| Un mod que no tiene activo | Sale en «Novedades» con el «Ver cambios» del builder | Igual: sin copia local, usa lo del builder |
| Un mod añadido | Todos sus textos como nuevos, hasta 200 | Igual |
| El inglés | Del comentario `EN:` del archivo de RML | Igual, del comentario `EN:`, que el builder lleva a `output/` en `Ingles.tsv` |
| Textos que caben | Hasta 200 por mod (el tope del builder) | Sin tope: se calcula en el PC, solo de los mods que cambiaron |

## Cómo funcionaría

1. **Una copia de los textos que el jugador tenía.** El juego guarda, en su carpeta de
   configuración, los textos de RML de cada mod tal como estaban la última vez que se cerró la
   ventana: `Config/RML_Textos/<packageId>.tsv`, una línea por texto (clave, traducción e inglés),
   con una huella de los archivos al principio (ruta, tamaño y fecha de cada XML del mod).
2. **Al iniciar, en segundo plano**, RML recorre sus carpetas de mod que el juego cargó y compara la
   huella de cada una con la guardada. Solo las que cambiaron se leen enteras y se comparan texto por
   texto con su copia. El resultado es un `Cambios.tsv` local, con el mismo formato, en la carpeta de
   configuración.
3. **«Ver cambios» usa el local si existe** para ese mod, y si no, el `Cambios.tsv` del builder. Las
   filas de «Novedades» (qué mod, qué tipo, qué fecha) siguen saliendo de `Novedades.tsv`, que no
   cambia.
4. **Al cerrar la ventana**, la copia de los mods mostrados se pone al día con lo que hay ahora, para
   que la próxima vez compare contra esto. Como con `NovedadesVistas`.
5. **La primera vez no hay copia**: se guarda y la ventana muestra lo del builder, como hoy. Desde la
   siguiente actualización ya compara contra el PC.

## Decisiones

- **El inglés sale del comentario `EN:` del archivo de traducción, como hoy** (Angel, 25-oct-2026).
  Se descartó pedírselo al juego (`DefInjectionPackage.DefInjection.replacedString` y
  `LanguageDatabase.defaultLanguage.keyedReplacements`, que existen en la 1.7): Angel prefiere el
  mismo origen que ya usa el builder, y así los `Patches/` también tienen su inglés.
  - La copia que carga el juego (`output/`) no trae los comentarios, para pesar menos. Por eso
    `CopiaLimpia` escribe además `Ingles.tsv` en la raíz de `output/`: una línea por texto con
    packageId, clave e inglés, el de `CambiosDeTexto.Leer`. No se commitea, como `Cambios.tsv`.
  - Medido el 25-oct-2026: el inglés de los comentarios de `Data/` son unos 7 MB sin comprimir,
    sobre los 15 MB de `output/`. Comprimido para el Workshop o la release, alrededor de 1,5 MB más.
  - El juego lo lee línea por línea y solo las de los mods que cambiaron, como hoy `Cambios.tsv`.
  - Se descartó dejar los comentarios dentro de los XML de `output/`: pesa lo mismo y el juego los
    recorre al cargar cada archivo.
  - Los saltos de `\n` y las listas (`li`) se guardan igual que en la traducción, y las entidades
    (`&apos;`) ya decodificadas, como en `Cambios.tsv`.
- **Solo los mods activos** (propuesta, a confirmar por Angel). Guardar todos los mods de RML serían
  varios MB en la configuración y más trabajo al iniciar. Con los activos queda chico y rápido. Un mod
  que se activa después empieza a comparar desde ese momento, y mientras tanto usa lo del builder.
- **Las carpetas son las que el juego cargó** (`ModContentPack.foldersToLoadDescendingOrder` de RML),
  no todo `Data/`: así la copia sigue la versión del juego (1.6 o 1.7) y solo los mods presentes.
  También comprobado en la 1.7.
- **El mismo lector de textos en los dos lados.** `CambiosDeTexto.Leer` y `Comparar` viven hoy en el
  builder (.NET 10). Se pasan a `Source/RmlVentana/Logica/` (sin tipos del juego, .NET Framework
  4.7.2) y el builder los compila con un enlace al mismo archivo, como hacen hoy las pruebas. Así la
  clave de un texto es la misma en el `Cambios.tsv` del builder y en el local.
- **No se toca `Novedades.tsv`** ni la lógica de qué filas salen (`NovedadesRml`): cambia solo de
  dónde sale el detalle.

## Riesgos y cómo se cubren

- **Tiempo al iniciar.** RML tiene unos 3600 XML. Leer solo los mods activos, solo si cambió su
  huella, y en un hilo aparte (`Task.Run`, como la consulta a GitHub), no en el de la pantalla. Si la
  ventana se abre antes de que termine, usa lo del builder.
- **Un error al leer no puede romper el juego**: todo en `try`, y un mod que falla se salta, como el
  resto de la DLL.
- **El tamaño de la copia**: medirlo con los mods activos de Angel antes de cerrar la tarea. Si pasa de
  unos pocos MB, guardar solo las claves con su texto y no el inglés: el inglés de ahora sale de
  `Ingles.tsv`, y el de antes solo hace falta si cambió.
- **Rutas largas**: los archivos de la copia van por packageId, no por el nombre de la carpeta.

## Etapas

| # | Qué | Pruebas |
|---|---|---|
| 1 | Pasar `Leer`/`Comparar` a `Logica/` (`TextosDeTraduccion.cs`), con el builder enlazándolo; `CopiaLimpia` escribe `Ingles.tsv` | Las de `CambiosDeTextoTests` siguen en verde, ahora contra `Logica/`; la línea de `Ingles.tsv` y su lectura |
| 2 | La copia: escribir y leer `RML_Textos/<packageId>.tsv` con su huella (`Logica/CopiaDeTextos.cs`) | Escribir, leer, huella igual y distinta, archivo roto |
| 3 | El diff local: copia contra textos de ahora → líneas de `Cambios.tsv` | Cambiado, nuevo, quitado, inglés que cambió, mod sin copia |
| 4 | En el juego: recorrer las carpetas cargadas en segundo plano, tomar el inglés de `Ingles.tsv`, armar el `Cambios.tsv` local | En el juego: cambiar un texto de un mod activo, reiniciar y verlo en «Ver cambios» |
| 5 | La ventana: usar el local si existe; poner al día la copia al cerrar | Cerrar, reabrir y que ya no salga; el de GitHub y el del Workshop dan lo mismo |

Pruebas primero en `RmlVentana.Tests` para las etapas 2 y 3. Costo estimado: una sesión.

## Cómo lo prueba Angel

1. Con el juego cerrado, cambiar a mano un texto de un mod activo en `output/` (o reextraer uno) y
   abrir el juego: ese mod tiene que salir en «Novedades» con «Ver cambios» mostrando solo ese texto,
   con su inglés en gris.
2. Cerrar la ventana, reiniciar: ya no sale como novedad, y «Ver todo el historial» lo sigue mostrando.
3. Un mod que no se tocó no tiene que aparecer, aunque el builder lo tenga en su `Cambios.tsv`.
