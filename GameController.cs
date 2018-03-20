using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using UnityEngine.XR.WSA.Input;

public class GameController : MonoBehaviour, ISubscriber
{
    #region PublicVariables
    //NOT SURE I NEED THESE
    //public int RoundsToWin = 3;                 //number of rounds to win -- Play until you lose?
    public float RoundStartDelay = 3f;          //round start delay
    public float RoundEndDelay = 3f;            //round end delay
    public Text RounderNumberText;              //updates player on what round
    //NOT SURE I NEED THESE ^^

    public enum GameStates { Tutorial, RoundSetup, RoundPlay, RoundEnd,
                                GamePause, GameOver}
    public GameStates _GameState;

    public static GameController instance = null;

    #endregion

    #region privateVariables
    //Tutorial
    [SerializeField] Transform snakeSpawnLocation;
    [SerializeField] Transform tutorialFoodSpawnLocatoin;
    [SerializeField] GameObject tutorialInstructions;
    [SerializeField] GameObject instructionsLocation;
    [SerializeField] GameObject tutorialFingerTapIcon;
    [SerializeField] GameObject tutorialCompletionMessage;

    //Menus
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject afterActionReport;

    //Food spawning
    [SerializeField] int amountOfFoodSpawnedAtBeginningOfRound = 5; //first round starts with 5 food spawned

    //snake
    [SerializeField] GameObject snakeGameObject;
    GameObject clone;
    private Snake snakeReference;                        // refernce to Snake script

    //Player
    [SerializeField] PlayerManager playerManager;

    //FoodManager
    [SerializeField] FoodManager foodManager;

    //ConnectManager
    [SerializeField] ConnectManager CM;

    //Pointers and UI
    [SerializeField] SnakeCamera snakeCamera;
    [SerializeField] SnakeCompass snakeCompass;
    [SerializeField] TargetCamera targetCamera;
    [SerializeField] FoodCompass foodCompass;
    [SerializeField] GameObject snakeHunger3DUIPrefab;
    [SerializeField] GameObject score3DUIPrefab;
    [SerializeField] GameObject multiplier3DUIPrefab;
    [SerializeField] GameObject gameOverUIPrefab;
    [SerializeField] Transform gameOverSpawnLocation;
    [SerializeField] GameObject roundEndMessage;
    [SerializeField] GameObject nextRoundMessage;
    [SerializeField] Color multiplierHitColor;
    [SerializeField] Color multiplierHitShadowColor;
    [SerializeField] Color multiplierSnakeColor;
    [SerializeField] Color multiplierSnakeShadowColor;
    GameObject cloneHunger3DUI;
    GameObject cloneScore3DUI;
    GameObject cloneMultipier3DUI;
    GameObject cloneGameOverUI;

    //Audio
    [SerializeField] AudioSource bgMusic;
    [SerializeField] AudioSource menuMusic;
    [SerializeField] AudioSource countdownSFX;

    //Round GamePlay
    private int roundNumber;                    //current round number

    private bool isGameRunning;
    private bool isNextRound = false;
    private bool isRoundOver = false;
    private bool isGameOver = false;
    private bool isTutorialActive = true;
    private float snakeSpeedDifferential = .25f;
    private float snakeSpeedDifferentialAdjusted;
    private float snakeStartSpeed = 3.0f;
    private float snakeStartSpeedAdjusted;
    private int snakeStartHealth = 3;
    private int snakeStartHealthAdjusted;
    private int snakeStartHunger = 10;
    private int snakeStartHungerAdjusted;
    private int amountOfFoodSpawnedAtBeginningOfRoundHolder;
    private int snakeScoreMultiplierHolder;

    private WaitForSeconds startDelay;          //delay for round starting
    private WaitForSeconds endDelay;            //delay for round ending
    private GameStates _gameStateHolder;

