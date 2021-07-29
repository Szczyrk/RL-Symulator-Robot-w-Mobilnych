using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordFight : MonoBehaviour
{
    public int recordRobotSumo1 = 0, recordRobotSumo2 = 0;
    public int maxCountMatches = 1000;
    private CSVParser _csvParser;
    string[] data = new string[2];
    private bool _isEnter = false;
    public string robotAgent1;
    public string robotAgent2;
    public string nameRobotAgent1;
    public string nameRobotAgent2;

    private void Start()
    {
        List<string> dataName = new List<string>();
        dataName.Add("Date");
        dataName.Add("Win" + nameRobotAgent1);
        dataName.Add("Win" + nameRobotAgent2);
        _csvParser = new CSVParser(dataName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isEnter)
        {
            _isEnter = true;
            RobotAgent robotAgent = other.GetComponentInParent<RobotAgent>();
            if (robotAgent.CompareTag(robotAgent1))
            {
                recordRobotSumo2++;
                data[0] = "0";
                data[1] = "1";
                _csvParser.addData(data);
            }

            if (robotAgent.CompareTag(robotAgent2))
            {
                recordRobotSumo1++;
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
        if (recordRobotSumo1 + recordRobotSumo2 == maxCountMatches)
        {
            data[0] = recordRobotSumo1.ToString();
            data[1] = recordRobotSumo2.ToString();
            _csvParser.addData(data);
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
