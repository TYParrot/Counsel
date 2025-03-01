using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;

public class SurveyManager : MonoBehaviour
{
    #region dataLoad
    private List<string[]> surveyQuestion;
    private string path = "Assets/Project/SurveyScene/Scripts/pss_10Question.json"; //질문 파일 경로
    #endregion

    #region UI
    public GameObject resultPanel;
    public TextMeshProUGUI question;
    public GameObject choiceGroup;
    private Toggle[] toggles; //toggle들을 저장하는 배열
    private int questionNumber = 0;
    public TextMeshProUGUI nullText;
    private bool isChecked = false;
    private int[] userSubmits; //사용자가 선택한 토글 점수 저장 
    private int surveyScore = 0; //설문조사 최종 점수
    #endregion

    public GameObject mainManager;

    // Start is called before the first frame update
    void Start()
    {
        //json 파일에서 질문 로드 및 surveyQuestion에 저장
        LoadQuestion();

        //첫번째 문항 세팅
        questTextSet(questionNumber);

        if(choiceGroup == null){

            Debug.LogError("ChoiceGroup 항목을 할당해주세요.");

        }else{
            
            toggles = new Toggle[choiceGroup.transform.childCount];
            for(int i = 0; i < toggles.Length; i++){

                toggles[i] = choiceGroup.transform.GetChild(i).GetComponent<Toggle>();

            }

        }

        if(mainManager == null){
            mainManager = GameObject.Find("MainManager");
        }

        //사용자 점수 저장 배열
        userSubmits = new int[surveyQuestion.Count];
    }

    //json 파일에서 질문 목록 불러오기
    void LoadQuestion(){
        
        if(!File.Exists(path)){
            Debug.LogError("Survey Json 파일이 존재하지 않습니다.");
            return;
        }

        string json = File.ReadAllText(path);
        QuestionData qData = JsonConvert.DeserializeObject<QuestionData>(json);

        //변수에 할당
        if(qData != null && qData.questions != null){
            surveyQuestion = new List<string[]>(qData.questions);
        }else{
            Debug.LogError("Survey Json 파일이 올바르지 않습니다.");
        }
    }

    void questTextSet(int num){

        question.text = surveyQuestion[num][0];

    }

    //점수 계산
    void CalculateScore(){
        
        for(int i = 0; i < surveyQuestion.Count; i++){

            if(surveyQuestion[i][1].Equals("+")){
                surveyScore += userSubmits[i];
            }else{
                surveyScore -= toggles.Length - 1 - userSubmits[i];
            }
        }

        mainManager.GetComponent<MainManager>().ParamUpdate(surveyScore, userSubmits, surveyQuestion);

    }
    
    //다음 및 제출 버튼 이벤트
    public void SubmitBtn(){

        //토글이 모두 비어있는지 확인하고, 체크되었다면 몇 점짜리인지 기록.
        for(int i = 0; i < toggles.Length; i++){

            if(toggles[i].isOn){

                userSubmits[questionNumber] = i;
                isChecked = true;
            }
        }
        
        //토글 항목이 하나도 체크되지 않았다면, nullText를 띄우기.
        if(!isChecked){
            nullText.text = "선택이 비어있습니다.";
            return;
        }
        
        //마지막 질문이면 제출 처리
        questionNumber++;

        if(questionNumber < surveyQuestion.Count){

            nullText.text = "";

            for(int i = 0; i<toggles.Length; i++){

                toggles[i].isOn = false;

            }

            questTextSet(questionNumber);

        }else{
            
            CalculateScore();
            gameObject.SetActive(false);
            resultPanel.SetActive(true);

        }

        isChecked = false;

    }
}

[System.Serializable]
public class QuestionData{
    public List<string[]> questions;
}
