using UnityEngine;
using System.Collections.Generic;
using Mirror;

public enum GameStatus
{
    Pending,
    WaitingForConnection,
    Preparing,
    Matching,
    GameOver,
}

public class GameManager : NetworkBehaviour
{
    public SafeZoneController m_SafeZone;

    [SyncVar]
    public GameStatus m_Status;
    [SyncVar]
    public int m_TotalPlayerNum = 2;
    [SyncVar(hook = nameof(OnConnectedPlayerNumChanged))]
    private int m_ConnectedPlayerNum;

    private List<CarController> m_Cars = new List<CarController>();

    void Update()
    {
        if (!isServer)
        {
            return;
        }
        CheckGameStart();
        CheckGameOver();
        GameTick();
    }

    [Server]
    public void GameInit(int playerNum)
    {
        if (m_Status != GameStatus.Pending)
        {
            Debug.LogError($"Can't init game current game status is {m_Status}");
            return;
        }
        m_Status = GameStatus.WaitingForConnection;
        m_TotalPlayerNum = playerNum;
        m_ConnectedPlayerNum = 0;
        m_Cars.Clear();
        m_SafeZone.Init();
    }

    public void AddPlayer(CarController car)
    {
        if (m_Status != GameStatus.WaitingForConnection)
        {
            Debug.LogError($"Can't add player current game stauts is {m_Status}");
            return;
        }
        m_ConnectedPlayerNum++;
        m_Cars.Add(car);
        Debug.Log($"[server] client connect {m_ConnectedPlayerNum} / {m_TotalPlayerNum}");
    }

    public void RemovePlayer(CarController car)
    {
        m_Cars.Remove(car);
    }

    private void OnConnectedPlayerNumChanged(int oldNum, int newNum)
    {
        Debug.Log($"[client] client connect {m_ConnectedPlayerNum} / {m_TotalPlayerNum}");
    }

    private void CheckGameStart()
    {
        if (m_Status != GameStatus.WaitingForConnection)
        {
            return;
        }
        if (m_ConnectedPlayerNum < m_TotalPlayerNum)
        {
            return;
        }
        Debug.Log("[server] game start");
        foreach (var car in m_Cars)
        {
            car.Init();
        }
        m_SafeZone.StartShrinking();
        m_Status = GameStatus.Matching;
        RpcGameStart();
    }

    [ClientRpc]
    private void RpcGameStart()
    {
        Debug.Log($"[client] game start");
        m_Status = GameStatus.Matching;
    }

    private void GameTick()
    {
        if (m_Status != GameStatus.Matching)
        {
            return;
        }
        m_SafeZone.CheckDamage(m_Cars);
    }

    private void CheckGameOver()
    {
        if (m_Status != GameStatus.Matching)
        {
            return;
        }
        int livedCar = 0;
        foreach(var car in m_Cars)
        {
            if (car.IsAlive())
            {
                livedCar++;
            }
        }
        if (livedCar <= 1)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        m_Status = GameStatus.GameOver;
        Debug.Log("game over");
        RpcGameOver();
        m_Status = GameStatus.Pending;
    }

    [ClientRpc]
    private void RpcGameOver()
    {
        Debug.Log($"[client] game over");
        m_Status = GameStatus.GameOver;
        m_Status = GameStatus.Pending;
    }
}