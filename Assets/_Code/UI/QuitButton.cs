using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuitButton : MonoBehaviour, IPointerEnterHandler
{
    public MainMenuUI menuRef;
    public RectTransform quitbutton;
    public List<RectTransform> quitWayPoints;
    private int quitTravelled = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("t");
        if (quitTravelled < quitWayPoints.Count)
        {
            gameObject.transform.position = quitWayPoints[quitTravelled].position;
            quitTravelled++;
        }

        if (quitTravelled == 2)
        {
            menuRef.SkillIssue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
