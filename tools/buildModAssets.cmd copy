@echo off
@echo Building %1 mod assets...
@"C:\Program Files\Unity\Editor\Unity.exe" -projectPath %cd%\..\src\Buildron.ClassicMods.%1 -quit -batchmode -executeMethod ModBuilder.BuildFromCommandLine %cd%\..\build StandaloneWindows

if "%errorlevel%"=="0" GOTO SUCCESS
GOTO FAILURE

:SUCCESS
exit

:FAILURE
@echo %1 FAILED.
pause


