# Plan: RimPacts - Diplomacy Overhaul

Traducir RimPacts (Workshop 3762723122, packageId `wowgag.RimPacts`, autor wowgag). Lo pidió Angel el
24-oct-2026. La carpeta ya está extraída y sin commitear, con la versión del mod de ese mismo día: es
nueva, así que no hay nada traducido que cuidar ni `UNUSED.xml`. Los términos van en
[GLOSARIO-RIMPACTS.md](GLOSARIO-RIMPACTS.md).

## Contexto

- **Casi todo es la ventana del mod, y sí se puede traducir.** A diferencia de Hegemony, RimPacts trae
  sus textos en `Languages/English/Keyed` (cinco archivos): la pestaña Diplomacia, los tratados, las
  cartas, los avisos y los ajustes. El extractor los juntó en `Keyed/4n7njq.xml`.
- **Es un mod de diplomacia con muchos sistemas:** confianza, tratados, pedidos de bienes, edificios de
  la colonia, cortes y matrimonios, espionaje, tributos y guerra de independencia, guerras con puntaje,
  guerra santa, unión personal, alianzas, estado cliente, guerra mundial con bloques, compañías de
  mercenarios, rutas comerciales, ligas y diplomacia con razas de otros mods. La guía del autor
  (`Docs/RimPacts_Features_and_Numbers.md` en la carpeta del mod) explica cada uno y sirve para entender
  el contexto de una frase.
- **Una carpeta de versión para 1.6 y 1.7.** Los `Patches` del mod (Combat Extended, Empire, Medieval
  Overhaul) no dejaron nada que traducir.
- **Las razas opcionales entraron todas juntas.** El mod carga la diplomacia de seis razas solo si su
  mod está activo (`LoadFolders/RaceDip/`), pero el extractor puso las seis en un solo archivo:
  `RimPacts.RptRaceDiplomacyDef/yy5us3.xml` (17 entradas). Se traducen igual. Sin el mod de la raza,
  el juego no encuentra ese def; eso debería quedar solo en el informe de traducción y no como error
  en el log, pero hay que confirmarlo al probar. Cinco de las razas son de mods que no están en RML;
  la sexta es la pielcerdil de Biotech.
- **No hay colores ni marcadores raros:** ningún `<color>`. Sí hay muchos `{0}` y `\n`, que van igual
  que en el inglés.

## Cifras (contadas el 24-oct-2026)

| Parte | Pendientes | Palabras en inglés |
|---|---|---|
| `Keyed` (la ventana, cartas, avisos y ajustes) | 4.162 | 37.428 |
| `FactionDef` (estados clientes y compañías de mercenarios) | 95 | 641 |
| `HediffDef` (efectos de edificios, guerra y mercenarios) | 67 | 465 |
| `TreatyDef` (los diez tratados) | 20 | 455 |
| `ThoughtDef` | 48 | 303 |
| Edificios (`RptBuildingDef`, `RptMercBuildingDef`, `RptPuppetBuildingDef`) | 64 | 564 |
| `RptRaceDiplomacyDef` (razas) | 17 | 181 |
| `AbilityDef`, `ResearchProjectDef`, `MainButtonDef`, `IncidentDef`, `ThingDef` | 25 | 188 |
| **Total** | **4.498** | **unas 40.200** |

Es un poco más que Hegemony (36.000), y el doble de entradas, porque los textos de la ventana son
cortos.

## Decisiones de Angel (24-oct-2026)

1. **Tabla y script, como en Hegemony: aprobado, con una condición** («sí, solo si se traducen
   naturalmente, y revisa el resultado»). A mano serían más de 4.000 ediciones sueltas. Cada traducción
   se escribe a mano, una por una, en una tabla clave → traducción; el script
   (`Traducciones RimWorld/herramientas-traduccion/aplicar.py`) solo la copia sobre los `TODO`. La
   condición quiere decir dos cosas:
   - **Que suene a español, no a inglés traducido.** Reordenar la frase cuando haga falta, nada de
     calcos («Ahora mismo no hay compañías de mercenarios disponibles», no «No hay compañías de
     mercenarios disponibles ahora mismo»), y la misma voz del juego: tuteo, frases cortas en los
     botones.
   - **Revisar el resultado en el archivo, no solo la tabla.** Después de cada tanda, además de
     `revisar.py` y `terminos.py`, leer el XML ya traducido en orden, como lo leería el jugador, y
     corregir lo que suene raro junto a sus vecinos (un aviso y su descripción, un botón y su ayuda).
2. **«Client state» es «estado cliente»**, no «estado títere».
3. **Los términos marcados con ✔ en el glosario quedan aprobados** (confianza, corte, tributario,
   puntaje de guerra, guerra mundial, compañía de mercenarios, liga, exposición), después de ver frases
   reales del mod traducidas con ellos.
