using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankItem : MonoBehaviour
{
    [SerializeField]
    TMP_Text UserTMP;

    [SerializeField]
    TMP_Text ScoreRankTMP;

    [SerializeField]
    bool isMyUser;

    public void SetRankItem(string userID,string score,string rank)
    {
        UserTMP.text = userID;
        ScoreRankTMP.text = $"{score}/{rank}";

        if(userID == NetworkManager.Instance.GetUserID())
        {
            isMyUser = true;
        }
        else
        {
            isMyUser = false;
        }
        
        UserTMP.color = isMyUser ? Color.red : Color.white;
    }
}
