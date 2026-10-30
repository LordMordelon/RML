# Armas y torretas de The Dead Man's Switch II

Lista de las 72 armas, armas de torreta y torretas del mod, con su nombre en inglés, cómo está hoy en
RML y la propuesta para dejarlas al estilo del juego. Sacada de los `Defs` del mod (versión del
24-oct-2026) y de `Languages/SpanishLatin` de esta carpeta. Los términos aprobados están en
[GLOSARIO-DMS.md](GLOSARIO-DMS.md).

**Estado:** aplicada el 24-oct-2026, después de que Angel la aprobó. 116 valores cambiados en
`DefInjected/ThingDef/go2jb9.xml` (nombres, etiquetas de disparo y descripciones), más «automatroids»
→ «automatroides» en cuatro descripciones. Las decisiones están también en `GLOSARIO-DMS.md`, en
«Armas y torretas».

## Decisiones de Angel (24-oct-2026)

1. **«fusil» antes que «rifle»**, como el juego, y «rifle» solo donde quede mejor. Criterio usado: fusil
   para las armas militares; rifle para las de caza o civiles (rifle de supervivencia, rifle para
   elefantes), que es como se dicen en Latinoamérica.
2. **Al estilo del juego** (ver «Cómo nombra el juego» más abajo).
3. **Los apodos se quedan si se pueden traducir, pero sin comillas y en minúscula**: «GAU-8 "Avenger"»
   pasa a «GAU-8 vengador».
4. **En minúscula**, como el juego. Quedan en mayúscula solo los códigos (GAU-8, MK44, 2B9M) y los
   nombres reales de armas que existen fuera del juego (Kord, Vasilek, Okhotnik). El juego hace lo
   mismo con «martillo Zeus».
5. **Revisar cada nombre en su contexto**: qué dice la descripción del arma, cómo se llama en el resto
   del mod lo que nombra (un dron, un meca) y, en el equipo, dónde se lo pone el colono. Lo que salió
   de esa revisión está en la columna «Por qué» de cada tabla.

## Cómo nombra el juego

**Armas.** Todo en minúscula, con «fusil» y con las siglas en inglés:

| Inglés | Oficial |
|---|---|
| assault rifle / bolt-action rifle / sniper rifle / charge rifle | fusil de asalto / fusil de cerrojo / fusil de francotirador / fusil de cargas |
| LMG / heavy SMG / minigun | LMG / SMG pesado / minigun |
| machine pistol / autopistol / revolver | pistola ametralladora / pistola automática / revólver |
| pump shotgun / chain shotgun | escopeta / escopeta automática |
| EMP launcher / smoke launcher / incendiary launcher | lanzagranadas PEM / lanzagranadas de humo / lanzador de incendiarios |
| mini-flameblaster / inferno cannon | minibláster de llamas / cañón inferno |
| frag grenades / EMP grenades | granadas fragmentarias / granadas PEM |

**Torretas.** El edificio y el arma que dispara tienen nombres distintos. El edificio es «torreta de
<arma>» o el nombre del arma si es artillería. El arma es el nombre del arma, sin «torreta»:

| Edificio (oficial) | Su arma (oficial) |
|---|---|
| torreta de cañón automática | cañón automático |
| torreta de balas de uranio | cañón de balas de uranio |
| minitorreta | pistola de minitorreta |
| torreta de cargas | bláster de cargas ligero |
| torreta inferno | cañón inferno |
| lanzacohetes múltiple | tubo de cohetes múltiples |
| mortero / mortero automático | mortero / mortero automático |
| torreta defensora antigua | cañón de torreta defensora antigua |

**Armas de meca.** El juego no les agrega «meca» (el bláster de cargas pesado de los mecanoides se
llama así nomás). Este mod sí lo pone en inglés («Mech Assault Rifle»), y el glosario aprobó dejarlo
detrás y sin «de»: «fusil de asalto meca».

## Lo que cambia con la propuesta

1. **«de meca» → «meca»** en 10 nombres, como dice el glosario.
2. **«rifle» → «fusil»** en las armas militares.
3. **Siglas como el juego:** LMG y SMG (y HMG, que el juego no tiene, por coherencia) en vez de
   «ametralladora ligera» o «subfusil».
