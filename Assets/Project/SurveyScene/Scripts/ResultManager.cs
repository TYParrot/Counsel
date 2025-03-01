using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    private int surveyScore;
    private int psrScore;
    public GameObject mainManager;
    
    // Start is called before the first frame update
    void Start()
    {
        if(mainManager == null){
            mainManager = GameObject.Find("MainManager");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable(){

        surveyScore = mainManager.GetComponent<MainManager>().ReturnSurveyScore();
        Debug.Log(surveyScore);
        
    }

}
