# Plan: The Generic Ammo Experience for Combat Extended

Copia de https://github.com/LordMordelon/RML/issues/4. Al avanzar, actualizar los dos.

## Avance

**22-oct-2026, sesión 1:** `DefInjected` y `Keyed` traducidos, cero `TODO` (326 renglones). Falta
`Patches/` (171 renglones, 16 archivos), en otra sesión.

- **Copia de Combat Extended** (decisión 1): 53 entradas por script, solo sobre `TODO` y solo si el inglés
  tenía una sola traducción en CE. Quedaron fuera a propósito las recetas (las arma el script de recetas con
  el nombre del producto de este mod) y `SEX_PistolSlugs.labelShort`: acá «slug» es la bala de uranio de
  pistola, no la bala de escopeta de CE. La única ambigua (`Gun_Autopistol.label`, «pistol») fue a mano.
  Revisadas una por una. Las de `Keyed` se pasaron a mayúscula inicial, como el inglés de este archivo.
- **Recetas** (decisión 2): 36 de 72 por script con las reglas de CE; las otras 36 a mano, porque el inglés
  tiene erratas («r grenades», «(HE) cartridges» en un misil, una sin cantidad) o el plural lleva adjetivo
  («células de combustible incendiarias», «granadas propulsadas»). Las 72 leídas y con la cantidad del def.

## Términos de este mod

Siguen al glosario de Combat Extended; esto es lo que este mod agrega. Usarlos igual en `Patches/`.

| Inglés | Español | Notas |
|---|---|---|
| lead slug | bala de plomo / balas de plomo | La munición improvisada, para cualquier arma |
| uranium slug | bala de uranio | `labelShort`: «bala» |
| fire bomb | bomba incendiaria | |
| rocket grenade | granada propulsada / granadas propulsadas | «granada propulsada (termobárica)» |
| charged intermediate rifle | cartucho de carga para fusil intermedio | Como «cartucho de carga para pistola» de CE. Juego y categoría: «munición de carga para fusil intermedio». (Conc.) e (Ion): «de carga concentrada», «de carga iónica» |
| charged demolition cell | célula de carga de demolición | |
| incendiary / thermobaric / foam fuel cell | célula de combustible incendiaria / termobárica / de espuma | |
| railgun sabot | sabot de cañón de riel / sabots de cañón de riel | Copiado de CE |
| hand cannon | cañón de mano | Juego: «munición de cañón de mano» |
| autoloader | cargador automático | de cartuchos / de gran calibre / de cañón / de misiles / incendiario |
| mounted (rocket launcher, flamethrower, gun) | lanzacohetes / lanzallamas montado, arma montada | |
| X auto-turret | torreta automática X | lanzagranadas, de escopeta, lanzallamas. *mini* → «minitorreta automática»; *light* → «torreta automática ligera» |
| X mortar shell | obús de mortero X | «obús de mortero explosivo», «… cortafuego», «… de antigrano» |
| mm sharp / mpa blunt | mm cortante / MPa contundente | Este mod reemplaza `CE_mmRHA` y `CE_MPa` de CE |

Las armas del juego base que el mod renombra llevan el nombre oficial si el inglés no cambia; si cambia, se
traduce lo que dice: «light machine gun» → «ametralladora ligera» (no «LMG»), «pistol» → «pistola», «pilum» →
«pilum». La frase de las torretas («Its dumb AI brain…») es la oficial de la minitorreta.

## Próxima sesión (22-oct-2026)

**Objetivo:** traducir The Generic Ammo Experience for Combat Extended
(`Data/The Generic Ammo Experience for Combat Extended - 3284460811`).

**Estado:** Angel ya lo extrajo; la carpeta está en el PC **sin commit** (junto con `About.xml`,
`LoadFolders.xml`, `ModList.tsv` y `docs/index.html`, que se regeneraron con la extracción). No se tradujo
nada todavía. `master` al día en `8e4b693`.

Contado el 22-oct-2026: **486 entradas en `TODO`** (497 renglones, contando cada elemento de lista aparte):

| Tipo | Renglones |
|---|---|
| `Patches/` (16 archivos; `z47okr.xml` tiene 112) | 171 |
| `ThingDef` (munición y proyectiles) | 159 |
| `RecipeDef` | 72 |
| `Keyed` (configuración) | 30 |
| `CombatExtended.AmmoCategoryDef` | 27 |
| `ResearchProjectDef` (3 archivos) | 15 |
| `CombatExtended.AmmoSetDef` | 14 |
| `ThingCategoryDef` | 6 |
| `StatDef` | 2 |
| `DesignatorDropdownGroupDef` | 1 |

**69** de esas entradas tienen el mismo inglés ya traducido en la carpeta de Combat Extended (13 de ellas con
más de una traducción posible). Las otras ~417 van a mano.

**Antes de empezarla (Angel):** nada. Si Combat Extended o el mod se actualizan antes, reextraer.

**Al empezar (Claude):**
1. Leer los dos glosarios de Combat Extended (abajo) y el `GLOSARIO.md` general.
2. Traducir `DefInjected` y `Keyed` (~326 renglones). Con eso se llena una sesión (estimado).
3. En otra sesión, `Patches/` (171 renglones).
4. Al terminar cada parte: cero `TODO` en lo tocado, XML bien formados, las tres comprobaciones de
   `RML/AGENTS.md`, `-copia` para el juego, y commit solo de la carpeta del mod.

**Decisiones de Angel:**
1. **Copiar las 69 traducciones idénticas de Combat Extended** con un script que solo llena `TODO` (como la
   tabla de recetas de Dead Man's Switch II); las 13 ambiguas, a mano. *Recomiendo que sí:* es el mismo texto
   y ahorra trabajo.
2. **Recetas por script** con las reglas de «Recetas por script» de `PLAN-COMBAT-EXTENDED.md`, que hoy solo
   valen para Combat Extended. *Recomiendo que sí:* las 72 recetas son el mismo patrón («make X ammo»).

**Guías:** `traspaso` al cerrar si no alcanza.

**Dónde leer:**
- `Data/Combat Extended - 2890901044/GLOSARIO-COMBAT-EXTENDED.md`
- `Data/Combat Extended - 2890901044/GLOSARIO-CE-COMPATIBILIDAD.md`
- `Data/Combat Extended - 2890901044/PLAN-COMBAT-EXTENDED.md` («Recetas por script»)
- `RML/AGENTS.md` («Editar XML de traducción» y las comprobaciones del diff)

Nota: también está instalado **Buffs 'n Nerfs for the Generic Ammo Experience** (3304694092), que no está en
RML. Si tiene texto, va en otra sesión.
