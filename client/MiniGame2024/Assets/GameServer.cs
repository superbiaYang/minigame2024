using System.Collections;
using UnityEngine;
using NativeWebSocket;
using SimpleJSON;
using System.Threading.Tasks;

public class GameServer : MonoBehaviour
{
    private WebSocket m_WebSocket;
    private bool m_IsTryingToReconnect = false;

    async void Start()
    {
        await ConnectWebSocket();
    }

    private async Task ConnectWebSocket()
    {
        m_WebSocket = new WebSocket("ws://localhost:8765");
        
        m_WebSocket.OnOpen += () =>
        {
            Debug.Log("Connected to server");
        };

        m_WebSocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            if (!m_IsTryingToReconnect)
            {
                StartCoroutine(TryReconnect());
            }
        };

        m_WebSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            if (!m_IsTryingToReconnect)
            {
                StartCoroutine(TryReconnect());
            }
        };

        m_WebSocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            HandleMessage(message);
        };

        await m_WebSocket.Connect();
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        m_WebSocket.DispatchMessageQueue();
#endif
    }

    public void RegisterGameServer()
    {
        m_WebSocket.SendText("{\"action\":\"register_gs\"}");
    }

    public void DeregisterGameServer()
    {
        m_WebSocket.SendText("{\"action\":\"deregister_gs\"}");
    }

    void OnDestroy()
    {
        if (m_WebSocket != null)
        {
            m_WebSocket.Close();
        }
    }

    private IEnumerator TryReconnect()
    {
        Debug.Log("Try reconnect!");
        m_IsTryingToReconnect = true;
        while (m_WebSocket.State != WebSocketState.Open)
        {
            Task connectTask = Task.Run(async () => await ConnectWebSocket());
            while (!connectTask.IsCompleted)
            {
                yield return null;
            }
            yield return new WaitForSeconds(1);
        }
        m_IsTryingToReconnect = false;
    }

    private void HandleMessage(string message)
    {
        Debug.Log("Message from server: " + message);

        var data = JSON.Parse(message);
        var action = data["action"].Value;

        if (action == "start_game")
        {
            Debug.Log("GS start game");
            var gameManager = FindObjectOfType<GameManager>();
            gameManager.GameInit(data["player_num"].AsInt);
        }
    }
}
