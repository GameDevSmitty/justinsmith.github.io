using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerGunControls : MonoBehaviour
{

    public int PlayerNumber;

    string FireInput;
    string ReloadInput;
    string HammerAxisInput;
    bool isCocked;
    bool wasFired = true;
    bool isShootingCoroutineRunning;
    bool isReloadCoroutineRunning;

    public int Ammo = 6;

    [SerializeField]
    Image[] bulletImages;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    CrossHairController playerAimInformation;

    [SerializeField]
    AudioClip shootingSound;
    [SerializeField]
    AudioSource audio;
    [SerializeField]
    Animator animTest;

    float hammerReset = 0;
    // Use this for initialization
    void Start()
    {
        FireInput = "Fire" + PlayerNumber;
        ReloadInput = "Reload" + PlayerNumber;
        HammerAxisInput = "Vertical" + PlayerNumber;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis(HammerAxisInput) != 0 && hammerReset == 0)
        {
            hammerReset = 1;
            if (!isCocked && wasFired)
            {
                isCocked = true;
                wasFired = false;
            }
        }
        else if (Input.GetAxis(HammerAxisInput) == 0)
        {
            hammerReset = 0;
            isCocked = false;
        }

        if (gameManager.GameState == "Shooting")
        {
            if (Input.GetAxis(FireInput) != 0 && Ammo > 0)
            {
                if (!isCocked && !wasFired)
                {
                    if (!isShootingCoroutineRunning)
                        StartCoroutine(ShotDelayIfNotCocked());
                }
                else if (hammerReset == 1 && !wasFired)
                {
                    audio.clip = shootingSound;
                    audio.Play();
                    Debug.Log("Fire Gun");
                    //gunAnimator.SetBool("isIdle", false);
                    //gunAnimator.SetBool("isFiring", true);
                    animTest.SetBool("isIdle", false);
                    animTest.SetBool("isFiring", true);
                    StartCoroutine(ResetFireAnimation());
                    if (playerAimInformation.playerInCrossHairs != null)
                        playerAimInformation.playerInCrossHairs.GetComponentInChildren<PlayerHealth>().TakeDamage();
                    bulletImages[Ammo - 1].enabled = false;
                    Ammo--;
                    isCocked = false;
                    wasFired = true;
                }
            }
            else if (Ammo == 0)
            {
                //todo play miss-fire sound.
            }

            if (Input.GetButtonDown(ReloadInput) && Ammo < 6)
            {
                if (!isReloadCoroutineRunning)
                {
                    StartCoroutine(ReloadGun());
                    StartCoroutine(ReloadAnimation());
                }
            }
        }
    }

    IEnumerator ReloadGun()
    {
        Debug.Log("Reload Gun");
        isReloadCoroutineRunning = true;
        float timeToWait = .25f * (6 - Ammo);
        yield return new WaitForSeconds(timeToWait);

        Ammo = 6;
        isReloadCoroutineRunning = false;
    }

    IEnumerator ReloadAnimation()
    {

        foreach (Image bulletImage in bulletImages)
        {
            if (!bulletImage.enabled)
                yield return new WaitForSeconds(.25f);
            //todo play reload bullet sound
            bulletImage.enabled = true;
            //bulletImage.color = new Color(bulletImage.color.r, bulletImage.color.g, bulletImage.color.b, 255f);
        }
        yield return null;
    }

    IEnumerator ShotDelayIfNotCocked()
    {
        isShootingCoroutineRunning = true;

        yield return new WaitForSeconds(.5f);

        Debug.Log("Fire Gun");
        audio.clip = shootingSound;
        audio.Play();
        //DYLAN WAS HERE: Set the fire animation state to true!
        animTest.SetBool("isIdle", false);
        animTest.SetBool("isFiring", true);
        StartCoroutine(ResetFireAnimation());
        if (playerAimInformation.playerInCrossHairs != null)
            playerAimInformation.playerInCrossHairs.GetComponentInChildren<PlayerHealth>().TakeDamage();
        isCocked = false;
        wasFired = true;
        bulletImages[Ammo - 1].enabled = false;
        //bulletImages[Ammo - 1].color = new Color(bulletImages[Ammo - 1].color.r, bulletImages[Ammo - 1].color.g, bulletImages[Ammo - 1].color.b, 50f);
        Ammo--;
        isShootingCoroutineRunning = false;
    }

    IEnumerator ResetFireAnimation()
    {
        yield return new WaitForSeconds(1);
        animTest.SetBool("isIdle", true);
        animTest.SetBool("isFiring", false);
    }
}
