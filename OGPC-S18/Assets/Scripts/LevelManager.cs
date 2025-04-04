using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private float countdown;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private Button continueButton;
    private Image buttonSlider;
    [SerializeField] float timeToWaitOnContinueButton;
    private float timeWaitedOnContinueButton = 0;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private int scoreFromSavedPorts;

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
        inputActions.FindActionMap("Player").Enable();
        inputActions.FindActionMap("Map").Enable();

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
        countdownText.text = "0.00";
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
        // Figures out how many ports exist and how many are saved
        Port[] ports = FindObjectsByType<Port>(FindObjectsSortMode.None);
        int savedPorts = 0;
        // Count how many ports are saved
        foreach (Port port in ports)
        {
            if (port.portSaved)
            {
                savedPorts++;
            }
        }

        // Shows win screen
        winScreen.SetActive(true);
        foreach (Transform child in winScreen.transform)
        {
            child.gameObject.SetActive(true);
        }

        // Fill in extra information
        Transform dataGrid = winScreen.transform.GetChild(2);

        // Fill in score depending on how much time is left
        dataGrid.GetChild(4).GetComponent<TextMeshProUGUI>().text = countdownText.text; // Says time remaining
        float timeRemainingScore = CalculateTimeScore(countdown);
        dataGrid.GetChild(5).GetComponent<TextMeshProUGUI>().text = timeRemainingScore.ToString();

        dataGrid.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = savedPorts.ToString() + "/" + ports.Length.ToString(); // Says saved ports out of total ports
        int savedPortsScore = CalculateSavedPortScore(savedPorts, ports.Length);
        dataGrid.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = savedPortsScore.ToString(); // Says saved ports score
        
        // Fill in total score
        float totalScore = timeRemainingScore + savedPortsScore;
        winScreen.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Total Score: " + totalScore.ToString();
    }

    private int CalculateTimeScore(float timeRemaining)
    {
        // Calculate score based on time remaining
        float timeScore = Mathf.Round(Mathf.Log(timeRemaining, 1.7f) * 250);
        return (int)timeScore;
    }

    private int CalculateSavedPortScore(int savedPorts, int totalPorts)
    {
        // Calculate score based on saved ports
        if (totalPorts == 0) { return 0; } // Avoid division by zero
        float portScore = (float)savedPorts/(float)totalPorts * (float)scoreFromSavedPorts;
        return (int)portScore;
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
        inputActions.FindActionMap("Player").Disable();
        inputActions.FindActionMap("Map").Disable();
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
