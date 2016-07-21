using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buildron.Domain.Builds;

namespace Buildron.ClassicMods.SoundMod.Controllers
{
    [RequireComponent(typeof(AudioSource))]
    public class BuildSoundEffectController : MonoBehaviour
    {
        #region Fields
        private static List<AudioClip> s_sounds = new List<AudioClip>();
        #endregion

        #region Methods
        private void Start()
        {
            if (Mod.Context.CIServer.FxSoundsEnabled)
            {
                LoadSounds();

                Mod.Context.BuildStatusChanged += (sender, e) => {
                    var build = e.Build;

                    if (build.IsFailed()
                    || build.IsQueued()
                    || build.IsSuccess()
                    || build.Status == BuildStatus.Running) // Just first running status.
                    {
                        PlayAudio(build);
                    }
                };
                
            }
            else
            {
                Destroy(this);
            }
        }

        private void LoadSounds()
        {
            if (s_sounds.Count == 0)
            {
                StartCoroutine(LoadSound());
            }
        }

        private IEnumerator LoadSound()
        {
            var folderPath = UnityEngine.Application.dataPath.Substring(0, UnityEngine.Application.dataPath.LastIndexOf("/")) + "/mods/sounds/current/";
            folderPath = FixPath(folderPath);
            var soundFiles = Directory.GetFiles(folderPath, "*.wav", SearchOption.AllDirectories);

            Mod.Context.Log.Debug("Found {0} sound files on folder {1} and subfolders.", soundFiles.Length, folderPath);

            foreach (var file in soundFiles)
            {
                var filename = "file://" + file;
                filename = FixPath(filename);

                var www = new WWW(filename);
                yield return www;
                var clip = www.GetAudioClip(true);
                clip.name = filename;

                Mod.Context.Log.Debug("Sound file loaded: {0}", clip.name);
                s_sounds.Add(clip);
            }
        }

        private string FixPath(string path)
        {
            return path.Replace(@"\", "/");
        }   

        private void PlayAudio(IBuild build)
        {
            if (build.TriggeredBy != null)
            {
                var username = build.TriggeredBy.UserName;
                var status = build.Status;

                var filter = "/{0}/{1}".With(username, status);
                var availableSounds = s_sounds.Where(s => s.name.Contains(filter)).ToList();

                if (availableSounds.Count == 0)
                {
                    filter = "/current/{0}".With(status);
                    availableSounds = s_sounds.Where(s => s.name.Contains(filter)).ToList();
                }

                Mod.Context.Log.Debug("Found {0} sounds for user {1} and status {2}.", availableSounds.Count, username, status);

                if (availableSounds.Count > 0)
                {
                    var source = GetComponent<AudioSource>();
                    source.volume = 1f;
                    source.clip = availableSounds[Random.Range(0, availableSounds.Count)];
                    source.Play();
                }
            }
        }
        #endregion
    }
}