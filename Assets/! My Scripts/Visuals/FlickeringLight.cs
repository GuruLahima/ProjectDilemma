using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuruLaghima;

namespace Workbench.ProjectDilemma
{
  public class FlickeringLight : MonoBehaviour
  {
    [SerializeField] float cycleInterval;
    [SerializeField] float flickerInterval;
    [SerializeField] float frequency;
    [SerializeField] float emissionMaxValue;
    [SerializeField] Light light;
    [SerializeField] GameObject objectWithEmissiveMaterials;
    [SerializeField] float noiseThreshold;

    #region private fields

    IEnumerator flicker;
    IEnumerator flickerCycle;
    private bool isInCycle;
    private bool isFlickering;
    private Color emissionColor;

    #endregion

    private void Start()
    {

      if (objectWithEmissiveMaterials)
        emissionColor = objectWithEmissiveMaterials.GetComponentInChildren<MeshRenderer>().material.GetColor("_EmissionColor");

      if (light != null || objectWithEmissiveMaterials != null)
        StartCoroutine(Flickering());


    }

    IEnumerator Flickering()
    {

      while (true)
      {
        if (!isInCycle)
        {
          if (flickerCycle != null)
            StopCoroutine(flickerCycle);
          flickerCycle = FlickerCycle(cycleInterval);
          yield return StartCoroutine(flickerCycle);

        }
      }
    }

    IEnumerator FlickerCycle(float interval)
    {
      MyDebug.Log("FlickerCycle", "cycle started");
      isInCycle = true;

      float timer = 0;
      while (timer < interval)
      {
        timer += Time.deltaTime;


        float val = Mathf.PerlinNoise(Time.time / frequency, 0f);
        // MyDebug.Log(val.ToString());
        if (val > noiseThreshold)
        {
          if (!isFlickering)
          {
            if (flicker != null)
              StopCoroutine(flicker);
            flicker = Flicker(flickerInterval);
            // yield return StartCoroutine(flicker);
            StartCoroutine(flicker);
          }
        }

        yield return null;
      }

      isInCycle = false;
    }

    IEnumerator Flicker(float interval)
    {
      // MyDebug.Log("FlickerCycle", "flickered");
      isFlickering = true;

      // light flicker
      float prevIntensity = 0;
      if (light)
      {
        prevIntensity = light.intensity;
        light.intensity = 0;
      }

      // emissive material flicker
      if (objectWithEmissiveMaterials)
      {
        // MyDebug.Log("FlickerCycle", "emisive material should flickered");

        foreach (MeshRenderer mr in objectWithEmissiveMaterials.GetComponentsInChildren<MeshRenderer>())
        {
          // MyDebug.Log("FlickerCycle", "emisive material flickered");
          mr.material.SetColor("_EmissionColor", emissionColor * 0f);
        }
      }

      yield return new WaitForSeconds(interval);

      if (light)
      {
        light.intensity = prevIntensity;
      }

      if (objectWithEmissiveMaterials)
      {

        foreach (MeshRenderer mr in objectWithEmissiveMaterials.GetComponentsInChildren<MeshRenderer>())
        {
          mr.material.SetColor("_EmissionColor", emissionColor * emissionMaxValue);
        }
      }

      isFlickering = false;

    }
  }
}