using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotalController : MonoBehaviour
{
    public GameObject targetObj;
    public GameObject toObj;

  
    private void Update()
    {
        Collider2D[] colliderPlayer = Physics2D.OverlapBoxAll(portalPos.position, portalSize, 0, LayerMask.GetMask("Player"));
        
        foreach (Collider2D collider in colliderPlayer)
        {
            if(Vector2.Distance(transform.position,collider.transform.position) < 3.0f)
            {
                targetObj = collider.gameObject;

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                 
                    targetObj.transform.position = toObj.transform.position;
                }
            }
         
        }

       
    }

   

    public Transform portalPos;
    public Vector2 portalSize;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(portalPos.position, portalSize);//DrawWireCube(pos.position,boxsize)

    }
}
