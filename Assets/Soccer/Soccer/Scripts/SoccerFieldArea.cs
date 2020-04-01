using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class PlayerState
{
    public int playerIndex;
    [FormerlySerializedAs("agentRB")]
    public Rigidbody agentRb;
    public Vector3 startingPos;
    public AgentSoccer agentScript;
    public float ballPosReward;
}

public class SoccerFieldArea : MonoBehaviour
{
    public GameObject ball;
    [FormerlySerializedAs("ballRB")]
    [HideInInspector]
    public Rigidbody ballRb;
    public GameObject ground;
    public GameObject centerPitch;
    SoccerBallController m_BallController;
    public List<PlayerState> playerStates = new List<PlayerState>();
    [HideInInspector]
    public Vector3 ballStartingPos;
    public GameObject goalTextUI;
    [HideInInspector]
    public bool canResetBall;
    Material m_GroundMaterial;
    Renderer m_GroundRenderer;
    SoccerAcademy m_Academy;
    FSM finite;
    //int blueScore = 0;
   // int purpleScore = 0;

    public IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time);
        m_GroundRenderer.material = m_GroundMaterial;
    }

    void Awake()
    {
        finite = FindObjectOfType<FSM>();
        m_Academy = FindObjectOfType<SoccerAcademy>();
        m_GroundRenderer = centerPitch.GetComponent<Renderer>();
        m_GroundMaterial = m_GroundRenderer.material;
        canResetBall = true;
        if (goalTextUI) { goalTextUI.SetActive(false); }
        ballRb = ball.GetComponent<Rigidbody>();
        m_BallController = ball.GetComponent<SoccerBallController>();
        m_BallController.area = this;
        ballStartingPos = ball.transform.position;
    }

    IEnumerator ShowGoalUI()
    {
        if (goalTextUI) goalTextUI.SetActive(true);
        yield return new WaitForSeconds(.25f);
        if (goalTextUI) goalTextUI.SetActive(false);
    }

    public void AllPlayersDone(float reward)
    {
        foreach (var ps in playerStates)
        {
            if (ps.agentScript.gameObject.activeInHierarchy)
            {
                if (reward != 0)
                {
                    ps.agentScript.AddReward(reward);//update reward by incrementing new reward with exising reward
                }
                ps.agentScript.Done();
            }
        }
    }

    public void GoalTouched(AgentSoccer.Team scoredTeam)
    {
        foreach (var ps in playerStates)
        {

            if (scoredTeam == AgentSoccer.Team.Blue)
            {
                RewardOrPunishPlayer(ps, m_Academy.PlayerReward);//reward agent if it scores
                StartCoroutine(GoalScoredSwapGroundMaterial(m_Academy.blueMaterial, 1));//higlight centre spot as blue
                m_Academy.blueScore++;//increment the score of blue by 1
            }
            else
            {
                RewardOrPunishPlayer(ps, m_Academy.PlayerPunish);//punish agent if opponent scores
                StartCoroutine(GoalScoredSwapGroundMaterial(m_Academy.purpleMaterial, 1));//higlight centre spot as purple
                m_Academy.purpleScore++;//increment the score of purple by 1
            }
            if (goalTextUI)
            {
                StartCoroutine(ShowGoalUI());
            }
            Debug.Log("Blue Score = " + m_Academy.blueScore + "             Purple Score = " + m_Academy.purpleScore);//Display Score
        }
    }

    public void RewardOrPunishPlayer(PlayerState ps, float player)//add a reward or a punishment to the agent
    {
        if (ps.agentScript.agentRole == AgentSoccer.AgentRole.Player)
        {
            ps.agentScript.AddReward(player);
        }
        ps.agentScript.Done(); 
    }

    public Vector3 GetRandomSpawnPos(AgentSoccer.AgentRole role, AgentSoccer.Team team)//reset agent positon
    {
        var randomSpawnPos = ground.transform.position +
            new Vector3(-7f, 0f, 0f)
            + (Random.insideUnitSphere * 2);
        randomSpawnPos.y = ground.transform.position.y + 2;
        return randomSpawnPos;
    }

    public Vector3 GetBallSpawnPosition()//reset ball position anywhere in center circle
    {
        var randomSpawnPos = ground.transform.position +
            new Vector3(0f, 0f, 0f)
            + (Random.insideUnitSphere * 2);
        {
            randomSpawnPos.y = ground.transform.position.y + 2;
        };
        return randomSpawnPos;
    }

    public void ResetScene()//reset the scene after a team scores
    {
        ball.transform.position = GetBallSpawnPosition();//reset ball
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        var ballScale = m_Academy.resetParameters["ball_scale"];//size of ball
        ballRb.transform.localScale = new Vector3(ballScale, ballScale, ballScale);

        finite.Reset();//reset finite state machine
    }
}
