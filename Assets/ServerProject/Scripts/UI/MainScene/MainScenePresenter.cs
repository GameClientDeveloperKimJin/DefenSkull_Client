using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainScenePresenter
{
    IMainSceneView view;

    public MainScenePresenter(IMainSceneView view)
    {
        this.view = view;
    }
    public void CreateRoom(string RoomName , int MaxMber)
    {
        try
        {
            //요청할 객체 생성 및 직렬화 준비 
            var reqData = new ReqCreateRoom();
            reqData.RoomName = RoomName;
            //reqData.UserID = NetworkManager.Instance.GetUserID();
            reqData.MaxMembers = MaxMber;



            //요청할 객체 직렬화 ( MessagePack 직렬화  )
            byte[] bodyData = MessagePackSerializer.Serialize(reqData);

            //패킷 전송
            NetworkManager.Instance.SendPacket(PACKETID.CreateRoom_Request, bodyData);

            view.ChatJoinRoomName = RoomName;

        }
        catch (Exception ex)
        {
            Debug.LogError($"회원가입 요청 오류: {ex.Message}");
        }
    }
    public void JoinRoom(string JoinRoomName)
    {
        try
        {
            //요청할 객체 생성 및 직렬화 준비 
            var reqData = new ReqJoinRoom();
            reqData.RoomName = JoinRoomName;
            reqData.UserID = NetworkManager.Instance.GetUserID();
            Debug.Log("방에 참가한 유저 ID " + reqData.UserID);


            //요청할 객체 직렬화 ( MessagePack 직렬화  )
            byte[] bodyData = MessagePackSerializer.Serialize(reqData);

            //패킷 전송
            NetworkManager.Instance.SendPacket(PACKETID.JoinRoom_Request, bodyData);

            view.ChatJoinRoomName = JoinRoomName;

        }
        catch (Exception ex)
        {
            Debug.LogError($"회원가입 요청 오류: {ex.Message}");
        }
    }
    public void SendChatMessage(string JoinRoomName ,string chatMessage)
    {
        try
        {
            //요청할 객체 생성 및 직렬화 준비 
            var reqData = new ReqChat();
            reqData.RoomName = JoinRoomName;
            reqData.UserID = NetworkManager.Instance.GetUserID();
            reqData.Message = chatMessage;


            //요청할 객체 직렬화 ( MessagePack 직렬화  )
            byte[] bodyData = MessagePackSerializer.Serialize(reqData);

            //패킷 전송
            NetworkManager.Instance.SendPacket(PACKETID.Chat_Request, bodyData);

        }
        catch (Exception ex)
        {
            Debug.LogError($"회원가입 요청 오류: {ex.Message}");
        }
    }
    public void OnChatReceived(NotifyChat notifyData)
    {
        try
        {
            // View에게 전달
            view.ShowChatMessage(notifyData.UserId, notifyData.Message);

            Debug.Log($"[채팅 수신] {notifyData.UserId}: {notifyData.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"채팅 수신 처리 오류: {ex.Message}");
        }
    }
    public void OnChatHistoryReceived(List<RedisChatMessage> messages)
    {
        try
        {
            foreach (var msg in messages)
            {
                view.ShowChatMessage(msg.UserId, msg.Message);
            }

            Debug.Log($"[히스토리 표시] {messages.Count}개");
        }
        catch (Exception ex)
        {
            Debug.LogError($"히스토리 표시 오류: {ex.Message}");
        }
    }

    public void LeaveRoom(string currentRoom)
    {
        try
        {
            var reqData = new ReqLeaveRoom();
            reqData.RoomName = currentRoom;
            reqData.UserID = NetworkManager.Instance.GetUserID();

            byte[] bodyData = MessagePackSerializer.Serialize(reqData);
   
            NetworkManager.Instance.SendPacket(PACKETID.LeaveRoom_Request, bodyData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"방 나가기 오류: {ex.Message}");
        }
    }
}
