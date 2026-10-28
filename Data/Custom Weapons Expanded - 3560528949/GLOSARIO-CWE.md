# Glosario: Custom Weapons Expanded

Términos que este mod repite, con la traducción que ya usan sus 992 entradas (contadas el 28-oct-2026).
[RML/GLOSARIO.md](../../GLOSARIO.md), la terminología oficial de RimWorld, manda sobre este. Para los nombres
de armas y las piezas comunes (cañón, culata, empuñadura), ver también el glosario de Vanilla Weapons Expanded,
[GLOSARIO-VWE.md](../!Oskar%20Potocki/Vanilla%20Weapons%20Expanded%20-%201814383360/GLOSARIO-VWE.md).

El mod agrega **módulos** que se montan en un arma (`ThingDef`) y, por cada uno, un **rasgo** del mismo nombre
(`WeaponTraitDef`). El módulo y su rasgo se traducen igual.

## Piezas y módulos

| Inglés | Español | Notas |
|---|---|---|
| weapon module | **módulo de arma** | Las categorías: «módulos de arma: cañones», «…: miras», «…: culatas», «…: bocachas», «…: munición» |
| attachment | **accesorio** | «accesorio de escopeta», «accesorio de escopeta calibre 20» |
| underbarrel | **bajo el cañón** | «granadas de plasma bajo el cañón» |
| barrel | **cañón** | «cañón extendido», «cañón pesado», «cañón de alto calibre» |
| muzzle | **bocacha** | «bocacha armonizadora»; *muzzle brake* = «freno de boca» |
| magazine | **cargador** | «cargador acoplado», «cargador de aljaba de cristal» |
| scope / sight | **mira** | «mira de precisión», «mira láser» |
| grip | **empuñadura** | «empuñadura vertical», «empuñadura de carbono» |
| stock | **culata** | «culata de eltex» |
| choke | **estrangulador** | «estrangulador inteligente», «estrangulador de pico de pato» |
| frame receiver | **mecanismos de armazón** | «mecanismos de armazón pesado / ligero» |
| compensator | **compensador** | «compensador de flujo» |
| shroud | **cubierta** | «cubierta criorrefrigerante», «cubierta térmica ventilada» |
| governor | **regulador** | «regulador de acreción», «regulador de sobrecorriente» |
| collimator | **colimador** | «colimador de apertura», «colimador de campo» |
| energy cell / cell | **celda de energía / celda** | «celda de energía de alta densidad» |
| capacitor (bank) | **(banco de) condensadores** | «banco de condensadores pesado», «pack de supercondensadores» |
| coils | **bobinas** | «bobinas superconductoras», «sobreimpulso de bobinas» |
| quiver | **aljaba** | |
| kit | **kit** | «kit de alineación de rieles» |

## Munición

| Inglés | Español | Notas |
|---|---|---|
| ammunition | **munición** | «munición criogénica de la Alianza» |
| rounds | **proyectiles** | «proyectiles de erosión», «proyectiles de mecanitas» |
| slug | **bala maciza** | «bala maciza de escopeta». Ver «Correcciones propuestas» |
| pellets | **perdigones** | «perdigones de erosión»; *pellet count* = «número de perdigones» |
| arrows | **flechas** | «flechas de punta ancha», «flechas perforantes», «flechas paralizantes» |
| shell | **proyectil** | |
| grenade | **granada** | «granada de singularidad» |
| EMP | **PEM** | Como la oficial (`DamageDef` EMP). Ver «Correcciones propuestas» |

## Lanzadores bajo el cañón

| Inglés | Español |
|---|---|
| bioburner | **bioquemador** |
| flare launcher | **lanzabengalas** |
| foam launcher | **lanzaespuma** |
| tox launcher | **lanzagranadas tóxicas** |
| plasma grenade launcher | **lanzagranadas de plasma** |
| eletric pulse launcher | **lanzador de pulso eléctrico** |
| hand grenade launcher attachment | **accesorio lanzagranadas de mano** |

## La Alianza

Los módulos de la Alianza llevan el nivel en romanos al final: «cañón extendido de la Alianza I», «… II», «… III».
*Alliance* = «la Alianza», con mayúscula porque es un nombre propio.

## Títulos de armas únicas

Los adjetivos de los rasgos (`traitAdjectives`) siguen la regla del glosario principal, sección 9: una forma que no
cambie con el género («de precisión», «con buscador», «veloz»).

## Cotejado con la oficial (28-oct-2026)

`Herramientas/oficial/cotejar.py` marca tres términos con otra traducción oficial, todos de otro sentido: *sight*
(«vista», la del colono), *shell* (partes del cuerpo) y *slug* («perdigón», el proyectil del lanzaperdigones
mecanoide, que es otra arma). La *slug* de escopeta es una bala maciza.

## Correcciones propuestas (esperan la aprobación de Angel)

Encontradas al armar este glosario. No se aplicaron.

1. **Mayúscula inicial.** Unas 285 etiquetas empiezan con mayúscula: 155 de los módulos (`ThingDef`), 103 de los
   rasgos, 13 de las habilidades y 11 de los daños. El juego las usa en minúscula, como la oficial («cargador
   extendido»), y en este mod el mismo nombre sale con mayúscula como rasgo y en minúscula como pieza («Lanzabengalas»
   y «lanzabengalas»). Propuesta: todas en minúscula, salvo nombres propios («la Alianza») y siglas.
2. **EMP → PEM**, como la oficial: «Balas EMP» → «balas macizas PEM», «proyectil EMP de escopeta» → «proyectil PEM
   de escopeta», «Accesorio de escopeta EMP» → «accesorio de escopeta PEM», «{0} ha muerto por un EMP.» → «… por
   un PEM.».
3. **slug = bala maciza** en todas: «Bala de choque» (*shock slug*) → «bala maciza de choque».
4. **mod → módulo:** «Mod perforante de la Alianza I» → «módulo perforante de la Alianza I» (y II, III).
5. **toxic bullet** tiene dos traducciones en `DamageDef` («Proyectil tóxico» y «Bala tóxica»): dejar «bala tóxica».
