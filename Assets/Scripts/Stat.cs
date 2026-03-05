using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    //플레이어(Warrior,Archer)가 갖고 있어야 하는 변수
    //1. 체력 
    //2. 이동속도 
    //3. WarriorDamage
    //4. ArcherDamage
    
    //적
    //1. 체력
    //2. 이동속도
    //3. 공격 데미지
    [SerializeField]
    protected float hp;
    [SerializeField]
    protected float movespeed;

   
    public float Hp {  get {   return hp;  } set { hp = value;} }
    public float MoveSpeed{  get {   return movespeed;  } set { movespeed = value;  } }

}
