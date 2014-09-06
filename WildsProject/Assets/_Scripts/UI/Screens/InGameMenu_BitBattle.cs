using UnityEngine;
using System.Collections;

public class InGameMenu_BitBattle : MenuScreen {

    // --- Assigned in editor
   
    // How many bits to start with
    public int initialBitSize = 2;

    // How much time on clock when the level begins 
    public float initialTimer = 30f;
    
    // How much time does the clock lose when the player makes a fail move
    public float timePenaltyFail = 5f;

    // How much time is added when the player makes a success move
    public float timeAddedSuccess = 5f;

    public BitSet m_enemyBits;    
    public BitSet m_playerBits;
    public TextBox m_targetNumberDisplay;
    public TextBox m_nextTargetNumberDisplay;
    public TextBox m_scoreNumberDisplay;
    public TextBox m_bestScoreNumberDisplay;

    public ParticleSystem m_successParticles;
    public ParticleSystem m_failParticles;


    public GameObject m_targetBox;

    // ---
    // -----------------------------------------------------------------------
    protected int m_score = 0;
    protected int m_bestScore = 0;

    protected int m_targetNumber = 0;
    protected int m_nextTargetNumber = 0;

    protected UITimer m_timer;

    protected UIButton m_startButton;

    // Parent object that holds player bits and buttons for XOR, OR, AND
    // This is de-activated when game is not in progress
    protected GameObject m_actionDisplay;
    
    // Current state of the game. CPU_TURN not yet implemented
    protected enum eBattleState { STOPPED, CPU_TURN, PLAYER_TURN };
    protected eBattleState m_currentState = eBattleState.STOPPED;

    protected enum eMove { XOR, AND, OR }

    // Difficulty interval is the score threshold between increasing the size of the bitfield
    protected int m_difficultyInterval = 1;
    protected int m_nextDifficultyIncrease = 0;

