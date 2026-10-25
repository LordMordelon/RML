# Plan: ajustes de Combat Extended (para revisar)

Salió de revisar la familia de AobaKuma el 25-oct-2026 ([PLAN-AOBAKUMA.md](../!AobaKuma/PLAN-AOBAKUMA.md)).
Son tres ajustes independientes; cada uno es una tarea propia. **Nada de esto está hecho ni aprobado
todavía**: Angel lo revisa y decide.

| # | Ajuste | Dónde | Tamaño | Qué decide Angel |
|---|---|---|---|---|
| 1 | La condición de Combat Extended en los `Patches/` | Extractor | 1 sesión, más reextraer | Aprobar la forma de arreglarlo |
| 2 | Cuatro términos que contradicen a la oficial | RML (glosario de CE y textos) | 1 sesión | Nada: ya rige la regla de la oficial. Solo revisar el resultado |
| 3 | *slug* y *airburst*: CE contra la familia DMS | Glosarios | Una decisión | Cuál de las dos formas va en la munición de CE |

## 1. La condición de Combat Extended en los `Patches/`

**Qué pasa.** Cuando un mod traduce algo que solo existe con Combat Extended, el parche de RML lleva
una condición: «aplicar solo si está CE». Al reextraer, el extractor la escribe por **packageId**
(`MayRequireAnyOf="CETeam.CombatExtended,CETeam.CombatExtended_steam"`), aunque el mod original la
pidiera por **nombre** (`PatchOperationFindMod` «Combat Extended»). El CE que Angel usa en 1.7 es el no
oficial: se llama «Combat Extended», pero su packageId es `CETeam.CombatExtended.17UnofficialVer`. Por
nombre se cumple; por packageId, no. Resultado: esos parches de RML no se aplican y el texto sale en
inglés, sin ningún aviso.

**Dónde está hoy.** Siete archivos de `Patches/` de la familia de AobaKuma (ocho bloques) ya tienen la condición por
packageId: Synthetic, MobileDragoon (dos bloques), AncientCorps, DMS II (subido en `4424e32f`),
Expedition, Ascension Megacorp y Taktikmagier. El 21-oct se habían devuelto a git los de 10 mods; la
reextracción del 25-oct los volvió a escribir así, y esta vez se commitearon (decisión de Angel).

**Un dato que cambia el arreglo.** Los mods de AobaKuma cargan su propio contenido para CE con
`IfModActive="CETeam.CombatExtended,CETeam.CombatExtended_steam"` en su `LoadFolders.xml`, o sea por
packageId: con el CE no oficial, **tampoco el mod carga su carpeta de CE** (el autor lo dice en el de
Expedition: «Combat Extended no tiene versión 1.7, así que 1.7/CE no se carga»). En cambio, los parches
que el mod pide por nombre sí se aplican con el CE no oficial. Los que se pierden son esos: MobileDragoon
tiene dos.

**Propuesta: que RML pida CE igual que el mod.**

| El mod pide CE… | RML escribe… | Con el CE no oficial |
|---|---|---|
| por nombre (`PatchOperationFindMod` «Combat Extended») | por nombre, igual | Se aplican los dos, el del mod y el de RML |
| por packageId (`IfModActive`, `MayRequire`) | por packageId, los mismos | No se aplica ninguno de los dos, y está bien: no hay nada que traducir |

Así la traducción se aplica exactamente cuando existe lo que traduce, con cualquier versión de CE. La
otra forma que se consideró el 21-oct, agregar a mano el packageId del no oficial, sirve solo mientras
exista esa versión y hay que repetirla en cada mod.

**Qué tocar en el extractor** (`RimworldExtractor/`):
- `IO.cs`, donde arma la condición (`porPackageId`, «MayRequire en un li de lista…»): hoy convierte todo
  a packageId. Tiene que conservar la forma por nombre cuando la operación original era un `FindMod`.
- `ModLister.FindModsNamedInPatches` y `ModsConPackageId`: de ahí salen los nombres y los packageId.
- Prueba primero, que falle antes del arreglo: un mod con un `FindMod` «Combat Extended» y el CE
  instalado como `CETeam.CombatExtended.17UnofficialVer` tiene que salir con `FindMod` «Combat Extended».
  Otra con `IfModActive` por packageId tiene que salir con los mismos packageId.
- De paso: `CombatExtendedDevuelveSuMunicionGenerica` sale omitida porque busca el packageId oficial.

