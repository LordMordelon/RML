# Traducir un mod de RML con tabla. La traduccion se escribe a mano en una tabla clave<TAB>texto; este
# comando la copia solo sobre los TODO y revisa. No traduce nada (PLAN-HERRAMIENTAS.md).
#
# Uso (desde cualquier carpeta; <mod> es el comienzo del nombre de su carpeta o su numero del Workshop):
#   python -I Herramientas/traducir.py estado [mod]
#   python -I Herramientas/traducir.py tabla <mod> [--archivo texto] [--palabras N] [--claves] [--salida tabla.tsv]
#   python -I Herramientas/traducir.py aplicar <mod> <tabla.tsv> [--forzar] [--claves] [--glosario ruta.md]...
#   python -I Herramientas/traducir.py revisar <mod> [--contra HEAD|carpeta]
#   python -I Herramientas/traducir.py terminos <mod> [--glosario ruta.md]...
#   python -I Herramientas/traducir.py comprobar
#   python -I Herramientas/traducir.py prueba <mod> [--desde commit]
#   python -I Herramientas/traducir.py lineas <copia.xml> <archivo.xml>
#   python -I Herramientas/traducir.py corregir <mod> <cambios.tsv>
import argparse, collections, glob, os, re, shutil, subprocess, sys, tempfile, time, unicodedata
import xml.etree.ElementTree as ET

RAIZ = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
BOM = b"\xef\xbb\xbf"

RE_VALOR = re.compile(r"^(\s*)<([^>\s/!?]+)>(.*)</\2>(\s*)$", re.S)
RE_LI = re.compile(r"^(\s*)<li>(.*)</li>(\s*)$", re.S)
RE_ABRE = re.compile(r"^\s*<([^>\s/!?]+)>\s*$")
RE_XPATH = re.compile(r"<xpath>(.*?)</xpath>")
# Lo mismo que revisaba revisar.py: los marcadores que el juego reemplaza y que la traduccion tiene que conservar
MARCADORES = re.compile(r"\{[^}]*\}|\[[A-Za-z_][A-Za-z0-9_]*\]|&lt;/?[a-z]+[^&]*&gt;|\\n|%[sd]")


class Error(Exception):
    pass


Entrada = collections.namedtuple("Entrada", "clave i en valor lista")


def ruta(p):
    # Python de Windows no entiende las rutas de Git Bash (/c/Users/...)
    if os.name == "nt" and re.match(r"^/[a-zA-Z]/", p):
        return p[1].upper() + ":\\" + p[3:].replace("/", "\\")
    return p


def es_patch(rel):
    return rel.startswith("Patches/") or "/Patches/" in rel


def _comentario(bloque):
    texto = "".join(bloque).replace("\r\n", "\n").strip()
    if not texto.startswith("<!-- EN:"):
        return None
    texto = texto[len("<!-- EN:"):]
    if texto.startswith(" "):
        texto = texto[1:]
    texto = texto[:-3] if texto.endswith("-->") else texto
    return texto[:-1] if texto.endswith(" ") else texto


def leer_entradas(lineas, en_patches):
    """Las entradas traducibles de un archivo: (clave, renglon, ingles, valor, lista).

    En DefInjected y Keyed la clave es la etiqueta; un elemento de lista es «Lista[0]», emparejado con los
    comentarios EN que estan todos juntos arriba de la lista, solo si hay uno por elemento. En Patches la
    clave es «xpath|campo» o «xpath|li[0]», y el EN es el comentario justo arriba. Un renglon sin
    comentario EN arriba (los <mods><li> de los Patches) no es una entrada.
    Devuelve tambien las listas con TODO que no se pudieron emparejar, para hacerlas a mano."""
    ent, sueltas = [], []
    pend, xpath, n_li = [], None, 0
    i = 0
    while i < len(lineas):
        l = lineas[i]
        if l.strip().startswith("<!--"):
            j = i
            while not lineas[j].rstrip().endswith("-->") and j + 1 < len(lineas):
                j += 1
            en = _comentario(lineas[i:j + 1])
            pend = pend + [en] if en is not None else []
            i = j + 1
            continue
        m = RE_XPATH.search(l)
        if m:
            xpath, n_li = m.group(1), 0
        if en_patches:
            mv, ml = RE_VALOR.match(l), RE_LI.match(l)
            if len(pend) == 1 and xpath:
                if ml:
                    ent.append(Entrada(f"{xpath}|li[{n_li}]", i, pend[0], ml.group(2), xpath))
                    n_li += 1
                elif mv and mv.group(2) not in ("xpath", "success"):
                    ent.append(Entrada(f"{xpath}|{mv.group(2)}", i, pend[0], mv.group(3), None))
            pend = []
            i += 1
            continue
        ma = RE_ABRE.match(l)
        if ma and i + 1 < len(lineas) and lineas[i + 1].lstrip().startswith("<li"):
            nombre, k, lis = ma.group(1), i + 1, []
            while k < len(lineas) and lineas[k].lstrip().startswith("<li"):
                lis.append(k)
                k += 1
            valores = [RE_LI.match(lineas[x]) for x in lis]
            con_todo = [bool(v and v.group(2) == "TODO") for v in valores]
            # En una lista ya traducida el orden del español puede no seguir al de los comentarios (pasa en
            # RML): un TODO entre elementos traducidos no se empareja por posicion, va a mano
            mezclada = any(con_todo) and not all(con_todo)
            if len(pend) == len(lis) and all(valores):
                for n, (x, v) in enumerate(zip(lis, valores)):
                    if not (mezclada and con_todo[n]):
                        ent.append(Entrada(f"{nombre}[{n}]", x, pend[n], v.group(2), nombre))
            if any(con_todo) and (mezclada or len(pend) != len(lis) or not all(valores)):
                sueltas.append(nombre)
            pend = []
            i = k
            continue
        mv = RE_VALOR.match(l)
        if mv and mv.group(2) != "li":
            ent.append(Entrada(mv.group(2), i, pend[-1] if pend else None, mv.group(3), None))
        pend = []
        i += 1
    return ent, sueltas


