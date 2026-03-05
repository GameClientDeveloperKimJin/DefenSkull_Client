using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Extension 
{
    public static void ResetListener(Button button, UnityAction onClickAction)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(onClickAction);
    }
}
