using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{

    public float speed = 3f;
    public SoccerFieldArea area;

    private enum State { MoveToBall = 0, MoveToGoal = 1, DefendPlayer = 2};
    private State aiState = State.MoveToBall;

    GameObject Ball;
    GameObject PurpleGoal;
    GameObject BlueGoal;
    void Start()
    {
        Ball = GameObject.Find("Soccer Ball");
        PurpleGoal = GameObject.Find("GoalPurple");
        BlueGoal = GameObject.Find("GoalBlue");
    }

    void FixedUpdate()
    {
        Rigidbody rigidBodyComp = GetComponent<Rigidbody>();
        transform.LookAt(Ball.transform); //always start by looking at ball and determining position of ball
        BaseMovementAndAnimation();
        switch (aiState)
        {
            case State.MoveToBall:
                Move();
                break;
            case State.MoveToGoal:
                Goal();
                break;
            case State.DefendPlayer:
                Defend();               
                break;
            default:
                break;
        }

        

    }

    private void BaseMovementAndAnimation()
    {
        Rigidbody rigidBodyComp = GetComponent<Rigidbody>();
        if (rigidBodyComp.transform.rotation.y > -180f && rigidBodyComp.transform.rotation.y < 0f)
            
        {
            aiState = State.MoveToBall; // if ball is ahead of player

            if(this.transform.position.x - Ball.transform.position.x < 0.015  && this.transform.position.z - Ball.transform.position.z < 0.015 )
            {
                aiState = State.MoveToGoal; // if difference of ground position of ball to ground position of player < 0.015 (Ball is close to player)
            }
        }

        else  if ((rigidBodyComp.transform.rotation.y >= 0f && rigidBodyComp.transform.rotation.y <= 180f))
        {
           aiState = State.DefendPlayer; // if ball is behind the player
            if (rigidBodyComp.transform.position.x > 15.2f) //if player has reached its own goal it will look towards the ball and defend the corner of the goal
            {
                aiState = State.MoveToBall;
            }

        }
    }


    void Move ()
    {
        Rigidbody rigidBodyComp = GetComponent<Rigidbody>();
        rigidBodyComp.velocity = transform.forward * speed; //move towards ball
    }

    void Goal()
    {
        Rigidbody rigidBodyComp = GetComponent<Rigidbody>();
        transform.LookAt(BlueGoal.transform.position);
        rigidBodyComp.velocity = transform.forward * speed; //move towards goal with ball

    }

    void Defend()
    {
        Rigidbody rigidBodyComp = GetComponent<Rigidbody>();
        transform.LookAt(PurpleGoal.transform.position);
        rigidBodyComp.velocity =  transform.forward * (speed + 1); //move towards own goal to defend with a higher speed
        
    }

   

    public void Reset() //called after any team scores to reset the scene
    {
        Rigidbody rigidBodyComp = GetComponent<Rigidbody>();
        rigidBodyComp.transform.position = new Vector3(7f, 0.5f, 0f); //initial position of the player
        rigidBodyComp.velocity = Vector3.zero;
        rigidBodyComp.angularVelocity = Vector3.zero;
    }

}
