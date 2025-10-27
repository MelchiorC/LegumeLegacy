using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance {get; private set; }

    [Header("Internal Clock")]
    [SerializeField]
    GameTimestamp timestamp;
    public float timeScale = 10.0f;

    [Header ("Day and Night cycle")]
    //The tranform of the directional light (sun)
    public Transform sunTransform;

    //List of objects to inform of changes to the time
    List<ITimeTracker> listeners = new List<ITimeTracker>();

    public void Awake()
    {
        //If there is more than one instance, destroy the extra
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Initialize the time stamp
        timestamp = new GameTimestamp(0, GameTimestamp.Season.Panas, 1, 6, 0);
        StartCoroutine(TimeUpdate());
    }
    public GameTimestamp TimeGiver()
    {
        return timestamp;
    }
    IEnumerator TimeUpdate()
    {
        while (true)
        {
            Tick();
            yield return new WaitForSeconds(1 / timeScale);
        }
        
    }

    //A tick of the in-game time
    private int lastDayChecked = -1;

    public void Tick()
    {
        timestamp.UpdateClock();

        // Inform each of the listeners of the new time state
        foreach (ITimeTracker listener in listeners)
        {
            listener.ClockUpdate(timestamp);
        }

        // Only update weather when the day changes
        if (timestamp.hour == 0 && timestamp.minute == 0 && timestamp.day != lastDayChecked)
        {
            lastDayChecked = timestamp.day;
            WeatherManager.Instance?.UpdateWeather(timestamp.season);
        }
    }

    //Day an night cycle
    void UpdateSunMovement()
    {
        //Confvert the current time to minutes
        int timeInMinutes = GameTimestamp.HoursToMinutes(timestamp.hour) + timestamp.minute;

        //Sun moves 15 degrees in a hour
        //.25 degrees in a minute
        //At midnight (00.00), the angle of the sun should be -90 degrees
        float sunAngle = 20+.25f * timeInMinutes - 90;

        //Apply the angle of the directional light
        sunTransform.eulerAngles = new Vector3(sunAngle, 0, 0);
    }

    //Get the timestamp
    public GameTimestamp GetGameTimestamp()
    {
        //Return a cloned instance
        return new GameTimestamp(timestamp);
    }
    public void skipTimeStamp()
    {
        // Clone and increment the day
        GameTimestamp nextDay = new GameTimestamp(timestamp);
        nextDay.updateDay();

        // Update the weather FIRST for the next day
        WeatherManager.Instance?.UpdateWeather(nextDay.season);

        // Set the timestamp to the new day (06:00)
        timestamp = new GameTimestamp(nextDay);

        // Update the day tracker so weather doesn't re-trigger in Tick()
        lastDayChecked = timestamp.day;

        Debug.Log("Time skipped to day " + timestamp.day + ", weather updated.");
    }

    //Handling Listeners


    //Add the object to the list of listeners
    public void RegisterTracker(ITimeTracker listener)
    {
        listeners.Add(listener);
    }
    
    //Remove the object from the list of listeners
    public void UnregisterTracker(ITimeTracker listener)
    {
        listeners.Remove(listener);
    }
}
