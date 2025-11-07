using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button host;
    [SerializeField] private Button client;

    private MenuSystem menuSystem;

    private void Start()
    {
        menuSystem = GameObject.FindGameObjectWithTag("NEALogic").GetComponent<MenuSystem>();

        host.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log($"Host started. LocalClientId: {NetworkManager.Singleton.LocalClientId}");
            menuSystem.HostGame();
        });

        client.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log($"Client connecting. LocalClientId: {NetworkManager.Singleton.LocalClientId}");
            StartCoroutine(WaitForClientAndJoinGame());
        });

        NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApproval;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= OnConnectionApproval;
        }
    }

    private void OnConnectionApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // Approve connection
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.Pending = false;

        Debug.Log($"Connection approved for client {request.ClientNetworkId}. Waiting for connection.");
    }

    private IEnumerator WaitForClientAndJoinGame()
    {
        // Wait until the client is connected
        while (!NetworkManager.Singleton.IsConnectedClient)
        {
            yield return null;
        }

        Debug.Log("Client successfully connected.");
        yield return new WaitForSeconds(1f); // Small delay
        menuSystem.JoinGame();
        Debug.Log("Client joined the game.");
    }
}
