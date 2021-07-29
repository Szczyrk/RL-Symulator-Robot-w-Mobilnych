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

public class RobotSumoAgentWL2 : RobotAgent
{
    public bool IsDebug;
    public Transform Cube;
    float[] arrayFuzzy = new float[5];
    private float[] powerMotors;
    private float _maxRange, _maxRangeReflection, reward, powerMotorSum;

    public new void Start()
    {
        base.Start();
        _maxRange = sensors[2].maxRange;
        _maxRangeReflection = sensors[0].maxRange;
    }

    public new void EndEpisode()
    {
        base.EndEpisode();
        var enenmyRobot = transform.parent.GetComponentsInChildren<RobotAgent>();
        if (enenmyRobot.Length > 1)
        {
            var en = enenmyRobot.First(r => r.transform != transform);
            if (en != null)
            {
                en.AddReward(1f);
                en.EndEpisode();
            }
        }
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
                    arrayFuzzy[i] = sensors[i].distance / _maxRange;
                }
                else
                {
                    arrayFuzzy[i] = _maxRange;
                }
            }

            sensor.AddObservation(arrayFuzzy[i]);
        }

        powerMotors = motors.Select(m => m.powerMotor).ToArray();
        powerMotorSum = powerMotors.Select(m => Math.Abs(m)).Sum() / 30f;
        reward = powerMotorSum / (arrayFuzzy.Sum() + arrayFuzzy[3]);
        SetReward(reward);

        if (IsDebug)
            Debug.Log(transform.parent.name + "/" + name + " " + String.Join(", ", arrayFuzzy) + " " + powerMotorSum +
                      " " + String.Join(", ", powerMotors) + " " + reward
            );
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