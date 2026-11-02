# Plan: comillas «…» → "…" en todo RML

Regla del 1-nov-2026 (`GLOSARIO.md`, sección 9): las traducciones usan `"…"`, como la oficial, que no usa
nunca «…»; dentro de una frase ya entrecomillada, comillas simples `'…'`. El título de algo que llega en una
carta va con `(*Name)…(/Name)`. Mini Expeditions ya está hecho. Angel pidió pasar el resto el 1-nov-2026.

## Lo que queda

**234 renglones en 32 archivos de 21 mods** (sin contar los `UNUSED.xml` ni los comentarios `EN:`). Cinders
quedó hecho el 1-nov-2026, junto con sus títulos femeninos. Los que
más tienen: DMS Expedition (4 archivos), DMS Taktikmagier (3); RimPacts, Combat Extended,
Apex Mechanoids, ReGrowth 2, DMS II y Ascension (2). Para verlos:

```sh
grep -rn "«" Data --include=*.xml | grep -v UNUSED | grep -v "<!--"
```

## Cómo

- **A mano, texto por texto**, con `traducir.py corregir` (clave, viejo, nuevo). No un reemplazo global:
  hay que mirar cada uno, porque algunas «…» van dentro de una frase que ya tiene comillas (pasan a `'…'`),
  y otras pueden ser nombres de cosas en una carta (pasan a `(*Name)…(/Name)`, como en Mini Expeditions).
- Mirar el `EN:` de cada texto: si el inglés usa `"…"`, va igual; si no tiene comillas, ver si sobran.
- `traducir.py comprobar` antes del commit. Las comprobaciones 1 y 5 muestran a propósito los textos
  cambiados.
- Un solo commit para todo, porque es el mismo cambio en todos los mods (regla de `AGENTS.md`: lo que cruza
  varios mods va en un commit que nombra el cambio), con un renglón por mod en el cuerpo.
- Cabe en una sesión. Al terminar, borrar este plan.
