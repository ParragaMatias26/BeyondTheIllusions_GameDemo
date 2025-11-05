using UnityEngine;

[System.Serializable]
public class RoomArea : MonoBehaviour
{
    [Header("Room Info")]
    public string roomName = "Room";

    [Header("Room Bounds (half size)")]
    public Vector2 roomBounds = new Vector2(5, 3);

    [Header("Gizmo")]
    public Color gizmoColor = Color.cyan;

    private CameraRoomManager myRoomManager;

    public Vector2 GetWorldMinBounds() => (Vector2)transform.position - roomBounds;
    public Vector2 GetWorldMaxBounds() => (Vector2)transform.position + roomBounds;

    private void Start()
    {
        myRoomManager = GameManager.Instance.RoomManager;
        myRoomManager.LevelRooms.Add(this);
    }

    public bool Contains(Vector2 position)
    {
        Vector2 worldMin = GetWorldMinBounds();
        Vector2 worldMax = GetWorldMaxBounds();

        return position.x >= worldMin.x && position.x <= worldMax.x &&
               position.y >= worldMin.y && position.y <= worldMax.y;
    }

    //Cambiar al Terminar
    private void OnDrawGizmos()
    {
        Vector3 worldMin = transform.position - (Vector3)roomBounds;
        Vector3 worldMax = transform.position + (Vector3)roomBounds;

        Vector3 center = transform.position;
        Vector3 size = worldMax - worldMin;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(center, size);
    }
}
