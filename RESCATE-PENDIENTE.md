# El rescate de traducciones huérfanas

Cuando un mod se actualiza, el extractor cruza la extracción nueva con lo que ya estaba
traducido. Cruza **por identidad**: qué def, qué campo. Si el mod movió un nodo de lugar,
hay un segundo pase que lo busca **por el texto original en inglés y el mismo campo**.

Entre las dos cosas se recuperaron 7644 traducciones en la corrida de los 246 mods.

## Lo que ese segundo pase no toca, y por qué

Exige que coincida el campo. Sin esa guarda, un `label` sin traducir se llevaría la
traducción huérfana de un `labelFemale`: en inglés los dos dicen `hunter`, en español uno
es «cazador» y el otro «cazadora». Quedaría mal puesta y **nadie la revisaría**, porque
solo se mira lo que dice `TODO`.

La guarda se queda. Equivocarse en silencio es peor que pedir la traducción de nuevo.

## El caso que la guarda deja afuera

Cuando un mod **renombra sus claves**, el campo cambia entero y la guarda bloquea todo,
aunque el inglés sea idéntico. Pasó dos veces:

```
[FSF] FrozenSnowFox Tweaks    DefInjected posicional  ->  Keyed con nombres propios
    XmlExtensions.SettingsMenuDef / FrozenSnowFoxTweaksSettings.settings.1.text
    Keyed / FSFTweaksModWarning

[DR] Auto Ability             claves en chino  ->  claves en inglés
    粘贴  ->  Paste
    复制  ->  Copy
```

En esos casos el mod no cambió el texto: cambió cómo lo nombra.

## Qué se hizo

Un rescate **manual**, de una sola vez, cruzando por el texto en inglés e ignorando el
campo: **635 traducciones en 18 mods**. No se relajó la regla general.

Cada reemplazo se hizo con dos condiciones:

- El inglés tiene **una sola** traducción posible entre las huérfanas. Con dos distintas no
  se toca nada: quedaron 11 así.
- La clave aparece **una sola vez** en su archivo, para no reemplazar la ocurrencia
  equivocada.

Los tres mods con más recuperado: FrozenSnowFox Tweaks (480), Auto Ability (34) y Ancient
hydroponic farm facilities (32).

## Si vuelve a pasar

El síntoma es un mod que aparece con muchos `TODO` y un `UNUSED.xml` grande a la vez, con
los mismos textos en inglés de los dos lados. Ahí conviene revisar si renombró sus claves
antes de traducir nada de nuevo.

La alternativa de fondo sería un segundo nivel en el rescate automático: primero con la
guarda del campo y, para lo que quede, sin ella pero solo si el inglés es único entre todas
las huérfanas. El caso peligroso —`label` contra `labelFemale`— es justamente el ambiguo,
así que seguiría bloqueado. Se decidió no hacerlo: lo rescatado por contenido conviene
mirarlo, y a mano queda revisado.
