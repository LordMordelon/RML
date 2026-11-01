# Plan: revisión de Vanilla Psycasts Expanded

Revisión de la traducción de Vanilla Psycasts Expanded y sus agregados Hemosage y Puppeteer contra la
traducción oficial del juego. Se armó el 31-oct-2026. Los términos quedan en
[GLOSARIO-VPE.md](GLOSARIO-VPE.md); acá va lo que hay que cambiar, dónde se ve en el juego y lo
que falta decidir.

**Estado:** las secciones 1 a 6 están hechas (1-nov-2026). Angel aprobó las propuestas
recomendadas. Al revisar el contexto cambiaron dos cosas: *enthrall* revive a un muerto y no deja
el rasgo *thrall*, así que queda «esclavizar» y solo el rasgo pasa a «siervo»; y la carta del
hemosabio dice «Hemogénicos». Después Angel pidió sacar «Solo» de todas las cartas bloqueadas:
«Imperiales», «Tribales», «Hemogénicos» y, en Vanilla Races Expanded - Archon, «Trascendentes». Además se corrigió «sangúfago» → «sanguífago» en Hemosage (4 textos).
Lo nuevo de la segunda revisión, al final, está **propuesto**.

**Cómo se hace:** a mano sobre los XML de `Data/`, un commit por arreglo («Vanilla Psycasts
Expanded: «salto» por «teletransportación» en los poderes de Royalty»), con las cinco
comprobaciones (`python -I Herramientas/traducir.py comprobar`) antes de cada commit. Sin push
sin preguntar.

## Capturas

Sacadas por Angel en el juego el 31-oct-2026, antes de cualquier cambio. Están fuera del repo, en
`Traducciones RimWorld/capturas-rml/vpe/`, porque lo que hay en `Data/` va a la copia del Workshop
(`CopiaLimpia` solo deja afuera `.md`, `.yaml` y `UNUSED.xml`).

- `01-pestanas.png`: la fila de pestañas al seleccionar un colono. «Psico» entra justo; las
  pestañas son angostas («Necesidad» ya va pegada a «Personaje»).
- `02-ventana-psicoemisiones.png`: la ventana de la pestaña. Se ve:
  - El título de la pestaña interna dice **«Psycasts», en inglés**. No está en ningún `Keyed` del
    mod (la única clave con ese inglés es `VPE.Psycasts`, que es la pestaña de abajo): está escrito
    en la DLL. Solo se traduce con Harmony, como los nombres de VE en la configuración.
  - **«Bloqueado: Solo imperiale…» y «Bloqueado: Solo hemogén…» se cortan.** El botón deja unos
    25 caracteres; «Bloqueado: Solo tribales» (24) entra.
  - «Tipos de foco:» y «focos de meditación»: la oficial dice «enfoque de meditación».
  - «Gana experiencia psíquica meditando.» y «Estadísticas psíquicas»: *psycaster*.
  - Los caminos salen en orden alfabético del nombre en español (Acechador nocturno, Arconte,
    Armonista…): cambiar un nombre de camino cambia su lugar en la ventana.

## 1. Los «saltos»

**Copiadas de Royalty: van con el nombre oficial.**

| Inglés | Hoy | Propuesta | Dónde se ve en el juego |
|---|---|---|---|
| skip | salto | **teletransportación** | Botón del poder al seleccionar al psicoemisor, y su carta en la ventana Psico (camino del maestro del salto) |
| farskip | salto lejano | **transposición aliada** | Igual |
| chaos skip | salto caótico | **caoskinesis** | Igual |
| mass chaos skip | salto caótico masivo | **caoskinesis múltiple** | Igual |
| chunk skip | salto de trozos | **psicokinesis** | Igual |
| waterskip | salto de agua | **acuakinesis** | Igual |
| skipshield | escudo de salto | **psicoescudo** | Igual |

**Propias del mod: se quedan con «salto».** (decide Angel)

