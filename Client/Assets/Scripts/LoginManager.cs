using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System;
using System.Text;


public class LoginManager : MonoBehaviour
{
    public TextMeshProUGUI username;
    public TextMeshProUGUI password;
    public Text wrongUserText;
    void Start()
    {
        wrongUserText.gameObject.SetActive(false);
    }
    public void AttemptLogin()
    {
        GameObject network = GameObject.FindGameObjectWithTag("NetworkManager");
        string usernameStr = username.text;
        string passwordStr = password.text;
        usernameStr = usernameStr.Substring(0, usernameStr.Length - 1);
        passwordStr = Hash(passwordStr.Substring(0, passwordStr.Length - 1));

        bool response = network.GetComponent<Client>().Login(usernameStr, passwordStr);
        if (response)
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            wrongUserText.gameObject.SetActive(true);
        }
    }
    public void LoadSignUpScene()
    {
        SceneManager.LoadScene("SignUp");
    }
    void Update()
    {
        if (Input.GetKeyDown("enter"))
            AttemptLogin();
    }

    private string Hash(string password)
    {
        var sha256 = SHA256.Create();
        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        string hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        return hash;
    }
}
