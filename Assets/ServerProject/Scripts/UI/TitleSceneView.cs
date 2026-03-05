using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// UI 입력 및 UI 표시를 담당하고 로직이 없음.
/// 실제 입력 처리는 Presenter에게 전달
/// </summary>
public class TitleSceneView : MonoBehaviour, ITitleSceneView
{
    private TitleScenePresenter presenter;
    
    [SerializeField]
    Button StartButton, LoginButton, 
        RegisterButton, RegisterFinishButton,
        DeleteButton,DeleteFinisiButton,
        UpdateButton,UpdateFinishButton;

    [Header("가입 정보 관련")]
    [SerializeField]
    GameObject InfoImage;
    [SerializeField]
    TMP_Text InfoTmp;

    [Header("회원가입 관련")]
    [SerializeField]
    TMP_InputField RegisterUserID;
    [SerializeField]
    TMP_InputField RregisterUserPassword;
    [SerializeField]
    GameObject RegisterImage;

    [Header("로그인 관련")]
    [SerializeField]
    GameObject LoginImage;
    [SerializeField]
    TMP_InputField LoginUserID;
    [SerializeField]
    TMP_InputField LoginUserPassword;
    [SerializeField]
    bool isLoginSucess;

    [Header("계정삭제 관련")]
    [SerializeField]
    GameObject DeleteImage;
    [SerializeField]
    TMP_InputField DeleteUserID;

    [Header("비밀번호 변경 관련")]
    [SerializeField]
    GameObject UpdateImage;
    [SerializeField]
    TMP_InputField CurrentUserID;
    [SerializeField]
    TMP_InputField CurrentPassword;
    [SerializeField]
    TMP_InputField UpdatePassword;
    private void Start()
    {
        presenter = new TitleScenePresenter(this);

        NetworkManager.Instance.errorCode += SetErrorCode;

        LoginImage.gameObject.SetActive(false);
        RegisterImage.gameObject.SetActive(false);
        DeleteImage.gameObject.SetActive(false);
        UpdateImage.gameObject.SetActive(false);

        Extension.ResetListener(StartButton, OnStartButton);
        Extension.ResetListener(LoginButton, OnLoginButton);
        Extension.ResetListener(RegisterButton, OnRegisterButton);
        Extension.ResetListener(RegisterFinishButton, OnRegisterFinishButton);
        Extension.ResetListener(DeleteButton, OnDeleteButton);
        Extension.ResetListener(DeleteFinisiButton, OnDeleteButtonFinishButton);
        Extension.ResetListener(UpdateButton, OnUpdatePasswordButton);
        Extension.ResetListener(UpdateFinishButton, OnUpdatePasswordFinishButton);
    }

    private static object _lock = new object();
    private void SetErrorCode(ErrorCode code)
    {
        lock(_lock)
        {
            Debug.Log("로그인/회원가입/비밀번호 변경 : 상태 = " + code);

            InfoImage.gameObject.SetActive(true);

            switch (code)
            {
                case ErrorCode.Register_Success:
                    InfoTmp.text = "회원가입 성공";
                    break;
                case ErrorCode.Login_Success:
                    isLoginSucess = true;
                    InfoTmp.text = "로그인 가능";

                    Button btn = InfoImage.GetComponentInChildren<Button>();
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() =>
                    {
                        SceneManager.LoadScene(1);
                    });
                    break;
                case ErrorCode.UpdatePassword_Success:
                    InfoTmp.text = "비밀번호 변경 완료!";
                    break;
                case ErrorCode.Delete_Sucess:
                    InfoTmp.text = "계정삭제 성공";
                    break;
                case ErrorCode.Register_DuplicateUserId:
                    InfoTmp.text = "ID 중복";
                    break;
                case ErrorCode.Login_UserNotFound:
                    InfoTmp.text = "존재하지 않는 ID";
                    break;
                case ErrorCode.Login_InvalidPassword:
                    InfoTmp.text = "아이디 또는 비밀번호가 맞지 않음";
                    break;
                case ErrorCode.Delete_UserNotFound:
                    InfoTmp.text = "존재하지 않는 ID";
                    break;
                case ErrorCode.UpdatePassword_UserNotFound:
                    InfoTmp.text = "존재하지 않는 ID ";
                    break;
                case ErrorCode.UpdatePassword_WrongCurrentPassword:
                    InfoTmp.text = "현재 비밀번호 불일치";
                    break;
                case ErrorCode.UpdatePassword_InvalidNewPassword:
                    InfoTmp.text = "비밀번호를 입력하세요";
                    break;
                case ErrorCode.UpdatePassword_SamePassword:
                    InfoTmp.text = "현재 비밀번호와 변경하려는 비밀번호가 같습니다";
                    break;
                case ErrorCode.DB_Error:
                    InfoTmp.text = "서버에 문제가 발생";
                    break;
            }
        }
  
    }

    public void OnStartButton()
    {
        StartButton.gameObject.SetActive(false);
        LoginImage.gameObject.SetActive(true);

        presenter.StartButton();
    }

    public void OnLoginButton()
    {
        //if(isLoginSucess)//로그인에 성공했다면, 로그인 확인 필요 없으므로 return;
        //{
        //    SceneManager.LoadScene(1);
        //    return;
        //}

        string userID = LoginUserID.text;
        string userPassword = LoginUserPassword.text;

        if (!string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(userPassword))
        {
            presenter.LoginButton(userID, userPassword);
            //회원가입 내용 초기화
            RegisterUserID.text = "";
            RregisterUserPassword.text = "";
        }
    }

    public void OnRegisterFinishButton()
    {
        string userID = RegisterUserID.text;
        string userPassword = RregisterUserPassword.text;

        if(userID.Length > 9)
        {
            InfoImage.gameObject.SetActive(true);
            InfoTmp.text = "ID 제한 길이는 10글자 입니다";
            return;
        }

        if (!string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(userPassword))
        {
            presenter.RegisterFinishButton(userID, userPassword);
            //로그인 내용 초기화
            LoginUserID.text = "";
            LoginUserPassword.text = "";
        }

    }

    public void OnDeleteButtonFinishButton()
    {
        string userID = DeleteUserID.text;

        if (!string.IsNullOrEmpty(userID))
        {
            presenter.DeleteButton(userID);
            //로그인 및 회원가입 내용 초기화
            LoginUserID.text = "";
            LoginUserPassword.text = "";
            RegisterUserID.text = "";
            RregisterUserPassword.text = "";
        }
    }

    public void OnUpdatePasswordFinishButton()
    {
        string currentUserID = CurrentUserID.text;
        string currentPassword = CurrentPassword.text;
        string updatePassword = UpdatePassword.text;

        presenter.UpdateButton(currentUserID, currentPassword, updatePassword);

        LoginUserID.text = "";
        LoginUserPassword.text = "";
        RegisterUserID.text = "";
        RregisterUserPassword.text = "";
        CurrentUserID.text = "";
        CurrentPassword.text = "";
        UpdatePassword.text = "";
    }
    public void OnUpdatePasswordButton() => UpdateImage.gameObject.SetActive(true);


    public void OnRegisterButton() => RegisterImage.gameObject.SetActive(true);
    
     

    public void OnDeleteButton() => DeleteImage.gameObject.SetActive(true);
  
    private void OnDestroy()
    {
        NetworkManager.Instance.errorCode -= SetErrorCode;
    }

    private void OnApplicationQuit()
    {
        NetworkManager.Instance.errorCode -= SetErrorCode;
    }

}
