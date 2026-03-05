using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScene : BaseScene
{

  
    private void Awake()
    {
        DialogueManager.isPlayerSkill = true;

        GameManager.Instance.playerInfo.gameObject.SetActive(true);
        GameManager.StageLevel = 1;
        SoundManager.Sound.PlayBgm(true);
        //GameManager.totalEnemies = 0;
        GameManager.tutorialEnemyDie = 0; //스테이지씬으로 왔으니 이전스테이지 죽은 적 횟수 초기화. 

        GameManager.Instance.CreateEnemyOption();

        
        Init(FadeImage);
    }

    public GameObject Npc;
    protected override void Init(Image FadeImage)
    {
        if (isFadeInComplete)
        {
            base.Init(FadeImage);
        }
        SceneType = Define.SceneType.Stage;


        StartCoroutine(GameManager.Instance.PlayerVisiable());
    }

    public Image FadeImage;

    public void DiePlayer()
    {
        FadeOut(FadeImage, SceneType);
    }
    public override void Clear()
    {

    }
}
