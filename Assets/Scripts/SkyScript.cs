using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SkyScript : MonoBehaviour
{
    [SerializeField] GameObject lightSource;
    [SerializeField] Material shaderMaterial; //The skybox shader

    float time;
    float prevTime;
    float nextTime;
    int prevKeyframe;
    int nextKeyframe;
    int numKeyframes;

    [SerializeField] float timeSpeed = 0.01f;
    [SerializeField] float dayLength = 1440;

    //Skycolour is Color_9A3F8B33
    //Horizoncolour is Color_5DF3F889
    //Stripcontroller is Vector1_9578139A
    //shaderMaterial.SetColor("Color_9A3F8B33", new Color(skyColour,0.5f,0.7f)); //Sets the colour of Skycolour

    private static SkyKeyframe[] SkyKeyframes; //2D array stores  ! [TIME, STRIPECONTROLLER, R1, G1, B1, R2, G2, B2, STARALPHA, LIGHTLEVEL] !

    void Start()
    {
        time = 0;
        prevKeyframe = 0;
        nextKeyframe = 1;
        numKeyframes = 7;

        SkyKeyframes = new SkyKeyframe[7];
        SkyKeyframes[0] = new SkyKeyframe(0, 60, new Color(0.048f, 0.023f, 0.33f), (new Color(0.15f, 0.02f, 0.15f)), 100, 0.01f); //00:01
        SkyKeyframes[1] = new SkyKeyframe(300, 167.76f, new Color(0.01f, 0.01f, 0.28f), (new Color(0.26f, 0.24f, 0.26f)), 150, 0.1f); //5:00
        SkyKeyframes[2] = new SkyKeyframe(360, 10.45f, new Color(0.004f, 0.8f, 1), (new Color(0.8f, 0.79f, 0.41f)), 1200, 0.6f); //6:00
        SkyKeyframes[3] = new SkyKeyframe(720, 29.7f, new Color(0.31f, 0.887f, 0.9f), (new Color(0.37f, 0.84f, 0.835f)), 1200, 1); //12:00
        SkyKeyframes[4] = new SkyKeyframe(1140, 50, new Color(0.753f, 0.11f, 0.499f), (new Color(0.66f, 0.3f, 0.1f)), 1200, 0.6f); //19:00
        SkyKeyframes[5] = new SkyKeyframe(1200, 15.6f, new Color(0.16f, 0.09f, 0.71f), (new Color(0.745f, 0.123f, 0.381f)), 150, 0.15f); //20:00
        SkyKeyframes[6] = new SkyKeyframe(1440, 60, new Color(0.048f, 0.023f, 0.33f), (new Color(0.15f, 0.02f, 0.15f)), 100, 0.01f); //00:01 //23:59

        prevTime = SkyKeyframes[0].GetTime();
        nextTime = SkyKeyframes[1].GetTime();

        //string path = "Assets/TextFiles/SkyKeyframes.txt";
        //StreamReader reader = new StreamReader(path);
        //for (int i = 0; i < numKeyframes; i++) {
        //    string fullText = reader.ReadLine();
        //} FOR READING DATA FROM A TEXT FILE
        
        
    } //START

  
    void Update()
    {
        
    } //UPDATE

    

    private void FixedUpdate()
    {
        time += timeSpeed; //Increase time every fixed update, wrap around to zero after 1440min (24hr * 60min)
        if (time > dayLength)
            time = 0;

        if (time >= nextTime)
        {
            prevKeyframe++;
            nextKeyframe++;
            if (prevKeyframe >= numKeyframes)
                prevKeyframe = 0;
            if (nextKeyframe >= numKeyframes)
                nextKeyframe = 0;

            nextTime = SkyKeyframes[nextKeyframe].GetTime();
            prevTime = SkyKeyframes[prevKeyframe].GetTime();
        }

        float lerpTime = (time - prevTime) / (nextTime - prevTime);

        Color skycolourCurrent = new Color(Mathf.Lerp(SkyKeyframes[prevKeyframe].GetSkyColour().r, SkyKeyframes[nextKeyframe].GetSkyColour().r, lerpTime), Mathf.Lerp(SkyKeyframes[prevKeyframe].GetSkyColour().g, SkyKeyframes[nextKeyframe].GetSkyColour().g, lerpTime), Mathf.Lerp(SkyKeyframes[prevKeyframe].GetSkyColour().b, SkyKeyframes[nextKeyframe].GetSkyColour().b, lerpTime));
        Color horizoncolourCurrent = new Color(Mathf.Lerp(SkyKeyframes[prevKeyframe].GetHorizonColour().r, SkyKeyframes[nextKeyframe].GetHorizonColour().r, lerpTime), Mathf.Lerp(SkyKeyframes[prevKeyframe].GetHorizonColour().g, SkyKeyframes[nextKeyframe].GetHorizonColour().g, lerpTime), Mathf.Lerp(SkyKeyframes[prevKeyframe].GetHorizonColour().b, SkyKeyframes[nextKeyframe].GetHorizonColour().b, lerpTime));
        float stripecontrollerCurrent = Mathf.Lerp(SkyKeyframes[prevKeyframe].GetStripeController(), SkyKeyframes[nextKeyframe].GetStripeController(), lerpTime);
        float starControllerCurrent = Mathf.Lerp(SkyKeyframes[prevKeyframe].GetStarAlpha(), SkyKeyframes[nextKeyframe].GetStarAlpha(), lerpTime);

        lightSource.GetComponent<Light>().intensity = Mathf.Lerp(SkyKeyframes[prevKeyframe].GetLightLevel(), SkyKeyframes[nextKeyframe].GetLightLevel(), lerpTime); //Lerps the light intensity
        lightSource.transform.Rotate((timeSpeed/dayLength)*360, 0, 0); //Rotates the directional light
        //Debug.Log((time / dayLength) * -360);

        shaderMaterial.SetVector("SunDirection1", new Vector3(20,123,14));
        shaderMaterial.SetColor("Color_9A3F8B33", skycolourCurrent);
        shaderMaterial.SetColor("Color_5DF3F889", horizoncolourCurrent);
        shaderMaterial.SetFloat("Vector1_9578139A", stripecontrollerCurrent);
        shaderMaterial.SetFloat("_StarController", starControllerCurrent);
        //colour = LERP(prevColour,nextColour,(time-prevTime)/(nextTime-prevTime))

        //Debug.Log("Time: "+time+" LerpTime: "+ lerpTime);
    } //FIXED
}

public class SkyKeyframe : MonoBehaviour
{
    float time;
    float stripeController;
    Color skyColour;
    Color horizonColour;
    float starAlpha;
    float lightLevel;

    public SkyKeyframe(float newTime, float newStripeController, Color newSkyColour, Color newHorizonColour, float newStarAlpha, float newLightLevel)
    {
        time = newTime;
        stripeController = newStripeController;
        starAlpha = newStarAlpha;
        skyColour = newSkyColour;
        horizonColour = newHorizonColour;
        lightLevel = newLightLevel;
    }

    public float GetTime()
    {
        return time;
    }
    public float GetStripeController()
    {
        return stripeController;
    }

    public Color GetSkyColour()
    {
        return skyColour;
    }

    public Color GetHorizonColour()
    {
        return horizonColour;
    }

    public float GetLightLevel()
    {
        return lightLevel;
    }

    public float GetStarAlpha()
    {
        return starAlpha;
    }

}
