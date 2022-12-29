using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameMessagesUIHandler : MonoBehaviour
{
    public TextMeshProUGUI[] textMeshProUGUIs;
    Queue messageQueue = new Queue();

    public void OnGameMessageRecieved(string message)
    {
        Debug.Log($"OnGameMessageRecieved {message}");
        messageQueue.Enqueue(message);
        if (messageQueue.Count > 4)
        {
            messageQueue.Dequeue();
        }

        int queueIndex = 0;
        foreach (string item in messageQueue)
        {
            textMeshProUGUIs[queueIndex].text = item;
            queueIndex++;
        }
    }
}
