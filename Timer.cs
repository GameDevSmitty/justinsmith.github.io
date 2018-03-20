using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public enum TimerStates { Play, Pause, Reset };
    public TimerStates timerStates;

    public static Timer instance = null;

    [SerializeField] float timer = 5f;
    public float TimerChanger { get { return timer; } set { timer = value; } }
    //[SerializeField] int numberOfInitialSpawns = 5;
    bool isGameRunning;
    float time;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Use this for initialization
    void Start ()
    {
        time = timer;
        isGameRunning = true;
        StartCoroutine(HandleTimer());
	}

    public void RoundStartSpawns(int numberOfSpawns)
    {
        for (int i = 0; i < numberOfSpawns; i++)
            EventManager.CallTimerAction(Vector3.zero);
    }
	
    IEnumerator HandleTimer()
    {
        while(isGameRunning)
        {
            switch(timerStates)
            {
                case TimerStates.Play:
                    //Reduce the time and fire once at or below 0 then reset
                    time -= Time.deltaTime;

                    if(time <= 0.0f)
                    {
                        EventManager.CallTimerAction(Vector3.zero);
                        time = timer;
                    }
                    break;
                case TimerStates.Pause:
                    //Timer is paused nothing happens
                    break;
                case TimerStates.Reset:
                    //Reset the time to timer then Play the timer;
                    time = timer;
                    timerStates = TimerStates.Play;
                    break;
            }
            yield return null;
        }
    }
    
    void TimerActivated(Vector3 v)
    {
        EventManager.CallTimerAction(v);
    }
}