def _lineas(b):
    return b.decode("utf-8-sig").splitlines(keepends=True)


def _bytes(lineas, bom):
    return (BOM if bom else b"") + "".join(lineas).encode("utf-8")


def _saltos(en):
    return en.count("\n") + en.count("\\n")


Problema = collections.namedtuple("Problema", "grave texto")
LLAVE = re.compile(r"\{[^}]*\}")
ETIQUETA = re.compile(r"&lt;(/?)([a-z]+)[^&]*?&gt;")


def _problemas_de(e, es, n):
    """Lo que una traduccion tiene que conservar del ingles. Grave es lo que se ve roto en el juego: una
    {llave} que el ingles no tiene, una etiqueta de color sin cerrar, una regla sin su «simbolo->». Lo
    demas solo avisa: medido sobre las 105.507 traducciones de RML (26-oct-2026), el español saca con
    razon [PAWN_pronoun] y {X_pronoun} y traduce palabras entre corchetes; frenar por eso obligaba a
    forzar casi cada tanda."""
    if e.en is None:
        return []
    mal = []
    donde = f"renglon {n} {e.clave}"
    inventadas = [x for x in LLAVE.findall(es) if "?" not in x and x not in LLAVE.findall(e.en)]
    if inventadas:
        mal.append(Problema(True, f"{donde}: {inventadas[0]} no esta en el ingles"))
    pila = []
    for cierre, tag in ETIQUETA.findall(es):
        if not cierre:
            pila.append(tag)
        elif pila and pila[-1] == tag:
            pila.pop()
        else:
            pila = [None]
            break
    if pila:
        mal.append(Problema(True, f"{donde}: etiqueta de color sin cerrar o mal cerrada"))
    if "-&gt;" in e.en:  # regla de gramatica: el simbolo de la izquierda no se traduce
        simbolo = e.en.split("-&gt;", 1)[0]
        if "-&gt;" not in es or es.split("-&gt;", 1)[0] != simbolo:
            mal.append(Problema(True, f"{donde}: la regla tiene que empezar con «{simbolo}-&gt;»"))
    lista = lambda t: sorted(x for x in MARCADORES.findall(t) if x != "\\n")
    if lista(e.en) != lista(es):
        mal.append(Problema(False, f"{donde}: marcadores distintos {lista(e.en)} vs {lista(es)}"))
    if es != es.strip() and e.en == e.en.strip():  # la tabla ya no recorta: un espacio de mas se veria
        mal.append(Problema(False, f"{donde}: espacios al principio o al final que el ingles no tiene"))
    if _saltos(e.en) != es.count("\\n"):
        mal.append(Problema(False, f"{donde}: saltos de linea {_saltos(e.en)} en ingles, {es.count(chr(92) + 'n')} en espanol"))
    return mal


def revisar(orig, nuevo, en_patches=False):
    """Que fuera de los TODO no haya cambiado ni un byte y que cada traduccion conserve lo del ingles."""
    a, b = _lineas(orig), _lineas(nuevo)
    if len(a) != len(b):
        return [Problema(True, f"cambio el numero de renglones: {len(a)} -> {len(b)}")]
    todo = {e.i: e for e in leer_entradas(a, en_patches)[0] if e.valor == "TODO"}
    mal = []
    for i, (x, y) in enumerate(zip(a, b)):
        if x == y:
            continue
        e = todo.get(i)
        rx = RE_LI if e and e.clave.endswith("]") else RE_VALOR
        mx, my = rx.match(x), rx.match(y)
        if not (e and mx and my and mx.group(1) == my.group(1) and mx.groups()[-1] == my.groups()[-1]
                and (rx is RE_LI or mx.group(2) == my.group(2))):
            mal.append(Problema(True, f"renglon {i + 1}: cambio algo que no era un TODO"))
            continue
        mal += _problemas_de(e, my.groups()[-2], i + 1)
    return mal


class Informe:
    def __init__(self):
        self.aplicadas = collections.Counter()
        self.no_encontradas, self.avisos, self.tocadas = [], [], []
        self.vacias = 0


