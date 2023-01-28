using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerMind : NetworkBehaviour
{
    public int ID;

    public PlayerBody Body;




    public int GetPenultimateAttacker()
    {

        return Body.penultimateAttacker.ID;
    }



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
