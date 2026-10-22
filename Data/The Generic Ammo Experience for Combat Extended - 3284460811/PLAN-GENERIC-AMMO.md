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

Las armas del juego base que el mod renombra llevan siempre el nombre oficial, aunque el inglés del mod lo
cambie (Angel, 22-oct-2026): «light machine gun» → «LMG», «pistol» → «pistola automática», «pilum» → «pila». La frase de las torretas («Its dumb AI brain…») es la oficial de la minitorreta.

## Próxima sesión (22-oct-2026, después de la sesión 1)

**Objetivo:** traducir los `Patches/` de The Generic Ammo Experience for Combat Extended.

**Estado:** `DefInjected` y `Keyed` traducidos y en el commit `8356cce` de `master`, **sin subir**. Quedan
**156 `TODO` en `Patches/`** (contados; 16 archivos, 99 de ellos en `z47okr.xml`). Los 15 que ya estaban en
Combat Extended se copiaron por script. `About.xml`, `LoadFolders.xml`, `ModList.tsv` y `docs/index.html`
siguen modificados sin commit: son los generados de la extracción.

**Antes de empezarla (Angel):**
1. Decir si se sube el commit `8356cce` ahora o junto con `Patches/`.

**Al empezar (Claude):**
1. Leer «Términos de este mod» en `PLAN-GENERIC-AMMO.md` y los glosarios de Combat Extended.
2. Traducir los 156 `TODO` de `Patches/` a mano. Son compatibilidades con otros mods (Highfleet Turrets,
   Impact Weaponry, Grenadier rifles, Dead Man's Switch, etc.): el nombre de cada arma se copia de la carpeta
   de su mod en RML si existe. Hay 30 textos de recetas (contados): van a mano con las reglas de CE (sección 6 del glosario).
   Caben en una sesión (estimado).
3. Cero `TODO`, XML bien formados, las tres comprobaciones de `RML/AGENTS.md`, `-copia`, y commit solo de la
   carpeta del mod.

**Decisiones de Angel:** ninguna pendiente. Las de la sesión 1 (copiar de CE y recetas por script, con
revisión) ya se aplicaron; no se extienden a `Patches/` sin volver a preguntar.

**Guías:** `traspaso` al cerrar si no alcanza.

**Dónde leer:**
- `Data/The Generic Ammo Experience for Combat Extended - 3284460811/PLAN-GENERIC-AMMO.md` («Avance» y «Términos de este mod»)
- `Data/Combat Extended - 2890901044/GLOSARIO-COMBAT-EXTENDED.md` y `GLOSARIO-CE-COMPATIBILIDAD.md`
- `RML/AGENTS.md` («Editar XML de traducción» y las comprobaciones del diff)

Nota: **Buffs 'n Nerfs for the Generic Ammo Experience** (3304694092) sigue fuera de RML; si tiene texto, va
en otra sesión.
