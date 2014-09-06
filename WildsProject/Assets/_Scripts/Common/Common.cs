using UnityEngine;
using System.Collections;

public enum eAbilitySlot
{
    SLOT1, // For player, Left Click
    SLOT2  // For Player, Right Click
};

// -----------------------------------------------------------------------
/// <summary>
/// Pooled items implement this
/// </summary>
public interface IPoolable
{
    void PoolReset();
    void PoolStart();
};

// -----------------------------------------------------------------------

public enum eTileType
{
    GRASS, DIRT, WHITE, WATER
};

// -----------------------------------------------------------------------

// Percentage of each tile in randomization
[System.Serializable]
public class TilePercentageData
{
    public eTileType tileType = eTileType.DIRT;
    public float percentage;
}

/// <summary>
/// All states. This is from TOL and has way too many.
/// </summary>
public enum eStateName
{
    PLAYER_IDLE, PLAYER_WALK, PLAYER_JUMP, PLAYER_WALLJUMP, PLAYER_FALLING, PLAYER_HOVER, PLAYER_GROUNDPOUND, PLAYER_WALLCLING, PLAYER_NETWORK,
    PLAYER_LEDGEGRAB, PLAYER_TEETER, PLAYER_WALLRUN, PLAYER_FALLFROMWALL, PLAYER_DEAD, PLAYER_PHYSICS, PLAYER_FREEZE,

    ENEMY_IDLE = 50, ENEMY_ATTACK, ENEMY_DEAD, ENEMY_DYING, ENEMY_FLEE, ENEMY_PATROL, ENEMY_PURSUIT, ENEMY_SEARCH, ENEMY_POOLING, ENEMY_STUNNED, ENEMY_TIMEDATTACK,

    // Menus are implemented as States
    MENU_MAIN = 100, MENU_PAUSE, MENU_NONE, MENU_OPTIONS, MENU_BACK, MENU_QUIT, MENU_ERROR, MENU_PRE_MAIN, MENU_GAMETYPE, MENU_NEWGAME,

    MENU_INGAME_OPTIONS = 200, MENU_INGAME_BITBATTLE, MENU_MULTIPLAYER, MENU_CREATEGAME,  MENU_LOADING,


};

// -----------------------------------------------------------------------

    public enum eItemSlot
    {
        R_HAND, R_FOOT, L_HAND, L_FOOT, BACK, HEAD
    };

    // -----------------------------------------------------------------------

    public enum eItemType
    {
        NONE, POWERUP, WEAPON
    };

    // -----------------------------------------------------------------------

    public enum eMessageType
    {
        NONE, INPUT, RETURN_TO_POOL, TAKE_FROM_POOL, DELIVER_FROM_POOL,

        MENU_TRANSITION = 1000,


        BIT_BATTLE_START = 10000, BIT_BATTLE_XOR, BIT_BATTLE_AND, BIT_BATTLE_OR
    };

    // -----------------------------------------------------------------------

    public enum eButtonState { INACTIVE, DOWN, HOLD, UP};

    // -----------------------------------------------------------------------

    public enum eTransitionType { IN, OUT };

    // -----------------------------------------------------------------------
    // For localization
    // Should map these to string in external files
    public enum eStringName
    {
        OPTIONS, CONTINUE, QUIT, DONE, ARE_YOU_SURE_QUIT, RESUME, CREATE, MAX_PLAYERS, LEVEL, VOICE_CHAT, FRIENDS_ONLY, GAME_NAME,
        PSN_ID, NONE,

        MAGE_INFO = 1000, HEAVY_INFO
    };

    // -----------------------------------------------------------------------

    public struct Message
    {
        public eMessageType mType;
        public object data;
    }

    // -----------------------------------------------------------------------

    public struct InputData
    {
        public Vector2 leftAnalog;
        public Vector2 rightAnalog;
        public eButtonState jump;
        public eButtonState attack;
        public eButtonState leftClick;
        public bool shiftClick;
        public Vector3 screenSpaceClick;
        public Vector3 worldDestination;
    }

    // -----------------------------------------------------------------------

    public struct PoolRequestData
    {
        public Entity item;
        public MessageEntity requester;
    }

    // -----------------------------------------------------------------------

////}
