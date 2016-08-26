echo ================[ Building mods for $1

echo ================[ Compiling mods C# class libraries...
xbuild ../src/Solution/Buildron.ClassicMods.sln /verbosity:quiet /t:rebuild /p:Configuration=DEV >/dev/null

echo ================[ Starting mods assets building...
./buildModAssets.sh BuildMod $1
./buildModAssets.sh CameraMod $1
./buildModAssets.sh EasterEggMod $1
./buildModAssets.sh EnvironmentMod $1
./buildModAssets.sh SoundMod $1
./buildModAssets.sh UserMod $1

echo ================[  Build mods done.