using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeatHUD : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private RectTransform _crosshair;
    //[SerializeField] private RectTransform _beatArrow_start_left;
    //[SerializeField] private RectTransform _beatArrow_start_right;
    //[SerializeField] private RectTransform _beatArrow_left;
    //[SerializeField] private RectTransform _beatArrow_right;
    [SerializeField] public Image _beatSimpleIndicator;
    bool beat = true;




    private Vector2 leftArrowStartPos, rightArrowStartPos, CenterPos;


    

    public void DoBeatTick()
    {
        _beatSimpleIndicator.gameObject.SetActive(false);
        StartCoroutine(showBeat());
    }

    IEnumerator showBeat()
    {
        _beatSimpleIndicator.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(0.2f);

        _beatSimpleIndicator.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        //leftArrowStartPos = _beatArrow_left.position;
        //rightArrowStartPos = _beatArrow_right.position;
        CenterPos = _crosshair.position;
        _beatSimpleIndicator.gameObject.SetActive(false);
        //Debug.Log("leftArrowStartPos - " + leftArrowStartPos);
        //Debug.Log("rightArrowStartPos - " + rightArrowStartPos);
        //Debug.Log("CenterPos - " + CenterPos);
    }

    // Update is called once per frame
    void Update()
    {
    }





    IEnumerator MoveOnBeatLeft()
    {
        yield return new WaitForSecondsRealtime(1f);
    }

    IEnumerator MoveOnBeatRight()
    {
        yield return new WaitForSecondsRealtime(1f);
    }


    void ResetArrowPos()
    {

    }

}
