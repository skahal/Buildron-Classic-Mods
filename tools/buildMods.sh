echo Compiling mods C# class libraries...
xbuild ../src/Solution/Buildron.ClassicMods.sln /t:rebuild /p:Configuration=DEV

echo Starting async building...
echo Please wait for all mods build done or all Unity instances be closed.

./buildModAssets.sh BuildMod &
./buildModAssets.sh CameraMod &
./buildModAssets.sh EasterEggMod &
./buildModAssets.sh EnvironmentMod &
./buildModAssets.sh SoundMod &
./buildModAssets.sh UserMod &