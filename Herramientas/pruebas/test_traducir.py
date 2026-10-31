# Pruebas de traducir.py, sin el juego ni git. Uso (desde RML/):
#   python -I Herramientas/pruebas/test_traducir.py
import os, sys, tempfile, unittest

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
import traducir as T

BOM = b"\xef\xbb\xbf"


def xml(cuerpo, fin="\r\n"):
    lineas = ['<?xml version="1.0" encoding="utf-8"?>', "<LanguageData>"] + cuerpo + ["</LanguageData>", ""]
    return (BOM + fin.join(lineas).encode("utf-8"))


SIMPLE = xml([
    "  <!-- EN: steel beam -->",
    "  <A.label>TODO</A.label>",
    "  <!-- EN: Already done -->",
    "  <B.label>hecho</B.label>",
    "  <!-- EN: line one",
    "line two -->",
    "  <C.description>TODO</C.description>",
])

LISTA = xml([
    "  <!-- EN: r_name-&gt;Operation [first] -->",
    "  <!-- EN: first-&gt;Iron -->",
    "  <R.rulePack.rulesStrings>",
    "    <li>TODO</li>",
    "    <li>TODO</li>",
    "  </R.rulePack.rulesStrings>",
    "  <!-- EN: only one comment -->",
    "  <S.rulePack.rulesStrings>",
    "    <li>TODO</li>",
    "    <li>TODO</li>",
    "  </S.rulePack.rulesStrings>",
])

PATCH = (BOM + "\r\n".join([
    '<?xml version="1.0" encoding="utf-8"?>',
    "<Patch>",
    '  <Operation Class="PatchOperationFindMod">',
    "    <mods>",
    "      <li>Royalty</li>",
    "    </mods>",
    '    <match Class="PatchOperationSequence">',
    "      <operations>",
    '        <li Class="PatchOperationReplace">',
    '          <xpath>/Defs/ThingDef[defName="X"]/tools/li[1]/label</xpath>',
    "          <value>",
    "            <!-- EN: stock -->",
    "            <label>TODO</label>",
    "          </value>",
    "        </li>",
    '        <li Class="PatchOperationReplace">',
    '          <xpath>/Defs/ThingDef[defName="X"]/tools/li[2]/label</xpath>',
    "          <value>",
    "            <!-- EN: barrel -->",
    "            <label>TODO</label>",
    "          </value>",
    "        </li>",
    '        <li Class="PatchOperationAdd">',
    '          <xpath>/Defs/RulePackDef[defName="Y"]/rulePack/rulesStrings</xpath>',
    "          <value>",
    "            <!-- EN: subject-&gt;riot armor -->",
    "            <li>TODO</li>",
    "          </value>",
    "        </li>",
    "      </operations>",
    "    </match>",
    "  </Operation>",
    "</Patch>",
    "",
]).encode("utf-8"))


def aplicar(tabla, archivos, **kw):
    return T.aplicar_tabla(tabla, archivos, kw.pop("keyed", set()), **kw)


