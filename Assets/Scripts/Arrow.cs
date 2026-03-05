using Data;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private float BasicArrowspeed;
    [SerializeField]
    private float ComboArrowSpeed;
    [SerializeField]
    private float CurveArrowSpeed;
    [SerializeField]
    private float MultipleArrowSpeed;

    [SerializeField]
    Define.ArrowType arrowType;

    Transform enemy;
    GameObject player;
    GameObject tile;
    PlayerStat playerstat;
    float move_x_rate;
    float move_y_rate;

    Vector2 PlayerArrowDir;

    int bounceCount = 0;
    bool isBouncing = false;  // 초기에는 직선 이동 상태

    
    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerstat = player.GetComponent<PlayerStat>();

            // 방향 설정
            if (player.transform.localScale.x > 0)
            {
                PlayerArrowDir = Vector2.right;
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                PlayerArrowDir = Vector2.left;
                transform.localScale = new Vector3(-1, 1, 1);
            }

        }
        else
        {
            Debug.LogWarning("Player 객체를 찾을 수 없습니다.");
            // 필요한 경우, 플레이어가 없을 때의 대체 동작 추가
        }



        if (tile == null)
        {
            tile = GameObject.FindGameObjectWithTag("TileCollider");
            backgroundBounds = tile.GetComponent<TilemapCollider2D>();
        }

   

        bounceCount = 0;
        isBouncing = false; // 화살이 활성화될 때마다 초기화
                            // z축 회전 초기화
        transform.rotation = Quaternion.Euler(0, 0, 0);



    }

    private Vector2 AiArrowDir;

    public void Initialize(Vector2 direction)
    {
        AiArrowDir = direction;
        transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
    }
    private void Update()
    {
        if (player == null)
        {
            ArrowDestroy();
            return; // 더 이상 업데이트하지 않음
        }
        switch (arrowType)
        {
            case Define.ArrowType.AI:
                transform.Translate(AiArrowDir * BasicArrowspeed * Time.deltaTime);
                break;
            case Define.ArrowType.Basic:
                if (isBouncing == false) //타일 충돌 x 
                {
                    transform.Translate(PlayerArrowDir * BasicArrowspeed * Time.deltaTime); //직선으로이동
                }
                break;
            case Define.ArrowType.Combo:
                transform.Translate(PlayerArrowDir * ComboArrowSpeed * Time.deltaTime);
                break;
            case Define.ArrowType.Curve:
                if (isBouncing == true)
                {
                    CurveArrow(); // 곡선 이동 메서드 호출
                    float angle = Mathf.Atan2(move_y_rate, move_x_rate) * Mathf.Rad2Deg;
                    //Mathf.Atan2(y,x) : y와 x를 벡터 성분으로 만듦. 즉, y는 상하 방향 x는 좌우 방향                                                                                     
                    //Mathf.Atan2(move_y_rate, move_x_rate)을 통해 라디안 값을 계산하고,Mathf.Rad2Deg을 통해 라디안 값을 각도로 변환. 
                    //각도를 angle에 저장
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                }     
                else
                {
                    transform.Translate(PlayerArrowDir * CurveArrowSpeed * Time.deltaTime); //타일과 충돌전까지 직선으로 이동
                }
                break;
            case Define.ArrowType.Multiple:
                transform.Translate(PlayerArrowDir * MultipleArrowSpeed * Time.deltaTime);
                break;

        }
       
       
    }
     TilemapCollider2D backgroundBounds;  // 배경의 BoxCollider2D를 할당
    void CurveArrow()
    {
        transform.Translate(Vector3.right * Time.deltaTime * CurveArrowSpeed * move_x_rate, Space.World);
        transform.Translate(Vector3.up * Time.deltaTime * CurveArrowSpeed * move_y_rate, Space.World);

        Vector3 position = transform.position;

        // 현재 화살 위치의 x축이 타일의 x축 최솟값(수평선 기준 맨 왼쪽)보다 작을 경우,
        // 현재 화살 위치의 x축이 타일의 x축 최대값(수평선 기준 맨 오른쪽)보다 클 경우
        // => 방향 전환 (-move_x_rate하여 x축의 방향 전환)
        if (position.x <= backgroundBounds.bounds.min.x || position.x >= backgroundBounds.bounds.max.x)
        {
            move_x_rate = -move_x_rate;
        }
        // 현재 화살 위치의 y축이 타일의 x축 최솟값(수직선 기준 맨 아래쪽)보다 작을 경우,
        // 현재 화살 위치의 y축이 타일의 y축 최대값(수직선 기준 맨 윗쪽)보다 클 경우
        // => 방향 전환 (-move_y_rate하여 y축의 방향 전환)
        if (position.y <= backgroundBounds.bounds.min.y || position.y >= backgroundBounds.bounds.max.y)
        {
            move_y_rate = -move_y_rate;
        }

        float angle = Mathf.Atan2(move_y_rate, move_x_rate) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // z축 회전을 0으로 고정
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,
            Mathf.Clamp(transform.rotation.eulerAngles.z, -45f, 45f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            ArrowDamageCalculate(collision);

            HitEnemyCreateParticle(collision);
        }
        else if(collision.CompareTag("TrainingEnemy"))
        {
            HitEnemyCreateParticle(collision);
        }
        else if (collision.CompareTag("TileCollider"))
        {
            if(arrowType == Define.ArrowType.Curve)
            {
                if (isBouncing == false)
                {
                    StartBouncing();  //곡선 
                }
                else
                {
                    BounceArrow(); //타일 충돌 시 무조건 방향 전환.
                }
            } 
            else
            {
                 ArrowDestroy();
            }
        }
    }

    void StartBouncing()
    {
        isBouncing = true;

        // 곡선 이동을 위한 초기 랜덤 값 설정
        move_x_rate = Random.Range(-1.0f, 1.0f); //-1.0~1.0까지 랜덤
        move_y_rate = Random.Range(-1.0f, 1.0f); //-1.0~1.0까지 랜덤

        // move_x_rate와 move_y_rate가 0이 되면, 이동속도가 0이 되므로 움직이질 않음. 따라서 0.3보다 작을 경우 0.3을 곱해준다. 0.3보다 클 경우
        // move_x_rate와 move_y_rate에 원래 값을 저장하여 유지.
        move_x_rate = Mathf.Abs(move_x_rate) < 0.3f ? Mathf.Sign(move_x_rate) * 0.3f : move_x_rate;
        //move_x_rate가 -0.3일 경우 절댓값 함수를 통해 0.3이 되었지만, Mathf.Sign(move_x_rate)에 의해 move_x_rate가 -0.3 이므로 음수이다. 따라서 -1을 반환하고 0.3을 곱해 -0.3

        move_y_rate = Mathf.Abs(move_y_rate) < 0.3f ? Mathf.Sign(move_y_rate) * 0.3f : move_y_rate;

        //Mathf.Sign(move_x_rate): 부호를 반환하는 함수. move_x_rate가 음수 일 경우 -1을 반환. 양수 일 경우 +1을 밚놘

    }

    void BounceArrow()
    {
        move_x_rate = -move_x_rate;
        move_y_rate = -move_y_rate;

        bounceCount++;

        if (bounceCount >= 3)
        {
            ArrowDestroy();
        }
    }

  

    void ArrowDestroy()
    {
        Managers.Resource.Destroy(gameObject);
    }

    void HitEnemyCreateParticle(Collider2D collision)
    {
        Managers.Resource.Instantiate("HitParticle", collision.transform.position);
        if (arrowType != Define.ArrowType.Curve)
        {
            ArrowDestroy(); // Curve 화살이 아닐 때만 파괴
        }
    }
    private void ArrowDamageCalculate(Collider2D collision)
    {
       
        switch (arrowType)
        {
            case Define.ArrowType.AI:
            case Define.ArrowType.Basic:
                collision.GetComponent<EnemyController>().OnDamaged(playerstat.BasicDamage, collision.transform.position);
                break;
            case Define.ArrowType.Combo:
                collision.GetComponent<EnemyController>().OnDamaged(playerstat.ComboDamage, collision.transform.position);
                break;
            case Define.ArrowType.Curve:          
            case Define.ArrowType.Multiple:
                collision.GetComponent<EnemyController>().OnDamaged(playerstat.SkillDamage, collision.transform.position);
                break;
        }

    
    }
}
