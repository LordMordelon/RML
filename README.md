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
About/About.xml                 metadatos del mod y supportedVersions
Data/!<Autor>/                  autores con 4 o más mods; el ! los fija arriba
  <Nombre del mod> - <ID>/
Data/<Nombre del mod> - <ID>/   los demás, sueltos
  Languages/SpanishLatin (Español(Latinoamérica))/
    Keyed/ DefInjected/ Strings/
  Patches/                      fuera de Languages, a la par
  LoadFolders.Build.yaml        a qué mod se engancha esta carpeta
  UNUSED.xml                    traducciones cuyo nodo ya no existe en el mod
LoadFolders.xml                 GENERADO — no editar a mano
ModList.tsv                     GENERADO — índice de qué mods cubre RML
actualizar.cmd                  regenera los dos anteriores
publicar.cmd                    arma la copia limpia para el Workshop
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
cualquier nivel bajo `Data/`, así que mover una de lugar no rompe nada.

**Se regenera solo en los dos casos que importan:** el extractor lo rehace al terminar
una traducción rápida, y una GitHub Action lo rehace al hacer push. `actualizar.cmd` es
para el caso que queda, que es cambiar algo a mano sin pasar por ninguno de los dos.

## Requisitos

- **.NET SDK 10** — lo usan tanto las herramientas de `Source/` como el extractor, así que alcanza con uno solo.
- El fork del extractor clonado **como carpeta hermana de este repositorio**:

  ```
  INVESTIGAR/
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
   quede `Languages/SpanishLatin (Español(Latinoamérica))/…`.
5. Crear el `LoadFolders.Build.yaml` de esa carpeta con el `packageId` real del mod
   (se lee del `About.xml` del mod en el workshop). Ver
   `LoadFolders.Build.Example.yaml` para la referencia de campos.
6. Regenerar el índice con **`actualizar.cmd`** (doble clic). Deja al día
   `LoadFolders.xml` y `ModList.tsv`, que es lo que hace que el mod cargue.
7. Probar en el juego y commitear el `LoadFolders.xml` regenerado junto al resto.

No hace falta hacer nada si el mod trae su propia traducción al español: RML va último
en la lista de mods y le gana igual. El extractor lo avisa en el log por si esa
traducción sirve para partir de ahí, no porque haya que anotarla en ningún lado.

### El atajo: la traducción rápida

Si en el extractor marcás **«Traducción rápida»** al elegir el mod, los pasos 3 a 6 los
hace él solo: escribe directamente en `Data/`, conserva lo que ya estaba traducido, genera
el `LoadFolders.Build.yaml` y regenera el índice. Solo queda probar en el juego.

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
- **Steam Workshop** — doble clic en **`publicar.cmd`**. Deja en `salida/` una copia
  limpia del mod, sin `Source/` ni documentación, para copiar a `Mods/` y subirla desde
  el juego.

Los dos usan la misma lista de qué entra: `About/`, `Data/`, `LoadFolders.xml`,
`ModList.tsv` y `LICENSE`. Y los dos comprueban que la copia tenga los mismos archivos
que el original antes de publicar nada.

Falta un `Preview.png` en `About/` para que el Workshop tenga imagen de portada.

## Licencia

GPL-3.0. Las herramientas de `Source/` derivan de
[RimWorldKorea/RMK](https://github.com/RimWorldKorea/RMK), también GPL-3.0.

Los derechos sobre RimWorld y el contenido traducido pertenecen a Ludeon Studios Inc.
según el EULA de RimWorld.
