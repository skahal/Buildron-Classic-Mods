@echo Compiling mods C# class libraries...
MSBuild.exe ..\src\Solution\Buildron.ClassicMods.sln /t:rebuild /p:Configuration=CI

if "%errorlevel%"=="0" GOTO SUCCESS

GOTO FAILURE

:SUCCESS
buildModAssets.cmd BuildMod
buildModAssets.cmd EasterEggMod
buildModAssets.cmd EnvironmentMod
buildModAssets.cmd UserMod

:FAILURE
@echo FAILED.