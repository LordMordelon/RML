# Cómo traducir mods de RimWorld

Guía para colaborar traduciendo, **sin saber programar**. No hace falta git, ni consola, ni
instalar nada raro. Si sabes usar Excel, puedes hacer esto.

> 📖 **Antes de traducir, mira el [GLOSARIO](GLOSARIO.md).** Dice cómo se traduce cada término del
> juego siguiendo la traducción oficial de RimWorld: *mechanitor* es «mecanizador», *chemfuel* es
> «quimbustible», *bandwidth* es «banda ancha». Es lo que hace que dos personas distintas traduzcan
> igual.

---

## 1. De qué se trata

RimWorld tiene miles de mods y casi todos están solo en inglés. Este proyecto junta las
traducciones al español latino de muchos mods en un solo paquete que la gente instala una vez.

Tu trabajo es concreto: **una herramienta te genera una planilla de Excel con todos los textos de
un mod en inglés, y tú completas la columna de al lado en español.** Eso es todo.

Un mod pequeño tiene 30 frases. Uno grande puede tener 2000. Puedes hacer una parte y dejar el resto:
no hace falta terminar nada para que sirva.

---

## 2. Qué necesitas

| | |
|---|---|
| **RimWorld** | Instalado en Steam. Se usa para leer los mods y para probar tu traducción. |
| **Excel** o **WPS Office** | Para editar la planilla. LibreOffice también sirve, pero lee la sección 9 antes. |
| **El extractor** | Un programa que descargas y abres. No se instala. |

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

## 4. Generar la planilla de un mod

1. Clic en **1. Elegir el mod a extraer**. Aparece la lista de todos tus mods.

2. Busca el mod y haz **clic izquierdo** para elegirlo.

3. **Este paso no lo saltees**, aunque parezca opcional:

   > **Clic derecho** sobre el mod → **Elegir como referencia todos los mods relacionados con este**

   Muchos mods se apoyan en otros. Por ejemplo, casi todos los "Vanilla Expanded" dependen del
   *Vanilla Expanded Framework*. Si no marcas las referencias, la herramienta no encuentra esas
   definiciones y **te extrae textos incompletos o mal armados**. Es un solo clic y resuelve toda
   la cadena de dependencias sola.

4. Clic en **Listo**.

5. Antes de extraer, entra a **Opciones** y verifica que *Formato de extracción* diga
   **Archivo Excel (.xlsx) para trabajar la traducción**. Guarda y cierra.

6. Clic en **2. Extraer los datos de traducción**.

Cuando termina te pregunta si quieres abrir la carpeta. Adentro está tu `.xlsx`.

---

## 5. Cómo se lee la planilla

Tiene seis columnas. **Solo escribes en la última.**

| Columna | Qué es | ¿La tocas? |
|---|---|---|
| **A** `Class+Node` | Identificador interno de la fila | ❌ Nunca |
| **B** `Class` | Qué tipo de cosa es (`ThingDef`, `Keyed`…) | ❌ Nunca |
| **C** `Node` | Dónde va la traducción dentro del mod | ❌ Nunca — pero **léela**, ver sección 7 |
| **D** `Required Mods` | Qué mods tienen que estar activos | ❌ Casi nunca — ver abajo |
| **E** `English [Source string]` | El texto original en inglés | ❌ Nunca |
| **F** `SpanishLatin (…) [Translation]` | **Aquí escribes tú** | ✅ Sí |

### Reglas que no se rompen

- **No ordenes, muevas, insertes ni borres filas.** Aunque te tiente ordenar alfabéticamente para
  trabajar más cómodo: hay tipos de texto que se guardan **en el orden en que están en la planilla**,
  y reordenarlos rompe la traducción en el juego. Si quieres agrupar, usa el filtro de Excel (que no
  cambia el orden real) y quítalo antes de guardar.

- **No toques la primera fila.** Los títulos de las columnas se usan para reconocerlas. Si los
  cambias, la herramienta no puede leer el archivo.

- **No vacíes las columnas B ni C.** Si una queda vacía, esa fila desaparece **sin dar ningún
  error**. La traducción simplemente nunca aparece en el juego y es dificilísimo darse cuenta.

