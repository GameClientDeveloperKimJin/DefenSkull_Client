using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameSceneView : MonoBehaviour
{

    [SerializeField]
    TMP_Text currentTMP;


    [SerializeField]
    TMP_Text currentNickName;

     public int localScore;


    [Header("랭킹 관련")]
    [SerializeField]
    RankItem rankItem;
    [SerializeField]
    Transform rankItemContent; //채팅 프리펩 부모

    string userID;

    private void Start()
    {
        NetworkManager.Instance.errorCode += NetworkResponse;
        NetworkManager.Instance.rankTopUsers += NetworkGetRankerResponse; //순위 업데이트 응답 메서드 등록

        userID = NetworkManager.Instance.GetUserID();

        currentTMP.text = $"점수 : {0}";
        currentNickName.text = $"닉네임 : {userID}";
    }

    private void NetworkResponse(ErrorCode errorCode)
    {
        Debug.Log("점수 상태: " + errorCode);
    }

    public void LocalScoreSet()
    {
        localScore = 0;
    }


    /// <summary>
    /// 점수 업데이트 요청
    /// </summary>
    /// <param name="score"></param>
    public void NetworkUpdataeRequest(int score)
    {
        localScore += score;

        currentTMP.text = $"점수 : {localScore.ToString()}";

        try
        {
             userID = NetworkManager.Instance.GetUserID();

            var reqData = new ReqUpdateScore();
            reqData.UserID = userID;
            reqData.Score = localScore;

            var bodyData = MessagePackSerializer.Serialize(reqData);
            NetworkManager.Instance.SendPacket(PACKETID.UpdateScore_Request, bodyData);
        }
        catch (Exception e)
        {
            Debug.LogError($"점수 업데이트 요청 오류: {e.Message}");
        }


    }

    /// <summary>
    /// 순위 업데이트 응답 메서드
    /// </summary>
    /// <param name="rankingDatas"></param>
    private void NetworkGetRankerResponse(List<RankingData> rankingDatas)
    {
        Debug.Log("NetworkGetRankerResponse");

        ClearRankingUI();


        foreach (var rankingData in rankingDatas)
        {
            RankItem rank = Instantiate(rankItem, rankItemContent);

            if (rankingData.Score == 0)
            {
                Debug.Log("점수 0일 때: " + rankingData.UserId);
                rank.SetRankItem(rankingData.UserId, rankingData.Score.ToString(), "-");
            }
            else
            {
                Debug.Log("점수 0이 아닐 때: " + rankingData.UserId);
                rank.SetRankItem(rankingData.UserId, rankingData.Score.ToString(), rankingData.Rank.ToString());
            }
        }
    }
    /// <summary>
    /// 상위 유저 점수 순위 요청
    /// </summary>
    public void NetworkGetRankerRequest()
    {
        try
        {
            var reqData = new ReqGetTopRankers();
            reqData.Count = 10;

            var bodyData = MessagePackSerializer.Serialize(reqData);
            NetworkManager.Instance.SendPacket(PACKETID.GetTopRankers_Request, bodyData);
        }
        catch (Exception e)
        {
            Debug.LogError($"순위 가져오기 요청 오류: {e.Message}");
        }
    }

    private void ClearRankingUI()
    {
        // 기존 랭킹 UI 전부 삭제
        foreach (Transform child in rankItemContent)
        {
            Destroy(child.gameObject);
        }
    }
    private void OnDestroy()
    {
        NetworkManager.Instance.errorCode -= NetworkResponse;
        NetworkManager.Instance.rankTopUsers -= NetworkGetRankerResponse; //순위 업데이트 응답 메서드 등록
    }

    private void OnApplicationQuit()
    {
        NetworkManager.Instance.errorCode -= NetworkResponse;
        NetworkManager.Instance.rankTopUsers -= NetworkGetRankerResponse; //순위 업데이트 응답 메서드 등록
    }
}
