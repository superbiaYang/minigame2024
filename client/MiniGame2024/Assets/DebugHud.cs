using UnityEngine;
using UnityEngine.UI;

public class DebugHud : MonoBehaviour
{
    public GameManager m_GameManager;
    public Text logText;
    private string logMessages = "";

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logMessages += logString + "\n";
        logText.text = logMessages;
    }

    public Text m_PlayerNumText;
    public void StartGame()
    {
        m_GameManager.GameInit(int.Parse(m_PlayerNumText.text));
    }
}
