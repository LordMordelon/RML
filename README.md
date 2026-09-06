# RimWorld Mod Latino (RML)

Mod que reúne traducciones al **español latinoamericano** de mods de RimWorld.

Cada traducción se aplica únicamente si el mod correspondiente está activo, así que
el mod se puede tener puesto siempre sin importar qué mods se usen en cada partida.

Arquitectura basada en [RimWorld Mod Korean (RMK)](https://github.com/RimWorldKorea/RMK).

---

### 👉 ¿Querés colaborar traduciendo? Leé **[TRADUCIR.md](TRADUCIR.md)**

Esa guía está escrita para alguien que no sabe programar: no necesita git, ni consola, ni .NET.
Lo que sigue en este README es la documentación del **mantenedor** del proyecto.

---

## Cómo está organizado

```
About/About.xml                 metadatos del mod y supportedVersions
Data/<Nombre del mod> - <ID>/   una carpeta por mod traducido
  Languages/SpanishLatin (Español(Latinoamérica))/
    Keyed/ DefInjected/ Patches/
  LoadFolders.Build.yaml        a qué mod se engancha esta carpeta
LoadFolders.xml                 GENERADO — no editar a mano
Source/LoadFoldersBuilder/      genera LoadFolders.xml a partir de los .yaml
Source/FileNameEncoder/         normaliza los nombres de los XML
```

`LoadFolders.xml` es lo que hace que RimWorld cargue la traducción de cada mod solo
cuando ese mod está presente, mediante el atributo `IfModActive`.

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
6. Normalizar los nombres de archivo, pasándole la carpeta del mod:

   ```
   dotnet run --project Source/FileNameEncoder/FileNameEncoder -- "Data/<Nombre del mod> - <WorkshopID>"
   ```

   Sin argumentos pregunta la ruta por teclado.
7. Regenerar el índice: `dotnet run --project Source/LoadFoldersBuilder` y escribir
   `-build` cuando lo pida. El `-build` **no** se puede pasar como argumento: el
   programa lo lee por teclado.
8. Probar en el juego y commitear el `LoadFolders.xml` regenerado junto al resto.

## Instalar para probar

Enlazar o copiar este repositorio dentro de la carpeta `Mods/` de RimWorld,
activarlo **último** en la lista de mods y elegir el idioma `SpanishLatin`.

## Licencia

GPL-3.0. Las herramientas de `Source/` derivan de
[RimWorldKorea/RMK](https://github.com/RimWorldKorea/RMK), también GPL-3.0.

Los derechos sobre RimWorld y el contenido traducido pertenecen a Ludeon Studios Inc.
según el EULA de RimWorld.
