using UnityEngine;

namespace Valve.VR.InteractionSystem {
	public class SoundPlayOneshot : MonoBehaviour {
		[SerializeField] private AudioClip[] waveFiles;

		[SerializeField] private float volMin;
		[SerializeField] private float volMax;

		[SerializeField] private float pitchMin;
		[SerializeField] private float pitchMax;

		[SerializeField] private bool playOnAwake;
        [SerializeField] private float soundFalloff = 5f;

		private AudioSource thisAudioSource;

		private void Awake() {
			thisAudioSource = GetComponent<AudioSource>();

            if (thisAudioSource.spatialBlend != 1f)
                thisAudioSource.spatialBlend = 1f;

            if (thisAudioSource.maxDistance != soundFalloff)
                thisAudioSource.maxDistance = soundFalloff;

            if (playOnAwake)
				Play();
		}

		public void Play(bool clipAtPoint = false) {
			if (thisAudioSource != null && thisAudioSource.isActiveAndEnabled && !Util.IsNullOrEmpty(waveFiles)) {
				//randomly apply a volume between the volume min max
				thisAudioSource.volume = Random.Range(volMin, volMax);

				//randomly apply a pitch between the pitch min max
				thisAudioSource.pitch = Random.Range(pitchMin, pitchMax);

                // play the sound

                AudioClip audioClip = waveFiles[Random.Range(0, waveFiles.Length)];

                //if the audio clip is null check to see if we can find one that isn't
                if(audioClip == null)
                {
                    for (int i = 0; i < waveFiles.Length; i++)
                    {
                        if (waveFiles[i] == null)
                            continue;

                        audioClip = waveFiles[i];
                        break;
                    }
                }

                //if it is still null at this point just break out
                if (audioClip == null)
                    return;

                if (clipAtPoint)
                    AudioSource.PlayClipAtPoint(audioClip, transform.position);
                else
                    thisAudioSource.PlayOneShot(audioClip);
			}
		}

		public void Pause() {
			if (thisAudioSource != null)
				thisAudioSource.Pause();
		}

		public void UnPause() {
			if (thisAudioSource != null)
				thisAudioSource.UnPause();
		}
	}
}
