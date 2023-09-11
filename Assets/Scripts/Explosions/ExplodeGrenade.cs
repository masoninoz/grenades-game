using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using System;

public class ExplodeGrenade : NetworkBehaviour
{
    
    public float explosionDelay;

    public CustomNetworkManager manager;

    public GameObject parentPlayer;

    public float explosionPower;

    public float maxRange;

    public float heightMultiplier;

    public float maxHeight;

    public GameObject explosion;

    void Start(){
        Physics.IgnoreCollision(this.gameObject.GetComponent<CapsuleCollider>(), parentPlayer.GetComponent<CapsuleCollider>(), true);
    }
    
    void Update()
    {
        explosionDelay  -= Time.deltaTime;
        if(explosionDelay <= 0){
            
            Debug.Log(hasAuthority);
            CmdExplosion();
        }
    }

    [Command(requiresAuthority=false)]
    void CmdExplosion(){
        Debug.Log("Is a command");
        RPCExplosion();
        Debug.Log("What is going on?");
    }

    [ClientRpc]
    public void RPCExplosion(){

        Debug.Log("Is Run");
        
        foreach (GameObject player in manager.Players){
            //Calculate difference in position
            //I can use the formula for intesnity to calculate the estimated
            //force at a certain distance away from the player

            //Just need to calculate the length of the hypotenuse and use that as the radius from the explosion instance

            //Get the difference in position, the last vector that is added is to remove potential issues caused by 0 values
            
            //Modified the player location from closest location to center of mass, this would cause players standing on a grenade
            //To fly upwards, and decreasing chances of infinte velocities. Technically worse, but it could be better overall
            Vector3 differenceInLocation = player.GetComponent<Transform>().localPosition - this.gameObject.transform.localPosition;
        
            Debug.Log(differenceInLocation);
            //I = (P)/4*Pi * r


            Vector3 explosionVelocity = new Vector3((1/(differenceInLocation.x)) * explosionPower, (1/differenceInLocation.y) * explosionPower, (1/differenceInLocation.z) * explosionPower);
            
            //The logic for this is hard to explain. In essence, the explosion velocity is currently a hyperbola
            //This code modifies the hyperbola to have certain end points, where it hits the axis' and so, there
            //can be a max range. For further explanation please graph
            //y = 1/(x + (1/a)) and
            //y = (1/x) - (1/a) and look at the first quadrant

            //Some issue arrises during these calculations, logical issues

            //Further thought has provoked this idea. If the player is super close in all x, y and z directions, 
            //Then they should be blasted off strongly in those directions, however if the player is super far on
            //Lets say the z axis, but close on the x axis, all of the force should be applied on the z axis not
            //the x

            //Hence trig is necessary to determine the force ratios between the different axis.

            //Find ratio between x and z
            //Current issue in relation to the positive x values, causing the player to be pulled towards the grenade

            float horizontalAngle = (float) Math.Atan(differenceInLocation.x/differenceInLocation.z); 

            Debug.Log(horizontalAngle + " horizontal angle");
            
            //Pythag to figure out hypotenuse without using trig
            float horizontalHypotenuse = (float) Math.Sqrt(Math.Pow(differenceInLocation.x, 2) + Math.Pow(differenceInLocation.z, 2));

            float verticalAngle = (float) Math.Atan(differenceInLocation.y/horizontalHypotenuse);

            //Pythag to figure out hypotenuse
            float verticalHypotenuse = (float) Math.Sqrt(Math.Pow(differenceInLocation.y, 2) + Math.Pow(horizontalHypotenuse, 2));

            float feltExplosionPower = (1/(verticalHypotenuse + (1/maxRange))) * explosionPower - (1/maxRange);

           explosionVelocity.y = feltExplosionPower * (float) Math.Sin(verticalAngle) * heightMultiplier;

           if(explosionVelocity.y > maxHeight){
            explosionVelocity.y = maxHeight;
           }

           float feltHorizontalHypotenuse = feltExplosionPower * (float) Math.Cos(verticalAngle);


        if(differenceInLocation.x < 0){
           explosionVelocity.x = -feltHorizontalHypotenuse * (float) Math.Sin(horizontalAngle);
        } else {
            explosionVelocity.x = feltHorizontalHypotenuse * (float) Math.Sin(horizontalAngle);
        }

        if(differenceInLocation.z < 0){
           explosionVelocity.z = -feltHorizontalHypotenuse * (float) Math.Cos(horizontalAngle);
        } else {
            explosionVelocity.z = feltHorizontalHypotenuse * (float) Math.Cos(horizontalAngle);
        }

        if(differenceInLocation.x < 0 && differenceInLocation.z > 0){
            Debug.Log("Is both negative");
            explosionVelocity.x = feltHorizontalHypotenuse * (float) Math.Sin(horizontalAngle);
            explosionVelocity.z = feltHorizontalHypotenuse * (float) Math.Cos(horizontalAngle);
        }

        

           Debug.Log(explosionVelocity + " is the explosionVelocity");

            player.GetComponent<PlayerController>().isTouchingGround = false;
            

            Vector3 additionalHeight = new Vector3(0f, (1/horizontalHypotenuse) * explosionPower, 0f);
           
           
            if(explosionVelocity.y >= maxHeight){
                additionalHeight = new Vector3(0f, 0f, 0f);
            }
            
            player.GetComponent<Rigidbody>().velocity += explosionVelocity + additionalHeight;

            player.GetComponent<PlayerController>().airVelocity = player.GetComponent<Rigidbody>().velocity;

            player.GetComponent<PlayerController>().isItTouchingGround();

            
            
        }

        GameObject Explosion = Instantiate(explosion);
        Explosion.transform.localPosition = this.gameObject.transform.localPosition;
        Explosion.transform.Find("ExplosionParticles").localScale += new Vector3(maxRange/20, maxRange/20, maxRange/20);
        Explosion.transform.Find("Smoke").localScale += new Vector3(maxRange/20, maxRange/20, maxRange/20);
        Explosion.GetComponent<DestroyExplosion>().initialIntensity = maxRange * 100;
        NetworkServer.Spawn(Explosion);

        Destroy(this.gameObject);
        
    }
}
