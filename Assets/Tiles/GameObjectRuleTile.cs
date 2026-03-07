using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GameObjectRuleTile : RuleTile<GameObjectRuleTile.Neighbor>
{
    static readonly Vector3Int[] neighborOffsets =
    {
        new Vector3Int(-1, 0, -1),
        new Vector3Int(0, 0, -1),
        new Vector3Int(1, 0, -1),

        new Vector3Int(-1, 0, 0),
        new Vector3Int(1, 0, 0),

        new Vector3Int(-1, 0, 1),
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 0, 1)
    };

    public class Neighbor : RuleTile.TilingRule.Neighbor { }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);

        Tilemap map = tilemap.GetComponent<Tilemap>();

        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int pos = position + new Vector3Int(x, y, 0);
                tilemap.RefreshTile(pos);

                if (map != null)
                {
                    GameObject tileInstance = map.GetInstantiatedObject(pos);
                    if (tileInstance != null)
                        tileInstance.GetComponent<TileCornerController>()?.UpdateCorners(map, pos);
                }
            }

        // Refresh all neighbours
        foreach (Vector3Int offset in neighborOffsets)
            tilemap.RefreshTile(position + offset);

    }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject instantiatedGameObject)
    {
        if (instantiatedGameObject != null)
        {
            var tmpMap = tilemap.GetComponent<Tilemap>();
            var orientMatrix = tmpMap.orientationMatrix;

            var iden = Matrix4x4.identity;
            var gameObjectTranslation = new Vector3();
            var gameObjectRotation = new Quaternion();
            var gameObjectScale = new Vector3();

            var ruleMatched = false;
            var transform = iden;
            foreach (var rule in m_TilingRules)
                if (RuleMatches(rule, position, tilemap, ref transform))
                {
                    transform = orientMatrix * transform;

                    // Converts the tile's translation, rotation, & scale matrix to values to be used by the instantiated GameObject
                    gameObjectTranslation = new Vector3(transform.m03, transform.m13, transform.m23);
                    gameObjectRotation = Quaternion.LookRotation(
                        new Vector3(transform.m02, transform.m12, transform.m22),
                        new Vector3(transform.m01, transform.m11, transform.m21));
                    gameObjectRotation.x = 0;
                    gameObjectRotation.z = 0;
                    gameObjectScale = transform.lossyScale;

                    ruleMatched = true;
                    break;
                }

            if (!ruleMatched)
            {
                // Fallback to just using the orientMatrix for the translation, rotation, & scale values.
                gameObjectTranslation = new Vector3(orientMatrix.m03, orientMatrix.m13, orientMatrix.m23);
                gameObjectRotation = Quaternion.LookRotation(
                    new Vector3(orientMatrix.m02, orientMatrix.m12, orientMatrix.m22),
                    new Vector3(orientMatrix.m01, orientMatrix.m11, orientMatrix.m21));
                gameObjectRotation.x = 0;
                gameObjectRotation.z = 0;
                gameObjectScale = orientMatrix.lossyScale;
            }

            instantiatedGameObject.transform.localPosition = gameObjectTranslation +
                                                             tmpMap.CellToLocalInterpolated(position +
                                                                 tmpMap.tileAnchor);
            instantiatedGameObject.transform.localRotation = gameObjectRotation;
            instantiatedGameObject.transform.localScale = gameObjectScale;
        }

        return true;
    }
}
