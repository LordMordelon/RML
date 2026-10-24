# Plan: Buffs 'n Nerfs for the Generic Ammo Experience

Mod de WhoUp (`WhoUp.TweakingThey.BalanceRn`, Workshop 3304694092), solo para 1.6. Rebalancea The
Generic Ammo Experience: cambia las cantidades de casi todas las recetas de munición y reescribe
descripciones de armas, armaduras e investigaciones de otros mods (VWE, VAE, VFE - Medieval 2,
VFE - Pirates, VFE - Tribals, Reinforced Mechanoid 2, Grenade Variety). Los términos están en
[GLOSARIO-BUFFS-N-NERFS.md](GLOSARIO-BUFFS-N-NERFS.md).

## Estado

**Traducido entero el 24-oct-2026**: 251 entradas en 11 archivos, sin `TODO`.

| Archivo | Qué tiene | Entradas |
|---|---|---|
| `DefInjected/RecipeDef` | Recetas nuevas: fundir munición, espuma cortafuego, FSX, molotov y bomba de palo de a uno | 39 |
| `DefInjected/ThingDef` | Espuma cortafuego, y descripciones del juego que el mod cambia (casco antibalas, pistola automática, pistola ametralladora, piel de oso, torreta de espuma) | 9 |
| `DefInjected/` (otras) | Categoría y juego de munición de la espuma, célula de combustible | 5 |
| `Patches/74sgqv.xml` | Por mod: investigaciones de VWE con sus reglas, sarcotraje, bláster de cargas, recetas de flechas y paquetes de carga | 118 |
| `Patches/wywkdw.xml` | 23 recetas de Generic Ammo con la cantidad nueva | 69 |
| `Patches/` (otros cuatro) | Descripciones de VFE - Medieval 2, VAE, VWE y VWE - Laser | 11 |

Comprobado: los XML se leen bien, solo cambiaron los renglones con `TODO`, los saltos de línea quedaron
como los escribió el extractor y las tres comprobaciones del diff de [AGENTS.md](../../AGENTS.md) salen
vacías. **Falta verlo en el juego.**

## Decisiones que conviene que Angel confirme

1. **La cantidad sale de la receta**, no del inglés: «fabricar misil (Frag) x5» aunque el mod diga x1, y
   «diluir FSX en quimbustible x100» aunque diga x10. Es la regla de Combat Extended.
2. **«entombed» es «encerrado»** (condición de salud del sarcotraje). VAspirE dice «sepultado», que
   suena a muerto.
3. **«locust» es «langosta»**: el meca de Reinforced Mechanoid 2 no está traducido en RML, así que no hay
   con qué coincidir. Si algún día se traduce ese mod, cambiarlo acá.
4. **«mundotóxico»** para *toxicworld*: no es oficial, se armó como el oficial «mundourbano».
5. **La pistola automática conserva su frase oficial** («accionamiento por soplado») y solo suma «y muy
   fácil de usar». En la investigación de VWE *blowback-operated* va «accionadas por retroceso», que es
   lo correcto; cambiar el texto oficial no tocaba.

## Pendiente: las cantidades de las recetas no se van a ver bien

Este mod reemplaza recetas de Combat Extended y de Generic Ammo con otras cantidades, pero RML ya tiene
esas recetas traducidas con la cantidad vieja. Con el mod activo, el juego muestra la etiqueta de RML:
por ejemplo «fabricar cartucho de pistola (FMJ) x500», cuando la receta da 200.

- **110 recetas** están así: 91 traducidas en la carpeta de Combat Extended y 19 en la de Generic Ammo.
  Entre ellas, las de FSX de Combat Extended («sintetizar FSX x8», que con este mod da 1).
- **23 de ellas sí tienen la traducción nueva**, en `Patches/wywkdw.xml`, pero no se va a ver: RimWorld
  aplica los `Patches` y después inyecta los `DefInjected`, así que gana la de Generic Ammo, con la
  cantidad vieja. Es la misma trampa que «Un `Patches` no le puede ganar a la traducción oficial» del
  `AGENTS.md` del extractor.
- **Las otras 87 no se extrajeron**: el mod reemplaza la receta entera, y el extractor no la escribe
  porque la clave ya está traducida en otra carpeta.

Cómo se lista: comparar el `<label>` de cada `RecipeDef` del mod instalado
(`D:\SteamLibrary\steamapps\workshop\content\294100\3304694092\1.6`) con el comentario `EN:` de la misma
clave en los `DefInjected/RecipeDef` de `Data/`, y quedarse con las que tienen otro número.

Dos caminos, para que Angel elija:

- **A. Un ajuste** en `Ajustes/Buffs 'n Nerfs for the Generic Ammo Experience - 3304694092/`, con un
  `DefInjected/RecipeDef/Ajustes-3304694092.xml` que repita esas 110 etiquetas y descripciones con la
  cantidad nueva. Gana porque los ajustes van arriba, y solo se carga con este mod activo. Es un arreglo
  de contexto (otro mod cambia el número), que es para lo que existe `Ajustes/`, pero son unas 330
  entradas que nadie revisa al reextraer: si el mod vuelve a cambiar una cantidad, hay que acordarse de
  tocarlo a mano.
- **B. Cambiar el extractor** para que, cuando un mod reemplaza una receta que otra carpeta ya traduce,
  la escriba como `DefInjected` en su propia carpeta y no como `Patches`. Es más limpio y sirve para
  otros rebalanceos, pero toca cómo se generan los archivos y hay que ver qué carpeta gana entre dos
  `DefInjected` con la misma clave.

Mientras no se elija, el mod se puede subir así: todo lo demás se ve bien, y las recetas solo muestran
un número que no corresponde.
