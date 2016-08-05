@echo Building %1
@"C:\Program Files\Unity\Editor\Unity.exe" -projectPath %cd%\..\src\Buildron.ClassicMods.%1 -quit -batchmode -executeMethod ModBuilder.BuildFromCommandLine %cd%\..\..\Buildron\build\Mods StandaloneWindows
@echo %1 done.
pause
exit
