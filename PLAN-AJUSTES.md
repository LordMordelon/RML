# Plan: carpeta de ajustes condicionales

**Aprobado por Angel el 22-oct-2026.**

## Qué y por qué

A veces una traducción correcta no sirve en el contexto de otro mod. Nice Health Tab
(3328729902) muestra el texto del juego `AllowFood` («Permitir comida») seguido de «:» en una
columna angosta, y la segunda palabra se va a otra línea que no se ve. Con ese mod activo
tendría que decir «Comida».

RimWorld ya permite hacerlo: entre archivos distintos, gana la clave `Keyed` que se carga
última (`LoadedLanguage.LoadFromFile_Keyed` usa `SetOrAdd`; solo avisa de duplicados dentro
de un mismo archivo), y RML carga después del juego. El problema es dónde ponerlo: la carpeta
del mod en `Data/` la borra y la reescribe el extractor en cada actualización.

## Diseño

```
RML/
  Data/      <- lo del extractor, sin cambios
  Ajustes/   <- nueva: cambios a mano, puntuales, que el extractor no toca
    Nice Health Tab - 3328729902/
      LoadFolders.Build.yaml                              <- mismo formato que en Data/
      Languages/SpanishLatin/Keyed/Ajustes-3328729902.xml  <- <AllowFood>Comida</AllowFood>
```

- **Condicional:** el `PackageID` del yaml da el `IfModActive` de su entrada en `LoadFolders.xml`.
- **El extractor no la ve:** `ActualizacionPorLotes.Carpetas` y `Agrupador` solo recorren `Data/`.
  No hay que tocar el extractor.
- **Gana siempre:** las entradas de `Ajustes/` van al **principio** de cada versión en
  `LoadFolders.xml`, antes de las de `Data/`. El plan decía «al final», pero al hacerlo se miró el
  código del juego: `ModContentPack.InitLoadFolders` invierte la lista, así que la carpeta de más
  arriba se lee última y su clave pisa a las demás.
- **Nombre del archivo único:** `Ajustes-<WorkshopID>.xml`. Dos archivos con la misma ruta relativa
  en carpetas distintas de RML hacen que el juego cargue uno solo, sin avisar (ver «Cosas que ya
  pasaron» en `AGENTS.md`).
- Sirve también para `DefInjected`, con el mismo cuidado con el nombre.

## Pasos (una sesión, todo en `Source/LoadFoldersBuilder`)

1. **El builder lee `Ajustes/*/LoadFolders.Build.yaml`** y agrega sus entradas al final de
   `LoadFolders.xml` (`LoadFoldersWriter`, `Statics`). Prueba primero, si el builder tiene dónde;
   si no, comprobar con `-build` y mirar el `LoadFolders.xml`.
2. **`CopiaLimpia`** copia `Ajustes/` a `output/RimWorld Mod Latino/Ajustes/`.
3. **`Rutas`** (`-rutas`, `-copia`, la CI) revisa también `Ajustes/`: el largo con la ruta del
   Workshop y las rutas relativas repetidas contra `Data/`.
4. **`forceLoadAfter`** de `About.xml` incluye los packageId de `Ajustes/`, para cuando se corrija
   un texto propio del mod.
5. **`ModList.tsv` y `docs/index.html`** no listan `Ajustes/`.
6. **El primer ajuste:** Nice Health Tab, `<AllowFood>Comida</AllowFood>`, con su yaml copiado de
   `Data/!Andromeda/Nice Health Tab - 3328729902/LoadFolders.Build.yaml`.
7. **Documentación:** `Ajustes/` en «Estructura» de `AGENTS.md` y una sección corta: cuándo se usa
   (solo arreglos de espacio o de contexto, nunca para traducir), el nombre del archivo y que el
   extractor no la toca. Una línea en `TRADUCIR.md` y en `README.md` si hablan de la estructura.
8. **Verificar:** `-build`, `-rutas`, las tres comprobaciones del diff y, Angel, en el juego: la
   pestaña de salud de Nice Health Tab dice «Comida:» en una línea.

Con Nice Health Tab activo, la pestaña de salud del juego también dice «Comida:», porque es la
misma clave. Ese mod la reemplaza, así que no debería verse.

## Estado (22-oct-2026)

**Hecho:** pasos 1 a 7 y la parte de Claude del 8. `-build` pone
`Ajustes/Nice Health Tab - 3328729902` arriba de `v1.6` y `v1.7`, y `output/` lleva el archivo;
`-rutas` sale en verde, y en rojo si un ajuste repite un nombre de archivo de `Data/` (probado).
`ModList.tsv` y `docs/` no cambian. `loadfolders.yml` también corre con un push que toque `Ajustes/`.

**Falta (Angel):** abrir el juego con Nice Health Tab y mirar que la pestaña de salud diga
«Comida:» en una línea; y decir si se sube. Un push a `master` publica una versión nueva de RML.