| Inglés | Hoy | Propuesta | Dónde se ve en el juego |
|---|---|---|---|
| reunion farskip | salto lejano de reunión | **transposición de reunión** | Botón del poder y ventana Psico |
| skipdoor | puerta de salto | igual | Botón del poder, y el portal en el mapa al pasarle el mouse |
| killskip | salto mortal | igual | Botón del poder |
| shadowskip | salto de sombra | igual | Botón del poder (acechador nocturno) |
| skipmaster | maestro del salto | igual | Ventana Psico: nombre del camino |
| guardian skipbarrier | barrera de salto guardiana | igual | Botón del poder, y pestaña Salud del lanzador mientras dura |
| craft / plant / season timeskip | salto temporal de fabricación / de plantas / de estación | igual | Botones del cronópata |
| timeskip meditation | meditación de salto temporal | igual | Botón del cronópata |
| skipgate | portal de salto | igual | Solo en las descripciones, al pasar el mouse por el botón |

## 2. «thrall» (decide Angel)

Hoy «esclavo», que es el *slave* oficial de Ideology. No es el sanguiesclavo de Biotech.

| Inglés | Hoy | Propuesta A (recomendada) | Propuesta B | Dónde se ve en el juego |
|---|---|---|---|---|
| thrall (rasgo) | esclavo | **siervo** | sanguiesclavo | Pestaña Personaje, rasgos del afectado |
| Psychic enthrallment… (descripción del rasgo) | Esclavitud psíquica… Los esclavos… | **Servidumbre psíquica**… Los **siervos**… | — | Al pasar el mouse por el rasgo |
| Thrall (pensamiento) | Esclavo | **Siervo** | Sanguiesclavo | Pestaña Necesidad, estado de ánimo |
| enthrall (poder) | esclavizar | **someter** | esclavizar | Botón del poder del necrópata |
| mind break: «…a serving, restless thrall» | …un esclavo servil e inquieto | …un **siervo** servil e inquieto | — | Al pasar el mouse por «quiebre mental» |
| necrópata: «wicked enthrallment» | una esclavitud perversa | un **sometimiento** perverso | — | Ventana Psico, al pasar el mouse por el camino |

«El objetivo debe ser prisionero o esclavo» (Puppeteer) queda: ese sí es *slave*.

## 3. «timequake» y «neuroquake» (decide Angel el de timequake)

| Inglés | Hoy | Propuesta | Dónde se ve en el juego |
|---|---|---|---|
| neuroquake (poder) | neuroterremoto | **neurosismo** (oficial) | Botón del poder |
| «Do you really want to cast neuroquake?» | ¿Realmente quieres lanzar neuroterremoto? | …lanzar **neurosismo**? | Ventana de confirmación al usarlo |
| timequake (poder) | terremoto temporal | **sismo temporal** | Botón del poder del cronópata |
| «Do you really want to cast timequake?» | …lanzar terremoto temporal? | …lanzar **sismo temporal**? | Ventana de confirmación al usarlo |
| time quake (condición) | terremoto temporal | **sismo temporal** | Arriba a la derecha, condiciones del mapa |
| time quake (pensamiento) | terremoto temporal | **sismo temporal** | Pestaña Necesidad: «…Este sismo temporal realmente me afectó» |
| descripción del cronópata | …extraños terremotos temporales… | …extraños **sismos temporales**… | Ventana Psico, al pasar el mouse por el camino |

## 4. «blizzard» (decide Angel)

| Inglés | Hoy | Propuesta A (recomendada) | Propuesta B | Dónde se ve en el juego |
|---|---|---|---|---|
| blizzard (poder) | ventisca | igual | tormenta de nieve | Botón del poder del formador de escarcha |
| blizzard (efecto) | ventisca | igual | tormenta de nieve | Pestaña Salud de quienes están dentro |
| casting blizzard | lanzando ventisca | igual | lanzando tormenta de nieve | Pestaña Salud del lanzador |

