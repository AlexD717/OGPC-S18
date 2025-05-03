using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private float nextSFXTime = 0f;

    [SerializeField] private AudioClip[] boatTookDamage;
    [SerializeField] private AudioClip[] playerWon;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void BoatTookDamage()
    {
        SpawnAudio(player.position, boatTookDamage[Random.Range(0, boatTookDamage.Length)]);
    }

    public void PlayerWon()
    {
        SpawnAudio(player.position, playerWon[Random.Range(0, playerWon.Length)]);
    }

    // Call this function to spawn and play the audio
    public void SpawnAudio(Vector2 position, AudioClip audioClip)
    {
        if (Time.time < nextSFXTime)
        {
            return; // Prevents overlapping sounds
        }

        // Create a new GameObject at the specified position
        GameObject audioObject = new GameObject("AudioSourceObject");

        // Add an AudioSource component to the new GameObject
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();

        // Assign the AudioClip to the AudioSource
        audioSource.clip = audioClip;

        // Set the position of the audioObject
        audioObject.transform.position = position;

        // Play the audio
        audioSource.Play();

        // Destroy the GameObject after the audio clip has finished playing
        Destroy(audioObject, audioClip.length + 0.2f);
        nextSFXTime = audioClip.length + Time.time + 0.2f; // Prevents overlapping sounds
    }
}