    #endregion

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    // Use this for initialization
    void Start ()
    {
        //Setup singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //initialize wait for round to start and to end
        snakeScoreMultiplierHolder = 1;
        isGameRunning = true;
        amountOfFoodSpawnedAtBeginningOfRoundHolder = amountOfFoodSpawnedAtBeginningOfRound;
        roundNumber = 1;
        _GameState = GameStates.Tutorial;
        startDelay = new WaitForSeconds(RoundStartDelay);
        endDelay = new WaitForSeconds(RoundEndDelay);

        //deactivate UI
        tutorialInstructions.SetActive(false);
        tutorialInstructions.transform.position = instructionsLocation.transform.position;
        pauseMenu.SetActive(false);
        roundEndMessage.SetActive(false);
        nextRoundMessage.SetActive(false);
        afterActionReport.SetActive(false);
        //deactivate audio  Not needed, I switched to AudioSources and they are starting deactivated.
        //bgMusic.SetActive(false);
        //countdownSFX.SetActive(false);

        StartCoroutine(GameLoop());//or do I call the Tutorial Game State??
    }//end Start()
    
    void InitializePointers(Transform snakeTransform)
    {
        snakeCamera.Snake = snakeTransform;
        snakeCompass.SnakeToPointAt = snakeTransform;
        targetCamera.Snake = snakeTransform;
        foodCompass.SnakeToGetFoodFrom = snakeTransform;
        cloneHunger3DUI = Instantiate(snakeHunger3DUIPrefab, new Vector3(snakeTransform.position.x, snakeTransform.position.y + .85f, snakeTransform.position.z), Quaternion.identity);
        cloneHunger3DUI.GetComponent<SnakeHungerUI>().TheSnake = snakeTransform;
        //scoreAmountUI.ThePlayer = playerManager;
}

    #region Game State Loop
    private IEnumerator GameLoop() //will run through the each phase one after another---Does it stil though?
    {
        while (isGameRunning)
        {
            switch (_GameState)
            {
                case GameStates.Tutorial:
                    StartCoroutine(TutorialRunning());
                    break;
                case GameStates.GamePause:
                    //Pause Game Function
                    //This needs to be blank because it fires every frame
                    //PauseGame();

                    break;
                case GameStates.RoundSetup:
                    //round setting up function
                    RoundSettingUp();

                    yield return startDelay;

                    if (_GameState == GameStates.RoundSetup)
                    {
                        snakeReference.Resume();
                        Timer.instance.timerStates = Timer.TimerStates.Reset;
                        _GameState = GameStates.RoundPlay;
                    }
                    break;
                case GameStates.RoundPlay:
                    //round playing coroutine
                    StartCoroutine(RoundPlaying());
                    break;
                case GameStates.RoundEnd:
                    //round end courutine
                    RoundEnding();

                    yield return endDelay;
                    
                    if(_GameState == GameStates.RoundEnd)
                        _GameState = GameStates.RoundSetup;
                    break;
                case GameStates.GameOver:
                    //after action bullshit
                    
                    //TODO isGameRunning = false;
                    break;
            }
            yield return null;
        }
    }//end GameLoop()
    #endregion

    #region Game State Coroutines and Functions
    private IEnumerator TutorialRunning()
    {
        Timer.instance.timerStates = Timer.TimerStates.Pause;
        isNextRound = false;
        //Snake location
        if(foodManager.Predators != null)
        {
            if(foodManager.Predators.Length < 1)
            {
                clone = Instantiate(snakeGameObject, snakeSpawnLocation.position, Quaternion.identity);
                snakeReference = clone.GetComponent<Snake>();
                snakeReference.InitializeSnake(snakeScoreMultiplierHolder, snakeStartHealth, snakeStartHunger, 
                    snakeStartSpeed, true, snakeSpeedDifferential, snakeSpawnLocation);//random numbers to satisfy parameters by Ingargiola
                EventManager.CallSnakeSpawn(clone);
                //Tuorial food location
                EventManager.CallTutorialFoodSpawn(tutorialFoodSpawnLocatoin.position);
                //initialize pointers
                InitializePointers(clone.transform);
                snakeReference.Resume();
            }
        }

        //UI instructions
        tutorialInstructions.SetActive(true);
        tutorialInstructions.transform.position = instructionsLocation.transform.position;
        //UI finger tapo icon
        tutorialFingerTapIcon.SetActive(true);
        //hide completion message
        tutorialCompletionMessage.SetActive(false);

        while (isTutorialActive)
        {
            yield return null;
        }

        if(_GameState == GameStates.Tutorial)
            _GameState = GameStates.RoundSetup;
    }//end TutorialRunning()