4. **El MK44 queda con un solo nombre.** Hoy tiene tres («rifle de alta potencia», «torreta de cañon
   pesada», «torreta de alta potencia») y ninguno dice que es un cañón automático de 30 mm.
5. **El 2A38M deja de ser «rifle alba menor»**, que no tiene que ver con el Hoja del Alba: es un cañón
   automático doble.
6. **«cañon» → «cañón»** en tres nombres.
7. **El Vasilek es un mortero**, no un cañón ni artillería genérica.
8. **«rifle sin retroceso» → «cañón sin retroceso»**, como el glosario.
9. **El equipo no es mochila**: los cuatro van en la cintura (capa «cinturón» del juego). El juego
   llama «pack» a casi todo lo que va ahí («pack de control», «pack de banda ancha») y «cinturón» al
   cinturón escudo. Se sigue lo que dice el inglés: «pack» → pack, «belt» → cinturón.
10. **Los drones se llaman como en el resto del mod**: el «hound» es «sabueso» («estación de
    sabuesos») y el «Verling bird» es «dron explorador».

## Armas de meca

| defName | Inglés | Hoy en RML | Propuesta | Por qué |
|---|---|---|---|---|
| DMS_TacticalPistol | Mech pistol | pistola de meca | pistola meca | Arma de mano para automatroides o exoesqueletos |
| DMS_SemiAutoRifle | Mech Semiauto Rifle | rifle semiautomático meca | fusil semiautomático meca | Militar |
| DMS_AssaultRifle | Mech Assault Rifle | rifle de asalto meca | fusil de asalto meca | Como el oficial «fusil de asalto» |
| DMS_BattleRifle | Mech Battle Rifle | rifle de batalla meca | fusil de batalla meca | Militar |
| DMS_MarksmanRifle | Mech Marksman Rifle | rifle de tirador de meca | fusil de tirador meca | Fusil automático de precisión |
| DMS_AssaultShotgun | Mech Assault Shotgun | escopeta de asalto de meca | escopeta de asalto meca | Escopeta automática |
| DMS_AntiMaterialRifle | anti-material rifle | AMR de meca | fusil antimaterial | Francotirador de gran calibre. El inglés no dice «mech» |
| DMS_Gun_DragoonRifle | Mech Anti-Tank Rifle | rifle antitanque meca | fusil antitanque meca | |
| DMS_AssaultPiercer | Assault Piercer | perforador de asalto | sin cambio | Subfusil de calibre alto que perfora blindaje |
| DMS_DoorBreaker | Door Buster | rompepuertas | sin cambio | Ariete con lanzador y cabeza explosiva |
| DMS_SniperGrenadeLauncher | Sniper grenade launcher | lanzagranadas de francotirador | sin cambio | Lanzagranadas de precisión |
| DMS_AutomaticGrenadeLauncher | Mech Grenade Launcher | lanzagranadas de meca | lanzagranadas meca | Lanzagranadas automático |
| DMS_Gun_DragoonGrenade | Mech Heavy Grenade Launcher | lanzagranadas pesado meca | sin cambio | |
| DMS_LightMachineGun | Mech LMG | ametralladora ligera de meca | LMG meca | Sigla, como el oficial «LMG» |
| DMS_HeavyMachineGun | Mech HMG | ametralladora pesada meca | HMG meca | Sigla, pareja con LMG |
| DMS_M214Gatlinggun | M214 | M214 | sin cambio | Ametralladora rotativa de calibre de fusil; el inglés es solo el código |
| DMS_GAU19Gatlinggun | GAU-19/A "Attacker" | GAU-19/A "Atacante" | GAU-19/A atacante | Rotativa de calibre de HMG |
| DMS_GAU4Gatlinggun_Hold | GAU-4 "Vulcan" | GAU-4 "Vulcan" | GAU-4 vulcano | Rotativa de calibre de cañón; Vulcan es el dios Vulcano |
| DMS_GAU8Gatlinggun | GAU-8 "Avenger" | GAU-8 "Vengador" | GAU-8 vengador | El cañón rotativo de los aviones antiguos |
| DMS_Autocannon | Mech Autocannon | cañón automático meca | sin cambio | Como el oficial «cañón automático» |
| DMS_Gun_MK44_Autocannon | MK44 BushMasterII AutoCannon | rifle de alta potencia | cañón automático MK44 | Cañón automático de 30 mm. «Bushmaster II» se saca: el código ya lo nombra |
| DMS_Gun_2A38M | 2A38M | rifle alba menor | cañón automático 2A38M | Cañón automático doble, antiaéreo. No tiene que ver con el Hoja del Alba |
| DMS_Gun_DawnbladeM | MPQFS-85M "Dawnblade" | MPQFS-85M "Hoja del Alba" | MPQFS-85M hoja del alba | La versión que carga un meca del cañón de la torreta |
| DMS_Gun_Recoilless | Recoilless Rifle | rifle sin retroceso | cañón sin retroceso | Glosario |
| DMS_SniperCannon | Bolt Cannon | cañón de cerrojo | sin cambio | Fusil enorme que dispara munición de cañón automático |