def _leer_tabla(tabla, archivos):
    filas, vistas, actual, vacias = [], set(), None, 0
    for n, l in enumerate(tabla.split("\n"), 1):
        l = l.rstrip("\r")
        if l.startswith("## archivo:"):
            actual = l[len("## archivo:"):].strip().replace("\\", "/")
            if actual not in archivos:
                raise Error(f"tabla renglon {n}: el mod no tiene el archivo {actual}")
            continue
        if not l.strip() or l.startswith("#"):
            continue
        if "\t" not in l:
            raise Error(f"tabla renglon {n}: falta el TAB")
        k, v = l.split("\t", 1)
        if not v.strip():
            vacias += 1
            continue
        if v.strip() == "TODO":
            raise Error(f"tabla renglon {n}: {k} dice TODO")
        if "<" in v or ">" in v or re.search(r"&(?!(lt|gt|amp|apos|quot|#\d+);)", v):
            raise Error(f"tabla renglon {n}: < > o & sin escapar en {k}")
        if (actual, k) in vistas:
            raise Error(f"tabla renglon {n}: clave repetida {k}")
        vistas.add((actual, k))
        filas.append((actual, k, v, n))
    return filas, vacias


def aplicar_tabla(tabla, archivos, claves_keyed, forzar=False, claves=False):
    """Copia la tabla sobre los TODO de los archivos (ruta relativa -> bytes). Devuelve solo los que
    cambian, sin escribir nada: si algo no esta bien, sale con Error y no cambia ningun archivo."""
    inf = Informe()
    filas, inf.vacias = _leer_tabla(tabla, archivos)
    lineas, todo, por_clave = {}, {}, collections.defaultdict(list)
    for rel, b in archivos.items():
        lineas[rel] = _lineas(b)
        for e in leer_entradas(lineas[rel], es_patch(rel))[0]:
            if e.valor == "TODO":
                todo[(rel, e.clave)] = e
                por_clave[e.clave].append(rel)
    cambiados, listas = set(), set()
    for rel, k, v, n in filas:
        if rel is None:
            donde = por_clave.get(k, [])
            if len(donde) > 1:
                raise Error(f"tabla renglon {n}: {k} es TODO en {len(donde)} archivos ({', '.join(donde)}); "
                            "poner «## archivo: <ruta>» arriba")
            rel = donde[0] if donde else None
        e = todo.get((rel, k))
        if e is None:
            inf.no_encontradas.append(k)
            continue
        if e.en is not None and e.en.strip() in claves_keyed and not claves:
            raise Error(f"tabla renglon {n}: el ingles de {k} es una clave de Keyed ({e.en.strip()}); "
                        "se deja en TODO (o --claves si de verdad es texto)")
        l = lineas[rel][e.i]
        m = (RE_LI if e.clave.endswith("]") else RE_VALOR).match(l)
        if m is None:
            raise Error(f"renglon {e.i + 1} de {rel}: no se reconoce")
        if e.clave.endswith("]"):
            lineas[rel][e.i] = f"{m.group(1)}<li>{v}</li>{m.group(3)}"
        else:
            lineas[rel][e.i] = f"{m.group(1)}<{m.group(2)}>{v}</{m.group(2)}>{m.group(4)}"
        cambiados.add(rel)
        inf.aplicadas[rel] += 1
        inf.tocadas.append(e._replace(valor=v))
        if e.lista:
            listas.add((rel, e.lista))
    for rel, lista in sorted(listas):
        faltan = [e.clave for (r, _), e in todo.items()
                  if r == rel and e.lista == lista and lineas[rel][e.i] == _lineas(archivos[rel])[e.i]]
        if faltan:
            raise Error(f"{rel}: la lista {lista} queda a medias (faltan {', '.join(faltan[:3])}"
                        f"{'…' if len(faltan) > 3 else ''}); una lista va entera")
    nuevos = {}
    for rel in sorted(cambiados):
        b = _bytes(lineas[rel], archivos[rel].startswith(BOM))
        try:
            ET.fromstring(b)
        except ET.ParseError as ex:
            raise Error(f"{rel}: el XML no queda valido ({ex})")
        mal = revisar(archivos[rel], b, es_patch(rel))
        graves = [p.texto for p in mal if p.grave]
        if graves and not forzar:
            raise Error(f"{rel}: {len(graves)} problema(s) grave(s), no se guardo nada (--forzar si de verdad "
                        "estan bien):\n  " + "\n  ".join(graves))
        inf.avisos += [f"{rel}: {'GRAVE (forzado) ' if p.grave else ''}{p.texto}" for p in mal]
        nuevos[rel] = b
    return nuevos, inf


def _palabras(t):
    return len(re.findall(r"[A-Za-z]+", t or ""))


def plantilla(archivos, claves_keyed, sugerencias=None, palabras=None, claves=False):
    """La tabla para llenar: un encabezado por archivo, el ingles arriba de cada clave y la traduccion vacia.
    Con «palabras», corta antes de pasarse (una lista va siempre entera): la tanda siguiente sale de
    volver a pedirla despues de aplicar esta."""
    out = ["# Tabla para llenar a mano: clave<TAB>traduccion. Solo se aplica sobre TODO; un renglon vacio se salta.",
           "# Los saltos de linea se escriben \\n. Los comentarios (#) no se aplican.", ""]
    total, lleno = 0, False
    for rel in sorted(archivos):
        ent, sueltas = leer_entradas(_lineas(archivos[rel]), es_patch(rel))
        bloque = [f"# LISTA SIN EMPAREJAR (a mano): {s}" for s in sueltas]
        grupos = collections.OrderedDict()
        for e in ent:
            if e.valor == "TODO":
                grupos.setdefault(e.lista or e.clave, []).append(e)
        for grupo in grupos.values():
            n = sum(_palabras(e.en) for e in grupo)
            if palabras and total and total + n > palabras:
                lleno = True
                break
            for e in grupo:
                if e.en is not None and e.en.strip() in claves_keyed and not claves:
                    bloque.append(f"# CLAVE DE KEYED, se deja en TODO: {e.clave} ({e.en.strip()})")
                    continue
                bloque.append("# EN: " + (e.en or "").replace("\n", "\\n"))
                sug = (sugerencias or {}).get((e.en or "").strip().lower())
                if sug:
                    mas = sorted(sug.items(), key=lambda x: -x[1])[:2]
                    bloque.append("# RML: " + "; ".join(f"{t} ({c})" for t, c in mas))
                bloque.append(f"{e.clave}\t")
            total += n
        if bloque:
            out += [f"## archivo: {rel}"] + bloque + [""]
        if lleno:
            out.append("# (tope de palabras: el resto, en la tanda siguiente)")
            break
    out.append(f"# {total} palabras de ingles")
    return "\n".join(out) + "\n"


