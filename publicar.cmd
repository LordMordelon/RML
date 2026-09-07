@echo off
REM Deja una copia limpia del mod en "salida\RimWorld Mod Latino", con solo lo que
REM RimWorld necesita. Es la carpeta que se copia a Mods\ para subir al Workshop
REM desde el juego: Steam sube la carpeta tal cual la encuentra.
REM
REM El .zip de GitHub Releases lo arma la Action con esta misma lista. Si cambia
REM aca, cambiarla alla.
REM
REM Se usa robocopy y no xcopy: las rutas de Data pasan los 254 caracteres que
REM aguanta xcopy —nombre de autor, nombre del mod, la carpeta del idioma con sus
REM parentesis— y xcopy las descarta sin decir nada. La primera version de este
REM script perdia 2666 de 3527 archivos en silencio.

setlocal
cd /d "%~dp0"

if not exist "LoadFolders.xml" (
    echo No esta el LoadFolders.xml. Corre actualizar.cmd primero.
    pause
    exit /b 1
)

set "DESTINO=salida\RimWorld Mod Latino"
if exist "salida" rmdir /s /q "salida"
mkdir "%DESTINO%"

robocopy "About" "%DESTINO%\About" /e /njh /njs /ndl /nc /ns /np > nul
robocopy "Data"  "%DESTINO%\Data"  /e /njh /njs /ndl /nc /ns /np > nul
if %errorlevel% geq 8 (
    echo Fallo la copia de Data.
    pause
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
    pause
    exit /b 1
)

echo.
echo Listo: %DESTINO%   ^(%COPIA% archivos^)
echo Copiala a la carpeta Mods\ de RimWorld y subila desde el juego.
echo.
pause
