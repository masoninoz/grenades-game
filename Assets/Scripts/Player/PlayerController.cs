using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerController : NetworkBehaviour
{
    public float xRotation;
    public float yRotation;

    public float walkSpeed = 2;

    public float runSpeed = 4;

    public float airSpeed = 0.5f;

    float movementSpeed;

    public float sensitivity = 2f;

    public bool isTouchingGround = false;

    public bool isJumping = false;

    public List<Collider> groundObjects = new List<Collider>();

    public List<Collider> wallObjects = new List<Collider>();

    public float jumpHeight;

    public Vector3 motionVector;

    public float minimumYHeight;

    public Vector3 airVelocity = new Vector3();

    void Start(){
        walkSpeed = walkSpeed/30;
        runSpeed = runSpeed/30;
    }


    // Update is called once per frame
    void Update()
    {
        if (hasAuthority)
        {
            Movement();
        }

    }

    void FixedUpdate(){
        if(hasAuthority){
        this.transform.localPosition += motionVector;
        }
    }

    void Movement(){
        Rigidbody rigidbody = this.gameObject.GetComponent<Rigidbody>();

            xRotation += -Input.GetAxis("Mouse Y") * sensitivity;

            yRotation += Input.GetAxis("Mouse X") * sensitivity;

            if (xRotation > 90)
            {
                xRotation = 90;
            }
            else if (xRotation < -90)
            {
                xRotation = -90;
            }
            
            this.GetComponent<Transform>().rotation = Quaternion.Euler(0f, yRotation, 0f);
            this.GetComponent<Transform>().Find("LocalCamera").rotation = Quaternion.Euler(xRotation, this.GetComponent<Transform>().rotation.eulerAngles.y, 0f);

            //Player motion
            double xVelocity = 0;
            double zVelocity = 0;

            //Checks if player is running or not

            if(Input.GetKey(KeyCode.LeftShift)){
                movementSpeed = runSpeed;
            } else {
                movementSpeed = walkSpeed;
            }
            
            if(!isTouchingGround){
                movementSpeed = walkSpeed;
            }
          


            //Goes through each key, checks if it's an acceptable key and then modifies the velocity values to show that
            
            //Prevents players from moving forwards and backwards in the air, this is done to reduce air control
            //And to prevent the players from completely overriding the explosion velocity
            //if(isTouchingGround){
            if(Input.GetKey(KeyCode.W)){
                    xVelocity = (double)movementSpeed * Math.Sin(yRotation* Math.PI/180);
                    zVelocity = (double)movementSpeed * Math.Cos(yRotation * Math.PI/180);
            }
            if(Input.GetKey(KeyCode.S)){
                    xVelocity = (double)-movementSpeed * Math.Sin(yRotation* Math.PI/180);
                    zVelocity = (double)-movementSpeed * Math.Cos(yRotation * Math.PI/180);
            }
            //}
            //The left and right motions are 
            if(Input.GetKey(KeyCode.A)){
                xVelocity += (double)movementSpeed * Math.Sin((yRotation - 90)* Math.PI/180);
                zVelocity += (double)movementSpeed * Math.Cos((yRotation - 90) * Math.PI/180);
            }
            if(Input.GetKey(KeyCode.D)){
                xVelocity += (double)movementSpeed * Math.Sin((yRotation + 90)* Math.PI/180);
                zVelocity += (double)movementSpeed * Math.Cos((yRotation + 90) * Math.PI/180);
            }

            
            motionVector = new Vector3((float)xVelocity, 0f, (float)zVelocity); //The y motion is just to attempt to counteract the upwards force produced by the players motion
            
            if(!isTouchingGround && !isJumping && wallObjects.Count > 0 && rigidbody.velocity.y > -1f){
                motionVector.y = -0.05f;
                Debug.Log("Go down ya bastard!");
            }

            if(Input.GetKey(KeyCode.Space) && isTouchingGround){
                //Using the suvat equation: V^2 = U^2 + 2as
                //And using the jump height as 's', gravity as 'a' and 'v' as 0 (Apex of jump), we can find the necessary upwards velocity
                float upwardsVelocity = (float) Math.Sqrt((double)(-2 * Physics.gravity.y * jumpHeight));
                
                rigidbody.velocity = new Vector3(0f, upwardsVelocity, 0f); 
                isTouchingGround = false;
                isJumping = true;
            }

            if(rigidbody.velocity.y <= 0 && isJumping) {
                isJumping = false;
                isItTouchingGround();
            }

           

            if(isTouchingGround){
            rigidbody.velocity = new Vector3(0f, rigidbody.velocity.y, 0f);
            } else {
                rigidbody.velocity = new Vector3(airVelocity.x, rigidbody.velocity.y, airVelocity.z); //Subtracting the velocity from the previous frame prevents infinite velocity
            }
        }

    private void OnCollisionEnter(Collision collision){
        airVelocity = new Vector3(0f, 0f, 0f);

        if((collision.GetContact(0).point.y-this.transform.localPosition.y) < minimumYHeight){
            isTouchingGround = true;
            
            if(!groundObjects.Contains(collision.collider)){
                groundObjects.Add(collision.collider);
            }
        } else {
            if(!wallObjects.Contains(collision.collider)){
                wallObjects.Add(collision.collider);
            }

        }

    }

    public void isItTouchingGround(){
        if(groundObjects.Count > 0){
            isTouchingGround = true;
        } else {
            isTouchingGround = false;
            airVelocity = this.gameObject.GetComponent<Rigidbody>().velocity;
        }
    }

    private void OnCollisionExit(Collision collision){
        
        if(groundObjects.Contains(collision.collider)){
                groundObjects.Remove(collision.collider);
        }
        if(wallObjects.Contains(collision.collider)){
            wallObjects.Remove(collision.collider);
        }
        if(groundObjects.Count == 0){
            isTouchingGround = false;
            airVelocity = this.gameObject.GetComponent<Rigidbody>().velocity;
        }
    }

}
