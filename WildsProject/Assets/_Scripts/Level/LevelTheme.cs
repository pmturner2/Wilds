using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelTheme : Entity {

    /// <summary>
    /// All tiles that will be in this this theme. 
    /// Assign tiles here.
    /// </summary>
    public LevelTile[] allTiles;

    /// <summary>
    /// Parsed set of tiles, organized by type for easy access.
    /// </summary>
    public Dictionary<eTileType, List<LevelTile>> typedTiles;

    // -----------------------------------------------------------------------

    void Awake()
    {
        fillDictionary();
    }

    // -----------------------------------------------------------------------

    public void fillDictionary()
    {
        typedTiles = new Dictionary<eTileType, List<LevelTile>>();

        // Sort all tiles into their proper typed buckets
        for (int i = 0; i < allTiles.Length; i++)
        {
            // Subarray does not exist, so allocate
            if (!typedTiles.ContainsKey(allTiles[i].tileType))
            {
                typedTiles[allTiles[i].tileType] = new List<LevelTile>();
            }

            // Add tile to the typed list
            typedTiles[allTiles[i].tileType].Add(allTiles[i]);
        }
    }

    // -----------------------------------------------------------------------

	// Use this for initialization
	protected override void Start () {
        base.Start();

       
	}

    // -----------------------------------------------------------------------

    public LevelTile GetRandomTile(eTileType tileType)
    {
        if (typedTiles.ContainsKey(tileType))
        {
            int range = typedTiles[tileType].Count;

            return typedTiles[tileType][Random.Range(0, range)] ;
        }

        return null;
    }

    // -----------------------------------------------------------------------

    public LevelTile GetRandomTile()
    {
        int range = allTiles.Length;

        return allTiles[Random.Range(0, range)];
    }

    // -----------------------------------------------------------------------

    public LevelTile GetRandomTile(eTileType tileType, int dimension)
    {
        /* NOT YET IMPLEMENTED. Get a specific Size.
         * 
        if (typedTiles.ContainsKey(tileType))
        {
            int range = typedTiles[tileType].Count;

            return typedTiles[tileType][Random.Range(0, range)];
        }
        */
        return null;
    }
 
}
