using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class MakeImage : NetworkBehaviour
{

    [SerializeField] public GameObject square;

    public void makeSquare() {
        Debug.Log("Button pressed");
        CmdMakeSquare();
    }

    [Command(requiresAuthority = false)]
    public void CmdMakeSquare() {
        RPCMakeSquare();
    }

    [ClientRpc]
    public void RPCMakeSquare(){
        Debug.Log("The square is being made");
        GameObject sqre = Instantiate(square);
        sqre.transform.parent = GameObject.Find("Canvas").transform;
        sqre.transform.localPosition = new Vector3(-400f, -150f, 0f);
        NetworkServer.Spawn(sqre);
    }
  
}
