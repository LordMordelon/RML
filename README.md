# RimWorld Mod Latino (RML)

Traducciones al **español latinoamericano** de mods de RimWorld, todas en un solo mod. Cada
traducción se aplica solo si su mod está activo, así que puedes dejarlo puesto siempre.

- **[Lista de mods traducidos](https://lordmordelon.github.io/RML/)**
- **[Mods más pedidos](https://lordmordelon.github.io/RML/solicitudes.html)**

## Instalar

1. Suscríbete en el [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3541009729).
   Sin Steam, baja el [zip de la última versión](https://github.com/LordMordelon/RML/releases/latest/download/RimWorld-Mod-Latino.zip)
   y descomprímelo en la carpeta `Mods` de RimWorld.
2. Ponlo **último** en la lista de mods: así su traducción le gana a cualquier otra.
3. Elige el idioma **Español (Latinoamérica)**.

Al iniciar, una ventana muestra qué mods activos están traducidos. Para pedir uno que falta, aprieta
**«Solicitar»** junto al mod (la ventana se abre también en Opciones → Configuración de mods).

## Colaborar

Para traducir un mod, sigue **[TRADUCIR.md](TRADUCIR.md)**: copias el repositorio, extraes el mod,
traduces lo nuevo en VS Code y lo subes para que se revise. Los términos están en el
**[GLOSARIO](GLOSARIO.md)**. Para avisar de un error, abre un
[issue](https://github.com/LordMordelon/RML/issues) o comenta en Steam.

## Para el mantenedor

Las traducciones las produce el [extractor](https://github.com/LordMordelon/RimworldExtractor), clonado
como carpeta hermana de este repo (`RML/` y `RimworldExtractor/`), con el .NET SDK 10.

1. **Traducir un mod:** en el extractor, «Elegir un mod y extraer» sin marcar «Extraer en carpeta».
   Después reemplaza los `TODO` de los XML.
2. **Después de una actualización del juego:** «Actualizar todo RML».
3. **Cambios a mano en `Data/`:** corre `01-regenerar-indice.cmd`; sin eso no se ven en el juego.
4. **Antes de commitear:** revisa el diff como dice
   [AGENTS.md](AGENTS.md#antes-de-commitear-mirar-el-diff).
5. **Publicar:** las versiones de GitHub salen solas con cada push. Al Workshop se sube con
   `03-subir-al-workshop.cmd`: muestra la nota de cambios («Añadidos:» y los mods nuevos desde la subida
   anterior), pregunta y sube con SteamCMD, sin abrir el juego. Sin SteamCMD configurado, deja la nota
   copiada y se sube desde el juego.

   Para configurar SteamCMD, una sola vez: descomprime
   [steamcmd.zip](https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip) en `C:\steamcmd`, fuera
   de esta carpeta y de cualquiera que se sincronice; define las dos variables y abre una consola nueva:

   ```
   setx STEAMCMD "C:\steamcmd\steamcmd.exe"
   setx STEAM_USUARIO "<tu usuario de Steam>"
   ```

   Después corre `C:\steamcmd\steamcmd.exe +login <tu usuario> +quit`: se actualiza, pide la contraseña
   y el código de Steam Guard, y guarda la sesión en esa carpeta. Quien la copie puede subir con tu
   cuenta, así que no la compartas. La cuenta tiene que ser la dueña del mod en el Workshop.

Cómo está organizado y qué no hay que tocar: [AGENTS.md](AGENTS.md). Qué cambia respecto de
[RimWorld Mod Korean](https://github.com/RimWorldKorea/RMK), del que deriva: [CAMBIOS.md](CAMBIOS.md).

## Licencia

GPL-3.0. Las herramientas de `Source/` derivan de [RimWorldKorea/RMK](https://github.com/RimWorldKorea/RMK),
también GPL-3.0. Los derechos sobre RimWorld y el contenido traducido pertenecen a Ludeon Studios Inc.
según el EULA de RimWorld.
