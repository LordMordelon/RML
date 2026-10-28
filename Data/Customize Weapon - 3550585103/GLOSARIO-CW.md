# Glosario: Customize Weapon

Términos que este mod repite, con la traducción que ya usan sus 236 entradas (contadas el 28-oct-2026; ninguna
pendiente). Es la base del sistema de módulos: Custom Weapons Expanded le suma módulos y sigue estos nombres
([GLOSARIO-CWE.md](../Custom%20Weapons%20Expanded%20-%203560528949/GLOSARIO-CWE.md)). [RML/GLOSARIO.md](../../GLOSARIO.md),
la terminología oficial de RimWorld, manda sobre este.

El mod ya sigue el estilo del juego: las etiquetas van en minúscula y *EMP* es «PEM», como la oficial.

## El sistema

| Inglés | Español | Notas |
|---|---|---|
| weapon module | **módulo de arma** | La categoría y el panel: «Módulos de arma» |
| part | **parte** | Cada hueco del arma: «Partes», «Parte de» |
| install / uninstall | **instalar / desinstalar** | «Confirmar instalación», «Desinstalar todos los módulos» |
| compatible modules | **módulos compatibles** | |
| module effects | **efectos del módulo** | |
| weapon panel | **panel de arma** | |
| assembly preset | **configuración de montaje** | «Guardar la configuración de montaje actual» |
| rarity: standard / rare / legendary | **rareza: estándar / raro / legendario** | |
| style: original / dynamic | **estilo: original / dinámica** | «apariencia dinámica» |
| dye | **teñir** (la acción) | El objeto del juego es «tinte» |
| Customize Weapon | **Customize Weapon** | Nombre del mod, en inglés |

## Piezas

| Inglés | Español | | Inglés | Español |
|---|---|---|---|---|
| barrel | cañón | | extended / shortened / tactical barrel | cañón extendido / acortado / táctico |
| magazine | cargador | | extended / quick / drum / belt magazine | cargador extendido / rápido / de tambor / de cinta |
| stock | culata | | fold / telescopic / shortened stock | culata plegable / telescópica / acortada |
| grip | empuñadura | | vertical / triangular / custom grip | empuñadura vertical / triangular / personalizada |
| sight / scope | mira | | red dot / holo / laser / 2x, 4x, 8x | mira de punto rojo / holográfica / láser / mira 2x, 4x, 8x |
| muzzle | bocacha | | suppressor | silenciador |
| receiver | mecanismos | | rapid receiver | cajón de mecanismos rápido |
| trigger | gatillo | | rapid trigger | gatillo rápido |
| stabilizer brace | abrazadera estabilizadora | | | |

## Munición y lanzadores

| Inglés | Español |
|---|---|
| rounds | **munición** («munición perforante», «munición de punta hueca», «munición PEM», «munición de carga de pulso») |
| pellets | **perdigones** («perdigones incendiarios», «perdigones tóxicos»; *birdshot pellets* = «perdigones») |
| shell | **proyectil** («lanzar proyectil PEM / incendiario / de humo») |
| grenade launcher / incendiary / smoke / EMP launcher | **lanzagranadas / lanzador incendiario / de humo / PEM** |

## Estadísticas de la ventana

| Inglés | Español hoy |
|---|---|
| DPS | Daño por segundo |
| Cooldown | Enfriamiento (como la oficial) |
| Accuracy | Precisión (como la oficial) |
| Touch / Short / Medium / Long | A quemarropa / Corta / Media / Larga |
| Fire rate multiplier | Multiplicador de cadencia de fuego |

## Correcciones propuestas (esperan la aprobación de Angel)

La ventana del mod muestra estadísticas que el juego ya nombra en la ficha del arma. Si se llaman distinto, el
jugador ve dos nombres para lo mismo. Propuesta: usar los de la oficial.

| Inglés | Hoy | Oficial (propuesta) | De dónde |
|---|---|---|---|
| Range | Alcance | **Rango** | `Keyed:Range` |
| Warmup time | Tiempo de puntería | **Calentamiento** | `Keyed:WarmupTime` |
| Armor penetration | Penetr. de blindaje | **Penetración de blindaje** | `Keyed:ArmorPenetration`. La abreviatura va contra la regla de no abreviar; si en el juego no entra, se vuelve a mirar |
| Stopping power (y su bonificación) | Poder de detención | **Potencia de parada** | `Keyed:StoppingPower`. También en CWE: «poder de detención adicional» |
| Burst count (y su multiplicador) | Disparos por ráfaga | **Tiros por ráfaga** | `Keyed:BurstShotCount` |
| Touch | A quemarropa | **Quemarropa** | `StatDef:AccuracyTouch`: «precisión (quemarropa)» |
