using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDataReceiver : MonoBehaviour
{
    public ChatManager chat;
    public void ReceiveData(string extremeQ)
    {
        Debug.Log("MainManager로부터 받은 극단 문항: " + extremeQ);

        chat.UpdateSystemSet(extremeQ);

    }
}
