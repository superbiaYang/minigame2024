using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public struct RoomBrief
{
    public string Name;
    public int Id;

    public RoomBrief(string name, int id)
    {
        Name = name;
        Id = id;
    }
}
public class RoomList : MonoBehaviour
{
    public GameObject m_Room;
    [HideInInspector] public int m_SelectedRoom;
    private Transform m_ContentTransform;
    private RectTransform m_ContentRectTransform;
    private float m_RoomHegiht;

    void Awake()
    {
        m_ContentTransform = transform.Find("Viewport/Content");
        m_ContentRectTransform = m_ContentTransform.GetComponent<RectTransform>();
        m_RoomHegiht = m_Room.GetComponent<RectTransform>().rect.height;
    }

    public void DisplayRoomList(List<RoomBrief> rooms)
    {
        foreach (Transform child in m_ContentTransform)
        {
            Destroy(child.gameObject);
        }
        
        foreach (var room in rooms)
        {
            var roomUi = Instantiate(m_Room);
            roomUi.transform.SetParent(m_ContentTransform, false); 

            Text text = roomUi.GetComponentInChildren<Text>();
            text.text = $"{room.Name}'s room";

            Button button = roomUi.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => OnRoomButtonClicked(room.Id));
        }
        
        var height = m_RoomHegiht * rooms.Count;
        m_ContentRectTransform.sizeDelta = new Vector2(m_ContentRectTransform.sizeDelta.x, height);
    }

    
    private void OnRoomButtonClicked(int roomId)
    {
        m_SelectedRoom = roomId; 
    }
}
