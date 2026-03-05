using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;


public class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance;

    private static readonly object _lock = new object();
    public static NetworkManager Instance
    {
        get
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<NetworkManager>();

                    if (instance == null)
                    {
                        GameObject obj = new GameObject("NetworkManager");
                        instance = obj.AddComponent<NetworkManager>();
                    }
                }

                return instance;
            }

           
        }
    }

    //네트워크 관련 객체
    private ClientSimpleTcp _network;

    private PacketBufferManager _packetBuffer;

    // 연결 정보
    private bool _isConnected = false;
    private string _serverIP = "127.0.0.1";
    private int _serverPort = 32452;

    //스레드 관련 
    [SerializeField]
    private int workerCount;
    private List<Thread> workerThreads;


    /// <summary>
    /// 싱글톤 초기화
    /// </summary>
    private void Awake()
    {
        lock (_lock)
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                NetworkInit();
            }
            else if (instance != this)
            {
                Destroy(gameObject); // 중복 생성 방지
            }
        }
    }

    /// <summary>
    /// 네트워크 초기화
    /// </summary>
    public void NetworkInit()
    {
        Debug.Log("[NetworkManager] 초기화 시작");

        // ClientSimpleTcp 생성
        _network = new ClientSimpleTcp();

        // PacketBufferManager 생성
        _packetBuffer = new PacketBufferManager();

        workerThreads = new List<Thread>();

        // 버퍼 초기화 (버퍼크기: 8192, 헤더크기: 5, 최대패킷: 2048)
        _packetBuffer.Init(8192, PacketDef.PACKET_HEADER_SIZE, 2048);
  

        if (Connect(_serverIP, _serverPort))
        {
            ThreadInit();
        }
        else
        {
            Debug.Log("[NetworkManager] 초기화 실패");
        }
       
    }

    /// <summary>
    /// 스레드 초기화 및 작업 할당
    /// </summary>
    private void ThreadInit()
    {
        for (int i = 0; i < workerCount; i++)
        {
            int workerID = i; // 클로징 문제 해결 -> 람다식으로 함수를 등록할 때 안 꼬이게 

            Thread worker = new Thread(() => WorkerProcess(workerID));//1. 스레드 작업 할당
            worker.Name = $"PacketWorker {workerID}";
            worker.Start(); //2. 스레드 작업 시작(isRunning이 true일 때)
            workerThreads.Add(worker); //워커를 리스트에 등록 
        }
    }

    /// <summary>
    /// 서버 연결
    /// </summary>
    public bool Connect(string ip, int port)
    {
        bool result = _network.Connect(ip, port);

        if(result)
        {
            _isConnected = true;
        }

        return result;
        
    }
    /// <summary>
    /// 일반 패킷 전송
    /// </summary>
    public void SendPacket(PACKETID packetID, byte[] bodyData = null)
    {
        if (!IsConnected())
        {
            Debug.LogError("서버에 연결되어 있지 않습니다!");
            return;
        }
        try
        {
            var packet = PacketToBytes.Make(packetID, bodyData);
            _network.Send(packet);

            Debug.Log($"클라이언트 -  패킷 전송: PacketID={packetID}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"클라이언트 -  패킷 전송 오류: {ex.Message}");
        }
    }

    /// <summary>
    /// 패킷 수신 루프
    /// </summary>
    private void WorkerProcess(int workerID)
    {
        while (_isConnected && _network.IsConnected())
        {
            // 데이터 수신
            var recvData = _network.Receive();

            if (recvData != null)
            {
                int recvLength = recvData.Item1;
                byte[] recvBytes = recvData.Item2;

                // 버퍼에 쓰기
                _packetBuffer.Write(recvBytes, 0, recvLength);

                // 완성된 패킷 읽기
                while (true)
                {
                    var packet = _packetBuffer.Read();
                    if (packet.Count < 1)
                    {
                        break;
                    }

                    ProcessReceivedPacket(packet.Array, packet.Offset, packet.Count);
                }
            }
            Thread.Sleep(10); // 10ms 대기
        }

    }
    /// <summary>
    /// 수신한 패킷 처리 -> 바이트 변환
    /// </summary>
    private void ProcessReceivedPacket(byte[] buffer, int offset, int count)
    {
        try
        {
            // 헤더 파싱
            var totalSize = BitConverter.ToUInt16(buffer, offset);
            var packetID = (PACKETID)BitConverter.ToUInt16(buffer, offset + 2);
            var type = buffer[offset + 4];

            // 바디 파싱
            var bodySize = totalSize - PacketDef.PACKET_HEADER_SIZE;
            byte[] bodyData = null;

            if (bodySize > 0)
            {
                bodyData = new byte[bodySize];
                Buffer.BlockCopy(buffer, offset + PacketDef.PACKET_HEADER_SIZE, bodyData, 0, bodySize);
            }

            // 패킷 처리
            ProcessPacket(packetID, bodyData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"패킷 처리 오류: {ex.Message}");
        }
    }

    public Action<NotifyChat> chatBroadCast;
    public Action<ErrorCode> errorCode;
    public Action<List<RedisChatMessage>> chatHistory;

    public Action<List<RankingData>> rankTopUsers;

    [SerializeField]
    private string UserID;

    /// <summary>
    /// 패킷 ID별 처리
    /// </summary>
    private void ProcessPacket(PACKETID packetID, byte[] bodyData = null)
    {
        switch (packetID)
        {
            case PACKETID.ECHO_Response:
                if (bodyData != null && bodyData.Length > 0)
                {
                    //string text = System.Text.Encoding.UTF8.GetString(bodyData); //byte[] -> string 변환
                    //Debug.Log($" {text} ");
                    Debug.Log($" 서버 접속 완료 ");
                }
                break;
            case PACKETID.Register_Response:
                if(bodyData.Length > 0)
                {
                    //서버 응답 패킷(byte[])을 객체로 역직렬화
                    var resData = MessagePackSerializer.Deserialize<ResRegister>(bodyData);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        errorCode.Invoke(resData.Result); //TitleSceneView 클래스에서 이벤트 등록 중
                    });
                    
                }
                break;
            case PACKETID.Login_Response:
                if (bodyData.Length > 0)
                {
                    //서버 응답 패킷(byte[])을 객체로 역직렬화
                    var resData = MessagePackSerializer.Deserialize<ResLogin>(bodyData);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        UserID = resData.UserID;

                        errorCode.Invoke(resData.result); //TitleSceneView 클래스에서 이벤트 등록 중
                    });

                }
                break;
            case PACKETID.Delete_Response:
                if (bodyData.Length > 0)
                {
                    //서버 응답 패킷(byte[])을 객체로 역직렬화
                    var resData = MessagePackSerializer.Deserialize<ResDelete>(bodyData);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        Debug.Log($" 계정 삭제 완 - 유저가 입력한 삭제 ID ={resData.UserID}");
                        errorCode.Invoke(resData.result); //TitleSceneView 클래스에서 이벤트 등록 중
                    });

                }
                break;
            case PACKETID.UpdatePassword_Response:
                if (bodyData.Length > 0)
                {
                    //서버 응답 패킷(byte[])을 객체로 역직렬화
                    var resData = MessagePackSerializer.Deserialize<ResUpdatePassword>(bodyData);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        errorCode.Invoke(resData.result); //TitleSceneView 클래스에서 이벤트 등록 중
                    });
                }
                break;
            case PACKETID.CreateRoom_Response:
                if (bodyData.Length > 0)
                {
                    //서버 응답 패킷(byte[])을 객체로 역직렬화
                    var resData = MessagePackSerializer.Deserialize<ResCreateRoom>(bodyData);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        Debug.Log($" 방 생성 완 -  유저가 입력한 방 이름 ={resData.RoomName}");
                        errorCode.Invoke(resData.result); //MainSceneView 클래스에서 이벤트 등록 중
                    });
                }
                break;
            case PACKETID.JoinRoom_Response:
                if (bodyData.Length > 0)
                {
                    //서버 응답 패킷(byte[])을 객체로 역직렬화
                    var resData = MessagePackSerializer.Deserialize<ResJoinRoom>(bodyData);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        Debug.Log($" 방 참가 완 -  유저가 입력한 방 이름 ={resData.RoomName}");
                        Debug.Log($" 방 참가 완 - 방에 참가한 유저 이름 ={resData.UserID}");

                        errorCode.Invoke(resData.result); //MainSceneView 클래스에서 이벤트 등록 중
                    });
                }
                break;
            case PACKETID.Chat_Response:
                if (bodyData.Length > 0)
                {
                    //서버 응답 패킷(byte[])을 객체로 역직렬화
                    var resData = MessagePackSerializer.Deserialize<ResChat>(bodyData);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        Debug.Log($" 채팅 보내기 완 - 방 이름 ={resData.RoomName}");
                        Debug.Log($" 채팅 보내기 완 - 방에 참가한 유저 이름 ={resData.UserID}");
                        Debug.Log($" 채팅 보내기 완 - 채팅 내용 ={resData.Message}");

                        errorCode.Invoke(resData.result); //MainSceneView 클래스에서 이벤트 등록 중
                    });
                }
                break;
            case PACKETID.Chat_Notify:
                if (bodyData.Length > 0)
                {
                    //서버 응답 패킷(byte[])을 객체로 역직렬화
                    var resData = MessagePackSerializer.Deserialize<NotifyChat>(bodyData);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        chatBroadCast.Invoke(resData); //MainSceneView 클래스에서 이벤트 등록 중
                    });
                }
                break;
            case PACKETID.ChatHistory_Response:
                if (bodyData.Length > 0)
                {
                    //서버 응답 패킷(byte[])을 객체로 역직렬화
                    var resData = MessagePackSerializer.Deserialize<ResChatHistory>(bodyData);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        chatHistory.Invoke(resData.Messages); //MainSceneView 클래스에서 이벤트 등록 중
                    });
                }
                break;
            case PACKETID.LeaveRoom_Response:
                if (bodyData.Length > 0)
                {
                    //서버 응답 패킷(byte[])을 객체로 역직렬화
                    var resData = MessagePackSerializer.Deserialize<ResLeaveRoom>(bodyData);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        errorCode.Invoke(resData.result); //MainSceneView 클래스에서 이벤트 등록 중
                    });
                }
                break;
            case PACKETID.UpdateScore_Response:
                if (bodyData.Length > 0)
                {
                    //서버 응답 패킷(byte[])을 객체로 역직렬화
                    var resData = MessagePackSerializer.Deserialize<ResUpdateScore>(bodyData);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        Debug.Log("점수 조회" + resData.Score);
                        errorCode.Invoke(resData.Result); //MainSceneView 클래스에서 이벤트 등록 중
                    });
                }
                break;
            case PACKETID.GetTopRankers_Response:
                if (bodyData.Length > 0)
                {
                    //서버 응답 패킷(byte[])을 객체로 역직렬화
                    var resData = MessagePackSerializer.Deserialize<ResGetTopRankers>(bodyData);

                    MainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        if(resData.Rankings != null)
                        {
                            rankTopUsers.Invoke(resData.Rankings); //MainSceneView 클래스에서 이벤트 등록 중
                        }
                      
                    });
                }
                break;
            default:
                Debug.LogWarning($"[NetworkManager] 처리되지 않은 PacketID: {packetID}");
                break;
        }
    }


    private void OnDestroy()
    {
        Disconnect();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }
    /// <summary>
    /// 연결 상태 확인 - 조건 : 서버 접속 & TCP 있음 & TCP 소켓 연결 완료 
    /// </summary>
    public bool IsConnected()
    {
        return _isConnected && _network != null && _network.IsConnected();
    }
    /// <summary>
    /// 서버 연결 해제
    /// </summary>
    public void Disconnect()
    {
        if (!_isConnected)
        {
            return;
        }

        Debug.Log("[NetworkManager] 서버 연결 해제");

        // 소켓 닫기
        _network.Close();
        _isConnected = false;

        Debug.Log("[NetworkManager] 연결 해제 완료");
    }

    public string GetUserID()
    {
        return UserID;
    }
}
