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

public class RobotSumoAgentVsCubeWL : RobotAgent
{
    public bool IsDebug;
    public Transform Cube;
    float[] arrayFuzzy = new float[5];
    private float _maxRange, _maxRangeReflection;

    public new void Start()
    {
        base.Start();
        _maxRange = sensors[2].maxRange;
        _maxRangeReflection = sensors[0].maxRange;
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
            if (sensors[i].GetType() == typeof(ReflectionSensor))
            {
                if (sensors[i].distance < _maxRangeReflection)
                {
                    arrayFuzzy[i] = 1;
                }
                else
                {
                    arrayFuzzy[i] = 0;
                } 
            }
            else
            {
                if (sensors[i].distance < _maxRange)
                {
                    arrayFuzzy[i] = sensors[i].distance;
                }
                else
                {
                    arrayFuzzy[i] = _maxRange;
                }
            }

            sensor.AddObservation(arrayFuzzy[i]);
        }


        int tmp = 0;
        if (arrayFuzzy[2] < _maxRange && arrayFuzzy[3] == _maxRange && arrayFuzzy[4] == _maxRange && motors[1].powerMotor > 20f)
        {
            tmp = 1;
            SetReward(0.1f);
        }
        else if (arrayFuzzy[2] == _maxRange && arrayFuzzy[3] == _maxRange && arrayFuzzy[4] < _maxRange && motors[0].powerMotor > 20f)
        {
            tmp = 2;
            SetReward(0.1f);
        }
        else if (arrayFuzzy[2] == _maxRange && arrayFuzzy[3] < _maxRange && arrayFuzzy[4] == _maxRange && motors[0].powerMotor > 20f &&
                 motors[1].powerMotor > 20f)
        {
            tmp = 3;
            SetReward(0.1f);
        }
        else if (arrayFuzzy[2] < _maxRange/2 && arrayFuzzy[3] < _maxRange/2 && arrayFuzzy[4] < _maxRange/2 && motors[0].powerMotor > 20f &&
                 motors[1].powerMotor > 20f)
        {
            tmp = 4;
            SetReward(0.1f);
        }
        if(IsDebug)
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