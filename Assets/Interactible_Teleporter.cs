using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class Interactible_Teleporter : MonoBehaviour
{
    public GameObject partner;
    // Start is called before the first frame update
    void Start()
    {
        if (partner == null)
        {
            string path = GetGameObjectPath(gameObject);
            Debug.LogError("Teleporter without an exit point @ " + path);
            
        }




    }


   public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CharacterController c = other.GetComponent<CharacterController>();
            c.enabled = false;

            other.transform.position = partner.transform.position;


            c.enabled = true;

        }
    }


    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
     if(obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