    public void PauseGame()
    {
        bgMusic.Pause();
        _gameStateHolder = _GameState;
        _GameState = GameStates.GamePause;
        Timer.instance.timerStates = Timer.TimerStates.Pause;
        if(snakeReference.speed > 0)
        {
            snakeReference.Pause();
        }
        playerManager.CanShoot = false;
        pauseMenu.SetActive(true);
    }

    public void UnpauseGame()
    {
        switch (_gameStateHolder)
        {
            case GameStates.Tutorial:

                if (isTutorialActive)
                {
                    snakeReference.Resume();
                    playerManager.CanShoot = true;
                    _GameState = GameStates.Tutorial;
                }
                else
                    _GameState = GameStates.RoundSetup;
                pauseMenu.SetActive(false);
                break;
            case GameStates.RoundSetup:

                snakeReference.Resume();
                Timer.instance.timerStates = Timer.TimerStates.Reset;
                _GameState = GameStates.RoundPlay;
                pauseMenu.SetActive(false);
                break;
            case GameStates.RoundPlay:

                Timer.instance.timerStates = Timer.TimerStates.Play;
                snakeReference.Resume();
                playerManager.CanShoot = true;
                pauseMenu.SetActive(false);
                _GameState = GameStates.RoundPlay;
                pauseMenu.SetActive(false);
                break;
            case GameStates.RoundEnd:

                _GameState = GameStates.RoundSetup;
                pauseMenu.SetActive(false);
                break;
            case GameStates.GamePause:

                Timer.instance.timerStates = Timer.TimerStates.Play;
                snakeReference.Resume();
                playerManager.CanShoot = true;
                pauseMenu.SetActive(false);
                _GameState = GameStates.RoundPlay;
                break;
            case GameStates.GameOver:

                _GameState = _gameStateHolder;
                pauseMenu.SetActive(false);
                break;
            default:
                break;
        }
        bgMusic.UnPause();
    }

    public void RestartGame()
    {
        menuMusic.Stop();
        roundNumber = 0;
        isNextRound = true;
        isGameOver = false;
        amountOfFoodSpawnedAtBeginningOfRound = amountOfFoodSpawnedAtBeginningOfRoundHolder - 2;
        playerManager.PlayerScore = 0;
        Destroy(cloneGameOverUI);
        Destroy(cloneHunger3DUI);
        _GameState = GameStates.RoundSetup;
    }

