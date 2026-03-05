using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;
using static Unity.Collections.AllocatorManager;

public class MainSceneView : MonoBehaviour , IMainSceneView
{
    MainScenePresenter presenter;

    [SerializeField]
    Button StartButton, CreateRoomButton,CreateRoomFinishButton,JoinRoomButton,JoinRoomFinishButton,LeaveRoomButton;

    private static object _lock = new object();

    private Queue<ChatItem> chatQueue = new Queue<ChatItem>();
    [SerializeField]
    private int maxChatCount;

    private ErrorCode code;

    public string ChatJoinRoomName { get; set; }

    [Header("방 생성 관련")]
    [SerializeField]
    GameObject CreateRoomImage;
    [SerializeField]
    TMP_InputField RoomName;
    [SerializeField]
    TMP_InputField MaxMembers;

    [Header("방 참가 관련")]
    [SerializeField]
    GameObject JoinRoomImage;
    [SerializeField]
    TMP_InputField JoinRoomNameInputField;


    [Header("정보 관련")]
    [SerializeField]
    GameObject InfoImage;
    [SerializeField]
    TMP_Text InfoTMP;
    [SerializeField]
    Button InfoCheckButton;

    [Header("채팅메시지 관련")]
    [SerializeField]
    GameObject ChatScrollView;
    [SerializeField]
    TMP_InputField ChatMessage;
    [SerializeField]
    Button ChatSendButton;

    [SerializeField]
    Transform chatContent; //채팅 프리펩 부모
    [SerializeField]
    GameObject chatMessagePrefab; //채팅 프리펩
    [SerializeField]
    ScrollRect chatScrollRect;



