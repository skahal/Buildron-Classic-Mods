echo Starting async building...
echo Please wait for all mods build done or all Unity instances be closed.
./buildMod.sh BuildMod &
./buildMod.sh CameraMod &
./buildMod.sh EasterEggMod &
./buildMod.sh EnvironmentMod &
./buildMod.sh SoundMod &
./buildMod.sh UserMod &