using System;
using UnityEngine;

public class SunController : MonoBehaviour
{
    public GameObject sun;
    public float latitude = 51.05f;
    public float twilightColorBuffer = 2;
    public float twilightIntensityBuffer = 1;

    [Range(0, 24)]
    public int currHour;
    [Range(0, 59)]
    public int currMinute;
    [Range(1,365)]
    public int currDay;
    public bool UseSystemTime;
    
    float sunrise, sunset;
    float currTime;

    public Color twilightColor = Color.orange;
    public Color dayColour = Color.white;
    public float dayIntensity = 2;
    //DateTime time;

    private Light sunlight;

    // Start is called once befobre the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //time = DateTime.Now;
        UpdateDay();
        sunlight = sun.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (UseSystemTime)
        {
            DateTime time = DateTime.Now;
            currHour = time.Hour;
            currMinute = time.Minute;
            currDay = time.DayOfYear;
        }

        UpdateDay();

        float dayLength = sunset - sunrise;
        currTime = currHour + ((float)currMinute / 60);

        float rotateX = -90;
        if ((currTime >= sunrise) && (currTime <= sunset))
            rotateX = ((currTime - sunrise) / dayLength) * 180;

        sun.transform.rotation = Quaternion.Euler(rotateX, 0, 0);
        sunlight.color = GetSunColour();
        sunlight.intensity = GetSunIntensity();
    }

    private Color GetSunColour()
    {
        Color sunColour = dayColour;
        float twilightProgress;

        if ((currTime > sunrise) && (currTime < sunrise + twilightColorBuffer))
        {
            twilightProgress = (currTime - sunrise) / twilightColorBuffer;
            sunColour = Color.Lerp(twilightColor, dayColour, twilightProgress);
        }
        else if ((currTime < sunset) && (currTime > sunset - twilightColorBuffer))
        {
            twilightProgress = (sunset - (currTime + twilightColorBuffer)) * -1;
            sunColour = Color.Lerp(dayColour, twilightColor, twilightProgress);
        }

        return sunColour;
    }

    private Color GetMoonColour()
    {
        return dayColour;
    }

    private float GetSunIntensity()
    {
        float sunIntensity;
        float twilightProgress;

        if ((currTime > sunrise) && (currTime < sunrise + twilightIntensityBuffer))
        {
            twilightProgress = (currTime - sunrise) / twilightIntensityBuffer;
            //sunIntensity = (twilightIntensityBuffer / twilightProgress) * dayIntensity;
            sunIntensity = Mathf.Lerp(0, dayIntensity, twilightProgress);
        }
        else if ((currTime < sunset) && (currTime > sunset - twilightIntensityBuffer))
        {
            twilightProgress = (sunset - currTime) / twilightIntensityBuffer;
            //twilightProgress = (sunset - (currTime - twilightIntensityBuffer)) * -1;
            sunIntensity = Mathf.Lerp(0, dayIntensity, twilightProgress);
        }
        else if ((currTime < sunrise) || (currTime > sunset))
            sunIntensity = 0;
        else
            sunIntensity = dayIntensity;

        return sunIntensity;
    }

    private void UpdateDay()
    {
        float declination = 23.44f * Mathf.Deg2Rad * Mathf.Sin(Mathf.Deg2Rad * (360f / 365f) * (currDay - 81));
        float hourAngle = Mathf.Acos(-Mathf.Tan(latitude * Mathf.Deg2Rad) * Mathf.Tan(declination));
        sunrise = 12f - (24f / (2f * Mathf.PI)) * hourAngle;
        sunset = 12f + (24f / (2f * Mathf.PI)) * hourAngle;
    }
}