- **Dejar una celda de F vacía es la forma correcta de decir "esto todavía no lo traduje".**
  Esa fila se saltea y listo. **Nunca copies el inglés en F para "completar"**: eso hace que el
  juego muestre el inglés como si fuera la traducción, y nadie va a volver a revisarla.

### Sobre la columna D

Normalmente está vacía o tiene nombres de mods. **Solo la tocas si ves algo como
`##packageId##loquesea`**: eso significa que la herramienta no supo qué mod es. Cuando pasa,
la celda tiene un comentario explicándolo. Reemplaza todo ese texto por el nombre del mod tal como
aparece en Steam. Si no sabes cuál es, avisa y no inventes.

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

> ⚠️ **Nunca uses Alt+Enter dentro de la celda** para hacer un salto de línea de verdad. Se ve
> parecido en Excel, pero rompe el formato del archivo.

### Etiquetas de formato

```
<color=#FF0000>peligro</color>
<b>importante</b>
```

Traduces **lo de adentro** (`peligro` → `peligro`, `importante` → `importante`), nunca la etiqueta
ni el código de color.

### Referencias a otras filas

Si ves algo como `{*ThingDef+Algo.label}`, es una referencia a otra fila de la misma planilla.
Se copia tal cual.

### Los símbolos `&`, `<` y `>` se escriben normales

Escribe `Pedro & Juan`, no `Pedro &amp; Juan`. La herramienta hace la conversión sola; si tú
escribes `&amp;`, en el juego se va a leer literalmente `&amp;`.

### Los números de la columna C no se tocan

Si la columna C dice `verbs.0.label`, ese `0` es la posición de un elemento dentro del mod.
No lo cambies (igual, la columna C no se toca nunca).

---

## 7. Lee la columna C: te dice cómo traducir

La columna C no se edita, pero **te dice qué es el texto**, y eso cambia la traducción. Los casos
que más importan:

| Si C termina en… | Es… | Ejemplo |
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

### Vas a ver filas repetidas, y está bien

La herramienta **inventa algunas filas a propósito**, para cubrir cosas que el autor del mod no
escribió pero el juego igual necesita:

- En las facciones aparecen `pawnSingular` / `pawnsPlural` / `leaderTitle` con las palabras
  genéricas `member`, `members`, `leader`. **Tradúcelas igual**: son las que el juego va a usar.
- En los escenarios, el mismo texto aparece dos veces (como `label` y como `scenario.name`).
  **Completa las dos con lo mismo.**
- En las armas, el nombre del arma puede aparecer también como `verbs.0.label`.

No son errores ni duplicados por accidente.

---

## 8. Las celdas de colores

Si te pasan una planilla **actualizada** (porque el mod cambió desde la última vez), vas a ver
celdas pintadas con globitos amarillos al lado. Significan esto:

| Color | Dónde | Qué pasó | Qué haces |
|---|---|---|---|
| 🔴 Rojo | Columna E | El mod **eliminó** ese texto | Nada. Esa fila ya no se usa. Tu traducción vieja quedó guardada en el comentario por si acaso |
| 🟠 Naranja | Columna E | **El inglés cambió** | **Revisa y rehaz** la traducción. En el comentario está el texto viejo para comparar |
| 🟠 Naranja | Columna E | Se recuperó un texto que se había perdido | Fíjate que tu traducción siga teniendo sentido |
| 🔵 Celeste | Columna E | **Aquí arrancan las filas nuevas** | Todo de esa fila para abajo es trabajo nuevo |
| 🔵 Celeste | Columna F | Sin traducir | Complétala |

Pasa el mouse por encima de la celda para leer el comentario.

---

## 9. Si usas LibreOffice

Funciona, pero con tres advertencias reales:

1. **Guarda siempre como `.xlsx`**, nunca como `.ods`. Si te pregunta si quieres conservar el
   formato, di que sí.
2. **Vas a perder los comentarios de colores** de la sección 8. LibreOffice los escribe de una
   forma que la herramienta no entiende, así que los descarta al leer el archivo. Si te pasaron una
   planilla actualizada, anota aparte qué filas estaban marcadas antes de guardarla.
3. Si puedes, usa **Excel** o **WPS Office**. Es el camino probado.

