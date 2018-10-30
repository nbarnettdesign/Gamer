using UnityEngine;

namespace Valve.VR.InteractionSystem {
	[RequireComponent(typeof(AudioSource))]
	public class PlaySound : MonoBehaviour {
		[Tooltip("List of audio clips to play.")]
		[SerializeField] private AudioClip[] waveFile;
		[Tooltip("Stops the currently playing clip in the audioSource. Otherwise clips will overlap/mix.")]
		[SerializeField] private bool stopOnPlay;
		[Tooltip("After the audio clip finishes playing, disable the game object the sound is on.")]
		[SerializeField] private bool disableOnEnd;
		[Tooltip("Loop the sound after the wave file variation has been chosen.")]
		[SerializeField] private bool looping;
		[Tooltip("If the sound is looping and updating it's position every frame, stop the sound at the end of the wav/clip length. ")]
		[SerializeField] private bool stopOnEnd;
		[Tooltip("Start a wave file playing on awake, but after a delay.")]
		[SerializeField] private bool playOnAwakeWithDelay;
        [SerializeField] private float soundFalloff = 5f;

        [Header("Random Volume")]
		[SerializeField] private bool useRandomVolume = true;
		[Tooltip("Minimum volume that will be used when randomly set.")]
		[Range(0.0f, 1.0f)]
		[SerializeField] private float volMin = 1.0f;
		[Tooltip("Maximum volume that will be used when randomly set.")]
		[Range(0.0f, 1.0f)]
		[SerializeField] private float volMax = 1.0f;

		[Header("Random Pitch")]
		[Tooltip("Use min and max random pitch levels when playing sounds.")]
		[SerializeField] private bool useRandomPitch = true;
		[Tooltip("Minimum pitch that will be used when randomly set.")]
		[Range(-3.0f, 3.0f)]
		[SerializeField] private float pitchMin = 1.0f;
		[Tooltip("Maximum pitch that will be used when randomly set.")]
		[Range(-3.0f, 3.0f)]
		[SerializeField] private float pitchMax = 1.0f;

		[Header("Random Time")]
		[Tooltip("Use Retrigger Time to repeat the sound within a time range")]
		[SerializeField] private bool useRetriggerTime = false;
		[Tooltip("Inital time before the first repetion starts")]
		[Range(0.0f, 360.0f)]
		[SerializeField] private float timeInitial = 0.0f;
		[Tooltip("Minimum time that will pass before the sound is retriggered")]
		[Range(0.0f, 360.0f)]
		[SerializeField] private float timeMin = 0.0f;
		[Tooltip("Maximum pitch that will be used when randomly set.")]
		[Range(0.0f, 360.0f)]
		[SerializeField] private float timeMax = 0.0f;

		[Header("Random Silence")]
		[Tooltip("Use Retrigger Time to repeat the sound within a time range")]
		[SerializeField] private bool useRandomSilence = false;
		[Tooltip("Percent chance that the wave file will not play")]
		[Range(0.0f, 1.0f)]
		[SerializeField] private float percentToNotPlay = 0.0f;

		[Header("Delay Time")]
		[Tooltip("Time to offset playback of sound")]
		[SerializeField] private float delayOffsetTime = 0.0f;

        public bool Playing { get { return audioSource && audioSource.isPlaying; } }

		private AudioSource audioSource;
		private AudioClip clip;

		private void Awake() {
			audioSource = GetComponent<AudioSource>();

            if (audioSource.spatialBlend != 1f)
                audioSource.spatialBlend = 1f;

            if (audioSource.maxDistance != soundFalloff)
                audioSource.maxDistance = soundFalloff;

            clip = audioSource.clip;

			// audio source play on awake is true, just play the PlaySound immediately
			if (audioSource.playOnAwake) {
				if (useRetriggerTime)
					InvokeRepeating("Play", timeInitial, Random.Range(timeMin, timeMax));
				else
					Play();
			}

			// if playOnAwake is false, but the playOnAwakeWithDelay on the PlaySound is true, play the sound on away but with a delay
			else if (audioSource.playOnAwake == false && playOnAwakeWithDelay) {
				PlayWithDelay(delayOffsetTime);

				if (useRetriggerTime)
					InvokeRepeating("Play", timeInitial, Random.Range(timeMin, timeMax));
			}

			// in the case where both playOnAwake and playOnAwakeWithDelay are both set to true, just to the same as above, play the sound but with a delay
			else if (audioSource.playOnAwake && playOnAwakeWithDelay) {
				PlayWithDelay(delayOffsetTime);

				if (useRetriggerTime)
					InvokeRepeating("Play", timeInitial, Random.Range(timeMin, timeMax));
			}
		}

		// Play a random clip from those available
		public void Play() {
			if (looping) {
				PlayLooping();

			} else PlayOneShotSound();
		}

		public void PlayWithDelay(float delayTime) {
			if (looping)
				Invoke("PlayLooping", delayTime);
			else
				Invoke("PlayOneShotSound", delayTime);
		}

		// Play random wave clip on audiosource as a one shot
		public AudioClip PlayOneShotSound() {
			if (audioSource.isActiveAndEnabled == false)
				return null;

			SetAudioSource();
			if (stopOnPlay == false)
				audioSource.Stop();
			if (disableOnEnd)
				Invoke("Disable", clip.length);

			audioSource.PlayOneShot(clip);
			return clip;
		}

		public AudioClip PlayLooping() {
			// get audio source properties, and do any special randomizations
			SetAudioSource();

			// if the audio source has forgotten to be set to looping, set it to looping
			if (!audioSource.loop)
				audioSource.loop = true;

			// play the clip in the audio source, all the meanwhile updating it's location
			this.audioSource.Play();

			// if disable on end is checked, stop playing the wave file after the first loop has finished.
			if (stopOnEnd)
				Invoke("Stop", audioSource.clip.length);

			return clip;
		}

		public void Disable() {
			gameObject.SetActive(false);
		}

		public void Stop() {
			audioSource.Stop();
		}

		private void SetAudioSource() {
			if (useRandomVolume) {
				//randomly apply a volume between the volume min max
				audioSource.volume = Random.Range(volMin, volMax);

				if (useRandomSilence && (Random.Range(0, 1) < percentToNotPlay))
					audioSource.volume = 0;
			}

			//randomly apply a pitch between the pitch min max
			if (useRandomPitch)
				audioSource.pitch = Random.Range(pitchMin, pitchMax);

			if (waveFile.Length > 0) {
				// randomly assign a wave file from the array into the audioSource clip property
				audioSource.clip = waveFile[Random.Range(0, waveFile.Length)];
				clip = audioSource.clip;
			}
		}
	}
}
