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

    }
    public void AttemptToConnect()
    {
        string ip = text.text;
        GameObject network = GameObject.FindGameObjectWithTag("NetworkManager");
        bool temp = network.GetComponent<Client>().StartClient(ip);
        Debug.Log(temp);
        if (temp)
            SceneManager.LoadScene("Login");
        else
        {
            //if couldnt connect to client

        }

    }
    private void Update()
    {
        if (Input.GetKeyDown("enter"))
            AttemptToConnect();
    }
    public void SetIP()
    {
        //TODO:
        //add condition if the ip cant connect
        ip = text.text;
        SceneManager.LoadScene("Game");
        
    }
    public string GetIP() => ip;
}
