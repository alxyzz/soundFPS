using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun_anim_event : MonoBehaviour
{
    public void StopFiring()
    {
        GetComponent<Animator>().SetBool(0, false);
    }
}
