@echo Building %1 mod assets...
echo on
@"C:\Program Files\Unity\Editor\Unity.exe" -projectPath  C:\projects\buildron-classic-mods\src\Buildron.ClassicMods.%1 -username "%2" -password "%3" -serial %4 -quit -batchmode -executeMethod ModBuilder.BuildFromCommandLine  C:\projects\buildron-classic-mods\build StandaloneWindows