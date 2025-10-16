using Unity.Netcode;
using UnityEngine;

public class NetworkManagerController : MonoBehaviour
{
    public void StartHost()
    {
        if (!Unity.Netcode.NetworkManager.Singleton.IsListening)
            Unity.Netcode.NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        if (!Unity.Netcode.NetworkManager.Singleton.IsListening)
            Unity.Netcode.NetworkManager.Singleton.StartClient();
    }

    public void StartServer()
    {
        if (!Unity.Netcode.NetworkManager.Singleton.IsListening)
            Unity.Netcode.NetworkManager.Singleton.StartServer();
    }
}
