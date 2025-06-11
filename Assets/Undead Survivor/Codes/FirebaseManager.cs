using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    public DatabaseReference dbRef;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(InitFirebase());
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator InitFirebase()
    {
        yield return new WaitForSeconds(0.1f);

        var checkTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => checkTask.IsCompleted);

        if (checkTask.Result == DependencyStatus.Available)
        {
            dbRef = FirebaseDatabase
                .GetInstance("https://undeadsurvivor-77af8-default-rtdb.firebaseio.com/")
                .RootReference;

            Debug.Log("Firebase 초기화 성공");
        }
        else
        {
            Debug.LogError("Firebase 초기화 실패: " + checkTask.Result);
        }
    }

    public void SaveUserData()
    {
        Debug.Log("savedata");
        if (FirebaseAuth.DefaultInstance.CurrentUser == null || dbRef == null)
        {
            Debug.LogWarning("SaveUserData: 사용자 인증 또는 DB 참조가 없음");
            return;
        }

        string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        Dictionary<string, object> userData = new Dictionary<string, object>
    {
        { "level", GameManager.instance.level },
        { "exp", GameManager.instance.exp },
        { "kill", GameManager.instance.kill },
        { "maxHealth", (int)GameManager.instance.player.maxHealth },
        { "playerId", GameManager.instance.playerId }
    };

        dbRef.Child("users").Child(uid).UpdateChildrenAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SaveUserData: 저장 취소됨");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("SaveUserData: 저장 중 오류 발생 - " + task.Exception);
            }
            else
            {
                Debug.Log("SaveUserData: 사용자 데이터 저장 완료");
            }
        });
    }


    public void ResetUserData()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser == null || dbRef == null)
        {
            Debug.LogWarning("ResetUserData: Firebase 초기화 전이거나 로그인되지 않음");
            return;
        }

        string uid = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        Dictionary<string, object> resetData = new Dictionary<string, object>
        {
            { "level", 0 },
            { "exp", 0 },
            { "kill", 0 },
            { "maxHealth", 100 },
            { "playerId", GameManager.instance.playerId }
        };

        dbRef.Child("users").Child(uid).SetValueAsync(resetData);
    }
}

