def norm(s):
    s = unicodedata.normalize("NFD", s.lower())
    return "".join(c for c in s if unicodedata.category(c) != "Mn")


Termino = collections.namedtuple("Termino", "en es re_en re_es")


def _raiz(w):
    # Para que «misil» acepte «misiles» y «merodeadora», «merodeadoras»: sin la s y sin la vocal final
    if len(w) > 4 and w.endswith("s"):
        w = w[:-1]
    if len(w) > 3 and w[-1] in "aeo":
        w = w[:-1]
    return w


def _re_es(es):
    raices = [_raiz(w) for w in re.findall(r"\w+", norm(es))]
    if not raices:
        return None
    return re.compile(r"\b" + raices[0] + "".join(r"\w*\W+(?:\w+\W+){0,2}?" + r for r in raices[1:]))


def _completar_en(alts):
    """Los glosarios abrevian: «large / medium / small fragments» es «large fragments», «medium fragments»…
    Una alternativa mas corta que no tiene el sustantivo de la ultima se completa con el resto de esa.
    Devuelve tambien cuales se completaron: solo esas se completan del lado del español."""
    completa = alts[-1].split() if alts else []
    nucleo = re.split(r"[\s\-]+", alts[-1].lower())[-1] if alts else ""
    salida, completadas = [], set()
    for n, a in enumerate(alts):
        p = a.split()
        if len(alts) > 1 and len(p) < len(completa) and nucleo not in re.split(r"[\s\-]+", a.lower()):
            a = " ".join(p + completa[1:])
            completadas.add(n)
        salida.append(a)
    return salida, completadas


def _completar_es(alts):
    # «**fragmentos grandes** / **medianos** / **pequeños**»: el sustantivo va adelante, y se toma de la
    # primera. Solo se llama si el ingles de la misma fila venia abreviado
    completa = alts[0].split() if alts else []
    salida = []
    for a in alts:
        p = a.split()
        if len(p) < len(completa) and completa[0].lower() not in (x.lower() for x in p):
            a = " ".join(completa[:len(completa) - len(p)] + p)
        salida.append(a)
    return salida


def _columnas(cabecera):
    """Pares (ingles, español) de columnas segun el encabezado de la tabla. Los glosarios de RML usan
    «Inglés | Español», pero tambien «Sigla | Nombre en inglés | Español», «Inglés | Singular | Plural»,
    «Inglés | Masculino | Femenino» y dos pares en la misma fila. Una tabla sin esas columnas no es de
    terminos («Opción | Cómo queda»)."""
    n = [norm(c) for c in cabecera]
    pares = []
    for i, c in enumerate(n):
        if c.startswith("ingles") or c.startswith("serie (ingles") or c == "nombre en ingles":
            for j in range(i + 1, len(n)):
                if n[j].startswith("espanol") or n[j] in ("singular", "masculino"):
                    pares.append((i, j))
                    break
                if n[j].startswith("ingles"):
                    break
    return pares


def _separador(linea):
    return linea.startswith("|") and set(linea.strip()) <= set("|-: ")


