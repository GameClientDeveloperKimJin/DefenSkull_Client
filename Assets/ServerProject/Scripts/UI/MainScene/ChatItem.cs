using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatItem : MonoBehaviour
{
    [SerializeField]
    TMP_Text ChatMessage;

    public void SetChatItem(string userID, string chatMessage)
    {
        ChatMessage.text = $"{userID} : {chatMessage}";
    }
}
