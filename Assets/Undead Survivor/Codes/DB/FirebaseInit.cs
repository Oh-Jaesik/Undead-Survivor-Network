using Firebase;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    void Awake() // ? 반드시 Awake로 변경
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // ? Database URL 설정
                app.Options.DatabaseUrl = new System.Uri("https://undeadsurvivor-77af8-default-rtdb.firebaseio.com/");

                Debug.Log("? Firebase 초기화 성공 + Database URL 설정 완료");
            }
            else
            {
                Debug.LogError("? Firebase 초기화 실패: " + task.Result);
            }
        });
    }
}