    private void RoundSettingUp() 
    {
        //get rid of tutorial extras but add a congrats
        
        tutorialInstructions.SetActive(false);
        tutorialInstructions.transform.position = instructionsLocation.transform.position;
        tutorialFingerTapIcon.SetActive(false);
        tutorialCompletionMessage.SetActive(true);
        roundEndMessage.SetActive(false);

        //play the countdown
        countdownSFX.Play();

        //disablesnake
        snakeReference.Pause(); 
        //disable shooting
        playerManager.CanShoot = false;

        //spawn in food
        //increment RoundNumber
        if (foodManager.Predators.Length < 1)
        {
            clone = Instantiate(snakeGameObject, snakeSpawnLocation.position, Quaternion.identity);
            snakeReference = clone.GetComponent<Snake>();

            snakeStartHealthAdjusted = snakeStartHealth + roundNumber;
            snakeStartHungerAdjusted = snakeStartHunger - roundNumber;
            snakeStartSpeedAdjusted = snakeStartSpeed + (roundNumber * snakeSpeedDifferential);
            snakeSpeedDifferentialAdjusted = snakeSpeedDifferential + (.01f * roundNumber);

            snakeReference.InitializeSnake(snakeScoreMultiplierHolder, snakeStartHealthAdjusted, snakeStartHungerAdjusted, 
                snakeStartSpeedAdjusted, false, snakeSpeedDifferentialAdjusted, snakeSpawnLocation);//random numbers to satisfy parameters by Ingargiola
            EventManager.CallSnakeSpawn(clone);

            //initialize pointers
            InitializePointers(clone.transform);
        }
        snakeReference.CanSlither = false;
        foodManager.TemporaryTargetReset();

        if (isNextRound == true)
        {
            roundNumber += 1;
            amountOfFoodSpawnedAtBeginningOfRound += 2;
            Timer.instance.RoundStartSpawns(amountOfFoodSpawnedAtBeginningOfRound);
            tutorialCompletionMessage.SetActive(false);
            nextRoundMessage.SetActive(true);

        }
        else
        {
            Timer.instance.RoundStartSpawns(amountOfFoodSpawnedAtBeginningOfRound);
        }

    }//end RoundStarting()

    private IEnumerator RoundPlaying()
    {
        //get rid of meesages
        tutorialCompletionMessage.SetActive(false);
        nextRoundMessage.SetActive(false);

        //disable countdown object so it can play again before next round starts
        //countdownSFX.

        //enable BG music
        if(!bgMusic.isPlaying)
            bgMusic.Play();

        //enable shooting
        playerManager.CanShoot = true;
        snakeReference.CanSlither = true;
        //enable scoring???


        //check if player did well enough
        if(isGameOver == true)
        {
            _GameState = GameStates.GameOver;
            InputManager.Instance.OverrideFocusedObject = null;
            Timer.instance.timerStates = Timer.TimerStates.Pause;
            snakeReference.Pause();
            snakeReference.KillSnake();
            GameObject[] leftOverSnakeBits;
            leftOverSnakeBits = GameObject.FindGameObjectsWithTag("Predators");
            foreach (GameObject go in leftOverSnakeBits)
            {
                Destroy(go);
            }
            foodManager.ClearAllForRoundReset(true);
            playerManager.CanShoot = false;
            afterActionReport.SetActive(true);
            CM.SendScoreToServerLeaderBoard();
            afterActionReport.transform.Find("PlayerName").GetComponent<TextMesh>().text = CM.playername;// here?
            bgMusic.Stop();
            menuMusic.Play();

        }

        if(isRoundOver == true)
        {
            _GameState = GameStates.RoundEnd;
        }
        

        yield return null;
    }//end RoundStarting()

    private void RoundEnding()
    {
        GameObject[] leftOverSnakeBits;
        leftOverSnakeBits = GameObject.FindGameObjectsWithTag("Predators");
        if(leftOverSnakeBits != null)
        {
            foreach (GameObject go in leftOverSnakeBits)
            {
                Destroy(go);
            }
        }
        
        foodManager.ClearAllForRoundReset(true);
        foodManager.TemporaryTargetReset();
        Timer.instance.timerStates = Timer.TimerStates.Pause;
        //TODO:: Round ending indicator
        roundEndMessage.SetActive(true);
        //TODO:: Round score indicator

        isNextRound = true;
        isRoundOver = false;

        Destroy(cloneHunger3DUI);

        //disable shooting
        playerManager.CanShoot = false;
        
        //wait to change to RoundSetup
    }//end RoundEnding();

    private void EndTutorial(bool NA)
    {
        isTutorialActive = false;
    }

    private void RestartGame(bool shouldIncludeTutorial)
    {
        if (shouldIncludeTutorial)
        {
            _GameState = GameStates.Tutorial;
        }
        else
        {
            _GameState = GameStates.RoundSetup;
        }
            
        isGameRunning = true;
        StartCoroutine(GameLoop());
    }

