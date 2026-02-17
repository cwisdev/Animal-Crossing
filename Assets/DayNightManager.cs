using System;
using UnityEngine;
using UnityEngine.UIElements;

public class DayNightManager: MonoBehaviour
{
    public GameObject sun;
    public GameObject moon;
    public Color twilightColor = Color.orange;
    public Color dayColour = Color.white;
    public Color nightColour = Color.blue;
    public float dayIntensity = 2;
    public float nightIntensity = .5f;
    public float latitude = 51.05f;
    public float twilightColorBuffer = 2;
    public float twilightIntensityBuffer = 1;
    public float maxMoonHeight = 180;
    public float minMoonHeight = 155;
    public float moonApexYaw = 90;
    public float moonYawRange = 90;

    [Range(0, 23)]
    public int currHour;
    [Range(0, 59)]
    public int currMinute;
    [Range(1,365)]
    public int currDay;
    public bool UseSystemTime;

    int lastDay, lastHour, lastMinute;
    
    float sunrise, sunset;
    float currTime;

    private Light sunlight;
    private Light moonlight;

    void Start()
    {
        sunlight = sun.GetComponent<Light>();
        moonlight = moon.GetComponent<Light>();
    }

    void Update()
    {
        if (UseSystemTime)
        {
            DateTime time = DateTime.Now;
            currHour = time.Hour;
            currMinute = time.Minute;
            currDay = time.DayOfYear;
        }

        // If this is a new day, get the new sunrise and sunset times
        if (DateHasChanged())
            CalcSunriseSunset();

        // Change the positions and lighting of the sun and moon when the time changes
        if (TimeHasChanged())
        {
            float dayLength = sunset - sunrise;
            currTime = currHour + ((float)currMinute / 60);

            float rotateX = -90;
            if ((currTime >= sunrise) && (currTime <= sunset))
                rotateX = ((currTime - sunrise) / dayLength) * 180;

            sun.transform.rotation = Quaternion.Euler(rotateX, 0, 0);

            UpdateSunColour();
            UpdateSunIntensity();
            UpdateMoonRotation();
            UpdateMoonIntensity();
            moonlight.color = nightColour;
        }

        lastDay = currDay;
        lastHour = currHour;
        lastMinute = currMinute;
    }

    // Transition between the daylight hue and the dusk/dawn hue
    private void UpdateSunColour()
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

        sunlight.color = sunColour;
    }

    // Increase or dims the moon's light during dawn and dusk
    private void UpdateSunIntensity()
    {
        float sunIntensity;
        float twilightProgress;

        if ((currTime > sunrise) && (currTime < sunrise + twilightIntensityBuffer))
        {
            twilightProgress = (currTime - sunrise) / twilightIntensityBuffer;
            sunIntensity = Mathf.Lerp(0, dayIntensity, twilightProgress);
        }
        else if ((currTime < sunset) && (currTime > sunset - twilightIntensityBuffer))
        {
            twilightProgress = (sunset - currTime) / twilightIntensityBuffer;
            sunIntensity = Mathf.Lerp(0, dayIntensity, twilightProgress);
        }
        else if ((currTime < sunrise) || (currTime > sunset))
            sunIntensity = 0;
        else
            sunIntensity = dayIntensity;

        sunlight.intensity = sunIntensity;
    }

    private void UpdateMoonRotation()
    {
        float rotateX, rotateY;
        float minMoonYaw = moonApexYaw - moonYawRange / 2;
        float maxMoonYaw = moonApexYaw + moonYawRange / 2;

        if ((currTime < sunrise) || (currTime > sunset))
        {
            // Get the current progress of the night as a percentage
            float nightLength = 24 - (sunset - sunrise);
            float nightTime;
            if (currTime < sunrise)
                nightTime = 24 - sunset + currTime;
            else
                nightTime = currTime - sunset;
            nightTime /= nightLength;

            // Arc the moon across the sky
            float midpoint = (maxMoonHeight + minMoonHeight) / 2;
            float amplitude = (maxMoonHeight - minMoonHeight) / 2;
            rotateX = midpoint + amplitude * Mathf.Cos(2 * Mathf.PI * nightTime);
            rotateY = minMoonYaw + (moonYawRange * nightTime);
        }
        else
        {
            rotateX = maxMoonHeight;

            // Snap the moon to the east or west to ease the transition between dusk and dawn
            if (currTime < 12)
                rotateY = maxMoonYaw;
            else
                rotateY = minMoonYaw;
        }

        moon.transform.rotation = Quaternion.Euler(rotateX, rotateY, 0);
    }

    // Dims or increases the moon's light during dawn and dusk
    private void UpdateMoonIntensity()
    {
        float moonIntensity;
        float twilightProgress;

        if ((currTime > sunrise) && (currTime < sunrise + twilightIntensityBuffer))
        {
            twilightProgress = (currTime - sunrise) / twilightIntensityBuffer;
            moonIntensity = Mathf.Lerp(nightIntensity, 0, twilightProgress);
        }
        else if ((currTime < sunset) && (currTime > sunset - twilightIntensityBuffer))
        {
            twilightProgress = (sunset - currTime) / twilightIntensityBuffer;
            moonIntensity = Mathf.Lerp(nightIntensity, 0, twilightProgress);
        }
        else if ((currTime < sunrise) || (currTime > sunset))
            moonIntensity = nightIntensity;
        else
            moonIntensity = 0;

        moonlight.intensity = moonIntensity;
    }

    private bool TimeHasChanged()
    {
        return (currHour != lastHour) || (currMinute != lastMinute);
    }

    private bool DateHasChanged()
    {
        return (currDay != lastDay);
    }

    private void CalcSunriseSunset()
    {
        float declination = 23.44f * Mathf.Deg2Rad * Mathf.Sin(Mathf.Deg2Rad * (360f / 365f) * (currDay - 81));
        float hourAngle = Mathf.Acos(-Mathf.Tan(latitude * Mathf.Deg2Rad) * Mathf.Tan(declination));
        sunrise = 12f - (24f / (2f * Mathf.PI)) * hourAngle;
        sunset = 12f + (24f / (2f * Mathf.PI)) * hourAngle;
    }
}
