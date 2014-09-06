using UnityEngine;
using System.Collections;

/// <summary>
/// A single tile.
/// </summary>
public class LevelTile : Entity {
    public eTileType tileType = eTileType.GRASS;
        
    // How much space (in grid units) does this tile take up?
    public int width = 1;
    public int height = 1;

    // How Tall can this be (random in world units between these 2)
    public float yOffsetMin = 0.0f;
    public float yOffsetMax = 0.0f;

	// Use this for initialization
	protected override void Start () {
        base.Start();	
	}
	
}
