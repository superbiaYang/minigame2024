using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RoomDetail : MonoBehaviour
{
    public TMP_Text m_RoomName;
    public GameObject m_Player;
    private Transform m_ContentTransform;
    private RectTransform m_ContentRectTransform;
    private float m_PlayerHegiht;

    void Awake()
    {
        m_ContentTransform = transform.Find("Viewport/Content");
        m_ContentRectTransform = m_ContentTransform.GetComponent<RectTransform>();
        m_PlayerHegiht = m_Player.GetComponent<RectTransform>().rect.height;

        DisplayPlayers(new List<string>{"test", "test", "test"});
    }

    public void SetRoomName(string creator)
    {
        m_RoomName.text = $"{creator}'s Room";
    }
    public void DisplayPlayers(List<string> players)
    {
        foreach (Transform child in m_ContentTransform) { Destroy(child.gameObject); }
        foreach (var player in players)
        {
            var playerUi = Instantiate(m_Player);
            playerUi.transform.SetParent(m_ContentTransform);

            Text text = playerUi.GetComponentInChildren<Text>();
            text.text = player;
        }
        var height = m_PlayerHegiht * players.Count;
        m_ContentRectTransform.sizeDelta = new Vector2(m_ContentRectTransform.sizeDelta.x, height);
    }
}
