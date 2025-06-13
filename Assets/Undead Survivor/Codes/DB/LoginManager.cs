using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;

    public void OnLoginButtonClick()
    {
        string email = inputEmail.text.Trim();
        string password = inputPassword.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("이메일 또는 비밀번호가 비어 있습니다.");
            return;
        }

        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogWarning("로그인 실패, 사용자 존재하지 않을 수 있음. 회원가입 시도...");

                    FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password)
                        .ContinueWithOnMainThread(createTask =>
                        {
                            if (createTask.IsCanceled || createTask.IsFaulted)
                            {
                                Debug.LogError("회원가입 실패: " + createTask.Exception?.Flatten().InnerExceptions[0].Message);
                                return;
                            }

                            Debug.Log("회원가입 성공");
                            SessionData.userId = createTask.Result.User.UserId;
                            FirebaseManager.Instance.LoadMaxKill();

                            SceneManager.LoadScene("SampleScene"); // playerId 없이 바로 씬 전환
                        });
                }
                else
                {
                    Debug.Log("로그인 성공");
                    SessionData.userId = task.Result.User.UserId;
                    FirebaseManager.Instance.LoadMaxKill();

                    SceneManager.LoadScene(1); // playerId 없이 바로 씬 전환
                }
            });
    }
}