    // -----------------------------------------------------------------------
    /// <summary>
    /// Turns on Action bars and Player Bits
    /// </summary>
    protected void EnableActions()
    {
        m_actionDisplay.SetActive(true);
        m_playerBits.gameObject.SetActive(true);

        Animation[] anims = m_actionDisplay.GetComponentsInChildren<Animation>();
        foreach (Animation a in anims)
        {
            a.Rewind();
        }
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Turns off action Buttons and Player bits
    /// </summary>
    protected void DisableActions()
    {        
        m_actionDisplay.SetActive(false);
        m_playerBits.gameObject.SetActive(false);
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Sets score equal to amount, updates display, and then checks if we have reached a threshold for difficulty.
    /// If we have, we expand the number of bits and calculate a new difficulty threshold
    /// </summary>
    /// <param name="amount"></param>
    protected void SetScore(int amount)
    {
        m_score = amount;
        if (m_score > m_bestScore)
        {
            m_bestScore = m_score;
            m_bestScoreNumberDisplay.SetText(m_bestScore.ToString());
        }
        m_scoreNumberDisplay.SetText(m_score.ToString());

        // check difficulty threshold
        if (m_score > m_nextDifficultyIncrease)
        {
            // We are scaling up the difficulty gap by our current progress.
            // The longer we play, the longer the gap between growing bits.
            
            // TODO: Try option where instead of m_playerBits.MaxValue we use m_playerBits.NumBits
            m_nextDifficultyIncrease += m_difficultyInterval * m_playerBits.MaxValue;

            m_playerBits.IncreaseSize(1);
            m_enemyBits.IncreaseSize(1);
        }
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Increase score by amount
    /// </summary>
    /// <param name="amount">Amount to increase score</param>
    protected void AddScore(int amount)
    {
        SetScore(m_score + amount);

    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Look at the bits on the board, and assess if their move is equal to the target.
    /// </summary>
    /// <param name="move">Operation: OR, XOR, AND</param>
    /// <returns></returns>
    protected bool AssessTarget(eMove move)
    {
        // get value of enemy and player
        byte enemy = (byte)m_enemyBits.GetByteValue();
        byte player = (byte)m_playerBits.GetByteValue();

        int value = 0;
        switch (move)
        {
            case eMove.AND:
                value = enemy & player;
                break;
            case eMove.OR:
                value = enemy | player;
                break;
            case eMove.XOR:
                value = enemy ^ player;
                break;
        }
        
        Debug.Log("Enemy: " + enemy + " Player: " + player + " XOR: " + value);

        if (value == m_targetNumber)
        {
            Debug.Log("Success");
            return true;
        }
        // Incorrect, failure
        Debug.Log("Incorrect: " + value + " is not " + m_targetNumber);
        return false;
    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// After player makes a move, start next number
    /// </summary>
    protected void NextTarget()
    {
        // Set player's bits as the enemy bits
        m_enemyBits.SetByteValue(m_playerBits.GetByteValue());
        // Reset player bits to 0
        m_playerBits.SetByteValue(0);

        // Bring down "Next" Target
        m_targetNumber = m_nextTargetNumber;
        // Randomize New "Next"
        m_nextTargetNumber  = Random.Range(0, m_enemyBits.MaxValue );
        m_targetNumberDisplay.SetText(m_targetNumber.ToString());
        m_nextTargetNumberDisplay.SetText(m_nextTargetNumber.ToString());

        // Play Spinning Animation on the Target
        m_targetBox.animation.Play();

    }

    // -----------------------------------------------------------------------
    /// <summary>
    /// Begin a new Round. (Pressed Start Button)
    /// Reset all values and setup board.
    /// </summary>
    protected void StartRound()
    {
        SetScore(0);
        
        m_playerBits.SetNumBits(initialBitSize);
        m_enemyBits.SetNumBits(initialBitSize);
        m_nextDifficultyIncrease = m_difficultyInterval;

        m_timer.BeginCountdown(initialTimer);

        m_enemyBits.RandomizeBits();
        m_startButton.gameObject.SetActive(false);
        m_currentState = eBattleState.PLAYER_TURN;
        EnableActions();

        // Initialize first target and second target
        m_targetNumber = Random.Range(0, m_enemyBits.MaxValue ); 
        m_nextTargetNumber = Random.Range(0, m_enemyBits.MaxValue );
        // Display the text
        m_targetNumberDisplay.SetText(m_targetNumber.ToString());
        m_nextTargetNumberDisplay.SetText(m_nextTargetNumber.ToString());

    }

    // -----------------------------------------------------------------------

    public override bool OnMessage(Message message, Agent entity)
    {
        switch (message.mType)
        {
            case eMessageType.BIT_BATTLE_START:
                StartRound();
                return true;

            case eMessageType.BIT_BATTLE_XOR:
                // If player pressed XOR, check for success and continue on to the next number
                if (AssessTarget(eMove.XOR))
                {
                    AddScore(1);
                    m_successParticles.Play();
                    m_timer.AddTime(timeAddedSuccess);
                }
                else
                {
                    m_timer.AddTime(-timePenaltyFail);
                    m_failParticles.Play(); 
                }
                NextTarget();
              return true;

            case eMessageType.BIT_BATTLE_AND:
                // If player pressed AND, check for success and continue on to the next number
                if (AssessTarget(eMove.AND))
                {
                    AddScore(1);
                    m_successParticles.Play();
                    m_timer.AddTime(timeAddedSuccess);
                }
                else
                {
                    m_timer.AddTime(-timePenaltyFail);
                    m_failParticles.Play(); 
                }
                NextTarget();
             return true;

            case eMessageType.BIT_BATTLE_OR:
                // If player pressed OR, check for success and continue on to the next number
                if (AssessTarget(eMove.OR))
                {
                    AddScore(1);
                    m_successParticles.Play();
                    m_timer.AddTime(timeAddedSuccess);
                }
                else
                {
                    m_timer.AddTime(-timePenaltyFail);
                    m_failParticles.Play(); 
                }
                NextTarget();
                return true;

            default:
                return false;
        }
    }


    // -----------------------------------------------------------------------

    public override void Enter(Agent entity)
    {
        if (transitionIn)
        {
            transitionIn.Begin();
        }

        // Initialize Board.
        m_playerBits.SetNumBits(initialBitSize);
        m_enemyBits.SetNumBits(initialBitSize);
        m_nextDifficultyIncrease = m_difficultyInterval;
        // Hide Player's Bits and Buttons
        DisableActions();
    }

    // -----------------------------------------------------------------------

    // Update is called once per frame
    public override void Execute(Agent entity)
    {
        switch (m_currentState)
        {
            // If game is running
            case eBattleState.PLAYER_TURN:
                if (m_timer.IsStopped())
                {
                    // If timer has run down, end Round.
                    // TODO: Add Some Bling, show Score, etc.
                    // Process High Score name

                    m_currentState = eBattleState.STOPPED;
                    m_startButton.gameObject.SetActive(true);
                    DisableActions();
                }

                break;
        }
    }

    // -----------------------------------------------------------------------

    protected override void Start()
    {
        base.Start();
        m_timer = GetComponentInChildren<UITimer>();
        m_startButton = transform.FindChild("Button_Start").GetComponent<UIButton>();
        m_actionDisplay = transform.FindChild("ActionDisplay").gameObject;
        
    }
}
