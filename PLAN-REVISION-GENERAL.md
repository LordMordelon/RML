# Plan: revisión general de RML

El 27-oct-2026, con la familia de AobaKuma terminada, Angel pidió revisar todo RML con las herramientas y
dejar los arreglos para sesiones posteriores. Los glosarios se revisan primero el principal
([GLOSARIO.md](GLOSARIO.md)) y después los de cada mod. Este plan reúne lo que salió y el orden para
arreglarlo. Las cifras son de ese día.

**La regla que decide casi todo:** si un glosario contradice a la traducción oficial, gana la oficial (Angel,
25-oct-2026). Lo que sigue de esa regla no necesita a Angel. Lo que la oficial no resuelve está en
«Decisiones de Angel».

## Cómo se revisó

- **Glosarios contra la oficial:** `Herramientas/oficial/cotejar.py` sobre los 18 glosarios. Muestra las filas
  cuyo inglés es idéntico a una etiqueta oficial y cuyo español no la contiene. Casi todos sus avisos son una
  palabra suelta que en el juego es otra cosa («light» como «luz», «heat» como «calor»): cada uno se miró en su
  fila.
- **Glosario principal, término por término:** cotejar.py solo ve etiquetas idénticas, así que además se buscó
  cada término del principal en los textos de la oficial. Los que no aparecían se buscaron por su def.
- **Glosarios de mod contra el principal:** las filas con el mismo inglés y otro español.
- **Las traducciones:** las 108.353 de `Data/` y `Ajustes/` pasaron por las revisiones de `traducir.py aplicar`
  (XML válido, llaves, colores, marcadores), y las 1.021 listas de reglas de gramática, por una revisión aparte:
  cada `<li>` con su `->` y ningún símbolo usado sin definir. Todos los XML se leen y las tres comprobaciones
  del diff salen vacías.

La oficial se lee de los `.tar` del juego, sacados a la carpeta temporal con
`tar --force-local -xf "<.tar>" -C <carpeta>`: hoy vuelven a estar empaquetados.

## Paso 1: el glosario principal

Cotejado contra la oficial, no choca en ninguna fila: la pasada del 25-oct ya lo había corregido. Está
ordenado y sin filas repetidas. Faltan estos arreglos:

| Fila | Dice | La oficial | Arreglo | Usos en RML |
|---|---|---|---|---|
| *ripscanner* | «escanecorte» / «escáner de corte» | **rasgaescáner de subnúcleos** (`SubcoreRipscanner`) | Cambiar la fila | 2 de «escanecorte» |
| *softscanner* | junto con *subcore scanner*: «escáner de subnúcleos» | **flexiescáner de subnúcleos** (`SubcoreSoftscanner`) | Fila propia | ninguno con el nombre oficial: mirar cómo quedó |
| *supercharger* | «sobrecargador» | **supercargador neural** (`NeuralSupercharger`) | «supercargador» | 0 de «sobrecargador», 5 de «supercargador» |
| *command range* | «rango de mando / alcance de mando» | «Fuera de **rango del comando**» | Dejar «rango del comando» como la oficial | 9 de «alcance de mando» |
| *raid / assault* | «asalto / incursión» (no dice cuál es cuál) | `Raid` = **asalto**, «asalto enemigo»; en la oficial «asalto» sale 46 veces e «incursión» 12 | *raid* = «asalto»; «incursión» queda para *incursion* | Ver «Decisiones de Angel» |
| *creepjoiner* | «forastero anómalo» | El juego no lo nombra en pantalla (`CreepJoiner` = «humano») | Anotar que es propuesta | — |

Lo demás del principal (mecanizador, banda ancha, subnúcleo, sub-IA, packs de residuos, hilodiablo, tipos de
trabajo, las descripciones de referencia…) aparece tal cual en la oficial. «Sarcófago de mando», «aparato de banda»
y «quimiofango» no están en el juego base: son de mods y quedan como están.

## Paso 2: los glosarios de cada mod

