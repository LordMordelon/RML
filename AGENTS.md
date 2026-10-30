# Notas para agentes — RimWorld Mod Latino (RML)

Contexto para trabajar en este repositorio sin tener que redescubrirlo. El
[README](README.md) es corto a propósito (Angel pidió dejar solo lo importante): qué es,
cómo instalarlo y los pasos del mantenedor. Lo demás va acá.

## Qué es esto

Mod de RimWorld que reúne traducciones al **español latinoamericano** de otros mods.
Una carpeta por mod bajo `Data/`, y un `LoadFolders.xml` generado que hace que RimWorld
cargue cada traducción solo si ese mod está presente. Arquitectura derivada de
[RMK](https://github.com/RimWorldKorea/RMK); lo que cambia está en
[CAMBIOS.md](CAMBIOS.md).

Las traducciones **no se escriben a mano desde cero**: las produce el extractor, que vive
como repositorio hermano.

```
<carpeta de trabajo>/
  RML/                  <- este repo
  RimworldExtractor/    <- https://github.com/LordMordelon/RimworldExtractor
```

Ese repositorio tiene su propio `AGENTS.md`. Si el trabajo toca cómo se generan los
archivos —y no solo su contenido— el cambio va allá, no acá.

## Estructura

```
About/About.xml                 metadatos del mod. supportedVersions: 1.6 y 1.7. Su forceLoadAfter es GENERADO
Data/
  !<Autor>/                     autores con 4 o más mods; el ! los fija arriba
    <Nombre del mod> - <ID>/
  <Nombre del mod> - <ID>/      los demás, sueltos
Ajustes/
  <Nombre del mod> - <ID>/      cambios a mano que le ganan a Data/; el extractor no los toca (ver «Ajustes»)
Common/                          se carga siempre, sin IfModActive, arriba de cada versión de LoadFolders.xml
  Assemblies/RmlVentana.dll     GENERADA — la compila el builder desde Source/RmlVentana; no se commitea
  Languages/<idioma>/Keyed/     los textos de la ventana, en SpanishLatin y English. A mano: el extractor no recorre Common/
LoadFolders.xml                 GENERADO — no editar a mano
docs/index.html                 GENERADO — lista de mods como página web ordenable (GitHub Pages)
ModList.tsv                     GENERADO — la misma lista en texto plano: mods que cubre RML, fecha de actualización del mod y de la traducción
Novedades.tsv                   GENERADO solo en output/, sin commitear — lo que cambió en español en cada mod, para la ventana del juego (Novedades.cs, PLAN-SOLO-INGLES.md)
Cambios.tsv                     GENERADO solo en output/, sin commitear — los textos de cada mod antes y después de la última subida, para «Ver cambios» (TextosDeTraduccion.cs)
Ingles.tsv                      GENERADO solo en output/, sin commitear — el inglés de cada texto, que output/ no trae en comentarios, para comparar contra el PC (PLAN-CAMBIOS-LOCALES.md)
01-regenerar-indice.cmd         regenera los anteriores
02-armar-copia-limpia.cmd       deja al día output/, la copia liviana que carga el juego y se sube (el 01 ya lo hace)
03-subir-al-workshop.cmd        corre el 02, deja Mods\RML enlazado a output/ y sube al Workshop con SteamCMD
                                (LoadFoldersBuilder -workshop, SubidaWorkshop.cs), con la nota de cambios de
                                NotaDeSteam.cs: «Añadidos:» y los mods nuevos desde la última subida. Avisa lo sin
                                commitear y pregunta antes. Sin STEAMCMD y STEAM_USUARIO, copia la nota (-nota) y se
                                sube desde el juego. Solo en el PC de Angel, nunca en la CI
nota-de-steam.txt               GENERADO por el 03 sin SteamCMD, sin commitear — esa misma nota
LoadFolders.Build.Example.yaml  referencia de campos, con cada uno comentado
GLOSARIO.md                     terminología oficial de RimWorld ES
TRADUCIR.md                     guía para colaboradores: fork, extractor, traducir en VS Code y pull request
CAMBIOS.md                      qué cambia respecto de RMK
RESCATE-PENDIENTE.md            traducciones huérfanas y decisiones tomadas
Source/LoadFoldersBuilder/      genera LoadFolders.xml a partir de los .yaml
Source/FileNameEncoder/         normaliza nombres de XML que NO vengan del extractor
Source/RmlVentana/              la DLL de RML (.NET Framework 4.7.2, contra Krafs.Rimworld.Ref 1.6): la ventana de mods
                                traducidos y sus ajustes (PLAN-VENTANA.md), y el botón que baja la traducción oficial
                                de GitHub (PLAN-TRADUCCION-OFICIAL.md; solo en 1.7, toca Data/ del juego). Logica/ no usa tipos del juego.
                                «Ver cambios» compara contra la copia de los textos que guarda en Config/RML_Textos
                                (CambiosEnElPc.cs, PLAN-CAMBIOS-LOCALES.md); Logica/TextosDeTraduccion.cs lo compila también el builder
Source/RmlVentana.Tests/        las pruebas de Logica/, sin el juego (dotnet test; la CI las corre en loadfolders.yml)
Source/LoadFoldersBuilder.Tests/ las del builder que no necesitan git ni Steam: hoy, la comparación de Cambios.tsv
.github/workflows/              loadfolders.yml (índice en push), pages.yml (publica docs/ si cambió), release.yml (zip de output/ si cambió),
                                pagina-diaria.yml (la página con las fechas de Steam del día), revisado.yml (anota una revisión),
                                solicitud.yml (anota un pedido de traducción), herramientas.yml (las pruebas de Herramientas/)
Herramientas/traducir.py        el comando para traducir con tabla (PLAN-HERRAMIENTAS.md); no llega al juego ni dispara la CI del mod
Herramientas/oficial/           buscar en la traducción oficial del juego y cotejar un glosario; sus cachés (*.pkl) no se commitean
Herramientas/pruebas/           las pruebas del comando, sin el juego ni git
```

Y cada carpeta de mod, por dentro:

```
<Nombre del mod> - <ID>/
  Languages/SpanishLatin/
    DefInjected/  Keyed/  Strings/
  Patches/                    <- fuera de Languages, a la par
    <codigo>.xml              <- codificado: el nombre tiene que ser único en todo Data/
  LoadFolders.Build.yaml
  UNUSED.xml                  <- traducciones apartadas
  *.md                        <- notas de trabajo de una traducción grande (hoy, el plan y los dos glosarios
                                 de Combat Extended). El extractor no las toca y no van a output/
```

**`Patches/` va en la raíz de la carpeta del mod, nunca dentro de `Languages/`.** Es a
propósito: ahí adentro RimWorld lo cargaría como una traducción más, y las claves muertas
le llenarían el log de errores al jugador. Hoy hay 49 carpetas `Patches` y ninguna está
bajo `Languages`.

Agrupar por autor es solo para ordenar. El extractor busca la carpeta de un mod en
cualquier nivel bajo `Data/`, así que mover una no rompe nada. El `!` se usa porque el
guion bajo no alcanza: ordena después de las carpetas tipo `[FSF]`.

El umbral es de cuatro traducciones por autor, y lo mantiene el `Agrupador` del extractor:
un mod nuevo cae directo en `Data/!Autor/` si esa carpeta ya existe, y las sueltas de ese
autor se mueven ahí en la siguiente traducción rápida. Un autor que recién llega a cuatro
se avisa por log y no se mueve nada: la carpeta la crea una persona, y alcanza con crearla
vacía. El autor sale del `<author>`
del `About.xml` del mod instalado, no del `packageId` —los mods de Oskar Potocki usan
cuatro prefijos distintos—, y se normaliza antes de comparar.

## Ajustes

`Ajustes/` es para cuando una traducción correcta no sirve junto a otro mod. Nice Health Tab
muestra el `AllowFood` del juego seguido de «:» en una columna angosta, y «Permitir comida» se
cortaba: con ese mod activo, `Ajustes/Nice Health Tab - 3328729902/` lo cambiaba por «Comida».
El autor agregó su propia clave, `NHT_AllowFood`, el 22-oct-2026, y el ajuste se sacó. Hoy
hay uno: con Combat Extended, los botones de la pestaña Políticas van sin «Gestionar», porque su
botón «Equipamientos» los angosta. Cómo agregar un ajuste y cuándo sacarlo está en
[Ajustes/README.md](Ajustes/README.md).

- **Solo arreglos de espacio o de contexto, nunca para traducir.** Una traducción va en `Data/`,
  por el extractor. Lo que se pone acá le gana a todo lo demás y nadie lo revisa al reextraer.
- **Mismo formato que `Data/`:** una carpeta por mod con su `LoadFolders.Build.yaml`, copiado del
  de ese mod en `Data/` si lo tiene, y su `Languages/SpanishLatin/`. El `PackageID` del yaml decide
  con qué mod activo se carga.
- **El archivo se llama `Ajustes-<WorkshopID>.xml`.** Dos archivos con la misma ruta interna en
  dos carpetas de RML hacen que el juego cargue uno solo (ver «Cosas que ya pasaron»);
  `LoadFoldersBuilder -rutas` lo comprueba contra `Data/` y la CI queda en rojo si se repite.
- **Gana porque va arriba.** El builder pone sus entradas al principio de cada versión en
  `LoadFolders.xml`. RimWorld recorre esa lista de abajo hacia arriba
  (`ModContentPack.InitLoadFolders` la invierte) y en `Keyed` y `DefInjected` la clave leída al
  final pisa a la anterior, así que gana la carpeta de más arriba. En `DefInjected` el juego anota
  además un «Duplicate def-injected translation key» en el informe de traducción: se aplica igual.
- **El extractor no la ve:** `ActualizacionPorLotes` y el `Agrupador` solo recorren `Data/`. Sí
  van a `output/` (`CopiaLimpia`) y al `forceLoadAfter`, y no van a `ModList.tsv` ni a la página.

## Cómo se produce una traducción

1. **Extraer** el mod con el extractor, con idioma de destino
   `SpanishLatin (Español(Latinoamérica))`. La carpeta que escribe se llama
   `Languages/SpanishLatin`, sin el paréntesis, y no `Spanish`, que es el castellano.

   El nombre corto es a propósito. Con el largo, RML instalado desde el Workshop con Steam
   en su carpeta por defecto (74 caracteres de ruta base) tenía archivos de más de 260
   caracteres. RimWorld no los abre y queda en pantalla negra al cargar. En `Mods\RML` la
   ruta es más corta, así que probando en local no se ve. RimWorld acepta el nombre corto
   como nombre legado del idioma. `LoadFoldersBuilder -rutas` lo comprueba contra la ruta
   del Workshop, y `02-armar-copia-limpia.cmd` y la CI no dejan pasar una ruta larga ni un mod con las
   dos carpetas de idioma. Un extractor anterior a este cambio escribe con el nombre largo:
   no correrlo sobre este RML.
2. **Traducir**, reemplazando los `TODO` en el XML. La planilla `.xlsx` ya no existe: el
   extractor 1.0 la sacó, y [TRADUCIR.md](TRADUCIR.md) explica el camino sobre el XML.
3. **Regenerar el índice.** Lo hace `01-regenerar-indice.cmd`, y también el extractor al terminar
   una traducción rápida y la GitHub Action al hacer push.

Con «Extraer en carpeta» sin marcar, que es la traducción rápida, el extractor escribe
directamente en `Data/`, conserva lo ya traducido, genera el `LoadFolders.Build.yaml` y
regenera el índice. Después de una actualización del juego, «Actualizar todo RML» hace lo
mismo con todos los mods de una vez; los que no están instalados quedan como están.

## Reglas

1. **El glosario manda.** Revisar [GLOSARIO.md](GLOSARIO.md) antes de traducir cualquier
   texto. Tiene la terminología oficial de RimWorld en español. No inventar términos.

2. **No editar lo generado.** `LoadFolders.xml`, `ModList.tsv` y `docs/index.html` se rehacen enteros. Para
   cambiar algo, editar el `LoadFolders.Build.yaml` del mod y correr `01-regenerar-indice.cmd`.
   En `About/About.xml` lo generado es solo el `<forceLoadAfter>`, con el packageId de cada
   mod de `Data/` y de `Ajustes/`: el resto del archivo se edita a mano y el builder no lo toca.

   Los regeneran tanto el extractor como la CI, así que después de un push es normal
   que el remoto traiga un «Regenerar el indice» con lo mismo que acabas de generar en
   local. Para que traer cambios lo resuelva solo, configurar una vez por clon:

   ```sh
   git config pull.rebase true        # el commit repetido se descarta solo
   git config rebase.autoStash true   # sin frenar por cambios sin commitear
   git config merge.generado.driver true
   ```

   La última activa el `merge=generado` de `.gitattributes`: si lo generado difiere,
   queda la versión del remoto en lugar de un conflicto.

3. **No tocar el `UNUSED.xml`.** Guarda las traducciones cuyo nodo desapareció del mod.
   El extractor lo relee en cada actualización, así que si el mod devuelve el nodo a su
   lugar la traducción se recupera sola. Borrarlo pierde ese trabajo de forma definitiva.

4. **No duplicar lo que ya hacen las herramientas .NET.** Si algo lo resuelve
   `LoadFoldersBuilder`, `FileNameEncoder` o el extractor, no rehacerlo en Python ni en
   PowerShell.

5. **No romper el formato del YAML.** Es fijo. Un campo nuevo obliga a tocar
   `BuildRuleYaml.cs`, y además el extractor genera estos archivos: ver
   `LoadFoldersBuild.Contents` en el repositorio del extractor.

6. **La carpeta de origen es siempre `Data/`** (o `Ajustes/`, o `Common/`). Nunca editar `output/`: se rehace en cada
   `LoadFoldersBuilder -build` y en `-copia` (`CopiaLimpia.cs`), y lo que se toque ahí se
   pierde. Es la copia liviana del mod —sin `UNUSED.xml`, `LoadFolders.Build.yaml` ni
   comentarios en los XML, y con la DLL de `Common/Assemblies`, que el builder compila justo
   antes (`Dll.cs`; si no compila, `-copia` sale con 1 y la CI no publica)—, y es la que carga el juego: `Mods\RML` enlaza a
   `output\RimWorld Mod Latino`, no al repo. Por eso un cambio hecho a mano en `Data/` no
   se ve en el juego hasta regenerar el índice. Al Workshop se sube esa copia; subido el
   repo tal cual llevaba `.git` y los binarios de `Source/` (57 MB contra 11,5).

   Los comentarios se sacan solo en la copia. En `Data/` el `<!-- EN: ... -->` se queda:
   es lo que deja revisar las traducciones (ver «Editar XML de traducción»).

7. **Rutas largas.** Las de `Data/` pasan los 254 caracteres. No copiarlas con `xcopy`,
   que trunca en silencio: una vez se perdieron 2666 de 3527 archivos sin un solo error.
   `CopiaLimpia` usa .NET, que no tiene ese límite, y además cuenta los archivos.

   Y ninguna puede llegar a 260 contando la ruta del Workshop (ver «Cómo se produce una
   traducción»). Un nombre de mod o de carpeta de autor nuevo puede volver a pasarse:
   `01-regenerar-indice.cmd` lo avisa en amarillo y la CI queda en rojo.

8. **Nombres con espacios, `!` y acentos.** Casi todas las rutas de `Data/` los tienen.
   En consola hay que entrecomillar cada ruta por separado; un `git checkout --` con
   varias rutas sin comillas no restaura nada y **no da error**. En Python, `glob` trata
   `[FSF]` como clase de caracteres y saltea esas carpetas: usar `os.walk`.

9. **`extractor.log` no se versiona.** Está en `.gitignore`. Es el log de cada corrida.

## La página de mods sigue a Steam

`pagina-diaria.yml` corre todos los días: rearma `docs/index.html` con `LoadFoldersBuilder -pagina`
y lo publica en GitHub Pages **sin commitear**. Por eso la página puede tener fechas de Steam más
nuevas que el `docs/` de `master`, y está bien: el de `master` se pone al día en el próximo push
que toque `Data/`.

Un mod actualizado en Steam después de su última traducción lleva la etiqueta «posiblemente
atrasada». Si la actualización no cambiaba nada que traducir, la reextracción no deja diff y la
etiqueta no se iría nunca. Para eso está `Revisados.tsv`: WorkshopID, la fecha de Steam revisada
y el nombre. Con esa fecha anotada la fila dice «revisada»; si el mod se vuelve a actualizar, la
fecha de Steam pasa a ser más nueva y la etiqueta vuelve sola.

`Revisados.tsv` vive en la rama `revisados`, que no tiene nada más, y no en `master`: cada marca
es un commit, y en `master` llenarían el historial de las traducciones. El builder lo lee de
`origin/revisados` con `git show`, así que en local alcanza con un `git pull` o `git fetch`; sin
la rama, la página sale igual, sin revisiones. No se trabaja en esa rama a mano: no comparte
historia con `master` y no se une nunca.

Se marca desde la página abierta con `?revisar` al final de la dirección
(`https://lordmordelon.github.io/RML/?revisar`): cada fila atrasada muestra «marcar como
revisada», que abre un issue ya escrito. Al crearlo, `revisado.yml` anota la fecha, republica la
página y cierra el issue. Solo actúa con issues del dueño del repo. Esos issues llevan la
etiqueta `revisión`, que los separa de los de traducción: la pide el enlace y la pone el flujo.

Junto al filtro aparece además «marcar las N como revisadas», que hace lo mismo con todas las
atrasadas en un solo issue. Ese issue lleva una línea `- <WorkshopID> <fecha>` por mod, y se anota
la fecha de Steam que mostraba la página, no la del momento: si un mod se actualizó después de
mirarla, esa actualización sigue saliendo como atrasada. Los nombres no van en el enlace, que con
80 mods se acercaría al largo máximo de una dirección; `revisado.yml` los saca de `ModList.tsv`.

## Los mods más pedidos

El botón «Solicitar» de la ventana del juego abre un issue «Solicitud: <nombre>» con una línea
`- <WorkshopID> <packageId>`. `solicitud.yml` lo anota en `Pedidos.tsv`, en la rama `solicitudes`
(huérfana, como `revisados`), y cierra el issue con el puesto en que quedó. Hay una línea por mod y
por usuario de GitHub, así que se cuentan personas y no issues. El nombre sale de Steam, que de paso
confirma que el número es un mod de RimWorld. A diferencia de `revisado.yml`, acepta issues de
cualquiera: el cuerpo pasa solo por variables de entorno y se lee únicamente esa línea.

Con un pedido que no tiene nada que traducir, el dueño le pone la etiqueta `sin-texto` y el flujo lo
anota en `SinTexto.tsv`, en la misma rama: sale del gráfico, los pedidos siguientes se responden
solos y el builder lo copia a `output/`, donde la ventana lo muestra como «Sin texto». Si el mod
agrega texto después, se saca la fila a mano.

`docs/solicitudes.html` es un gráfico de barras con los 20 más pedidos, sin los que ya tienen
carpeta en `Data/` ni los sin texto. Lo arman `-pagina` y `-build`, pero **no se commitea**
(`.gitignore`): lo publica `pagina-diaria.yml`. Por eso `pages.yml` ya no publica el `docs/` del
commit tal cual: llama a `pagina-diaria.yml`, que lo rearma con el gráfico. Publicado tal cual,
cada push que tocara la lista borraría el gráfico hasta el día siguiente. La lógica está en
`Source/LoadFoldersBuilder/Solicitudes.cs`.

## Antes de commitear: mirar el diff

Es la regla que más veces salvó el repositorio. Una reextracción puede dañar traducciones
sin que nada falle ni avise: los archivos se reescriben enteros, así que un borrado o un
valor cambiado se ve igual que un cambio legítimo. Así se encontraron tres pérdidas
distintas, ninguna reportada por ninguna herramienta.

Las tres comprobaciones que importan, y qué significa cada una. Van contra `HEAD`, no
contra el índice: si los archivos ya están «staged», un `git diff` a secas sale vacío
aunque el daño esté ahí.

```sh
# 1) Que ninguna traducción haya cambiado de valor.
#    Compara clave por clave el valor viejo y el nuevo, ignorando los TODO.
git diff HEAD -U0 -- "Data/*/Languages/*" "Data/*/*/Languages/*" \
  | grep -E "^[-+]  <[A-Za-z]" \
  | sed 's/^\([-+]\) *<\([^>]*\)>\(.*\)<\/.*/\1 \2 = \3/' \
  | awk '{k=$2; v=substr($0, index($0,"= ")+2);
          if ($1=="-") old[k]=v; else new[k]=v}
     END {for (k in new) if (k in old && old[k]!="TODO" && new[k]!="TODO" && old[k]!=new[k])
            print k ": " old[k] " -> " new[k]}'

# 2) Que no se haya borrado ningún UNUSED.xml.
git status --short | grep -E '^( D|D )'

# 3) Que toda regla de gramática conserve su prefijo «simbolo->».
#    Dentro de un bloque *.rulesStrings cada <li> es una regla entera, y sin el "->"
#    deja de serlo. La comprobación 1 no lo ve: solo mira las líneas <clave>valor</clave>.
git diff HEAD --name-only -z | xargs -0 -r awk '
  /<[A-Za-z0-9_.]+\.rulesStrings>/ {dentro=1}
  /<\/[A-Za-z0-9_.]+\.rulesStrings>/ {dentro=0}
  dentro && /<li>/ && !/-&gt;/ {print FILENAME ": " $0}
'
```

Las tres tienen que salir vacías. Si sale algo, hay que entender **por qué** antes de
commitear: puede ser correcto, pero nunca se da por bueno sin mirarlo.

Cuando algo aparece dañado, lo primero es averiguar si lo causó el cambio en curso:
volver a correr los mismos mods con ese cambio desactivado. Tres veces el resultado fue
que el daño ya existía y el cambio solo lo destapó.

**El mensaje de commit dice qué se tocó y en qué mod.** «Magic: devolver sus nombres a las
reglas del meme Transcendent» sirve; «correccion» no. Cuando aparece una pérdida, se rastrea
con `git log`, y una fila de «correccion» obliga a abrir cada commit para saber cuál fue.
Un commit por arreglo: si uno resulta mal, se revierte ese solo.

## Editar XML de traducción

- **A mano, directo o con tabla y script.** Cada traducción se escribe a mano, una por una. Puede
  ir directo sobre el archivo, con las herramientas de edición normales, o en una tabla
  clave → traducción que un script copia al archivo. **El camino de la tabla vale para cualquier
  mod desde el 26-oct-2026** (Angel), después de probarse en DMS II, Hegemony y Expedition:
  1. `python -I Herramientas/traducir.py tabla <mod> --palabras 1500 --salida <tabla.tsv>` arma la
     tabla de la tanda (en la carpeta temporal): el inglés arriba de cada clave y, si el mismo inglés
     ya está traducido en otro mod, cómo (`# RML: …`). Es solo una sugerencia.
  2. Escribir la tabla a mano, entrada por entrada, con el glosario del mod y la traducción oficial
     abiertos (`Herramientas/oficial/`).
  3. `python -I Herramientas/traducir.py aplicar <mod> <tabla.tsv>` la copia solo sobre `TODO`,
     guarda una copia de antes y, en el mismo informe, revisa la tanda, los términos del glosario
     del mod (`--glosario` para sumar los de la familia) y las tres comprobaciones del diff.
  4. Si un término nuevo salió en la tanda, va al glosario del mod antes de la siguiente.

  Lo que hace el comando y por qué está en
  [PLAN-HERRAMIENTAS.md](PLAN-HERRAMIENTAS.md); cómo se usa cada orden, al principio de
  [traducir.py](Herramientas/traducir.py). No guarda nada si el XML queda inválido o si una
  traducción rompe algo que se vería en el juego: una `{llave}` que el inglés no tiene, un color sin
  cerrar o una regla de gramática sin su `simbolo->`. Los marcadores que faltan y los saltos de
  línea solo los avisa, porque el español a menudo saca con razón `[PAWN_pronoun]`. Las listas se
  llenan enteras y los `Patches` se nombran por su `xpath`; una lista que no se puede emparejar con
  su inglés sale en la tabla para hacerla a mano. Los campos cuyo inglés es una clave de `Keyed` se
  dejan en `TODO` solos. Las pruebas: `python -I Herramientas/pruebas/test_traducir.py` (la CI las
  corre en `herramientas.yml`).

  Lo que no se hace nunca es un script que **genere o reemplace** texto en masa («cambiar X por Y»
  en todos los archivos): un reemplazo global rompe cosas que no se ven hasta que el juego las
  carga. Hay dos excepciones, las dos de Combat Extended: las recetas y las etiquetas de partes de
  armas de `Patches/`, que se arman por script con las reglas de «Recetas por script» y «Partes de
  armas por script» de
  [PLAN-COMBAT-EXTENDED.md](Data/Combat%20Extended%20-%202890901044/PLAN-COMBAT-EXTENDED.md). Esas
  no se extienden a otros mods sin volver a preguntar.

  Los planes que ya usan la tabla: «Decisiones de Angel» de
  [PLAN-DMS-II.md](Data/!AobaKuma/The%20Dead%20Man's%20Switch%20-%203819799478/PLAN-DMS-II.md),
  [PLAN-HEGEMONY.md](Data/Hegemony%20-%20Faction%20Territories%20and%20Vassalage%202%20-%203825320105/PLAN-HEGEMONY.md)
  (en pausa desde el 25-oct-2026: el autor todavía no le da soporte de traducción a la ventana del
  mod) y [PLAN-AOBAKUMA.md](Data/!AobaKuma/PLAN-AOBAKUMA.md).

  Esto vale para el **contenido**. Las operaciones estructurales —reextraer, actualizar
  por lotes, regenerar el índice— las hace el extractor, no una edición a mano.

- **Buscar sí, con lo que sea.** Localizar `TODO` o verificar términos es solo lectura y
  no tiene ninguna restricción.

- **No tocar la estructura.** Las etiquetas, los comentarios `<!-- EN: ... -->` y la
  codificación `UTF-8` quedan como están. El comentario `EN:` es el original en inglés:
  es lo que permite ver si una traducción quedó puesta en el campo equivocado.

- **Reemplazar solo el `TODO`.** Nada más.

- **Nunca modificar identificadores de def ni rutas de etiqueta**, del estilo
  `PMP_CivilMechhive_Mechanitor` o
  `OpportunitySite_AbandonedMechanitorPlatform.questDescriptionRules`.

## Reglas estrictas de XML

- **Etiquetas de color escapadas**, con los dos símbolos:
  `&lt;color=#RRGGBB&gt;texto&lt;/color&gt;`. Sin escapar rompe el analizador de
  RimWorld. Se traduce el texto interior; el valor del color no se toca.

- **Todo en `UTF-8`.**

- **No se traduce:**
  - Nombres propios de mods (*Save Our Ship 2*, *Biotech*, *Royalty*).
  - Marcadores dinámicos: `{0}`, `{PAWN_nameDef}`, `[resolvedQuestName]`.
  - Escapes literales como `\n`.
  - `Vanilla`, cuando se refiere al contenido base del juego.

## Cosas que ya pasaron

- **Una traducción de `Patches/` que estaba escrita y no aparecía en el juego.** El nombre
  del archivo salía del mod dueño del def, y `Odyssey.xml` estaba en nueve carpetas. Las de
  `Data/` son carpetas de un solo mod de RimWorld, que las recorre deduplicando por ruta
  relativa: cargaba uno y descartaba los otros ocho sin avisar. Eran 30 archivos de 107, con
  336 traducciones adentro. Ahora el nombre va codificado, como el de los `Keyed` y los
  `DefInjected`, y `01-regenerar-indice.cmd` avisa en amarillo si dos carpetas vuelven a
  compartir una ruta interna.

  Se probó antes con el id del workshop adelante, que también era único pero dejaba el peor
  archivo en 242 de los 259 que RimWorld puede abrir desde el Workshop; codificado queda en
  174. Acá el largo de la ruta pesa más que la legibilidad (ver la regla 7). También caían en
  la colisión dos `Jobs_Misc.xml` de traducciones heredadas, con el nombre sin codificar, que
  no vienen del extractor: esos quedaron con el id adelante, porque sus rutas están holgadas
  y el extractor no los rehace.

- **Un mismo nodo traducido dos veces.** Puede estar en `DefInjected/` y en `Patches/` a
  la vez, con valores distintos. El extractor unifica las dos formas, así que una gana:
  gana la de `DefInjected`, que es la que el juego aplica última. Cuando pasa, el
  extractor lo avisa en el log. Un aviso así significa que sobra una de las dos, casi
  siempre la del `Patches`, que quedó de una versión anterior del mod.

- **Traducciones que vuelven a `TODO`.** Pasaba cuando el `xpath` de un `Patches` no
  encontraba su objetivo, porque el def lo agrega otro mod o vive en una carpeta
  condicional. Ya está arreglado en el extractor, pero si vuelve a aparecer, el síntoma
  es ese y la causa está de aquel lado.

- **Los motes no se traducen.** Sus etiquetas se heredan de `MoteBase` y no se le
  muestran nunca al jugador. El extractor ya no las extrae; si vuelven a aparecer
  entradas con `<!-- EN: Mote -->`, es un síntoma, no algo para traducir.
