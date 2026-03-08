using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapRefresher : MonoBehaviour
{
    void Start()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.RefreshAllTiles();

        // Ensure the inner corner logic is activate on all tiles when the tilemap loads
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            GameObject map = tilemap.GetInstantiatedObject(pos);
            if (map == null) continue;

            var cornerController = map.GetComponent<TileCornerController2>();
            if (cornerController != null)
                cornerController.UpdateCorners(tilemap, pos);
        }
    }
}
