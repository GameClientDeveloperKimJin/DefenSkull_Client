using UnityEngine;

public class EnemyStat : Stat
{
    [SerializeField]
    public Define.EnemyType enemyType;

    [SerializeField]
    private float enemy_attackDamage_0;

    public float EnemyAttackDamage { get { return enemy_attackDamage_0; } set { enemy_attackDamage_0 = value; } }

    private float initialHp;
    private float initialMoveSpeed;

    private void Awake()
    {
        // 초기값 설정
        switch (enemyType)
        {
            case Define.EnemyType.TutorialEnemy:
                initialHp = 150;
                initialMoveSpeed = 0;
                break;
            case Define.EnemyType.NormalEnemy:
                initialHp = 50;
                initialMoveSpeed = 2;
                EnemyAttackDamage = 5;
                break;
            case Define.EnemyType.NormalEnemy_2F:
                initialHp = 30;
                initialMoveSpeed = 1;
                EnemyAttackDamage = 5;
                break;
            case Define.EnemyType.ShieldEnemy:
                initialHp = 70;
                initialMoveSpeed = 3;
                EnemyAttackDamage = 7;
                break;
        }

        // 초기 체력과 이동 속도를 현재 체력과 이동 속도로 설정
        hp = initialHp;
        movespeed = initialMoveSpeed;
    }


    public void ResetStats()
    {
        Hp = initialHp;
        MoveSpeed = initialMoveSpeed;
    }

    public void ApplyStageModifiers()
    {
        this.gameObject.GetComponent<EnemyController>().EnemyDieSet();
        int stageLevel = GameManager.StageLevel;

        // 초기값을 기준으로 스테이지 증가 효과 적용
        Hp = initialHp + (initialHp * 0.6f * stageLevel);

        MoveSpeed = initialMoveSpeed + (initialMoveSpeed * 0.05f * stageLevel);
        EnemyAttackDamage += EnemyAttackDamage * 0.6f * stageLevel;
    }
}
