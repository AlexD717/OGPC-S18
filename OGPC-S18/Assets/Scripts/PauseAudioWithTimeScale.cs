using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PauseAudioWithTimeScale : MonoBehaviour
{
    private AudioSource audioSource;
    private bool wasPlayingBeforePause = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                wasPlayingBeforePause = true;
            }
        }
        else
        {
            if (!audioSource.isPlaying && wasPlayingBeforePause)
            {
                audioSource.UnPause();
                wasPlayingBeforePause = false;
            }
        }
    }
}