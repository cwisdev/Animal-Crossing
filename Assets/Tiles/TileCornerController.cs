using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCornerController : MonoBehaviour
{
    public GameObject topLeft, topRight, bottomLeft, bottomRight;

    public void UpdateCorners(Tilemap tilemap, Vector3Int position)
    {
        bool up = tilemap.HasTile(position + Vector3Int.up);
        bool down = tilemap.HasTile(position + Vector3Int.down);
        bool left = tilemap.HasTile(position + Vector3Int.left);
        bool right = tilemap.HasTile(position + Vector3Int.right);

        bool upLeft = tilemap.HasTile(position + Vector3Int.up + Vector3Int.left);
        bool upRight = tilemap.HasTile(position + Vector3Int.up + Vector3Int.right);
        bool downLeft = tilemap.HasTile(position + Vector3Int.down + Vector3Int.left);
        bool downRight = tilemap.HasTile(position + Vector3Int.down + Vector3Int.right);

        topLeft.transform.rotation = Quaternion.Euler(0, 90, 0);
        topRight.transform.rotation = Quaternion.Euler(0, 180, 0);
        bottomLeft.transform.rotation = Quaternion.Euler(0, 0, 0);
        bottomRight.transform.rotation = Quaternion.Euler(0, 270, 0);

        topLeft.SetActive(up && left && !upLeft);
        topRight.SetActive(up && right && !upRight);
        bottomLeft.SetActive(down && left && !downLeft);
        bottomRight.SetActive(down && right && !downRight);
    }
}
