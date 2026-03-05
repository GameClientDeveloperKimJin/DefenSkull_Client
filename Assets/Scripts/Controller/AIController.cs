using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    Animator anim;
    Vector3 arrowPos;
    void Start()
    {
        anim = GetComponent<Animator>();
        StartAIShoot();
    }

    void Update()
    {
        
    }

    public void StartAIShoot()
    {

        InvokeRepeating("AIShoot", 1.0f, 2.0f);


    }
    public void StopAIShoot()
    {
        CancelInvoke("AIShoot");
    }
    private void AIShoot()
    {

        if (this.gameObject.GetComponent<NPC>().isTalk == true)
        {
            StopAIShoot();
            return;
        }


        anim.SetTrigger("FirstAttack");
        arrowPos = transform.GetChild(0).position;
        GameObject aiArrow = Managers.Resource.Instantiate("Arrow/AiArrow", arrowPos);
        Arrow arrow = aiArrow.GetComponent<Arrow>();

        Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        arrow.Initialize(dir);
    }
}
