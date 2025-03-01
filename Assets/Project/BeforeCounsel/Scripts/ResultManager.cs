using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    private int surveyScore;
    private int gsrScore;
    public GameObject mainManager;
    public TextMeshProUGUI resultScoreTxt;
    public RawImage envImg;
    public Texture highStressTexture;
    public Texture lowStressTexture;
    
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

    //현재 gsr 점수가 없어 pss 점수로 세팅되는 중
    void OnEnable(){

        //점수 세팅
        surveyScore = mainManager.GetComponent<MainManager>().ReturnSurveyScore();
        resultScoreTxt.text = surveyScore.ToString() + "점";

        if(highStressTexture == null || lowStressTexture == null || envImg == null){
            Debug.Log("상담 환경 이미지 혹은 객체가 비어있습니다.");
        }

        //상담 환경 이미지 세팅
        if(surveyScore < 16){
            envImg.texture = lowStressTexture;
        }else{
            envImg.texture = highStressTexture;
        }
        
    }

    public void LoadCounselScene(){
        
        if(surveyScore < 16){
            mainManager.GetComponent<MainManager>().LoadLowStress();
        }else{
            mainManager.GetComponent<MainManager>().LoadHighStress();
        }
    }

}
