using UnityEngine;
using NativeWebSocket;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Threading.Tasks;

public class RoomManager : MonoBehaviour
{
    public RoomList m_RoomList;
    public RoomDetail m_RoomDetail;
    public GameObject m_CreateButton;
    public GameObject m_JoinButton;
    public GameObject m_StartButton;
    private WebSocket websocket;
    private string playerName = "Player1";
    private int currentRoomId = -1;
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
            GetRooms();
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
        
        if (action == "room_created" || action == "room_joined")
        {
            m_CreateButton.SetActive(false);
            m_JoinButton.SetActive(false);
            m_StartButton.SetActive(true);
            m_RoomList.gameObject.SetActive(false);
            m_RoomDetail.gameObject.SetActive(true);
            Debug.Log(m_RoomList);
            currentRoomId = data["room"]["room_id"].AsInt;
            m_RoomDetail.SetRoomName(data["room"]["creator"].Value);
            JSONArray playersArray = data["room"]["players"].AsArray;
            List<string> playersList = new List<string>();
            foreach (var player in playersArray)
            {
                playersList.Add(player.Value["name"].Value);
            }
            m_RoomDetail.DisplayPlayers(playersList);
        }
        else if (action == "rooms_list")
        {
            m_CreateButton.SetActive(true);
            m_JoinButton.SetActive(true);
            m_StartButton.SetActive(false);
            m_RoomList.gameObject.SetActive(true);
            m_RoomDetail.gameObject.SetActive(false);
            JSONArray roomsArray = data["rooms"].AsArray;
            List<RoomBrief> roomsList = new List<RoomBrief>();
            foreach (var room in roomsArray)
            {
                roomsList.Add(new RoomBrief(room.Value["creator"].Value, room.Value["room_id"].AsInt));
            }
            m_RoomList.DisplayRoomList(roomsList);
        }
    }

    private void DisplayPlayerList(List<object> players)
    {
        Debug.Log("Players in room:");
        foreach (var player in players)
        {
            Debug.Log("Player: " + player);
        }
    }

    public async void GetRooms()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("{\"action\":\"get_rooms\"}");
        }
    }

    public async void CreateRoom()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText($"{{\"action\":\"create_room\", \"creator\":\"{PlayerName()}\"}}");
        }
    }

    public async void JoinRoom()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText($"{{\"action\":\"join_room\", \"room_id\":{m_RoomList.m_SelectedRoom}, \"player_name\":\"{PlayerName()}\"}}");
        }
    }

    public async void LeaveRoom()
    {
        if (currentRoomId != -1 && websocket.State == WebSocketState.Open)
        {
            await websocket.SendText($"{{\"action\":\"leave_room\", \"room_id\":{currentRoomId}, \"player_name\":\"{playerName}\"}}");
            currentRoomId = -1;
        }
    }

    public async void StartGame()
    {
        if (currentRoomId != -1 && websocket.State == WebSocketState.Open)
        {
            await websocket.SendText($"{{\"action\":\"start_game\", \"room_id\":{currentRoomId}}}");
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

    protected virtual string PlayerName()
    {
        return $"Player{Random.Range(1, 1000)}";
    }
}
