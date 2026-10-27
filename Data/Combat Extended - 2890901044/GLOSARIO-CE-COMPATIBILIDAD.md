# Glosario de Combat Extended: compatibilidad con otros mods

Lo que agregan las carpetas `ModPatches/` de Combat Extended, la munición y los ajustes que se
cargan solo si otro mod está activo. Va aparte a pedido de Angel (19-oct-2026):
[GLOSARIO-COMBAT-EXTENDED.md](GLOSARIO-COMBAT-EXTENDED.md) queda para el núcleo y el juego base, y
[GLOSARIO.md](../../GLOSARIO.md) sigue mandando sobre los dos.

**La regla de esta parte:** el arma ya tiene nombre en la carpeta de su mod en RML, así que la
munición lo copia, aunque no sea el que habría elegido el núcleo. Lo que el núcleo ya fijó (tipos,
orden de las palabras, recetas, descripciones repetidas) se usa tal cual. Origen de cada fila:

- **RML**: el nombre sale de la traducción del mod en RML.
- **oficial**: traducción oficial del juego.
- **propuesta**: sin nombre previo; se usa así hasta que alguien lo corrija acá.
- **Angel**: decisión de Angel.

## Por mod

| Mod | Inglés | Español | Origen | Notas |
|---|---|---|---|---|
| A RimWorld of Magic | ice shard / fire blast / lightning bolt / blazing power | esquirla de hielo / ráfaga de fuego / rayo / poder ardiente | RML (los dos últimos), propuesta | Proyectiles de las varitas |
| Alpha Mechs | coil lance | lanza de bobina | RML | «disparo de lanza de bobina» |
| Alpha Mechs | soul drain / cryogenic pellet | drenaje de alma / perdigón criogénico | RML | |
| Alpha Mechs | Acidic (tipo) | ácida | propuesta | «5x50mm sin vaina (ácida)», como «(tóxica)» |
| Alpha Mechs | Hyper (bala) | hiper | propuesta | «bala 12x64mm hiper» |
| Biological Warfare | flesh-breaker | rompecarne | RML | |
| Biological Warfare | flu / malaria / plague / scaria / sleeping sickness | gripe / malaria / peste / escariosis / enfermedad del sueño | oficial | Entre paréntesis: «granada 40x46mm (peste)» |
| Biological Warfare | necroa | necroa | RML | |
| Biological Warfare | shambler | caminante | RML | Como en el mod y en VQE Deadlife |
| Erin's Auraeyl | lightbow arrow | flecha de arco ligero | RML | «flecha de arco ligero de acero» |
| Progression Education | training (munición) | de entrenamiento | RML | «bala 5.56mm NATO de entrenamiento (FMJ)», «flechas de entrenamiento» |
| Spacer Arsenal | photon / neutron | de fotones / de neutrones | RML | «célula de fotones», «partícula de neutrones» |
| Spacer Arsenal | hot plasma cell | célula de plasma caliente | propuesta | |
| Spacer Arsenal | coilgun | cañón de bobina | propuesta | «6mm de cañón de bobina»; *railgun* sigue siendo «cañón de riel» |
| Spacer Arsenal | thump / plasma / contact grenade | granada de golpe / de plasma / de contacto | RML | |
| Spacer Arsenal | spacer tech | tecnología espacial | oficial | |
| Ushankas Glittertech | ADP (Area Denial Pulse) | PDA (pulso de denegación de área) | RML | «obús de mortero 81mm (PDA)» |
| Vanilla Arsenal | stun grenade | granada de aturdimiento | RML | Distinta de la *flashbang* del núcleo, «granada aturdidora» |
| VFE - Deserters / Empire | fletchling / fletcher (dart) | dardo acidificador | RML | La categoría de munición: «acidificadora». El cartucho, «cartucho acidificador» |
| VFE - Empire | charge cell (demolition) | célula de carga (demolición) | propuesta | «célula de carga 40x68mm (demolición)» |
| VFE - Empire | double autocannon | cañón automático doble | RML | |
| VFE - Pirates | warbolter | de sarcotraje | Angel | RML llama al arma «lanzacohetes (sarcotraje)»: «cohete de sarcotraje (HE)», «cohetes de sarcotraje» |
| VFE - Pirates | slugthrower | lanza perdigones | oficial | «cartucho de lanza perdigones (postas)», con los tipos de escopeta del núcleo. Decía «lanzabalas» hasta el 27-oct-2026 |
| VFE - Pirates | crypto cannon | criptocañón | RML | «combustible de criptocañón». *Crypto* suelto sigue siendo «criogénico», como en el núcleo |
| VFE - Pirates | gauntlet charge cannon | cañón de carga de guantelete | RML | |
| VFE - Pirates | salvaged laser eradicator | pulsoláser | RML | «rayo de pulsoláser» |
| VFE - Security | dummy (turret, barrier) | simulado/a | propuesta | Defs de relleno de las defensas ocultas |
| VGE - Chapter 1 | mass lance | lanza de masa | RML | |
| VQE - Cryptoforge | crypto bolt | criptovirote | RML | El arma es la «criptoballesta». La categoría de munición, «criogénica» |
| VRE - Android | self-preserve protocols | protocolos de autopreservación | propuesta | Gen de subrutina |
| VWE | throwing rocks / knives | piedras / cuchillos arrojadizos | RML | |
| VWE - Bioferrite | disruptor flare shell | obús de bengala disruptora | oficial | |
| VWE - Coilguns | gauss rifle / lance / magnum | fusil gauss / lanza gauss / revólver gauss | RML | «disparo de fusil gauss de alta potencia» |
| VWE - Heavy Weapons | swarm missile launcher | lanzamisiles de enjambre | RML | |
| VWE - Laser | tesla gun (arc discharge) | descarga de arco | propuesta | El arma es «cañón tesla» |
| VWE - Non-Lethal | taser | táser | propuesta | Como en el núcleo; RML escribe «taser» en el mod |
| VWE - Non-Lethal | dart rifle | pistola de dardos | RML | «cartucho de pistola de dardos», «munición para pistola de dardos» |
| VWE - Non-Lethal | rubber bullet | bala de goma | RML | «bala de goma calibre .43» |
| VWE - Non-Lethal | CN | CN | propuesta | Sigla del gas, queda igual: «granada 40x46mm (CN)» |
| VWE - Non-Lethal | tear gas grenades | granadas de gas lacrimógeno | RML | |
| Volt Weaponry | volt laser cell / volt beam | célula láser de voltios / haz de voltios | RML (el haz), propuesta | «(baja)», «(alta)», «(dispersión)» |

## Lo que se repite

- **Las recetas siguen al núcleo** (sección 6 del glosario de Combat Extended): el nombre sale del
  producto y la cantidad del def. Si el inglés dice otra, va la real: la receta del cohete M73 dice
  «x5» y da 4.
- **Las descripciones que ya tradujo el núcleo se copian**, aunque el mod las haya pegado en el def
  equivocado (la célula de demolición de VFE Empire dice que lleva combustible incendiario, y el
  cartucho de la pistola de dardos, que es una bala de fusil). Se traduce lo que dice.
