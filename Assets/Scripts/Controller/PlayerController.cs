using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerController : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rigid;
    PlayerStat playerstat;
    Vector3 arrowPos;

  

    public bool isShoot;
    public bool isCurveShoot;
    public bool isMultipleShoot;

    public int atkCount;
    public int skillCount;

    public float attackCooldown = 0.5f; // 공격 쿨타임 설정
    private float nextAttackTime = 0f; // 다음 공격 가능 시간

    [SerializeField]
    private float CurSpeed;

    private Vector2 moveDir;

    [SerializeField]
    private float PlayerInitHP;

    private void Awake()
    {
        playerstat = gameObject.GetComponent<PlayerStat>();
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();     

        SliderHP = GameObject.Find("UI_Canvas").GetComponentInChildren<Slider>();

        isPlayerDie = false;

        CurSpeed = playerstat.MoveSpeed;
        PlayerInitHP = playerstat.Hp;

        SliderHP.value = (PlayerInitHP / playerstat.maxHp) * SliderHP.maxValue;
    }
    private void Update()
    {
        if (isMove)
        {
            arrowPos = transform.GetChild(0).position;

            if (Time.time > nextAttackTime)
            {
                if (Input.GetKeyDown(KeyCode.A) && isShoot == false)
                {
                    atkCount++;
                    StartCoroutine(PlayerShoot());
                    nextAttackTime = Time.time + attackCooldown;
                }

            }

            if (Input.GetKeyDown(KeyCode.Q) && isCurveShoot == false)
            {
                PlayerSkillShoot();
            }

            if (Input.GetKeyDown(KeyCode.W) && isMultipleShoot == false)
            {
                StartCoroutine(CreateMultiPleArrow());

            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (isRespwanStage)
            {
                PlayerDieScene playerDieScene = FindAnyObjectByType<PlayerDieScene>();
                playerDieScene.DiePlayer();
            }
            else if (isTutorialStage)
            {
                TutorialScene tutorialScene = FindAnyObjectByType<TutorialScene>();
                tutorialScene.OnStart();
            }

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.OpenPasueMenu();
        }
        if (isShoot == true)
        {
            GetComponent<CoolTime>().ArcherBasicAtk();
        }
        if (isCurveShoot == true)
        {
            GetComponent<CoolTime>().ArcherCurveAtk();
        }
        if (isMultipleShoot == true)
        {
            GetComponent<CoolTime>().ArcherMultipleAtk();
        }

    }
    void PlayerMove()
    {
        float h = Input.GetAxis("Horizontal");
        if (h > 0)
        {
            moveDir = new Vector2(h, 0);
            rigid.velocity = new Vector2(moveDir.x * CurSpeed, rigid.velocity.y);
            anim.SetFloat("PositionX", moveDir.x);
            transform.localScale = new Vector3(1, 1, 1);
        }

        else if (h < 0)
        {
            moveDir = new Vector2(-h, 0);
            rigid.velocity = new Vector2(-moveDir.x * CurSpeed, rigid.velocity.y);
            anim.SetFloat("PositionX", moveDir.x);
            transform.localScale = new Vector3(-1, 1, 1);
        }

    }
    public void PlayerStatUpdate()
    {
        playerstat.ApplyStageModifiers();
    }




    IEnumerator PlayerShoot()
    {
        anim.SetTrigger("BaseAttack");
        yield return new WaitForSeconds(0.2f);
        anim.SetFloat("PositionX", 0.0f);
        if (atkCount == 1)
        {
            Managers.Resource.Instantiate("Arrow/PlayerBasicArrow", arrowPos);
            SoundManager.Sound.PlaySfx(SoundManager.Sfx.Player_Basic_Attack);
        }
        else if (atkCount == 2)
        {
            rigid.velocity = Vector2.zero;
            Vector2 dir = transform.localScale.x > 0 ? Vector2.left : Vector2.right;
            rigid.AddForce(dir.normalized * playerstat.KnockBackSpeed, ForceMode2D.Impulse);

            Managers.Resource.Instantiate("Arrow/PlayerComboArrow", arrowPos);
            SoundManager.Sound.PlaySfx(SoundManager.Sfx.Player_Basic_Attack);

            isShoot = true;
        }


    }
    private void PlayerSkillShoot()
    {
        isCurveShoot = true;
        anim.SetTrigger("CurveAttack");
        SoundManager.Sound.PlaySfx(SoundManager.Sfx.Player_Curve_Attack);
    }

    public void CreateCurveArrow()
    {
        Managers.Resource.Instantiate("Arrow/PlayerCurveArrow", arrowPos);
        Managers.Resource.Instantiate("PlayerCurveAtkParticle", arrowPos);
    }

    public bool isMove = true;
    IEnumerator CreateMultiPleArrow()
    {
        arrowPos = transform.GetChild(0).position;
        isMove = false;
        isMultipleShoot = true;

        int MultipleAtkCount = 0;
        int Count = Random.Range(3, 7);
        
        while (true)
        {
            arrowPos = transform.GetChild(0).position;
            MultipleAtkCount++;


            anim.SetBool("SkillAttack",true);
            SoundManager.Sound.PlaySfx(SoundManager.Sfx.Player_Combo_Attack);

            Managers.Resource.Instantiate("Arrow/PlayerMultiPleArrow", arrowPos);
            Managers.Resource.Instantiate("PlayerMultiPleAtkParticle", arrowPos);
            yield return new WaitForSeconds(0.5f);

            if(MultipleAtkCount > Count)
            {
                isMove = true;
                anim.SetBool("SkillAttack", false);
                anim.SetFloat("PositionX", 0.0f);
                yield break;
            }
        }
    }

   

   
    public Slider SliderHP;
    float currentHp;
    public void OnDamaged(float damage)
    {
        

        playerstat = gameObject.GetComponent<PlayerStat>();
        playerstat.Hp -= damage;

        Debug.Log("플레이어 현재 체력" + playerstat.Hp);
        SliderHP.value = (playerstat.Hp / playerstat.maxHp) * SliderHP.maxValue;

        if (playerstat.Hp <= 0.0f )
        {
            OnDie();

        }
       
    }

    public bool isPlayerDie;
    public void OnDie()
    {
        isCurveShoot = false;
        isMultipleShoot = false;
        isShoot = false;
        DialogueManager.isPlayerSkill = false;

        isPlayerDie = true;
  
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SoundManager.Sound.PlaySfx(SoundManager.Sfx.Player_Die);
            player.GetComponent<Animator>().SetTrigger("PlayerDie");
            Debug.Log("플레이어 죽는 애니메이션 실행" + this.gameObject.name);
        }
       

        Debug.Log("플레이어 죽음");
    }


    public GameObject player;


    public void DestroyPlayer()
    {
        //GameScene game = FindObjectOfType<GameScene>();
        //game.DiePlayer();

        GameManager.Instance.GameOverUI();
        anim.enabled = false;
        GameManager.Instance.DisableAllEnemies();
              
    }
    private void FixedUpdate()
    {
        if (isMove == true && isPlayerDie == false)
        {
            PlayerMove();
        }

    }

    [SerializeField]
    private bool isTutorialStage = false;
    [SerializeField]
    private bool isRespwanStage = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "TutorialPortal")
        {
            isTutorialStage = true;
        }
        if (collision.gameObject.name == "RespawnPortal")
        {
            isRespwanStage = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "TutorialPortal")
        {
            isTutorialStage = false;
        }
        if (collision.gameObject.tag == "RespawnPortal")
        {
            isRespwanStage = false;
        }
    }

}