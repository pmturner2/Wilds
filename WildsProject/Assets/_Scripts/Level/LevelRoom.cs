using UnityEngine;
using System.Collections;

// -----------------------------------------------------------------------
/// <summary>
/// DATA Class for a single room.
/// Corridors will count as rooms.
/// </summary>
[System.Serializable]
public class LevelRoom  {

    [System.Serializable]
    public class RoomDimensions
    {
        // In # of Tiles
        public int minWidth = 3;
        public int maxWidth = 5;

        public int minHeight = 10;
        public int maxHeight = 20;
    }
    
    public RoomDimensions m_dimensions = new RoomDimensions();

    protected int m_width = 0;
    public int Width
    {
        get { return m_width; }
        set { m_width = value; }
    }

    protected int m_height = 0;
    public int Height
    {
        get { return m_height; }
        set { m_height = value; }
    }
 
    public LevelTile[][] tiles;

    /// <summary>
    /// Allocate
    /// </summary>
    public void Init()
    {
        m_width = Random.Range(m_dimensions.minWidth, m_dimensions.maxWidth);
        m_height = Random.Range(m_dimensions.minHeight, m_dimensions.maxHeight);
        // Pointers to this room.
        tiles = new LevelTile[m_height][];

        for (int i = 0; i < m_height; i++)
        {
            tiles[i] = new LevelTile[m_width];
        }
    }
    
    // -----------------------------------------------------------------------
    /// <summary>
    /// Tries to add specific tile to the room. If unable to add, return false
    /// </summary>
    /// <param name="tile">Tile to add</param>
    /// <param name="x">LEFT MOST x position of tile</param>
    /// <param name="y">TOP MOST y position of tile</param>
    /// <returns>true if successful. false if out of bounds, or tile already occupied</returns>
    public bool AddTile(LevelTile tile, int x, int y, bool overwrite = false)
    {
        if (TestAddTile(tile, x, y, overwrite))
        {
            for (int i = y; i < y + tile.height; i++)
            {
                for (int j = x; j < x + tile.width; j++)
                {
                    tiles[i][j] = tile;
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Tries to add specific tile to the room. If unable to add, return false
    /// </summary>
    /// <param name="tile">Tile to add</param>
    /// <param name="x">LEFT MOST x position of tile</param>
    /// <param name="y">TOP MOST y position of tile</param>
    /// <returns>true if successful. false if out of bounds, or tile already occupied</returns>
    public bool TestAddTile(LevelTile tile, int x, int y, bool overwrite = false)
    {
        for (int i = y; i < y + tile.height; i++)
        {
            for (int j = x; j < x + tile.width; j++)
            {
                if (j < 0 || j >= m_width || i < 0 || i >= m_height)
                {
                    // Out of bounds
                    return false;
                }

                if (tiles[i][j] != null)
                {
                    // Tile already occupied
                    if (!overwrite)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Is the space occupied already?
    /// </summary>
    /// <returns>true if already occupied. false if empty</returns>
    public bool IsTileEmpty(int x, int y)
    {
        return tiles[y][x] == null;
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Update the tilePercentages based on neighboring tiles.
    /// TODO: Could do different distributions. This is essentially a box filter that weights all direct neighbors evenly.
    /// We could do a gaussian and take multiple neighbors into account, or only filter on 1 direction (x or y).
    /// </summary>
    /// <param name="x">X of tile</param>
    /// <param name="y">Y of tile</param>
    /// <param name="tilePercentages">Pre-Allocated array of percentages</param>
    /// <param name="percentBump">How much weight to give each neighbor</param>
    public void CalculateNeighborPercentages(int x, int y, ref TilePercentageData[] tilePercentages, float percentBump = 0.1f)
    {      
        for (int i = x - 1;  i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (j < 0 || j >= m_height || i < 0 || i >= m_width)
                {
                    // Out of bounds                    
                }
                else if (tiles[j][i] != null)
                {
                    for (int percentIndex = 0; percentIndex < tilePercentages.Length; percentIndex++)
                    {
                        if (tilePercentages[percentIndex].tileType == tiles[j][i].tileType)
                        {
                            tilePercentages[percentIndex].percentage += percentBump;
                        }
                    }
                }
            }
        }
    }
}

// -----------------------------------------------------------------------
