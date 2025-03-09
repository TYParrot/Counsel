using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine.UI;

using ApiConfig;

public class ChatManager : MonoBehaviour
{
    public TextMeshProUGUI aiChat; 
    public TMP_InputField userChat;
    private string userTxt;
    private HttpClient client;

    private float delayTime = 15.0f;

    //gpt의 설정
    private string systemSet = "You are a friendly counselor. Show empathy in your responses and ask only one short, key question based on the user's answer. The question should guide the conversation further, using kind and respectful language, encouraging the user to open up about their thoughts and feelings.";

    private List<object> messages = new List<object>();

    void Start()
    {
        // HttpClient를 한 번만 초기화
        client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiCollection.chatApiKey}");
        client.DefaultRequestHeaders.Add("api-key", ApiCollection.chatApiKey);
        messages.Add( new { role = "system", content = systemSet });
    }

    public void btn1(){
        
        if(string.IsNullOrWhiteSpace(userChat.text)){
            aiChat.text = "준비되시면 말씀해주세요.";
        }else{
            userTxt = userChat.text;

            UpdateMessages(userTxt);
            userChat.text = "";
            
            StartCoroutine(CallAzureOpenAIAsync(userTxt, response =>
            {
                aiChat.text = response;
                
                // 응답을 메시지 히스토리에 추가
                messages.Add(new { role = "assistant", content = response });

            }));
        }
    }

    private void UpdateMessages(string userInput){

        //messages(0)은 시스템 성격
        //성격 => 사용자 => 답변 => 사용자 => 답변.... 
        if(messages.Count >= 5){
            messages.RemoveAt(1);
            messages.RemoveAt(1);
        }

        messages.Add(new { role = "user", content = userInput});

        for(int i = 0; i<messages.Count; i++){
            Debug.Log(messages[i]);
        }
    }

    public void btn2(){
        for(int i = 0; i<messages.Count; i++){
            Debug.Log(messages[i]);
        }
    }
    
    public IEnumerator CallAzureOpenAIAsync(string userInput, Action<string> callback)
    {
        var requestBody = new
        {
            messages = messages.ToArray(),
            max_tokens = 400,
            temperature = 0.8
        };

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //noResponse 방지용
        yield return new WaitForSeconds(delayTime);

        // HttpClient를 재사용하여 요청 보내기
        Task<HttpResponseMessage> responseTask = client.PostAsync(ApiCollection.chatEndPoint, content);
        while (!responseTask.IsCompleted) yield return null;

        HttpResponseMessage response = responseTask.Result;
        Task<string> responseStringTask = response.Content.ReadAsStringAsync();
        while (!responseStringTask.IsCompleted) yield return null;

        string responseString = responseStringTask.Result;
        JObject responseJson = JObject.Parse(responseString);

        string resultText = responseJson["choices"]?[0]?["message"]?["content"]?.ToString() ?? "No response";

        // Unity에서 UI 업데이트를 위해 콜백 사용
        callback?.Invoke(resultText);
    }

}
