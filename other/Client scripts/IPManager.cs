using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IPManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Text wrongIP;
    private void Start()
    {
        wrongIP.gameObject.SetActive(false);
    }
    public void AttemptToConnect()
    {
        string ip = text.text;
        GameObject network = GameObject.FindGameObjectWithTag("NetworkManager");
        bool isValidServerIP = network.GetComponent<Client>().StartClient(ip);
        if (isValidServerIP)
            SceneManager.LoadScene("Login");
        else
        {
            //if couldnt connect to client
            wrongIP.gameObject.SetActive(true);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown("enter"))
           AttemptToConnect();
    }
}
