using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private float sfxIntervalBeforeRandomSFX = 8f;
    [SerializeField] private float minSFXInterval = 0.5f; // Minimum interval between sound effects
    private float nextSFXTime = 0f;

    [SerializeField] private AudioClip[] clipsDuringSailing;
    [SerializeField] private AudioClip[] boatTookDamage;
    [SerializeField] private AudioClip[] playerWon;
    [SerializeField] private AudioClip[] playerLost;
    [SerializeField] private AudioClip[] playerDocked;
    [SerializeField] private AudioClip[] playerUndocked;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Update the nextSFXTime if needed
        if (Time.time >= nextSFXTime + sfxIntervalBeforeRandomSFX)
        {
            SailingSFX();
        }
    }

    private void SailingSFX()
    {
        SpawnAudio(player.position, clipsDuringSailing[Random.Range(0, clipsDuringSailing.Length)]);
    }

    public void BoatTookDamage()
    {
        SpawnAudio(player.position, boatTookDamage[Random.Range(0, boatTookDamage.Length)]);
    }

    public void PlayerLost()
    {
        SpawnAudio(player.position, playerLost[Random.Range(0, playerLost.Length)]);
    }

    public void PlayerWon()
    {
        SpawnAudio(player.position, playerWon[Random.Range(0, playerWon.Length)]);
    }

    public void PlayerDocked()
    {
        SpawnAudio(player.position, playerDocked[Random.Range(0, playerDocked.Length)]);
    }

    public void PlayerUndocked()
    {
        SpawnAudio(player.position, playerUndocked[Random.Range(0, playerUndocked.Length)]);
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
        Destroy(audioObject, audioClip.length + minSFXInterval);
        nextSFXTime = audioClip.length + Time.time + minSFXInterval; // Prevents overlapping sounds
    }
}
