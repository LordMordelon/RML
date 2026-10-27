"""Busca la traduccion oficial (SpanishLatin) de textos en ingles del juego.

Uso: python -I oficial.py "texto1" "texto2" ...   (coincidencia exacta, sin mayusculas)
     python -I oficial.py --contiene "texto"      (subcadena, hasta 15 resultados)
"""
import os, sys, re, pickle
import xml.etree.ElementTree as ET

G = r"D:\SteamLibrary\steamapps\common\RimWorld\Data"
DLC = ["Core", "Royalty", "Ideology", "Biotech", "Anomaly", "Odyssey"]
LANG = "SpanishLatin (Español(Latinoamérica))"
CACHE = os.path.join(os.path.dirname(os.path.abspath(__file__)), "oficial.pkl")
CAMPOS = ("label", "labelShort", "description", "jobString", "labelNoun", "customLabel",
          "title", "titleShort", "labelMale", "labelFemale", "gerund", "verb", "reportString")


def parse(path):
    try:
        with open(path, encoding="utf-8", errors="replace") as f:
            return ET.fromstring(f.read().lstrip("\ufeff"))
    except Exception:
        return None


def construir():
    en = []  # (texto, clave)
    es = {}
    for d in DLC:
        for raiz, _, archivos in os.walk(os.path.join(G, d, "Defs")):
            for a in archivos:
                if not a.endswith(".xml"):
                    continue
                r = parse(os.path.join(raiz, a))
                if r is None:
                    continue
                for de in r:
                    dn = de.findtext("defName")
                    if not dn:
                        continue
                    # Solo los campos directos del def (y los customLabel del cuerpo): con de.iter() la etiqueta
                    # de un ataque («barrel») quedaba con la clave del arma y «barrel» salia como «LMG»
                    for el in list(de) + [x for x in de.iter() if x.tag == "customLabel"]:
                        if el.tag in CAMPOS and el.text and el.text.strip():
                            # ruta relativa al def, solo los campos directos y los customLabel del cuerpo
                            en.append((el.text.strip(), f"{de.tag}:{dn}.{el.tag}"))
        eng = os.path.join(G, d, "Languages", "English", "Keyed")
        for raiz, _, archivos in os.walk(eng):
            for a in archivos:
                r = parse(os.path.join(raiz, a))
                if r is None:
                    continue
                for el in r:
                    if el.text:
                        en.append((el.text.strip(), f"Keyed:{el.tag}"))
        base = os.path.join(G, d, "Languages", LANG)
        for raiz, _, archivos in os.walk(base):
            for a in archivos:
                if not a.endswith(".xml"):
                    continue
                r = parse(os.path.join(raiz, a))
                if r is None:
                    continue
                tipo = os.path.basename(raiz)
                for el in r:
                    if el.text is None:
                        continue
                    if "Keyed" in raiz:
                        es[f"Keyed:{el.tag}"] = el.text.strip()
                    else:
                        dn, _, campo = el.tag.partition(".")
                        es[f"{tipo}:{dn}.{campo}"] = el.text.strip()
    with open(CACHE, "wb") as f:
        pickle.dump((en, es), f)
    return en, es


def cargar():
    if os.path.exists(CACHE):
        with open(CACHE, "rb") as f:
            return pickle.load(f)
    return construir()


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    en, es = cargar()
    args = sys.argv[1:]
    contiene = False
    if args and args[0] == "--contiene":
        contiene, args = True, args[1:]
    for q in args:
        ql = q.lower()
        vistos, n = set(), 0
        for texto, clave in en:
            t = texto.lower()
            if (ql in t) if contiene else (t == ql):
                trad = es.get(clave)
                if trad is None or (texto, trad) in vistos:
                    continue
                vistos.add((texto, trad))
                print(f"[{q}] {texto[:70]!r} -> {trad[:110]!r}  ({clave})")
                n += 1
                if n >= (15 if contiene else 4):
                    break
        if n == 0:
            print(f"[{q}] (sin oficial)")


main()
