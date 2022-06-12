using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System;
using System.Text;

public class SignUpManager : MonoBehaviour
{
    public TextMeshProUGUI username;
    public TextMeshProUGUI password;
    public Text usernameTakenText;
    private void Start()
    {
        usernameTakenText.text = "";
        usernameTakenText.gameObject.SetActive(true);
    }
    public void SignUp()
    {
        GameObject network = GameObject.FindGameObjectWithTag("NetworkManager");
        string usernameStr = username.text;
        string passwordStr = password.text;
        usernameStr = usernameStr.Substring(0, usernameStr.Length - 1);
        passwordStr = Hash(passwordStr.Substring(0, passwordStr.Length - 1));

        bool response = network.GetComponent<Client>().SignUp(usernameStr, passwordStr);
        if (response)
        {
            usernameTakenText.text = "User successfuly created :)";
            usernameTakenText.color = Color.green;
        }
        else
        {
            usernameTakenText.text = "Username already taken";
            usernameTakenText.color = Color.red;
        }
    }
    public void LoadLoginScene()
    {
        SceneManager.LoadScene("Login");
    }
    void Update()
    {
        if (Input.GetKeyDown("enter"))
            SignUp();
    }
    private string Hash(string password)
    {
        var sha256 = SHA256.Create();
        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        string hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        return hash;
    }
}
