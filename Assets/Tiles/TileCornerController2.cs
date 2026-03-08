using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCornerController2 : MonoBehaviour
{
    public GameObject cliffTL, cliffTR, cliffBL, cliffBR;
    public GameObject groundTL, groundTR, groundBL, groundBR;

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
        cliffTL.transform.rotation = Quaternion.Euler(0, 0, 0);
        cliffTR.transform.rotation = Quaternion.Euler(0, 0, 0);
        cliffBL.transform.rotation = Quaternion.Euler(0, 0, 0);
        cliffBR.transform.rotation = Quaternion.Euler(0, 0, 0);
        groundTL.transform.rotation = Quaternion.Euler(0, 0, 0);
        groundTR.transform.rotation = Quaternion.Euler(0, 0, 0);
        groundBL.transform.rotation = Quaternion.Euler(0, 0, 0);
        groundBR.transform.rotation = Quaternion.Euler(0, 0, 0);

        // Set cliff inner corner states
        cliffTL.SetActive(up && left && !upLeft);
        cliffTR.SetActive(up && right && !upRight);
        cliffBL.SetActive(down && left && !downLeft);
        cliffBR.SetActive(down && right && !downRight);

        // Set ground inner corner states
        groundTL.SetActive(up && left && upLeft);
        groundTR.SetActive(up && right && upRight);
        groundBL.SetActive(down && left && downLeft);
        groundBR.SetActive(down && right && downRight);
    }
}