class Aplicar(unittest.TestCase):
    def test_solo_escribe_sobre_todo(self):
        nuevos, inf = aplicar("A.label\tviga de acero\nB.label\totra cosa\n", {"a.xml": SIMPLE})
        t = nuevos["a.xml"].decode("utf-8-sig")
        self.assertIn("<A.label>viga de acero</A.label>", t)
        self.assertIn("<B.label>hecho</B.label>", t)
        self.assertIn("B.label", inf.no_encontradas)

    def test_conserva_bom_finales_de_renglon_y_espacios(self):
        mezcla = SIMPLE.replace(b"  <!-- EN: line one\r\n", b"  <!-- EN: line one\n")
        nuevos, _ = aplicar("A.label\t  con espacios  \n", {"a.xml": mezcla})
        b = nuevos["a.xml"]
        self.assertTrue(b.startswith(BOM))
        self.assertIn(b"  <!-- EN: line one\n", b)
        self.assertIn(b"<A.label>  con espacios  </A.label>\r\n", b)
        self.assertEqual(len(b.split(b"\n")), len(mezcla.split(b"\n")))

    def test_rechaza_lo_que_romperia_el_xml(self):
        for malo in ("a <b>", "a > b", "a & b"):
            with self.assertRaises(T.Error):
                aplicar(f"A.label\t{malo}\n", {"a.xml": SIMPLE})
        nuevos, _ = aplicar("A.label\ta &amp; b &lt;color=#FF0000&gt;x&lt;/color&gt;\n", {"a.xml": SIMPLE},
                            forzar=True)  # el ingles no trae el color: la revision lo frenaria
        self.assertIn(b"a &amp; b", nuevos["a.xml"])

    def test_apostrofos_y_comillas_en_las_dos_formas(self):
        # El juego lee igual «'» y «&apos;», «"» y «&quot;»: el comando acepta las dos y no convierte nada
        for v in ("l'arme", "l&apos;arme", 'dijo "sí"', "dijo &quot;sí&quot;", "dijo «sí»"):
            nuevos, _ = aplicar(f"A.label\t{v}\n", {"a.xml": SIMPLE})
            self.assertIn(f"<A.label>{v}</A.label>".encode("utf-8"), nuevos["a.xml"])

    def test_rechaza_valores_vacios_repetidos_o_todo(self):
        with self.assertRaises(T.Error):
            aplicar("A.label\tuno\nA.label\tdos\n", {"a.xml": SIMPLE})
        with self.assertRaises(T.Error):
            aplicar("A.label\tTODO\n", {"a.xml": SIMPLE})
        nuevos, inf = aplicar("A.label\t\nC.description\tuno\\ndos\n", {"a.xml": SIMPLE})
        self.assertEqual(inf.vacias, 1)
        self.assertIn(b"<A.label>TODO</A.label>", nuevos["a.xml"])

    def test_clave_en_dos_archivos_necesita_encabezado(self):
        with self.assertRaises(T.Error):
            aplicar("A.label\tviga\n", {"a.xml": SIMPLE, "b.xml": SIMPLE})
        nuevos, _ = aplicar("## archivo: b.xml\nA.label\tviga\n", {"a.xml": SIMPLE, "b.xml": SIMPLE})
        self.assertNotIn("a.xml", nuevos)
        self.assertIn(b"<A.label>viga</A.label>", nuevos["b.xml"])

    def test_lo_grave_frena_y_lo_demas_avisa(self):
        nuevos, inf = aplicar("C.description\tsin el salto\n", {"a.xml": SIMPLE})
        self.assertIn(b"sin el salto", nuevos["a.xml"])
        self.assertEqual(len(inf.avisos), 1)
        for grave in ("con {1} inventada", "&lt;color=#FF0000&gt;sin cerrar"):
            with self.assertRaises(T.Error):
                aplicar(f"A.label\t{grave}\n", {"a.xml": SIMPLE})
        nuevos, inf = aplicar("A.label\tcon {1} inventada\n", {"a.xml": SIMPLE}, forzar=True)
        self.assertIn(b"{1}", nuevos["a.xml"])
        self.assertTrue(inf.avisos[0].startswith("a.xml: GRAVE"))

    def test_el_genero_entre_llaves_no_es_inventado(self):
        x = xml(["  <!-- EN: {0} in the {1} -->", "  <G>TODO</G>"])
        nuevos, _ = aplicar("G\t{0} en {1_gender ? el : la} {1}\n", {"g.xml": x})
        self.assertIn(b"{1_gender ? el : la}", nuevos["g.xml"])

    def test_listas_emparejadas_y_sin_emparejar(self):
        ent, sueltas = T.leer_entradas(LISTA.decode("utf-8-sig").splitlines(True), False)
        claves = [e.clave for e in ent]
        self.assertEqual(claves, ["R.rulePack.rulesStrings[0]", "R.rulePack.rulesStrings[1]"])
        self.assertEqual(ent[1].en, "first-&gt;Iron")
        self.assertEqual(sueltas, ["S.rulePack.rulesStrings"])

    def test_lista_mezclada_va_a_mano(self):
        mezclada = LISTA.replace(b"    <li>TODO</li>\r\n    <li>TODO</li>\r\n  </R.",
                                 b"    <li>r_name-&gt;Hecha [first]</li>\r\n    <li>TODO</li>\r\n  </R.")
        ent, sueltas = T.leer_entradas(mezclada.decode("utf-8-sig").splitlines(True), False)
        self.assertIn("R.rulePack.rulesStrings", sueltas)
        self.assertNotIn("R.rulePack.rulesStrings[1]", [e.clave for e in ent])

    def test_lista_a_medias_no_se_aplica(self):
        tabla = "R.rulePack.rulesStrings[0]\tr_name-&gt;Operación [first]\n"
        with self.assertRaises(T.Error):
            aplicar(tabla, {"r.xml": LISTA})
        tabla += "R.rulePack.rulesStrings[1]\tfirst-&gt;Hierro\n"
        nuevos, _ = aplicar(tabla, {"r.xml": LISTA})
        self.assertIn(b"<li>first-&gt;Hierro</li>", nuevos["r.xml"])

    def test_regla_de_gramatica_conserva_su_simbolo(self):
        base = "R.rulePack.rulesStrings[0]\tr_name-&gt;Operación [first]\n"
        for malo in ("Hierro", "segundo-&gt;Hierro"):
            with self.assertRaises(T.Error):
                aplicar(base + f"R.rulePack.rulesStrings[1]\t{malo}\n", {"r.xml": LISTA})

    def test_patches_por_xpath(self):
        ent, _ = T.leer_entradas(PATCH.decode("utf-8-sig").splitlines(True), True)
        claves = [e.clave for e in ent]
        self.assertEqual(claves, [
            '/Defs/ThingDef[defName="X"]/tools/li[1]/label|label',
            '/Defs/ThingDef[defName="X"]/tools/li[2]/label|label',
            '/Defs/RulePackDef[defName="Y"]/rulePack/rulesStrings|li[0]',
        ])
        tabla = ('/Defs/ThingDef[defName="X"]/tools/li[2]/label|label\tcañón\n'
                 '/Defs/RulePackDef[defName="Y"]/rulePack/rulesStrings|li[0]\tsubject-&gt;armadura antidisturbios\n')
        nuevos, _ = aplicar(tabla, {"Patches/p.xml": PATCH})
        t = nuevos["Patches/p.xml"].decode("utf-8-sig")
        self.assertIn("<label>cañón</label>", t)
        self.assertEqual(t.count("<label>TODO</label>"), 1)
        self.assertIn("<li>Royalty</li>", t)

    def test_campo_que_guarda_una_clave_de_keyed(self):
        x = xml(["  <!-- EN: DMSAC_PatrolSquad -->", "  <W.letterLabel>TODO</W.letterLabel>"])
        with self.assertRaises(T.Error):
            aplicar("W.letterLabel\tpatrulla\n", {"w.xml": x}, keyed={"DMSAC_PatrolSquad"})
        nuevos, _ = aplicar("W.letterLabel\tpatrulla\n", {"w.xml": x}, keyed={"DMSAC_PatrolSquad"}, claves=True)
        self.assertIn(b"patrulla", nuevos["w.xml"])


