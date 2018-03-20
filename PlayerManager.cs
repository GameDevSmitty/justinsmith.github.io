using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class PlayerManager : MonoBehaviour, ISubscriber, IInputClickHandler, IHoldHandler
{
    [SerializeField] private float missedShotDelay = .5f;
    [SerializeField] private int foodLayer;
    [SerializeField] private int targetFoodLayer;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject pauseMenuGameObject;
    [SerializeField] private AudioClip missedBulletClip;
    [SerializeField] private AudioClip plasmaShotClip;
    [SerializeField] GameObject missTextPrefab;
    [SerializeField] private AudioSource audioSourceMiss;
    [SerializeField] private AudioSource audioSourceShoot;
    [SerializeField] private AimingReticle aimingReticle;
    private int playerScore = 0;
    public int PlayerScore
    {
        get { return playerScore; }
        set { playerScore = value; }
    }
    private bool canShoot = true;
    public bool CanShoot { get { return canShoot; } set { canShoot = value; } }
    private int consecutiveHitMultiplier;
    public int ConsecutiveHitMultiplier { get { return consecutiveHitMultiplier; } set { consecutiveHitMultiplier = value; } }
    private bool isGunJammed;
    private bool isJamPlayersGunRunning;
    private bool hasMissed;
    private int layerMaskForFood;
    private int layerMaskForTarget;
    private int finalLayerMask;

    private WaitForSeconds delay;
    private RaycastHit hit;

    private void OnEnable()
    {
        Subscribe();
    }

    void Awake()
    {
        InputManager.Instance.OverrideFocusedObject = this.gameObject;
    }

    void Start ()
    {
        delay = new WaitForSeconds(missedShotDelay);
        audioSourceShoot.clip = plasmaShotClip;
        audioSourceMiss.clip = missedBulletClip;
        layerMaskForFood = 1 << foodLayer;
        layerMaskForTarget = 1 << targetFoodLayer;
        finalLayerMask = layerMaskForFood | layerMaskForTarget;
        consecutiveHitMultiplier = 1;
    }
	
	void Update ()
    {
        RaycastForTarget();

#if UNITY_EDITOR
        HandleInput();
#endif
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    void RaycastForTarget()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, finalLayerMask))
        {
            Debug.Log("Aiming at Target");
            aimingReticle.HasTarget = true;
            aimingReticle.ChangeReticlePositionAndScale(hit.point);
        }
        else
        {
            aimingReticle.HasTarget = false;
        }
    }
    
    void HandleInput()
    {
        if (canShoot)
        {
            if(!isGunJammed)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    Instantiate(bulletPrefab, transform.position, transform.rotation);
                    audioSourceShoot.Play();
                    StartCoroutine(JamPlayersGun());
                }
            }
        }
    }

    void MissedShot(GameObject bulletThatMissed, bool hasHitFood)
    {
        if(!hasHitFood)
        {
            audioSourceMiss.Play();

            Instantiate(missTextPrefab, bulletThatMissed.transform.position, Quaternion.identity);

            consecutiveHitMultiplier = 1;
        }
    }

    IEnumerator JamPlayersGun()
    {
        isJamPlayersGunRunning = true;
        isGunJammed = true;

        yield return delay;

        isGunJammed = false;
        isJamPlayersGunRunning = false;
    }

    public void ResetPlayer()
    {

        playerScore = 0;
        consecutiveHitMultiplier = 1;

    }

    private void HandlePlayerScore(int i)
    {
        playerScore += i;
    }

    private void HandleBulletHitScore(int i)
    {
        playerScore += (i * consecutiveHitMultiplier);
        consecutiveHitMultiplier++;
    }

    public void Subscribe()
    {
        EventManager.OnBulletMissed += MissedShot;
        EventManager.OnPlayerScored += HandlePlayerScore;
        EventManager.OnBulletHitScore += HandleBulletHitScore;
    }

    public void Unsubscribe()
    {
        EventManager.OnBulletMissed -= MissedShot;
        EventManager.OnPlayerScored -= HandlePlayerScore;
        EventManager.OnBulletHitScore -= HandleBulletHitScore;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (canShoot)
        {
            if (!isGunJammed)
            {
                Instantiate(bulletPrefab, transform.position, transform.rotation);
                audioSourceShoot.Play();
                StartCoroutine(JamPlayersGun());
            }
        }
    }

    public void OnHoldStarted(HoldEventData eventData)
    {
        //TODO:: Create Coroutine so that player has to hold longer before the menu pops up... If needed
        if(GameController.instance._GameState != GameController.GameStates.GamePause)
        {
            GameController.instance.PauseGame();
            InputManager.Instance.OverrideFocusedObject = null;
        }
    }

    public void OnHoldCompleted(HoldEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnHoldCanceled(HoldEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
