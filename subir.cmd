@echo off
REM Sube al Workshop la copia limpia del mod, y no el repo entero.
REM
REM Para probar las traducciones en vivo, Mods\RML es un enlace (junction) a este
REM repo. Pero RimWorld sube la carpeta tal cual la encuentra, y asi al Workshop iba
REM todo: .git, Source con sus binarios. Este script arma la copia limpia con
REM publicar.cmd, apunta el enlace a esa copia mientras se sube desde el juego, y
REM despues lo devuelve al repo.
REM
REM Necesita la variable RIMWORLD_MODS con la carpeta Mods de RimWorld. Se define una
REM sola vez; asi ninguna ruta de una PC queda escrita en el repo.

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
set "REPO=%CD%"
set "COPIA=%CD%\salida\RimWorld Mod Latino"

REM Con el juego abierto, RimWorld ya leyo la carpeta vieja y subiria esa.
tasklist /fi "imagename eq RimWorldWin64.exe" | find /i "RimWorldWin64.exe" > nul
if not errorlevel 1 (
    echo Cierra RimWorld antes de empezar.
    pause
    exit /b 1
)

REM rmdir sobre un enlace quita solo el enlace. Sobre una carpeta de verdad la borraria,
REM asi que si no es un enlace no se toca.
fsutil reparsepoint query "%ENLACE%" > nul 2>&1
if errorlevel 1 (
    echo "%ENLACE%" no existe o no es un enlace. No se toca nada.
    echo Se crea con:  mklink /J "%ENLACE%" "%REPO%"
    pause
    exit /b 1
)

REM Por su ruta y no por el nombre solo: con NoDefaultCurrentDirectoryInExePath
REM definida, cmd no busca comandos en la carpeta actual y no lo encuentra.
call "%~dp0publicar.cmd" /sinpausa
if errorlevel 1 (
    pause
    exit /b 1
)

rmdir "%ENLACE%"
mklink /J "%ENLACE%" "%COPIA%" > nul
if errorlevel 1 (
    echo No se pudo apuntar el enlace a la copia limpia. Se devuelve al repo.
    mklink /J "%ENLACE%" "%REPO%" > nul
    pause
    exit /b 1
)

echo.
echo Mods\RML apunta ahora a la copia limpia.
echo   1. Abre RimWorld y sube RML desde el juego.
echo   2. Cierra RimWorld.
echo   3. Vuelve aca y presiona una tecla para devolver el enlace al repo.
echo.
echo No cierres esta ventana: si la cierras, el enlace queda apuntando a la copia.
pause

rmdir "%ENLACE%"
mklink /J "%ENLACE%" "%REPO%" > nul
if errorlevel 1 (
    echo.
    echo ERROR: no se pudo devolver el enlace. Rehazlo a mano con:
    echo     mklink /J "%ENLACE%" "%REPO%"
    pause
    exit /b 1
)

echo.
echo Listo: Mods\RML vuelve a apuntar al repo.
pause
