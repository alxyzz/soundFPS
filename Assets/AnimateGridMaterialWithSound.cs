using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateGridMaterialWithSound : MonoBehaviour
{

    public AudioSource audioSource;
    public TMPro.TextMeshProUGUI titleText;
    public Material MaterialToAnimate;
    private Material originalMat;


    public float updateStep = 0.1f;
    public int sampleDataLength = 1024;

    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float[] clipSampleData;

    // Use this for initialization
    void Awake()
    {
        originalMat = MaterialToAnimate;
        if (!audioSource)
        {
            Debug.LogError(GetType() + ".Awake: there was no audioSource set.");
        }
        clipSampleData = new float[sampleDataLength];

    }

    // Update is called once per frame
    void Update()
    {

        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
            if (clipLoudness > 0.2)
            {
                Color newColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f); //new Color((float)clipLoudness, originalMat.color.g, originalMat.color.b);
                MaterialToAnimate.SetColor("_EmissionColor", newColor);
                titleText.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f) ;
            }
            else
            {
                Color newColor = new Color(originalMat.color.r, originalMat.color.g, originalMat.color.b);
                MaterialToAnimate.SetColor("_EmissionColor", newColor);
                titleText.color = Color.white;
            }
            

            Debug.LogWarning("clipLoudness ->" + clipLoudness);
        }
        
    }

}
