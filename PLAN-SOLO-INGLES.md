# Plan: que un cambio solo en el inglés no abra la ventana

Pedido de Angel el 29-oct-2026, a partir de Toxin Warcasket: el autor pasó su inglés a ortografía estadounidense
(«synergises» → «synergizes», «armour» → «armor»), la reextracción cambió solo los comentarios `<!-- EN: ... -->` y
las traducciones quedaron iguales, porque ya estaban bien. En casos así, la ventana de RML no tiene que abrirse
sola al iniciar el juego para mostrar «Corregido» en un mod donde no cambió nada en español.

**Estado: hecho el 29-oct-2026, sin subir.** Aprobado por Angel ese mismo día, también lo de dejar afuera las
notas `.md`, el `UNUSED.xml` y el yaml. Falta la prueba en el juego (ver «Hecho»).

## Por qué pasa

La pestaña «Novedades» sale de `Novedades.tsv`, que arma el builder (`Source/LoadFoldersBuilder/Novedades.cs`).
Ahí un mod cuenta como cambiado por **cualquier** archivo tocado en su carpeta:

1. **La marca «c»** (cambió desde la última subida al Workshop) sale de `git diff` sobre `Data/`: un comentario EN,
   un `.md` de notas, el `UNUSED.xml` o el yaml la ponen igual que una traducción nueva. La cuenta de textos de esa
   fila sí mira solo el español (`TextosDeTraduccion.Comparar`), así que en Toxin Warcasket queda «c» con la cuenta
   vacía: «Corregido» y, en «Ver cambios», nada.
2. **La fecha del último cambio** sale de `UltimosCambios.Historial`, que cuenta todo commit que tocó la carpeta.
   El juego recuerda lo visto por esa fecha (`ClaveDeCambio`), así que una fecha nueva vuelve a ser «sin ver».

En el juego hay una defensa a medias: si el jugador ya tiene la copia de sus textos en el PC
(`CambiosEnElPc`, PLAN-CAMBIOS-LOCALES.md), esa comparación mira solo el español y descarta el mod. Pero no
alcanza: la primera vez no hay copia, y al iniciar la cuenta corre en segundo plano y, si no terminó, la ventana
usa lo del builder.

## Qué se cambia

1. **Builder: un mod cambió solo si cambió su español.** En `Novedades.cs`, la marca «c» se pone solo si
   `TextosCambiados` trae algo (texto cambiado, nuevo o quitado). Lo demás (inglés, notas, `UNUSED.xml`, yaml)
   no marca nada.
2. **Builder: la fecha y el tipo de `Novedades.tsv`, del último cambio en español.** Se recorren los commits que
   tocaron la carpeta del más nuevo al más viejo y se toma el primero que cambió algún texto en español (casi
   siempre el primero o el segundo, con un tope para no recorrer todo el historial). Solo para `Novedades.tsv`.
3. **Juego, como red:** en `NovedadesRml`, una fila «c» cuya cuenta de textos viene y está vacía no sale como
   novedad ni abre la ventana. Cubre un `Novedades.tsv` armado antes del arreglo. Sin conexión con Steam la cuenta
   no viene, y ahí todo sigue como hoy.

**No cambia:** la fecha de traducción de `ModList.tsv` y de la página. Ahí conviene que una reextracción sin
cambios en español sí cuente: es lo que dice que el mod se revisó después de su actualización en Steam y le saca
«posiblemente atrasada».

## Pruebas

- `Source/RmlVentana.Tests`: una fila «c» con cuenta vacía no queda en `Pendientes` ni hace `DebeAparecerSola`;
  con la cuenta en «1,0,0» sí; sin la columna (sin Steam), como hoy.
- `Source/LoadFoldersBuilder.Tests`: un XML que cambia solo el comentario EN no da textos cambiados ni marca;
  uno que cambia el valor, sí.
- Las pruebas nuevas primero, viéndolas fallar; después el código. Al final, `-copia` y mirar en
  `output/Novedades.tsv` que Toxin Warcasket ya no lleve la «c».

## Decisiones de Angel (29-oct-2026)

1. Plan aprobado.
2. Los cambios en `.md` de notas, `UNUSED.xml` o el yaml también quedan afuera: ninguno se ve en el juego.

## Hecho (29-oct-2026)

- **Builder:** `UltimosCambios.Historial` da además los commits de cada carpeta con sus archivos, de la misma pasada
  por `git log`. `Novedades.UltimoCambioEnEspanol` los recorre hacia atrás, todas las carpetas a la vez y con un
  `git cat-file` por vuelta (tope 20), y se queda con el primero que cambió un texto en español
  (`Novedades.CambiosDeTexto`). Si ninguno lo cambió, la fecha es la de cuando apareció. La marca «c» sale de
  `MarcaDeCambio`: solo con textos cambiados. `-copia` tardó 17 s.
- **Juego:** `EntradaDeNovedades.SinCambiosEnEspanol`; `Pendientes` no cuenta esa fila como cambio.
- **Pruebas:** las cinco del plan, vistas fallando antes del código. La de «Novedades» que daba por buena una fila
  «c» sin textos se ajustó.
- **Resultado en `output/Novedades.tsv`:** Toxin Warcasket queda sin «c» y con su último cambio en español
  (25-oct). Otros 25 mods cambiaron de fecha; se revisaron a mano Citadel, Alpha Prefabs, Hegemony, Buffs 'n
  Nerfs, ReGrowth 2, Synthetic y VAE Endangered: en todos, los commits que se dejan de contar tocaron solo
  glosarios o planes `.md`, el inglés, o reacomodaron un `Patches` con los mismos textos.
- **Falta:** probar en el juego que la ventana no se abre sola con la versión que se suba.
