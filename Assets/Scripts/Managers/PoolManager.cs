using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager 
{
    class Pool //Pool을 관리
    {
        public GameObject Original
        {
            get;
            private set;
        }

        public Transform Root
        {
            get;
            set;
        }

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject original,int count = 7)
        {
            this.Original = original;

            Root = new GameObject().transform;

            Root.name = $"{original.name}_Root";

            for(int i = 0; i< count; i++)
            {
                Push(Create());
            }
        }

    
        private Poolable Create()
        {
            
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;

            return go.GetComponent<Poolable>();
        }

        public void Push(Poolable poolable)
        {
            if(poolable == null)
            {
                return;
            }

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            poolable.isUsing = false;

            _poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            Poolable poolable;

            if(_poolStack.Count > 0)
            {
                poolable = _poolStack.Pop();
            }
            else
            {
                poolable = Create();
            }

            poolable.gameObject.SetActive(true);
            //if (parent == null) //풀링된 원본객체 프리펩의 부모 오브젝트가 없을 수 있다. 해당 프리펩은 DonDestroyOnLoad가 걸려있을 텐데, 
            //{
            //    poolable.transform.parent = Managers.Game.CurrentScene.transform;
            //}
            poolable.transform.parent = parent;
            poolable.isUsing = true;

            return poolable;
        }
    }

    Dictionary<string, Pool> _poolDictionary = new Dictionary<string, Pool>();

    Transform root;
    GameObject archer;
    public void Init()
    {


        if (root == null)
        {
            root = new GameObject
            {
                name = "@Pool_Root"
            }.transform; //root 변수는 Transform 타입이므로, .transform 하여 transform을 빼온다.
        }
        Object.DontDestroyOnLoad(root);
    }

    public void CreatePool(GameObject original, int count = 7)
    {
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = root;

        _poolDictionary.Add(original.name, pool);
    }

    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;

        if(_poolDictionary.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }
        _poolDictionary[name].Push(poolable);
    }

    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if(_poolDictionary.ContainsKey(original.name )== false)
        {
            CreatePool(original);
        }
        return _poolDictionary[original.name].Pop(parent);
    }


    public GameObject GetOriginal(string name) //게임오브젝트를 받는 메서드
    {
        if(_poolDictionary.ContainsKey(name) == false)
        {
            return null;
        }
        return _poolDictionary[name].Original;
    }

    public void Clear()
    {
        foreach(Transform child in root)
        {
            GameObject.Destroy(child.gameObject);
        }

        _poolDictionary.Clear();
    }
}
