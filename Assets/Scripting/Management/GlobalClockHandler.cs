using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalClockHandler : MonoBehaviour
{
    [SerializeField] private bool startMidDay;
    [SerializeField] private float timeScale;
    [SerializeField] private float fullDayLengthMinutes;

    public int workingDayStart;
    public int workingDayEnd;

    [Header("Time")]
    [SerializeField] private int secondsPerMinute;
    public int seconds;
    public int minutes;
    public int hours;
    public float currentSecondsOfDay;
    public float maxSecondsOfDay;

    [Header("Date")]
    public int dayOfTheWeek;
    public int dayOfTheMonth;
    public int month;
    public int year;

    [Header("Day Night")]
    public Transform sunLight;
    public float rotationSpeed;
    public bool isDayTime;

    private void Start()
    {
        timeScale = 1;
        ResetCalender();
        sunLight.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (startMidDay) hours = 12;
    }

    private void Update()
    {
        AdjustTimeScale();
        if (timeScale > 0.1f)
        {
            secondsPerMinute = (int)(((((fullDayLengthMinutes / 24) * (60 / Time.timeScale)))));
            if (secondsPerMinute <= 0) secondsPerMinute = 1;
            if (seconds < secondsPerMinute) seconds++;
            else
            {
                seconds = 0;
                minutes++;
                if (minutes >= 60 / Time.timeScale)
                {
                    minutes = 0;
                    hours++;
                    if (hours >= 24) StartNextDay();
                }
            }

            if (hours > workingDayStart && hours < workingDayEnd) isDayTime = true;
            else isDayTime = false;

            currentSecondsOfDay = ((((float)hours * (60 / Time.timeScale)) + (float)minutes) * (float)secondsPerMinute) + seconds;
            maxSecondsOfDay = ((float)secondsPerMinute * (60 / Time.timeScale)) * 24;
        }
        sunLight.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 360 * (currentSecondsOfDay / maxSecondsOfDay)), rotationSpeed * Time.deltaTime);
    }

    private void StartNextDay()
    {
        dayOfTheWeek++;
        if (dayOfTheWeek > 7) dayOfTheWeek = 1;
        dayOfTheMonth++;
        if (dayOfTheMonth >= 30)
        {
            dayOfTheMonth = 1;
            month++;
            if (month > 12)
            {
                month = 1;
                year++;
            }
        }
        seconds = 0;
        minutes = 0;
        hours = 0;
    }

    private void ResetCalender()
    {
        dayOfTheWeek = 1;
        dayOfTheMonth = 1;
        month = 1;
        year = 0;
    }

    private void AdjustTimeScale()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (timeScale > 0.1f) timeScale = 0.1f;
            else timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) timeScale = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2)) timeScale = 2.25f;
        if (Input.GetKeyDown(KeyCode.Alpha3)) timeScale = 3.75f;

        if (Time.timeScale != timeScale) Time.timeScale = timeScale;
    }
}
