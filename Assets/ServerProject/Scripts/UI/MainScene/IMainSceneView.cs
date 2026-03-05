using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMainSceneView
{
    public void OnStartButton();
    public void OnCreateRoomButton();
    public void OnJoinRoomButton();
    public void OnJoinRoomFinishButton();
    public void OnSendMessageButton();

    public void OnLeaveRoomButton();
    public void ShowChatMessage(string userId, string message);

    public string ChatJoinRoomName { get; set; }

}
