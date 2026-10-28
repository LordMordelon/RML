# Plan: las actualizaciones del 27-oct-2026

Angel reextrajo cinco mods el 27-oct-2026: tres porque sus autores los actualizaron y dos que entran
nuevos a RML. Este plan reúne lo que dejó la extracción, lo que hay que revisar y el orden de trabajo.
Las cifras se contaron ese día sobre los archivos de `Data/`, antes de commitear.

## Qué dejó la extracción

| Mod | Carpeta | Pendientes | Palabras en inglés | Qué pasó |
|---|---|---|---|---|
| SimpleBabyCarry | `Data/` (suelto) | 265 | ~2.240 | Nuevo |
| Vanilla Outposts Expanded | `!Oskar Potocki/` | 87 | ~950 | Nuevo |
| Animals Logic | `Data/` (suelto) | 24 | ~310 | El autor rehízo sus opciones y estadísticas |
| Shipcracker Warcasket | `Data/` (suelto) | 2 | ~40 | Una opción nueva para Combat Extended |
| Altered Carbon 2: ReSleeved | `!Helixien/` | 0 | — | El autor corrigió el inglés de 166 textos |
| **Total** | | **378** | **~3.540** | |

A unas 3.000 palabras por sesión, son **dos sesiones**: una para SimpleBabyCarry y otra para el resto.

**Las tres comprobaciones del diff** de `AGENTS.md` salen vacías: ninguna traducción cambió de valor,
no se borró ningún `UNUSED.xml` y ninguna regla de gramática perdió su `->`. También se regeneraron
bien el índice, `ModList.tsv`, la página y el `forceLoadAfter` de `About.xml`, con los dos mods nuevos.

## Lo que la extracción hizo bien, y conviene saber

- **Animals Logic apartó 30 traducciones en un `UNUSED.xml` nuevo.** El autor sacó las estadísticas
  de leche, lana y huevos (`MilkType`, `EggAmount`, `GestationPeriod`…) y la opción de convertir
  huevos estropeados. Están en el juego base o ya no existen: el `UNUSED.xml` se commitea tal cual y
  no se toca (regla 3 de `AGENTS.md`).
- **Altered Carbon conservó todas sus traducciones.** Solo cambió el comentario `EN:` de 166 textos.
  En 123 el cambio es de ortografía o de comas y la traducción sigue sirviendo; en 43 cambió alguna
  palabra (ver el paso 3).

## Decisiones de Angel (27-oct-2026)

1. **Todo lo que lleva al bebé es «portabebés»**, también el *sling*: «portabebés de tela»,
   «portabebés tribal», «portabebés espacial». Está en
   [GLOSARIO-SBC.md](Data/SimpleBabyCarry%20-%203692756679/GLOSARIO-SBC.md).
2. **Los puestos se llaman «puesto de avanzada …»**, como propone
   [GLOSARIO-VOE.md](Data/!Oskar%20Potocki/Vanilla%20Outposts%20Expanded%20-%202688941031/GLOSARIO-VOE.md).
3. **Vanilla Outposts Expanded va en `Data/!Oskar Potocki/`**, con los demás Vanilla Expanded. Su
   `About.xml` pone primero a legodude17 en `<authors>`, así que el `Agrupador` lo había dejado suelto.
   Se movió a mano; el extractor lo encuentra en cualquier nivel.
4. **Este plan se borra cuando estén las cinco traducciones.**

## Orden de trabajo

Primero se commitea la extracción tal como quedó, en un commit aparte («Reextraer cinco mods
actualizados…»). Así cada tanda de traducción después tiene su propio diff limpio y, si una sale mal,
se revierte sola.

Cada tanda va con la tabla (`Herramientas/traducir.py`, ver «Editar XML de traducción» de
`AGENTS.md`): `tabla`, escribirla a mano con el glosario del mod y la traducción oficial abiertos, y
`aplicar`, que corre las tres comprobaciones. Un commit por mod.

### Paso 1. Los chicos: Shipcracker Warcasket y Animals Logic (sesión 1)

- **Shipcracker** (2 textos): la penetración de armadura en MPa del aterrizaje, para Combat Extended.
  Usar los términos de [GLOSARIO-COMBAT-EXTENDED.md](Data/Combat%20Extended%20-%202890901044/GLOSARIO-COMBAT-EXTENDED.md)
  («penetración de blindaje», oficial; *flak armor* es la armadura del juego, «chaleco antibalas»).
- **Animals Logic** (24 textos): la crisis mental «Hunting spree» («cacería frenética», como ya está
  en tres mods de RML), su frecuencia, y las estadísticas
  de recursos de animales de otros mods (cantidad, intervalo, por año, valor de mercado, productos
  al azar o variables, extras). «Market value» es «valor de mercado», oficial.
