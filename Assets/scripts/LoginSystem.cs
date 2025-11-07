using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using System.Xml.Schema;
using UnityEngine.UI;
using System.Threading;
using System.Security.Cryptography;
using System;
using UnityEngine.UIElements;
using System.Text;
using System.Linq;
using System.Xml;

public class LoginSystem : MonoBehaviour
{
    // Variables
    private string signType;
    public float waitTime = 3f;
    private string salt = null;
    public int saltLength = 8;
    private string hashedPassword;
    // GameObjects
    public GameObject registerButton;
    public GameObject registerText;
    public GameObject loginButton;
    public GameObject loginText;

    public GameObject usernameDesign;
    public GameObject passwordDesign;
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public GameObject backButton;
    public GameObject frontButton;

    public GameObject showPassword;

    public Text message;
    // GameObject accessors

    // Other files
    public BackgroundHandler backgroundHandler;
    public Database database;
    public MenuSystem menuSystem;
    public GameSystem gameSystem; // temp

    void Start()
    {
        passwordInput.contentType = TMP_InputField.ContentType.Password;
    }

    public void Register()
    {
        // Show page to register
        signType = "register";
        DisplayMainScreen(false);
        DisplayLoginPage(true);
        registerText.SetActive(true);
        message.gameObject.SetActive(false);
    }

    public void Login()
    {
        // Show page to login
        signType = "login";
        DisplayMainScreen(false);
        DisplayLoginPage(true);
        loginText.SetActive(true);
        message.gameObject.SetActive(false);
    }

    public void Back()
    {
        // Return to main page
        DisplayMainScreen(true);
        DisplayLoginPage(false);
        registerText.SetActive(false);
        loginText.SetActive(false);
        message.gameObject.SetActive(false);
        usernameInput.text = null;
        passwordInput.text = null;
    }

    public void DisplayMainScreen(bool booli)
    {
        registerButton.SetActive(booli);
        loginButton.SetActive(booli);
    }

    void DisplayLoginPage(bool booli)
    {
        usernameDesign.SetActive(booli);
        passwordDesign.SetActive(booli);
        usernameInput.gameObject.SetActive(booli);
        passwordInput.gameObject.SetActive(booli);
        backButton.SetActive(booli);
        frontButton.SetActive(booli);
        showPassword.SetActive(booli);
    }

    public int CreateID(string username, string password) // CREATE MERGE SORT SOMEWHERE
    {
        // Creates unique ID based on username and password,
        // ensuring it is unique using collision detection
        int total = 0;
        string textMess = username + password;

        foreach (char i in textMess)
        {
            int num = (int)i * (int)i;
            total += num;
        }

        int newID = total % 10000;
        while (database.CheckForExistingData(newID.ToString(), "ID"))
        {
            newID++;
            if (newID >= 9999) newID = 0;
        }
        return newID;
    }

    public (string hashedPassword, string salt) Hashing(string password, string tempSalt)
    {
        // Creates an unbreakable password through hashing
        tempSalt ??= Salt();

        string input = password + tempSalt;
        const int hashLength = 64; // Desired hash length
        const int primeMultiplier = 31; // prime number for hashing

        int hashValue = 0;
        foreach (char c in input)
        {
            hashValue = (hashValue * primeMultiplier + c) % int.MaxValue;
        }

        // Convert the hash value into a hexadecimal string
        StringBuilder builder = new(hashLength);
        for (int i = 0; i < hashLength; i++)
        {
            // Cycle through the hashValue repeatedly to fill the desired hash length
            int charValue = (hashValue + i * primeMultiplier) % 256; // Map to a byte value
            builder.Append(charValue.ToString("x2"));
        }

        return (builder.ToString(), tempSalt);
    }

    public string Salt()
    {
        // Creates 8 random ASCII characters
        // for a salt
        StringBuilder salt = new();
        System.Random randomGen = new System.Random();
        for (int i = 0; i < saltLength; i++)
        {
            // Makes a random ASCII character between ASCII value 32 and 123
            char randomChar = (char)randomGen.Next(32, 123);
            salt.Append(randomChar);
        }
        return salt.ToString();
    }

    private IEnumerator Delay(string mode)
    {
        // A delay the length of waitTime
        yield return new WaitForSeconds(waitTime);

        if (mode == "Hide password")
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
        }
        else if (mode == "Hide message")
        {
            message.gameObject.SetActive(false);
        }
    }

    public void ShowPassword()
    {
        passwordInput.contentType = TMP_InputField.ContentType.Standard;
        passwordInput.ForceLabelUpdate();

        StartCoroutine(Delay("Hide password"));
    }

    public void ErrorMessage()
    {
        message.gameObject.SetActive(true);
        message.gameObject.transform.localPosition = new Vector3(50, -200, 0);
        message.color = new Color32(201, 1, 1, 255);
    }

    public void SuccessMessage()
    {
        message.gameObject.SetActive(true);
        message.gameObject.transform.localPosition = new Vector3(75, 150, 0);
        message.color = new Color32(23, 175, 0, 255);
        StartCoroutine(Delay("Hide message"));
    }

    public void SignType()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;
        if (signType == "register")
        {
            SignUp(username, password);
        }
        else
        {
            SignIn(username, password);
        }
    }

    public void SignUp(string username, string password)
    {
        bool spaceBeginningUsername = true;
        // Checks validity of username and password
        if (!string.IsNullOrEmpty(username) && username[0] != ' ')
        {
            spaceBeginningUsername = false;
        }
        bool usernameExists = database.CheckForExistingData(username, "username");
        // Checks length of username and password
        if (spaceBeginningUsername || username.Length > 30 || username.Length < 1 || password.Length > 30 || password.Length < 1 || usernameExists)
        {
            ErrorMessage();
            if (spaceBeginningUsername || username.Length > 30 || username.Length < 1)
            {
                message.text = "Invalid username";
            }
            else if (password.Length > 30 || password.Length < 8)
            {
                message.text = "Invalid password";
            }
            else
            {
                message.text = "Username exists";
            }
        }
        else
        {
            (hashedPassword, salt) = Hashing(password, null);
            int ID = CreateID(username, hashedPassword);
            database.AddUserData(username, hashedPassword, salt, ID);
            Back();
            SuccessMessage();
            message.text = "Register successful!";
        }
    }

    public void SignIn(string username, string password)
    {
        // Checks whether user exists
        string salt = database.FetchSalt(username);
        if (salt == null)
        {
            ErrorMessage();
            message.text = "User doesn't exist";
            return;
        }
        (hashedPassword, _) = Hashing(password, salt);
        int passwordsMatch = database.CompareHashedPasswords(hashedPassword);
        if (passwordsMatch > 0)
        {
            usernameInput.text = null;
            passwordInput.text = null;
            DisplayLoginPage(false);
            message.gameObject.SetActive(false);
            loginText.SetActive(false);
            backgroundHandler.MenuBackground();
            menuSystem.DisplayMainMenu(true);
        }
        else
        {
            ErrorMessage();
            message.text = "Wrong password";
        }
    }
}