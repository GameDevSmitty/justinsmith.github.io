using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastingProbe : MonoBehaviour, ISubscriber
{
    
    [SerializeField] Timerold[] allTimers;
    [SerializeField] float distanceForSpawnRaycasting = 50f;
    [SerializeField] float distanceForTimerRaycasting = 15f;
    float fRandom;
    float raycastDegreeLimit = 30f;
    float spawnedCellHeightLimit = 9.5f;
    float spawnedCellHeightRandom;
    int layerMaskForSpawning = 1 << 9;
    int layerMaskForTiming = 1 << 8;
    int layerMaskForCells = 1 << 10;
    string hitSpawnerName;
    Vector3 forward;
    Vector3 hitSpawnerPoint;
    Timerold timer;
    SpawnManager spawnManager;
    RaycastHit hitTimer;
    RaycastHit hitSpawner;

    // Use this for initialization
    void Start ()
    {
        spawnManager = GameObject.Find("Managers").GetComponent<SpawnManager>();
        
        //forward = transform.TransformDirection(Vector3.forward);
        //RaycastForSpawning();
        //spawnManager.SpawnCorrectObject(hitSpawnerPoint);
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    // Update is called once per frame
    void Update ()
    {
        forward = transform.TransformDirection(Vector3.forward);
        RaycastForTimer();

        if (hitTimer.transform.GetComponent<Timerold>() != timer)
        {
            if (hitTimer.transform.GetComponent<Timerold>() != null)
            {
                StartCorrectTimer();
            }
        }

        //if (hitTimer.transform.GetComponent<Timer>() != null)//has a timer that is not null
        //{
        //    if (timer.ShouldSpawn)// should spawn == true
        //    {
        //        RaycastForSpawning();
        //        spawnManager.SpawnCell(hitSpawnerPoint);
        //    }
        //}
    }

    /// <summary> RaycastForSpawning:
    /// Checks for overlap between cell and spawned object? JUSTIN DOUBLE CHECK THIS
    /// Yes that is accurate, well it checks for overlap and then sets the hitSpawnerPoint to and empty space once there is no overlap.
    /// </summary>
    void RaycastForSpawning()
    {
        Vector3 hitOverlapTest;
        bool isOverlapping = false;
        do
        {
            spawnedCellHeightRandom = Random.Range(-spawnedCellHeightLimit, spawnedCellHeightLimit);
            fRandom = Random.Range(-raycastDegreeLimit, raycastDegreeLimit);

            Vector3 vRandom = Quaternion.Euler(0, fRandom, 0) * forward;

            Physics.Raycast(transform.position, vRandom, out hitSpawner, distanceForSpawnRaycasting, layerMaskForSpawning);

            hitSpawnerName = hitSpawner.transform.name;
            
            hitOverlapTest = new Vector3(hitSpawner.point.x, hitSpawner.point.y + spawnedCellHeightRandom, hitSpawner.point.z);

            //RaycastHit sphereHit;
            //Physics.SphereCast(hitOverlapTest, 1.1f, transform.forward, out sphereHit, layerMaskForCells)

            if (Physics.CheckSphere(hitOverlapTest, .55f, layerMaskForCells))
                isOverlapping = true;
            else
                isOverlapping = false;

        } while (isOverlapping);

        hitSpawnerPoint = hitOverlapTest;

        EventManagerOld.CallCellSpawn(hitSpawnerPoint, transform);
    }

    /// <summary> RaycastForTimer:
    /// returns the RaycastHit info of the timer that is hit by the raycast from player.
    /// Yes and it is always forward and there should always be a timer within range and in front of the player
    /// </summary>
    void RaycastForTimer()
    {
        Physics.Raycast(transform.position, forward, out hitTimer, distanceForTimerRaycasting, layerMaskForTiming);        
    }

    /// <summary> StartCorrectTimer:
    /// activates every timer attached to each spawned cell, if equal to the area timer? JUSTIN DOUBLE CHECK THIS 
    /// flip it and reverse it...  It makes sure the timer that is in front of the player is the only one that is active.
    /// and stops all other timers. Also this is independant of cells, these are the Timers that spawn the cells, not the CellTimers (I know confusing my B)
    /// In the world there are (currently) sixe Timers one corresponding to each colored area (play area) and this makes sure the Timer that you are facing
    /// is active and inturn spawning cells in that area.  They are also independant of each other so the player can rotate and "reset" the cell spawning timer.
    /// </summary>
    void StartCorrectTimer()
    {
        timer = hitTimer.transform.GetComponent<Timerold>();
        foreach (Timerold t in allTimers)
        {
            if (t != timer)
                t.StopTimer();
            else
                t.StartTimer();
        }
    }

    public void Subscribe()
    {
        EventManagerOld.CellEvents += RaycastForSpawning;
    }

    public void Unsubscribe()
    {
        EventManagerOld.CellEvents -= RaycastForSpawning;
    }
}
