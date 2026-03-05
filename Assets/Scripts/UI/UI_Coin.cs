using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Coin : MonoBehaviour
{
    public Transform CoinParent; //Coin_Image의 위치를 CoinParent 산하에 배치
    public Transform CoinStart; //CoinParent산하에 배치된 Coin_Image의 시작 위치

    public Transform coinEnd;
    public float moveDuration;
    public Ease moveEase;

    public int coinAmount;

    //코인 10개가 한번에 움직이기 때문에 입체감이 없음. 따라서, 딜레이 기능을 추가하여 연달아 움직일 수 있게 끔 한다.
    public float coinPerDelay;


    public void OnGetButtonClicked()
    {

        for (int i = 0; i < coinAmount; i++)
        {
            var targetDelay = i * coinPerDelay;
            ShowCoin(targetDelay);
        }

    }

  
    public void ShowCoin(float delay)
    {
        Vector2 coinPos = CoinParent.transform.position;

        var offset = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), 0f);

        var startPos = offset + CoinStart.transform.position;

        var coinObject = Managers.Resource.Instantiate("UI/Coin_Image", coinPos, CoinParent);

        coinObject.transform.position = startPos;

        coinObject.transform.localScale = new Vector3(1f, 1f, 1f);
        coinObject.transform.DOScale(Vector3.one, delay); 

        coinObject.transform.DOMove(coinEnd.position, moveDuration).SetEase(moveEase).SetDelay(delay).
            OnComplete( () => { Managers.Resource.Destroy(coinObject); } );
    }
}
