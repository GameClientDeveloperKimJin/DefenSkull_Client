using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerStat : Stat
{

    [SerializeField]
    Define.PlayerType playerType;
    //플레이어한테만 갖고 있는 변수

    [Header("플레이어 밸런스 변수")]
    [SerializeField]
    private float ArrowBasicdamage;
    [SerializeField]
    private float ArrowCombodamage;
    [SerializeField]
    private float ArrowSkilldamage;

    [SerializeField]
    private float knockbackSpeed;

    [SerializeField]
    float StatUpgradeValue = 0.7f;

    public float BasicDamage { get { return ArrowBasicdamage; } set { ArrowBasicdamage = value; } }
    public float ComboDamage { get { return ArrowCombodamage; } set { ArrowCombodamage = value; } }
    public float SkillDamage { get { return ArrowSkilldamage; } set { ArrowSkilldamage = value; } }
    public float KnockBackSpeed { get { return knockbackSpeed; } set { knockbackSpeed = value; } }

    public float maxHp;

    private float initialBasicDamage;
    private float initialComboDamage;
    private float initialSkillDamage;
    private float initialMoveSpeed;

    void Awake()
    {
        //플레이
        hp = 100;
        maxHp = hp;

        switch(playerType)
        {
            case Define.PlayerType.Player:
                initialBasicDamage  = 8;
                initialComboDamage  = 13;
                initialSkillDamage  = 14;
                initialMoveSpeed = 8;
                KnockBackSpeed = 5;
                break;
            case Define.PlayerType.PlayerAI:
      
                break;
        }
        BasicDamage = initialBasicDamage;
        ComboDamage = initialComboDamage;
        SkillDamage = initialSkillDamage;
        movespeed = initialMoveSpeed;
    }

    public void PlayerHPSet()
    {
        Hp = 100;
        maxHp = 100;
    }

    public void ApplyStageModifiers()
    {
        int stageLevel = GameManager.StageLevel;
        Debug.Log("스테이지 레벨" + stageLevel);
       
        BasicDamage += BasicDamage * StatUpgradeValue;
        ComboDamage += ComboDamage * StatUpgradeValue;
        SkillDamage += SkillDamage * StatUpgradeValue;

    }

}
