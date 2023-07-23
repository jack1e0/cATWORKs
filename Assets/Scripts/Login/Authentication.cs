using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Authentication : MonoBehaviour {
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text loginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    [Header("UI")]
    //Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;

    void Awake() {
        registerUI.SetActive(false);
        loginText.text = "";
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            } else {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase() {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    //Function for the login button
    public async void LoginButton() {
        //Call the login coroutine passing the email and password
        await Login(emailLoginField.text, passwordLoginField.text);
    }
    //Function for the register button
    public async void RegisterButton() {
        //Call the register coroutine passing the email, password, and username
        await Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text);
    }

    private async Task Login(string _email, string _password) {
        //Call the Firebase auth signin function passing the email and password
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        try {
            await LoginTask;

            User = new FirebaseUser(LoginTask.Result.User);
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            SceneTransition.instance.user = await LoadUserData(User.UserId, User.DisplayName, User.Email);
            loginText.text = "Logged In";
            SceneTransition.instance.firstEnteredRoom = true;
            SceneTransition.instance.ChangeScene("RoomScene");
            Debug.Log("change scene");
        } catch {
            if (LoginTask.Exception != null) {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Login Failed!";
                switch (errorCode) {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WrongPassword:
                        message = "Wrong Password";
                        break;
                    case AuthError.InvalidEmail:
                        message = "Invalid Email";
                        break;
                    case AuthError.UserNotFound:
                        message = "Account does not exist";
                        break;
                }
                loginText.text = message;
                Debug.Log(message);
            }
        }
    }

    private async Task<UserData> LoadUserData(string userId, string name, string email) {
        UserData user = new UserData(name, userId);
        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(userId).GetValueAsync();
        await DBTask;
        if (DBTask.Exception != null) {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        } else if (DBTask.Result.Value != null) {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            user.growth = int.Parse(snapshot.Child("growth").Value.ToString());
            user.catfoodCount = int.Parse(snapshot.Child("catfoodCount").Value.ToString());
            user.level = int.Parse(snapshot.Child("level").Value.ToString());
            user.currXP = int.Parse(snapshot.Child("currXP").Value.ToString());
            user.maxXP = int.Parse(snapshot.Child("maxXP").Value.ToString());
            user.currHappiness = int.Parse(snapshot.Child("currHappiness").Value.ToString());
            user.prevExitTime = snapshot.Child("prevExitTime").Value.ToString();
            user.alarmId = int.Parse(snapshot.Child("alarmId").Value.ToString());

            if (snapshot.Child("alarmDict").Value.ToString() == null) {
                user.alarmDict = null;
            } else {
                user.alarmDict = JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(snapshot.Child("alarmDict").Value.ToString());
            }
        }

        return user;

    }

    private async Task Register(string _email, string _password, string _username) {
        if (_username == "") {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        } else if (passwordRegisterField.text != passwordRegisterVerifyField.text) {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        } else {
            //Call the Firebase auth signin function passing the email and password
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            await RegisterTask;

            if (RegisterTask.Exception != null) {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode) {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            } else {
                //User has now been created
                //Now get the result
                User = new FirebaseUser(RegisterTask.Result.User);

                if (User != null) {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    Task ProfileTask = User.UpdateUserProfileAsync(profile);
                    await ProfileTask;

                    if (ProfileTask.Exception != null) {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    } else {
                        //Username is now set
                        //Now return to login screen
                        await WriteNewUser(User.UserId, User.DisplayName);
                        LoginScreen();
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }

    private async Task WriteNewUser(string userId, string name) {
        UserData user = new UserData(name, userId);
        string username = JsonConvert.SerializeObject(user.username);
        string id = JsonConvert.SerializeObject(user.userId);
        string growth = JsonConvert.SerializeObject(user.growth);
        string catfoodCount = JsonConvert.SerializeObject(user.catfoodCount);
        string level = JsonConvert.SerializeObject(user.level);
        string currXP = JsonConvert.SerializeObject(user.currXP);
        string maxXP = JsonConvert.SerializeObject(user.maxXP);
        string currHappiness = JsonConvert.SerializeObject(user.currHappiness);
        string prevExitTime = JsonConvert.SerializeObject(System.DateTime.Now.ToString());
        string alarmId = JsonConvert.SerializeObject(user.alarmId);
        string alarmDict = JsonConvert.SerializeObject(user.alarmDict);

        await DBreference.Child("users").Child(userId).Child("username").SetValueAsync(username);
        await DBreference.Child("users").Child(userId).Child("id").SetValueAsync(id);
        await DBreference.Child("users").Child(userId).Child("growth").SetValueAsync(growth);
        await DBreference.Child("users").Child(userId).Child("catfoodCount").SetValueAsync(catfoodCount);
        await DBreference.Child("users").Child(userId).Child("level").SetValueAsync(level);
        await DBreference.Child("users").Child(userId).Child("currXP").SetValueAsync(currXP);
        await DBreference.Child("users").Child(userId).Child("maxXP").SetValueAsync(maxXP);
        await DBreference.Child("users").Child(userId).Child("currHappiness").SetValueAsync(currHappiness);
        await DBreference.Child("users").Child(userId).Child("prevExitTime").SetValueAsync(prevExitTime);
        await DBreference.Child("users").Child(userId).Child("alarmId").SetValueAsync(alarmId);
        await DBreference.Child("users").Child(userId).Child("alarmDict").SetValueAsync(alarmDict);

    }


    public void LoginScreen() //Back button
    {
        loginUI.SetActive(true);
        registerUI.SetActive(false);
    }
    public void RegisterScreen() // Register button
    {
        loginUI.SetActive(false);
        registerUI.SetActive(true);
        warningRegisterText.text = string.Empty;
    }
}
