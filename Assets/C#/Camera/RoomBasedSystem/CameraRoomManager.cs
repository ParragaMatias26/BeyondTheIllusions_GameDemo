using System.Collections.Generic;
using UnityEngine;

public class CameraRoomManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private CameraFollow myCam;
    [SerializeField] private List<RoomArea> rooms;
    public List<RoomArea> LevelRooms { get { return rooms; } }

    [SerializeField] private RoomArea currentRoom;

    public void SetBounds(RoomArea room) 
    {
        currentRoom = room;
        myCam.SetBounds(currentRoom.GetWorldMinBounds(), currentRoom.GetWorldMaxBounds());
        myCam.RecenterCamera();
    }
    private void Update()
    {
        if (rooms == null || player == null || myCam == null)
            return;

        Vector2 playerPos = player.position;

        foreach (RoomArea room in rooms)
        {
            if (room.Contains(playerPos) && room != currentRoom)
            {
                currentRoom = room;
                myCam.SetBounds(currentRoom.GetWorldMinBounds(), currentRoom.GetWorldMaxBounds());
                myCam.RecenterCamera();
                break;
            }
        }
    }
}
