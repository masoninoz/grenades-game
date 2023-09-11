using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System;
using UnityEngine.SceneManagement;

public class ThrowGrenade : NetworkBehaviour
{
    public CustomNetworkManager manager;
    public GameplayManager gameplayManager;

    public GameObject Grenade;

    public Vector3 throwOffset;

    public double throwVelocity;

    void Start(){
        gameplayManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
        manager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name == "TestScene"){
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            Debug.Log("Hey stop pressing that!");
            if(hasAuthority){
                throwGrenade((double) -this.gameObject.GetComponent<PlayerController>().xRotation, (double) this.gameObject.GetComponent<PlayerController>().yRotation, this.gameObject);
            }
        }
        }
    }

    

    [Command]
    void throwGrenade(double xRotation, double yRotation, GameObject player){
        RPCThrowGrenade(xRotation, yRotation);
    }

    [ClientRpc]
    void RPCThrowGrenade(double xRotation, double yRotation){

        Debug.Log("X rotation: " + xRotation);
        Debug.Log("Y rotation: " + yRotation);

        //Convert the rotations into radians
        xRotation = xRotation * Math.PI/180;
        yRotation = yRotation * Math.PI/180;

        //Instantiate the prefab
        GameObject grenade = Instantiate(Grenade);

        grenade.GetComponent<ExplodeGrenade>().parentPlayer = this.gameObject;

        Debug.Log(this.manager + " is the manager");

        grenade.GetComponent<ExplodeGrenade>().manager = this.manager;


        //Set it's position to be at the player
        grenade.transform.localPosition = this.gameObject.transform.localPosition + throwOffset;

        //Calculate upwards velocity, using a triangle that is standing up, emmited from the player
        //The hypotenuse of the triangle being the throw velocity 
        double yVelocity = throwVelocity * Math.Sin(xRotation);

        //Find the hypotenuse of the x and z velocities
        double xzHypotenuse = throwVelocity * Math.Cos(xRotation);

        //Find the x velocity
        double xVelocity = xzHypotenuse * Math.Sin(yRotation);

        //Find the z velocity
        double zVelocity = xzHypotenuse * Math.Cos(yRotation);
        
        grenade.GetComponent<Rigidbody>().velocity = new Vector3((float) xVelocity, (float) yVelocity, (float) zVelocity);

        //Spawn grenade
        NetworkServer.Spawn(grenade, this.gameObject.GetComponent<NetworkIdentity>().connectionToClient);
    }
}