**La falla de MobileDragoon va en la misma tarea.** Tres `chargeNoun` («misil», «misil», «cohete»)
volvieron a `TODO` porque su xpath pasó de posición (`comps/li[5]`) a clase
(`comps/li[@Class="CompProperties_ApparelReloadable"][1]`) y el cruce no reconoció la clave vieja; no
quedaron en el `UNUSED.xml`. Prueba: un `Patches` de RML con el xpath por posición tiene que conservar su
traducción al reextraer con el xpath por clase. (Los tres textos se escriben a mano en la próxima sesión
de AobaKuma; esto es para que no vuelva a pasar.)

**Después:** reextraer los mods con `Patches/` condicionados a CE, mirar el diff con las tres
comprobaciones, y que Angel pruebe en el juego un texto de MobileDragoon con CE activo.

El detalle anterior está en «Tarea 1» de `RimworldExtractor/PLAN-POSICIONES.md`.

## 2. Cuatro términos que contradicen a la oficial

Por la regla del 25-oct-2026 (si un glosario contradice a la oficial, gana la oficial). Los cuatro son
textos que el juego muestra en la misma ficha que CE, así que hoy se ven las dos formas juntas:

| Inglés | Glosario de CE hoy | Oficial | Dónde lo usa la oficial | Archivos de RML con el término viejo |
|---|---|---|---|---|
| armor penetration | penetración de armadura | **penetración de blindaje** | `Keyed:ArmorPenetration` | 20 (5 de CE; A RimWorld of Magic, VWE, VFE - Pirates, Vanilla Skills…) |
| stopping power | poder de detención | **potencia de parada** | `Keyed:StoppingPower` | 9 (3 de CE; Custom Weapons Expanded, Customize Weapon, Nice Bill Tab) |
| fire rate | cadencia de fuego | **cadencia de tiro** | `Keyed:BurstShotFireRate` | 23 (2 de CE y otros mods de armas) |
| hold fire | alto el fuego | **cese al fuego** | `Keyed:CommandHoldFire` (el botón de no disparar) | 5 (1 de CE) |

**Cuidado con «alto el fuego»:** en RimPacts y en DMS probablemente traduce *ceasefire* (una tregua
entre facciones), que es otro sentido y se queda. Solo cambia donde el inglés es *hold fire*. Lo mismo
vale para los otros tres: se revisa cada uso con su `EN:` al lado, no se reemplaza a ciegas.

**Cómo:** primero el glosario de CE (secciones 1 y 2), con una nota de cuándo cambió; después los
textos, a mano y mod por mod, un commit por mod, sin tocar los `UNUSED.xml`; al final, las tres
comprobaciones del diff. La comprobación 1 va a mostrar estos cambios de valor: son los buscados, pero
se miran uno por uno.

**No cambian** (se revisaron y son otro concepto): la «supresión» de CE (estar bajo fuego), que no es la
«represión» oficial de los esclavos; «casilla» (*tile* del mapa), que no es la «baldosa» del piso;
«obús» (*shell*), que la oficial ya usa para la munición.

## 3. *slug* y *airburst*: CE contra la familia DMS

| Inglés | Glosario de CE | Glosario de DMS (aprobado el 20-oct) |
|---|---|---|
| slug | **bala de escopeta**; en el nombre del cartucho, «(bala)» (24 usos en CE) | **posta**; *rubber slug* «posta de goma» |
| airburst | **de detonación aérea** (36 usos en CE) | **de estallido aéreo**; «cartucho de estallido aéreo» |

Ninguna de las cuatro es oficial, así que la regla de la oficial no decide. Importa para la munición de CE
que traen Expedition, Ascension y Cinders, que aparece en las mismas listas de CE que la de cualquier mod.

**Recomendación:** en la munición de CE, la forma de CE («bala de escopeta», «de detonación aérea»),
para que toda la munición de CE se nombre igual. En la munición propia de DMS, que no es de CE (la
KS-23K de AncientCorps, por ejemplo), la de la familia. Si Angel lo aprueba, se anota en
`GLOSARIO-DMS.md`, en «Reglas comunes» de `PLAN-AOBAKUMA.md` y en los glosarios de Expedition, Ascension
y Cinders, antes de traducir esos tres.

## Orden sugerido

1. **Decidir el punto 3** antes de la sesión de Expedition: es una línea y cambia cómo se traduce.
2. **Punto 1** (extractor) en una sesión propia, cuando Angel lo apruebe. Para pedirla: «Extractor: la
   condición de Combat Extended según PLAN-CE-AJUSTES.md».
3. **Punto 2** en otra sesión. Para pedirla: «RML: los términos de Combat Extended que contradicen a la
   oficial, según PLAN-CE-AJUSTES.md».
