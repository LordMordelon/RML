# Plan: [qsx] Insectoids: Lore Realized

Traducir Insectoids: Lore Realized (Workshop 3831266112, `qsx.hivequeen`) entero. Lo pidió Angel el
1-nov-2026, con el mod recién extraído y sin nada traducido.

## Contexto

- **Extracción nueva:** la carpeta no estaba en RML. La trajo la traducción rápida del extractor
  («0 conservadas, 207 sin traducir, 0 sin uso»). El índice se regeneró con `LoadFoldersBuilder -build`.
- **Sin `UNUSED.xml`**: no había traducción anterior que conservar.
- **De qué trata:** se domestica a la reina de la colmena de vanilla y un colono con un suero de
  feromonas se vincula con ella. Con el vínculo, la reina se puede reclutar, manda a su séquito de
  insectos a atacar, construye colmenas con sus megaescarabajos, y las colmenas se expanden a
  megacolmenas que, con la adaptación avanzada, se convierten en una reina nueva.
- Los términos están en [GLOSARIO-INSECTOIDS-LORE.md](GLOSARIO-INSECTOIDS-LORE.md). Lo que el mod
  copia de vanilla usa el texto oficial.

## Cifras (contadas el 1-nov-2026)

| Qué | Cuántas |
|---|---|
| Pendientes (`TODO`) | 207, 1546 palabras de inglés |
| En `Keyed` (botones, avisos, textos de inspección) | 128 |
| En `DefInjected` | 79, en 12 archivos (lo más grande, `ThingDef` con 31 y `HediffDef` con 12) |

## Etapas

| # | Etapa | Tamaño | Estado |
|---|---|---|---|
| 1 | Glosario del mod | chica | hecha: [GLOSARIO-INSECTOIDS-LORE.md](GLOSARIO-INSECTOIDS-LORE.md) |
| 2 | Traducir todo, con tabla (`traducir.py tabla` / `aplicar`) | chica | hecha (1-nov-2026) |
| 3 | Verificar: las cinco comprobaciones | chica | hecha: las cinco vacías |
| 4 | Confirmar términos y probar en partida | chica | falta: Angel |
| 5 | Nombres oficiales de insectos en otros mods | chica | hecha (1-nov-2026): ver «Corrección en otros mods» |

### 2. Decisiones al traducir

- **«cocoon» es «crisálida»**, como en la oficial (`CocoonMegascarab`: «crisálida de megaescarabajo»),
  no «capullo», que sugería RML por un solo mod.
- **«espelópedo» y «megaraña»**, como la oficial, no «espeleopeda» ni «megaaraña».
- **Textos en coreano.** Catorce claves de los defs `ThornHive_*` (los de prueba del autor) tienen el
  inglés en coreano. Se tradujeron del coreano: «suero de feromonas», «sacar jalea», «colmena de
  insectos amistosa (de prueba)», etc. «Thorn Hive», el nombre de la pestaña de investigación, quedó
  sin traducir por ser nombre propio.
- **`qsx_EclosionConsonant` y `qsx_EclosionVowel`** van iguales («Emerger como {0}»): el autor los separa
  por una partícula del coreano que el español no necesita.
- **Los `Keyed` que se arman por partes** (`\nTiempo restante: `, ` / Crisálidas: `) conservan los
  espacios y los `\n` de los bordes tal cual.
- **Revisado contra vanilla** (1-nov-2026, a pedido de Angel): «Transportando» (*hauling*),
  «infranqueable» (*impassable*), «Reclutar» / «Licenciar», «Encendido» / «Apagado», «impacto social»,
  «conciencia», «baba» y la descripción de la megacolmena, que sigue a la de `HiveQueen`.

### 4. Para confirmar con Angel

- «adaptación feromonal» (básica / avanzada) para *pheromone adaptation*.
- «feromona inhibidora del crecimiento» para *growth-suppressing pheromone*.
- «séquito» para *retinue*.
- Probar en partida que los textos de inspección armados por partes se lean bien.

## Corrección en otros mods (1-nov-2026, aprobada por Angel)

46 textos en 6 mods, un commit por mod, revisado cada uno contra su `EN`. Sin tocar los `UNUSED.xml`.

- **«megaaraña», «espelópodo», «espeleopeda» y «megascarabajo»** pasaron a «megaraña», «espelópedo» y
  «megaescarabajo»: VFE - Insectoids 2, Props and Decor, VRE - Insector y Alpha Books.
- **«capullo» pasó a «crisálida»** donde es la de un insecto, con los artículos y adjetivos en femenino:
  VFE - Insectoids 2 (la crisálida de jalea de los enjambradores), Props and Decor, VGE - Chapter 2 (la
  descripción del exogusano copia la de vanilla y quedó con el texto oficial), VRE - Insector y Zoology.
  `VFEI_ToBeCocooned` es el enjambrador que va a formar su crisálida (`CompSwarmlingToCocoon` en la DLL),
  no alguien envuelto: «por convertirse en crisálida».
- **VRE - Insector decía «metápodo» en las etiquetas y «metacapullo» en los `Keyed`**: quedó «metápodo»
  en todos, que es lo que se ve sobre el objeto.
- Los tres términos quedaron en [RML/GLOSARIO.md](../../GLOSARIO.md).

## Para más adelante

- **El género de las etiquetas.** El juego elige «un» o «una» con las listas `WordInfo/Gender` de la
  traducción oficial, que tienen el nombre completo de cada cosa («crisálida de megaraña»). RML no
  trae esas listas para ningún mod, así que una etiqueta femenina nueva («crisálida insectoide») puede
  salir con artículo masculino en algún mensaje. Pasa en todo RML, no solo acá.
