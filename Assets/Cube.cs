using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    
    public RobotAgent robotSumoAgentVsCube;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ResetTrigger>(out ResetTrigger destroyTrigger))
        {
            robotSumoAgentVsCube.SetReward(1f);
            robotSumoAgentVsCube.EndEpisode();
        }
    }
}