## Armas que puede usar un colono

| defName | Inglés | Hoy en RML | Propuesta | Por qué |
|---|---|---|---|---|
| DMS_Revolver | Riot Revolver | revólver antidisturbios | sin cambio | Revólver de munición de escopeta |
| DMS_KnuckledusterRevolver | Knuckleduster Revolver | revólver de nudillos | revólver manopla | Se pliega y sirve de manopla («brass knuckles») |
| DMS_PDW | PDW | PDW | sin cambio | Sigla, como el oficial «SMG» |
| DMS_MiniMachineGun | Riot LMG | LMG antidisturbios | sin cambio | Ametralladora de calibre de pistola para motines |
| DMS_RiotShotgun | Rifled slug gun | escopeta de cañón estriado | sin cambio | Escopeta de balas sólidas para motines |
| DMS_SurvivalRifle | Survival rifle | fusil de supervivencia | rifle de supervivencia | Arma civil: rifle |
| DMS_ElephantGun | Elephant Rifle | fusil elefante | rifle para elefantes | Rifle de caza mayor, de un tiro |
| DMS_Flameblaster | Flameblaster | lanzallamas | bláster de llamas | Como el oficial «minibláster de llamas». Lo usan mecas |
| DMS_Gun_RecoillessLight | light recoilless rifle | cañón sin retroceso ligero | sin cambio | Tubo desechable de 4 kg |
| DMS_Weapon_GrenadeSubsonic | infrasonic grenades | granadas de infrasonido | sin cambio | Como el oficial «granadas PEM» |
| DMS_Machete | Machete | machete | sin cambio | Machete de filo vibratorio |
| DMS_StunBaton | Stun Baton | bastón aturdidor | sin cambio | Bastón eléctrico antidisturbios |

## Armas montadas (sobre mecas o vehículos)

| defName | Inglés | Hoy en RML | Propuesta | Por qué |
|---|---|---|---|---|
| DMS_SubMachineGunMounted | Mounted SMG | subfusil montado | SMG montado | Sigla, como el oficial «SMG pesado» |
| DMS_LightMachineGunMounted | Mounted LMG | ametralladora ligera | LMG montada | Hoy le falta «montada» |
| DMS_HeavyMachineGunMounted | Mounted HMG | ametralladora pesada | HMG montada | Hoy le falta «montada» |
| DMS_ShotgunGunMounted | Close Inbound Defense System | sistema de defensa cercana | sin cambio | Escopeta de varios cañones |
| DMS_BullHound_ShotgunMounted | bullhound autoshotgun | escopeta automática del can de presa | sin cambio | Va sobre el lomo del can de presa |
| DMS_GAU4Gatlinggun | GAU-4 "Vulcan" | GAU-4 "Vulcan" | GAU-4 vulcano | |
| DMS_Gun_Kord_Vehicle | Kord | kord | HMG Kord | La ametralladora pesada Kord |
| DMS_Gun_Vasilek_B | 2B9A Vasilek | cañon vasilek | mortero 2B9A Vasilek | Dispara cuatro granadas de mortero por ráfaga |

## Torretas y sus armas

Cada torreta (el edificio) va con el arma que dispara debajo (↳), como en el juego.

