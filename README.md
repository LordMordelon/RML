# RimWorld Mod Latino (RML)

Mod que reúne traducciones al **español latinoamericano** de mods de RimWorld.

Cada traducción se aplica únicamente si el mod correspondiente está activo, así que
el mod se puede tener puesto siempre sin importar qué mods se usen en cada partida.

Arquitectura basada en [RimWorld Mod Korean (RMK)](https://github.com/RimWorldKorea/RMK).
Qué cambia respecto de ese original: **[CAMBIOS.md](CAMBIOS.md)**.

---

### 👉 ¿Quieres colaborar traduciendo? Lee **[TRADUCIR.md](TRADUCIR.md)** y el **[GLOSARIO](GLOSARIO.md)**

Esa guía está escrita para alguien que no sabe programar: no necesita git, ni consola, ni .NET.
Lo que sigue en este README es la documentación del **mantenedor** del proyecto.

---

## Cómo está organizado

```
About/About.xml                 metadatos del mod, supportedVersions y forceLoadAfter (GENERADO)
Data/!<Autor>/                  autores con 4 o más mods; el ! los fija arriba
  <Nombre del mod> - <ID>/
Data/<Nombre del mod> - <ID>/   los demás, sueltos
  Languages/SpanishLatin/       nombre corto a propósito: ver AGENTS.md
    Keyed/ DefInjected/ Strings/
  Patches/                      fuera de Languages, a la par
  LoadFolders.Build.yaml        a qué mod se engancha esta carpeta
  UNUSED.xml                    traducciones cuyo nodo ya no existe en el mod
LoadFolders.xml                 GENERADO — no editar a mano
docs/index.html                 GENERADO — lista de mods como página web ordenable (GitHub Pages)
ModList.tsv                     GENERADO — la misma lista en texto plano: mods que cubre RML, fecha de actualización del mod y de la traducción
01-regenerar-indice.cmd         regenera los anteriores
02-armar-copia-limpia.cmd       deja al día output/, la copia liviana que carga el juego y se sube (el 01 ya lo hace)
03-subir-al-workshop.cmd        corre el 02 y deja Mods\RML enlazado a output/ para subir desde el juego
Source/LoadFoldersBuilder/      genera LoadFolders.xml a partir de los .yaml
Source/FileNameEncoder/         normaliza nombres de XML que NO vengan del extractor
```

`LoadFolders.xml` es lo que hace que RimWorld cargue la traducción de cada mod solo
cuando ese mod está presente, mediante el atributo `IfModActive`.

Los `Patches/` y el `UNUSED.xml` van fuera de `Languages/` por el mismo motivo: ahí adentro
RimWorld los cargaría como una traducción más. El `UNUSED.xml` guarda las traducciones cuyo
nodo desapareció del mod, y sus claves muertas le llenarían el log de errores al jugador. No
hay que tocarlo: el extractor lo vuelve a leer en cada actualización, así que si el mod
devuelve el nodo a su lugar la traducción se recupera sola.

Agrupar por autor es solo para ordenar: el extractor busca la carpeta de un mod en
cualquier nivel bajo `Data/`, así que mover una de lugar no rompe nada. Los autores con
cuatro traducciones o más tienen su `Data/!Autor/`, y de mantenerlo se encarga el extractor:
un mod nuevo cae directo ahí si su autor ya tiene carpeta, y las sueltas de ese autor se
mueven solas en la siguiente traducción rápida. Si un autor recién llega a cuatro, lo avisa
por log: la carpeta la creas tú, y basta con crearla vacía.

**Se regenera solo en los dos casos que importan:** el extractor lo rehace al terminar
una traducción rápida, y una GitHub Action lo rehace al hacer push. `01-regenerar-indice.cmd` es
para el caso que queda, que es cambiar algo a mano sin pasar por ninguno de los dos.

## Requisitos

- **.NET SDK 10** — lo usan tanto las herramientas de `Source/` como el extractor, así que alcanza con uno solo.
- El fork del extractor clonado **como carpeta hermana de este repositorio**:

  ```
  <carpeta de trabajo>/
    RML/                  <- este repo
    RimworldExtractor/    <- https://github.com/LordMordelon/RimworldExtractor
  ```

  `FileNameEncoder` usa `RimworldExtractorInternal.Utils.GenerateFileName`. Si el
  extractor está en otro lado:

  ```
  dotnet build -p:ExtractorInternalPath=<ruta al RimworldExtractorInternal.csproj>
  ```

## Agregar la traducción de un mod

1. Extraer el mod con el extractor, con idioma de destino
   `SpanishLatin (Español(Latinoamérica))`.
2. Traducir en la planilla `.xlsx`.
3. Convertir XLSX → XML con el propio extractor.
4. Copiar el resultado a `Data/<Nombre del mod> - <WorkshopID>/`, verificando que
   quede `Languages/SpanishLatin/…`, con el nombre corto: con el largo, instalado desde el
   Workshop algunas rutas pasan el límite de Windows y el juego queda en pantalla negra.
5. Crear el `LoadFolders.Build.yaml` de esa carpeta con el `packageId` real del mod
   (se lee del `About.xml` del mod en el workshop). Ver
   `LoadFolders.Build.Example.yaml` para la referencia de campos.
6. Regenerar el índice con **`01-regenerar-indice.cmd`** (doble clic). Deja al día
   `LoadFolders.xml`, que es lo que hace que el mod cargue, la lista de mods
   (`ModList.tsv` y la página `docs/index.html`, publicada en
   https://lordmordelon.github.io/RML/) y el `forceLoadAfter` de `About/About.xml`,
   que hace que RML cargue después de cada mod que traduce.
7. Probar en el juego y commitear el `LoadFolders.xml` regenerado junto al resto.

No hace falta hacer nada si el mod trae su propia traducción al español: RML va último
en la lista de mods y le gana igual. El extractor lo avisa en el log por si esa
traducción sirve para partir de ahí, no porque haya que anotarla en ningún lado.

### El atajo: la traducción rápida

Si en el extractor marcas **«Traducción rápida»** al elegir el mod, los pasos 3 a 6 los
hace él solo: escribe directamente en `Data/`, conserva lo que ya estaba traducido, genera
el `LoadFolders.Build.yaml` y regenera el índice. Solo queda probar en el juego.

### Después de una actualización del juego: «Actualizar todo RML»

El botón **«Actualizar todo RML»** del extractor hace la traducción rápida de todos los mods
de `Data/`, uno detrás de otro, contra la versión instalada. Lo nuevo sale como `TODO` y lo
que el mod ya no tiene va a su `UNUSED.xml`. Los mods que no tengas instalados quedan como
están. Tarda varios minutos; antes de commitear, revisa el diff como indica
[AGENTS.md](AGENTS.md#antes-de-commitear-mirar-el-diff).

### Sobre `FileNameEncoder`

**No hay que correrlo sobre lo que genera el extractor**, aunque el nombre sugiera que sí.
Los dos calculan el nombre con la misma función pero con entradas distintas:

| | Entrada del hash |
|---|---|
| Extractor | nombre del mod + `Clase\|ArchivoOrigen` |
| FileNameEncoder | nombre de la carpeta en `Data/` + `Clase` |

Como el extractor borra y reescribe la carpeta entera en cada actualización, correr el
encoder encima renombraría todo, y la siguiente extracción lo devolvería a su nombre
original. **Cada ciclo sería un borrar+agregar completo en git** en vez de un diff línea
por línea.

Sigue sirviendo para XML que vengan de otro lado —hechos a mano, o sacados con otra
herramienta—, y ahí se usa así:

```
dotnet run --project Source/FileNameEncoder/FileNameEncoder -- "Data/<Nombre del mod> - <WorkshopID>"
```

Sin argumentos pregunta la ruta por teclado. **No toca la carpeta `Patches`**: esos
archivos ya vienen nombrados con el mod al que le aplica cada uno, que es legible y
estable, así que codificarlos sólo perdería esa información.

## Instalar para probar

Enlazar `output\RimWorld Mod Latino` dentro de la carpeta `Mods/` de RimWorld,
activarlo **último** en la lista de mods y elegir el idioma `SpanishLatin`. El enlace lo
crea `03-subir-al-workshop.cmd` la primera vez (ver «Publicar»), o a mano:

```
mklink /J "D:\SteamLibrary\steamapps\common\RimWorld\Mods\RML" "<repo>\output\RimWorld Mod Latino"
```

Se enlaza `output` y no el repo: es la copia liviana, la misma que se sube al Workshop
—sin `.git`, `Source/`, `UNUSED.xml`, `LoadFolders.Build.yaml` ni los comentarios de los
XML—, así que en el juego se prueba exactamente lo que se publica. Se rehace sola cada vez
que se regenera el índice: una traducción rápida del extractor se ve en el juego sin hacer
nada más. **Un cambio hecho a mano en `Data/` no se ve hasta correr
`01-regenerar-indice.cmd`.**

Activarlo último no es un detalle: es lo que hace que sus traducciones le ganen a
las que traiga cualquier otro mod.

## Publicar

- **GitHub Releases** — etiquetar una versión (`git tag v1.0 && git push --tags`) y la
  Action arma el `.zip` con solo lo que el juego necesita.
- **Steam Workshop** — doble clic en **`03-subir-al-workshop.cmd`**. Corre
  `02-armar-copia-limpia.cmd`, que deja al día la copia liviana de `output/`, y se
  asegura de que `Mods\RML` enlace a ella: si no existe lo crea, y si todavía apunta al
  repo lo cambia (para eso RimWorld tiene que estar cerrado). Después abres el juego y
  subes RML.

  Necesita, una sola vez, la variable con la carpeta Mods de RimWorld:

  ```
  setx RIMWORLD_MODS "D:\SteamLibrary\steamapps\common\RimWorld\Mods"
  ```

  Si `Mods\RML` es una carpeta de verdad y no un enlace, no la toca.

La copia de `output/` la arma `LoadFoldersBuilder -copia` (`CopiaLimpia.cs`): comprueba que
ninguna ruta, instalada desde el Workshop, pase el límite de Windows, y que no falte ningún
archivo. Pesa unos 11,5 MB contra los 19 de copiar `About/` y `Data/` tal cual.

El `.zip` de GitHub Releases todavía se arma con `cp` en la Action, sin quitar los
`UNUSED.xml` ni los comentarios: el builder no corre en Linux (ver CAMBIOS.md).

Falta un `Preview.png` en `About/` para que el Workshop tenga imagen de portada.

## Licencia

GPL-3.0. Las herramientas de `Source/` derivan de
[RimWorldKorea/RMK](https://github.com/RimWorldKorea/RMK), también GPL-3.0.

Los derechos sobre RimWorld y el contenido traducido pertenecen a Ludeon Studios Inc.
según el EULA de RimWorld.
