@echo Compiling mods C# class libraries...
@C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe ..\src\Solution\Buildron.ClassicMods.sln /t:rebuild /p:Configuration=DEV

@if "%errorlevel%"=="0" GOTO SUCCESS

@GOTO FAILURE

:SUCCESS
@echo Starting async building...
@echo Please wait for all mods build done or all Unity instances be closed.
@call start buildModAssets.cmd BuildMod
@call start buildModAssets.cmd EasterEggMod
@call start buildModAssets.cmd EnvironmentMod
@call start buildModAssets.cmd UserMod

:FAILURE
@echo FAILED.