Choques reales con la oficial o con el principal. Los de la columna «Ya pendiente» ya figuraban en un plan.

| Glosario | Fila | Dice | Debe decir | Usos en RML | Ya pendiente |
|---|---|---|---|---|---|
| DMS (`GLOSARIO-DMS.md`) | *exostrider* | «exoandador» | **exozancador** (oficial de Odyssey, y el principal) | 0: solo el glosario | |
| Citadel | *concrete* | «concreto» | **hormigón** (`Concrete`) | 26 en 10 mods | |
| Citadel | *autocannon turret* | «torreta de cañón automático» | **torreta de cañón automática** | 9 | |
| Expedition | *ancient stockpile entrance* | «entrada de almacén antiguo» | **acceso al almacén antiguo** | 3 | |
| ReGrowth 2 | *chokevine* | «enredadera estranguladora» | **chokevine** (la oficial de Odyssey lo deja en inglés) | 4 | |
| Buffs 'n Nerfs | *slugthrower* | «lanzabalas» | **lanza perdigones** (oficial) | 31 (no todos serán *slugthrower*) | |
| Alpha Prefabs | *magister* | «magister» | **magistrado** | — | sí (AobaKuma) |
| Caravan Adventures | *town councilman* | «concejal» | **concejal citadino** | — | sí |
| Combat Extended | *armor penetration*, *fire rate*, *hold fire*, *stopping power* | «penetración de armadura», «cadencia de fuego», «alto el fuego», «poder de detención» | **penetración de blindaje**, **cadencia de tiro**, **cese al fuego**, **potencia de parada** | — | sí |
| Combat Extended | *autocannon shell* | «obús de cañón …» | **proyectil de cañón automático** (oficial) | a contar | |
| RimPacts | *ideoligion*, *peace talks*, *shuttle* | «ideología», «charlas de paz», «lanzadera» | **ideoligión**, **diálogos de paz**, **transbordador** | — | sí |
| RimPacts | *raid* | «incursión» | **asalto** | ver «Decisiones» | |

Avisos que **no** son choques, para no volver a mirarlos: los rangos militares de Pirates («major» = «mayor», no
«grave»); «venerado» de DMS (grado de un precepto propio, no el animal «sagrado» del juego); «psicofoco» de
Taktikmagier (es el *psyfocus* oficial); *gestate* = «ensamblar» en DMS (solo para automatroides, explicado en
su fila); *EMP* = «PEM» en CE (la fila ya lo dice); *casing* = «vaina» en CE (la vaina de un cartucho, no el
chasis de un meca); las mayúsculas y el género de las palabras sueltas.

## Paso 3: las reglas de gramática rotas

Esto sí se ve en el juego: la regla falla o sale el símbolo entre corchetes.

| Mod | Archivo | Qué pasa |
|---|---|---|
| DMS de 1.6 (3121742525) | `QuestScriptDef/1vu7a5.xml`, `3opti5.xml`, `reqdcf.xml`; `RulePackDef/1ezexq.xml` y `ft1lfo.xml` | 44 reglas con `&gt;` sin el guion (`manufacture&gt;…`): no son reglas |
| DMS de 1.6 | `RulePackDef/06gdc7.xml`, `b61nht.xml`, `wvd86k.xml`, `1ezexq.xml`, `ft1lfo.xml` | Símbolos traducidos en un lado y no en el otro: se usa `[Tipo]`/`[MecaTipo]` y se define `Type`/`MechType` |
| Ascension Megacorp | `RulePackDef/7y4ut7.xml` | `[corpGod] de[memeConceptDef]`: el inglés dice `[memeConcept]`, y falta el espacio |
| Vanilla Social Interactions Expanded | 12 `RulePackDef` (`VSIE_Teaching_*`) | La regla 3 usa `[commentedabout]`, que la traducción no define: faltan sus 5 opciones |
| VFE - Insectoids 2 | `QuestScriptDef/ok12s0.xml` | Se usa `[insectoid_masc]` y solo se define `insectoid_fem` |
| Hospitality (Continued) | `InteractionDef/98958g.xml` | Se usa `[adjwords_m]` y solo se define `adjwords_f` |
| VFE - Deserters | `QuestScriptDef/adaojn.xml` | «está siendo [hunted] por»: `[hunted]` no existe; va «perseguido» como texto |
| Alpha Books | `RulePackDef/4hkvi8.xml` | `[CENSURADO]`, `[CLOQUEO]`, `[TIEMPO]`: el inglés también usa `[REDACTED]` sin definir. Mirar en el juego antes de tocar |

