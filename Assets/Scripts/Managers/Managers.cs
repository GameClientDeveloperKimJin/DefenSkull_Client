using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers instance = null;

    public static Managers Instance
    {
        get
        {
            Init();
            return instance;
        }
    }


    ResourceManager resource = new ResourceManager();
    PoolManager pool = new PoolManager();
    DataManager data = new DataManager();
    SceneManagerEX scene = new SceneManagerEX();

 
    public static SceneManagerEX Scene
    {
        get
        {
            return Instance.scene ;
        }
    }
    public static DataManager Data
    {
        get
        {
            return Instance.data;
        }
    }
    public static ResourceManager Resource
    {
        get
        {
            return Instance.resource;
        }
    }
    public static PoolManager Pool
    {
        get
        {
            return Instance.pool;
        }
    }

   
    
    public static NPC  npc;

    private static void Init()
    {
        if(instance == null)
        {
            GameObject manager = GameObject.Find("@Managers");
            if(manager == null)
            {
                manager = new GameObject
                {
                    name = "@Managers"
                };
                manager.AddComponent<Managers>();
            }
            //manager가 있을 경우 
            DontDestroyOnLoad(manager); //전환되도 삭제 x
            instance = manager.GetComponent<Managers>(); //manager에 Managers 스크립트를 instance에 저장.

            instance.pool.Init();
            instance.data.Init();

        }

    
        if (npc == null)
        {
            npc = FindAnyObjectByType<NPC>();
        }
    }

  

}
