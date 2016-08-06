echo Compiling mods C# class libraries...
xbuild ../src/Solution/Buildron.ClassicMods.sln /t:rebuild /p:Configuration=DEV

echo Starting building mods assets...

./buildModAssets.sh BuildMod
./buildModAssets.sh CameraMod 
./buildModAssets.sh EasterEggMod
./buildModAssets.sh EnvironmentMod
./buildModAssets.sh SoundMod
./buildModAssets.sh UserMod

echo 'Logs from build'
cat $(pwd)/unity.log