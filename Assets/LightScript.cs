using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Shader.SetGlobalVector("_SunDirection", transform.forward);
        Shader.SetGlobalVector("_MoonDirection", transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_SunDirection", transform.forward);
        Shader.SetGlobalVector("_MoonDirection", transform.forward);
    }
}