def leer_glosario(md):
    """Las filas de terminos de un glosario: por el encabezado de cada tabla se sabe que columna es el
    ingles y cual el español. En el español manda lo que va en negrita; si la celda no tiene negrita
    (RimPacts, Alpha Prefabs, Combat Extended), la celda entera. Las alternativas «a / b» se emparejan si
    son tantas de un lado como del otro; si no, cada ingles acepta cualquiera de las traducciones."""
    terminos, lineas, pares = [], md.splitlines(), []
    filas = []
    for k, linea in enumerate(lineas):
        if not linea.startswith("|"):
            pares = []
            continue
        if _separador(linea):
            continue
        celdas = [c.strip() for c in linea.strip().strip("|").split("|")]
        if k + 1 < len(lineas) and _separador(lineas[k + 1]):
            pares = _columnas(celdas)
            continue
        filas += [(celdas[i], celdas[j]) for i, j in pares if j < len(celdas)]
    for celda_en, celda_es in filas:
        if "**" in celda_es:
            # «(detonación por calor)» traduce solo el parentesis del ingles: no es un termino que buscar
            es_alts = [x for x in re.findall(r"\*\*(.+?)\*\*", celda_es) if not x.startswith("(")]
        else:
            limpia = re.sub(r"\(.*?\)|`|«|»|\"", "", celda_es)
            es_alts = [x.strip() for x in re.split(r"\s*/\s*", limpia) if x.strip()]
        if not es_alts:
            continue
        en = re.sub(r"\(.*?\)|`|\*|«|»|\"|[^\w\s/\-'.&]", "", celda_en)
        en_alts, completadas = _completar_en([a.strip() for a in re.split(r"\s*/\s*", en) if a.strip()])
        # La abreviatura no siempre se reconoce («handwheels / optical sight» son dos terminos): el español
        # completado se acepta ademas del original, nunca en su lugar, para no inventar un aviso
        comp = _completar_es(es_alts) if completadas and len(en_alts) == len(es_alts) else es_alts
        pares = (zip(en_alts, [list(dict.fromkeys([c, x])) for c, x in zip(comp, es_alts)])
                 if len(en_alts) == len(es_alts) else ((a, es_alts) for a in en_alts))
        for en_t, es_l in pares:
            es_l = [x for x in es_l if "…" not in x and "tal cual" not in x and x.strip() != "—"]
            palabras = re.findall(r"[A-Za-z0-9]+", en_t)
            if "…" in en_t or "..." in en_t or len(en_t) < 3 or not es_l or not palabras:
                continue
            # «union → unión cliente»: con una sola palabra en ingles, el sustantivo del español (la primera)
            # tambien vale, porque el ingles suelto no siempre lleva el resto
            if len(palabras) == 1:
                es_l = list(dict.fromkeys(es_l + [x.split()[0] for x in es_l if len(x.split()) > 1]))
            regs = [r for r in map(_re_es, es_l) if r]
            if regs:
                # Palabra entera, con su plural: «EMP» no tiene que encontrar «employed», ni «EVA», «evaded»
                re_en = re.compile(r"(?<![A-Za-z])" + r"[\s\-]+".join(map(re.escape, palabras))
                                   + r"(?:e?s)?(?![A-Za-z])", re.I)
                terminos.append(Termino(en_t, tuple(es_l), re_en, tuple(regs)))
    return terminos


def terminos(entradas, glosario):
    """Termino -> claves cuyo ingles lo usa y cuyo español no tiene la traduccion acordada."""
    avisos = collections.OrderedDict()
    for e in entradas:
        if e.valor == "TODO" or not e.en:
            continue
        es = norm(e.valor)
        cumplidos, faltan = [], []
        for t in glosario:
            # «heat» (calor) no es «HEAT» (la sigla de la municion): un termino en minuscula no busca
            # su forma toda en mayusculas
            m = [x for x in t.re_en.findall(e.en) if not (t.en.islower() and x.isupper() and len(x) > 1)]
            if m:
                (cumplidos if any(r.search(es) for r in t.re_es) else faltan).append(t)
        for t in faltan:
            # «shell» es «obús», pero en «shotgun shell» manda «cartucho de escopeta»: si un termino mas
            # largo que lo contiene se cumple, el corto no avisa
            if not any(t.en.lower() in c.en.lower() and len(c.en) > len(t.en) for c in cumplidos):
                avisos.setdefault(t, []).append(e.clave)
    return avisos


def devolver_lf(orig, nuevo):
    """Devuelve a LF los renglones que en la copia no tenian CR (Edit pasa todo a CRLF)."""
    a, b = orig.split(b"\n"), nuevo.split(b"\n")
    if len(a) != len(b):
        raise Error("distinto numero de renglones")
    return b"\n".join(y[:-1] if not x.endswith(b"\r") and y.endswith(b"\r") else y for x, y in zip(a, b))


def archivos_del_mod(carpeta):
    rels = []
    for raiz, _, nombres in os.walk(carpeta):
        for a in nombres:
            rel = os.path.relpath(os.path.join(raiz, a), carpeta).replace("\\", "/")
            if a.endswith(".xml") and a != "UNUSED.xml" and rel.startswith(("Languages/", "Patches/")):
                rels.append(rel)
    return rels


# ---- Lo de abajo toca disco, git y bash: lo de arriba es lo que cubren las pruebas ----

def carpetas_de_mods():
    for base in ("Data", "Ajustes"):
        for raiz, dirs, nombres in os.walk(os.path.join(RAIZ, base)):
            if "LoadFolders.Build.yaml" in nombres:
                dirs[:] = []
                yield raiz


def buscar_mod(nombre):
    p = ruta(nombre)
    if os.path.isdir(p):
        return os.path.abspath(p)
    candidatos = [c for c in carpetas_de_mods()
                  if os.path.basename(c).lower().startswith(nombre.lower()) or c.endswith(" - " + nombre)]
    exactos = [c for c in candidatos if os.path.basename(c).lower() == nombre.lower()
               or re.fullmatch(re.escape(nombre.lower()) + r" - \d+", os.path.basename(c).lower())]
    # Un mod con ajustes tiene dos carpetas con el mismo nombre: la de Data/ es la traduccion; la de
    # Ajustes/ se pide por su ruta
    en_data = [c for c in exactos if os.path.relpath(c, RAIZ).startswith("Data")]
    if len(exactos) > 1 and len(en_data) == 1:
        return en_data[0]
    if len(exactos) == 1 or len(candidatos) == 1:
        return (exactos or candidatos)[0]
    if not candidatos:
        raise Error(f"no hay ninguna carpeta de mod que empiece con «{nombre}»")
    raise Error(f"«{nombre}» es ambiguo:\n  " + "\n  ".join(os.path.relpath(c, RAIZ) for c in candidatos))


