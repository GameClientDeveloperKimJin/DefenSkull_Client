using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 워커 스레드에서 메인 스레드로 작업 전달. 워커 스레드는 UnityAPI 이용 할 수 없으므로 워커 스레드가 메인스레드로 작업을 전달하게
/// </summary>
public class MainThreadDispatcher : MonoBehaviour
{
    private readonly Queue<Action> actionQueue = new Queue<Action>();

    private readonly static object _lock = new object();

    private static MainThreadDispatcher instance;
    public static MainThreadDispatcher Instance
    {
        get
        {
            lock(_lock)
            {
                return instance;
            }
            
                
        
          
        }
    }
    private void Awake()
    {
        lock (_lock)
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[MainThreadDispatcher] 초기화 완료");
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
    /// <summary>
    /// 메인 스레드에서 실행할 작업 추가
    /// </summary>
    public void Enqueue(Action action)
    {
        lock (_lock)
        {
            actionQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        lock (_lock)
        {
            while (actionQueue.Count > 0)
            {
                var action = actionQueue.Dequeue();
                action?.Invoke();  // 메인 스레드에서 실행
            }
        }
    }
}
