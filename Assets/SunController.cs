using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SunController : MonoBehaviour
{
    public GameObject sun;
    public GameObject moon;
    public Color twilightColor = Color.orange;
    public Color dayColour = Color.white;
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
    
    float sunrise, sunset;
    float currTime;

    //DateTime time;

    private Light sunlight;
    private Light moonlight;

    void Start()
    {
        //time = DateTime.Now;
        UpdateDay();
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

        UpdateDay();

        float dayLength = sunset - sunrise;
        currTime = currHour + ((float)currMinute / 60);

        float rotateX = -90;
        if ((currTime >= sunrise) && (currTime <= sunset))
            rotateX = ((currTime - sunrise) / dayLength) * 180;

        sun.transform.rotation = Quaternion.Euler(rotateX, 0, 0);
        sunlight.color = GetSunColour();
        sunlight.intensity = GetSunIntensity();

        UpdateMoonRotation();
        UpdateMoonIntensity();
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

    private float GetSunIntensity()
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

        return sunIntensity;
    }

    private void UpdateMoonRotation()
    {
        float moonRotateX, moonRotateY;
        float minMoonYaw = moonApexYaw - moonYawRange / 2;
        float maxMoonYaw = moonApexYaw + moonYawRange / 2;

        if ((currTime < sunrise) || (currTime > sunset))
        {
            float nightLength = 24 - (sunset - sunrise);
            float nightTime;
            if (currTime < sunrise)
                nightTime = 24 - sunset + currTime;
            else
                nightTime = currTime - sunset;
            nightTime /= nightLength;


            float midpoint = (maxMoonHeight + minMoonHeight) / 2;
            float amplitude = (maxMoonHeight - minMoonHeight) / 2;
            moonRotateX = midpoint + amplitude * Mathf.Cos(2 * Mathf.PI * nightTime);

            moonRotateY = minMoonYaw + (moonYawRange * nightTime);
        }
        else
        {
            moonRotateX = maxMoonHeight;
            moonRotateY = minMoonYaw;
        }

        moon.transform.rotation = Quaternion.Euler(moonRotateX, moonRotateY, 0);
    }

    private void UpdateMoonIntensity()
    {
        float twilightProgress;
        float moonIntensity;
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

    private void UpdateDay()
    {
        float declination = 23.44f * Mathf.Deg2Rad * Mathf.Sin(Mathf.Deg2Rad * (360f / 365f) * (currDay - 81));
        float hourAngle = Mathf.Acos(-Mathf.Tan(latitude * Mathf.Deg2Rad) * Mathf.Tan(declination));
        sunrise = 12f - (24f / (2f * Mathf.PI)) * hourAngle;
        sunset = 12f + (24f / (2f * Mathf.PI)) * hourAngle;
    }
}
