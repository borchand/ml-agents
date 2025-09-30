using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System;

public class Ball3DAgent : Agent
{
    [Header("Specific to Ball3D")]
    public GameObject ball;
    [Tooltip("Whether to use vector observation. This option should be checked " +
        "in 3DBall scene, and unchecked in Visual3DBall scene. ")]
    public bool useVecObs;
    Rigidbody m_BallRb;
    EnvironmentParameters m_ResetParams;

    TargetBehavior _behavior = TargetBehavior.ALTERNATECORNER;

    public override void Initialize()
    {
        m_BallRb = ball.GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVecObs)
        {
            var corner = GetCorner(StepCount / 200 % 4);
            sensor.AddObservation(gameObject.transform.rotation.z);
            sensor.AddObservation(gameObject.transform.rotation.x);
            sensor.AddObservation(ball.transform.position - gameObject.transform.position);
            sensor.AddObservation(m_BallRb.linearVelocity);
            sensor.AddObservation(corner);
            sensor.AddObservation(StepCount / 200);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var actionZ = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        var actionX = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);

        if ((gameObject.transform.rotation.z < 0.25f && actionZ > 0f) ||
            (gameObject.transform.rotation.z > -0.25f && actionZ < 0f))
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 1), actionZ);
        }

        if ((gameObject.transform.rotation.x < 0.25f && actionX > 0f) ||
            (gameObject.transform.rotation.x > -0.25f && actionX < 0f))
        {
            gameObject.transform.Rotate(new Vector3(1, 0, 0), actionX);
        }

        // we dont care about y axis as we already know its above the platform
        var ballPos = new Vector2(ball.transform.position.x, ball.transform.position.z);
        var agentPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);

        switch (_behavior)
        {
            case TargetBehavior.NORMAL:
                if ((ball.transform.position.y - gameObject.transform.position.y) < -2f ||
                    Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f ||
                    Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
                {
                    SetReward(-1f);
                    EndEpisode();
                }
                else
                {
                    SetReward(0.1f);
                }
                break;
            case TargetBehavior.CENTER:

                if ((ball.transform.position.y - gameObject.transform.position.y) < -2f ||
                    Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f ||
                    Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
                {
                    SetReward(-1f);
                    EndEpisode();
                }
                else
                {
                    float distToCenter = Vector2.Distance(ballPos, agentPos);

                    if (distToCenter < 0.5f)
                    {
                        SetReward(1.0f);
                    }
                    else
                    {
                        SetReward(0.1f);
                    }
                }
                break;
            case TargetBehavior.ONECORNER:
                if ((ball.transform.position.y - gameObject.transform.position.y) < -2f ||
                    Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f ||
                    Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
                {
                    SetReward(-1f);
                    EndEpisode();
                }
                else
                {
                    float distToCorner = Vector2.Distance(ballPos, agentPos + new Vector2(1.5f, 1.5f));

                    if (distToCorner < 0.5f)
                    {
                        SetReward(1.0f);
                    }
                    else
                    {
                        SetReward(0.1f);
                    }
                }

                break;
            case TargetBehavior.ALTERNATECORNER:
                if ((ball.transform.position.y - gameObject.transform.position.y) < -2f ||
                    Mathf.Abs(ball.transform.position.x - gameObject.transform.position.x) > 3f ||
                    Mathf.Abs(ball.transform.position.z - gameObject.transform.position.z) > 3f)
                {
                    SetReward(-1f);
                    EndEpisode();
                }
                else
                {
                    var cornerIndex = StepCount / 200 % 4;

                    var corner = GetCorner((int)cornerIndex);

                    float distToAlternateCorner = Vector2.Distance(ballPos, agentPos + corner);

                    SetReward(4.5f - distToAlternateCorner);
                }
                break;
        }

    }

    private Vector2 GetCorner(int cornerIndex)
    {
        switch (cornerIndex)
        {
            case 0:
                return new Vector2(1.5f, 1.5f);
            case 1:
                return new Vector2(1.5f, -1.5f);
            case 2:
                return new Vector2(-1.5f, -1.5f);
            case 3:
                return new Vector2(-1.5f, 1.5f);
            default:
                return Vector2.zero;
        }
    }

    public override void OnEpisodeBegin()
    {
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        gameObject.transform.Rotate(new Vector3(1, 0, 0), Random.Range(-10f, 10f));
        gameObject.transform.Rotate(new Vector3(0, 0, 1), Random.Range(-10f, 10f));
        m_BallRb.linearVelocity = new Vector3(0f, 0f, 0f);
        ball.transform.position = new Vector3(Random.Range(-1.5f, 1.5f), 4f, Random.Range(-1.5f, 1.5f))
            + gameObject.transform.position;
        //Reset the parameters when the Agent is reset.
        SetResetParameters();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = -Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public void SetBall()
    {
        //Set the attributes of the ball by fetching the information from the academy
        m_BallRb.mass = m_ResetParams.GetWithDefault("mass", 1.0f);
        var scale = m_ResetParams.GetWithDefault("scale", 1.0f);
        ball.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetResetParameters()
    {
        SetBall();
    }

    enum TargetBehavior
    {
        NORMAL,
        CENTER,
        ONECORNER,
        ALTERNATECORNER,
    }

}
