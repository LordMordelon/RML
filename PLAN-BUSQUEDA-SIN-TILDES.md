# Plan: buscar sin distinguir tildes

**Estado: hecho como mod aparte y probado en el juego por Angel en 1.7** (28-oct-2026). Funciona el buscador del
mapa (Z). La primera versión ponía el prefijo en `Dialog_Search<T>.TextMatch` y en 1.7 no hacía nada, sin ningún
error: ahora se cambian las que la llaman (`Dialog_MapSearch.CanAddThing`, `Dialog_WorldSearch.ElementMatch`) con
un transpilador. `QuickSearchFilter.MatchImpl` sigue con prefijo; si algún buscador de esos no anda, va igual. Angel cambió de idea después de la
búsqueda de abajo: no va en RML, sino en un mod propio, **Buscar sin tildes** (`Mordelon.BuscarSinTildes`), en la
carpeta `BuscarSinTildes/`, al lado de RML. RML no cambia ni pide Harmony. Del plan de abajo, lo que cambió al
hacerlo:

- Sin casilla en los ajustes: para apagarlo se quita el mod, que no guarda nada en la partida.
- La comparación va con una tabla de letras y no con `string.Normalize`, por si el .NET 10 de la 1.7 no la trae
  completa.
- Harmony: la 1.6 trae la 2.4.3 y la 1.7 la 3.0. Los mods de 1.7 instalados están compilados contra cualquiera de
  las dos, así que va una sola DLL compilada contra la 2, como la de RML.

## El problema

En los buscadores del juego, «Modulos» no encuentra la «Mesa de módulos de armas», y «Módulos» y «módulos»
sí la encuentran (capturas de Angel en el buscador del mapa). Las mayúsculas no importan; las tildes, sí. Con las
traducciones al español, quien escribe sin tilde no encuentra nada.

## Dónde compara el juego (RimWorld 1.6, decompilado)

- `RimWorld.QuickSearchFilter.MatchImpl(string)`: la usan casi todos los buscadores (órdenes de trabajo,
  almacenes, menú de construcción, y el de Nice Bill Tab, que pasa por `QuickSearchWidget`). Compara con
  `IndexOf(texto, StringComparison.InvariantCultureIgnoreCase)`.
- `RimWorld.Dialog_Search<T>.TextMatch(string)`: el buscador del mapa (tecla Z, `OpenMapSearch`), que es
  `Dialog_MapSearch : Dialog_Search<Thing>`, y el del mapa del mundo (`Dialog_WorldSearch`). Compara igual, por
  su cuenta. Es de un tipo genérico: el parche va sobre cada tipo cerrado (`Dialog_Search<Thing>` y el del mundo),
  no sobre el abierto.

## Qué se haría

1. **En la DLL de RML** (`Source/RmlVentana`), no como mod aparte (decisión de Angel).
2. Una comparación en `Logica/` que no distingue tildes ni mayúsculas, **pero sí la ñ** (decisión de Angel:
   «pena» no encuentra «peña»). `CompareOptions.IgnoreNonSpace` sola la iguala a la n, así que la ñ se separa antes.
3. Dos parches de Harmony que la usan en esos dos puntos, si la casilla está encendida.
4. Una casilla en los ajustes de RML, «Buscar sin distinguir tildes», encendida de entrada, con sus textos en
   `Common/Languages/SpanishLatin` y `English`.
5. Pruebas primero, en `Source/RmlVentana.Tests`: «modulos» encuentra «Módulos», «ENERGIA» encuentra «energía»,
   «pena» no encuentra «peña».
6. RML pasa a pedir Harmony: `modDependencies` y `loadAfter` de `brrainz.harmony` en `About/About.xml`, y el paquete
   `Lib.Harmony` con `ExcludeAssets="runtime"` en el `.csproj`, para no copiar la DLL de Harmony a `Common/Assemblies`.
7. Angel lo prueba en el juego (1.6 y 1.7) antes de subirlo.

## A revisar antes de empezar

- **Qué Harmony usa la 1.7.** Una fuente dice que la 1.7 corre .NET 10 con Harmony 3, y la 1.6 Harmony 2.4.3 en
  .NET Framework 4.7.2. Hay que confirmarlo contra el juego instalado y ver si una sola DLL sirve para las dos.
- **Buscadores propios de otros mods.** Los que no pasan por `QuickSearchFilter` no se arreglan; se ven de a uno
  si aparecen.
- **Costo.** `QuickSearchFilter` guarda los resultados en caché, así que el parche corre una vez por texto.
  `TextMatch` no tiene caché: medir con un mapa grande.

## Mods que ya lo hacen

Se buscó el 28-oct-2026 y no apareció ninguno. RimSearch (1.4) es un buscador del mapa, no cambia cómo se compara
el texto.
