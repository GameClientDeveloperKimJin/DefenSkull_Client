using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// View로 부터 전달받은 UI 입력 처리를 하고, 요청할 객체를 직렬화해서 body를 만들고, 패킷 id에 맞게 Send
/// </summary>
public class TitleScenePresenter 
{

    ITitleSceneView view;

  

    public TitleScenePresenter(ITitleSceneView view)
    {
        this.view = view;
    }


    public void StartButton()
    {
        byte[] bodyData = System.Text.Encoding.UTF8.GetBytes("서버 접속 시도 중..");
        NetworkManager.Instance.SendPacket(PACKETID.ECHO_Request, bodyData);
    }
    public void RegisterFinishButton(string userID, string userPassword)
    {
        try
        {
            //요청할 객체 생성 및 직렬화 준비 
            var reqData = new ReqRegister();
            reqData.UserID = userID;
            reqData.Password = userPassword;


            //요청할 객체 직렬화 ( MessagePack 직렬화  )
            byte[] bodyData = MessagePackSerializer.Serialize(reqData);

            //패킷 전송
            NetworkManager.Instance.SendPacket(PACKETID.Register_Request, bodyData);

        }
        catch (Exception ex)
        {
            Debug.LogError($"회원가입 요청 오류: {ex.Message}");
        }
        
    }
    public void LoginButton(string userID, string userPassword)
    {
        try
        {
            //요청할 객체 생성 및 직렬화 준비 
            var reqData = new ReqLogin();
            reqData.UserID = userID;
            reqData.Password = userPassword;


            //요청할 객체 직렬화 ( MessagePack 직렬화  )
            byte[] bodyData = MessagePackSerializer.Serialize(reqData);

            //패킷 전송
            NetworkManager.Instance.SendPacket(PACKETID.Login_Request, bodyData);

        }
        catch (Exception ex)
        {
            Debug.LogError($"회원가입 요청 오류: {ex.Message}");
        }

    }

    public void DeleteButton(string userID)
    {
        try
        {
            //요청할 객체 생성 및 직렬화 준비 
            var reqData = new ResDelete();
            reqData.UserID = userID;

            //요청할 객체 직렬화 ( MessagePack 직렬화  )
            byte[] bodyData = MessagePackSerializer.Serialize(reqData);

            //패킷 전송
            NetworkManager.Instance.SendPacket(PACKETID.Delete_Request, bodyData);

        }
        catch (Exception ex)
        {
            Debug.LogError($"회원가입 요청 오류: {ex.Message}");
        }

    }
    public void UpdateButton(string currentUserID,string currentPassword, string updatePassword)
    {
        try
        {
            //요청할 객체 생성 및 직렬화 준비 
            var reqData = new ReqUpdatePassword();
            reqData.UserID = currentUserID;
            reqData.CurrentPassword = currentPassword;
            reqData.UpdatePassword = updatePassword;

            //요청할 객체 직렬화 ( MessagePack 직렬화  )
            byte[] bodyData = MessagePackSerializer.Serialize(reqData);

            //패킷 전송
            NetworkManager.Instance.SendPacket(PACKETID.UpdatePassword_Request, bodyData);

        }
        catch (Exception ex)
        {
            Debug.LogError($"회원가입 요청 오류: {ex.Message}");
        }
    }


}
