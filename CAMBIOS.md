# Qué cambia respecto de RMK

Las herramientas de `Source/` derivan de [RimWorldKorea/RMK](https://github.com/RimWorldKorea/RMK),
el proyecto coreano que inventó esta arquitectura: una carpeta por mod traducido, con un
`LoadFolders.Build.yaml` que dice a qué mod se engancha, y un `LoadFolders.xml` generado que hace
que RimWorld cargue cada traducción solo si ese mod está presente.

Eso se conservó tal cual. Lo que cambió es todo lo de alrededor.

---

## El builder, ahora automatizable

`LoadFoldersBuilder` no se podía lanzar desde otro programa. Cuatro cosas, y ninguna era cosmética:

- `-build` se leía **solo por teclado**, nunca de los argumentos.
- Con la entrada redirigida, `Console.ReadLine()` devuelve `null`, caía en el `default` del switch
  y **giraba para siempre**.
- `ClearLastLine()` usa `Console.CursorTop`, que tira excepción cuando la salida está redirigida.
  En CI ni llegaba a colgarse: reventaba.
- `StopProgram()` terminaba siempre en 0, **incluso después de un error**, así que un fallo habría
  pasado en verde por la CI.

Ahora acepta `-build` como argumento, no pregunta nada en ese modo y devuelve un código distinto de
cero si algo falló. El modo interactivo quedó igual.

Se quitaron el modo `-migrate` y `MigrationHelper.cs`, que convertían la planilla de Google de RMK en
los `LoadFolders.Build.yaml`: una migración de una sola vez que acá nunca se usó. En RML los yaml los
genera el extractor, mod por mod.

---

## El índice se regenera solo

`LoadFolders.xml` es lo que hace que un mod cargue. Antes había que acordarse de regenerarlo, y
olvidarse no daba ningún error: la traducción simplemente no aparecía.

Ahora se rehace en los dos caminos que importan: **el extractor lo corre** al terminar una
traducción rápida, y **una GitHub Action** lo corre al hacer push. `regenerar-indice.cmd` queda para los
cambios hechos a mano.

La Action va en `windows-latest` a propósito: `Statics.cs` arma la ruta como
`Path.Combine(Location, "About\About.xml")`, con la barra invertida cableada, así que en Linux no
encuentra el `About.xml`.

---

## `FileNameEncoder` salió del flujo

**No hay que correrlo sobre lo que genera el extractor.** Los dos calculan el nombre con la misma
función pero con entradas distintas:

| | Entrada del hash |
|---|---|
| Extractor | nombre del mod + `Clase\|ArchivoOrigen` |
| FileNameEncoder | nombre de la carpeta en `Data/` + `Clase` |

Como el extractor borra y reescribe la carpeta entera en cada actualización, correr el encoder
encima renombraría todo y la siguiente extracción lo devolvería a su nombre original. **Cada ciclo
sería un borrar+agregar completo en git** en vez de un diff línea por línea.

Sigue en el repositorio para XML que vengan de otro lado. Y se le sacó el renombrado de `Patches`:
esos archivos ya vienen nombrados con el mod al que le aplica cada uno, que es legible y estable.

---

## Organización de `Data/`

Los mods de un mismo autor con cuatro o más traducciones van agrupados en `Data/!Autor/`. El `!`
los fija arriba al ordenar; el guión bajo no servía, porque en orden ordinal cae después de las
carpetas `[FSF]`, `[HRK]` y `[sbz]`.

El extractor busca la carpeta de un mod en cualquier nivel bajo `Data/`, así que agrupar o
desagrupar no rompe nada.

---

## Documentación

Nada de esto existía en RMK, que documenta en coreano:

- **[TRADUCIR.md](TRADUCIR.md)** — la guía para colaborar traduciendo sin saber programar.
- **[GLOSARIO.md](GLOSARIO.md)** — cómo se traduce cada término, siguiendo la traducción oficial de
  RimWorld. Es lo que hace que dos personas traduzcan igual.
- **[RESCATE-PENDIENTE.md](RESCATE-PENDIENTE.md)** — qué traducciones quedaron sin recuperar y por
  qué, para que la decisión no se rediscuta desde cero.

Todo en español neutro: tuteo, sin voseo y sin regionalismos, porque lo leen traductores de toda
Latinoamérica.
