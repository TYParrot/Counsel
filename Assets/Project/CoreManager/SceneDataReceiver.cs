using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDataReceiver : MonoBehaviour
{
    private string extremeQuestion;
    public void ReceiveData(string extremeQ)
    {
        Debug.Log("MainManager로부터 받은 극단 문항: " + extremeQ);

        extremeQuestion = extremeQ;

    }

    public string GetExtremeQ()
    {
        return extremeQuestion;
    }
}
