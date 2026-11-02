# Plan: títulos femeninos de los trasfondos

Pedido de Angel del 1-nov-2026: que a una colona el título de su trasfondo le salga en femenino. «Lo ideal
es que sí»: que el extractor los saque para todos los mods, no solo a mano en Vanilla Backstories Expanded.

## Lo que pasa hoy

- Un trasfondo (`BackstoryDef`) tiene `title` y `titleShort`, y puede tener `titleFemale` y
  `titleShortFemale`. Casi ningún mod trae los femeninos en inglés, porque en inglés el título casi nunca
  cambia. El extractor saca solo los campos que el XML del mod trae, así que RML no los tiene.
- El juego hace lo mismo en inglés (5 `titleFemale` en 784 trasfondos), pero la traducción oficial le
  agrega `titleFemale` a 770: el juego acepta inyectar ese campo aunque el inglés no lo traiga.
- En RML hay unos **636 trasfondos en 16 mods** y solo 37 tienen los títulos femeninos (Android, porque
  el mod los trae, y unos pocos de AobaKuma). Los que más tienen: Vanilla Backstories Expanded (174),
  Persona Mechanoid Pawns 2 (113), Cinders (54), VRE Archon y VQE Deadlife (52 cada uno).

## Cómo se haría

1. **Extractor** (repositorio `RimworldExtractor`): al extraer un `BackstoryDef` que tiene `title` y no
   `titleFemale`, sumar una entrada `titleFemale` con el mismo inglés del `title`; igual con
   `titleShort` → `titleShortFemale`. Con prueba nueva primero (que falle sin el cambio), como pide
   `AGENTS.md`. Publica una versión del extractor al subir.
   - El inglés del comentario `EN:` es el del título, así que quien traduce ve qué tiene que poner en
     femenino.
   - Un `TODO` no se ve en el juego: mientras falte, sale el título masculino que ya tiene, como hoy. No
     empeora nada.
2. **RML**: reextraer los 16 mods con el extractor nuevo (`RimworldExtractorLotes`), mirar el diff (las
   cinco comprobaciones) y commitear la estructura, un commit por mod.
3. **Traducir** los femeninos a mano con la tabla (`traducir.py tabla` / `aplicar`), mod por mod: unos
   1270 textos, pero muchos son iguales al masculino («piloto», «prototipo», «chef»). Un commit por mod.
   Empezar por Vanilla Backstories Expanded.

## Decisiones de Angel

- [x] Que lo haga el extractor para todos los mods (1-nov-2026).
- [x] **Plan aprobado por Angel el 1-nov-2026.** Es grande: toca el extractor, publica una versión y suma unos 1270 `TODO` en
  16 mods. Antes de cambiar el extractor, probar en el juego que un `titleFemale` agregado a mano en un
  trasfondo que no lo trae se ve (por ejemplo, uno de Vanilla Backstories Expanded).

## Hecho (1-nov-2026)

- **Extractor**, rama local `titulos-femeninos` (sin subir): `Extractor.TitulosFemeninos` suma `titleFemale` y
  `titleShortFemale` a cada trasfondo que no los trae, con el inglés del masculino. Pruebas en
  `TituloFemeninoTests` (fallaban sin el cambio); la tanda entera en verde.
- **Vanilla Backstories Expanded**, reextraído y completo: su `UNUSED.xml` tenía **298 títulos femeninos
  ya traducidos** que el extractor viejo había apartado. Volvieron todos a su lugar (comprobado uno por
  uno) y el `UNUSED.xml` quedó vacío y se borró. Los 48 que faltaban (24 trasfondos nuevos) se tradujeron
  con la tabla. Ningún otro mod tiene títulos femeninos en su `UNUSED.xml`.
- Angel lo probó en el juego: el femenino se ve («Esclava artificial»). Pero **la ficha corta el título** a
  un ancho fijo. Medido con sus capturas, la fuente es Arial y cabe lo que mide hasta ~1080 unidades a
  tamaño 100 (`ImageFont.truetype("arial.ttf", 100).getlength(titulo)`, con la primera letra en
  mayúscula; «Emprendedor de mundo» cabe justo). Se usa 1050 para dejar margen. Se mide en ancho y no
  en letras porque «m» ocupa el triple que «i».
- VBE quedó con los 174 dentro del ancho (f9674fdb): 42 trasfondos reescritos completos y sin abreviar
  («cazarrecompensas», «criptozoólogo»), los términos del glosario (`mundomedio`, `quimbustible`) y
  femeninos mal formados («comercianta»). «Programador de mundomedio» no cabe de ninguna forma y quedó
  «programador».
- En los otros 15 mods hay **74 títulos que no caben**, que se arreglan junto con sus femeninos: Persona
  Mechanoid Pawns 2 (14), VQE Deadlife (12), Ascension Megacorp y Cinders (9), DMS II (8), Archon (5),
  DMS 1.6, VQE Ancients (4), The Generator, Android (3), Taktikmagier, Altered Carbon y Magic (1).