def leer_mod(carpeta):
    return {rel: open(os.path.join(carpeta, rel), "rb").read() for rel in archivos_del_mod(carpeta)}


def claves_de_keyed(archivos):
    return {e.clave for rel, b in archivos.items() if "/Keyed/" in rel
            for e in leer_entradas(_lineas(b), False)[0]}


def git(*args):
    r = subprocess.run(["git", "-C", RAIZ, *args], capture_output=True)
    return r.stdout if r.returncode == 0 else None


def rel_git(carpeta, rel):
    return os.path.relpath(os.path.join(carpeta, rel), RAIZ).replace("\\", "/")


def glosarios(carpeta, extra):
    rutas = sorted(glob.glob(os.path.join(glob.escape(carpeta), "GLOSARIO*.md"))) + [ruta(x) for x in extra or []]
    return rutas, [t for r in rutas for t in leer_glosario(open(r, encoding="utf-8-sig").read())]


def mostrar_terminos(avisos, glosario):
    for t, cl in avisos.items():
        print(f"  «{t.en}» sin «{' / '.join(t.es)}»: {len(cl)} — {', '.join(cl[:3])}{' …' if len(cl) > 3 else ''}")
    print(f"  {sum(map(len, avisos.values()))} aviso(s) de terminos (para mirar; no bloquean)")


def indice_rml(excluir):
    """Ingles exacto -> traducciones que ya usa RML fuera de este mod (las sugerencias de la tabla)."""
    par = re.compile(r"<!-- EN: ([^\n]*?) -->[ \t]*\r?\n[ \t]*<([A-Za-z0-9_.\-]+)>([^<\n]*)</\2>")
    h = collections.defaultdict(collections.Counter)
    for raiz, _, nombres in os.walk(os.path.join(RAIZ, "Data")):
        if os.path.abspath(raiz).startswith(os.path.abspath(excluir)):
            continue
        for a in nombres:
            if a.endswith(".xml") and a != "UNUSED.xml":
                texto = open(os.path.join(raiz, a), encoding="utf-8", errors="replace").read()
                for en, _, val in par.findall(texto):
                    if val != "TODO":
                        h[en.strip().lower()][val] += 1
    return h


def bash():
    if os.name != "nt":
        return "bash"
    # En Windows, «bash» suelto puede ser el de WSL: se usa el de Git, que trae grep, sed y awk
    g = shutil.which("git")
    d = os.path.dirname(os.path.realpath(g)) if g else ""
    for _ in range(4):
        c = os.path.join(d, "bin", "bash.exe")
        if d and os.path.exists(c):
            return c
        d = os.path.dirname(d)
    c = r"C:\Program Files\Git\bin\bash.exe"
    if os.path.exists(c):
        return c
    raise Error("no se encontro el bash de Git")


# Las tres comprobaciones del diff de AGENTS.md («Antes de commitear: mirar el diff»), tal cual
COMPROBACIONES = [
    ("1) que ninguna traduccion haya cambiado de valor", r'''git diff HEAD -U0 -- "Data/*/Languages/*" "Data/*/*/Languages/*" \
  | grep -E "^[-+]  <[A-Za-z]" \
  | sed 's/^\([-+]\) *<\([^>]*\)>\(.*\)<\/.*/\1 \2 = \3/' \
  | awk '{k=$2; v=substr($0, index($0,"= ")+2);
          if ($1=="-") old[k]=v; else new[k]=v}
     END {for (k in new) if (k in old && old[k]!="TODO" && new[k]!="TODO" && old[k]!=new[k])
            print k ": " old[k] " -> " new[k]}' '''),
    ("2) que no se haya borrado ningun UNUSED.xml", r'''git status --short | grep -E '^( D|D )' '''),
    ("3) que toda regla de gramatica conserve su «simbolo->»", r'''git diff HEAD --name-only -z | xargs -0 -r awk '
  /<[A-Za-z0-9_.]+\.rulesStrings>/ {dentro=1}
  /<\/[A-Za-z0-9_.]+\.rulesStrings>/ {dentro=0}
  dentro && /<li>/ && !/-&gt;/ {print FILENAME ": " $0}
' '''),
]


def comprobar(tocados=None):
    b, sucio = bash(), False
    for titulo, cmd in COMPROBACIONES:
        r = subprocess.run([b, "-c", cmd], cwd=RAIZ, capture_output=True)
        salida = r.stdout.decode("utf-8", "replace").strip()
        print(f"  {titulo}: {'vacia' if not salida else 'SALIO ALGO'}")
        if salida:
            sucio = True
            print("    " + salida.replace("\n", "\n    "))
    if tocados is not None:
        estado = (git("-c", "core.quotepath=off", "status", "--short") or b"").decode("utf-8", "replace")
        ajenos = [l[3:].strip('"') for l in estado.splitlines() if l[3:].strip('"') not in tocados]
        if ajenos:
            print("  Cambios sin commitear que no son de esta tanda (de otra sesion o de -build):")
            for a in ajenos:
                print("    " + a)
    return sucio


