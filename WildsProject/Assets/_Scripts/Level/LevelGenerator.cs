#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
/// <summary>
/// TODO:
///  - Laying out many rooms in "random" directions and look good.
///  - Optimize tilePercentages so we can sort by type
///  - Prefab Setpieces that make the room pretty
///  - Props
///  - Enemy Spawners
///  - Treasure
///  - Paths, Rivers
///  - "Active" Tiles to show energy coursing through the world (actually important, I think)
/// </summary>
public class LevelGenerator : Entity {
    public bool generateOnStart;
    // Contains all of the prefabs, etc for the level style
    public LevelTheme levelTheme;
   
    // Initial data.
    // TODO: add another set, sorted by type
    public TilePercentageData[] tilePercentages;
    
    // How hard do we try to match our neighbors type?
    // 0 = do not consider neighbors
    // 1 = heavily consider
    [Range(0.0f, 1.0f)]
    public float similarityWeight = 0.0f;

    public LevelRoom[] rooms;

    public float shadowHeightThreshold = 0.5f;

    // -----------------------------------------------------------------------

    protected int numRooms = 0;

    // Conversion for 1 tile unit to world space
    protected const float tileSize = 4;
    
    // -----------------------------------------------------------------------
   
	// Use this for initialization
	protected override void Start () {
        base.Start();

        if (generateOnStart)
            GenerateLevel(false);
	}

    // -----------------------------------------------------------------------

    public void GenerateLevel(bool inEditor)
    {
        Vector3 roomStart = Vector3.zero;

        numRooms = rooms.Length;
        // Init empty parent object
        GameObject room = new GameObject("room");

        for (int i = 0; i < numRooms; i++)
        {
            // Allocate
            rooms[i].Init();

            // Specific tile Percentages tell us how to weight our next tile type choice
            // Start with the defaults
            TilePercentageData[] specificTilePercentages = new TilePercentageData[tilePercentages.Length];
            for (int j = 0; j < specificTilePercentages.Length; j++)
            {
                specificTilePercentages[j] = new TilePercentageData();
                specificTilePercentages[j].tileType = tilePercentages[j].tileType;
                specificTilePercentages[j].percentage = tilePercentages[j].percentage;
            }

            // Prefab we will instantiate for this tile. Reused per iteration.
            LevelTile tPrefab = null;
            int tileHeight = 1;

            // For Each Tile in the room
            for (int j = 0; j < rooms[i].m_dimensions.minWidth; j++)
            {
                tileHeight = 1;
                for (int k = 0; k < rooms[i].m_dimensions.minHeight; k += tileHeight)
                {
                    tileHeight = 1;
                    if (rooms[i].IsTileEmpty(j, k))
                    {
                        // Clear out the percentages for the next tile
                        for (int specificTileIndex = 0; specificTileIndex < specificTilePercentages.Length; specificTileIndex++)
                        {
                            specificTilePercentages[specificTileIndex].percentage = tilePercentages[specificTileIndex].percentage;
                        }

                        // Random chunks removed from edges
                        int exists = Random.Range(0, 2);

                        if (k != 0 && j != 0 && j != rooms[i].m_dimensions.minWidth && k != rooms[i].m_dimensions.minHeight)
                        {
                            exists = 1;
                        }

                        // If we don't remove a random chunk.
                        if (exists == 1)
                        {

                            eTileType tileType = eTileType.GRASS;

                            // Weight our tile type choice by the neighbors
                            rooms[i].CalculateNeighborPercentages(j, k, ref specificTilePercentages, this.similarityWeight);
                            float totalPercent = 0.0f;

                            // Calculate our total weight
                            for (int percentages = 0; percentages < specificTilePercentages.Length; percentages++)
                            {
                                totalPercent += specificTilePercentages[percentages].percentage;
                            }

                            float randomValue = Random.Range(0.0f, totalPercent);
                            float randomThreshold = 0.0f;

                            // Find an appropriate tile type choice, given our weights
                            for (int randomChoice = 0; randomChoice < specificTilePercentages.Length; randomChoice++)
                            {
                                randomThreshold += specificTilePercentages[randomChoice].percentage;
                                if (randomValue <= randomThreshold)
                                {
                                    tileType = specificTilePercentages[randomChoice].tileType;
                                    break;
                                }
                            }

                            // TEMP: Can Remove after testing. Preventing Infinite Loop
                            int count = 0;
                            // Now that we know the type, Loop until we have a good tile size
                            do
                            {
                                if (inEditor)
                                    levelTheme.fillDictionary();

                                tPrefab = levelTheme.GetRandomTile(tileType);
                                // For testing. 100 is arbitrary "big" number
                                if (++count > 100)
                                {
                                    Debug.LogWarning("UNABLE TO PLACE TILE: " + j + " " + k);
                                    continue;
                                }
                                // Continue trying tile types until we find one that fits.
                            } while (!rooms[i].TestAddTile(tPrefab, j, k));

                            // Randomize the height of the terrain slightly
                            float yRandom = Random.Range(tPrefab.yOffsetMin, tPrefab.yOffsetMax);

                            LevelTile t = null;

                            if (inEditor)
                            {
                                #if UNITY_EDITOR
                                t = inEditorInstantiate(tPrefab, roomStart + new Vector3(j * tileSize, yRandom, k * tileSize), Quaternion.identity) as LevelTile;
                                #endif
                            }
                            else
                            {
                                t = Instantiate(tPrefab, roomStart + new Vector3(j * tileSize, yRandom, k * tileSize), Quaternion.identity) as LevelTile;
                            }
               
                            rooms[i].AddTile(t, j, k);
                            // Parent it to the whole room
                            t.transform.parent = room.transform;

                            // If terrain is high enough, it should cast shadows. By default, tiles do not cast, only receive shadows.
                            if (yRandom > shadowHeightThreshold)
                            {
                                t.renderer.castShadows = true;
                            }

                            // Used for loop variable
                            tileHeight = t.height;

                        }
                    }
                }
            }

            // Prepare for next room    
            roomStart += Vector3.right * tileSize * rooms[i].m_dimensions.minWidth;// +Vector3.right * Random.Range(0, 10);

        }
    }

    // -----------------------------------------------------------------------
    #if UNITY_EDITOR
    Object inEditorInstantiate(Object original, Vector3 position, Quaternion rotation)
    {
        LevelTile temp = (LevelTile) PrefabUtility.InstantiatePrefab(original);

        temp.transform.position = position;
        temp.transform.rotation = rotation;

        return temp;
    }
    #endif
    // -----------------------------------------------------------------------
    // Update is called once per frame
    void Update()
    {
	
	}
}