## 5. La ventana Psico (de las capturas)

| Inglés | Hoy | Propuesta | Dónde se ve en el juego |
|---|---|---|---|
| Psycasts (`VPE.Psycasts`) | Psico | **igual**: «Psicoemisiones» no entra en la pestaña | Fila de pestañas (captura 01) |
| Psycasts (título interno) | Psycasts | Solo con Harmony; queda en inglés | Arriba de las cartas (captura 02) |
| Imperials only | Solo imperiales | (decide Angel) **Solo imperial** (24, entra) | Carta del arqueotecnólogo bloqueada |
| Hemogenic only (Hemosage) | Solo hemogénicos | (decide Angel) algo de ~14 caracteres; «Solo hemogénicos» no entra | Carta del hemosabio bloqueada |
| Focus types: | Tipos de foco: | **Tipos de enfoque:** | Bajo «Mejorar» |
| …unlocking meditation foci… | …desbloquear focos de meditación… | …desbloquear **enfoques** de meditación… | Texto bajo «Puntos disponibles» |
| Earn psycaster experience by meditating. | Gana experiencia psíquica meditando. | Gana experiencia **de psicoemisor** meditando. | Bajo la barra de experiencia |
| Psycaster stats | Estadísticas psíquicas | Estadísticas **de psicoemisor** | Junto a «Mejorar»; ya va en dos líneas, entra |

## 6. Lo demás, sin decisión pendiente

| Inglés | Hoy | Propuesta | Dónde se ve en el juego |
|---|---|---|---|
| Raid (Psycaster) / Friendlies (Psycaster) | Asalto (Psíquico) / Amistosos (Psíquico) | …(**Psicoemisor**) | Título de la carta del asalto |
| Not a psycaster / Must be a psycaster | No es un psíquico / Debe ser un psíquico | …un **psicoemisor** | Aviso en rojo al intentar algo con alguien sin psicoenlace |
| Unlocked via Psycaster points | …puntos de psíquico | …puntos de **psicoemisor** | Ventana Psico, al pasar el mouse por una carta |
| Maximum psycaster level / XP per Psyfocus % / Basilicus spawn chance | …de psíquico | …de **psicoemisor** | Opciones → Configuración de mods → VE - Psycasts |
| Descripciones de caminos y del equipo eltex | …psíquicos… | …**psicoemisores**… | Ventana Psico y pestaña Investigación |
| psychic shock | choque psíquico | **shock psíquico** | Botón del poder; pestaña Salud del afectado |
| neural heat dump | descarga de calor neural | **neurodescarga** | Botón del poder |
| painblock | bloqueo de dolor | **indoloro** | Botón del poder |
| focus | foco | **concentración** | Botón del poder |
| berserk / berserk pulse | furia / pulso de furia | **frenético** / **pulso frenético** | Botón del poder |
| manhunter pulse | pulso cazador de hombres | **pulso cazahombres** | Botón del poder |
| territorial manhunter | cazador de hombres territorial | **cazahombres territorial** | Sobre el animal y en la carta «¡{0} se ha convertido en…!» |
| flashstorm | tormenta repentina | **tormenta eléctrica** | Botón del poder; condiciones del mapa |
| wallraise | alzar muro | **psicomuro** | Botón del poder |
| smokepop | estallido de humo | **bomba de humo** | Botón del poder |
| smokepop (descripción) | Salta partículas de polvo… | **Levanta** partículas de polvo… | Al pasar el mouse por el botón |
| solar pinhole | agujero solar | **ojo solar** | Botón del poder |
| word of… (5 de Royalty y 11 propias) | palabra de… | **palabras de…** | Botones del empático y del armonista |
| deathrest (Hemosage) | descanso mortal | **mortisueño** | Tipos de enfoque: ventana Psico (captura 02) y la cápsula de mortisueño |
| rock rubble | escombros de roca | **escombros de piedra** | Suciedad en el suelo, al pasarle el mouse |
| psylink (implante) | psicoenlace (dev) | **psicoenlace** | Pestaña Salud del psicoemisor |
| subjugation: minor / major (Puppeteer) | menor / mayor | **leve / grave** | Pestaña Salud: «subyugación (leve)» |

