using UnityEngine;
using NativeWebSocket;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Threading.Tasks;

public class GsServerConnector : MonoBehaviour
{
    private WebSocket websocket;
    private bool isTryingToReconnect = false;
 
    async void Start()
    {
        await ConnectWebSocket();
    }

    private async Task ConnectWebSocket()
    {
        websocket = new WebSocket("ws://localhost:8765");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            RegisterGs();
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            if (!isTryingToReconnect)
            {
                StartCoroutine(TryReconnect());
            }
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            if (!isTryingToReconnect)
            {
                StartCoroutine(TryReconnect());
            }
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            HandleMessage(message);
        };

        await websocket.Connect();
    }

    private IEnumerator TryReconnect()
    {
        isTryingToReconnect = true;
        while (websocket.State != WebSocketState.Open)
        {
            Debug.Log("Attempting to reconnect...");
            // 使用 Task.Run 来调用异步方法
            Task connectTask = Task.Run(async () => await ConnectWebSocket());
            while (!connectTask.IsCompleted)
            {
                yield return null;
            }
            yield return new WaitForSeconds(1);
        }
        isTryingToReconnect = false;
    }

    private void HandleMessage(string message)
    {
        Debug.Log("Message from server: " + message);

        var data = JSON.Parse(message);
        var action = data["action"].Value;
        
        if (action == "gs_registered")
        {
        }
        else if (action == "init_game")
        {
            
        }
    }

    public async void RegisterGs()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("{\"action\":\"register_gs\"}");
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

}
