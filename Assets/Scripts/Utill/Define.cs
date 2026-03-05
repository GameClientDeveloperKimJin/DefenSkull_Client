using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour
{
    public enum PlayerType
    {
        Player,
        PlayerAI,
    }

    public enum EnemyType
    {
        TutorialEnemy,
        NormalEnemy,
        NormalEnemy_2F,
        ShieldEnemy,

    }

    public enum ArrowType
    {
        Basic, //기본 화살
        Combo, //콤보 화살
        Curve, //커브있는 화살
        Multiple,// 연발 기능 있는 화살
        AI,
    }
    public enum SceneType
    {
        Title,
        TutorialScene,
        Stage,
        PlayerDieScene,
    }


}
