@echo off
REM Deja al dia la copia liviana del mod en "output\RimWorld Mod Latino": la que carga
REM el juego (Mods\RML enlaza ahi) y la que se sube al Workshop.
REM
REM Normalmente no hace falta correrlo: 01-regenerar-indice.cmd y el extractor, al
REM regenerar el indice, ya dejan la copia al dia. 03-subir-al-workshop.cmd lo corre
REM igual antes de subir, por las dudas.
REM
REM Lo hace LoadFoldersBuilder -copia (Source\LoadFoldersBuilder\CopiaLimpia.cs):
REM   - Solo About, Data, LoadFolders.xml, ModList.tsv y LICENSE.
REM   - Sin UNUSED.xml ni LoadFolders.Build.yaml, que el juego no lee.
REM   - Los XML sin comentarios: el original en ingles sirve en el repo, no al jugador.
REM   - Antes comprueba que ninguna ruta pase el limite de Windows instalada desde el
REM     Workshop (si pasa, el juego queda en pantalla negra) y no copia nada si pasa.
REM   - Despues comprueba que la copia tenga todos los archivos.
REM
REM Solo ASCII en este archivo: con chcp 65001, cmd lee corrido un .cmd con saltos
REM LF y caracteres de mas de un byte, y ejecuta pedazos de los comentarios.
REM
REM Con /sinpausa no espera una tecla al terminar: es como la llama
REM 03-subir-al-workshop.cmd.

setlocal
chcp 65001 > nul
cd /d "%~dp0"

set "PAUSA=pause"
if /i "%~1"=="/sinpausa" set "PAUSA=rem"

dotnet run --project "Source\LoadFoldersBuilder" -c Release -- -copia
if errorlevel 1 (
    echo.
    echo ERROR: no se armo la copia. El motivo esta arriba.
    %PAUSA%
    exit /b 1
)

echo.
echo Listo: output\RimWorld Mod Latino
%PAUSA%
exit /b 0
