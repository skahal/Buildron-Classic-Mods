@echo Compiling mods C# class libraries...
MSBuild.exe ..\src\Solution\Buildron.ClassicMods.sln /t:rebuild /p:Configuration=CI

if "%errorlevel%"=="0" GOTO SUCCESS

GOTO FAILURE

:SUCCESS
buildModAssets-CI.cmd BuildMod
buildModAssets-CI.cmd EasterEggMod
buildModAssets-CI.cmd EnvironmentMod
buildModAssets-CI.cmd UserMod

:FAILURE
@echo FAILED.