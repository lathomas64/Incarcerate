using UnityEngine;
using System.Collections;

public class GameJoltAPIManager : MonoBehaviour {

    public int gameID;
    public string privateKey;
    public string userName;
    public string userToken;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        GJAPI.Init(gameID, privateKey);
        GJAPI.Users.Verify(userName, userToken);
        GJAPI.Users.VerifyCallback += OnVerifyUser;
    }

    void OnEnable()
    {
        GJAPI.Users.VerifyCallback += OnVerifyUser;
    }

    void OnDisable()
    {
        GJAPI.Users.VerifyCallback -= OnVerifyUser;
    }

    void OnVerifyUser(bool success)
    {
        if (success)
        {
            Debug.Log("Yepee!");
        }
        else
        {
            Debug.Log("Um... Something went wrong.");
        }
    }
}