## Segunda revisión (propuesta, 1-nov-2026)

Se buscaron los textos que el mod copió de Royalty con cambios chicos (el inglés no es idéntico,
así que el primer cotejo no los vio) y las frases que suenan a traducción automática. Las
descripciones copiadas de Royalty están bien y no se tocan: son tan fieles como la oficial, y
varias mejor. Lo que sí se propone:

| # | Inglés | Hoy | Propuesta | Textos | Dónde se ve en el juego |
|---|---|---|---|---|---|
| 1 | …scales with psychic sensitivity | La duración **escala con** la sensibilidad psíquica del lanzador | La duración **depende de** la sensibilidad psíquica del lanzador («escalando con» → «según») | ~40 | Al pasar el mouse por los botones de casi todos los poderes |
| 2 | mental break (palabras de serenidad) | quiebre mental | **colapso mental** (oficial) | 2 | Descripción del poder |
| 3 | joyfuzz (palabras de alegría) | un bucle de alegría difusa… La alegría difusa | un bucle de **júbilo**… El **júbilo** (oficial) | 1 | Descripción del poder; el estado que deja ya sale «júbilo» en Salud, porque es el del juego |
| 4 | sad wandering | deambular triste | **vagando tristemente** (oficial) | 3 | Encima del personaje, la carta y «Estado mental: …» |
| 5 | psychic soothe / psychic sooth (female/male) | calma psíquica / calma psíquica (mujer) | **sedante psíquico** / **sedante psíquico (mujeres)** (oficial, la condición) | 3 | Botones del empático; pestaña Salud |
| 6 | consciousness (la capacidad) | consciencia | **conciencia** (oficial) | 5 | Descripciones de Puppeteer (sanguijuela cerebral, drenaje, subyugación, marioneta). «cambio repentino de consciencia» (salto mental) se queda: ahí es la mente, no la capacidad |
| 7 | takes 12/20 seconds to cast | toma 12 segundos para lanzarse | **tarda** 12 segundos **en** lanzarse (como ya dice Hemosage) | 4 | Descripción y aviso de neurosismo y sismo temporal |
| 8 | insane (accuracy, amount) | precisión insana / cantidad insana | precisión **extraordinaria** / cantidad **descomunal** | 2 | Salud del lanzador (disparo guiado); descripción de las perlas de calor |
| 9 | item | artículo | **objeto** | 4 | Cristal de hielo, salto temporal de fabricación, transmutación, aviso «debe valer al menos $1 por objeto» |
| 10 | pawn | peón / peones | **colono(s)** si son de la colonia, **persona(s)** si no. «El peón no tiene el gen de hemógeno» → «No es hemogénico.», como la oficial | 11 | Descripciones de poderes y caminos; aviso de Hemosage |
| 11 | tooltip (opciones del mod) | las estadísticas se mueven a una información sobre herramientas | las estadísticas pasan a un **recuadro que aparece al pasar el cursor** | 1 | Opciones → Configuración de mods → VE - Psycasts |
| 12 | focus (en los poderes propios) | foco de calor, foco de sombra, concentración en la hoja, concentración de disparo, enfoque de sangre | Una sola palabra, **concentración**, como el *focus* oficial: concentración de calor / de sombra / de sangre (decide Angel) | 7 | Botones del poder y pestaña Salud |

## Después

- Agregar al glosario lo que decida Angel y sacar de ahí «Pendientes».
- Prueba en el juego: un psicoemisor por camino; mirar la ventana Psico, los botones y las cartas
  bloqueadas.
- Las 10 «entropía psíquica» de los poderes de hielo siguen en el paso 4 de
  `PLAN-REVISION-GENERAL.md`.
