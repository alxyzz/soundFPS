using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    private float timeleft;
    [HideInInspector]public Vector3 direction;
    // Start is called before the first frame update


    void Start()
    {
        timeleft = lifeTime;
    }
    // Update is called once per frame
    void Update()
    {
        if (timeleft <= 0)
        {
        Destroy(this);
        }

        timeleft -= Time.deltaTime;
        if (Vector3.Distance(transform.position, direction) > 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, direction, 0.5f);
        }
        else
        {
                Destroy(this); //todo - add object pooling for this
        }
    }
}
