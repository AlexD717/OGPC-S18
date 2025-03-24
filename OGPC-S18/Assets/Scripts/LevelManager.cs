using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float countdown;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private InputActionAsset inpuActions;

    [SerializeField] private Button continueButton;
    private Image buttonSlider;
    [SerializeField] float timeToWaitOnContinueButton;
    private float timeWaitedOnContinueButton = 0;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    private bool playerLost = false;
    private bool gameEnded = false;
    private bool showEndScreen = false;

    private void Start()
    {
        winScreen.gameObject.SetActive(false);
        loseScreen.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);

        buttonSlider = continueButton.transform.GetChild(0).gameObject.GetComponent<Image>();
        continueButton.onClick.AddListener(ShowEndScreen);
    }

    public void PlayerReachedEndPort()
    {
        PlayerWon();
    }

    private void Update()
    {
        if (showEndScreen) { return; }

        if (gameEnded)
        {
            timeWaitedOnContinueButton += Time.unscaledDeltaTime;
            buttonSlider.fillAmount = timeWaitedOnContinueButton / timeToWaitOnContinueButton;
            if (buttonSlider.fillAmount >= 1)
            {
                ShowEndScreen();
            }
            return;
        }

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
        Debug.Log("You Won!");
        EndGame();
    }

    private void PlayerLost()
    {
        playerLost = true;
        Debug.Log("You Lost!");
        countdownText.text = "0.00";
        EndGame();
    }

    private void ShowEndScreen()
    {
        showEndScreen = true;
        foreach (Transform child in winScreen.GetComponentInParent<Transform>())
        {
            child.gameObject.SetActive(false);
        }
        if (playerLost)
        {
            loseScreen.SetActive(true);
            foreach (Transform child in loseScreen.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            winScreen.SetActive(true);
            foreach (Transform child in winScreen.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    private void EndGame()
    {
        gameEnded = true;

        continueButton.gameObject.SetActive(true);

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
