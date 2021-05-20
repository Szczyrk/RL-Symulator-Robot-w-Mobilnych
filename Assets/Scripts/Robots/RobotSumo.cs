using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotSumo : Robot
{
    public override void OnUpdate()
    {
        if (sensors[0].detected)
        {
            motors[0].powerMotor = -15f;
            motors[1].powerMotor = -20f;
        }
        else if (sensors[1].detected)
        {
            motors[0].powerMotor = -20f;
            motors[1].powerMotor = -15f;
        }
        else if(sensors[4].detected)
        {
            motors[0].powerMotor = 20f;
            motors[1].powerMotor = 20f;
        }
        else
        {
            motors[0].powerMotor = 15f;
            motors[1].powerMotor = -15f;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ResetTrigger>(out ResetTrigger destroyTrigger))
        {
            var enenmyRobot = transform.parent.GetComponentsInChildren<RobotAgent>().First(r => r.transform != transform);
            if (enenmyRobot != null)
            {
                enenmyRobot.AddReward(1f);
                enenmyRobot.EndEpisode();
            }
            Reset();
        }
    }
}
