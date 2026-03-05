using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Define;

public class TitleScene : BaseScene
{

    private void Awake()
    {
        Init(FadeImage);
    }
    public Image FadeImage;

    protected override void Init(Image FadeImage)
    {
        Debug.Log("2");
        if(isFadeInComplete)
        {
            base.Init(FadeImage);
            SoundManager.Sound.PlayBgm(true);
        }
     
        SceneType = Define.SceneType.Title;
        
    }

    public void OnStart() //버튼으로 이벤트 실행
    {
       
       TitleFadeOut(FadeImage);

    }

    public GameObject soundOption;
    public void SoundOpenOption()
    {
        soundOption.gameObject.SetActive(true);
    }
    public void SoundCloseOption()
    {
        soundOption.gameObject.SetActive(false);
    }
    public void  TitleFadeOut(Image FadeImage)
    {
        if(isFadeOutComplete)
        {
            FadeOut(FadeImage, SceneType);
        }
   
    }
  


    public float F_time = 1.0f;
 
    public override void Clear()
    {

    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
