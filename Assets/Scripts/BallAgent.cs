using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;
using Random = UnityEngine.Random;

public class BallAgent : Agent
{
    private float bounds = 6f;
    public float ballSpeed = 10f;
    public float maxAngle = 30f;
    public float rotationSpeed = 0.5f;

    public GameObject platform;


    private float rotationX = 0f;
    private float rotationY = 0f;
    private float rotationZ = 0f;
    private float rotationTargetX = 0f;
    private float rotationTargetY = 0f;
    private float rotationTargetZ = 0f;

    Rigidbody rBody;
    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(platform.transform.rotation.eulerAngles);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(rBody.velocity);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * ballSpeed);

        // Fell off platform
        if (transform.localPosition.y < -bounds ||
            Math.Abs(transform.localPosition.x - platform.transform.localPosition.x) > bounds ||
            Math.Abs(transform.localPosition.z - platform.transform.localPosition.z) > bounds)
        {
            AddReward(-1f);
            EndEpisode();
        } else
        {
            AddReward(0.1f);
        }
    }


    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    private void ResetPosition()
    {
        // platform
        platform.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

        // ball
        transform.localPosition = new Vector3(0, 0.5f, 0);
        rBody.angularVelocity = Vector3.zero;
        rBody.velocity = Vector3.zero;
    }

    public void SetResetParameters()
    {
        ResetPosition();
    }

    private void FixedUpdate()
    {
        var nextX = rotationX;
        var nextY = rotationY;
        var nextZ = rotationZ;

        var deltaX = Math.Abs(Mathf.DeltaAngle(nextX, rotationTargetX));
        if (deltaX < 0.5f || Mathf.Approximately(deltaX, 0.5f))
        {
            rotationTargetX = Random.Range(-maxAngle, maxAngle);
        }
        else if (nextX < rotationTargetX)
        {
            nextX += 1f * rotationSpeed;
        }
        else
        {
            nextX -= 1f * rotationSpeed;
        }

        var deltaY = Math.Abs(Mathf.DeltaAngle(nextY, rotationTargetY));
        if (deltaY < 0.5f || Mathf.Approximately(deltaY, 0.5f))
        {
            rotationTargetY = Random.Range(-maxAngle, maxAngle);
        }
        else if (nextY < rotationTargetY)
        {
            nextY += 1f * rotationSpeed;
        }
        else
        {
            nextY -= 1f * rotationSpeed;
        }

        var deltaZ = Math.Abs(Mathf.DeltaAngle(nextZ, rotationTargetZ));
        if (deltaZ < 0.5f || Mathf.Approximately(deltaZ, 0.5f))
        {
            rotationTargetZ = Random.Range(-maxAngle, maxAngle);
        }
        else if (nextZ < rotationTargetZ)
        {
            nextZ += 1f * rotationSpeed;
        }
        else
        {
            nextZ -= 1f * rotationSpeed;
        }


        platform.transform.rotation = Quaternion.Euler(nextX, nextY, nextZ);
        rotationX = nextX;
        rotationY = nextY;
        rotationZ = nextZ;
    }
}
