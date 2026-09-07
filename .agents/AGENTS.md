# Reglas para Agentes IA — RimWorld Mod Latino (RML)

## Proyecto

RML es un mod de traducción al **español latinoamericano** de mods de RimWorld.
Arquitectura derivada de [RimWorld Mod Korean (RMK)](https://github.com/RimWorldKorea/RMK):
una carpeta por mod traducido bajo `Data/`, con un `LoadFolders.Build.yaml` que declara
a qué mod se engancha y un `LoadFolders.xml` generado que hace que RimWorld cargue cada
traducción solo si ese mod está presente.

---

## Estructura del Repositorio

```
RML/
+-- About/About.xml                 metadatos del mod y supportedVersions
+-- Data/
|   +-- !<Autor>/                   autores con 4+ mods (! los fija arriba)
|   |   +-- <Nombre del mod> - <WorkshopID>/
|   |       +-- Languages/SpanishLatin (Español(Latinoamérica))/
|   |       |   +-- DefInjected/
|   |       |   +-- Keyed/
|   |       |   +-- Patches/
|   |       +-- LoadFolders.Build.yaml   <- packageId + binding + metadata
|   |       +-- UNUSED.xml              <- traducciones cuyo nodo ya no existe
|   +-- <Nombre del mod> - <WorkshopID>/   mods sueltos (autores con <4 mods)
|       +-- (misma estructura)
|
+-- LoadFolders.xml                 GENERADO — no editar a mano
+-- ModList.tsv                     GENERADO — índice de qué mods cubre RML
+-- actualizar.cmd                  regenera LoadFolders.xml y ModList.tsv
+-- publicar.cmd                    arma la copia limpia para el Workshop
+-- LoadFolders.Build.Example.yaml  referencia de campos del .yaml
+-- GLOSARIO.md                     terminología oficial de RimWorld ES
+-- TRADUCIR.md                     guía para colaboradores sin git
+-- CAMBIOS.md                      qué cambió respecto de RMK
+-- RESCATE-PENDIENTE.md            traducciones huérfanas y decisiones
|
+-- Source/
|   +-- LoadFoldersBuilder/         genera LoadFolders.xml a partir de los .yaml
|   +-- FileNameEncoder/            normaliza nombres de XML que NO vengan del extractor
|
+-- .github/workflows/
    +-- loadfolders.yml             regenera el índice en push
    +-- release.yml                 arma .zip para GitHub Releases
```

El extractor (programa externo) vive como carpeta hermana del repositorio:
```
INVESTIGAR/
  RML/                   <- este repo
  RimworldExtractor/     <- https://github.com/LordMordelon/RimworldExtractor
```

---

## Source/LoadFoldersBuilder/ — Generador de LoadFolders.xml (.NET)

Herramienta C# que lee todos los `LoadFolders.Build.yaml` bajo `Data/` y genera
`LoadFolders.xml` y `ModList.tsv`.

Archivos principales:

| Archivo | Descripción |
|---|---|
| Program.cs | Entry point. Acepta `-build` como argumento para modo no-interactivo |
| Statics.cs | Rutas, constantes y lógica de descubrimiento de carpetas |
| BuildRule.cs | Parseo y validación de `LoadFolders.Build.yaml` |
| BuildRuleYaml.cs | Modelo YAML de la regla de construcción |
| LoadFoldersWriter.cs | Genera el XML de `LoadFolders.xml` con entradas `IfModActive` |
| MigrationHelper.cs | Migra formatos antiguos del yaml |

Se ejecuta así:
```
dotnet run --project "Source\LoadFoldersBuilder" -c Release -- -build
```
O con doble clic en `actualizar.cmd`.

---

## Source/FileNameEncoder/ — Codificador de nombres (.NET)

Normaliza nombres de archivos XML usando un hash determinista.
**NO se corre sobre lo que genera el extractor** (ver CAMBIOS.md para la explicación).

Solo sirve para XML hechos a mano o sacados con otra herramienta:
```
dotnet run --project Source/FileNameEncoder/FileNameEncoder -- "Data/<Nombre del mod> - <WorkshopID>"
```

No toca la carpeta `Patches`: esos archivos ya vienen con nombres legibles y estables.

---

## LoadFolders.Build.yaml — Cómo funciona

Cada carpeta de mod en `Data/` lleva un `LoadFolders.Build.yaml` con:

```yaml
BuildRule:
  Binding:
    PackageID: ["adaptive.storage.framework"]   # packageId del mod traducido
    Mode: "None"                                # "All" | "Any" | "None"
    Dependency: "Independent"                   # "Independent" | "Dependent"
  Order:
    After: []           # packageIds que deben cargarse antes
    Before: []          # packageIds que deben cargarse después
  Version:
    Default: "1.5"      # versión de RimWorld
    LeftBoundary:       # límite inferior de versión
    RightBoundary:      # límite superior de versión
    Designate: []       # versiones adicionales
    Ban: []             # versiones excluidas
Metadata:
  WorkshopID: "3033901359"
  ModName: "Adaptive Storage Framework"
```

- `PackageID` → se usa para el atributo `IfModActive` en `LoadFolders.xml`.
- `WorkshopID` → número de la URL de Steam; se usa para `ModList.tsv`.
- `ModName` → nombre legible para el índice.

REGLA: Si agregas un mod nuevo, crea su `LoadFolders.Build.yaml` copiando otro existente
y luego corre `actualizar.cmd` para regenerar el índice.

---

## Flujo de Trabajo Completo

1. **Extractor** (programa externo, RimworldExtractor)
   Mod de Steam → `Data/<NombreMod> - <WorkshopID>/Languages/SpanishLatin (Español(Latinoamérica))/`
   Genera `.xlsx` (planilla para traducir) o XML con `TODO` (para edición directa).

2. **Traducción**
   - **Vía Excel:** Traducir en la columna F del `.xlsx`, luego convertir XLSX → XML con el extractor.
   - **Vía XML directo:** Reemplazar `TODO` por la traducción en cada archivo XML.

3. **Regenerar índice** (`actualizar.cmd`)
   Lee `LoadFolders.Build.yaml` de cada mod → genera `LoadFolders.xml` + `ModList.tsv`.
   También lo hace el extractor al terminar una "traducción rápida" y la GitHub Action al push.

4. **Publicar** (`publicar.cmd`)
   Copia `About/`, `Data/`, `LoadFolders.xml`, `ModList.tsv` y `LICENSE` a `salida/`.
   Verifica que la copia tenga exactamente los mismos archivos que el origen.

---

## Convenciones de Nomenclatura

Carpetas de mod en `Data/`:
- Formato: `<Nombre del mod> - <WorkshopID>` (sin codificar)
- Autores con 4+ mods: van bajo `Data/!<Autor>/`
- El `!` fija arriba en orden de directorio; underscore no sirve por las carpetas `[FSF]`, `[HRK]`, etc.

Idioma de destino: `SpanishLatin (Español(Latinoamérica))`
- Nombre exacto de la carpeta de idioma. NO usar `SpanishLatin` solo ni `Spanish`.

---

## Reglas para Agentes IA

1. **Glosario obligatorio.** Revisar `GLOSARIO.md` antes de traducir cualquier texto.
   Contiene la terminología oficial de RimWorld ES-LatAm. No inventar términos.

2. **No editar archivos generados.** `LoadFolders.xml` y `ModList.tsv` se regeneran con
   `actualizar.cmd`. Si necesitas cambios, edita los `LoadFolders.Build.yaml` de cada mod
   y corre el builder.

3. **No duplicar funciones de las herramientas .NET.** Si algo lo hace `LoadFoldersBuilder`
   o `FileNameEncoder`, no reinventar en Python ni PowerShell.

4. **Cambios en herramientas .NET.** Son proyectos C#/.NET. Verificar con
   `dotnet build Source/LoadFoldersBuilder` después de cambios.

5. **No romper compatibilidad de YAML.** Los `LoadFolders.Build.yaml` tienen formato fijo.
   No agregar campos sin actualizar `BuildRuleYaml.cs`.

6. **La carpeta de origen siempre es `Data/`.** Nunca editar la carpeta de salida `salida/`.

7. **Paths largos.** Las rutas en `Data/` superan los 254 caracteres. Usar `robocopy` y no
   `xcopy` para copias. El `publicar.cmd` ya lo hace así.

8. **Ver sección "Preferencia de Edición y Traducción de Archivos XML"** (más abajo).

9. **Revisar `GLOSARIO.md`** para convenciones de traducción. Esta regla se repite a propósito:
   es la más importante.

---

## Preferencia de Edición y Traducción de Archivos XML

- **Edición directa, sin excepción:** Toda modificación o traducción de archivos XML de
  RimWorld se realiza directamente sobre los archivos con las herramientas nativas de
  edición (`replace_file_content` / `multi_replace_file_content`). No se ejecutan scripts
  intermedios ni comandos de PowerShell/consola para editar contenido, sin importar el
  tamaño del archivo.

- **Búsqueda (única excepción):** Se permite el uso de herramientas de búsqueda
  (`grep_search`) exclusivamente para localizar etiquetas `TODO` o verificar términos
  pendientes en el proyecto. `grep_search` es solo lectura — nunca modifica archivos.

- **Preservación de estructura:** Mantén intacta la estructura XML, las etiquetas, los
  comentarios originales en inglés (`<!-- EN: ... -->`) y la codificación `UTF-8`.

- **Reemplazo exacto:** Sustituye únicamente los placeholders (`TODO`) por la traducción
  correspondiente en Español Latino, cuidando la coherencia terminológica del juego.

- **Especificadores de color:** Al traducir texto que contiene especificadores de color de
  RimWorld, escribe las etiquetas con ambos símbolos escapados: `&lt;` en lugar de `<` y
  `&gt;` en lugar de `>`. Ejemplo: `&lt;color=#RRGGBB&gt;texto&lt;/color&gt;`. Traduce solo
  el texto interior; nunca alteres el valor del color ni la estructura de la etiqueta.

- **Identificadores Def:** No modificar nunca los identificadores de defs ni las rutas de
  etiquetas XML (p. ej. `OpportunitySite_AbandonedMechanitorPlatform.questDescriptionRules`,
  `PMP_CivilMechhive_Mechanitor`, etc.).

---

## Reglas Estrictas de XML

- Las etiquetas de color deben usar entidades XML escapadas:
  `&lt;color=#RRGGBB&gt;texto&lt;/color&gt;`
  (NUNCA `<color=...>` literal sin escapar, rompe el analizador XML de RimWorld).

- Dentro de `<!-- EN: ... -->` se respeta la etiqueta y el texto original en inglés sin
  alterar.

- Todos los archivos deben mantenerse en `UTF-8`.

- **No traducir:**
  - Nombres propios de mods (p. ej. *Save Our Ship 2*, *Biotech*, *Royalty*).
  - Placeholders dinámicos: `{...}`, `{0}`, `{PAWN_nameDef}`, `[resolvedQuestName]`, etc.
  - Secuencias literales de escape como `\n`.
  - La palabra `Vanilla` si se refiere al motor o contenido base no traducido.
