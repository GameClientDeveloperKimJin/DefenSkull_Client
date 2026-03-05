using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEX : MonoBehaviour
{
    public BaseScene CurrentScene
    {
        get
        {
            return GameObject.FindObjectOfType(typeof(BaseScene)) as BaseScene;
        }
    }
    public void LoadScene(Define.SceneType type)
    {
        CurrentScene.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    private string GetSceneName(Define.SceneType type)
    {
        string Scenename = System.Enum.GetName(typeof(Define.SceneType), type);
        Debug.Log(Scenename);
        return Scenename;
    }
}