**Google Sheets: mejor no.** Rompe los comentarios igual que LibreOffice y encima cambia cosas del
formato al exportar.

---

## 10. Probar tu traducción en el juego (opcional)

No hace falta, pero vale la pena ver el resultado antes de entregarlo.

1. **Guarda y cierra** el Excel.
2. En el extractor, entra a **Opciones** y cambia *Formato de extracción* a
   **Archivo XML distribuible**. Guarda.
3. Clic en **XLSX -> XML** y elige tu planilla. Al lado del Excel te aparece una carpeta `Languages`.
4. Crea una carpeta nueva dentro de `RimWorld\Mods\`, ponle el nombre que quieras (por ejemplo
   `MiPrueba`), y copia adentro:
   - la carpeta `Languages` que se generó,
   - una carpeta `About` con un archivo `About.xml` — pide la plantilla, es un archivo de 10 líneas.
5. En el juego: **Mods** → activa el mod original y `MiPrueba` **al final de la lista** →
   reinicia → **Opciones** → **Idioma** → **SpanishLatin**.

Deberías ver tus textos. Si algo sale en inglés, probablemente esa fila quedó vacía en la columna F.

> Esto es solo para verificar. **Lo que entregas sigue siendo el `.xlsx`.**

---

## 10b. El otro camino: traducir sobre el XML

El Excel es el camino principal y es lo que se entrega. Pero hay un segundo modo, útil para
**retoques puntuales** o para corregir algo ya traducido sin abrir una planilla entera.

En **Opciones** → *Formato de extracción*, elige **"Archivo XML para traducir a mano"**. En vez de
dejar el inglés como valor, escribe el original en un comentario y pone `TODO` donde falta traducir:

```xml
<!-- EN: Storage -->
<BuildingsNeatStorage.label>TODO</BuildingsNeatStorage.label>
```

Traduces reemplazando cada `TODO`, con el original a la vista justo arriba. Y para saber qué falta,
buscas `TODO` con Ctrl+F.

Valen las mismas reglas de la sección 6: lo que está entre llaves, los `\n` y las etiquetas de
formato **no se traducen**.

> ⚠️ **Un archivo con `TODO` sin terminar muestra literalmente "TODO" en el juego.** Es justamente la
> gracia —se nota enseguida lo que falta—, pero no lo entregues así pensando que se ve el inglés.

**¿Cuál conviene?** Para un mod entero, el Excel: tienes el original al lado en su columna y es mucho
más cómodo en volumen. El XML sirve para arreglos pequeños.

## 11. Cómo lo entregas

- **Envía el mismo archivo `.xlsx`, con el nombre exacto que tenía.**
  No le agregues "final", "v2", tu nombre ni la fecha: el nombre del archivo se usa para saber de
  qué mod es. Si lo renombras, hay que averiguarlo a mano.
- **Cierra el archivo antes de enviarlo.** Si queda abierto en Excel, no se puede leer.
- No hace falta que esté completo. Media traducción sirve.
- Si tuviste dudas con alguna frase, escríbelas aparte en un mensaje. No dejes comentarios dentro
  del Excel.

**Dónde enviarlo:** <!-- TODO: completar con el canal del proyecto (Discord, Drive, etc.) -->
_(pendiente de definir — pregúntale a quien te pasó esta guía)_

---

## 12. Preguntas frecuentes

**Me dice "No se encontró el mod original".**
Le cambiaron el nombre al archivo. Vuelve a ponerle el nombre original o pide una planilla nueva.

**Me dice "no se pudo guardar el archivo porque ya está en uso".**
Tienes el Excel abierto. Ciérralo y prueba de nuevo.

**Me dice "Ocurrió un error al leer los encabezados".**
Se modificó la primera fila de la planilla. Si tienes una copia sin tocar, usa esa; si no, pide una
nueva y vuelve a pegar tus traducciones en la columna F (sin mover filas).

**El mod ya tiene algo de traducción al español.**
Puede pasar, y el extractor te lo avisa en el log al terminar. **Pero la planilla te va a salir
igual con todo vacío:** el extractor siempre saca el original del inglés y nunca lee la traducción
que trae el mod.

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
