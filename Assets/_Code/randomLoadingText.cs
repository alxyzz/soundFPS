using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class randomLoadingText : MonoBehaviour
{
     [SerializeField]List<string> randomTips = new();
    [SerializeField] TextMeshProUGUI tooltipText;
    void Start()
    {
        tooltipText.text = randomTips[Random.Range(0, randomTips.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
