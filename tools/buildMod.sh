echo Building $1
/Applications/Unity/Unity.app/Contents/MacOS/Unity -projectPath $PWD/../src/Buildron.ClassicMods.$1 -quit -batchmode -executeMethod ModBuilder.BuildFromCommandLine $PWD/../../Buildron/build/Mods StandaloneOSXIntel
echo $1 done.