    void Start()
    {
        presenter = new MainScenePresenter(this);

  
        CreateRoomImage.gameObject.SetActive(false);
        InfoImage.gameObject.SetActive(false);
        JoinRoomImage.gameObject.SetActive(false);
        ChatScrollView.gameObject.SetActive(false);

        NetworkManager.Instance.errorCode += SetInfo;
        NetworkManager.Instance.chatBroadCast += ChatBroadCasting;
        NetworkManager.Instance.chatHistory += ChatHistory;

        Extension.ResetListener(StartButton, OnStartButton);
        Extension.ResetListener(CreateRoomButton, OnCreateRoomButton);
        Extension.ResetListener(CreateRoomFinishButton, OnCreateRoomFinishButton);
        Extension.ResetListener(JoinRoomButton, OnJoinRoomButton);
        Extension.ResetListener(JoinRoomFinishButton, OnJoinRoomFinishButton);
        Extension.ResetListener(ChatSendButton, OnSendMessageButton);

        Extension.ResetListener(LeaveRoomButton, OnLeaveRoomButton);


        Extension.ResetListener(InfoCheckButton, OnInfoBeActive);

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && ChatScrollView.activeSelf)
        {
            OnSendMessageButton();
        }
    }


    private void OnInfoBeActive() => InfoImage.gameObject.SetActive(false);

    /// <summary>
    /// 네트워크 매니저로 부터 브로드캐스팅 패킷을 전달 받았을 때 호출
    /// </summary>
    /// <param name="notifyChat"></param>
    private void ChatBroadCasting(NotifyChat notifyChat)
    {
        Debug.Log("ChatBroadCasting Result" + notifyChat.result);

        if(notifyChat.result == ErrorCode.Chat_Delete )
        {
            foreach(Transform item in chatContent)
            {
                Destroy(item.gameObject);
            }

            chatQueue.Clear();

            return;
        }
        presenter.OnChatReceived(notifyChat);
    }


    private void ChatHistory(List<RedisChatMessage> chatHistory)
    {
        Debug.Log("ChatHistory");
        presenter.OnChatHistoryReceived(chatHistory);
    }
    private void SetInfo(ErrorCode code)
    {
        lock (_lock)
        {
            this.code = code;

            Debug.Log("MainScene ErrorCode - 상태 : " + code);

            if(code == ErrorCode.Chat_Sucess || code == ErrorCode.GetRankAndScore_Success) { return; } //채팅 송수신은 정보 이미지 활성화 x

            InfoImage.gameObject.SetActive(true);

            switch (code)
            {
                case ErrorCode.RoomCreate_Sucess:
                    Extension.ResetListener(InfoCheckButton, ChatImageAcitve); //방 생성 -> 채팅창 활성화
                    InfoTMP.text = "방 생성 성공";
                    break;
                case ErrorCode.RoomJoin_Sucess:
                    Extension.ResetListener(InfoCheckButton, ChatImageAcitve); //방 참가 -> 채팅창 활성화
                    InfoTMP.text = "방 참가 성공";
                    break;
                case ErrorCode.LeaveRoom_Success:
                    Extension.ResetListener(InfoCheckButton, ImageAllBeAtive); //방 나갔을 때 모든 UI 이미지 비활성화
                    InfoTMP.text = "방 나가기 성공";
                    break;
                case ErrorCode.LeaveRoomDelete_Success:
                    Debug.Log("방 삭제");
                    Extension.ResetListener(InfoCheckButton, ImageAllBeAtive); //방 나갔을 때 모든 UI 이미지 비활성화
                    InfoTMP.text = "방 삭제 성공";
                    break;
                case ErrorCode.Room_DuplicateName:
                    Extension.ResetListener(InfoCheckButton, OnInfoBeActive);
                    InfoTMP.text = "방 이름 중복";
                    break;
                case ErrorCode.Room_NotFound:
                    Extension.ResetListener(InfoCheckButton, OnInfoBeActive);
                    InfoTMP.text = "방을 찾지 못함";
                    break;
                case ErrorCode.Room_Full:
                    Extension.ResetListener(InfoCheckButton, OnInfoBeActive);
                    InfoTMP.text = "방 인원 꽉참";
                    break;
                case ErrorCode.Room_InvalidName:
                    Extension.ResetListener(InfoCheckButton, OnInfoBeActive);
                    InfoTMP.text = "방 이름의 특수문자 발견!";
                    break;
                case ErrorCode.Room_NameTooLong:
                    Extension.ResetListener(InfoCheckButton, OnInfoBeActive);
                    InfoTMP.text = "방 이름이 너무 긺";
                    break;
                case ErrorCode.DB_Error:
                    Extension.ResetListener(InfoCheckButton, OnInfoBeActive);
                    InfoTMP.text = "서버에 문제가 발생";
                    break;
            }
        }
   
    }

    public void OnCreateRoomFinishButton()
    {
        if(string.IsNullOrEmpty(RoomName.text) || string.IsNullOrEmpty(MaxMembers.text))
        {
            InfoImage.gameObject.SetActive(true);
            InfoTMP.text = "방 제목 또는 최대 인원을 입력하세요";
            return;
        }

        string CreateRoomName = RoomName.text;
        int MaxMember = int.Parse(MaxMembers.text);

        if(MaxMember < 1 || MaxMember > 5)
        {
            InfoImage.gameObject.SetActive(true);
            InfoTMP.text = "최대 인원은 1명~5명 입니다";
            return;
        }

        Debug.Log("생성 버튼 - " + chatQueue.Count);

        chatQueue.Clear();

        Debug.Log("생성 버튼 챗 큐 삭제 - " + chatQueue.Count);
        presenter.CreateRoom(CreateRoomName, MaxMember);
    }

    public void OnJoinRoomFinishButton()
    {
        string JoinRoomName = JoinRoomNameInputField.text;

        if (string.IsNullOrEmpty(JoinRoomName))
        {
            InfoImage.gameObject.SetActive(true);
            InfoTMP.text = "참가 할 방 제목을 입력하세요";
            return;
        }

        presenter.JoinRoom(JoinRoomName);
    }


    public void OnJoinRoomButton() => JoinRoomImage.SetActive(true);

    public void OnCreateRoomButton() => CreateRoomImage.SetActive(true);
    public void OnLeaveRoomButton() => presenter.LeaveRoom(ChatJoinRoomName);


    public void OnStartButton()
    {
        SceneManager.LoadScene(2);
    }

   
    public void OnSendMessageButton()
    {
        string chatMessage = ChatMessage.text;

        if (string.IsNullOrEmpty(chatMessage))
        {
            InfoImage.gameObject.SetActive(true);
            InfoTMP.text = "채팅을 입력하세요";
            return;
        }
        presenter.SendChatMessage(ChatJoinRoomName, chatMessage);

        //입력창 초기화 부분
        ChatMessage.text = "";
        ChatMessage.ActivateInputField();
    }


    /// <summary>
    /// 채팅 보여주기, 방에 접속한 유저에게 채팅 보이게 하기
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="message"></param>
    public void ShowChatMessage(string userId, string message)
    {
        // 채팅 메시지 생성
        ChatItem chatItem = Instantiate(chatMessagePrefab, chatContent).GetComponent<ChatItem>();

        chatItem.SetChatItem(userId, message);

        chatQueue.Enqueue(chatItem);
        
        if(chatQueue.Count > maxChatCount)
        {
            ChatItem oldItem = chatQueue.Dequeue();
            Destroy(oldItem.gameObject);
        }
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0f;



        Debug.Log($"[UI] 채팅 표시: {userId} - {message}");
    }



    /// <summary>
    /// 채팅 이미지 활성화 되었을 때 초기화,채팅 UI 제외하고 이미지 UI 비활성화  
    /// </summary>
    private void ChatImageAcitve()
    {
        
        InfoImage.gameObject.SetActive(false);
        CreateRoomImage.gameObject.SetActive(false);
        JoinRoomImage.gameObject.SetActive(false);

        if(!ChatScrollView.gameObject.activeSelf)
        {
            ChatScrollView.gameObject.SetActive(true);
        }
        
    }
    /// <summary>
    /// 모든 이미지 전체 비활성화 - 방 나갔을 때
    /// </summary>
    private void ImageAllBeAtive()
    {
        InfoImage.gameObject.SetActive(false);
        CreateRoomImage.gameObject.SetActive(false);
        JoinRoomImage.gameObject.SetActive(false);
        ChatScrollView.gameObject.SetActive(false);
    }




    private void OnDestroy()
    {
        NetworkManager.Instance.errorCode -= SetInfo;
        NetworkManager.Instance.chatBroadCast -= ChatBroadCasting;
        NetworkManager.Instance.chatHistory -= ChatHistory;
    }

    private void OnApplicationQuit()
    {
        NetworkManager.Instance.errorCode -= SetInfo;
        NetworkManager.Instance.chatBroadCast -= ChatBroadCasting;
        NetworkManager.Instance.chatHistory -= ChatHistory;

        if(!string.IsNullOrEmpty(ChatJoinRoomName))
        {
            Debug.Log("참가했던 방 삭제");
            OnLeaveRoomButton();
        }
        
    }

}
