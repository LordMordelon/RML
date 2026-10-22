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

**22-oct-2026, sesión 2:** `Patches/` traducido a mano, cero `TODO` en todo el mod (156 renglones, 16
archivos). Sin scripts: cada texto con el editor, sobre su `TODO`.

- **Nombres de armas y defs de otros mods** copiados de su carpeta en RML (VWE, Quickdraw, VFE Pirates,
  Deserters, Empire, VAE, VAEA) o del juego oficial («lanza de cargas», «arco grande», «escudos»,
  «obús tóxico», «obús de necrovita»). Las descripciones que el mod reescribe se tradujeron desde su inglés,
  reusando las frases de la traducción que RML ya tenía.
- **Renombres del mod que chocan con el juego:** «automatic pistol» de Quickdraw queda «pistola ligera», como
  en RML, porque «pistola automática» ya es la del juego base. Los demás renombres van como dice el mod:
  «carabina», «SMG», «ametralladora pesada», «cañón automático portátil».
- **Ojo, en el juego:** cuando el mod dueño del def tiene su propia carpeta en RML (VWE, VFE, VAE…), el
  `DefInjected` de esa carpeta se aplica después del patch y gana, así que se ve el nombre de esa carpeta y no
  el de este mod. Lo mismo con los defs del juego oficial. No es un error de esta traducción.
- **Recetas de `Patches/`** (Highfleet Turrets, Impact Weaponry, Save Our Ship 2): con las reglas de CE, el
  nombre del producto y la cantidad del def (todas coinciden con el inglés).
- **Permisos de VFE Empire** con la plantilla oficial: «solicitar …» / «Solicita un pedido de transporte
  aéreo de …».

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
| superheavy (shell, rocket, uranium slug) | ultrapesado/a | «obús de cañón ultrapesado», «cohete ultrapesado», «bala de uranio ultrapesada». Como los «mecas ultrapesados» de RML |
| Proximity Fuse | espoleta de proximidad | «obús de cañón ultrapesado (espoleta de proximidad)» |
| grenadier rifle | fusil con lanzagranadas | «fusil de batalla con lanzagranadas», «fusil de servicio con lanzagranadas» (los nombres de VWE en RML) |
| impact shell | proyectil de impacto | Como `DV_Bullet_ImpactCannon` de Impact Weaponry. `labelShort`: «impacto» |
| starship parts | piezas de nave estelar | Save Our Ship 2 |
| throwspikes / smashing rock | púas arrojadizas / piedra aplastante | VFE Tribals; no tenían traducción en RML |
| harquebusier / warbow | arcabucero / arco de guerra | Como los arcabuceros de Dead Man's Switch |

Las armas del juego base que el mod renombra llevan siempre el nombre oficial, aunque el inglés del mod lo
cambie (Angel, 22-oct-2026): «light machine gun» → «LMG», «pistol» → «pistola automática», «pilum» → «pila». La frase de las torretas («Its dumb AI brain…») es la oficial de la minitorreta.

## Pendiente

- **Subir** los commits de las dos sesiones (`8356cce` y siguientes, sin subir al 22-oct-2026): lo decide Angel.
- **Probar en el juego** con Combat Extended y este mod: que la munición, las recetas y la configuración salgan en
  español.
- `About.xml`, `LoadFolders.xml`, `ModList.tsv` y `docs/index.html` siguen modificados sin commit: son los
  generados de la extracción, y la CI los rehace al subir.

Nota: **Buffs 'n Nerfs for the Generic Ammo Experience** (3304694092) sigue fuera de RML; si tiene texto, va
en otra sesión.
