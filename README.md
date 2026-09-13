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
ModList.md                      GENERADO — mods que cubre RML, con link a Steam, fecha de actualización del mod y de la traducción
ModList.tsv                     GENERADO — los mismos datos en texto plano, sin links
01-regenerar-indice.cmd         regenera los anteriores
02-armar-copia-limpia.cmd       arma la copia limpia para el Workshop en output/
03-subir-al-workshop.cmd        sube esa copia desde el juego sin romper el enlace de Mods\RML (corre el 02)
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
   (`ModList.md`, `ModList.tsv` y la página `docs/index.html`, publicada en
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

Enlazar o copiar este repositorio dentro de la carpeta `Mods/` de RimWorld,
activarlo **último** en la lista de mods y elegir el idioma `SpanishLatin`.

Activarlo último no es un detalle: es lo que hace que sus traducciones le ganen a
las que traiga cualquier otro mod.

## Publicar

- **GitHub Releases** — etiquetar una versión (`git tag v1.0 && git push --tags`) y la
  Action arma el `.zip` con solo lo que el juego necesita.
- **Steam Workshop** — con RimWorld cerrado, doble clic en **`03-subir-al-workshop.cmd`**. Corre
  `02-armar-copia-limpia.cmd`, que deja en `output/` una copia limpia del mod, sin `.git`, `Source/` ni
  documentación. Después apunta el enlace `Mods\RML` a esa copia y espera: abres el juego,
  subes RML, cierras el juego y presionas una tecla para que el enlace vuelva al repo.

  Necesita, una sola vez, la variable con la carpeta Mods de RimWorld:

  ```
  setx RIMWORLD_MODS "D:\SteamLibrary\steamapps\common\RimWorld\Mods"
  ```

  y que `Mods\RML` sea un enlace al repo (`mklink /J`). Si es una carpeta de verdad, no la
  toca.

Los dos usan la misma lista de qué entra: `About/`, `Data/`, `LoadFolders.xml`,
`ModList.tsv` y `LICENSE`. Y los dos comprueban que la copia tenga los mismos archivos
que el original antes de publicar nada. `02-armar-copia-limpia.cmd` además corre
`LoadFoldersBuilder -rutas` y no arma la copia si alguna ruta, instalada desde el
Workshop, pasaría el límite de Windows.

Falta un `Preview.png` en `About/` para que el Workshop tenga imagen de portada.

## Licencia

GPL-3.0. Las herramientas de `Source/` derivan de
[RimWorldKorea/RMK](https://github.com/RimWorldKorea/RMK), también GPL-3.0.

Los derechos sobre RimWorld y el contenido traducido pertenecen a Ludeon Studios Inc.
según el EULA de RimWorld.
