using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public InputField usernameField;
    [SerializeField] public InputField passwordField;
    [SerializeField] public Button loginButton;
    [SerializeField] public Button registerAndLoginButton;
    [SerializeField] public Text feedbackText;

    [Header("Panels")]
    [SerializeField] public GameObject loginPanel;
    [SerializeField] public GameObject gameMenuPanel;

    // ---------- Accounts store ----------
    [Serializable]
    private class Account { public string name; public string salt; public string hash; }

    [Serializable]
    private class AccountList { public List<Account> accounts = new List<Account>(); }

    private AccountList store = new AccountList();
    private string StorePath => Path.Combine(Application.persistentDataPath, "accounts.json");

    // Make the current user visible to other systems (AssignmentPart1)
    public static string CurrentUser { get; private set; } = null;

    // ---------- Unity ----------
    private void Start()
    {
        LoadStore();

        if (loginButton) loginButton.onClick.AddListener(OnLoginClicked);
        if (registerAndLoginButton) registerAndLoginButton.onClick.AddListener(OnRegisterAndLoginClicked);

        SetFeedback("");
        if (gameMenuPanel) gameMenuPanel.SetActive(false);
    }

    // ---------- Button handlers ----------
    private void OnLoginClicked()
    {
        var user = (usernameField ? usernameField.text : "").Trim();
        var pass = (passwordField ? passwordField.text : "");

        if (!ValidateInputs(user, pass)) return;

        var result = TryLogin(user, pass);
        switch (result)
        {
            case LoginResult.Success:
                OnLoginSuccess(user);
                break;
            case LoginResult.WrongPassword:
                SetFeedback("Wrong password", false);
                break;
            case LoginResult.NoSuchUser:
                SetFeedback("Account not found", false);
                break;
        }
    }

    private void OnRegisterAndLoginClicked()
    {
        var user = (usernameField ? usernameField.text : "").Trim();
        var pass = (passwordField ? passwordField.text : "");

        if (!ValidateInputs(user, pass)) return;

        var existing = FindAccount(user);
        if (existing == null)
        {
            CreateAccount(user, pass);
            SaveStore();
            OnLoginSuccess(user);
        }
        else
        {
            var result = TryLogin(user, pass);
            if (result == LoginResult.Success)
                OnLoginSuccess(user);
            else
                SetFeedback(result == LoginResult.WrongPassword ? "Wrong password" : "Account not found", false);
        }
    }

    // Centralized success flow (instance method)
    private void OnLoginSuccess(string user)
    {
        CurrentUser = user;
        SetFeedback($"Logged in as {user}", true);
        SwitchToGameMenu();
        LoadPartyForCurrentUser();
    }

    private void LoadPartyForCurrentUser()
    {
        // This calls AssignmentPart1 which now loads/saves per user (see change below)
        try
        {
            AssignmentPart1.LoadPartyButtonPressed(); // will no-op if no file yet
        }
        catch
        {
            GameContent.partyCharacters = new LinkedList<PartyCharacter>(); // start empty for new account
            GameContent.RefreshUI();
        }
    }

    // ---------- Core auth ----------
    private enum LoginResult { Success, WrongPassword, NoSuchUser }

    private LoginResult TryLogin(string name, string password)
    {
        var acct = FindAccount(name);
        if (acct == null) return LoginResult.NoSuchUser;

        string computed = Hash(acct.salt + password);
        return (computed == acct.hash) ? LoginResult.Success : LoginResult.WrongPassword;
    }

    private Account FindAccount(string name)
    {
        if (store?.accounts == null) return null;
        return store.accounts.Find(a => string.Equals(a.name, name, StringComparison.OrdinalIgnoreCase));
    }

    private void CreateAccount(string name, string password)
    {
        string salt = MakeSalt();
        string hash = Hash(salt + password);
        store.accounts.Add(new Account { name = name, salt = salt, hash = hash });
    }

    // ---------- Storage ----------
    private void LoadStore()
    {
        try
        {
            if (File.Exists(StorePath))
            {
                string json = File.ReadAllText(StorePath);
                store = JsonUtility.FromJson<AccountList>(json);
                if (store == null) store = new AccountList();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to load account store: {e.Message}");
            store = new AccountList();
        }
    }

    private void SaveStore()
    {
        try
        {
            string json = JsonUtility.ToJson(store, true);
            File.WriteAllText(StorePath, json);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to save account store: {e.Message}");
        }
    }

    // ---------- Utilities ----------
    private bool ValidateInputs(string user, string pass)
    {
        if (string.IsNullOrWhiteSpace(user))
        {
            SetFeedback("Enter a username", false);
            return false;
        }
        if (string.Equals(user, "username", StringComparison.OrdinalIgnoreCase))
        {
            SetFeedback("Invalid username", false);
            return false;
        }
        if (string.IsNullOrEmpty(pass))
        {
            SetFeedback("Enter a password", false);
            return false;
        }
        return true;
    }

    private void SetFeedback(string msg, bool ok = true)
    {
        if (!feedbackText) return;
        feedbackText.text = msg;
        feedbackText.color = ok ? Color.green : Color.red;
    }

    private static string MakeSalt(int bytes = 16)
    {
        var buf = new byte[bytes];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(buf);
        return Convert.ToBase64String(buf);
    }

    private static string Hash(string s)
    {
        using (var sha = SHA256.Create())
        {
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
            return Convert.ToBase64String(bytes);
        }
    }

    // ---------- Panel switching ----------
    private void SwitchToGameMenu()
    {
        if (loginPanel) loginPanel.SetActive(false);
        if (gameMenuPanel) gameMenuPanel.SetActive(true);
    }
}
