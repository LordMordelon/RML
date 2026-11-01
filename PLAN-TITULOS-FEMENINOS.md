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
- Pendiente aparte: varios títulos de VBE vienen abreviados de antes («activista D.C.», «alborot. de
  mundourbano», «médico militar en form.»). Los femeninos siguen al masculino; si se cambian, van juntos
  (ver «Nombres cortos sin abreviar»).

## Próxima sesión

**Objetivo:** si Angel vio los títulos femeninos en el juego (una colona con un trasfondo de VBE, por
ejemplo con Pawn Editor), subir el extractor y VBE, y seguir con los otros 15 mods: reextraer, cinco
comprobaciones, traducir con la tabla, un commit por mod. Quedan unos 850 textos; empezar por Persona
Mechanoid Pawns 2 (113 trasfondos).
**Al empezar (Claude):** `git -C RML pull` y `git -C RimworldExtractor pull`; reextraer con
`RimworldExtractorLotes` desde la rama `titulos-femeninos` (o `master`, si ya se unió).
