using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScene : BaseScene
{
    private void Awake()
    {
        GameManager.Instance.playerInfo.gameObject.SetActive(false);
        GameManager.StageLevel = 0;
        SoundManager.Sound.PlayBgm(true);
        Init(FadeImage);
        StartCoroutine(GameManager.Instance.PlayerVisiable());
        StartCoroutine(UpArrowKey());
    }
    protected override void Init(Image FadeImage)
    {
        if (isFadeInComplete)
        {
            base.Init(FadeImage);
        }
        SceneType = Define.SceneType.TutorialScene;
    }

    public Image FadeImage;
    public GameObject[] UparrowImage;
    [SerializeField]
    private float blinkSpeed;

    public void OnStart() //버튼으로 이벤트 실행
    {
        TitleFadeOut(FadeImage);

    }

    public void TitleFadeOut(Image FadeImage)
    {
        if (isFadeOutComplete)
        {
            FadeOut(FadeImage, SceneType);
        }

    }

    IEnumerator UpArrowKey()
    {
        float startTime = Time.time;

        while (true)
        {
            float elapsedTime = Time.time;

            foreach (GameObject arrowParent in UparrowImage)
            {
                // 부모 오브젝트의 알파 값을 감소
                Color parentColor = arrowParent.GetComponent<SpriteRenderer>().color;
                parentColor.a = Mathf.PingPong(elapsedTime * blinkSpeed, 1.0f);
                arrowParent.GetComponent<SpriteRenderer>().color = parentColor;

            }

            yield return null; // 한 프레임 대기
        }
    }
    public override void Clear()
    {

    }
}
