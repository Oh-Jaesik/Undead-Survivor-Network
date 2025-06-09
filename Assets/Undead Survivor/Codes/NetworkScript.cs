using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class NetworkScript : NetworkBehaviour
{
    public Canvas canvas;
    public InputField addressInput;

    void Start()
    {
        // 초기 기본 주소 설정
        if (string.IsNullOrEmpty(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = "127.0.0.1";
        }

        if (addressInput != null)
        {
            addressInput.text = NetworkManager.singleton.networkAddress;
        }
    }

    public void StartHost()
    {
        if (!NetworkClient.active && !NetworkServer.active)
        {
            Debug.Log("Starting Host...");
            NetworkManager.singleton.StartHost();
        }
    }

    public void StartClient()
    {
        if (!NetworkClient.active)
        {
            string address = "127.0.0.1"; // 기본 주소

            if (addressInput != null && !string.IsNullOrWhiteSpace(addressInput.text))
            {
                address = addressInput.text;
            }

            NetworkManager.singleton.networkAddress = address;
            Debug.Log("Starting Client to address: " + address);
            NetworkManager.singleton.StartClient();
        }
    }

    public void StopNetwork()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            // Host일 경우
            Debug.Log("Stopping Host...");
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            // Client일 경우
            Debug.Log("Stopping Client...");
            NetworkManager.singleton.StopClient();

            //// 필요 시 UI나 씬 상태 리셋
            //if (canvas != null)
            //    canvas.gameObject.SetActive(true); // 다시 UI 보여주기
        }
        else if (NetworkServer.active)
        {
            // Dedicated server
            Debug.Log("Stopping Server...");
            NetworkManager.singleton.StopServer();
        }
    }
}
