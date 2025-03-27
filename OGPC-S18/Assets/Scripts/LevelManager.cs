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

        // Enables input collection
        inpuActions.FindActionMap("Player").Enable();
        inpuActions.FindActionMap("Map").Enable();

        // Resets Time
        Time.timeScale = 1f;
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

    public void PlayerWon()
    {
        Debug.Log("You Won!");
        EndGame();
    }

    public void PlayerLost()
    {
        playerLost = true;
        Debug.Log("You Lost!");
        EndGame();
    }

    private void ShowEndScreen()
    {
        showEndScreen = true;
        // Hide all other items in UI
        foreach (Transform child in winScreen.GetComponentInParent<Transform>())
        {
            child.gameObject.SetActive(false);
        }
        
        if (playerLost)
        {
            ShowLoseScreen();
        }
        else
        {
            ShowWinScreen();
        }
    }

    private void ShowWinScreen()
    {
        winScreen.SetActive(true);
        foreach (Transform child in winScreen.transform)
        {
            child.gameObject.SetActive(true);
        }

        // Fill in extra information
        Transform dataGrid = winScreen.transform.GetChild(2);
        // Fill in score depending on how much time is left
        dataGrid.GetChild(4).GetComponent<TextMeshProUGUI>().text = countdownText.text; // Says time remaining
        float timeRemainingScore = Mathf.Round(Mathf.Log(countdown, 1.7f) * 250);
        dataGrid.GetChild(5).GetComponent<TextMeshProUGUI>().text = timeRemainingScore.ToString();

        // Fill in total score
        float totalScore = timeRemainingScore;
        winScreen.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Total Score: " + totalScore.ToString();
    }

    private void ShowLoseScreen()
    {
        loseScreen.SetActive(true);
        foreach (Transform child in loseScreen.transform)
        {
            child.gameObject.SetActive(true);
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
