using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//풀링의 특징 
//- 생성 경로 부분
//1. 경로를 찾아서 GameObject 타입으로 저장한 원본 프리펩을 잘 갖고 있으면, 매번마다 public T Load<T>(string path) where : Object 을 했던 부분이 개선된다.
//2. 개선점은 원본 프리펩을 이미 갖고 있다면 바로 사용 가능하게 할 것

//- 생성 부분
//1. 매번마다 Instantiate() 호출하는 것이 아닌, 풀링 매니저가 준비된 원본 프리펩이 있는지 혹은 풀링한 프리펩이 있는지 찾는다.

//- 원본 프리펩을 받아오는 부분
//ResourceManager의 원본 프리펩을 받아오는 부분을 게임오브젝트를 반환형 타입으로 하고, string 타입으로 게임오브젝트 이름으로 접근한다. 

//- 삭제 부분
//1. 만약 풀링이 필요한 오브젝트라면 풀링 매니저에게 책임을 맡긴다.
//2. 풀링 매니저가 바로 Destroy 하는 것이 아닌 내부적으로 어딘가에 숨겨놨다가 관리한다. 
//3. 오브젝트를 생성한다면 그때 Instantiate()을 호출한다.

// - 풀링할 대상이 무엇인지 정확히 알아야 한다. 
// 1. 풀링할 대상을 명확하게 하기 위해서 빈 객체를 Pool_Root 산하에 생성하여, 같은 종류의 오브젝트들은 빈 객체 산하에 위치 시키게 할 것이다.

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index > 0)
            {
                name = name.Substring(index + 1);
            }

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
            {
                return go as T;
            }
        }
        return Resources.Load<T>(path);
    }
    public GameObject Instantiate(string path, Vector3? position = null , Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if(prefab == null)
        {
            Debug.Log("경로를 찾지 못함");
            return null;
        }
        Vector3 finalPosition = position ?? Vector3.zero; //position이 null일 때 기본값으로 설정

        if (prefab.GetComponent<Poolable>() != null)
        {
            //return Managers.Pool.Pop(prefab,parent).gameObject;
            GameObject obj = Managers.Pool.Pop(prefab, parent).gameObject;
            obj.transform.position = finalPosition;
            return obj;
        }
        else
        {
            GameObject go = Object.Instantiate(prefab, parent);
            go.name = prefab.name;
            return go;
        }
    }

    public void Destroy(GameObject go)
    {
        if(go == null)
        {
            return;
        }

        Poolable poolable = go.GetComponent<Poolable>();
        if(poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }
        else
        {
            Object.Destroy(go);
        }
       
    }
}
