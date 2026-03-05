using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    private static RankingManager instance;

    private static readonly object _lock = new object();
    public static RankingManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<RankingManager>();

                    if (instance == null)
                    {
                        GameObject obj = new GameObject("RankingManager");
                        instance = obj.AddComponent<RankingManager>();
                    }
                }

                return instance;
            }


        }
    }

    [SerializeField]
    TMP_Text currentTMP;

    [SerializeField]
    int localScore;


    /// <summary>
    /// 싱글톤 초기화
    /// </summary>
    private void Awake()
    {
        lock (_lock)
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject); // 중복 생성 방지
            }
        }
    }
    private void Start()
    {
        NetworkManager.Instance.errorCode += NetworkResponse;

        currentTMP.text = $"현재 내 점수 : {0}";
    }
    /// <summary>
    /// 네트워크 매니저로 부터 받은 응답 결과 분리
    /// </summary>
    /// <param name="errorCode"></param>
    public void NetworkResponse(ErrorCode errorCode)
    {
        switch (errorCode)
        {
            case ErrorCode.UpdateScore_Success: //점수 요청 성공 시
                //NetworkGetRankerRequest();
                break;
        }
    }



 





    private void OnDestroy()
    {
        NetworkManager.Instance.errorCode -= NetworkResponse;
    }

    private void OnApplicationQuit()
    {
        NetworkManager.Instance.errorCode -= NetworkResponse;
    }
}
