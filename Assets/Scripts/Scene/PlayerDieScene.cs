using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDieScene : BaseScene
{
    private void Awake()
    {
        GameManager.Instance.DisableAllEnemies(); //모든적 비활성화
        Init(FadeImage);
        StartCoroutine(GameManager.Instance.PlayerVisiable());
    }
    protected override void Init(Image FadeImage)
    {
        if (isFadeInComplete)
        {
            base.Init(FadeImage);
        }
        DialogueManager.isPlayerSkill = true;
        SceneType = Define.SceneType.PlayerDieScene;
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