- **Animals Logic, 5 traducciones viejas que quedaron mal** porque el inglés cambió de sentido. No
  son `TODO`, así que van con `traducir.py corregir <mod> <cambios.tsv>`:

  | Clave | Inglés nuevo | Traducción que tiene hoy | Qué falla |
  |---|---|---|---|
  | `ALConfigShowExtraDisplayStatsLabel` | Show production stats for modded animal resources | Mostrar estadísticas adicionales para los peones | Ya no habla de peones |
  | `ALConfigShowExtraDisplayStatsTooltip` | Vanilla info cards only list milk and wool… | Muestra algunas estadísticas que el juego base no incluye… | Otro texto |
  | `ALConfigComeWhenCalledLabel` | …for training or play | …para entrenar | Falta «o jugar» |
  | `ALConfigComeWhenCalledTooltip` | …come to the handler… dropping whatever they were doing… | …irán hacia el entrenador… | Le falta la mitad del texto |
  | `AL_ResourceAmount` | {0} amount | Cantidad de producto | Falta el `{0}`: no muestra qué recurso es |

### Paso 2. Vanilla Outposts Expanded (sesión 1)

87 textos, ~950 palabras, con [GLOSARIO-VOE.md](Data/!Oskar%20Potocki/Vanilla%20Outposts%20Expanded%20-%202688941031/GLOSARIO-VOE.md).
Lo más largo son las once descripciones de puestos («Places a temporary, autonomously-led camp…»):
conviene traducir la primera y copiar ese comienzo en las demás, para que se lean parejas. Las cartas
de asalto interceptado usan `{0}` y `{1}` para lugares y facciones: dejarlos en el orden que pida el
español.

### Paso 3. Altered Carbon: revisar 43 traducciones (sesión 1, si sobra tiempo)

No hay nada que traducir, pero en 43 textos el autor cambió palabras y no solo comas. En casi todos la
traducción ya decía lo correcto, porque el traductor había entendido el inglés roto. Hay que leerlas
contra el inglés nuevo y corregir solo las que fallen, con `traducir.py corregir`. Una ya se sabe que
falla: **`AC.Tutorial`** dice «la neutralidad de los colonos almacenados» (el inglés viejo decía
*neurallity*, mal escrito); ahora dice *consciousness*, «la conciencia».

Las 43 claves: `AC_WingJumpAbility.description`, `AC_CryptoStasis.description`,
`AC_UnboundPersona.description`, `AC_EditActiveNeuralStack.description`,
`AC_EditActiveNeuralStackPawn.description`, `AC_NeuralCasting.description`,
`AC_ChrysalisPoweredArmor.description`, `AC_Apparel_WingJump.description`,
`AC_Apparel_HayabusaArmor.description`, `AC_Apparel_HayabusaHelmet.description`,
`AC_EmptyNeuralStack.description`, `AC_CompactNeuralEditor.description`,
`CryptosleepCasket.description`, `AC_StackDegradationThought.stages.0/2/3/4/5.description`,
`AC_XenoSleeving_Despised_Xenosleeved.stages.0.description`,
`AC_RaceSleeving_Despised_Racesleeved.stages.0.description`, `AC_ArchotechStackSkip.description`,
`AC.StartNeedlecastingDesc`, `AC.CaravanNeedlecastingIconTooltip`, `AC.CasterStackDestroyed`,
`AC.CannotTakeToSleeveCasketOrMedicalBed`, `AC.RepurposeCorpseDesc`, `AC.CancelRepurposingCorpseDesc`,
`AC.CuirassierBeltTooltip`, los seis `AC.AlreadyOrderedTo…`, `AC.ChangingPawnIdeologyWarning`,
`AC.Tutorial`, `AC.CancelStackResetDesc`, `AC.CancelBiocodingResetDesc`,
`AC.CancelUnbondingPersonaDesc`, `AC.TimeToGrowSleeveMultiplierDesc`,
`AC.CostToGrowSleeveMultiplierDesc`, `AC.StackEditEditTimeValueMultiplierDesc` y
`AC.BiocodableEquipWarning`.

Para ver el inglés de antes y el de ahora de cada una: `git diff <commit anterior a la extracción>
-- "Data/!Helixien/Altered Carbon 2 ReSleeved - 2196278117"`.

### Paso 4. SimpleBabyCarry (sesión 2)

265 textos, ~2.240 palabras, con [GLOSARIO-SBC.md](Data/SimpleBabyCarry%20-%203692756679/GLOSARIO-SBC.md).
Dos tandas de ~1.100 palabras:

1. **Los `DefInjected`** (108 textos): prendas, estados de salud, trabajos, pensamientos. Los
   `reportString` van como en el juego, en gerundio y con punto: «llevando al bebé a la cuna.».
   Los pensamientos del bebé hablan en primera persona («Me están llevando. Me siento abrigado y
   seguro»): el adjetivo cambia con el género solo si el texto recibe al bebé; si no, buscar una
   forma neutra («Me siento a salvo»).
2. **El `Keyed`** (157 textos): sobre todo opciones del mod. Muchos llevan `{BABY}`, `{PAWN_labelShort}`
   o `{0}`: el comando avisa si falta alguno. Los rótulos de opciones son largos en inglés; si en
   el juego se cortan, se acortan después, no antes, y sin abreviaturas como «conf.».

## Al terminar

- Probar en el juego los dos mods nuevos: el mapa del mundo con un puesto de avanzada, y un colono
  llevando un bebé (el menú del clic derecho y la ventana de opciones del mod).
- Si una propuesta del glosario cambió en el camino, el glosario se corrige en el mismo commit que la
  traducción.
- Cuando estén los cinco, este plan se borra: lo que quede de él va a los glosarios.
