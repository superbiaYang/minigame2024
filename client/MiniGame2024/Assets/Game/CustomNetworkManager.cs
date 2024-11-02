using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public GameManager m_GameManager;
    public Transform[] m_SpawnPoints;
    private List<int> m_AvailableSpawnIndices = new List<int>();
    public override void OnStartServer()
    {
        base.OnStartServer();
        InitializeSpawnPoints();
        var gs = FindObjectOfType<GameServer>();
        if (gs != null) gs.RegisterGameServer();
    }

    public override void OnStopServer()
    {
        var gs = FindObjectOfType<GameServer>();
        if (gs != null) gs.DeregisterGameServer();
        base.OnStopServer();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform startPoint = GetUniqueSpawnPoint();
        GameObject player = Instantiate(playerPrefab, startPoint.position, startPoint.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
        m_GameManager.AddPlayer(player.GetComponent<CarController>());
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            GameObject player = conn.identity.gameObject;
            m_GameManager.RemovePlayer(player.GetComponent<CarController>());
        }

        base.OnServerDisconnect(conn);
    }

    private void InitializeSpawnPoints()
    {
        m_AvailableSpawnIndices.Clear();
        for (int i = 0; i < m_SpawnPoints.Length; i++)
        {
            m_AvailableSpawnIndices.Add(i);
        }
    }

    private Transform GetUniqueSpawnPoint()
    {
        if (m_AvailableSpawnIndices.Count == 0)
        {
            InitializeSpawnPoints();
        }

        int randomIndex = Random.Range(0, m_AvailableSpawnIndices.Count);
        int spawnIndex = m_AvailableSpawnIndices[randomIndex];

        m_AvailableSpawnIndices.RemoveAt(randomIndex);
        return m_SpawnPoints[spawnIndex];
    }
}
