@echo off
REM Deja una copia limpia del mod en "salida\RimWorld Mod Latino", con solo lo que
REM RimWorld necesita. Es la carpeta que se sube al Workshop: Steam sube la carpeta
REM tal cual la encuentra, y el repo entero lleva .git y Source con sus binarios.
REM Para subirla desde el juego, usar subir.cmd, que la llama.
REM
REM El .zip de GitHub Releases lo arma la Action con esta misma lista. Si cambia
REM aca, cambiarla alla.
REM
REM Se usa robocopy y no xcopy: las rutas de Data pasan los 254 caracteres que
REM aguanta xcopy —nombre de autor, nombre del mod, la carpeta del idioma— y xcopy
REM las descarta sin decir nada. La primera version de este script perdia 2666 de
REM 3527 archivos en silencio.
REM
REM Con /sinpausa no espera una tecla al terminar: es como la llama subir.cmd.

setlocal
chcp 65001 > nul
cd /d "%~dp0"

set "PAUSA=pause"
if /i "%~1"=="/sinpausa" set "PAUSA=rem"

if not exist "LoadFolders.xml" (
    echo No esta el LoadFolders.xml. Corre actualizar.cmd primero.
    %PAUSA%
    exit /b 1
)

REM Que ninguna ruta pase el limite de Windows instalada desde el Workshop. Si pasa,
REM el mod deja el juego en pantalla negra a quien tenga Steam en su carpeta por
REM defecto, y en esta PC no se nota. Ver Source\LoadFoldersBuilder\Rutas.cs.
dotnet run --project "Source\LoadFoldersBuilder" -c Release -- -rutas
if errorlevel 1 (
    echo.
    echo ERROR: hay rutas que corregir antes de publicar. La lista esta arriba.
    %PAUSA%
    exit /b 1
)

set "DESTINO=salida\RimWorld Mod Latino"
if exist "salida" rmdir /s /q "salida"
mkdir "%DESTINO%"

robocopy "About" "%DESTINO%\About" /e /njh /njs /ndl /nc /ns /np > nul
robocopy "Data"  "%DESTINO%\Data"  /e /njh /njs /ndl /nc /ns /np > nul
if %errorlevel% geq 8 (
    echo Fallo la copia de Data.
    %PAUSA%
    exit /b 1
)

copy "LoadFolders.xml" "%DESTINO%\" > nul
copy "ModList.tsv"     "%DESTINO%\" > nul
copy "LICENSE"         "%DESTINO%\" > nul

REM Que la copia tenga los mismos archivos que el origen. Sin esto, una copia
REM incompleta se publica igual y el mod sale con traducciones faltantes.
for /f %%A in ('dir /s /b /a-d "Data" ^| find /c /v ""') do set ORIGEN=%%A
for /f %%A in ('dir /s /b /a-d "%DESTINO%\Data" ^| find /c /v ""') do set COPIA=%%A

if not "%ORIGEN%"=="%COPIA%" (
    echo.
    echo ERROR: se copiaron %COPIA% archivos de %ORIGEN%. La copia esta incompleta.
    %PAUSA%
    exit /b 1
)

echo.
echo Listo: %DESTINO%   ^(%COPIA% archivos^)
%PAUSA%
exit /b 0
