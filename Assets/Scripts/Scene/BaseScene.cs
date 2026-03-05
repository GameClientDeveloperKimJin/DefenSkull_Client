using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Define;

public abstract class BaseScene : MonoBehaviour
{
    Define.SceneType sceneType;
    public Define.SceneType SceneType
    {
        get;
        protected set;
    } = Define.SceneType.Title;

    protected bool isFadeInComplete = true;
    protected bool isFadeOutComplete = false;

    protected virtual void Init(Image FadePanel)
    {
        Debug.Log("1");
        StartCoroutine(FadeInCoroutine(FadePanel));
    }
    private IEnumerator FadeInCoroutine(Image FadePanel)        //페이드 인 어두운화면->밝은 화면
    {
        Debug.Log("FadeIn");

        Color alpha = FadePanel.color;
        float FadeTime = 0f;

        isFadeInComplete = false;
        isFadeOutComplete = true;

        while (alpha.a > 0.0f)
        {
            FadeTime += Time.deltaTime / 1.0f;
            alpha.a = Mathf.Lerp(1, 0, FadeTime);
            FadePanel.color = alpha;

            yield return null;
        }

        yield return null;
    }
   
    public void FadeOut(Image FadeImage,Define.SceneType sceneType)
    {
        StartCoroutine(FadeOutCoroutine(FadeImage, sceneType));
    }
    public IEnumerator FadeOutCoroutine(Image FadeImage, Define.SceneType sceneType)    //페이드 아웃 밝은화면->어두운화면
    {
        if(isFadeInComplete == true)
        {
            yield break;
        }

        float FadeTime = 0f;
        Color alpha = FadeImage.color;

        while (alpha.a < 1.0f)
        {
            FadeTime += Time.deltaTime / 1;
            alpha.a = Mathf.Lerp(0, 1, FadeTime);
            FadeImage.color = alpha;

            yield return null;
        }
        isFadeOutComplete = false;
        isFadeInComplete = true;

        SceneMove(sceneType);


    }

    public void SceneMove(Define.SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.Title:
                Managers.Scene.LoadScene(Define.SceneType.TutorialScene);
                break;
            case SceneType.TutorialScene:
                Managers.Scene.LoadScene(Define.SceneType.Stage);
                break;
            case SceneType.Stage:
                Managers.Scene.LoadScene(Define.SceneType.PlayerDieScene);
                break;
            case SceneType.PlayerDieScene:
                Managers.Scene.LoadScene(Define.SceneType.Stage);
                break;
        }
    }
    public abstract void Clear();


}
