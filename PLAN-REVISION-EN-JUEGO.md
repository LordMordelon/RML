# Plan: lo que se vio en el juego

El 27-oct-2026 Angel probó RML en el juego y mandó capturas con textos en inglés o dudosos. Este plan
junta lo que salió, lo que ya se arregló y lo que falta. Va aparte de [PLAN-REVISION-GENERAL.md](PLAN-REVISION-GENERAL.md),
que revisa los glosarios y los términos sin abrir el juego.

## 1. Nombres de mods en la lista de opciones de mods

**Regla (Angel, 27-oct):** el nombre de un mod va en inglés o en su idioma original, también en esa lista. Ya está
en la sección 9 del [GLOSARIO.md](GLOSARIO.md).

- **Hecho:** se leyó el código (`SettingsCategory`) de los mods de RML. 13 nombres salían de una clave de `Keyed`
  que RML había traducido. 10 volvieron al inglés, y los 3 de RimPsyche y Graphics Setter («Configurador gráfico», que también es su botón del menú principal) volvieron al
  español a pedido de Angel: Cinders (2), Work Modes, Void Universe, Manipulator Beam Emitter, Ancient Hydroponics y VE Framework - Outposts. Los tres nombres cortos de Vanilla
  Expanded («VE - Psycasts», «VE - Aspirations», «VE - Helixien Gas») ya estaban en inglés y se quedan.
- **Hospitality se queda «Hospitalidad»** (Angel, 27-oct): su clave se usa en 7 lugares del código, no solo como
  nombre, y un nombre que se usa en otras partes se deja en español.
- **Falta:** mirar en el juego que la lista salga entera en inglés. Los nombres escritos en el código de cada mod
  no pasan por RML.

## 2. Textos que el extractor no saca

Un texto que sigue en inglés en el juego y no está en `Data/` casi siempre es una etiqueta que el extractor no
conoce. El arreglo va en el extractor (`Prefabs.EtiquetasAgregadas`), con su prueba, y después se reextrae el mod.
Antes de agregar una etiqueta se mide en los mods de RML que siempre sea texto (regla de `RimworldExtractor/AGENTS.md`).

| Etiqueta | Dónde se vio | Mods de RML que la usan | Estado |
|---|---|---|---|
| `defaultLabel` / `defaultDesc` | Cinders: botón «Change to walking mode» de las torretas | Solo Cinders: 24 usos, todos texto | **Hecho el 27-oct** (extractor 6789080, Cinders 03c7fc8b): «Cambiar a modo caminante» |
| `namerLabels` (lista) | Cinders: títulos de arte «Greatsword twisted», «Dragon slayer del temor» | 6 mods | Lo que sigue |

**`namerLabels`:** es la lista de palabras con que el juego inventa el título de un arma («Dragon slayer»,
«longbow», «Great Bow»). La oficial la traduce (Odyssey: `Bow_Great_Unique.comps.CompUniqueWeapon.namerLabels.0` =
«arco grande»). Hay que ver si el juego la lee también fuera de `CompUniqueWeapon`, como la tiene Cinders, y de
dónde sale «twisted». **Los títulos ya generados quedan guardados en la partida:** las armas nuevas saldrán en
español y las viejas no.

## 3. Mayúscula inicial

«balas de uranio 14/15»: las etiquetas van en minúscula, como en la oficial. El juego pone la mayúscula donde
corresponde, y ponerla en la traducción la deja también en medio de una frase («cargar Balas de uranio»).

- **Falta:** ver qué mod muestra ese contador. Si es el código del mod el que no pone la mayúscula, se anota como
  algo del mod y la traducción no se toca.

## 4. Cómo seguir

Angel aprobó los puntos 1 y 2 el 27-oct-2026; el 3 quedó resuelto arriba.

1. ~~`defaultLabel` y `defaultDesc`~~: hecho el 27-oct.
2. Medir `namerLabels` en los 6 mods; si sirve, agregarla igual y traducir las listas, mod por mod.
3. ~~Hospitality~~: resuelto, se queda en español.
4. Probar en el juego con una partida nueva o un arma nueva, y mandar capturas de lo que siga en inglés. Cada
   caso nuevo se anota aquí con dónde se vio.

Cada mod con su commit, y las tres comprobaciones del diff antes de subir.
