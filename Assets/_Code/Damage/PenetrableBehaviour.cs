using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetrableBehaviour : MonoBehaviour
{
    [SerializeField] protected PenetrableMaterial _material;
    [SerializeField]
    [Range(0.0f,1.0f)]
    protected float _thicknessMultiplier = 1.0f;
}
