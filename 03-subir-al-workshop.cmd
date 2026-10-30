@echo off
REM Sube RML al Workshop: deja al dia la copia liviana de output\, se asegura de que
REM Mods\RML enlace a ella y la sube con SteamCMD, con la nota de cambios desde la
REM subida anterior. Pregunta antes de subir.
REM
REM SteamCMD necesita dos variables, definidas una sola vez (ver README): STEAMCMD con
REM la ruta de steamcmd.exe y STEAM_USUARIO con el usuario de Steam. Sin ellas, deja la
REM nota en el portapapeles y se sube desde el juego, como antes.
REM
REM Mods\RML enlaza (junction) a "output\RimWorld Mod Latino" y no al repo: el juego
REM carga exactamente lo que se sube, sin .git, Source, UNUSED.xml ni comentarios.
REM La copia se rehace sola cada vez que se regenera el indice, asi que una
REM traduccion rapida del extractor se ve en el juego sin hacer nada mas. Un cambio
REM hecho a mano en Data se ve despues de correr 01-regenerar-indice.cmd.
REM
REM La primera vez crea el enlace, o lo cambia si todavia apunta al repo.
REM
REM Necesita la variable RIMWORLD_MODS con la carpeta Mods de RimWorld. Se define una
REM sola vez; asi ninguna ruta de una PC queda escrita en el repo.
REM
REM Solo ASCII en este archivo, por lo mismo que 02-armar-copia-limpia.cmd.

setlocal
chcp 65001 > nul
cd /d "%~dp0"

if "%RIMWORLD_MODS%"=="" (
    echo Falta la variable RIMWORLD_MODS con la carpeta Mods de RimWorld.
    echo Definela una sola vez, por ejemplo:
    echo.
    echo     setx RIMWORLD_MODS "D:\SteamLibrary\steamapps\common\RimWorld\Mods"
    echo.
    echo y abre una consola nueva.
    pause
    exit /b 1
)

set "ENLACE=%RIMWORLD_MODS%\RML"
set "COPIA=%CD%\output\RimWorld Mod Latino"

REM Por su ruta y no por el nombre solo: con NoDefaultCurrentDirectoryInExePath
REM definida, cmd no busca comandos en la carpeta actual y no lo encuentra.
call "%~dp002-armar-copia-limpia.cmd" /sinpausa
if errorlevel 1 (
    pause
    exit /b 1
)

REM Si ya enlaza a la copia, no hay nada que tocar.
fsutil reparsepoint query "%ENLACE%" 2> nul | find /i "%COPIA%" > nul
if not errorlevel 1 (
    echo.
    echo Mods\RML ya apunta a output.
    goto :subir
)

REM Una carpeta de verdad no se toca: rmdir sobre ella la borraria.
if exist "%ENLACE%" (
    fsutil reparsepoint query "%ENLACE%" > nul 2>&1
    if errorlevel 1 (
        echo "%ENLACE%" es una carpeta y no un enlace. No se toca nada.
        echo Muevela a otro lado y vuelve a correr este script.
        pause
        exit /b 1
    )
)

REM Cambiar el enlace con el juego abierto no sirve: ya leyo la carpeta vieja.
tasklist /fi "imagename eq RimWorldWin64.exe" | find /i "RimWorldWin64.exe" > nul
if not errorlevel 1 (
    echo Mods\RML no apunta a output. Cierra RimWorld para cambiarlo.
    pause
    exit /b 1
)

if exist "%ENLACE%" rmdir "%ENLACE%"
mklink /J "%ENLACE%" "%COPIA%" > nul
if errorlevel 1 (
    echo ERROR: no se pudo crear el enlace. Hazlo a mano con:
    echo     mklink /J "%ENLACE%" "%COPIA%"
    pause
    exit /b 1
)

echo.
echo Mods\RML apunta ahora a output.

:subir
if defined STEAMCMD if defined STEAM_USUARIO goto :steamcmd

REM Sin SteamCMD: la nota queda en nota-de-steam.txt y en el portapapeles, para pegarla en
REM la pagina del Workshop despues de subir desde el juego (ver NotaDeSteam.cs).
dotnet run --project "Source\LoadFoldersBuilder" -c Release -- -nota
if not errorlevel 1 (
    powershell -NoProfile -Command "Get-Content -Raw -Encoding UTF8 'nota-de-steam.txt' | Set-Clipboard"
    echo La nota de cambios quedo copiada: pegala en el Workshop despues de subir.
)
echo.
echo Abre RimWorld y sube RML desde el juego.
pause
exit /b 0

REM Con SteamCMD: -workshop rehace la copia, avisa lo sin commitear, muestra la nota,
REM pregunta y sube (ver SubidaWorkshop.cs). Si la sesion de Steam vencio, la pide aca.
:steamcmd
dotnet run --project "Source\LoadFoldersBuilder" -c Release -- -workshop
if errorlevel 1 (
    echo.
    echo No se subio al Workshop. El motivo esta arriba.
    pause
    exit /b 1
)
pause
exit /b 0