4. **«Quarter» es «cuadrun».** Angel pidió revisarlo en la traducción oficial y en los otros mods de
   RML: la oficial dice siempre «cuadrun» para el período de 15 días y usa «trimestre» solo en el
   embarazo, y Days Matter también usa «Cuadrun». Los demás nombres de tiempo están en la tabla
   «Tiempo» del glosario. **Regla general que dejó Angel:** ante cualquier término dudoso, buscar
   primero en la traducción oficial y en los mods de RML, y recién después proponer uno.

## Etapas

### 1. Glosario y plan (hecho, 24-oct-2026)
Este archivo y [GLOSARIO-RIMPACTS.md](GLOSARIO-RIMPACTS.md). Antes de cada término nuevo, buscar en
`GLOSARIO.md` y en la traducción oficial (los `.tar` del juego), y anotarlo en la última tabla del
glosario.

### 2. Lo corto y los archivos chicos (sesión 2)
- Todos los `DefInjected`: 336 entradas, unas 2.800 palabras. Así los nombres de tratados, edificios,
  facciones y efectos quedan fijos antes de que aparezcan en la ventana.
- `Keyed`, líneas 4 a 1699: los cuatro archivos chicos del mod, 840 claves y unas 8.700 palabras.
  Mercenarios contratados (`Rpt_HM_`), unión santa y rivales (`Rpt_Union_`), espionaje (`Rpt_SD_`) y
  comercio (`Rpt_TX_` y lo que sigue hasta `Rpt_RD_ActFactionGone`).

### 3. El archivo grande, en tres tandas (sesiones 3 a 5)
`Keyed/4n7njq.xml` desde la línea 1701 (`Rpt_CantSign_NotTarget`), de arriba hacia abajo:

| Tanda | Líneas | Claves | Palabras |
|---|---|---|---|
| 2 | 1701 a 4016 | unas 1.140 | unas 11.300 |
| 3 | 4017 a 6361 | unas 1.150 | unas 10.000 |
| 4 | 6362 al final | unas 1.020 | unas 7.900 |

Por tanda, con las herramientas de `Traducciones RimWorld/herramientas-traduccion/`:

1. Al empezar la sesión, copiar `Languages/` a la carpeta temporal de la sesión. Es el original para
   `revisar.py`: como la carpeta no está commiteada, no hay `HEAD` contra el cual comparar, y la
   carpeta temporal cambia en cada sesión.
2. `pendientes.py <xml> <desde> <hasta>` lista renglón, clave e inglés de los `TODO`, con los saltos
   reales del comentario ya escritos como `\n`. Leerlos de a unos 200.
3. Escribir la tabla (clave, TAB, traducción) con la herramienta Write, no con un heredoc de Bash:
   las comillas angulares y los apóstrofos rompen el heredoc. `aplicar.py` y después `revisar.py`.
   `revisar.py` marca `Rpt_TX_SmuggleTag` («[Smuggling]» → «[Contrabando]») porque toma los
   corchetes por un marcador: es texto visible y está bien traducido.
4. `terminos-rimpacts.py` sobre todo lo traducido.
5. Releer el resultado en orden (decisión 1) y corregir a mano con Edit. **Edit pasa a CRLF los
   renglones interiores de los comentarios de varias líneas**, que en este archivo son LF: después de
   editar, `fin-de-linea.py <original> <editado>` los devuelve, y `revisar.py` tiene que volver a dar
   un solo aviso (el de arriba).

Los nombres que ya quedaron fijos en la etapa 2 se copian de ahí, no se traducen de nuevo. Los
números de renglón no cambian al traducir (cada valor ocupa un renglón), así que las tandas de abajo
siguen valiendo.

### 4. Verificar y probar (dentro de la última sesión)
- Las tres comprobaciones del diff de `AGENTS.md` (la 1 y la 2 no deberían mostrar nada, porque la
  carpeta es nueva).
- Que no quede ningún `TODO` y que los saltos de línea de los comentarios sigan igual que en la
  extracción.
- Regenerar el índice con `LoadFoldersBuilder -build`.
- Angel abre el juego con el mod: la pestaña Diplomacia, firmar un tratado, los ajustes del mod y el
  informe de traducción (para ver lo de las razas opcionales). Si algo sigue en inglés con todo
  traducido, está escrito dentro de la DLL y no se arregla desde RML.
- Commit «RimPacts: traducción completa». **Sin push hasta que Angel diga.**

## Sesiones (estimación)

Cuatro más después de esta: la etapa 2 en una, y una por cada tanda del archivo grande. Cada sesión
deja anotado acá hasta dónde llegó.

## Avance

- 24-oct-2026, sesión 1: plan, glosario y las cuatro decisiones de Angel. Sin traducir todavía: la
  sesión llegó al tope de tamaño al cerrar el glosario. Nada commiteado: la carpeta sigue sin
  versionar, con los 4.498 `TODO` de la extracción.