def cmd_estado(a):
    carpetas = [buscar_mod(a.mod)] if a.mod else list(carpetas_de_mods())
    total = 0
    for c in carpetas:
        archivos = leer_mod(c)
        keyed = claves_de_keyed(archivos)
        filas = []
        for rel, b in sorted(archivos.items()):
            ent, sueltas = leer_entradas(_lineas(b), es_patch(rel))
            todo = [e for e in ent if e.valor == "TODO"]
            claves = sum(1 for e in todo if e.en and e.en.strip() in keyed)
            if todo or sueltas:
                filas.append((rel, len(todo), claves, sum(_palabras(e.en) for e in todo), len(sueltas)))
        n = sum(f[1] for f in filas)
        total += n
        if a.mod:
            for rel, t, cl, p, s in filas:
                extra = (f", {cl} clave(s) de Keyed" if cl else "") + (f", {s} lista(s) a mano" if s else "")
                print(f"{t:5} TODO  {p:6} palabras  {rel}{extra}")
        if n:
            print(f"{n:5} TODO  {sum(f[3] for f in filas):6} palabras  {os.path.relpath(c, RAIZ)}")
    if not a.mod:
        print(f"{total:5} TODO en total")


def cmd_tabla(a):
    carpeta = buscar_mod(a.mod)
    archivos = leer_mod(carpeta)
    if a.archivo:
        archivos = {r: b for r, b in archivos.items() if a.archivo in r}
        if not archivos:
            raise Error(f"ningun archivo del mod contiene «{a.archivo}»")
    sug = None if a.sin_sugerencias else indice_rml(carpeta)
    texto = plantilla(archivos, claves_de_keyed(leer_mod(carpeta)), sug, a.palabras, a.claves)
    if a.salida:
        open(ruta(a.salida), "w", encoding="utf-8", newline="\n").write(texto)
        print(f"tabla escrita en {a.salida}: {texto.count(chr(9) + chr(10))} entradas; {texto.splitlines()[-1][2:]}")
    else:
        sys.stdout.write(texto)


def cmd_aplicar(a):
    carpeta = buscar_mod(a.mod)
    archivos = leer_mod(carpeta)
    nuevos, inf = aplicar_tabla(open(ruta(a.tabla), encoding="utf-8-sig").read(), archivos,
                                claves_de_keyed(archivos), a.forzar, a.claves)
    copia = os.path.join(tempfile.gettempdir(), "rml-traducir", time.strftime("%Y%m%d-%H%M%S"))
    for rel, b in nuevos.items():
        destino = os.path.join(copia, rel)
        os.makedirs(os.path.dirname(destino), exist_ok=True)
        open(destino, "wb").write(archivos[rel])
        open(os.path.join(carpeta, rel), "wb").write(b)
    for rel in sorted(nuevos):
        quedan = sum(1 for e in leer_entradas(_lineas(nuevos[rel]), es_patch(rel))[0] if e.valor == "TODO")
        print(f"{rel}: aplicadas {inf.aplicadas[rel]}; quedan TODO {quedan}")
    if nuevos:
        print(f"copia de antes: {copia}")
    if inf.vacias:
        print(f"{inf.vacias} renglon(es) de la tabla sin llenar: quedaron en TODO")
    for k in inf.no_encontradas:
        print("NO ENCONTRADA (o ya no era TODO):", k)
    for x in inf.avisos:
        print("AVISO (forzado):", x)
    rutas, glos = glosarios(carpeta, a.glosario)
    print(f"Terminos ({len(rutas)} glosario(s)):")
    mostrar_terminos(terminos(inf.tocadas, glos), glos)
    print("Comprobaciones del diff:")
    sucio = comprobar({rel_git(carpeta, r) for r in nuevos})
    return 1 if sucio or inf.no_encontradas else 0


def cmd_revisar(a):
    carpeta = buscar_mod(a.mod)
    total = 0
    for rel, b in sorted(leer_mod(carpeta).items()):
        if os.path.isdir(ruta(a.contra)):
            p = os.path.join(ruta(a.contra), rel)
            orig = open(p, "rb").read() if os.path.exists(p) else None
        else:
            # git show da LF y el archivo de trabajo puede tener CRLF: se comparan sin los CR
            orig = git("show", f"{a.contra}:{rel_git(carpeta, rel)}")
            if orig is not None:
                orig, b = orig.replace(b"\r\n", b"\n"), b.replace(b"\r\n", b"\n")
        if orig is None or orig == b:
            continue
        mal = revisar(orig, b, es_patch(rel))
        total += sum(p.grave for p in mal)
        for p in mal:
            print(f"{rel}: {'GRAVE ' if p.grave else 'aviso '}{p.texto}")
    print(f"revisado: {total} problema(s) grave(s)")
    return 1 if total else 0


def cmd_terminos(a):
    carpeta = buscar_mod(a.mod)
    entradas = [e for rel, b in leer_mod(carpeta).items() for e in leer_entradas(_lineas(b), es_patch(rel))[0]]
    rutas, glos = glosarios(carpeta, a.glosario)
    print(f"{len(rutas)} glosario(s), {len(glos)} termino(s):")
    mostrar_terminos(terminos(entradas, glos), glos)


def cmd_comprobar(a):
    return 1 if comprobar() else 0