- **Descripciones con género fijo.** Una colona con «Como inspector de seguridad, Felicia…» (captura de
  Angel). The Generator quedó hecho (f1287c81). Una búsqueda de «De niño», «experto», «conocido»,
  «Nacido», «como un»… sin `{PAWN_gender}` da **62 descripciones** para mirar una por una (algunas no
  son del colono): VBE 19, Persona Mechanoid Pawns 2 (11), VQE Ancients (8), Archon (6), DMS y DMS II (4
  cada uno), Deadlife (3), Altered Carbon, Android y Magic (2), Cryptoforge (1). Se arreglan junto con
  cada mod, con `{PAWN_gender ? o : a}`.
- La oficial tiene el mismo problema: 104 títulos de Core, Royalty y Anomaly se cortan («emprendedor de
  mundourbano»). No es de RML: va como corrección a Ludeon, si Angel quiere.

- **Persona Mechanoid Pawns 2** terminado (3501c342, sin subir): los 226 femeninos, los 14 títulos que se cortaban
  («Voidbore-Tech Mech» quedó «Meca de perforación») y 108 descripciones con género. «Meca cazadora-asesina» mide 1065:
  pasa el margen de 1050 pero no el máximo medido (1083, «Emprendedor de mundo»). Falta que Angel lo mire en el juego.
  Lo que sirvió: leer todas las descripciones de corrido y escribir una lista `clave | viejo | nuevo` a mano, que un
  script aplica comprobando que cada «viejo» aparezca una sola vez en su descripción. Ojo con los participios: la
  marca de género reemplaza solo la «o» final («gestad{PAWN_gender ? o : a}», no «gesta…»).

- Persona Mechanoid Pawns 2 subido el 1-nov-2026; Angel todavía no lo mira en el juego.
- **Cinders of the Embergarden** terminado (fdd41123, eb7fbfeb, 8067befc; sin subir): los 108 femeninos, los 9 títulos
  que se cortaban (anotados en su glosario), 7 descripciones con género y 3 apodos que estaban en femenino también para
  los colonos («muñeco viviente», «niño ídolo», «sirviente»). «Una soldado», como la oficial («soldado desertora»).
- **Tono de la oficial** (pedido de Angel, 1-nov-2026): al revisar las descripciones, además del género, párrafos
  separados por una línea en blanco (`\n\n`) y nada de frases coloquiales («a lo bruto», «a lo mejor», «de lo más…»).
  Y en pasado, como la oficial, aunque el inglés esté en presente (Angel, 1-nov-2026: «En su largo sueño, Valeria
  solía repartir regalos…»). Se queda en presente lo que sigue siendo cierto o es general («que le lleva regalos a
  todo el mundo», de Santa Claus), y el trasfondo «colono» («su historia todavía está por contarse»).

## Próxima sesión (1-nov-2026)

**Objetivo:** VQE Deadlife: títulos femeninos, 12 títulos que se cortan, descripciones con género y con el tono de la
oficial.
**Estado:** VBE, The Generator, Persona Mechanoid Pawns 2 y Cinders terminados; los tres commits de Cinders y este plan,
sin subir. El pedido a la oficial (https://github.com/Ludeon/RimWorld-SpanishLatin/pull/195) sigue abierto.
**Al empezar (Claude):**
1. `git -C RML pull`. Preguntarle a Angel si subir Cinders y si miró en el juego Cinders y Persona Mechanoid Pawns 2.
   Leer de este plan solo «Hecho» y esta sección.
2. Reextraer Deadlife con `RimworldExtractorLotes` (desde `RimworldExtractor/`, con el comienzo de su carpeta), mirar
   el diff con `traducir.py comprobar` y commitear la estructura.
3. Igual que en Cinders: femeninos con `traducir.py tabla` / `aplicar`, los títulos que se cortan (medir los dos
   géneros con Arial, límite 1050), las descripciones leídas una por una contra el inglés (género y tono) y una lista
   `clave | viejo | nuevo` que un script aplica comprobando que cada «viejo» aparezca una vez.
4. Cinco comprobaciones (la 1 y la 5 muestran lo cambiado a propósito: contarlo), un commit del mod, `-copia` y pedirle
   a Angel que lo mire en el juego.
**Decisiones de Angel:**
- The Dead Man's Switch 1.6 (29 trasfondos) quedó congelado con la II: no se toca (recomendación; Angel no la cambió).
- El orden: Deadlife, Archon, DMS II, Ancients, Ascension y los chicos. Quedan también las 19 descripciones de VBE.
- Las descripciones van en pasado, como la oficial, aunque el inglés esté en presente (1-nov-2026).
**Guías:** ninguna en especial; las reglas están en `RML/AGENTS.md` («Editar XML de traducción»).
**Dónde leer:** este plan; el commit de Persona Mechanoid Pawns 2 (3501c342) como ejemplo.
