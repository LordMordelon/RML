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

## Próxima sesión

**Objetivo:** subir el extractor y VBE cuando Angel diga, y seguir con los otros 15 mods: reextraer, cinco
comprobaciones, traducir con la tabla (femeninos que quepan en el ancho) y acortar sus títulos que no
caben, un commit por mod. Quedan unos 850 textos; empezar por Persona Mechanoid Pawns 2 (113 trasfondos).
**Al empezar (Claude):** `git -C RML pull` y `git -C RimworldExtractor pull`; reextraer con
`RimworldExtractorLotes` desde la rama `titulos-femeninos` (o `master`, si ya se unió).
