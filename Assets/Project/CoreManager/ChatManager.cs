using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TMPro; // TMP 네임스페이스 수정
using UnityEngine.UI;

using ApiConfig;

public class ChatManager : MonoBehaviour
{
    public TextMeshProUGUI aiChat; // Inspector에서 UI에 할당 필요
    public TMP_InputField userChat;
    private string userTxt;
    private HttpClient client;

    private float delayTime = 5.0f;

    //gpt의 설정
    private string systemSet = "You are a friendly counselor. Show empathy in your responses and ask only one short, key question based on the user's answer. The question should guide the conversation further, using kind and respectful language, encouraging the user to open up about their thoughts and feelings.";

    private List<object> messages = new List<object>();

    void Start()
    {
        // HttpClient를 한 번만 초기화
        client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiCollection.ApiKey}");
        client.DefaultRequestHeaders.Add("api-key", ApiCollection.ApiKey);
        messages.Add( new { role = "system", content = systemSet });
    }

    public void btn1(){
        
        if(string.IsNullOrWhiteSpace(userChat.text)){
            aiChat.text = "준비되시면 말씀해주세요.";
        }else{
            userTxt = userChat.text;

            UpdateMessages(userTxt);
            userChat.text = "";
            
            StartCoroutine(CallAzureOpenAIAsync1(userTxt, response =>
            {
                aiChat.text = response;
            }));
        }
    }

    private void UpdateMessages(string userInput){
        
        messages.Add(new { role = "user", content = userInput});
        
        //messages(0)은 시스템 성격
        //성격 => 사용자 => 답변 => 사용자 => 답변.... 
        if(messages.Count > 5){
            messages.Remove(1);
        }
    }
    
    public IEnumerator CallAzureOpenAIAsync1(string userInput, Action<string> callback)
    {
        //noResponse 방지용용
        yield return new WaitForSeconds(delayTime);

        var requestBody = new
        {
            messages = messages.ToArray(),
            max_tokens = 400,
            temperature = 0.8
        };

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // HttpClient를 재사용하여 요청 보내기
        Task<HttpResponseMessage> responseTask = client.PostAsync(ApiCollection.EndPoint, content);
        while (!responseTask.IsCompleted) yield return null;

        HttpResponseMessage response = responseTask.Result;
        Task<string> responseStringTask = response.Content.ReadAsStringAsync();
        while (!responseStringTask.IsCompleted) yield return null;

        string responseString = responseStringTask.Result;
        JObject responseJson = JObject.Parse(responseString);

        string resultText = responseJson["choices"]?[0]?["message"]?["content"]?.ToString() ?? "No response";

        // 응답을 메시지 히스토리에 추가
        messages.Add(new { role = "assistant", content = resultText });

        // Unity에서 UI 업데이트를 위해 콜백 사용
        callback?.Invoke(resultText);
    }

}
