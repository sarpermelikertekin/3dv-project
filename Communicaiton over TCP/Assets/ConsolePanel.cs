using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class ConsolePanel : MonoBehaviour
{
    public TextMeshProUGUI consoleText;

    void Start()
    {
        Application.logMessageReceived += HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        consoleText.text += logString + "\n";
    }
}