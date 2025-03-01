using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenderManager : MonoBehaviour
{
    #region UI
    public GameObject surveyManager;
    private Toggle[] toggles;
    private bool isChecked = false;
    public GameObject choiceGroup;
    public TextMeshProUGUI nullText;
    #endregion

    public GameObject mainManager;

    // Start is called before the first frame update
    void Start()
    {
        if(surveyManager == null){
            surveyManager = GameObject.Find("SurveyPanel");
        }

        if(mainManager == null){
            mainManager = GameObject.Find("MainManager");
        }

        if(choiceGroup == null){
            choiceGroup = GameObject.Find("choiceGroup_Gender");
        }

        toggles = new Toggle[choiceGroup.transform.childCount];

        for(int i = 0; i<toggles.Length; i++){
            toggles[i] = choiceGroup.transform.GetChild(i).GetComponent<Toggle>();
        }

    }

    public void SubmitBtn(){
        
        //0: 남성, 1: 여성
        for(int i = 0; i<toggles.Length; i++){
            if(toggles[i].isOn && i == 0){
                mainManager.GetComponent<MainManager>().GenderUpdate(true);
                isChecked = true;
            }else if(toggles[i].isOn && i == 1){
                mainManager.GetComponent<MainManager>().GenderUpdate(false);
                isChecked = true;
            }else{
                nullText.text = "필수 확인 항목입니다.";
            }
        }

        if(isChecked){
            NextPanel();
        }
    }

    private void NextPanel(){
        gameObject.SetActive(false);
        surveyManager.SetActive(true);
    }


}