- 24-oct-2026, sesión 2: etapa 2 completa. Los 13 `DefInjected` (336 entradas) y `Keyed` líneas 4 a
  1699 (840 claves: mercenarios contratados, guerra santa, rivales, unión personal, espionaje,
  compañía comercial y diplomacia por raza). `revisar.py` sin problemas (salvo el aviso conocido),
  `terminos-rimpacts.py` sin avisos reales, XML válido y fines de línea iguales a la extracción.
  Quedan 3.322 `TODO`, todos en `Keyed/4n7njq.xml` desde la línea 1701. Sin commitear.
  - Cambios al glosario: «staunch order» pasó a «orden de vendar» y «staunched» a «herida vendada»
    (la habilidad venda una herida; «resistir» y «firme» no correspondían). Unos 40 términos nuevos
    en la última tabla, y una regla sobre el género de los nombres de facción.
  - Tres nombres de ajustes del mod que ya aparecen en el texto quedaron propuestos y hay que
    respetarlos cuando lleguen los ajustes: «Conquista, auge y caída», «Modo estrategia» y «Exigir
    casus belli». También el botón «Tomar el control».
  - Para Angel, cuando vea el juego: «Compañía Élite de {facción}» es como queda el nombre de una
    compañía contratada (el mod arma «{facción}'s {perfil} company»).

- 24-oct-2026, sesión 3: tanda 2 completa, `Keyed/4n7njq.xml` líneas 1701 a 4016 (1.143 claves:
  ajustes generales, tratados, tributo, pedidos de bienes, comercio y caminos, acciones diplomáticas,
  guerras y alianzas, conquista y mapa de guerra, corte y matrimonios, cumbres, rehenes, edificios,
  guerra de independencia y defensas). Releído entero en orden y corregidas 19 frases. `revisar.py`
  da dos avisos más, los dos correctos: `Rpt_Chat_AutoBreakConfirm` («[Romper]», el nombre del botón)
  y `Rpt_UI_HegemonBadge` («[Hegemón]»); los dos son texto visible entre corchetes. XML válido y
  fines de línea iguales a la extracción. Quedan 2.179 `TODO`, todos desde la línea 4017. Sin commitear.
  - Unos 15 términos nuevos en la última tabla del glosario. Los nombres de los ajustes que ya estaban
    propuestos («Conquista, auge y caída», «Modo estrategia») se usaron tal cual.
  - Los nombres de las tres cumbres quedaron todos en femenino («Cumbre de no agresión mutua» en vez
    de «acuerdo»), porque el mod los pone detrás de «la»: «{2} partió a preparar la {0}».

- 24-oct-2026, sesión 4: tanda 3 completa, `Keyed/4n7njq.xml` líneas 4017 a 6361 (1.154 claves:
  el resto de los ajustes, la corte propia, la oficina de inteligencia y los agentes dobles, guerras
  civiles, asilo, visitas de Estado, los doce sucesos de la corte, crisis de sucesión, gobiernos en el
  exilio, economía mundial, exposiciones, sucesos mundiales, la crónica, renovación automática,
  gobernadores y sucesos de tributarios, las cuatro doctrinas y el estado cliente). Releído entero en
  orden y corregidas 16 frases. `revisar.py` sin avisos, `terminos-rimpacts.py` solo con los dos
  avisos viejos y correctos de la sesión 2, XML válido y fines de línea iguales a la extracción.
  Quedan 1.025 `TODO`, todos desde la línea 6362. Sin commitear.
  - Unos 20 términos nuevos en la última tabla del glosario, entre ellos los nombres oficiales de las
    habilidades y la regla de los decimales (coma, como en la sesión 2).
  - Los nombres de tratados dentro de una frase van como «el tratado «{2}»», igual que en la tanda 2:
    el mod los pone sin artículo y mezclan género («alianza», «pacto», «volverse tributario»).

- 24-oct-2026, sesión 5: tanda 4 completa, `Keyed/4n7njq.xml` líneas 6362 a 8462 (1.025 claves: el
  resto del estado cliente, la guerra mundial entera —bloques, conferencia de paz, cargos, decisiones,
  doctrina de combate y misiones del frente—, edificios del estado cliente y la compañía de mercenarios
  propia). Releído entero en orden y corregidas 10 frases. Ya no queda ningún `TODO` en el mod.
  `revisar.py` sin avisos, `terminos-rimpacts.py` sin avisos reales, XML válido y fines de línea
  iguales a la extracción. Etapa 4: las comprobaciones 2 y 3 del diff salen vacías (la 1 no aplica,
  la carpeta es nueva) y el índice quedó regenerado con `-build`. Unos 20 términos nuevos en el glosario.
  - Los nombres de los bloques van sin artículo, porque el mod los pone detrás de «a» o «de».
  - El juego estaba abierto al regenerar: `RmlVentana.dll` quedó con la versión anterior en `output/`.
    Cerrar el juego y correr `-copia` antes de probar.

## Próxima sesión

Solo falta la prueba en el juego (etapa 4), que hace Angel: la pestaña Diplomacia, firmar un tratado,
los ajustes del mod, la ventana de la guerra mundial, la de la compañía de mercenarios y el informe de
traducción (para ver lo de las razas opcionales). Lo que salga se corrige con `corregir.py`, un commit
por arreglo. Push solo cuando Angel lo diga.
