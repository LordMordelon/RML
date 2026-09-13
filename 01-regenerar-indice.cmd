@echo off
REM Regenera LoadFolders.xml y ModList.tsv a partir de los LoadFolders.Build.yaml, y
REM deja al dia la copia liviana de output\ (lo mismo que 02-armar-copia-limpia.cmd).
REM
REM Con la traduccion rapida no hace falta: el extractor ya lo corre al terminar.
REM Este script es para los cambios hechos a mano, que es cuando nadie lo corre:
REM editar una traduccion o un yaml, borrar una carpeta de Data, o tocar
REM supportedVersions en el About.xml. Como Mods\RML enlaza a output\, un cambio a
REM mano en Data no se ve en el juego hasta correr esto.

chcp 65001 > nul
cd /d "%~dp0"

dotnet run --project "Source\LoadFoldersBuilder" -c Release -- -build

if errorlevel 1 (
    echo.
    echo No se pudo regenerar el indice. El error esta arriba.
    pause
    exit /b 1
)
