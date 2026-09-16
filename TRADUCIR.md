# Cómo traducir mods de RimWorld

Guía para colaborar traduciendo, **sin saber programar**. No hace falta git, ni consola, ni
instalar nada raro. Si sabes abrir un archivo de texto y reemplazar palabras, puedes hacer esto.

> 📖 **Antes de traducir, mira el [GLOSARIO](GLOSARIO.md).** Dice cómo se traduce cada término del
> juego siguiendo la traducción oficial de RimWorld: *mechanitor* es «mecanizador», *chemfuel* es
> «quimbustible», *bandwidth* es «banda ancha». Es lo que hace que dos personas distintas traduzcan
> igual.

---

## 1. De qué se trata

RimWorld tiene miles de mods y casi todos están solo en inglés. Este proyecto junta las
traducciones al español latino de muchos mods en un solo paquete que la gente instala una vez.

Tu trabajo es concreto: **una herramienta te genera una carpeta con los textos del mod, cada uno
con el inglés arriba y la palabra `TODO` donde falta traducir. Tú reemplazas cada `TODO` por el
español.** Eso es todo.

```xml
<!-- EN: Storage -->
<BuildingsNeatStorage.label>TODO</BuildingsNeatStorage.label>
```

queda así:

```xml
<!-- EN: Storage -->
<BuildingsNeatStorage.label>almacenamiento</BuildingsNeatStorage.label>
```

Un mod pequeño tiene 30 frases. Uno grande puede tener 2000. Puedes hacer una parte y dejar el
resto: no hace falta terminar nada para que sirva.

---

## 2. Qué necesitas

