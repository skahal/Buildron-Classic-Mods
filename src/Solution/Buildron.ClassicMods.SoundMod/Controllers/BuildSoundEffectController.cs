using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Buildron.Domain.Builds;
using Buildron.Domain.Mods;

namespace Buildron.ClassicMods.SoundMod.Controllers
{
    public class BuildSoundEffectController : MonoBehaviour
    {
        #region Fields
        private static readonly List<AudioClip> s_sounds = new List<AudioClip>();
		private IModContext m_ctx;
        #endregion

        #region Methods
        private void Start()
        {
			m_ctx = Mod.Context;

		    LoadSounds();
			m_ctx.GameObjectsPool.CreatePool ("AudioSource", () => {
				return m_ctx.GameObjects.Create<AudioSource>().gameObject;
			});
			m_ctx.BuildFound += (sender, e) => PlayAudio(e.Build);
			m_ctx.BuildStatusChanged += (sender, e) => PlayAudio(e.Build);                
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
			var soundFiles = m_ctx.FileSystem.SearchFiles("*.wav");
			m_ctx.Log.Debug("Found {0} sound files on folder mod folder and subfolders.", soundFiles.Length);

            foreach (var file in soundFiles)
            {
                var filename = "file://" + file;
                filename = filename.Replace(@"\", "/");

                var www = new WWW(filename);
                yield return www;
                var clip = www.GetAudioClip(true);
                clip.name = filename;

				m_ctx.Log.Debug("Sound file loaded: {0}", clip.name);
                s_sounds.Add(clip);
            }
        }

        private void PlayAudio(IBuild build)
		{  
			if ((build.IsFailed()
			|| build.IsQueued()
			|| build.IsSuccess()
			|| build.Status == BuildStatus.Running) // Just first running status.
			&& build.TriggeredBy != null)
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

				m_ctx.Log.Debug("Found {0} sounds for user {1} and status {2}.", availableSounds.Count, username, status);

                if (availableSounds.Count > 0)
                {
                    var audioToPlay = availableSounds[Random.Range(0, availableSounds.Count)];

					var source = m_ctx
						.GameObjectsPool
						.GetGameObject("AudioSource", audioToPlay.length + 1) // +1 second.
						.GetComponent<AudioSource>();
                    
					source.volume = 1f;
                    source.clip = audioToPlay;
                    source.Play();                    
                }
            }
        }
        #endregion
    }
}