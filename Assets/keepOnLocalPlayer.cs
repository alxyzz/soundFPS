using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class keepOnLocalPlayer : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(this);
        }
    }


}
