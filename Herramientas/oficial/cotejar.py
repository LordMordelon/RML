"""Coteja las filas de un glosario (| ingles | español | ...) contra la traduccion oficial.

Uso: python -I cotejar.py <glosario.md>
Muestra las filas cuyo ingles tiene una traduccion oficial exacta (label) que el español de la
fila no contiene.
"""
import os, re, sys, pickle, unicodedata

CACHE = os.path.join(os.path.dirname(os.path.abspath(__file__)), "oficial.pkl")


def norm(s):
    s = unicodedata.normalize("NFD", s.lower())
    return "".join(c for c in s if unicodedata.category(c) != "Mn")


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    if not os.path.exists(CACHE):
        sys.exit("falta oficial.pkl: correr una vez oficial.py (necesita el juego instalado)")
    en, es = pickle.load(open(CACHE, "rb"))
    oficial = {}
    for texto, clave in en:
        if clave.endswith((".label", ".labelShort")) or clave.startswith("Keyed:"):
            t = es.get(clave)
            if t and len(texto) < 60:
                oficial.setdefault(texto.lower(), set()).add(t)
    for n, linea in enumerate(open(sys.argv[1], encoding="utf-8"), 1):
        if not linea.startswith("|") or "---" in linea:
            continue
        celdas = [c.strip() for c in linea.strip().strip("|").split("|")]
        if len(celdas) < 2:
            continue
        ing = re.sub(r"\(.*?\)|\*|`", "", celdas[0])
        esp = norm(re.sub(r"\*|`", "", celdas[1]))
        for termino in re.split(r"\s*/\s*|,\s*", ing):
            termino = termino.strip().lower()
            ofi = oficial.get(termino)
            if not ofi:
                continue
            if not any(norm(o) in esp or esp in norm(o) for o in ofi):
                print(f"{n}: [{termino}] glosario: {celdas[1][:60]!r}  oficial: {sorted(ofi)[:3]}")


main()
