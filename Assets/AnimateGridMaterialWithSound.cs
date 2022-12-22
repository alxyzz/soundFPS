using UnityEngine;
using UnityEngine.UI;
using GD.MinMaxSlider;

public class AnimateGridMaterialWithSound : MonoBehaviour
{
    [Header("References")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] TMPro.TextMeshProUGUI titleText;
    [SerializeField] Material MaterialToAnimate;
    private Material originalMat;
    [SerializeField] SpectrumAnalyzer soundAnalyzer;

    //sound spectrum animation stuff
    [Header("Sound Visualization")]
    [SerializeField] Button b_host;
    [SerializeField] Button b_settings;
    [SerializeField] Button b_credits;
    [SerializeField] Button b_quit;

    [SerializeField] float frequencyStartHz;

    [HideInInspector] float startButtonWidth;
    [Header("Frequency Ranges (in Hertz)")]
    [MinMaxSlider(20, 15000)]
    [SerializeField] Vector2 frequencyRange_host;
    [MinMaxSlider(20, 15000)]
    [SerializeField] Vector2 frequencyRange_settings;
    [MinMaxSlider(20, 15000)]
    [SerializeField] Vector2 frequencyRange_credits;
    [MinMaxSlider(20, 15000)]
    [SerializeField] Vector2 frequencyRange_quit;

    [SerializeField] float barDecayValue;
    [SerializeField] float barGrowMultiplier;
    [SerializeField] float barLerpStep;



    float targetWidth_host;
    float targetWidth_settings;
    float targetWidth_credits;
    float targetWidth_quit;

    //range starts at frequencyStart then each additional range adds on top of that
    [Header("Settings")]
    [HideInInspector] public bool epilepsy;
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


    RectTransform b_hostRect;
    RectTransform b_settingsRect;
    RectTransform b_creditsRect;
    RectTransform b_quitRect;



    /// <summary>
    /// 1. check if sound was higher in that frequency
    /// 2. if it was, we grow the relevant bar target
    /// 3. if it was not, we decay the relevant bar target
    /// 4. on update, we lerp the bar width to the bar target
    /// </summary>

    private void ButtonVisualizeSound()
    {

        b_hostRect.sizeDelta = new Vector2(Mathf.Clamp(Mathf.Lerp(b_hostRect.sizeDelta.x, targetWidth_host, barLerpStep), 125, 860), b_hostRect.sizeDelta.y);
        b_settingsRect.sizeDelta = new Vector2(Mathf.Clamp(Mathf.Lerp(b_settingsRect.sizeDelta.x, targetWidth_settings, barLerpStep), 125, 860), b_hostRect.sizeDelta.y);
        b_creditsRect.sizeDelta = new Vector2(Mathf.Clamp(Mathf.Lerp(b_creditsRect.sizeDelta.x, targetWidth_credits, barLerpStep), 125, 860), b_hostRect.sizeDelta.y);
        b_quitRect.sizeDelta = new Vector2(Mathf.Clamp(Mathf.Lerp(b_quitRect.sizeDelta.x, targetWidth_quit, barLerpStep), 125, 860), b_hostRect.sizeDelta.y);
    }



    private float GetWidthTarget(float sound, Button btn, float rangeStart, float rangeEnd)
    {

        
        if (inRange(rangeStart, rangeEnd, sound))
        {
            return sound * barGrowMultiplier;
        }
        else
        {
            return 105; //starts to decay
        }
    }



    private bool inRange(float min, float max, float value)
    {
        if (value >= min && value < max)
        {
            return true;
        }
        else return false;
    }
    // Use this for initialization
    void Awake()
    {
        startButtonWidth = b_host.GetComponent<RectTransform>().sizeDelta.x;
        originalLightColor = sunLight.color;
        originalMat = MaterialToAnimate;
        if (!audioSource)
        {
            Debug.LogError(GetType() + "AnimateGridMaterialWithSound.Awake: there was no audioSource set.");
        }
        clipSampleData = new float[sampleDataLength];


        b_hostRect = b_host.GetComponent<RectTransform>();
        b_settingsRect = b_settings.GetComponent<RectTransform>();
        b_creditsRect = b_credits.GetComponent<RectTransform>();
        b_quitRect = b_quit.GetComponent<RectTransform>();

        soundAnalyzer.SetAudio(audioSource);



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

            //we get the data
           




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
        //outside the period for smoother look
        float b = soundAnalyzer.AnalyzeSound();

        targetWidth_host = GetWidthTarget(b, b_host, frequencyRange_host.x, frequencyRange_host.y);
        targetWidth_settings = GetWidthTarget(b, b_settings, frequencyRange_settings.x, frequencyRange_settings.y);
        targetWidth_credits = GetWidthTarget(b, b_credits, frequencyRange_credits.x, frequencyRange_credits.y);
        targetWidth_quit = GetWidthTarget(b, b_quit, frequencyRange_quit.x, frequencyRange_quit.y);

        //we act accordingly
        ButtonVisualizeSound();

    }






}
