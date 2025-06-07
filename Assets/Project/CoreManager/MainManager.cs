using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//만약 gsr과 survey를 종합평가해서 최종점수가 나온다면 MainManager에서 처리.
public class MainManager : MonoBehaviour
{
    private int surveyScore;
    private int[] userSubmits;
    private List<string[]> surveyQuest;
    private int beforeGsrScore;
    private int afterGsrScore;
    private bool isMale = false;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ParamUpdate(string extremeQ)
    {
        
        Debug.Log("극단적인 응답 문항:\n" + extremeQ);

    }

    public void GenderUpdate(bool male){
        isMale = male;
    }
    
    //아래는 상담 씬 로드
    public void LoadBeforeCounsel(){
        SceneManager.LoadScene(0);
    }

    public void LoadForest(){
        SceneManager.LoadScene(2);
    }
    
    public void LoadBeach(){
        SceneManager.LoadScene(1);
    }
    
}
