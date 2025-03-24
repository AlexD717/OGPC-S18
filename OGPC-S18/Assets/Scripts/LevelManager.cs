using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float countdown;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private InputActionAsset inpuActions;

    private bool playerLost = false;

    public void PlayerReachedEndPort()
    {
        PlayerWon();
    }

    private void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            if (!playerLost)
            {
                PlayerLost();
            }
            return;
        }
        UpdateCountdownDisplay();
    }

    private void PlayerWon()
    {
        Debug.Log("YOU WON!");
        EndGame();
    }

    private void PlayerLost()
    {
        playerLost = true;
        Debug.Log("You Lost!");
        countdownText.text = "0.00";
        EndGame();
    }

    private void EndGame()
    {
        Time.timeScale = 0f;

        // Disables new input collection
        inpuActions.FindActionMap("Player").Disable();
        inpuActions.FindActionMap("Map").Disable();
    }

    private void UpdateCountdownDisplay()
    {
        float minutesFloat = countdown / 60f;
        int minutes = (int)Mathf.Floor(countdown / 60f);
        int seconds = (int)Mathf.Floor(countdown - minutes * 60);
        float milliSeconds = countdown - Mathf.Floor(countdown);
        if (minutes > 0)
        {
            countdownText.text = $"{minutes.ToString("D2")}:{seconds.ToString("D2")}";
        }
        else
        {
            countdownText.text = (seconds + milliSeconds).ToString("F2");
        }
    }
}
