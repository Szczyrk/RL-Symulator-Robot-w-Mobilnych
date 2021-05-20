using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class RobotSumoAgentVsCubeCL : RobotAgent
{
    public Transform Cube;
    private ClassicalLogic _classicalLogic;
    int[] arrayFuzzy = new int[5];

    public new void Start()
    {
        base.Start();
        _classicalLogic = new ClassicalLogic(0f, sensors[2].maxRange, 3);
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        Reset();
        if (Cube != null)
        {
            Cube.localPosition = new Vector3(Random.Range(-0.2f, 0.7f), 0.1f, Random.Range(-0.6f, 0.3f));
            Cube.localRotation = Random.rotation;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        
        for (int i = 0; i < sensors.Length; i++)
        {
            arrayFuzzy[i] = _classicalLogic.Fuzzy(sensors[i].distance);
            sensor.AddObservation(arrayFuzzy[i]);
        }

        int tmp = 0;
        if (arrayFuzzy[2] < 3 && arrayFuzzy[3] == 3 && arrayFuzzy[4] == 3 && motors[1].powerMotor > 20f)
        {
            tmp = 1;
            SetReward(0.1f);
        }
        else if (arrayFuzzy[2] == 3 && arrayFuzzy[3] == 3 && arrayFuzzy[4] < 3 && motors[0].powerMotor > 20f)
        {
            tmp = 2;
            SetReward(0.1f);
        }
        else if (arrayFuzzy[2] == 3 && arrayFuzzy[3] < 3 && arrayFuzzy[4] == 3 && motors[0].powerMotor > 20f &&
                 motors[1].powerMotor > 20f)
        {
            tmp = 3;
            SetReward(0.1f);
        }

        Debug.Log(tmp + " " + name + " " + String.Join(", ", arrayFuzzy) + " " +
                  String.Join(", ", motors.Select(m => m.powerMotor)));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        for (int i = 0; i < motors.Length; i++)
        {
            motors[i].powerMotor = actions.ContinuousActions[i] * 30f;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continusActions = actionsOut.ContinuousActions;
        ManualControl();
        for (int i = 0; i < motors.Length; i++)
        {
            Debug.Log(motors[i].name + " " + motors[i].powerMotor);
            continusActions[i] = motors[i].powerMotor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ResetTrigger>(out ResetTrigger destroyTrigger))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public void AddReward(float value)
    {
        SetReward(value);
    }
}