class Revisar(unittest.TestCase):
    def test_cambio_fuera_de_un_todo(self):
        nuevo = SIMPLE.replace(b"<B.label>hecho</B.label>", b"<B.label>otro</B.label>")
        self.assertTrue(T.revisar(SIMPLE, nuevo))

    def test_marcadores_y_saltos(self):
        x = xml(["  <!-- EN: Hello {0}, [PAWN_nameDef]\\nbye -->", "  <K>TODO</K>"])
        bien = x.replace(b"<K>TODO</K>", b"<K>Hola {0}, [PAWN_nameDef]\\nchao</K>")
        mal = x.replace(b"<K>TODO</K>", b"<K>Hola, [PAWN_nameDef] chao</K>")
        self.assertEqual(T.revisar(x, bien), [])
        self.assertEqual(len(T.revisar(x, mal)), 2)

    def test_cambio_de_renglones(self):
        self.assertTrue(T.revisar(SIMPLE, SIMPLE + b"<!-- x -->\r\n"))


class InglesCambiado(unittest.TestCase):
    # RimIris, 31-oct-2026: el mod cambio «RimIris Shader Packs» por «Shader Packs», la reextraccion
    # actualizo el comentario EN y el español viejo se commiteo sin que nada avisara
    def test_avisa_si_cambio_el_ingles_y_no_el_español(self):
        orig = xml(["  <!-- EN: RimIris Shader Packs -->", "  <T.title>Paquetes de shaders de RimIris</T.title>"])
        nuevo = xml(["  <!-- EN: Shader Packs -->", "  <T.title>Paquetes de shaders de RimIris</T.title>"])
        c = T.ingles_cambiado(orig, nuevo)
        self.assertEqual([(x.clave, x.antes, x.ahora) for x in c], [("T.title", "RimIris Shader Packs", "Shader Packs")])

    def test_no_avisa_si_el_español_cambio_o_es_todo_o_solo_cambia_el_escape(self):
        orig = xml(["  <!-- EN: a -->", "  <A.label>uno</A.label>", "  <!-- EN: b -->", "  <B.label>TODO</B.label>",
                    "  <!-- EN: it&apos;s -->", "  <C.label>es</C.label>"])
        nuevo = xml(["  <!-- EN: a2 -->", "  <A.label>uno2</A.label>", "  <!-- EN: b2 -->", "  <B.label>TODO</B.label>",
                     "  <!-- EN: it's -->", "  <C.label>es</C.label>"], fin="\n")
        self.assertEqual(T.ingles_cambiado(orig, nuevo), [])

    def test_patches_por_xpath(self):
        nuevo = PATCH.replace(b"EN: stock", b"EN: buttstock")
        self.assertEqual(T.ingles_cambiado(PATCH, nuevo, True), [])  # el valor es TODO
        hecho = PATCH.replace(b"<label>TODO</label>", b"<label>culata</label>", 1)
        c = T.ingles_cambiado(hecho, hecho.replace(b"EN: stock", b"EN: buttstock"), True)
        self.assertEqual([x.clave for x in c], ['/Defs/ThingDef[defName="X"]/tools/li[1]/label|label'])