| defName | Inglés | Hoy en RML | Propuesta | Por qué |
|---|---|---|---|---|
| DMS_Turret_Sentry | Sentry gun | arma centinela | torreta centinela | Es una torreta con un SMG |
| ↳ DMS_SubMachineGunMounted | Mounted SMG | subfusil montado | SMG montado | |
| DMS_Turret_LightMachineGun | Mech LMG | ametralladora ligera de meca | torreta de LMG | La LMG meca puesta sobre su bípode |
| ↳ DMS_Gun_LightMachineGun | mech LMG | ametralladora ligera de meca | LMG meca | |
| DMS_Turret_HeavyMachineGun | Mech HMG | ametralladora pesada meca | torreta de HMG | La HMG meca sobre su trípode |
| ↳ DMS_Gun_HeavyMachineGun | mech HMG | ametralladora pesada meca | HMG meca | |
| DMS_Turret_AutomaticGrenadeLauncher | Mech Grenade Launcher | lanzagranadas de meca | torreta lanzagranadas | El lanzagranadas meca sobre su trípode |
| ↳ DMS_Gun_AutomaticGrenadeLauncher | mech grenade launcher | lanzagranadas de meca | lanzagranadas meca | |
| DMS_Turret_GateKeeper | Gatekeeper | torreta guardián | sin cambio | Rotativa de gran calibre de naves |
| ↳ DMS_Gun_GateKeeper | Gatekeeper | torreta guardián | guardián | Su descripción dice que es el GAU-8 vengador |
| DMS_Turret_Dawnblade | Dawnblade | torreta Hoja del Alba | torreta hoja del alba | Cañón de disparo rápido de las fragatas |
| ↳ DMS_Gun_Dawnblade | Type 85 Multi-Purpose-Quick-Firing System | sistema de disparo rápido multipropósito Tipo 85 | sistema de disparo rápido multipropósito tipo 85 | |
| DMS_Turret_MK44_Navy | MK44 BushMaster-II | torreta de alta potencia | torreta MK44 | |
| ↳ DMS_Gun_MK44_Navy | MK44 BushMaster-II | torreta de cañon pesada | cañón automático MK44 | El mismo cañón de 30 mm que carga el meca |
| DMS_Turret_Okhotnik | Okhotnik | okhotnik | torreta Okhotnik | Torreta a control remoto de vehículos blindados |
| ↳ DMS_Gun_Kord | kord | kord | HMG Kord | |
| DMS_Turret_Vasilek | Vasilek | artillería vasilek | mortero Vasilek | Como el oficial: el edificio de un mortero se llama «mortero» |
| ↳ DMS_Gun_Vasilek | 2B9M vasilek | cañon vasilek | mortero 2B9M Vasilek | |

## Solo con Combat Extended

| defName | Inglés | Hoy en RML | Propuesta | Por qué |
|---|---|---|---|---|
| DMS_Iguana_SubMachineGun | Light Machinegun Turret | torreta de ametralladora ligera | torreta de LMG | Sigue al inglés, aunque su descripción dice calibre de pistola |
| DMS_Tarbosaurus_SubMachineGun | Heavy Machinegun turret | torreta de ametralladora pesada | torreta de HMG | |
| DMS_Tarbosaurus_ShotgunGun | Close Inbound Defense System | sistema de defensa cercana | sin cambio | |
| DMS_Dozer_KordMG | Kord HMG turret | torreta de ametralladora pesada Kord | torreta de HMG Kord | |
| DMS_SubTurret_127Gatling | Mounted GAU-19/A | GAU-19/A montada | sin cambio | Va al hombro |
| DMS_Gecko_FlameTurret | Flame Turret | torreta de llamas | sin cambio | |

## Equipo con arma

Los cuatro van en la cintura: en el juego ocupan la capa «cinturón», como el cinturón escudo. Ninguno
es mochila.

| defName | Inglés | Hoy en RML | Propuesta | Por qué |
|---|---|---|---|---|
| DMS_Apparel_PackHound | hound drone pack | paquete de drones sabueso | pack de sabuesos | «pack», como los oficiales que van en la cintura. El dron se llama «sabueso» («estación de sabuesos») |
| DMS_Apparel_PackScanner | scanner pack | mochila de escáner | pack de escáner | Va en la cintura, no en la espalda |
| DMS_Apparel_PackVerling | verlingbrid drone belt | cinturón de drones | cinturón de drones exploradores | El inglés dice «belt». El «Verling bird» ya es «dron explorador» |
| DMS_DeposableRocketLauncher | disposable launcher | lanzador desechable | sin cambio | Cañón sin retroceso de un solo uso que se lleva en la cintura |

