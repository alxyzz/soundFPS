using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BeatHUD : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private RectTransform _crosshair;
    [SerializeField] private RectTransform _beatArrow_start_left;
    [SerializeField] private RectTransform _beatArrow_start_right;
    [SerializeField] private RectTransform _beatArrow_left;
    [SerializeField] private RectTransform _beatArrow_right;
    [SerializeField] private Image _beatSimpleIndicator;
    bool beat = true;




    private Vector2 leftArrowStartPos, rightArrowStartPos, CenterPos;




    public void OnBeat()
    {
        _beatSimpleIndicator.gameObject.SetActive(false);
        StartCoroutine(showBeat());
    }

    IEnumerator showBeat()
    {
        _beatSimpleIndicator.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(0.1f);

        _beatSimpleIndicator.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        leftArrowStartPos = _beatArrow_left.position;
        rightArrowStartPos = _beatArrow_right.position;
        CenterPos = _crosshair.position;

        Debug.Log("leftArrowStartPos - " + leftArrowStartPos);
        Debug.Log("rightArrowStartPos - " + rightArrowStartPos);
        Debug.Log("CenterPos - " + CenterPos);
    }

    // Update is called once per frame
    void Update()
    {
        //if (beat)
        //{
        //    if ((_beatArrow_left.anchoredPosition - _crosshair.anchoredPosition).x >  5f)
        //    {
        //        _beatArrow_left.anchoredPosition = Vector2.MoveTowards(_beatArrow_left.anchoredPosition, _crosshair.anchoredPosition, 1f);
        //    }
        //    else
        //    {
        //        _beatArrow_left.anchoredPosition = leftArrowStartPos;
        //    }





        //    //if (_beatArrow_right.anchoredPosition.x > _beatArrow_right.anchoredPosition.x - 100)
        //    //{
        //    //    _beatArrow_right.anchoredPosition = new Vector2(_beatArrow_right.anchoredPosition.x - 1f, _beatArrow_right.anchoredPosition.y);
        //    //}
        //    //else
        //    //{
        //    //    _beatArrow_right.anchoredPosition = rightArrowStartPos;
        //    //}
        //}
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
