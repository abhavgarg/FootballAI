using UnityEngine;
using MLAgents;

public class AgentSoccer : Agent
{
    public enum Team
    {
        Purple,
        Blue
    }
    public enum AgentRole
    {
        Player
    }

    public Team team;
    public AgentRole agentRole;
    int m_PlayerIndex;
    public SoccerFieldArea area;

    [HideInInspector]
    public Rigidbody agentRb;
    SoccerAcademy m_Academy;
    Renderer m_AgentRenderer;
    RayPerception m_RayPer;

    float[] m_RayAngles = { 0f, 45f, 90f, 135f, 180f, 110f, 70f}; //angles the agent can see
    string[] m_DetectableObjectsBlue = { "ball", "blueGoal", "purpleGoal",
                                         "wall", "blueAgent", "purpleAgent" };// gameobjects to be detected by the agent



    public override void InitializeAgent()
    {
        base.InitializeAgent();
        m_AgentRenderer = GetComponentInChildren<Renderer>();
        m_RayPer = GetComponent<RayPerception>();
        m_Academy = FindObjectOfType<SoccerAcademy>();
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;

        var playerState = new PlayerState
        {
            agentRb = agentRb,
            startingPos = transform.position,
            agentScript = this,
        };
        area.playerStates.Add(playerState);
        m_PlayerIndex = area.playerStates.IndexOf(playerState);
        playerState.playerIndex = m_PlayerIndex;
    }

    public override void CollectObservations()
    {
        var rayDistance = 30f; //length of the rays of the agent's vision
        string[] detectableObjects;
        detectableObjects = m_DetectableObjectsBlue;
        AddVectorObs(m_RayPer.Perceive(rayDistance, m_RayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(m_RayPer.Perceive(rayDistance, m_RayAngles, detectableObjects, 1f, 0f));
    }

    public void MoveAgent(float[] act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = Mathf.FloorToInt(act[0]);

        //possible actions carried out by the agent during trianing
        if (agentRole == AgentRole.Player)
        {
  
            switch (action)
            {
                case 1:
                    dirToGo = transform.forward * 1f;
   
                    break;
                case 2:
                    dirToGo = transform.forward * -1f;
                    break;
                case 3:
                    rotateDir = transform.up * 1f;
                    break;
                case 4:
                    rotateDir = transform.up * -1f;
                    break;
                case 5:
                    dirToGo = transform.right * -0.75f;
                    break;
                case 6:
                    dirToGo = transform.right * 0.75f;
                    break;
            }
        }
        transform.Rotate(rotateDir, Time.deltaTime * 100f);//agent rotation about an axis in a given time
        agentRb.AddForce(dirToGo * m_Academy.agentRunSpeed,
            ForceMode.VelocityChange);//agent speed in a particular direction
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Existential reward if the opponent does not score
        if (agentRole == AgentRole.Player)
        {
            AddReward(1f / 3000f);
        }
        MoveAgent(vectorAction);
    }
    public override void AgentReset() // reset agent after any team scores a goal
    {
        transform.position = area.GetRandomSpawnPos(agentRole, team);
        transform.rotation = Quaternion.Euler(0f, 90f, 0f); 
        agentRb.velocity = Vector3.zero;
        agentRb.angularVelocity = Vector3.zero;
        SetResetParameters();
    }

    public void SetResetParameters()
    {
        area.ResetScene();
    }
}