## Descripciones con siglas

Pedido de Angel (24-oct-2026): si el nombre lleva una sigla en inglés, la descripción la explica como
el juego. La fórmula oficial es «<nombre en español> o por sus siglas en inglés <SIGLA> (<nombre en
inglés>) es…»:

> Una ametralladora ligera o por sus siglas en inglés LMG (Light machine gun) es un arma de fuego
> accionada por gas. *(LMG del juego)*
>
> El subfusil pesado o por sus siglas en inglés SMG (submachine gun) es un lanzador de proyectiles
> compacto y de gran calibre. *(SMG pesado del juego)*

Solo cambia la primera frase. El resto de cada descripción queda como está, salvo lo de la lista
«Otros arreglos» de más abajo.

| defName | Nombre propuesto | Primera frase hoy | Primera frase propuesta |
|---|---|---|---|
| DMS_LightMachineGun | LMG meca | Un arma especializada en supresión con recámara para un proyectil de rifle de gran calibre. | Una ametralladora ligera o por sus siglas en inglés LMG (light machine gun) es un arma especializada en supresión, con recámara para un cartucho de fusil de gran calibre. |
| DMS_Turret_LightMachineGun | torreta de LMG | (la misma) | (la misma) |
| DMS_Gun_LightMachineGun | LMG meca | Una ametralladora ligera montada sobre un bípode plegable. | Una ametralladora ligera o por sus siglas en inglés LMG (light machine gun), montada sobre un bípode plegable. |
| DMS_HeavyMachineGun | HMG meca | Una ametralladora pesada para soldados meca o plataformas de apoyo de infantería. | Una ametralladora pesada o por sus siglas en inglés HMG (heavy machine gun) es un arma para soldados meca o plataformas de apoyo de infantería. |
| DMS_Turret_HeavyMachineGun | torreta de HMG | Una ametralladora pesada para mecas soldado o plataformas de apoyo de infantería. | (la misma de arriba; también arregla «mecas soldado») |
| DMS_Gun_HeavyMachineGun | HMG meca | Una ametralladora pesada montada sobre un trípode. | Una ametralladora pesada o por sus siglas en inglés HMG (heavy machine gun), montada sobre un trípode. |
| DMS_LightMachineGunMounted | LMG montada | Un arma supresora con recámara para un proyectil de rifle estándar. | Una ametralladora ligera o por sus siglas en inglés LMG (light machine gun) es un arma supresora con recámara para un cartucho de fusil estándar. |
| DMS_HeavyMachineGunMounted | HMG montada | Una ametralladora pesada para soldados meca o plataformas de apoyo de infantería. | Una ametralladora pesada o por sus siglas en inglés HMG (heavy machine gun) es un arma para soldados meca o plataformas de apoyo de infantería. |
| DMS_SubMachineGunMounted | SMG montado | Un arma supresora con recámara para munición de pistola. | Un subfusil o por sus siglas en inglés SMG (submachine gun) es un arma supresora con recámara para munición de pistola. |
| DMS_MiniMachineGun | LMG antidisturbios | Ametralladora de calibre de pistola diseñada para la defensa de la colonia. La ametralladora ligera antidisturbios puede acabar… | Una ametralladora ligera o por sus siglas en inglés LMG (light machine gun) de calibre de pistola, diseñada para la defensa de la colonia. La LMG antidisturbios puede acabar… |
| DMS_PDW | PDW | Una metralleta de producción masiva diseñada para ser utilizada como arma de "disparo de punto". | Un arma de defensa personal o por sus siglas en inglés PDW (personal defense weapon) es un subfusil de producción masiva, diseñado para el disparo instintivo. |
| DMS_Gun_Kord_Vehicle | HMG Kord | ametralladora antiespecie con recámara de muerte. | La Kord es una ametralladora pesada o por sus siglas en inglés HMG (heavy machine gun) que sirve contra todo y dispara pura muerte. |
| DMS_Gun_Kord | HMG Kord | kord | Descripción nueva, ver «La descripción del Kord» |
| DMS_Iguana_SubMachineGun | torreta de LMG | Un arma especializada en la supresión, con recámara para un cartucho de calibre de pistola. | Una ametralladora ligera o por sus siglas en inglés LMG (light machine gun) es un arma especializada en la supresión, con recámara para un cartucho de calibre de pistola. |
| DMS_Tarbosaurus_SubMachineGun | torreta de HMG | Una ametralladora pesada para soldados meca o plataformas de apoyo de infantería. | Una ametralladora pesada o por sus siglas en inglés HMG (heavy machine gun) es un arma para soldados meca o plataformas de apoyo de infantería. |
| DMS_Dozer_KordMG | torreta de HMG Kord | Una ametralladora pesada para soldados meca o plataformas de apoyo de infantería. | (la misma de arriba) |

