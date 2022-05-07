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
    private void Update()
    {
        if (Input.GetKeyDown("enter"))
            SetIP();
    }
    public void SetIP()
    {
        //TODO:
        //add condition if the ip cant connect
        ip = text.text;
        Debug.Log(ip);
        SceneManager.LoadScene("Game");
    }
    public string GetIP() => ip;
}
