using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileChange
{
    [Tooltip("The Tile asset you originally painted for this sub?section (dirty).")]
    public TileBase dirtyTile;

    [Tooltip("The Tile asset you want to replace with (clean).")]
    public TileBase cleanTile;

    [Tooltip(
      "Exact Grid Cell Positions (Vector3Int) to check for dirtyTile. \n" +
      "Only at these coordinates will we swap dirtyTile ? cleanTile."
    )]
    public List<Vector3Int> positions = new List<Vector3Int>();
}

[System.Serializable]
public class RiverSegment
{
    [Tooltip("When this PlayerPrefs key = 1, we swap ALL the TileChange entries in this segment.")]
    public int levelNumber;

    [Tooltip("List of dirty?clean tile?pairs (with their own position?lists).")]
    public List<TileChange> changes = new List<TileChange>();
}
