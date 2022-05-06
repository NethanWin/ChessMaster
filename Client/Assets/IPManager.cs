using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IPManager : MonoBehaviour
{
    private string ip = "";
    public TextMeshProUGUI text;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void SetIP()
    {
        ip = text.text;
        Debug.Log(ip);
        SceneManager.LoadScene("Game");
    }
    public string GetIP() => ip;
}