| | |
|---|---|
| **RimWorld** | Instalado en Steam. Se usa para leer los mods y para probar tu traducción. |
| **Un editor de texto** | El Bloc de notas de Windows alcanza. [Notepad++](https://notepad-plus-plus.org/) o [VS Code](https://code.visualstudio.com/) son gratis y mucho más cómodos: pintan los colores, buscan en todos los archivos a la vez y te dicen cuántos `TODO` quedan. |
| **El extractor** | Un programa que descargas y abres. No se instala. |

> ⚠️ **No uses Word ni Google Docs.** Cambian las comillas, agregan formato invisible y guardan el
> archivo de una forma que el juego no puede leer. Tiene que ser un editor de texto plano.

### Descargar el extractor

Entra a la **[página de Releases](https://github.com/LordMordelon/RimworldExtractor/releases/latest)**
del extractor y descarga **`RimworldExtractor-Portable.exe`**.

Es un solo archivo, no se instala: lo guardas donde quieras y lo abres haciendo doble clic.
Ponlo en una carpeta propia (por ejemplo `Documentos\RimWorld Traducciones\`), porque el programa
deja un par de archivos al lado suyo.

> **Windows te va a avisar que el archivo no es seguro.** Es normal y no significa que tenga virus:
> pasa con cualquier programa que no esté firmado digitalmente, algo que cuesta cientos de dólares
> por año. Si aparece una pantalla azul, haz clic en **Más información** → **Ejecutar de todas
> formas**.
>
> El archivo pesa alrededor de 80 MB porque trae todo lo que necesita adentro, así no tienes que
> instalar nada más. La primera vez que lo abres tarda unos segundos en arrancar.

---

## 3. La primera vez que lo abres

Se abre una ventana que te pide **dos rutas**. Son las carpetas donde Steam guarda RimWorld.

**Para encontrar la primera:** en Steam, clic derecho sobre RimWorld → **Administrar** →
**Ver archivos locales**. Se abre una carpeta; copia su dirección de la barra de arriba. Va a ser
algo como:

```
D:\SteamLibrary\steamapps\common\RimWorld
```

**La segunda** es la carpeta de los mods del Workshop. Es fácil: toma la ruta anterior, borra
desde `common` en adelante y pega `workshop\content\294100`:

```
D:\SteamLibrary\steamapps\workshop\content\294100
```

Ese `294100` es el número de RimWorld en Steam. Es siempre el mismo.

Pon las dos rutas y haz clic en **Listo**.

> El idioma de destino ya viene configurado en `SpanishLatin (Español(Latinoamérica))`.
> **No lo cambies.**

---

## 4. Extraer los textos de un mod

1. Clic en **1. Elegir el mod a extraer**. Aparece la lista de todos tus mods.

2. Busca el mod y haz **clic izquierdo** para elegirlo.

3. **Este paso no lo saltees**, aunque parezca opcional:

   > **Clic derecho** sobre el mod → **Elegir como referencia todos los mods relacionados con este**

   Muchos mods se apoyan en otros. Por ejemplo, casi todos los "Vanilla Expanded" dependen del
   *Vanilla Expanded Framework*. Si no marcas las referencias, la herramienta no encuentra esas
   definiciones y **te extrae textos incompletos o mal armados**. Es un solo clic y resuelve toda
   la cadena de dependencias sola.

4. **Destilda la casilla «Traducción rápida».** Marcada, el resultado se escribe directamente
   dentro del repositorio del mod RML, que es el modo que usa quien mantiene el proyecto. Tú
   quieres una carpeta suelta para trabajar.

5. Deja marcada **«Extracción completa»**: hace falta para los mods que le cambian textos al
   juego base.

6. Clic en **Listo**, y después en **2. Extraer los datos de traducción**.

Cuando termina te pregunta si quieres abrir la carpeta. Adentro hay una carpeta con el nombre del
mod y su número, así:

```
Nombre del mod - 1234567890/
  Languages/
    SpanishLatin/
      DefInjected/    <- lo que más vas a traducir
      Keyed/
      Strings/
  Patches/            <- puede no estar; si está, también se traduce
  LoadFolders.Build.yaml
```

Esa carpeta entera es lo que vas a devolver. **No le cambies el nombre ni muevas nada de lugar.**

---

## 5. Cómo se lee el archivo

Abre cualquier `.xml` de `DefInjected` con tu editor. Vas a ver pares de líneas:

```xml
<?xml version="1.0" encoding="utf-8"?>
<LanguageData>
  <!-- EN: children inherit area restriction from parent when born -->
  <ChildrenInheritAreaFromParent.label>TODO</ChildrenInheritAreaFromParent.label>
  <!-- EN: show disabled settings in searches -->
  <ShowDisabledInSearch.label>TODO</ShowDisabledInSearch.label>
</LanguageData>
```

- La línea `<!-- EN: ... -->` es **el texto original en inglés**. Está ahí para que traduzcas con el
  original a la vista, y para poder revisar después si una traducción quedó en el campo equivocado.
- La línea de abajo es **la única que tocas**: reemplazas `TODO` por el español.
- Lo que está entre `<` y `>` es el nombre de la etiqueta. Aparece dos veces, al principio y al
  final con una barra. **Nunca se toca.**

Para saber qué falta, busca `TODO` con **Ctrl+F**. En Notepad++ o VS Code puedes buscarlo en toda
la carpeta de una vez y ver cuántos quedan.

### Reglas que no se rompen

- **Solo reemplazas el `TODO`.** Ni las etiquetas, ni el comentario `EN:`, ni nada más.

- **No borres ni muevas líneas.** Aunque te tiente ordenar para trabajar más cómodo: hay tipos de
  texto —las reglas de gramática, sobre todo— que valen **por el orden en que están**, y moverlos
  rompe la traducción en el juego.

- **No toques las dos primeras líneas** de cada archivo (`<?xml ...?>` y `<LanguageData>`) ni la
  última (`</LanguageData>`).

- **Guarda siempre en UTF-8.** Es lo que trae el archivo y lo que espera el juego. Si tu editor te
  ofrece «ANSI» o «UTF-8 con BOM», elige **UTF-8** a secas. Guardarlo mal se ve enseguida: los
  acentos y las eñes aparecen como símbolos raros.

- **Un `TODO` que dejaste sin traducir muestra literalmente "TODO" en el juego.** Es a propósito
  —así se nota lo que falta— pero no entregues un archivo pensando que ahí se va a ver el inglés.

- **Si una frase no la sabes, déjala en `TODO`.** Esa es la forma correcta de decir «esto todavía
  no». **Nunca copies el inglés**: eso hace que el juego muestre el inglés como si fuera la
  traducción, y nadie va a volver a revisarla.

---

## 6. Lo que NO se traduce

**Esta es la sección más importante de la guía.** Adentro de los textos hay pedazos que el juego
reemplaza por otra cosa cuando estás jugando. Si los traduces, se rompen.

### Nombres entre llaves

```
{0} mods por procesar...
{PAWN_labelShort} está durmiendo
```

`{0}`, `{1}`, `{PAWN_labelShort}`, `{PAWN_nameDef}`, `{PAWN_pronoun}`, `{RESEARCH}`, `{STAT}`…

Lo de adentro de las llaves **se copia tal cual**, con las mismas mayúsculas y minúsculas. El juego
lo cambia por un número o por el nombre del colono. **Sí puedes moverlo de lugar** dentro de la
frase, que muchas veces hace falta para que el español suene bien:

> `{PAWN_labelShort} has finished {0} research projects`
> → `{PAWN_labelShort} terminó {0} proyectos de investigación`

### Los saltos de línea se escriben `\n`

```
Advertencia:\n{0}
```

Esa barra invertida seguida de `n` es un salto de línea. **Escríbela con el teclado, tal cual.**

> ⚠️ **Nunca partas la línea con Enter de verdad.** Toda la traducción va en un solo renglón, entre
> la etiqueta de apertura y la de cierre, por larga que sea. El salto se escribe `\n`.

### Etiquetas de formato

En el XML las etiquetas de color vienen **escapadas**, con `&lt;` y `&gt;` en lugar de `<` y `>`:

```xml
<!-- EN: &lt;color=#E5E54C&gt;Gameplay effect:&lt;/color&gt; Will explode on death. -->
<Algo.description>&lt;color=#E5E54C&gt;Efecto de juego:&lt;/color&gt; explotará al morir.</Algo.description>
```

Traduces **lo de adentro** (`Gameplay effect:` → `Efecto de juego:`). El `&lt;color=#E5E54C&gt;`, el
código del color y el `&lt;/color&gt;` del final se copian tal cual. Si los escribes con `<` y `>`
de verdad, **el juego no puede leer el archivo** y no carga ninguna traducción de ese mod.

### Los símbolos `&`, `<` y `>`

Como el archivo es XML, esos tres caracteres no se pueden escribir directamente. Se escriben así:

| Quieres que se lea | Escribes |
|---|---|
| `&` | `&amp;` |
| `<` | `&lt;` |
| `>` | `&gt;` |

Ejemplo: `Pedro & Juan` se escribe `Pedro &amp; Juan`. Los demás símbolos —acentos, eñes, comillas,
signos de pregunta— se escriben normales.

### Referencias a otras entradas

Si ves algo como `{*ThingDef+Algo.label}`, es una referencia a otra entrada. Se copia tal cual.

### Los números adentro del nombre de la etiqueta no se tocan

Si la etiqueta dice `verbs.0.label`, ese `0` es la posición de un elemento dentro del mod.
No lo cambies (igual, el nombre de la etiqueta no se toca nunca).

---

## 7. El nombre de la etiqueta te dice cómo traducir

El nombre de la etiqueta no se edita, pero **te dice qué es el texto**, y eso cambia la traducción.
Los casos que más importan:

| Si la etiqueta termina en… | Es… | Ejemplo |
|---|---|---|
| `pawnSingular` | Singular | `member` → "miembro" |
| `pawnsPlural`, `labelPlural` | **Plural** | `members` → "miembros" |
| `labelFemale`, `titleFemale` | Variante **femenina** | `hunter` → "cazadora" |
| `gerund`, `gerundLabel` | **Gerundio o infinitivo**, no un sustantivo | `operating` → "operando", **no** "operación" |
| `adjective` | Un **adjetivo** | `wooden` → "de madera" |
| `labelNoun`, `noun` | Un **sustantivo** | |
| `description` | Texto largo, puedes escribir con más soltura | |

Préstale atención al plural y al género: en inglés `members` y `member` a veces son la misma
palabra, en español nunca.

### Vas a ver entradas repetidas, y está bien

La herramienta **inventa algunas entradas a propósito**, para cubrir cosas que el autor del mod no
escribió pero el juego igual necesita:

- En las facciones aparecen `pawnSingular` / `pawnsPlural` / `leaderTitle` con las palabras
  genéricas `member`, `members`, `leader`. **Tradúcelas igual**: son las que el juego va a usar.
- En los escenarios, el mismo texto aparece dos veces (como `label` y como `scenario.name`).
  **Completa las dos con lo mismo.**
- En las armas, el nombre del arma puede aparecer también como `verbs.0.label`.

No son errores ni duplicados por accidente.

---

## 8. Si te pasan una carpeta actualizada

Cuando un mod se actualiza, se vuelve a extraer y te pueden pasar la carpeta de nuevo. Lo que ya
estaba traducido sigue ahí: **solo lo nuevo viene con `TODO`**. Busca `TODO` y listo.

Hay un caso que sí conviene mirar: cuando el autor del mod **le cambió el texto en inglés** a algo
que ya estaba traducido. La traducción vieja se conserva, pero el comentario `EN:` de arriba ya es
el texto nuevo. Si ves que el español no dice lo mismo que el inglés de arriba, probablemente sea
eso: rehaz esa línea.

> El archivo `UNUSED.xml` que puedas ver en la carpeta **no se toca**. Ahí se guardan solas las
> traducciones cuyo texto desapareció del mod, por si vuelve. Borrarlo pierde ese trabajo.

---

## 9. Probar tu traducción en el juego (opcional)

No hace falta, pero vale la pena ver el resultado antes de entregarlo. Y es fácil, porque lo que
extrajiste **ya tiene la forma de un mod**.

1. **Guarda** los archivos en tu editor.
2. Crea una carpeta nueva dentro de `RimWorld\Mods\`, ponle el nombre que quieras (por ejemplo
   `MiPrueba`), y copia adentro:
   - la carpeta `Languages` que se generó (y la carpeta `Patches`, si la hay),
   - una carpeta `About` con un archivo `About.xml` — pide la plantilla, es un archivo de 10 líneas.
3. En el juego: **Mods** → activa el mod original y `MiPrueba` **al final de la lista** →
   reinicia → **Opciones** → **Idioma** → **SpanishLatin**.

Deberías ver tus textos. Si algo sale en inglés, probablemente esa línea quedó en `TODO`. Si no
carga **ninguna** traducción del mod, casi siempre es un `<` o un `&` escrito sin escapar: mira la
sección 6.

---

## 10. Cómo lo entregas

- **Comprime la carpeta entera** (`Nombre del mod - 1234567890`) en un `.zip` y envía eso.
- **No le cambies el nombre a la carpeta ni a los archivos de adentro.** El nombre dice de qué mod
  es y a qué parte del mod va cada archivo; si se renombran, hay que averiguarlo a mano.
- **No borres el `LoadFolders.Build.yaml`** ni el `UNUSED.xml`, aunque no los hayas tocado.
- No hace falta que esté completo. Media traducción sirve.
- Si tuviste dudas con alguna frase, escríbelas aparte en un mensaje. No las dejes como comentarios
  dentro del XML.

**Dónde enviarlo:** <!-- TODO: completar con el canal del proyecto (Discord, Drive, etc.) -->
_(pendiente de definir — pregúntale a quien te pasó esta guía)_

---

## 11. Preguntas frecuentes

**El juego no muestra ninguna traducción del mod.**
Casi siempre es un `<`, un `>` o un `&` escrito sin escapar, que rompe el archivo entero. Mira la
sección 6. También puede ser que se haya borrado sin querer la línea `</LanguageData>` del final.

**Me aparecen símbolos raros en lugar de acentos y eñes.**
El archivo se guardó con la codificación equivocada. Vuelve a guardarlo en **UTF-8** desde tu
editor (en Notepad++: menú *Codificación* → *Convertir a UTF-8*).

**En el juego veo la palabra "TODO".**
Es una línea que quedó sin traducir. Búscala con Ctrl+F.

**El mod ya tiene algo de traducción al español.**
Puede pasar, y el extractor te lo avisa en el log al terminar. **Pero la extracción te va a salir
igual con todo en `TODO`:** el extractor siempre saca el original del inglés y nunca lee la
traducción que trae el mod.

Si quieres aprovecharla, abre esa carpeta a mano —está dentro del mod, en
`Languages/SpanishLatin`— y copia lo que sirva. Revisa lo que copies: si el mod se actualizó
después, esa traducción puede estar vieja.

**¿Cuánto tarda un mod?**
Uno de interfaz pequeño son 30 frases cortas, un rato. Uno grande con descripciones de objetos puede
llevar varias sesiones. **Empieza por uno pequeño.**

**¿Puedo usar un traductor automático?**
Como punto de partida sí, pero hay que revisarlo **siempre**: los traductores automáticos rompen los
`{0}` y los `\n`, y no distinguen gerundios de sustantivos ni el género. Una traducción automática
sin revisar es peor que ninguna, porque nadie la vuelve a mirar.

---

¿Algo no se entiende o te atascaste? Avisa — si esta guía no alcanzó, es un problema de la guía.
