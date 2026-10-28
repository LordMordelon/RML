# Glosario: Vanilla Weapons Expanded

Términos que este mod repite, con la traducción que ya usan sus 839 entradas (contadas el 28-oct-2026). Sirve
también para la familia: VWE - Coilguns, Quickdraw, Laser, Non-Lethal, Heavy Weapons y Frontier, que usan los
mismos nombres de piezas y el mismo estilo. [RML/GLOSARIO.md](../../../GLOSARIO.md), la terminología oficial de
RimWorld, manda sobre este.

Las armas van al estilo del juego (regla del 24-oct-2026): **en minúscula**, «fusil» para las militares y
«rifle» solo para las de caza o civiles, y las **siglas como la oficial** («SMG pesado», «LMG»): la descripción
las explica.

## Armas

| Inglés | Español | Notas |
|---|---|---|
| charge X | **X de cargas** | Como la oficial («fusil de cargas», «lanza de cargas»): «pistola de cargas», «escopeta de cargas», «SMG de cargas», «fusil de francotirador de cargas» |
| charge blaster / heavy charge blaster | **bláster de cargas ligero / pesado** | |
| charge missile launcher | **lanzamisiles de cargas** | |
| SMG / light SMG / HMG | **SMG / SMG ligero / HMG** | Siglas, como la oficial |
| anti-materiel rifle | **fusil anti-material** | |
| battle rifle | **fusil de batalla** | |
| marksman rifle | **fusil de tirador** | |
| service rifle | **fusil de servicio** | |
| semi-automatic rifle | **fusil semiautomático** | |
| lever-action rifle | **fusil de palanca** | |
| carbine | **carabina** | |
| combat handgun | **pistola de combate** | |
| heavy revolver | **revólver pesado** | |
| sawn-off shotgun | **escopeta recortada** | |
| trench gun | **escopeta de trinchera** | |
| flamethrower | **lanzallamas** | |
| grenade launcher / rocket launcher | **lanzagranadas / lanzacohetes** | |
| compound bow / longbow / crossbow | **arco compuesto / arco largo / ballesta** | *recurve bow* = «arco recurvo» (oficial) |
| throwing knives / rocks | **cuchillos / piedras arrojadizas** | |
| unique X | **X único / única** | Concuerda con el arma: «carabina única», «fusil de batalla único» |

## Partes del arma (ataques cuerpo a cuerpo)

| Inglés | Español |
|---|---|
| barrel | **cañón** |
| stock | **culata** |
| grip | **empuñadura** |
| handle | **mango** |
| limb (de un arco) | **pala** |
| edge / blade | **filo / hoja** |
| head | **cabeza** |
| poke | **empujón** |

## Munición

| Inglés | Español |
|---|---|
| arrow | **flecha** («flecha explosiva», «flecha incendiaria») |
| bolt (de ballesta) | **virote** («virote de ballesta», «virote explosivo») |
| snake-shot load | **carga de perdigones** |

## Rasgos de armas únicas

Los nombres de los rasgos (`WeaponTraitDef`) van en minúscula, como la oficial: «empuñadura cómoda», «muescas
macabras», «marco esqueletizado», «miras iluminadas». Sus adjetivos (`traitAdjectives`), que arman el título del
arma, siguen la regla del glosario principal, sección 9: una forma que no cambie con el género. Al pasarlos a esa
forma el 28-oct-2026 quedaron así los más repetidos:

| Inglés | Español | | Inglés | Español |
|---|---|---|---|---|
| heavy | de gran peso | | rapid / quick / swift | veloz |
| explosive | de explosión | | cooled | de refrigeración |
| siege | de asedio | | thumping | de impacto |
| blunt | contundente (oficial) | | volley | de volea |
| twin | doble | | accurate / precise | de precisión / de exactitud (oficial) |
| seeking | con buscador (oficial) | | tactical | de uso táctico |

## Cotejado con la oficial (28-oct-2026)

`Herramientas/oficial/cotejar.py` marca *handle*, *blade*, *poke* y *arrow* con otra traducción oficial, pero son
otros sentidos: adiestrar animales, una parte del cuerpo de un mecanoide, la capacidad de un ataque («pinchar») y
el daño de una flecha («flechazo»). Como piezas de un arma se quedan como arriba.

## Para revisar

- **blast tip arrow** tiene dos traducciones: «flecha explosiva» y «flecha grande explosiva». Ver si la segunda es
  la del arco grande (*great bow*); si no, dejar una sola.