### La descripción del Kord

El autor dejó «kord» como descripción de `DMS_Gun_Kord`, y Angel pidió escribirle una (24-oct-2026),
como a la bestia. Lo que dice el mod de esta arma:

- **Es el arma de la torreta Okhotnik**, «un diseño antiguo de torreta controlada a distancia
  ampliamente utilizada en vehículos blindados y plataformas de blindaje de mecas». La torreta pide
  500 W, cañón reforzado (hay que cambiarle el cañón) y construcción 8.
- **La misma ametralladora va montada en el geochelone** (`DMS_Gun_Kord_Vehicle`, «Kord»), cuya
  descripción en inglés dice «Kord anti-everything machinegun chambered in death».
- **Con Combat Extended** aparece además como `DMS_Dozer_KordMG`, «Kord HMG turret»: «una ametralladora
  pesada para soldados meca o plataformas de apoyo de infantería».
- **Dispara** ráfagas de seis tiros, con un alcance de 40 celdas, y usa el `DMS_Bullet_Kord`, «uranium
  APT shell» (hoy «proyectil APT de uranio»): bala perforante trazadora con núcleo de uranio, de 18 de
  daño. El GAU-19/A usa la misma bala.
- Si está el mod The Dead Man's Switch - Ancient Corps, construir la torreta pide además una
  ametralladora Kord (`DMS_KordMachineGun`), que sale de ese mod.

**Propuesta**, con la fórmula de las siglas y sin inventar datos:

> La Kord es una ametralladora pesada o por sus siglas en inglés HMG (heavy machine gun) de diseño
> antiguo, la misma que llevan montada vehículos blindados y plataformas de blindaje de mecas como el
> geochelone. Dispara ráfagas cortas de balas perforantes trazadoras con núcleo de uranio.

**Y la bala:** «proyectil APT de uranio» también lleva una sigla en inglés (APT, *armor-piercing
tracer*). Las balas no muestran descripción, así que la propuesta es cambiar el nombre a «bala
perforante trazadora de uranio».

### Otros arreglos que salieron al leer las descripciones

- **«rifle» → «fusil»** también en las descripciones de las armas militares: «un proyectil de rifle de
  gran calibre», «un rifle de francotirador de alto calibre» (`DMS_AntiMaterialRifle`), y las demás.
  Se aplica con el mismo criterio que los nombres.
- **«proyectil de rifle» → «cartucho de fusil»**: «round» es el cartucho entero, no solo el proyectil.
- **«mecas soldado» → «soldados meca»**: aparece una vez; las otras 10 ya dicen «soldados meca», como
  el glosario.
- **«milltors» → «militoides»** en la descripción del PDW. Es una errata del autor por «militors», el
  meca del juego, que la traducción oficial llama «militoide».
- **«metralleta» → «subfusil»**: hoy el mod usa las dos palabras, dos veces cada una. «Subfusil» es la
  del juego («subfusil pesado»), así que quedarían las cuatro como «subfusil».

## Al aplicar

- Cada arma tiene también la etiqueta de su disparo (`verbs.Verb_Shoot.label`), que suele repetir el
  nombre: se cambia junto con el nombre.
- Las descripciones pueden nombrar el arma («El MK44…»): revisarlas con el mismo criterio.
- La descripción de `DMS_Gun_Kord` es nuestra (ver «La descripción del Kord»): anotarla en el glosario
  como la de la bestia, para reemplazarla si el autor escribe la suya.
- Después, pasar las decisiones a `GLOSARIO-DMS.md` y regenerar el índice.
