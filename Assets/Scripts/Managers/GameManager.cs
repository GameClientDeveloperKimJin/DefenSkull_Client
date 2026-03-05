using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("GameManager");
                    instance = managerObject.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    [SerializeField]
    GameSceneView gameSceneView;

    Vector2 SpawnerVec_1F;
    Vector2 SpawnerVec_2F;
    public Transform EnemySpawner1F;
    public Transform EnemySpawner2F;

    [SerializeField]
    Define.SceneType sceneType;

    public GameObject playerInfo;

    public GameObject pasueMenu;

    [SerializeField]
    private int NormalEnemyCreateCount;
    public static int totalEnemies; // 총 생성된 적 수

    public static int tutorialEnemyDie = 0;
    public static int StageLevel { get; set; } = 1;

    public GameObject gameOver;
    public Text stageLevel;
    public Text stageLevelTilte;

    [SerializeField]
    GameObject RankingImage;

    [SerializeField]
    GameObject WinnerUI;
    [SerializeField]
    Button WinnerButton;

    [SerializeField]
    bool isPlayerDead;

    [SerializeField]
    GameObject Player;


    [SerializeField]
    int MaxRound; //최대 라운드 설정

    private void Start()
    {
        //플레이어 끼리 충돌 무시
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Player"), true);

        //적과 플레이어, 적과 적 충돌 무시
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);

        //적 스폰 위치 설정
        SpawnerVec_1F = EnemySpawner1F.transform.position;
        SpawnerVec_2F = EnemySpawner2F.transform.position;

        //적 몇마리 소환할지 설정 


        Debug.Log("현재 총 적은?" + totalEnemies);
        Debug.Log("현재 죽은 적은?" + tutorialEnemyDie);


    }

    public void CreateEnemyOption()
    {

        SpawnerVec_1F = EnemySpawner1F.transform.position;
        SpawnerVec_2F = EnemySpawner2F.transform.position;

        switch (sceneType)
        {
            case Define.SceneType.TutorialScene:
                totalEnemies = (NormalEnemyCreateCount * 2) + 1;
                Debug.Log("튜토리얼 씬의 적은 총 몇명?" + totalEnemies);
                StartCoroutine(CreateNormalEnemy(NormalEnemyCreateCount));
                break;
            case Define.SceneType.Stage:
                NormalEnemyCreateCount = UnityEngine.Random.Range(4, 6);
                totalEnemies = (NormalEnemyCreateCount * 2);
                Debug.Log("Stage 씬의 적은 총 몇명?" + totalEnemies);
                StartCoroutine(CreateNormalEnemy(NormalEnemyCreateCount));
                break;
            case Define.SceneType.PlayerDieScene:
                break;
        }
    }


    public void ScoreUpdate(int score)
    {
        gameSceneView.NetworkUpdataeRequest(score);
    } 
  
    public void AllEnemyDie()
    {
        if (Managers.npc != null)
        {
            Managers.npc.AIShootOption(true);
        }


        if (sceneType == Define.SceneType.TutorialScene)
        {
            Managers.Resource.Instantiate("TutorialPortal");
        }
        else
        {
            if(StageLevel == MaxRound)
            {
                WinnerUI.gameObject.SetActive(true);
                RankingImage.gameObject.SetActive(true);

                gameSceneView.NetworkGetRankerRequest(); //네트워크 - 랭킹 조회

                Extension.ResetListener(WinnerButton, WinnerPlayer);
                return;
            }
            Debug.Log("스테이지씬 적 모두 죽음");
            NextLevel();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<PlayerController>().PlayerStatUpdate();

            tutorialEnemyDie = 0;
        }


    }


    /// <summary>
    /// 적 생성 코루틴 , 1층 - 적 , 2층  - 적
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    IEnumerator CreateNormalEnemy(int count)
    {
        yield return new WaitForSeconds(2.0f);

        if(Managers.npc != null)
        {
            Managers.npc.AIShootOption(false);
        }
       


        while (count > 0)
        {
            int spawnTime = UnityEngine.Random.Range(1, 5);

            // 1층과 2층 적을 생성하고, 초기화 메서드를 호출합니다.
            GameObject enemy1F = Managers.Resource.Instantiate("Enemy/NormalEnemy", SpawnerVec_1F);
            GameObject enemy2F = Managers.Resource.Instantiate("Enemy/NormalEnemy_2F", SpawnerVec_2F);

            // 각 적의 상태를 초기화
            EnemyStat enemyStat1F = enemy1F.GetComponent<EnemyStat>();
            if (enemyStat1F != null)
                enemyStat1F.ApplyStageModifiers();

            EnemyStat enemyStat2F = enemy2F.GetComponent<EnemyStat>();
            if (enemyStat2F != null)
                enemyStat2F.ApplyStageModifiers();

            count--;

            yield return new WaitForSeconds(spawnTime);

        }
      
    }

    public void NextLevel()
    {
        Debug.Log("스테이지 레벨 증가 ");
        StageLevel += 1;
        stageLevelTilte.text = "Stage Level " + StageLevel.ToString();
        CreateEnemyOption();
    }
    public void DisableAllEnemies()
    {
     
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");


        foreach (GameObject enemy in enemies)
        {
            Managers.Resource.Destroy(enemy);
        }

     
    }
    public IEnumerator PlayerVisiable()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector2 playerVec = player.transform.position;

        Managers.Resource.Instantiate("PlayerVisibleParticle", playerVec);

        player.GetComponent<PlayerController>().isMove = false;

        yield return new WaitForSeconds(3.0f);

        SpriteRenderer Playercolor = player.GetComponent<SpriteRenderer>();

        Color color = Playercolor.color;
        float time = 0;

        while (color.a < 1.0f)
        {
            time += Time.deltaTime / 1.0f;
            color.a = Mathf.Lerp(0, 1, time);
            Playercolor.color = color;
            yield return null;
        }
        player.GetComponent<PlayerController>().isMove = true;

        yield return null;
    }


 /// <summary>
 /// 라운드 모두 클리어한 상태에서 메인화면 이동 버튼 클릭 시
 /// </summary>
    private void WinnerPlayer()
    {
        PlayerStat playerStat = Player.GetComponent<PlayerStat>();

        playerStat.PlayerHPSet();

        RankingImage.gameObject.SetActive(false);

        WinnerUI.gameObject.SetActive(false);

        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// 게임 오버 창 활성화
    /// </summary>
    public void GameOverUI()
    {
        isPlayerDead = true;

        RankingImage.gameObject.SetActive(true);

        gameSceneView.NetworkGetRankerRequest(); //네트워크 - 랭킹 조회

        gameOver.SetActive(true);

        stageLevel.text = "현재 스테이지     " + StageLevel.ToString();
    }


    


    /// <summary>
    /// 게임 오버 창 - 메인화면 이동 , 에디터에서 버튼 이벤트로 이동
    /// </summary>
    public void MainSceneMove()
    {
        gameSceneView.LocalScoreSet();

        PlayerStat playerStat = Player.GetComponent<PlayerStat>();

        playerStat.PlayerHPSet();

        isPlayerDead = false;

        RankingImage.gameObject.SetActive(false);
        gameOver.SetActive(false);

        //gameSceneView.NetworkScoreSet(); //네트워크 - 점수 초기화

        Managers.Resource.Destroy(Player); ///플레이어 삭제

        SceneManager.LoadScene(1);
    }
    /// <summary>
    /// 일시정지 창 활성화 
    /// </summary>
    public void OpenPasueMenu()
    {
        if(isPlayerDead) //죽었을 경우 일시정지 창 활성화 x
        {
            return;
        }
        Time.timeScale = 0.0f;
        pasueMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// 일시정지 창 - 계속하기, 에디터에서 버튼 이벤트로 호출
    /// </summary>
    public void ResumeBtn()
    {
        Time.timeScale = 1.0f;
        pasueMenu.gameObject.SetActive(false);
    }

    /// <summary>
    /// 일시정지 창 - 나가기, 에디터에서 버튼 이벤트로 호출
    /// </summary>
    public void ExitBtn()
    {
        Time.timeScale = 1.0f;

        gameSceneView.NetworkUpdataeRequest(0); //네트워크 - 0으로 초기화


        SceneManager.LoadScene(1);
    }
    /// <summary>
    /// 일시정지 창 - 종료, 에디터에서 버튼 이벤트로 호출
    /// </summary>

    public void OnQuit()
    {
        gameSceneView.NetworkUpdataeRequest(0); //네트워크 -  0으로 초기화

        Application.Quit();
    }
}