    private void EndGame(bool NA)
    {
        isGameOver = true;
        cloneGameOverUI = Instantiate(gameOverUIPrefab, gameOverSpawnLocation.position, Quaternion.identity);
    }

    private void EndRound(bool NA)
    {
        isRoundOver = true;
    }

    private void DisplayScore(Vector3 foodPosition)
    {
        cloneScore3DUI = Instantiate(score3DUIPrefab, new Vector3(foodPosition.x, foodPosition.y + .8f, foodPosition.z), Quaternion.identity);
        cloneScore3DUI.GetComponent<ScoreAmountUI>().ThePlayer = playerManager;
    }

    private void DisplayMultiplier(int multiplierAmount, Vector3 foodPosition)
    {
        cloneMultipier3DUI = Instantiate(multiplier3DUIPrefab, new Vector3(foodPosition.x + 1.566f, foodPosition.y + .6f, foodPosition.z), Quaternion.identity);
        if (multiplierAmount == 0)
        {
            if(playerManager.ConsecutiveHitMultiplier - 1 > 1)
            {
                cloneMultipier3DUI.GetComponent<MultiplierAmountUI>().GetMultiplier(playerManager.ConsecutiveHitMultiplier - 1);
                cloneMultipier3DUI.GetComponent<MultiplierAmountUI>().MultiplierTextColor = multiplierHitColor;
                cloneMultipier3DUI.GetComponent<MultiplierAmountUI>().MultiplierShadowColor = multiplierHitShadowColor;
            }
        }
        else
        {
            if (multiplierAmount > 1)
            {
                cloneMultipier3DUI.GetComponent<MultiplierAmountUI>().GetMultiplier(multiplierAmount);
                cloneMultipier3DUI.GetComponent<MultiplierAmountUI>().MultiplierTextColor = multiplierSnakeColor;
                cloneMultipier3DUI.GetComponent<MultiplierAmountUI>().MultiplierShadowColor = multiplierSnakeShadowColor;
            }
        }
    }

    private void GetSnakeScoreMultiplier(int snakeScoreMultiplier)
    {
        snakeScoreMultiplierHolder = snakeScoreMultiplier;
    }

    private void InteractionManager_SourceLost(InteractionSourceLostEventArgs hand)
    {
        if (!isTutorialActive)
            tutorialFingerTapIcon.SetActive(true);
    }

    private void InteractionManager_SourceDetected(InteractionSourceDetectedEventArgs hand)
    {
        if (!isTutorialActive)
            tutorialFingerTapIcon.SetActive(false);
    }

    public void Subscribe()
    {
        EventManager.OnTutorialEnd += EndTutorial;
        EventManager.OnGameOver += EndGame;
        EventManager.OnGameRestart += RestartGame;
        EventManager.OnRoundOver += EndRound;
        EventManager.OnDisplayScore += DisplayScore;
        EventManager.OnSnakeScoreMultiplier += GetSnakeScoreMultiplier;
        EventManager.OnDisplayMultiplier += DisplayMultiplier;
        InteractionManager.InteractionSourceLost += InteractionManager_SourceLost;
        InteractionManager.InteractionSourceDetected += InteractionManager_SourceDetected;
    }

    public void Unsubscribe()
    {
        EventManager.OnTutorialEnd -= EndTutorial;
        EventManager.OnGameOver -= EndGame;
        EventManager.OnGameRestart -= RestartGame;
        EventManager.OnRoundOver -= EndRound;
        EventManager.OnDisplayScore -= DisplayScore;
        EventManager.OnSnakeScoreMultiplier -= GetSnakeScoreMultiplier;
        EventManager.OnDisplayMultiplier -= DisplayMultiplier;
        InteractionManager.InteractionSourceLost -= InteractionManager_SourceLost;
        InteractionManager.InteractionSourceDetected -= InteractionManager_SourceDetected;
    }
    #endregion

    //might need to add in a way to get score here or get the score?

}//end GameController Class