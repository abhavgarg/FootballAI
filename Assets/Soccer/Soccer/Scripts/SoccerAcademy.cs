using UnityEngine;
using MLAgents;

public class SoccerAcademy : Academy
{
    public Material purpleMaterial;
    public Material blueMaterial;
    public float gravityMultiplier = 1;

    public float agentRunSpeed;
    public float PlayerPunish;
    public float PlayerReward;
    public int blueScore = 0;
    public int purpleScore = 0;


    void Start()
    {
        Physics.gravity *= gravityMultiplier;
    }
    public override void AcademyReset()
    {
        Physics.gravity = new Vector3(0, -resetParameters["gravity"], 0);
    }
}
