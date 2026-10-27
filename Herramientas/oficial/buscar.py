"""Para cada termino en ingles: traduccion oficial exacta (label/Keyed) y las que ya usa RML (EN exacto).

Uso: python -I buscar.py <RML/Data> "termino" ...
"""
import os, re, sys, pickle
from collections import defaultdict, Counter

CACHE = os.path.join(os.path.dirname(os.path.abspath(__file__)), "oficial.pkl")
PAR = re.compile(r"<!-- EN: ([^\n]*?) -->[ \t]*\r?\n[ \t]*<([A-Za-z0-9_.\-]+)>([^<\n]*)</\2>")
IDX = os.path.join(os.path.dirname(os.path.abspath(__file__)), "rml.pkl")


def mas_nuevo(data):
    return max((os.path.getmtime(os.path.join(r, a)) for r, _, aa in os.walk(data) for a in aa if a.endswith(".xml")),
               default=0)


def indice_rml(data):
    # El cache se rehace si hay traducciones mas nuevas que el: si no, buscaria sobre las del dia en que se armo
    if os.path.exists(IDX) and os.path.getmtime(IDX) >= mas_nuevo(data):
        return pickle.load(open(IDX, "rb"))
    h = defaultdict(Counter)
    for raiz, _, archivos in os.walk(data):
        for a in archivos:
            if a.endswith(".xml") and a != "UNUSED.xml":
                with open(os.path.join(raiz, a), encoding="utf-8", errors="replace") as f:
                    for en, _, val in PAR.findall(f.read()):
                        if val != "TODO":
                            h[en.strip().lower()][val.strip()] += 1
    pickle.dump(h, open(IDX, "wb"))
    return h


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    data, terminos = sys.argv[1], sys.argv[2:]
    if not os.path.exists(CACHE):
        sys.exit("falta oficial.pkl: correr una vez oficial.py (necesita el juego instalado)")
    en, es = pickle.load(open(CACHE, "rb"))
    ofi = defaultdict(set)
    for texto, clave in en:
        if clave.endswith((".label", ".labelShort", ".customLabel")) or clave.startswith("Keyed:"):
            t = es.get(clave)
            if t and len(texto) < 70:
                ofi[texto.lower()].add(t)
    rml = indice_rml(data)
    for q in terminos:
        o = "; ".join(sorted(ofi.get(q.lower(), []))[:3]) or "-"
        r = "; ".join(f"{k} ({v})" for k, v in rml.get(q.lower(), Counter()).most_common(3)) or "-"
        print(f"{q} | oficial: {o} | RML: {r}")


main()
