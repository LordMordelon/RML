# Notas para agentes — RimWorld Mod Latino (RML)

Contexto para trabajar en este repositorio sin tener que redescubrirlo. El
[README](README.md) explica cómo está organizado y cómo publicarlo; acá está lo que hay
que saber antes de tocar nada.

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
About/About.xml                 metadatos del mod. supportedVersions: 1.6
Data/
  !<Autor>/                     autores con 4 o más mods; el ! los fija arriba
    <Nombre del mod> - <ID>/
  <Nombre del mod> - <ID>/      los demás, sueltos
LoadFolders.xml                 GENERADO — no editar a mano
docs/index.html                 GENERADO — lista de mods como página web ordenable (GitHub Pages)
ModList.md                      GENERADO — mods que cubre RML, con link a Steam, fecha de actualización del mod y de la traducción
ModList.tsv                     GENERADO — los mismos datos en texto plano, sin links
actualizar.cmd                  regenera los anteriores
publicar.cmd                    arma la copia limpia para el Workshop
LoadFolders.Build.Example.yaml  referencia de campos, con cada uno comentado
GLOSARIO.md                     terminología oficial de RimWorld ES
TRADUCIR.md                     guía para colaboradores sin git
CAMBIOS.md                      qué cambia respecto de RMK
RESCATE-PENDIENTE.md            traducciones huérfanas y decisiones tomadas
Source/LoadFoldersBuilder/      genera LoadFolders.xml a partir de los .yaml
Source/FileNameEncoder/         normaliza nombres de XML que NO vengan del extractor
.github/workflows/              loadfolders.yml (índice en push), pages.yml (publica docs/ si cambió), release.yml (.zip)
```

Y cada carpeta de mod, por dentro:

```
<Nombre del mod> - <ID>/
  Languages/SpanishLatin (Español(Latinoamérica))/
    DefInjected/  Keyed/  Strings/
  Patches/                    <- fuera de Languages, a la par
  LoadFolders.Build.yaml
  UNUSED.xml                  <- traducciones apartadas
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

## Cómo se produce una traducción

1. **Extraer** el mod con el extractor, con idioma de destino
   `SpanishLatin (Español(Latinoamérica))`. El nombre de la carpeta de idioma es exacto:
   ni `SpanishLatin` solo ni `Spanish`.
2. **Traducir**, en la planilla `.xlsx` o reemplazando los `TODO` en el XML.
3. **Regenerar el índice.** Lo hace `actualizar.cmd`, y también el extractor al terminar
   una traducción rápida y la GitHub Action al hacer push.

Con «Traducción rápida» marcada, el extractor escribe directamente en `Data/`, conserva
lo ya traducido, genera el `LoadFolders.Build.yaml` y regenera el índice. Después de una
actualización del juego, «Actualizar todo RML» hace lo mismo con todos los mods de una vez;
los que no están instalados quedan como están.

## Reglas

1. **El glosario manda.** Revisar [GLOSARIO.md](GLOSARIO.md) antes de traducir cualquier
   texto. Tiene la terminología oficial de RimWorld en español. No inventar términos.

2. **No editar lo generado.** `LoadFolders.xml`, `ModList.md`, `ModList.tsv` y `docs/index.html` se rehacen enteros. Para
   cambiar algo, editar el `LoadFolders.Build.yaml` del mod y correr `actualizar.cmd`.

3. **No tocar el `UNUSED.xml`.** Guarda las traducciones cuyo nodo desapareció del mod.
   El extractor lo relee en cada actualización, así que si el mod devuelve el nodo a su
   lugar la traducción se recupera sola. Borrarlo pierde ese trabajo de forma definitiva.

4. **No duplicar lo que ya hacen las herramientas .NET.** Si algo lo resuelve
   `LoadFoldersBuilder`, `FileNameEncoder` o el extractor, no rehacerlo en Python ni en
   PowerShell.

5. **No romper el formato del YAML.** Es fijo. Un campo nuevo obliga a tocar
   `BuildRuleYaml.cs`, y además el extractor genera estos archivos: ver
   `LoadFoldersBuild.Contents` en el repositorio del extractor.

6. **La carpeta de origen es siempre `Data/`.** Nunca editar `salida/`, que es la copia
   que arma `publicar.cmd`.

7. **Rutas largas.** Las de `Data/` pasan los 254 caracteres. Usar `robocopy`, no
   `xcopy`, que trunca en silencio: una vez se perdieron 2666 de 3527 archivos sin un
   solo error. `publicar.cmd` ya lo hace bien y además cuenta los archivos.

8. **Nombres con espacios, `!` y acentos.** Casi todas las rutas de `Data/` los tienen.
   En consola hay que entrecomillar cada ruta por separado; un `git checkout --` con
   varias rutas sin comillas no restaura nada y **no da error**. En Python, `glob` trata
   `[FSF]` como clase de caracteres y saltea esas carpetas: usar `os.walk`.

9. **`extractor.log` no se versiona.** Está en `.gitignore`. Es el log de cada corrida.

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

- **A mano y directo.** El contenido traducido se edita sobre el archivo, con las
  herramientas de edición normales. Nada de scripts que reemplacen texto en masa: un
  reemplazo global sobre estos archivos rompe cosas que no se ven hasta que el juego
  las carga.

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
