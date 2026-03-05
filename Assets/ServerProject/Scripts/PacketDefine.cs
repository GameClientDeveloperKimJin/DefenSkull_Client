using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ErrorCode : short
{
    //성공 관련
    None = 0, //문제 없음
    Register_Success = 1, //회원가입 성공
    Login_Success = 2, //로그인 성공
    Delete_Sucess = 3, //계정삭제 성공
    RoomCreate_Sucess = 4, //방 생성 성공
    RoomJoin_Sucess = 5, //방 참가 성공
    Chat_Sucess = 6, //채팅 보내기 성공
    UpdatePassword_Success = 7, //비밀번호 업데이트
    LeaveRoom_Success = 8, //방 나가기 성공
    UpdateScore_Success = 9, //점수 업데이트 성공
    GetRankAndScore_Success = 10,
    LeaveRoomDelete_Success = 11,
    Chat_Delete = 12,

    //비밀번호 변경 관련
    UpdatePassword_WrongCurrentPassword = 997, //현재 비밀번호 불일치 - 보안성
    UpdatePassword_UserNotFound = 998, //비밀번호 변경 중 존재하지 않는 ID
    UpdatePassword_InvalidNewPassword = 999, //비밀번호 공백
    UpdatePassword_SamePassword = 1000, //비밀번호 공백
    //회원가입 관련
    Register_DuplicateUserId = 1001,  //ID 중복

    //로그인 관련
    Login_UserNotFound = 1101,  //로그인 실패 - 존재하지 않는 ID
    Login_InvalidPassword = 1102,  //비밀번호 틀림

    //삭제 관련
    Delete_UserNotFound = 1103, //계정 삭제 실패 - 존재하지 않는 ID

    //채팅방 관련
    Room_InvalidName = 1104, //방 이름 부정확 및 특수문자
    Room_NameTooLong = 1105, //방 이름 길이 초과 방지
    Room_DuplicateName = 1106, //방 생성 시 , 방 이름 중복 체크 방지 
    Room_NotFound = 1107, //방 못 찾았을 때
    Room_Full = 1108, //방 인원 꽉찼을 때
    Room_NotMember = 1109, //퇴장 메서드 - 현재 방 멤버가 아닐 경우

    DB_Error = 9001 //DB  오류
}

// 1 ~ 10000
public enum PACKETID : int
{
    CS_BEGIN = 0,

    //이 사이에 있는 패킷 id만 허용

    ECHO_Request = 1, //에코 접속 요청
    ECHO_Response = 2, //에코 접속 응답

    UpdatePassword_Request = 999,
    UpdatePassword_Response = 1000,

    Register_Request = 1001,
    Register_Response = 1002,

    Login_Request = 1003,
    Login_Response = 1004,

    Delete_Request = 1005,
    Delete_Response = 1006,

    CreateRoom_Request = 1007,
    CreateRoom_Response = 1008,

    JoinRoom_Request = 1009,
    JoinRoom_Response = 1010,

    LeaveRoom_Request = 1011,
    LeaveRoom_Response = 1012,

    Chat_Request = 3001,
    Chat_Response = 3002,
    Chat_Notify = 3021,   //브로드캐스트용도
    ChatHistory_Response = 3022,


    UpdateScore_Request = 5001,
    UpdateScore_Response = 5002,

    GetTopRankers_Request = 5021,
    GetTopRankers_Response = 5022,


    CS_END = 9999,
}
public enum MessageType : byte
{
    Text = 0, //일반 텍스트
    System = 2, //시스템 메시지
    Delete = 4,
}

[MessagePackObject]
public class ReqConnectEcho
{
    [Key(0)]
    public string Messages;
}
[MessagePackObject]
public class ResConnectEcho
{
    [Key(0)]
    public string Messages;
    [Key(1)]
    public DateTime ServerTime;
}
//============================================================= 계정 관련  ====================================================================

[MessagePackObject]
public class ReqRegister
{
    [Key(0)]
    public string UserID { get; set; }

    [Key(1)]
    public string Password { get; set; }
}

[MessagePackObject]
public class ResRegister
{
    [Key(0)]
    public string UserID { get; set; }

    [Key(1)]
    public string Password { get; set; }

    [Key(2)]
    public ErrorCode Result { get; set; }

}

[MessagePackObject]
public class ReqLogin
{
    [Key(0)]
    public string UserID { get; set; }

    [Key(1)]
    public string Password { get; set; }
}

[MessagePackObject]
public class ResLogin
{
    [Key(0)]
    public string UserID { get; set; }

