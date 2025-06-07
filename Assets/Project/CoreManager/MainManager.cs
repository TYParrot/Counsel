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

    private string nextSceneExtremeQ = ""; // 다음 씬으로 넘길 극단 문항 텍스트 저장

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ParamUpdate(string extremeQ)
    {
        Debug.Log("극단적인 응답 문항:\n" + extremeQ);
        nextSceneExtremeQ = extremeQ;
    }

    public void GenderUpdate(bool male)
    {
        isMale = male;
    }

    //상담 씬 로드
    public void LoadBeforeCounsel()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadForest()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(2);
    }

    public void LoadBeach()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(1);
    }

    //씬이 완전히 로드된 이후에 호출됨
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var receiver = GameObject.Find("===Manager===")?.GetComponent<SceneDataReceiver>();
        if (receiver != null)
        {
            receiver.ReceiveData(nextSceneExtremeQ);
        }
        else
        {
            Debug.LogWarning("SceneReceiver 오브젝트를 찾지 못했습니다.");
        }

        // 이벤트 중복 등록 방지
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
