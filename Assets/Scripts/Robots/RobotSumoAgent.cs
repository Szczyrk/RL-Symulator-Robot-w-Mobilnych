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

public class RobotSumoAgent : RobotAgent
{
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        Reset();
        var enenmyRobot = transform.parent.GetComponentsInChildren<Robot>();
        if (enenmyRobot.Length > 0)
        {
            enenmyRobot.First(r => r.transform != transform).Reset();
        }
        foreach (var motor in motors)
        {
            motor.powerMotor = UnityEngine.Random.Range(-20f, 20f);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        foreach (var sensorRobot in sensors)
        {
            sensor.AddObservation(sensorRobot.detected);
        }
        foreach (var motor in motors)
        {
            sensor.AddObservation(motor.powerMotor);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        for(int i = 0; i< motors.Length; i++)
        {
            motors[i].powerMotor = actions.ContinuousActions[i] * 30f;
        }
        if(motors[0].powerMotor < 0 && motors[1].powerMotor < 0)
            SetReward(-0.001f);
        if(motors[0].powerMotor > 20f || motors[1].powerMotor > 20f)
                    SetReward(0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continusActions = actionsOut.ContinuousActions;
        ManualControl();
        for(int i = 0; i< motors.Length; i++)
        {
            Debug.Log(motors[i].name+" "+motors[i].powerMotor);
            continusActions[i] = motors[i].powerMotor;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ResetTrigger>(out ResetTrigger destroyTrigger))
        {
            SetReward(-1f);
            var enenmyRobot = transform.parent.GetComponentsInChildren<RobotSumoAgent>();
            if (enenmyRobot.Length > 0)
            {
                var robotSumoAgent =enenmyRobot.First(r => r.transform != transform);
                robotSumoAgent.EndEpisode();
                robotSumoAgent.SetReward(+1f);
            }
            EndEpisode();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Robot>(out Robot robot) && sensors[4].detected)
        {
            SetReward(+0.1f);
        }
    }

    public void AddReward(float value)
    {
        SetReward(value);
    }
}
