using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class taitorumodoru : MonoBehaviour
{
    //ランキングに戻るためのスクリプト（元タイトル戻るボタン）
    public void OnClick()
    {
        SceneManager.LoadScene("title");
    }
}
    