class Desaparecidas(unittest.TestCase):
    # Insectoids 2, 31-oct-2026: la reextraccion recorto la lista de reglas al largo del ingles y las
    # variantes [insectoid_masc] agregadas a mano se fueron al UNUSED.xml sin que nada avisara
    def test_avisa_la_regla_que_se_fue(self):
        antes = {"a.xml": xml(["  <Q.rulesStrings>", "    <li>x_fem-&gt;Voraz</li>", "    <li>x_masc-&gt;Voraz</li>",
                               "  </Q.rulesStrings>"])}
        despues = {"a.xml": xml(["  <Q.rulesStrings>", "    <li>x_fem-&gt;Voraz</li>", "  </Q.rulesStrings>"])}
        self.assertEqual(T.desaparecidas(antes, despues), ["x_masc-&gt;Voraz"])

    def test_mover_de_archivo_no_es_perderla_y_lo_del_unused_se_marca(self):
        antes = {"Patches/p.xml": PATCH.replace(b"<label>TODO</label>", b"<label>culata</label>", 1),
                 "k.xml": xml(["  <!-- EN: lab -->", "  <L.label>laboratorio</L.label>"])}
        despues = {"d.xml": xml(["  <!-- EN: stock -->", "  <X.tools.0.label>culata</X.tools.0.label>"]),
                   "k.xml": xml([])}
        unused = b'<entry class="RoomRoleDef" node="L.label" original="lab">laboratorio</entry>'
        self.assertEqual(T.desaparecidas(antes, despues, unused), ["(al UNUSED) laboratorio"])


class Tabla(unittest.TestCase):
    def test_la_plantilla_llenada_se_aplica_entera(self):
        archivos = {"Languages/SpanishLatin/DefInjected/ThingDef/a.xml": SIMPLE, "r.xml": LISTA}
        plantilla = T.plantilla(archivos, set())
        self.assertIn("# EN: line one\\nline two", plantilla)
        self.assertIn("S.rulePack.rulesStrings", plantilla)  # avisada como lista para hacer a mano
        llenas = []
        for l in plantilla.split("\n"):
            if l and not l.startswith("#") and l.endswith("\t"):
                clave = l[:-1]
                v = {"C.description": "uno\\ndos"}.get(clave, "x")
                if clave.endswith("[0]"):
                    v = "r_name-&gt;Operación [first]"
                if clave.endswith("[1]"):
                    v = "first-&gt;Hierro"
                l += v
            llenas.append(l)
        nuevos, inf = aplicar("\n".join(llenas), archivos)
        self.assertEqual(inf.no_encontradas, [])
        self.assertEqual(sum(b.count(b">TODO<") for b in nuevos.values()), 2)  # las de S

    def test_tope_de_palabras(self):
        p = T.plantilla({"a.xml": SIMPLE}, set(), palabras=2)
        self.assertIn("A.label\t", p)
        self.assertNotIn("C.description\t", p)

    def test_sugerencias_de_rml(self):
        p = T.plantilla({"a.xml": SIMPLE}, set(), sugerencias={"steel beam": {"viga de acero": 3}})
        self.assertIn("# RML: viga de acero (3)", p)


