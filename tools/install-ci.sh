#! /bin/sh

# This link changes from time to time. I haven't found a reliable hosted installer package for doing regular
# installs like this. You will probably need to grab a current link from: http://unity3d.com/get-unity/download/archive
echo 'Downloading from Unity package...'
curl -o Unity.pkg http://netstorage.unity3d.com/unity/a6d8d714de6f/MacEditorInstaller/Unity-5.4.0f3.pkg

echo 'Installing Unity.pkg'
installer -dumplog -package Unity.pkg -target /