def cmd_prueba(a):
    carpeta = buscar_mod(a.mod)
    grupos = collections.defaultdict(list)
    for rel, b in sorted(leer_mod(carpeta).items()):
        if "/Keyed/" in rel or "/Strings/" in rel:
            continue
        antes = {}
        if a.desde:
            viejo = git("show", f"{a.desde}:{rel_git(carpeta, rel)}")
            if viejo is not None:
                antes = {e.clave: e.valor for e in leer_entradas(_lineas(viejo), es_patch(rel))[0]}
        for e in leer_entradas(_lineas(b), es_patch(rel))[0]:
            if not re.search(r"(\.|\|)(label|labelShort|title)$", e.clave) or e.valor == "TODO":
                continue
            if a.desde and antes.get(e.clave, "TODO") != "TODO":
                continue
            m = re.search(r"/Defs/([A-Za-z.]+)\[", e.clave)
            tipo = m.group(1) if es_patch(rel) and m else rel.split("/")[-2]
            grupos[tipo].append(f"{e.valor}  ({e.en})")
    for tipo in sorted(grupos):
        print(f"{tipo} ({len(grupos[tipo])}):")
        for x, n in collections.Counter(grupos[tipo]).items():
            print("  " + x + (f"  x{n}" if n > 1 else ""))
    print(f"{sum(map(len, grupos.values()))} nombre(s) para mirar en el juego")


def cmd_lineas(a):
    p = ruta(a.archivo)
    nuevo = devolver_lf(open(ruta(a.copia), "rb").read(), open(p, "rb").read())
    open(p, "wb").write(nuevo)
    print("listo")


def cmd_corregir(a):
    """Cambia valores ya traducidos: cada renglon es clave<TAB>viejo<TAB>nuevo, y el viejo tiene que estar
    una sola vez en el valor (la clave puede contenerlo: DMS_Gun_2A38M -> «2A38M»)."""
    carpeta = buscar_mod(a.mod)
    archivos = leer_mod(carpeta)
    lineas = {rel: _lineas(b) for rel, b in archivos.items()}
    donde = collections.defaultdict(list)
    for rel, ls in lineas.items():
        for e in leer_entradas(ls, es_patch(rel))[0]:
            donde[e.clave].append((rel, e))
    cambiados = set()
    for n, l in enumerate(open(ruta(a.cambios), encoding="utf-8-sig").read().split("\n"), 1):
        l = l.rstrip("\r")
        if not l.strip() or l.startswith("#"):
            continue
        k, viejo, nuevo = l.split("\t")
        if len(donde[k]) != 1:
            raise Error(f"cambios renglon {n}: {k} aparece {len(donde[k])} veces en el mod")
        rel, e = donde[k][0]
        if e.valor.count(viejo) != 1:
            raise Error(f"cambios renglon {n}: «{viejo}» no esta una sola vez en {k}")
        es_li = e.clave.endswith("]")
        m = (RE_LI if es_li else RE_VALOR).match(lineas[rel][e.i])
        v = e.valor.replace(viejo, nuevo)
        lineas[rel][e.i] = (f"{m.group(1)}<li>{v}</li>{m.group(3)}" if es_li
                            else f"{m.group(1)}<{m.group(2)}>{v}</{m.group(2)}>{m.group(4)}")
        cambiados.add(rel)
    for rel in sorted(cambiados):
        b = _bytes(lineas[rel], archivos[rel].startswith(BOM))
        ET.fromstring(b)
        open(os.path.join(carpeta, rel), "wb").write(b)
        print(f"{rel}: corregido")


def main(args=None):
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8")
    p = argparse.ArgumentParser(prog="traducir.py", description="Traducir un mod de RML con tabla (PLAN-HERRAMIENTAS.md)")
    s = p.add_subparsers(dest="orden", required=True)
    o = s.add_parser("estado"); o.add_argument("mod", nargs="?"); o.set_defaults(f=cmd_estado)
    o = s.add_parser("tabla"); o.add_argument("mod"); o.add_argument("--archivo"); o.add_argument("--palabras", type=int)
    o.add_argument("--claves", action="store_true"); o.add_argument("--salida")
    o.add_argument("--sin-sugerencias", action="store_true"); o.set_defaults(f=cmd_tabla)
    o = s.add_parser("aplicar"); o.add_argument("mod"); o.add_argument("tabla"); o.add_argument("--forzar", action="store_true")
    o.add_argument("--claves", action="store_true"); o.add_argument("--glosario", action="append"); o.set_defaults(f=cmd_aplicar)
    o = s.add_parser("revisar"); o.add_argument("mod"); o.add_argument("--contra", default="HEAD"); o.set_defaults(f=cmd_revisar)
    o = s.add_parser("terminos"); o.add_argument("mod"); o.add_argument("--glosario", action="append"); o.set_defaults(f=cmd_terminos)
    o = s.add_parser("comprobar"); o.set_defaults(f=cmd_comprobar)
    o = s.add_parser("prueba"); o.add_argument("mod"); o.add_argument("--desde"); o.set_defaults(f=cmd_prueba)
    o = s.add_parser("lineas"); o.add_argument("copia"); o.add_argument("archivo"); o.set_defaults(f=cmd_lineas)
    o = s.add_parser("corregir"); o.add_argument("mod"); o.add_argument("cambios"); o.set_defaults(f=cmd_corregir)
    a = p.parse_args(args)
    try:
        return a.f(a) or 0
    except Error as ex:
        print("ERROR:", ex)
        return 1


if __name__ == "__main__":
    sys.exit(main())
