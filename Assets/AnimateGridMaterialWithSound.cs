using System;
using UnityEngine;
using UnityEngine.UI;
using GD.MinMaxSlider;
using System.Collections.Generic;
using Unity.VisualScripting;
using Random = UnityEngine.Random;


public class AnimatedMenuElement
{
    public Image image;

    public RectTransform imageDimensions;
    public float targetWidth;
    public Tuple<int, int> ActivationRange;

    public int matchNumber;


    public AnimatedMenuElement(GameObject obj, int matchnumber)
    {
        image = obj.GetComponent<Image>();
        imageDimensions = obj.GetComponent<RectTransform>();
        matchNumber = obj.GetComponent<AnimatedObjectInitializer>().matchNumber;
       

    }

    public void AssignActivationRange(int i, int b)
    {
        ActivationRange = new Tuple<int, int>(i, b);
    }

    public void ApplyVisualChange(float targetWidth, float lerpStep)
    {
        imageDimensions.sizeDelta = new Vector2(Mathf.Clamp(Mathf.Lerp(imageDimensions.sizeDelta.x, targetWidth, lerpStep), 100, 436.11f), imageDimensions.sizeDelta.y);
    }



}

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

    [SerializeField] float frequencyStartHz;
    [SerializeField] float frequencyEndHz;
    [HideInInspector] float startButtonWidth;

    [SerializeField] float barDecayValue;
    [SerializeField] float barGrowMultiplier;
    [SerializeField] float barLerpStep;

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
    [Header("Initialize the objects you desire to animate here. Needs RectTransform and AnimatedObjectInitializer components")]
    [SerializeField] private List<GameObject> objectsToAnimate = new();

    private List<AnimatedMenuElement> animatedSquares = new();


    /*
     *HOW TO USE
     *
     *ADD AnimatedObjectInitializer to objects with an image then add them into the ObjectsToAnimate list.
     * give them matchpoints, there should be no gaps between the matchpoint sequence (1,2,3,4,5 is good, 1,3,4,5 is not good.)
     *
     *
     *
     *
     *
     *
     *
     *
     *
     */





    private void Start()
    {
        InitializeAnimatedObjects();
        //foreach (AnimatedMenuElement a in animatedSquares)
        //{
        //    Debug.Log("animated element =>" + a + "with matchpoint => " + a.matchNumber);
        //}
    }





    private void InitializeAnimatedObjects()
    {
        //range - 20 to 15000
        int iteration = 0;
        int HzPerIteration;
        int amtOfObjects = objectsToAnimate.Count;

        HzPerIteration = (int)((frequencyEndHz) / amtOfObjects);
        


        foreach (GameObject each in objectsToAnimate)
        {
            if (each.GetComponent<AnimatedObjectInitializer>() == null || each.GetComponent<RectTransform>() == null)
            {
                throw new Exception("Error @ InitializeAnimatedObjects() @ AnimateGridMaterialWithSound.cs - object " + each + " has no initializer or rectTransform.");
            }

            AnimatedObjectInitializer d = each.GetComponent<AnimatedObjectInitializer>();
            Tuple<int, int> c = Tuple.Create((HzPerIteration * d.matchNumber),(HzPerIteration * d.matchNumber) + HzPerIteration);

            AnimatedMenuElement b = new AnimatedMenuElement(each, d.matchNumber);
            b.ActivationRange = c;
            animatedSquares.Add(b);
            //iteration++;


            Debug.Log(("the range of object " + each.name + " equals " + c));
            //Debug.Log(("iteration is now " + iteration + Environment.NewLine + " Hz Per Iteration is =>" + HzPerIteration + " and amount of objects is => + " + amtOfObjects));
            //  (rangeSegmentation* iteration), (rangeSegmentation* iteration) + rangeSegmentation 
        }


    }

    //we get every object
    //we put em in a list
    //we get 


    /// <summary>
    /// 1. check if sound was higher in that frequency
    /// 2. if it was, we grow the relevant bar target
    /// 3. if it was not, we decay the relevant bar target
    /// 4. on update, we lerp the bar width to the bar target
    /// </summary>


    private float GetWidthTarget(float sound, Image btn, float rangeStart, float rangeEnd)
    {

        
        if (inRange(rangeStart, rangeEnd, sound))
        {
            return sound * barGrowMultiplier;
        }
        else
        {
            return 15; //starts to decay
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
        //originalLightColor = sunLight.color;
        //originalMat = MaterialToAnimate;
        if (!audioSource)
        {
            Debug.LogError(GetType() + "AnimateGridMaterialWithSound.Awake: there was no audioSource set.");
        }
        clipSampleData = new float[sampleDataLength];


 

        soundAnalyzer.SetAudio(audioSource);



    }
    // Update is called once per frame
    void Update()
    {

        if (epilepsy || !started)
        {
            return;
        }

        GetClipLoudness();
        ShiftColor();
        float b = soundAnalyzer.AnalyzeSound();
        //and now we act accordingly
        Animate(b);

    }


    private void GetClipLoudness()
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

           
        }
    }

    private void ShiftColor()
    {
        //we get the data
        Color newColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f); //new Color((float)clipLoudness, originalMat.color.g, originalMat.color.b);
        if (clipLoudness > clipLoudnessBeatDetectionThreshold)
        {
            sunLight.color = new Color(originalLightColor.r + (clipLoudness * onBeatSunColorModifier), originalLightColor.g, originalLightColor.b);
            MaterialToAnimate.SetColor("_BaseColor", newColor);
            titleText.color = newColor;
        }
        else
        {
            sunLight.color = originalLightColor;
            MaterialToAnimate.SetColor("_BaseColor", Color.white);
            titleText.color = Color.white;
        }
    }


    private void Animate(float sound)
    {
        foreach (AnimatedMenuElement b in animatedSquares)
        { //this handles setting the target width for each match number
            b.ApplyVisualChange(GetWidthTarget(sound, b.image, b.ActivationRange.Item1, b.ActivationRange.Item2), barLerpStep);
        }
    }

    //private void AnimateMatchRange()
    //{
    //    //b_hostRect.sizeDelta = new Vector2(Mathf.Clamp(Mathf.Lerp(b_hostRect.sizeDelta.x, targetWidth_host, barLerpStep), 125, 860), b_hostRect.sizeDelta.y);
    //    //b_settingsRect.sizeDelta = new Vector2(Mathf.Clamp(Mathf.Lerp(b_settingsRect.sizeDelta.x, targetWidth_settings, barLerpStep), 125, 860), b_hostRect.sizeDelta.y);
    //    //b_creditsRect.sizeDelta = new Vector2(Mathf.Clamp(Mathf.Lerp(b_creditsRect.sizeDelta.x, targetWidth_credits, barLerpStep), 125, 860), b_hostRect.sizeDelta.y);
    //    //b_quitRect.sizeDelta = new Vector2(Mathf.Clamp(Mathf.Lerp(b_quitRect.sizeDelta.x, targetWidth_quit, barLerpStep), 125, 860), b_hostRect.sizeDelta.y);



    //}

}

