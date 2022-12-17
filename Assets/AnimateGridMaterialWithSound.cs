using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateGridMaterialWithSound : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public TMPro.TextMeshProUGUI titleText;
    public Material MaterialToAnimate;
    private Material originalMat;


    //sound spectrum animation stuff
    [Header("Sound Visualization")]
    [SerializeField] Button b_host;
    [SerializeField] Button b_settings;
    [SerializeField] Button b_credits;
    [SerializeField] Button b_quit;

    [SerializeField] float frequencyStartHz;

    [HideInInspector] float startButtonWidth;
    [SerializeField] float range_end_host;
    [SerializeField] float range_end_settings;
    [SerializeField] float range_end_credits;
    [SerializeField] float range_end_quit;

    //range starts at frequencyStart then each additional range adds on top of that
    [Header("Settings")]
    [HideInInspector]public bool epilepsy;
    [HideInInspector] public bool started;



    [SerializeField] Light sunLight;
    private Color originalLightColor;
    [SerializeField] float onBeatSunColorModifier;

    public float updateStep = 0.1f;
    public int sampleDataLength = 1024;

    private float currentUpdateTime = 0f;

    [SerializeField] float clipLoudnessBeatDetectionThreshold;
    private float clipLoudness;
    private float[] clipSampleData;





    private void ButtonVisualizeSound(float[] soundData, Button btn, float range, float start)
    {



    }






    // Use this for initialization
    void Awake()
    {
        startButtonWidth = b_host.GetComponent<RectTransform>().sizeDelta.x;
        originalLightColor = sunLight.color;
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
        
        if (epilepsy || !started)
        {
            return;
        }
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
            Color newColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f); //new Color((float)clipLoudness, originalMat.color.g, originalMat.color.b);

            clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for


            ButtonVisualizeSound(clipSampleData, b_host, range_end_host, frequencyStartHz);
            ButtonVisualizeSound(clipSampleData, b_settings, range_end_settings, range_end_host);
            ButtonVisualizeSound(clipSampleData, b_credits, range_end_credits, range_end_settings);
            ButtonVisualizeSound(clipSampleData, b_quit, range_end_quit, range_end_credits);


            if (clipLoudness > clipLoudnessBeatDetectionThreshold)
            {
                sunLight.color = new Color(originalLightColor.r + (clipLoudness * onBeatSunColorModifier), originalLightColor.g, originalLightColor.b);
                MaterialToAnimate.SetColor("_UnlitColor", newColor);
                titleText.color = newColor;
            }
            else
            {
                sunLight.color = originalLightColor;
                MaterialToAnimate.SetColor("_UnlitColor", Color.white);
                titleText.color = Color.white;
            }
            

            //Debug.LogWarning("clipLoudness ->" + clipLoudness);
        }
        
    }






}
