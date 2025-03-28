using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerPositionTracker : MonoBehaviour
{
    private bool firstPlayerPositionTracker = false;
    private GameObject player;

    private string data;
    private bool mapOn;

    private void Awake()
    {
        if (FindObjectsByType<PlayerPositionTracker>(FindObjectsSortMode.None).Length > 1 && !firstPlayerPositionTracker)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            firstPlayerPositionTracker = true;
        }
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += SceneLoaded;

        GameObject dataExportObject = GameObject.Find("Data Export");
        if (dataExportObject != null)
        {
            dataExportObject.GetComponent<Button>().onClick.AddListener(ExportData);
        }
    }

    private void SceneLoaded(Scene arg0, Scene arg1)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GameObject dataExportObject = GameObject.Find("Data Export");
        if (dataExportObject != null)
        {
            dataExportObject.GetComponent<Button>().onClick.AddListener(ExportData);
        }

        data += $"SceneLoaded,{arg1.name},{Time.unscaledTime}\n";
    }

    private void Update()
    {
        if (player == null) { return; } 
        
        if (mapOn)
        {
            data += $"mapActive,{Time.unscaledTime}\n";
        }
        else
        {
            data += $"{player.transform.position.x},{player.transform.position.y},{Time.unscaledTime}\n";
        }
    }

    public void ExportData()
    {
        Debug.Log("Exporting Data");
        Debug.Log(data);

        // Save the CSV to a file
        string filePath = "data:text/csv;charset=utf-8," + System.Uri.EscapeDataString(data);
        Application.OpenURL(filePath); // Open the URL to trigger the download

        Debug.Log($"Positions saved to {filePath}");
    }

    public void PlayerToggledMap(bool active)
    {
        mapOn = active;
    }
}
