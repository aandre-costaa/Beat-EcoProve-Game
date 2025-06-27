using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RiverCleanup : MonoBehaviour
{
    [Header("1) Drag your Tilemap here (where all segments are painted)")]
    public Tilemap riverTilemap;

    [Space(10)]
    [Header("2) Configure each segment below:")]
    [Tooltip(
      "For each RiverSegment:\n" +
      "- levelNumber: which Level_(X)_Completed key to check\n" +
      "- changes:   a list of TileChange entries (dirtyTile/cleanTile + positions)\n" +
      "  ? each TileChange will only run on its own 'positions' list."
    )]
    public List<RiverSegment> segments = new List<RiverSegment>();

    private void Start()
    {
        if (riverTilemap == null)
        {
            Debug.LogError("[RiverCleanup] No Tilemap assigned in Inspector.");
            return;
        }

        foreach (RiverSegment seg in segments)
        {
            string prefsKey = "Level_" + seg.levelNumber + "_Completed";
            bool isDone = PlayerPrefs.GetInt(prefsKey, 0) == 1;
            if (!isDone)
            {
                continue;
            }

            foreach (TileChange change in seg.changes)
            {
                foreach (Vector3Int cell in change.positions)
                {
                    TileBase current = riverTilemap.GetTile(cell);
                    if (current == null)
                        continue;

                    if (current == change.dirtyTile)
                    {
                        riverTilemap.SetTile(cell, change.cleanTile);
                    }
                }
            }
        }

        riverTilemap.RefreshAllTiles();
    }
}
