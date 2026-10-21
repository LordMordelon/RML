# Cómo traducir mods de RimWorld

Guía para colaborar traduciendo, **sin saber programar**. No hace falta git, ni consola, ni
instalar nada raro. Si sabes abrir un archivo de texto y buscar con Ctrl+F, puedes hacer esto.

> 📖 **Antes de traducir, mira el [GLOSARIO](GLOSARIO.md).** Dice cómo se traduce cada término del
> juego siguiendo la traducción oficial de RimWorld: *mechanitor* es «mecanizador», *chemfuel* es
> «quimbustible», *bandwidth* es «banda ancha». Es lo que hace que dos personas distintas traduzcan
> igual.

> 📊 **¿Prefieres trabajar en una planilla de Excel?** La versión actual del extractor ya no la
> genera. La anterior sí: es la
> **[0.1.30](https://github.com/LordMordelon/RimworldExtractor/releases/tag/0.1.30)**, y su
> [guía de la planilla](https://github.com/LordMordelon/RML/blob/e3650df538c3d2822bdaa5cf28ab410cf822e46d/TRADUCIR.md)
> sigue disponible. Esta es para la versión nueva, que trabaja sobre los archivos XML.

---

## 1. De qué se trata

RimWorld tiene miles de mods y casi todos están solo en inglés. Este proyecto junta las
traducciones al español latino de muchos mods en un solo paquete que la gente instala una vez.

Tu trabajo es concreto: **cada texto del mod está en un archivo XML, con el original en inglés
justo arriba y la palabra `TODO` donde falta la traducción. Tú reemplazas cada `TODO` por el texto
en español.** Eso es todo.

```xml
<!-- EN: Storage -->
<BuildingsNeatStorage.label>TODO</BuildingsNeatStorage.label>
```

queda así:

```xml
<!-- EN: Storage -->
<BuildingsNeatStorage.label>Almacenamiento</BuildingsNeatStorage.label>
```

Un mod pequeño tiene 30 frases. Uno grande puede tener 2000. Puedes hacer una parte y dejar el resto:
no hace falta terminar nada para que sirva.

---

## 2. Qué necesitas

| | |
|---|---|
| **Un editor de texto** | [Notepad++](https://notepad-plus-plus.org/) o [VS Code](https://code.visualstudio.com/), los dos gratis. El Bloc de notas de Windows sirve, pero no colorea nada y es fácil equivocarse. |
| **Los archivos del mod** | De RML, si el mod ya está; del extractor, si es nuevo. La sección 3 explica cómo. |
| **RimWorld** | Solo si vas a usar el extractor o a probar tu traducción en el juego. |

Los editores recomendados te muestran las etiquetas de un color y el texto de otro, así se ve
enseguida qué es lo que se toca y qué no. Y los dos pueden buscar en todos los archivos de una
carpeta a la vez, que es como se encuentra lo que falta (sección 7).

---

## 3. Conseguir los archivos del mod

Primero fíjate si el mod ya está en RML, en la
**[lista de mods](https://lordmordelon.github.io/RML/)**. Se puede buscar por nombre.

### Si el mod ya está en RML

Sus archivos ya tienen el original en inglés y un `TODO` en lo que falta. No necesitas el extractor.

1. Entra a **[github.com/LordMordelon/RML](https://github.com/LordMordelon/RML)**, haz clic en el
   botón verde **Code** y después en **Download ZIP**.
2. Descomprímelo en una carpeta de **ruta corta**, como `C:\RML`. Algunos nombres de archivo son
   largos, y dentro de una ruta larga Windows no los puede abrir.
3. Adentro, en `Data`, cada mod tiene su carpeta, con el nombre del mod y su número del Workshop:
   `Data\Vanilla Achievements Expanded - 2288125657`. Los autores con muchos mods tienen una
   carpeta propia que empieza con `!`: ese mod, por ejemplo, está en `Data\!Oskar Potocki\`.

Trabaja solo en la carpeta de tu mod. Las demás no las necesitas.

### Si el mod es nuevo

Lo genera **el extractor**, un programa que descargas y abres.

**Descargarlo.** Entra a la
**[página de Releases](https://github.com/LordMordelon/RimworldExtractor/releases/latest)**
del extractor y descarga **`RimworldExtractor-Portable.exe`**. Es un solo archivo, no se instala:
ponlo en una carpeta propia (por ejemplo `Documentos\RimWorld Traducciones\`), porque el programa
deja ahí su configuración y los mods que extraes.

> **Windows te va a avisar que el archivo no es seguro.** Es normal y no significa que tenga virus:
> pasa con cualquier programa que no esté firmado digitalmente, algo que cuesta cientos de dólares
> por año. Si aparece una pantalla azul, haz clic en **Más información** → **Ejecutar de todas
> formas**.
>
> El archivo pesa unos 70 MB porque trae todo lo que necesita adentro, así no tienes que
> instalar nada más. La primera vez que lo abres tarda unos segundos en arrancar.

**La primera vez que lo abres** te pide tres rutas. Las dos primeras son las carpetas donde Steam
guarda RimWorld:

- **Ruta de RimWorld.** En Steam, clic derecho sobre RimWorld → **Administrar** → **Ver archivos
  locales**. Se abre una carpeta; copia su dirección de la barra de arriba. Va a ser algo como
  `D:\SteamLibrary\steamapps\common\RimWorld`.
- **Ruta del workshop.** Toma la ruta anterior, borra desde `common` en adelante y pega
  `workshop\content\294100`: `D:\SteamLibrary\steamapps\workshop\content\294100`. Ese `294100` es
  el número de RimWorld en Steam, y es siempre el mismo.
- **Carpeta del mod RML.** **Déjala vacía.** Es para quien mantiene RML.

Haz clic en **Listo**.

> El idioma de destino ya viene configurado en `SpanishLatin (Español(Latinoamérica))`.
> **No lo cambies.**

**Extraer el mod.**

1. Clic en **Elegir un mod y extraer**. Aparece la lista de todos tus mods.
2. Marca la casilla **Extraer en carpeta**, arriba. **Sin ella no funciona**: el extractor intenta
   escribir dentro de RML, y te avisa que no sabe dónde está.
3. Busca el mod y haz **clic izquierdo** para elegirlo. Los mods en los que se apoya se agregan
   solos como referencia: no tienes que marcar nada más.
4. Clic en **Extraer los datos de traducción**.

Puede tardar un rato, porque antes carga todo el juego base. Cuando termina te pregunta si quieres
abrir la carpeta: queda al lado del programa, con el nombre del mod.

---

## 4. Qué hay en la carpeta

```
<Nombre del mod> - <número>\
  Languages\SpanishLatin\
    DefInjected\    los nombres y descripciones de objetos, investigaciones, facciones…
    Keyed\          los textos de la interfaz: botones, avisos, mensajes
    Strings\        listas de nombres (ver abajo)
  Patches\          textos que el mod les cambia a otros mods o al juego base
  LoadFolders.Build.yaml
  UNUSED.xml        solo en los que vienen de RML
```

- **Se traduce en `DefInjected`, `Keyed` y `Patches`**, en los `.xml`. No importa por cuál empieces.
- **No toques `LoadFolders.Build.yaml` ni `UNUSED.xml`.** El primero le dice al juego cuándo cargar
  la traducción. El segundo guarda traducciones de textos que el mod sacó, por si vuelven en una
  versión futura: no se ven en el juego, así que no hace falta traducirlas.
- **Los nombres de los archivos no se cambian.** Son códigos raros, como `ima1o0.xml`, y son así a
  propósito: el juego los necesita únicos.
- **Los `.txt` de `Strings`** son listas de nombres para generar personajes o lugares, uno por
  línea. Casi siempre se dejan como están. Si traduces alguno, no agregues ni borres líneas.

Los archivos de `Patches` se ven más complicados, pero lo que se traduce es lo mismo: el `TODO`
debajo de un `<!-- EN: ... -->`. Todo lo demás —`xpath`, `Operation`, `success`— no se toca.

```xml
<xpath>/Defs/ThingDef[defName="DMS_LightMachineGun"]/tools/li[1]/label</xpath>
<value>
  <!-- EN: stock -->
  <label>TODO</label>
</value>
```

---

## 5. Lo que NO se traduce

**Esta es la sección más importante de la guía.** Adentro de los textos hay pedazos que el juego
reemplaza por otra cosa cuando estás jugando. Si los traduces, se rompen.

### Nombres entre llaves y corchetes

```
{0} mods por procesar...
{PAWN_labelShort} está durmiendo
[PAWN_nameDef] se unió a la colonia
```

`{0}`, `{1}`, `{PAWN_labelShort}`, `{PAWN_nameDef}`, `[resolvedQuestName]`, `[PAWN_pronoun]`…

Lo de adentro de las llaves o los corchetes **se copia tal cual**, con las mismas mayúsculas y
minúsculas. El juego lo cambia por un número o por el nombre del colono. **Sí puedes moverlo de
lugar** dentro de la frase, que muchas veces hace falta para que el español suene bien:

> `{PAWN_labelShort} has finished {0} research projects`
> → `{PAWN_labelShort} terminó {0} proyectos de investigación`

### Los saltos de línea se escriben `\n`

```
Advertencia:\n{0}
```

Esa barra invertida seguida de `n` es un salto de línea. **Escríbela con el teclado, tal cual.**
Nunca aprietes Enter dentro de una traducción: cada traducción va en una sola línea del archivo.

### Etiquetas de color y formato

En el archivo se ven así, con `&lt;` y `&gt;` en lugar de los signos `<` y `>`:

```
&lt;color=#FF0000&gt;danger&lt;/color&gt;
```

Traduces **lo de adentro** (`danger` → `peligro`) y copias la etiqueta igual, con sus `&lt;` y
`&gt;` y el código de color sin tocar:

```
&lt;color=#FF0000&gt;peligro&lt;/color&gt;
```

### Los símbolos `&`, `<` y `>` van escritos con código

Los XML usan esos tres signos para su propia estructura, así que dentro de una traducción se
escriben así:

| Quieres que se lea | Escribes |
|---|---|
| `&` | `&amp;` |
| `<` | `&lt;` |
| `>` | `&gt;` |

`Pedro &amp; Juan` se ve en el juego como `Pedro & Juan`. Si escribes el `&` solo, el archivo
entero deja de cargar.

### Las reglas de gramática conservan su comienzo

En algunos archivos, sobre todo los de misiones y conversaciones, el original es una lista de
reglas con este aspecto:

```xml
<!-- EN: order-&gt;halt -->
<!-- EN: order-&gt;stand still -->
<DMS_GravekeeperChallengeTalk.logRulesInitiator.rulesStrings>
  <li>TODO</li>
  <li>TODO</li>
</DMS_GravekeeperChallengeTalk.logRulesInitiator.rulesStrings>
```

Cada comentario `EN` corresponde, en orden, a un `<li>`: el primero al primero, el segundo al
segundo. Lo que va **antes** de `-&gt;` es el nombre de la regla y se copia tal cual, con el
`-&gt;` incluido. Se traduce solo lo que va después:

```xml
<li>order-&gt;alto</li>
<li>order-&gt;quieto</li>
```

Sin el `order-&gt;`, la regla deja de funcionar y el juego no avisa nada. Tampoco agregues ni
borres `<li>`: tiene que haber uno por cada comentario.

---

## 6. Lee la etiqueta: te dice cómo traducir

Lo que está entre `<` y `>` no se edita, pero **te dice qué es el texto**, y eso cambia la
traducción. Los casos que más importan:

| Si la etiqueta termina en… | Es… | Ejemplo |
|---|---|---|
| `pawnSingular` | Singular | `member` → «miembro» |
| `pawnsPlural`, `labelPlural` | **Plural** | `members` → «miembros» |
| `labelFemale`, `titleFemale` | Variante **femenina** | `hunter` → «cazadora» |
| `gerund`, `gerundLabel` | **Gerundio o infinitivo**, no un sustantivo | `operating` → «operando», **no** «operación» |
| `adjective` | Un **adjetivo** | `wooden` → «de madera» |
| `labelNoun`, `noun` | Un **sustantivo** | |
| `description` | Texto largo, puedes escribir con más soltura | |

Préstale atención al plural y al género: en inglés `members` y `member` a veces son la misma
palabra, en español nunca.

### Vas a ver textos repetidos, y está bien

El extractor **agrega algunos textos a propósito**, para cubrir cosas que el autor del mod no
escribió pero el juego igual necesita:

- En las facciones aparecen `pawnSingular`, `pawnsPlural` y `leaderTitle` con las palabras
  genéricas `member`, `members`, `leader`. **Tradúcelas igual**: son las que el juego va a usar.
- En los escenarios, el mismo texto aparece dos veces (como `label` y como `scenario.name`).
  **Completa las dos con lo mismo.**
- En las armas, el nombre del arma puede aparecer también como `verbs.0.label`.

No son errores ni duplicados por accidente.

---

## 7. Reglas al editar

- **Reemplaza solo el `TODO`.** Las etiquetas, los comentarios `<!-- EN: ... -->`, el `xpath` y
  las líneas vacías quedan como están. El comentario `EN` es el original: con él se revisa después
  que cada traducción esté en su lugar.
- **No cambies la codificación.** Los archivos son `UTF-8`, que es lo que permite las tildes y la
  `ñ`. Notepad++ la muestra abajo a la derecha y VS Code en la barra de abajo: tiene que decir
  `UTF-8` o `UTF-8-BOM` (`UTF-8 with BOM`) antes y después de guardar. Si dice `ANSI`, no guardes.
- **Dejar el `TODO` es la forma correcta de decir «esto todavía no lo traduje».** En el juego se va
  a leer «TODO», y es justamente la gracia: se nota enseguida lo que falta.
- **Nunca copies el inglés en lugar del `TODO` para «completar».** El juego mostraría el inglés como
  si fuera la traducción, y nadie volvería a revisarla.

### Encontrar lo que falta

Dentro de un archivo, Ctrl+F y busca `TODO`.

Para ver todo lo que falta del mod de una vez:

- **Notepad++:** menú **Buscar** → **Buscar en archivos** (Ctrl+Shift+F). En *Buscar* pon `TODO`,
  en *Directorio* la carpeta del mod, y haz clic en **Buscar todo**. Abajo aparece la lista, y con
  doble clic vas a cada línea.
- **VS Code:** menú **Archivo** → **Abrir carpeta** y elige la del mod. Después Ctrl+Shift+F y
  busca `TODO`.

---

## 8. Probar tu traducción en el juego (opcional)

No hace falta, pero vale la pena ver el resultado antes de entregarlo.

1. Crea una carpeta nueva dentro de `RimWorld\Mods\` con el nombre que quieras, por ejemplo
   `MiPrueba`.
2. Copia adentro las carpetas `Languages` y `Patches` de tu mod.
3. Crea ahí una carpeta `About` y, adentro, un archivo `About.xml` con esto:

   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <ModMetaData>
     <name>Mi prueba de traducción</name>
     <author>Tu nombre</author>
     <packageId>miprueba.traduccion</packageId>
     <supportedVersions>
       <li>1.6</li>
       <li>1.7</li>
     </supportedVersions>
     <description>Prueba de una traducción.</description>
   </ModMetaData>
   ```

4. En el juego: **Mods** → activa el mod original y `MiPrueba` **al final de la lista**. Si tienes
   RML activo, desactívalo mientras pruebas, para ver tu versión y no la que ya está publicada.
5. Reinicia → **Opciones** → **Idioma** → **Español (Latino)**.

Deberías ver tus textos. Lo que todavía dice «TODO» es lo que falta. Si algo sale en inglés,
revisa que esa traducción esté en el archivo y que la etiqueta no haya cambiado.

> Si el juego muestra errores en rojo al cargar, casi siempre es un `&` sin escribir como `&amp;`,
> o una etiqueta que se borró sin querer. El error dice qué archivo es.

> Esto es solo para verificar. **Lo que entregas es la carpeta del mod**, no `MiPrueba`.

---

## 9. Cómo lo entregas

- **Comprime la carpeta del mod entera**, con su nombre: clic derecho → **Comprimir en archivo
  ZIP** (o **Enviar a** → **Carpeta comprimida**). El nombre de la carpeta lleva el número del
  Workshop, y con él se sabe de qué mod es.
- **No cambies los nombres** de la carpeta ni de los archivos, y no agregues archivos tuyos adentro.
- No hace falta que esté completo. Media traducción sirve.
- Si tuviste dudas con alguna frase, escríbelas aparte en un mensaje. No dejes comentarios tuyos
  dentro de los XML.

**Dónde enviarlo:** <!-- TODO: completar con el canal del proyecto (Discord, Drive, etc.) -->
_(pendiente de definir — pregúntale a quien te pasó esta guía)_

---

## 10. Preguntas frecuentes

**El extractor dice que necesita saber dónde está el mod RML.**
No marcaste **Extraer en carpeta** al elegir el mod. Vuelve a elegirlo con la casilla marcada.

**Windows no me deja descomprimir RML, o dice que la ruta es demasiado larga.**
Descomprímelo en una carpeta de ruta corta, como `C:\RML`.

**Me bajé RML y el mod tiene traducciones que no hice yo.**
Es lo normal: otras personas tradujeron antes. Corrige lo que veas mal y completa los `TODO`.

**El mod ya tiene algo de traducción al español.**
Puede pasar, y el extractor te lo avisa en el log al terminar. **Pero lo que extrae te va a salir
igual con todo en `TODO`:** el extractor siempre saca el original del inglés y nunca lee la
traducción que trae el mod.

Si quieres aprovecharla, abre esa carpeta a mano —está dentro del mod, en
`Languages/SpanishLatin` o `Languages/Spanish`— y copia lo que sirva. Revisa lo que copies: si el
mod se actualizó después, esa traducción puede estar vieja.

**¿Cuánto tarda un mod?**
Uno de interfaz pequeño son 30 frases cortas, un rato. Uno grande con descripciones de objetos puede
llevar varias sesiones. **Empieza por uno pequeño.**

**¿Puedo usar un traductor automático?**
Como punto de partida sí, pero hay que revisarlo **siempre**: los traductores automáticos rompen los
`{0}`, los `\n` y los `&amp;`, y no distinguen gerundios de sustantivos ni el género. Una traducción
automática sin revisar es peor que ninguna, porque nadie la vuelve a mirar.

---

¿Algo no se entiende o te atascaste? Avisa — si esta guía no alcanzó, es un problema de la guía.
