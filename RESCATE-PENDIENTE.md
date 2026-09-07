# Traducciones que quedaron sin rescatar

Cuando un mod se actualiza, el extractor cruza la extracción nueva con lo que ya estaba
traducido. Cruza primero por identidad —qué def, qué campo— y, para lo que queda sin
traducir, hace un segundo pase que busca entre las traducciones huérfanas una con **el
mismo texto en inglés y el mismo campo**. Eso recuperó **7644 traducciones** en la última
corrida.

Este documento anota lo que ese segundo pase **decidió no tocar**: casos donde el inglés
coincide pero el campo no.

## Por qué existe la guarda del campo

Sin ella, un `label` sin traducir se llevaría la traducción huérfana de un `labelFemale`.
En inglés los dos dicen `hunter`; en español uno es «cazador» y el otro «cazadora». La
traducción quedaría mal puesta y **nadie la volvería a mirar**, porque solo se revisa lo
que está marcado como `TODO`.

Ese es el criterio: equivocarse en silencio es peor que pedir la traducción de nuevo.

## Lo que cuesta

**626 entradas en 20 mods** se recuperarían si se relajara la guarda. Están muy
concentradas: 480 son de un solo mod.

| Mod | Rescatables sin la guarda | Ambiguas | Total sin traducir |
|---|---:|---:|---:|
| [FSF] FrozenSnowFox Tweaks - 2893432492 | 480 | 0 | 605 |
| Ancient hydroponic farm facilities - 3075384838 | 32 | 6 | 46 |
| [FSF] Advanced Bionics Expansion - 2006925330 | 22 | 0 | 67 |
| Vanilla Factions Expanded - Deserters - 3025493377 | 18 | 0 | 57 |
| Vanilla Quests Expanded - The Generator - 3411401573 | 18 | 0 | 27 |
| Vanilla Factions Expanded - Empire - 2938820380 | 14 | 0 | 35 |
| Zoology Realistic Animal Overhaul - 3679396881 | 8 | 9 | 336 |
| Vanilla Factions Expanded - Insectoids 2 - 3309003431 | 6 | 0 | 16 |
| The Dead Man's Switch - AncientCorps - 3469398006 | 4 | 0 | 60 |
| The Dead Man's Switch - 3121742525 | 3 | 0 | 143 |
| Altered Carbon 2 ReSleeved - 2196278117 | 3 | 0 | 64 |
| Vanilla Animals Expanded - 2871933948 | 2 | 5 | 119 |
| Vanilla Furniture Expanded - Props and Decor - 2102143149 | 2 | 0 | 1330 |
| Alpha Books - 3403180654 | 2 | 0 | 71 |
| Vanilla Psycasts Expanded - 2842502659 | 2 | 0 | 38 |
| Vanilla Races Expanded - Android - 2975771801 | 2 | 0 | 21 |
| Ushankas Glittertech Expansion - 3522676478 | 2 | 0 | 12 |
| Vanilla Races Expanded - Phytokin - 2927323805 | 2 | 0 | 7 |
| Mechanitor Orbital Platform - 3523146525 | 2 | 0 | 4 |
| Trader ships - 2046222331 | 2 | 0 | 3 |
La columna **Ambiguas** son casos donde el mismo inglés tiene dos traducciones distintas
entre las huérfanas. Esos no se rescatan ni relajando la guarda: no hay forma de saber cuál
corresponde.

## El caso grande

`[FSF] FrozenSnowFox Tweaks` aporta 480 de las 626. El mod **migró sus textos de
`DefInjected` posicional a `Keyed` con nombres propios**:

```
antes:  XmlExtensions.SettingsMenuDef / FrozenSnowFoxTweaksSettings.settings.1.text
ahora:  Keyed / FSFTweaksModWarning          <- mismo inglés exacto
```

El campo pasó de `text` a `FSFTweaksModWarning`, así que la guarda lo bloqueó. Es una
migración legítima y frecuente, y es el caso que más fuerte argumenta a favor de relajar.

## Si algún día se relaja

La forma segura son dos niveles: primero con la guarda, y para lo que quede, sin ella pero
solo cuando el inglés sea único entre **todas** las huérfanas del mod. El caso peligroso
—`label` contra `labelFemale`— es justamente el ambiguo, así que seguiría bloqueado.

Lo rescatado por el segundo nivel habría que listarlo aparte para revisarlo a ojo, porque
se decidiría por contenido y no por identidad.
