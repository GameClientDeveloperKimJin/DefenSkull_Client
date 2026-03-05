using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITitleSceneView 
{
    public void OnStartButton(); //최초 시작 버튼

    public void OnRegisterButton(); //회원가입 버튼 -> 회원가입 이미지 활성화

    public void OnLoginButton(); //로그인 버튼

    public void OnRegisterFinishButton(); //회원가입 완료 버튼 
    public void OnDeleteButton(); //계정 삭제 버튼 -> 계정 삭제 이미지 활성화

    public void OnDeleteButtonFinishButton(); //계정 삭제 완료 버튼

    public void OnUpdatePasswordButton(); //계정 비밀번호 변경 버튼 -> 계정 변경 이미지 활성화

    public void OnUpdatePasswordFinishButton(); //계정 변경 완료 버튼
}
