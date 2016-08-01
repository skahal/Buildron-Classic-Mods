echo Building $1
/Applications/Unity/Unity.app/Contents/MacOS/Unity -projectPath /Users/giacomelli/Dropbox/Skahal/Apps/Buildron-Classic-Mods/src/Buildron.ClassicMods.$1 -quit -batchmode -executeMethod ModBuilder.BuildFromCommandLine /Users/giacomelli/Dropbox/Skahal/Apps/Buildron/build/Mods StandaloneOSXIntel
echo $1 done.
