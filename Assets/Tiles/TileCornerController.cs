using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCornerController : MonoBehaviour
{
    public GameObject topLeft, topRight, bottomLeft, bottomRight;
    public bool reversed;

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

        // Cancel all rotation on the inner corners
        topLeft.transform.rotation = Quaternion.Euler(0, 0, 0);
        topRight.transform.rotation = Quaternion.Euler(0, 0, 0);
        bottomLeft.transform.rotation = Quaternion.Euler(0, 0, 0);
        bottomRight.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (reversed)
        {
            topLeft.SetActive(up && left && upLeft);
            topRight.SetActive(up && right && upRight);
            bottomLeft.SetActive(down && left && downLeft);
            bottomRight.SetActive(down && right && downRight);
        }
        else
        {
            topLeft.SetActive(up && left && !upLeft);
            topRight.SetActive(up && right && !upRight);
            bottomLeft.SetActive(down && left && !downLeft);
            bottomRight.SetActive(down && right && !downRight);
        }
    }
}
