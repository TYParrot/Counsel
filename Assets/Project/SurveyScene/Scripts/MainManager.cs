using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//만약 psr과 survey를 종합평가해서 최종점수가 나온다면 MainManager에서 처리.
public class MainManager : MonoBehaviour
{
    private int surveyScore;
    private int[] userSubmits;
    private List<string[]> surveyQuest;
    private int psrScore;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ParamUpdate(int survey, int[] submits, List<string[]> surveyQuestion)
    {
        surveyScore = survey;
        
        userSubmits = new int[submits.Length];
        submits.CopyTo(userSubmits, 0);

        surveyQuest = new List<string[]>(surveyQuestion);
    }

    public void PsrScoreUpdate(int psr){
        psrScore = psr;
    }

    public int ReturnSurveyScore(){
        return surveyScore;
    }

    public int ReturnPsrScore(){
        return psrScore;
    }
}