El DMS de 1.6 está congelado para no perder lo traducido, pero estas reglas están rotas en 1.6 y arreglarlas no
cambia ninguna traducción. Texto feo pero no roto: tres pensamientos de VSIE (`ThoughtDef/aiug33.xml`) dicen
«en deuda con {OTHERPAWN_objective}», que sale «con lo» o «con la»; mejor el nombre.

Falsos avisos de la revisión, para no volver a mirarlos: textos con una flecha «->» que no son reglas (Butter++,
RimIris, CE, FSF, ReSplice, Ushanka), los símbolos de arte que la oficial define en Noobert, `[PAWN_pronoun]`,
`[INITIATOR_nameDef]` y `[PRODUCT_definite]`, que son del juego, y 1.542 avisos de marcadores que el español saca
con razón.

## Paso 4: los términos viejos en los textos

Cada fila que se corrige en un glosario deja textos con el término viejo. Se corrigen a mano, mod por mod, sin
tocar los `UNUSED.xml`, con `traducir.py corregir`. A los de los pasos 1 y 2 se suman los que ya estaban
pendientes en `GLOSARIO.md` y en [PLAN-AOBAKUMA.md](Data/!AobaKuma/PLAN-AOBAKUMA.md) («Pendientes fuera de este
plan»): «lanzadera» (172 usos), «entropía psíquica» (26), «abrasador» (22, a revisar), «psiconjuro» (14), «EMP»
(15), «mundo del borde» (13), «calor neuronal» (7), «sanguófago» (6), «ensamblador de genes» (6), «clúster
mecanoide» (4), «cápsula de descenso» (4 mods), «garra de la cabeza» (3 mods) y los sueltos.

## Decisiones de Angel

Las tres se decidieron el 27-oct-2026, las tres como la traducción oficial.

1. **«asalto» para *raid*.** La oficial dice «asalto» (46 usos, contra 12 de «incursión»). El principal y
   RimPacts pasan a «asalto», y se revisa cada una de las 49 «incursión» de RML: solo se cambian las que vienen de
   *raid*; «incursión» queda para *incursion*.
2. **«celda» para una casilla del mapa (*tile*).** La oficial dice «celda» (86 usos, contra 5 de «casilla»);
   «baldosa» es el piso. Combat Extended y RimPacts pasan a «celda», y se revisan las 298 «casilla» de RML una por
   una, mod por mod: no todas son del mapa. Es el cambio más grande del paso 4.
3. **RimPacts, *cooldown* e *idle*: «enfriamiento» y «ocioso»**, como la oficial, también en la diplomacia. El
   glosario de RimPacts deja «tiempo de espera» y «libre».

## Orden de las sesiones

| # | Qué | Por qué en este orden |
|---|---|---|
| 1 | Paso 1: las filas del glosario principal | Angel pidió el principal primero; es el que manda sobre los demás |
| 2 | Paso 2: las filas de los glosarios de mod | Después del principal, para que no se contradigan |
| 3 | Paso 3: las reglas rotas | Son pocas y se ven en el juego; no dependen de los glosarios |
| 4 en adelante | Paso 4: los textos, mod por mod, un commit por mod | Lo más largo; se corrige con los glosarios ya firmes |

Cada sesión: un commit por arreglo, con el mod en el mensaje; las tres comprobaciones del diff; `LoadFoldersBuilder
-copia` al final. Las decisiones de Angel se esperan antes de tocar lo que dependa de ellas, no antes de empezar.
