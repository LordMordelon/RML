# Glosario: Ascension Megacorp

La facción United Stellar Armament Company (USAC): una corporación que vende armas y mecas por una
terminal propia, presta, cobra intereses y manda a cobrar. Lo que comparte con la familia está en
[GLOSARIO-DMS.md](../The%20Dead%20Man's%20Switch%20-%203819799478/GLOSARIO-DMS.md),
[ARMAS-DMS.md](../The%20Dead%20Man's%20Switch%20-%203819799478/ARMAS-DMS.md) y
[GLOSARIO-FFF.md](../../Fortified%20Features%20Framework%20-%203498575851/GLOSARIO-FFF.md); la munición
de Combat Extended sigue
[GLOSARIO-COMBAT-EXTENDED.md](../../Combat%20Extended%20-%202890901044/GLOSARIO-COMBAT-EXTENDED.md). Por
encima de todos manda la traducción oficial, y después [RML/GLOSARIO.md](../../../GLOSARIO.md). El plan
está en [PLAN-AOBAKUMA.md](../PLAN-AOBAKUMA.md).

Armado el 25-oct-2026, con el mod recién extraído (1.355 entradas, ninguna traducida). **Origen**:
*oficial*, *RML* (ya en uso en otro mod), *familia*, *CE* o *propuesta* (sin apoyo; se usa así hasta que
Angel lo cambie).

## Decisiones de Angel (25-oct-2026)

1. **El nombre de la corporación: Compañía Unida de Armamento Estelar**, con la sigla **USAC** en
   inglés, como la usa todo el mod (137 veces). La familia ya traduce los nombres de empresas
   («Industrias Nara», «Sistemas del Ejército Unido»). Cómo se escribe en cada lugar:

   | Dónde | Cómo | Ejemplo |
   |---|---|---|
   | El nombre de la facción (`USAC_Faction.label`) | El nombre completo, sin sigla | «Compañía Unida de Armamento Estelar» |
   | **La descripción de la facción** (`USAC_Faction.description`) | **Empieza explicando la sigla**, con la fórmula de la oficial y de las armas de la familia (ver abajo), aunque el inglés no la nombre: es donde el jugador ve la facción y tiene que poder unir el nombre con la sigla que lee en todo el mod | «La Compañía Unida de Armamento Estelar o por sus siglas en inglés USAC (United Stellar Armament Company) es un bloque de desarrolladores de recursos y casas de comercio armadas movido por las ganancias…» |
   | Las demás descripciones donde el inglés escribe el nombre entero (`USAC_Culture`, `USAC_RookieRockefeller`, `USAC_Bond`) | La fórmula, en la primera mención de esa descripción | «…bajo la Compañía Unida de Armamento Estelar o por sus siglas en inglés USAC (United Stellar Armament Company)» |
   | Donde el inglés dice solo «USAC» | «USAC», sin explicar | «caja de suministros USAC (P)», «comerciante orbital USAC» |
   | El encabezado de la terminal (`USAC.UI.Home.Banner`) | En mayúsculas, con el código en inglés (decisión 2) | «COMPAÑÍA UNIDA DE ARMAMENTO ESTELAR\n[CORPORATE_INTRANET_V4.2]» |

   **La fórmula** es la de la traducción oficial («Una ametralladora ligera o por sus siglas en inglés
   LMG (Light machine gun) es un arma de fuego…», «El subfusil pesado o por sus siglas en inglés SMG
   (submachine gun)…»), y la misma de las armas de la familia (`ARMAS-DMS.md`, «Descripciones con
   siglas»): **«<nombre en español> o por sus siglas en inglés <SIGLA> (<nombre en inglés>)»**. El
   nombre en inglés va entre paréntesis tal como lo escribe el mod. Se explica **una vez por
   descripción**, no en cada mención.
2. **Los rótulos de la terminal**, que el autor escribe como código (`ACTIVE_CONTRACTS // Active
   contracts`, `FINANCIAL_TERMINAL`, `MOVE_SPEED`): el código queda en inglés y se traduce lo que va
   después de `//` («ACTIVE_CONTRACTS // Contratos activos»); los rótulos que van solos, como encabezados
   de una ficha, se traducen con el mismo estilo («CATEGORÍA», «DAÑO», «VELOCIDAD_DE_MOVIMIENTO»).
3. **Los mecas con apodo**, con las reglas de la familia: código primero, apodo traducido si se puede,
   sin comillas y en minúscula: **M7 cobalto**, **MQ-9 verdugo**, y los que son nombres, como están:
   **M88 rocky**, **M110 omaha**, **M939 paraman**, **XM3 gonk**.

Las demás siglas del mod que el jugador no conoce del juego (GPMG, DMR, SDA, HESCO…) siguen la misma
fórmula en la descripción de su objeto, como las armas de la familia: «Una ametralladora de propósito
general o por sus siglas en inglés GPMG (general-purpose machine gun)…».

## La corporación y el dinero

| Inglés | Español | Origen | Notas |
|---|---|---|---|
| credit bond(s) | **bono(s) de crédito** | propuesta | La moneda de la USAC (`ThingCategoryDef` «USAC currency»: «moneda USAC») |
| credits | **créditos** | oficial | |
| debt / debt inheritor / debt contract | **deuda** / **heredero de deudas** / **contrato de deuda** | propuesta | |
| principal / interest / cycle | **capital** / **intereses** / **ciclo** | propuesta | «crecimiento del capital», «intereses del ciclo» |
| credit score | **calificación crediticia** | propuesta | No «puntaje» ni «puntuación», que cambian según el país |
| collection / forced collection | **cobranza** / **cobranza forzosa** | propuesta | |
| debt management department | **departamento de gestión de deudas** | propuesta | |
| asset / asset liquidation | **activo** / **liquidación de activos** | propuesta | |
| contract / military contract | **contrato** / **contrato militar** | RML | |
| lease services | **servicios de arrendamiento** | propuesta | |
| trade terminal | **terminal comercial** | propuesta | Femenino, como «terminal logística» en la familia |
| orbital trade beacon / trade beacon | **baliza de comercio orbital** | oficial | |
| comms console | **consola de comunicaciones** | oficial | |
| orbital trader / trade ship | **comerciante orbital** / **nave comercial** | propuesta | *trader* es oficial («comerciante») |
| user agreement / terms of service | **acuerdo de usuario** / **condiciones de servicio** | propuesta | «Aceptar el acuerdo», «Rechazar» |
| executive / CTO | **ejecutivo** / **CTO** | propuesta | CTO queda como sigla, como en la ideología |
| risk control site | **sitio de control de riesgos** | familia | «sitio», como en FFF |
| commercial outpost / outpost | **puesto de avanzada comercial** / **puesto de avanzada** | oficial | |
| temporary airbase / artillery position | **base aérea temporal** / **posición de artillería** | propuesta | |
| commercial raid / mining raid | **incursión comercial** / **incursión minera** | propuesta | |

## La terminal (`Keyed`, 26-27-oct-2026)

| Inglés | Español | Origen | Notas |
|---|---|---|---|
| settlement (pago) / orbital settlement network | **liquidación** / **red orbital de liquidación** | propuesta | «Liquidación de intereses», «Liquidación USAC: {0}» |
| missed payment | **pago incumplido** | propuesta | «pago incumplido n.º {1}», «Pagos incumplidos: {0}» |
| repay principal / repayment | **amortizar capital** / **amortización del capital** | propuesta | |
| surcharge / quarterly free allowance | **recargo** / **cupo gratuito del trimestre** | propuesta | |
| written off / settled | **cancelada** / **saldada** | propuesta | La deuda «se canceló», «saldada (compra con plata)» |
| Debt Management Department | **Departamento de Gestión de Deudas** | propuesta | Con mayúsculas cuando es el nombre propio, como en el inglés |
| trade beacon | **baliza de comercio orbital** | oficial | Siempre entera, aunque el inglés diga solo *trade beacon* |
| orbital gripper / transport clamp | **pinza orbital** / **pinza de transporte** | propuesta | |
| lease / mining rig | **arrendamiento** / **equipo de minería** | propuesta | |
| lost as change (USAC no da vuelto) | **perdido por falta de cambio** | propuesta | «cambio», no «vuelto», que es regional |
| rimworld | **mundoborde** | oficial | Como en `RML/GLOSARIO.md` |
| Patriot Financial Security Act | **Ley Patriota de Seguridad Financiera** | propuesta | |
| Hibernate (meca) | **Hibernar** / **hibernación** | propuesta | |
| Stat labels (MOVE_SPEED…) | **VELOCIDAD_DE_MOVIMIENTO**, **ARMADURA_CORTANTE**, **PENETRACIÓN_DE_ARMADURA**, **POTENCIAL_MÉDICO** | oficial | Los nombres oficiales de cada estadística, enteros y sin abreviar aunque el inglés abrevie (*ARMOR_PEN*, *INSUL_COLD*) |

`TraderArrival`, `TraderArrivalFromFaction` y `TraderArrivalNoFaction` son claves del juego que el mod
reemplaza con su propio texto: van traducidas desde el inglés del mod, no copiadas de la oficial (que dice
otra cosa y usa `{FACTION_name}`; el mod usa `{FACTION}`).

## El equipo y el catálogo (`ThingDef` y `USACProductDef`, 27-oct-2026)

Las fichas del catálogo llevan el nombre de su `ThingDef`, con mayúscula inicial como el inglés
(«Fusil automático avanzado», «Casco de asalto»). Donde la ficha dice otra cosa, se sigue la ficha:
«Fusil de tirador» (el objeto es «fusil de tirador avanzado»), «Bushmaster III», «Unidad Hardiman».

| Inglés | Español | Origen | Notas |
|---|---|---|---|
| mechanitor | **mecanizador** | oficial | «Inyectar {0_label} para convertirse en mecanizador» |
| bandwidth | **banda ancha** | oficial | También en «proxy VPN de banda ancha» |
| neuro-augment serum / neural enhancement drug | **suero de neuromejora** / **droga de mejora neural** | propuesta | |
| hardiman powered unit | **unidad motorizada Hardiman** | propuesta | Hardiman con mayúscula: es el nombre del exoesqueleto real. La ficha dice *Hardiman unit*: «Unidad Hardiman» |
| heavy sentry gun | **torreta centinela pesada** | familia | Como la «torreta centinela» de DMS |
| Foxhound GPMG | **GPMG Foxhound** | familia | Sigla primero, como «HMG Kord». La descripción explica GPMG con la fórmula |
| Viper DMR / M905 «Viper» | **DMR víbora** / **M905 víbora** | familia | La descripción explica DMR (*designated marksman rifle*) |
| M29 «SABR» | **M29 SABR** | propuesta | La descripción la explica entre paréntesis: *selectable assault battle rifle* |
| M109 «Payload» / M307 «Terminator» | **M109 carga útil** / **M307 exterminador** | propuesta | Apodos traducidos, como «MQ-9 verdugo». OSW y ACSW se explican entre paréntesis |
| XM931 Pulsar II / pulser beam | **XM931 pulsar II** / **haz de pulsos** | propuesta | |
| chain cannon / chaingun bullet | **cañón de cadena** / **bala de ametralladora de cadena** | propuesta | |
| hydra rocket / guided bomb | **cohete hydra** / **bomba guiada** | propuesta | |
| mineclearing line segment / towed mineclearing rocket | **segmento de línea de desminado** / **cohete de desminado remolcado** | propuesta | |
| 35mm APDS round / 105mm recoilless round | **proyectil APDS de 35 mm** / **proyectil sin retroceso de 105 mm** | familia | Proyectiles del juego, no de CE: «35 mm» con espacio |
| .277 Fury (DU) y las demás DU | **.277 Fury (DU)**; bala: **bala .277 Fury (DU)** | CE | Como CE: la munición sin sustantivo, la bala con «bala» |
| Krepost heavy walker / battle frame / geochelone chassis | **chasis del andador pesado krepost** / **del armazón de batalla** / **del geochelone** | familia | |
| USAC Marine Corps | **Cuerpo de Marines de la USAC** | propuesta | |
| field backpack | **mochila de campaña** | propuesta | |
| registered (certificado) | **nominativo** | propuesta | El bono solo lo gasta su titular |
| wetware material | **material biológico** | propuesta | La bolsa para cadáveres |
| hellhound | **sabueso del infierno** | propuesta | El apodo del M7 cobalto |

**HESCO no se explica con la fórmula:** no es una sigla sino la marca del fabricante, y la descripción
del muro lo dice en una frase («HESCO es la marca de su fabricante»).

## Permisos y títulos (al estilo de Royalty)

| Inglés | Español | Origen | Notas |
|---|---|---|---|
| call X (permiso) | **solicitar X** | oficial | Como «solicitar un transbordador», «solicitar ataque aéreo» |
| call orbital strike | **solicitar ataque orbital** | oficial | La oficial dice «ataque aéreo» para el de Royalty; acá el inglés dice *orbital*, y se respeta |
| call transport shuttle | **solicitar un transbordador** | oficial | Idéntico al de Royalty |
| call rifle squad / heavy team / elite detachment | **solicitar una escuadra de fusileros** / **un equipo pesado** / **un destacamento de élite** | propuesta | |
| logistic / combat / command supply drop | **suministros logísticos** / **de combate** / **de mando** | propuesta | «solicitar suministros de combate», como «solicitar comida» |
| close air strafing / close air bombard | **ametrallamiento aéreo cercano** / **bombardeo aéreo cercano** | propuesta | |
| strafing run / guided bombing run / EMP air strike | **pasada de ametrallamiento** / **bombardeo guiado** / **ataque aéreo PEM** | propuesta | EMP = PEM (oficial) |
| private / corporal / sergeant | **soldado raso** / **cabo** / **sargento** | RML / oficial | |
| ceremonial sergeant | **sargento ceremonial** | propuesta | |
| civilian aide / assistant / coordinator | **ayudante civil** / **asistente civil** / **coordinador civil** | propuesta | |

## Equipo, mecas y combate

| Inglés | Español | Origen | Notas |
|---|---|---|---|
| mech / automatroid | **meca** / **automatroide** | familia | |
| mechlink / temporary mechlink / kill switch | **mecaenlace** / **mecaenlace temporal** / **interruptor de apagado** | oficial / RML | «interruptor de apagado del mecaenlace» |
| Mechlink serum | **suero de mecaenlace** | oficial | «Suero», como la categoría oficial |
| neuro-booster / dependence | **neuropotenciador** / **dependencia del neuropotenciador** | propuesta | |
| readiness / low readiness | **preparación para el combate** / **preparación baja** | familia | Como en FFF |
| wreck (M7 Cobalt wreck) | **restos de M7 cobalto** | oficial | Como «restos de flamígero» |
| order (M7 «Cobalt» order) | **pedido de M7 cobalto** | propuesta | |
| supply crate (S/M/L) | **caja de suministros (P/M/G)** | RML | Las tallas en español |
| body bag / corpse storage | **bolsa para cadáveres** / **almacenamiento de cadáveres** | propuesta | |
| B-ration | **ración B** | propuesta | «calentar ración B» |
| HESCO wall | **muro HESCO** | propuesta | HESCO es una marca real |
| assault mining rig / orbital drill shell | **equipo de minería de asalto** / **proyectil de perforación orbital** | propuesta | |
| marksman rifle / battle rifle / auto rifle | **fusil de tirador** / **fusil de batalla** / **fusil automático** | RML | «fusil» para lo militar (familia) |
| DMR / GPMG / LMG | **DMR** / **GPMG** / **LMG** | RML / familia | Siglas como el juego, explicadas en la descripción |
| advanced support weapon / objective sniper weapon | **arma de apoyo avanzada** / **arma de francotirador objetivo** | propuesta | |
| Bushmaster autocannon / Cobra assault cannon | **cañón automático Bushmaster** / **cañón de asalto cobra** | oficial / familia | «cañón automático» es oficial |
| Torchwood recoilless rifle (RR) | **cañón sin retroceso Torchwood** | familia | «cañón sin retroceso» |
| assault / heavy / gladiator / night vision helmet | **casco de asalto** / **pesado** / **gladiador** / **de visión nocturna** | propuesta | |
| Hardiman unit | **unidad Hardiman** | propuesta | |
| APS | **SDA** | RML | Sistema de defensa activa |
| mineclearing shovel / towed mineclearing line | **pala de desminado** / **línea de desminado remolcada** | propuesta | |
| programmable shaped charge | **carga hueca programable** | familia | |
| depleted uranium (DU) | **uranio empobrecido** (**DU**) | CE | La sigla queda: «bala 7.62x51mm NATO (DU)» |
| rifleman / sniper / heavy sniper / exo infantry / elite | **fusilero** / **francotirador** / **francotirador pesado** / **infantería exo** / **élite** | RML / oficial | |

## Partes del cuerpo

66 de las 120 son idénticas a partes del juego y se toman de la oficial. Las de los mecas:

| Inglés | Español | Origen |
|---|---|---|
| sight sensor / hearing sensor (left, right) | **sensor de visión** / **sensor auditivo** (izquierdo, derecho) | oficial (`Bodies_Mechanoid.xml`: «sensor de visión izquierdo», «sensor auditivo derecho») |
| central sight sensor | **sensor de visión central** | propuesta, con la forma oficial |
| armor plate (chest, waist, leg) | **placa de blindaje** (pectoral, de cintura, de pierna) | propuesta |
| main battery / battery | **batería principal** / **batería** | propuesta |
| vertical / horizontal rotor | **rotor vertical** / **horizontal** | propuesta |
| mount lock / range finder | **traba de montaje** / **telémetro** | propuesta |
| left / right mechanical leg | **pierna mecánica izquierda** / **derecha** | propuesta |

## Nombres generados (`RulePackDef`)

- **Sitios** (`r_name->[sitetype]-[number]`): en español el adjetivo va detrás y concuerda, así que
  `[prefix] [sitetype]` pasa a `[sitetype] [prefix]` y todas las opciones de `sitetype` van en el mismo
  género: **depósito, puesto, centro, nodo, complejo, núcleo** (masculinos), con **avanzado, primario,
  secundario, remoto**. Las letras griegas y los números no se traducen.
- **La ideología corporativa** (`CultureDef` «Zenith Ascension»: propuesta **Ascensión Cénit**) arma
  nombres con `[corpdomain]`, `[corpeventtheme]`, `[cultureleadertitle]`: cada símbolo se traduce con
  opciones del mismo género que pida la regla que lo usa, igual que arriba.
- **«this world's enlightenment»** (la facción, *A vast corporation born out of…*): «la ilustración de este
  mundo», por el *Enlightened World* de Expedition, que es «mundoilustrado» (Angel, 26-oct-2026).