    [Key(1)]
    public ErrorCode result { get; set; }
}
[MessagePackObject]
public class ReqDelete
{
    [Key(0)]
    public string UserID { get; set; }
}

[MessagePackObject]
public class ResDelete
{
    [Key(0)]
    public string UserID { get; set; }

    [Key(1)]
    public ErrorCode result { get; set; }
}
[MessagePackObject]
public class ReqUpdatePassword
{
    [Key(0)]
    public string UserID { get; set; }

    [Key(1)]
    public string CurrentPassword { get; set; }

    [Key(2)]
    public string UpdatePassword { get; set; }
}
[MessagePackObject]
public class ResUpdatePassword
{
    [Key(1)]
    public ErrorCode result { get; set; }
}

//============================================================= 채팅방 및 채팅 관련  ====================================================================

[MessagePackObject]
public class ReqCreateRoom
{
    [Key(0)]
    public string RoomName { get; set; }

    [Key(1)]
    public int MaxMembers { get; set; }
}

[MessagePackObject]
public class ResCreateRoom
{
    [Key(0)]
    public string RoomName { get; set; }

    [Key(1)]
    public int MaxMembers { get; set; }

    [Key(2)]
    public ErrorCode result { get; set; }
}

[MessagePackObject]
public class ReqJoinRoom
{
    [Key(0)]
    public string RoomName { get; set; }

    [Key(1)]
    public string UserID { get; set; }
}
[MessagePackObject]
public class ResJoinRoom
{
    [Key(0)]
    public string RoomName { get; set; }

    [Key(1)]
    public string UserID { get; set; }

    [Key(2)]
    public ErrorCode result { get; set; }
}


[MessagePackObject]
public class ReqChat
{
    [Key(0)]
    public string RoomName { get; set; }

    [Key(1)]
    public string UserID { get; set; }

    [Key(2)]
    public string Message { get; set; }
}
[MessagePackObject]
public class ResChat
{
    [Key(0)]
    public string RoomName { get; set; }

    [Key(1)]
    public string UserID { get; set; }

    [Key(2)]
    public string Message { get; set; }

    [Key(3)]
    public ErrorCode result { get; set; }
}

[MessagePackObject]
public class NotifyChat //응답클래스만
{
    [Key(0)]
    public string RoomName { get; set; }

    [Key(1)]
    public string UserId { get; set; }

    [Key(2)]
    public string Message { get; set; }


    [Key(3)]
    public ErrorCode result { get; set; }
}

[MessagePackObject]
public class ResChatHistory
{
    [Key(0)]
    public string RoomName { get; set; }

    [Key(1)]
    public List<RedisChatMessage> Messages { get; set; }

    [Key(2)]
    public ErrorCode result { get; set; }

}
/// <summary>
/// 채팅 메시지
/// </summary>

[MessagePackObject]
public class RedisChatMessage //브로드 캐스팅을 위한 클래스. 요청 클래스는 없고 서버가 처리하여 응답 클래스를 bodyData로 만들어서 브로드캐스트 메서드 호출
{
    [Key(0)]
    public string UserId { get; set; }
    [Key(1)]
    public string Message { get; set; }
    [Key(2)]
    public MessageType MessageType { get; set; }
}

[MessagePackObject]
public class ReqLeaveRoom
{
    [Key(0)]
    public string UserID { get; set; }

    [Key(1)]
    public string RoomName { get; set; }
}

[MessagePackObject]
public class ResLeaveRoom
{
    [Key(0)]
    public ErrorCode result { get; set; }
}

//============================================================= 랭킹 ====================================================================
// 랭킹 데이터
[MessagePackObject]
public class RankingData
{
    //클라이언트 프로젝트 - RankUI에서 딱 이 3개의 데이터만 필요
    [Key(0)]
    public int Rank { get; set; }

    [Key(1)]
    public string UserId { get; set; }

    [Key(2)]
    public int Score { get; set; }
}

[MessagePackObject]
public class ReqUpdateScore
{
    [Key(0)]
    public string UserID { get; set; }

    [Key(1)]
    public int Score { get; set; }
}

[MessagePackObject]
public class ResUpdateScore
{
    [Key(0)]
    public ErrorCode Result { get; set; }

    [Key(1)]
    public string UserID { get; set; }

    [Key(2)]
    public int Score { get; set; }
}


[MessagePackObject]
public class ReqGetTopRankers
{
    [Key(0)]
    public int Count { get; set; }  // 몇등까지
}

[MessagePackObject]
public class ResGetTopRankers
{
    [Key(0)]
    public ErrorCode Result { get; set; }

    [Key(1)]
    public List<RankingData> Rankings { get; set; }
}