class Glosario(unittest.TestCase):
    MD = "\n".join([
        "| Inglés | Español | Origen | Notas |",
        "|---|---|---|---|",
        "| missile / light missile | **misil** / **misil ligero** | CE | |",
        "| loitering munition | **munición merodeadora** | propuesta | |",
        "| Scorer | **tal cual** | — | |",
        "| … missile body | **cuerpo de misil …** | propuesta | |",
    ])
    CABECERA = "| Inglés | Español | Origen |\n|---|---|---|\n"

    def test_sin_negrita_y_por_columnas(self):
        # RimPacts, Alpha Prefabs y Combat Extended no marcan el español en negrita; una tabla «Sigla |
        # Nombre en inglés | Español» tiene el ingles en la segunda columna; «Opción | ...» no es de terminos
        md = "\n".join([
            "| Inglés | Español | Origen | Notas |", "|---|---|---|---|",
            "| quarter / quadrum (15 días) ✔ | cuadrun / cuadrunes | oficial | nota |",
            "", "| Sigla | Nombre en inglés | Español | Origen |", "|---|---|---|---|",
            "| FMJ | full metal jacket | blindada | propuesta |",
            "", "| Opción | Cómo queda | A favor |", "|---|---|---|", "| A | algo | sí |"])
        self.assertEqual([(t.en, t.es) for t in T.leer_glosario(md)],
                         [("quarter", ("cuadrun",)), ("quadrum", ("cuadrunes",)), ("full metal jacket", ("blindada",))])

    def test_lee_las_filas(self):
        g = T.leer_glosario(self.MD)
        ingles = sorted(t.en for t in g)
        self.assertEqual(ingles, ["light missile", "loitering munition", "missile"])

    def test_completa_las_abreviaturas(self):
        g = T.leer_glosario(self.CABECERA + "| large / medium / small fragments | **fragmentos grandes** / **medianos** / **pequeños** | x |\n"
                            "| bullet (cook-off) | **(detonación por calor)** | x |")
        self.assertEqual([(t.en, t.es) for t in g], [("large fragments", ("fragmentos grandes",)),
                         ("medium fragments", ("fragmentos medianos", "medianos")),
                         ("small fragments", ("fragmentos pequeños", "pequeños"))])

    def test_palabra_entera_con_plural(self):
        g = T.leer_glosario(self.CABECERA + "| EMP | **PEM** | oficial |\n| loitering munition | **munición merodeadora** | x |")
        ent = [T.Entrada("a", 0, "It is employed in EMP grenades", "Se usa en granadas", None),
               T.Entrada("b", 0, "Two loitering munitions", "Dos municiones merodeadoras", None),
               T.Entrada("c", 0, "Employed widely", "Muy usado", None)]
        self.assertEqual({t.en: c for t, c in T.terminos(ent, g).items()}, {"EMP": ["a"]})

    def test_el_termino_largo_manda_y_la_sigla_no_es_la_palabra(self):
        g = T.leer_glosario(self.CABECERA + "| shell | **obús** | oficial |\n| shotgun shell | **cartucho de escopeta** | x |\n"
                            "| heat | **calor** | oficial |")
        ent = [T.Entrada("a", 0, "12 gauge shotgun shell", "cartucho de escopeta 12 Gauge", None),
               T.Entrada("b", 0, "90mm cannon shell (HEAT)", "obús de cañón 90mm (HEAT)", None),
               T.Entrada("c", 0, "Cannon shell", "Cartucho", None)]
        self.assertEqual({t.en: c for t, c in T.terminos(ent, g).items()}, {"shell": ["c"]})

    def test_avisa_solo_lo_que_falta(self):
        g = T.leer_glosario(self.MD)
        ent = [T.Entrada("a", 0, "Two loitering munitions", "dos municiones merodeadoras", None),
               T.Entrada("b", 0, "A light missile", "un misil ligero", None),
               T.Entrada("c", 0, "Missiles", "cohetes", None)]
        avisos = {t.en: c for t, c in T.terminos(ent, g).items()}
        self.assertEqual(avisos, {"missile": ["c"]})


class Varios(unittest.TestCase):
    def test_rutas_de_git_bash(self):
        if os.name == "nt":
            self.assertEqual(T.ruta("/c/Users/x/a b"), "C:\\Users\\x\\a b")
        self.assertEqual(T.ruta("Data/x"), "Data/x")

    def test_lineas_devuelve_lf(self):
        orig = b"a\nb\r\nc\n"
        self.assertEqual(T.devolver_lf(orig, b"a\r\nb\r\nc\r\n"), b"a\nb\r\nc\n")

    def test_no_abre_unused(self):
        with tempfile.TemporaryDirectory() as d:
            for rel in ("Languages/SpanishLatin/Keyed/k.xml", "UNUSED.xml", "Patches/p.xml", "notas.md"):
                os.makedirs(os.path.dirname(os.path.join(d, rel)) or d, exist_ok=True)
                open(os.path.join(d, rel), "wb").write(SIMPLE)
            self.assertEqual(sorted(T.archivos_del_mod(d)), ["Languages/SpanishLatin/Keyed/k.xml", "Patches/p.xml"])


if __name__ == "__main__":
    unittest.main()
