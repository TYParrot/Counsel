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
    public string aiChat; 
    public TMP_InputField userChat;
    private string userTxt;
    private HttpClient client;

    private float delayTime = 10.0f;

    //gpt의 설정
    private string systemSet = "You are a professional and compassionate counselor. Your role is to actively help the user navigate their emotions and concerns by offering thoughtful insights and practical guidance. Demonstrate a deep understanding of the user's situation, providing not only empathy but also actionable suggestions. Acknowledge that you are a counselor and ensure the user feels supported. Ask only one concise yet meaningful question that encourages them to explore their thoughts further, using warm and encouraging language.";
    //no response 대신 노출될 답변
    private string noResponse = "음...";

    private List<object> messages = new List<object>();
    private string userSpeech;
    
    public TTSManager ttsManager;
    public AnimationManager animationManager;

    void Start()
    {
        // HttpClient를 한 번만 초기화
        client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiCollection.chatApiKey}");
        client.DefaultRequestHeaders.Add("api-key", ApiCollection.chatApiKey);
        messages.Add( new { role = "system", content = systemSet });

        //변수 초기화
        if(ttsManager == null){
            ttsManager = GameObject.Find("TTSManager").GetComponent<TTSManager>();
        }

        if(animationManager == null){
            animationManager = GameObject.Find("AnimationManager").GetComponent<AnimationManager>();
        }
    }

    public void btn1(){
        
        if(string.IsNullOrWhiteSpace(userChat.text)){
            aiChat = "준비되시면 말씀해주세요.";
            
            //TTS 호출
            ttsManager.TTSStart(aiChat);
            
            //끄덕임 아바타 재생
            animationManager.Nodding();

        }else{
            userTxt = userChat.text;

            UpdateMessages(userTxt);
            userChat.text = "";
            
            StartCoroutine(CallAzureOpenAIAsync(userTxt, response =>
            {
                aiChat = response;

                //"no response"시 재요청
                if(aiChat == "No response"){
                    
                    aiChat = noResponse;
                    ttsManager.TTSStart(aiChat);

                    //재전송 전 Delay
                    StartCoroutine(ReSendMsgToGpt());
                    StartCoroutine(CallAzureOpenAIAsync(userTxt, retryResponse =>{
                        aiChat = retryResponse;
                        
                        //TTS 호출
                        ttsManager.TTSStart(aiChat);

                        //끄덕임 아바타 재생
                        animationManager.Nodding();

                        messages.Add(new { role = "assistant", content = retryResponse });
                    }));

                }else{
                    aiChat = response;

                    //TTS 호출
                    ttsManager.TTSStart(aiChat);
                    
                    //끄덕임 아바타 재생
                    animationManager.Nodding();

                    messages.Add(new { role = "assistant", content = response });
                }

            }));
        }
    }

    //여러 문단으로 말을 하고 있을 때
    public void speechToMsg(string speech){

        userSpeech = userSpeech + " " + speech;
        
    }

    //말이 끝난 것이 감지 되었을 때
    public void sendMsg(){

        if(string.IsNullOrEmpty(userSpeech)){

            aiChat = "준비되시면 말씀해주세요.";
            
        }else{
            userTxt = userSpeech;

            //화면 출력용
            userChat.text = userTxt;

            UpdateMessages(userTxt);
            userSpeech = "";

            StartCoroutine(CallAzureOpenAIAsync(userTxt, response => {
                
                aiChat = response;
                messages.Add(new { role = "assistant", content = response});

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

    public IEnumerator ReSendMsgToGpt(){
        yield return new WaitForSeconds(delayTime);
    }

}
