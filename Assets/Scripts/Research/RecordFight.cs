using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordFight : MonoBehaviour
{
    public int recordRobotSumoCL = 0, recordRobotSumoWL = 0;
    public int maxCountMatches = 1000;
    private CSVParser _csvParser;
    string[] data = new string[2];
    private bool _isEnter = false;

    private void Start()
    {
        List<string> dataName = new List<string>();
        dataName.Add("Date");
        dataName.Add("WinCL");
        dataName.Add("WinWL");
        _csvParser = new CSVParser(dataName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isEnter)
        {
            _isEnter = true;
            RobotAgent robotAgent = other.GetComponentInParent<RobotAgent>();
            if (robotAgent.GetType() == typeof(RobotSumoAgentCL))
            {
                recordRobotSumoWL++;
                data[0] = "0";
                data[1] = "1";
                _csvParser.addData(data);
            }

            if (robotAgent.GetType() == typeof(RobotSumoAgentWL2))
            {
                recordRobotSumoCL++;
                data[0] = "1";
                data[1] = "0";
                _csvParser.addData(data);
            }

            CheckEnd();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _isEnter = false;
    }

    void CheckEnd()
    {
        if (recordRobotSumoCL + recordRobotSumoWL == maxCountMatches)
        {
            data[0] = recordRobotSumoCL.ToString();
            data[1] = recordRobotSumoWL.ToString();
            _csvParser.addData(data);
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
