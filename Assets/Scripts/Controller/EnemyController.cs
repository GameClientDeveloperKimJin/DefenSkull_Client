using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Animator anim;
    Vector2 mydir;
    Vector2 pointdir;

    EnemyStat enemyStat;

    CapsuleCollider2D capsuleColl;
    Rigidbody2D rigid;
    private void Awake()
    {
        
    }

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        enemyStat = gameObject.GetComponent<EnemyStat>();
        capsuleColl = gameObject.GetComponent<CapsuleCollider2D>();
        rigid = gameObject.GetComponent<Rigidbody2D>();



        mydir = transform.position;

        GameObject enemyPoint = GameObject.FindGameObjectWithTag("EnemyPoint");
        pointdir = enemyPoint.transform.position;
    }

    public void EnemyDieSet() //죽고나서 적의 rigidbody와 collider 리셋
    {
        this.GetComponent<CapsuleCollider2D>().enabled = true;
        this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
    }


    private void Update()
    {
        Vector2 dir = pointdir - mydir;
        transform.Translate(dir.normalized * enemyStat.MoveSpeed * Time.deltaTime);

    }


    public bool isTutorialEnemyDie;
    public void OnDamaged(float damage,Vector3 collision)
    {
        enemyStat.Hp -= damage;
        Debug.Log($"[OnDamaged] 데미지 적용 후 HP={enemyStat.Hp}");

        if (enemyStat.Hp <= 0)
        {
            GameManager.Instance.ScoreUpdate(1);

            if (enemyStat.enemyType.Equals(Define.EnemyType.TutorialEnemy))
            {
                //  Managers.npc.SecondDialogue(); //2번째 대화내용으로 전환
                isTutorialEnemyDie = true;
                NPC npc = FindAnyObjectByType<NPC>();
                npc.AIShootOption(true); //npc 공격 중지
            }
            OnDie();
        }
        SoundManager.Sound.PlaySfx(SoundManager.Sfx.Skull_Hit);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyPoint"))
        {
            OnDie();

            //적에게 데미지 주는 코드
            collision.gameObject.GetComponent<Animator>().Play("Portal_Open_Anim");
            PlayerController player = FindObjectOfType<PlayerController>();
            if(player != null)
            {
                Debug.Log(GameManager.StageLevel);
                if(GameManager.StageLevel != 0 )
                {
                    player.OnDamaged(enemyStat.EnemyAttackDamage);
                }
              
            }


        }
    }

    
    private void OnDie()
    {
        enemyStat.MoveSpeed = 0;
        anim.SetTrigger("Skull_0_Die");
        SoundManager.Sound.PlaySfx(SoundManager.Sfx.Skull_Die);

    }

    public void CreateDeathParticle()
    {
        capsuleColl.enabled = false;
        rigid.gravityScale = 0.0f;

        GameManager.tutorialEnemyDie++; // GameManager를 통해 정적 변수에 접근
        Debug.Log("죽인 적" + GameManager.tutorialEnemyDie);
        Debug.Log("모든 적" + GameManager.totalEnemies);
        // 총 적 수와 비교하여 모두 죽었는지 확인
        if (GameManager.tutorialEnemyDie == GameManager.totalEnemies)
        {
            GameManager.Instance.AllEnemyDie();
        }

        Managers.Resource.Instantiate("deathParticle", transform.position);
      
        Invoke("DestroySkull", 0.5f);
    }

    public void DestroySkull()
    {
        Managers.Resource.Destroy(this.gameObject);
    }

  

}
