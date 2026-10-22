# Ajustes

Cambios a mano a una traducción que solo se cargan cuando cierto mod está activo, y que le
ganan a lo que hay en `Data/`. Son para cuando una traducción correcta no sirve junto a otro mod:
un texto que no cabe en su lugar o que ahí se lee mal.

**Solo arreglos de espacio o de contexto, nunca para traducir.** Una traducción va en `Data/`,
por el extractor. Lo que se pone acá le gana a todo lo demás, y nadie lo revisa al reextraer.

## Cómo funciona

- **Una carpeta por mod**, con el mismo formato que en `Data/`:

  ```
  Ajustes/
    <Nombre del mod> - <WorkshopID>/
      LoadFolders.Build.yaml
      Languages/SpanishLatin/Keyed/Ajustes-<WorkshopID>.xml
  ```

- **El yaml decide cuándo se carga.** Se copia el del mod en `Data/`, si lo tiene. Su `PackageID`
  es el mod que tiene que estar activo para que el ajuste se aplique.
- **El archivo se llama `Ajustes-<WorkshopID>.xml`.** Si dos archivos tienen la misma ruta interna
  en dos carpetas de RML, el juego carga uno solo y no avisa. `LoadFoldersBuilder -rutas` lo
  comprueba contra `Data/`, y la CI queda en rojo si se repite.
- **Gana porque va arriba.** El builder pone las entradas de `Ajustes/` al principio de cada
  versión en `LoadFolders.xml`. RimWorld lee esa lista de abajo hacia arriba, y en `Keyed` y
  `DefInjected` la última clave leída pisa a las anteriores.
- **El extractor no la ve.** «Actualizar todo RML» y el agrupador solo recorren `Data/`, así que
  un ajuste no se borra al reextraer. Sí va a `output/` y al `forceLoadAfter` de `About.xml`, pero
  no a `ModList.tsv` ni a la página de mods. Este `README.md` no llega al juego: los `.md` se
  quedan fuera de `output/`.

## Agregar un ajuste

1. Crear la carpeta del mod, con el mismo nombre que tiene en `Data/`, y copiar su
   `LoadFolders.Build.yaml`.
2. Escribir el XML en `Languages/SpanishLatin/Keyed/` (o en `DefInjected/`, con el mismo cuidado
   con el nombre) solo con las claves que cambian.
3. Correr `LoadFoldersBuilder -build` y comprobar que la carpeta aparece arriba de cada versión en
   `LoadFolders.xml`.
4. Probar en el juego con el mod activo.

## Cuándo se saca

Un ajuste es un parche mientras el autor del mod no lo resuelve. Lo mejor es pedirle que deje
traducir ese texto aparte, en una clave propia del mod, y cuando lo haga, traducir esa clave en
`Data/` y borrar la carpeta de `Ajustes/`.

Así pasó con el primero. Nice Health Tab mostraba el `AllowFood` del juego («Permitir comida»)
seguido de «:» en una columna angosta, y la segunda palabra no se veía. El 22-oct-2026 se
cambió acá por «Comida». Se le pidió al autor, Andromeda, una clave propia, y la agregó ese mismo
día: `NHT_AllowFood`, traducida como «Comida» en `Data/`.

Mientras siga, un ajuste de una clave del juego cambia ese texto en todo el juego, no solo en el
mod, siempre que el mod esté activo.
