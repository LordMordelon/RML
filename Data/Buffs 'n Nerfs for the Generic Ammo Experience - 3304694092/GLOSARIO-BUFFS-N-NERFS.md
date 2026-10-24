# Glosario de Buffs 'n Nerfs for the Generic Ammo Experience

Complementa a [GLOSARIO.md](../../GLOSARIO.md) y al
[glosario de Combat Extended](../Combat%20Extended%20-%202890901044/GLOSARIO-COMBAT-EXTENDED.md), que
siguen mandando: si un término está en uno de ellos, gana ese. Acá va solo lo que este mod agrega o lo
que hubo que decidir para él. El plan está en [PLAN-BUFFS-N-NERFS.md](PLAN-BUFFS-N-NERFS.md).

El mod es un rebalanceo de The Generic Ammo Experience: casi todo lo que trae son recetas con otras
cantidades y descripciones reescritas de armas, armaduras e investigaciones de otros mods. Por eso los
nombres de munición son los de la carpeta de
[The Generic Ammo Experience](../The%20Generic%20Ammo%20Experience%20for%20Combat%20Extended%20-%203284460811/),
sin cambios.

Cada fila dice de dónde sale:

- **oficial**: la traducción del juego en `SpanishLatin`. No se cambia.
- **RML**: ya estaba así en otra carpeta de RML; se copió para que coincida.
- **propuesta**: no había nada. Se usa así hasta que Angel lo corrija; si cambia, se cambia acá y en
  los archivos.

## 1. Términos nuevos o decididos para este mod

| Inglés | Español | Origen | Notas |
|---|---|---|---|
| firefoam | espuma cortafuego | oficial | La munición, su categoría y el combustible de la torreta de espuma. `labelShort` de la categoría: «espuma» |
| fuel cell (`Ammoset_IncendiaryLauncher`) | célula de combustible | RML | Como `AmmoSet_Fuel` |
| smelt (ammo) | fundir | oficial («fundir arma») | «fundir munición de alto calibre» |
| disassemble (launcher) | desarmar | propuesta | «desarmar lanzador» |
| fuel pod | cápsula de combustible | propuesta | |
| biomass | biomasa | oficial (Anomaly) | |
| textiles, leathers, wools | textil, cuero, lana | oficial (categorías) | `customSummary`: «cualquier textil, cuero o lana» |
| refine (FSX) / dilute (FSX) | refinar / diluir | propuesta | «refinar quimbustible en FSX x10», «diluir FSX en quimbustible x100» |
| fire bomb | bomba incendiaria | RML | Como `SEX_Firebomb` de Generic Ammo |
| throwspikes | pinchos arrojadizos | RML | El objeto de VFE - Tribals es plural: «Fabrica unos pinchos arrojadizos.» |
| warcasket | sarcotraje | RML | Como VFE - Pirates. VAspirE usa «sarcófago de guerra»: no copiarlo |
| entombed (en un sarcotraje) | encerrado / encierro | propuesta | La condición de salud. «Sepultado» sonaba a muerto |
| patch (warcasket) | reparar | propuesta | La habilidad «reparar sarcotraje» |
| locust (Reinforced Mechanoid 2) | langosta | propuesta | El meca no está traducido en RML; «langosta» como «gigalangosta» de VFE - Insectoids 2 |
| machinist (tribal) | maquinista | propuesta | El tribal que lleva armas improvisadas |
| musket ball | bala de mosquete | RML (glosario de CE) | |
| slugthrower | lanzabalas | RML | Como VFE - Pirates. La investigación «lanzabalas» |
| scattergun | escopeta | RML | Como las escopetas láser de VWE - Laser |
| pump-action / lever-action | de corredera / de palanca | RML | |
| marksman rifle | fusil de tirador | RML | Como `Bullet_MarksmanRifle` |
| weapon emplacement / gun emplacement | emplazamiento de armas / de artillería | propuesta | |
| advanced optics | óptica avanzada | propuesta | |
| flame-spewer | escupellamas | propuesta | |
| grenade belt / munitions belt | cinturón de granadas / de munición | propuesta | |
| bandolier | bandolera | propuesta | |
| warwalker | caminante de guerra | propuesta | |
| neanderthals | neandertales | oficial («tribu feroz neandertal») | |
| urbworld | mundourbano | oficial | Una palabra, como «mundoborde» |
| toxicworld | mundotóxico | propuesta | No es oficial; armado como «mundourbano» |
| ordnance expert | experto en explosivos | propuesta | |
| entrance team | equipo de asalto | propuesta | |
| blowback-operated | accionado por retroceso | propuesta | La pistola automática conserva la frase oficial, «accionamiento por soplado» (ver el plan) |
| ballistic helmet (`Apparel_AdvancedHelmet`) | casco antibalas | oficial | El mod lo renombra en inglés, pero el oficial ya dice eso |
| charge blaster | bláster de cargas | oficial | |

## 2. Recetas

Siguen la sección 6 del glosario de Combat Extended, con una regla que acá pesa mucho:

- **La cantidad es la que da la receta**, no la que dice el inglés. El misil de fragmentación dice
  «x1» y da 5: va «fabricar misil (Frag) x5» y «Fabrica 5 misiles (Frag).». «Dilute FSX into chemfuel
  x10» da 100 quimbustible: va «x100».
- **De a uno**, con artículo y en singular: «Fabrica un misil (HEAT).» / «Fabricando misil (HEAT).»
- **Los nombres de munición son los de Generic Ammo**, aunque el inglés de este mod los escriba en otro
  orden: «HEAT missile» es «misil (HEAT)», «time-fuzed missile» es «misil (HE de espoleta de tiempo)»,
  «canister missile» es «misil (CAN)».

## 3. Reglas de las investigaciones

Las `rulesStrings` de las investigaciones van en `Patches/`, una regla por `<li>`, y cada una conserva
su prefijo (`subject->`, `subject_story->`, `subject_gerund->`). El `subject_gerund` va como sustantivo,
igual que en Generic Ammo: «fabricación de granadas». Las historias se escriben sin marcar género
cuando el inglés no lo dice: «sufrió heridas…» y no «resultó herido…».

Una regla de óptica avanzada viene vacía en inglés (`subject_story->`): se deja vacía.
