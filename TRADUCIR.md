# Cómo traducir un mod

Antes de empezar, mira el **[GLOSARIO](GLOSARIO.md)**: dice cómo se traduce cada término del juego
según la traducción oficial (*mechanitor* es «mecanizador», *chemfuel* es «quimbustible»).

## Qué necesitas

- Una cuenta de [GitHub](https://github.com), [Git](https://git-scm.com/download/win) y
  [VS Code](https://code.visualstudio.com/), todo gratis.
- RimWorld con el mod que quieres traducir.
- El [extractor](https://github.com/LordMordelon/RimworldExtractor/releases/latest)
  (`RimworldExtractor-Portable.exe`). Windows avisa que no es seguro porque no está firmado:
  **Más información** → **Ejecutar de todas formas**.

## Los pasos

1. **Copia el repositorio.** En [github.com/LordMordelon/RML](https://github.com/LordMordelon/RML),
   botón **Fork**. Después, en VS Code: **Clonar repositorio Git** y pega la dirección de tu copia.
   Guárdalo en una carpeta de ruta corta, como `C:\RML`: algunos nombres de archivo son largos.
   Si Git se queja de rutas largas, corre una vez en la terminal de VS Code:
   `git config --global core.longpaths true`.
2. **Crea una rama** con el nombre del mod: en VS Code, abajo a la izquierda, clic en `master` →
   **Crear rama**.
3. **Extrae el mod.** Abre el extractor. La primera vez te pide la carpeta de RimWorld, la del
   Workshop (`...\steamapps\workshop\content\294100`) y la **carpeta del mod RML**: ahí va tu
   clon (`C:\RML`). Después **Elegir un mod y extraer**, deja **sin marcar** «Extraer en carpeta»,
   elige el mod y **Extraer los datos de traducción**. Escribe en `Data/` y conserva lo que ya
   estaba traducido; lo nuevo queda como `TODO`. Si al final avisa que no pudo regenerar el
   índice, no importa: GitHub lo hace solo.
4. **Traduce lo que cambió.** En VS Code, la vista **Control de código fuente** (Ctrl+Shift+G)
   muestra cada archivo modificado. Al abrir uno ves el antes y el después: reemplaza cada `TODO`
   de lo nuevo por el texto en español. Para ver todo lo que falta, Ctrl+Shift+F y busca `TODO`.
5. **Súbelo.** En esa misma vista escribe qué hiciste («Traducir Nombre del mod»), **Confirmar**
   y **Publicar rama**. GitHub te ofrece abrir un **pull request**: ábrelo. Lo reviso y lo uno.

No hace falta terminar: media traducción sirve. Lo que no alcances, déjalo en `TODO`.

## Qué se toca y qué no

Cada texto está debajo de su original en inglés. **Solo se reemplaza el `TODO`:**

```xml
<!-- EN: Storage -->
<BuildingsNeatStorage.label>Almacenamiento</BuildingsNeatStorage.label>
```

- **No toques** las etiquetas, los comentarios `<!-- EN: ... -->`, los nombres de archivo,
  `LoadFolders.Build.yaml` ni `UNUSED.xml`.
- **Nunca copies el inglés** en lugar del `TODO`: parecería traducido y nadie lo volvería a mirar.
- Los `.txt` de `Strings` son listas de nombres: casi siempre se dejan como están.

## Lo que se copia tal cual

| En el original | Cómo va |
|---|---|
| `{0}`, `{PAWN_labelShort}`, `[PAWN_nameDef]` | Igual, con las mismas mayúsculas. Se pueden mover dentro de la frase |
| `\n` (salto de línea) | Igual. Nunca aprietes Enter dentro de una traducción |
| `&lt;color=#FF0000&gt;danger&lt;/color&gt;` | Se traduce solo lo de adentro: `&lt;color=#FF0000&gt;peligro&lt;/color&gt;` |
| `&`, `<`, `>` dentro del texto | Se escriben `&amp;`, `&lt;`, `&gt;`. Un `&` solo rompe el archivo |
| `order-&gt;halt` (reglas de gramática) | Lo de antes de `-&gt;` se copia: `order-&gt;alto`. Un `<li>` por cada comentario `EN` |

## La etiqueta dice cómo traducir

| Si termina en… | Es… |
|---|---|
| `pawnsPlural`, `labelPlural` | Plural: `members` → «miembros» |
| `labelFemale`, `titleFemale` | Femenino: `hunter` → «cazadora» |
| `gerund`, `gerundLabel` | Gerundio: `operating` → «operando», no «operación» |
| `adjective` | Adjetivo: `wooden` → «de madera» |

Algunos textos salen repetidos a propósito (el nombre de un escenario como `label` y como
`scenario.name`, o el de un arma en `verbs.0.label`): tradúcelos todos igual.

## Dudas comunes

- **¿Traductor automático?** Solo como punto de partida, y revisado: rompe los `{0}`, los `\n` y
  los `&amp;`, y confunde el género y los gerundios.
- **El texto no cabe en una ventana de otro mod:** avisa en el pull request en vez de acortarlo.
- **Algo no se entiende:** pregunta en un [issue](https://github.com/LordMordelon/RML/issues). Si
  esta guía no alcanzó, es un problema de la guía.
