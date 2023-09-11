using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyExplosion : MonoBehaviour
{

    public float explosionDelay;

    public float initialDuration;

    public float lightDuration;

    public GameObject light;

    public float initialIntensity;
    
    void Start()
    {
        initialDuration = lightDuration;
      //  initialIntensity = light.GetComponent<Light>().intensity;
    }

    
    void Update()
    {   
        explosionDelay  -= Time.deltaTime;
        lightDuration -= Time.deltaTime;

        light.GetComponent<Light>().intensity = initialIntensity - initialIntensity * ((initialDuration-lightDuration)/initialDuration);
        
        if(explosionDelay <= 0){
            Destroy(this.gameObject);
        }
        
    }
}
