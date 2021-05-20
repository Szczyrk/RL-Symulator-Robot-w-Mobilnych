using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class ResearchDataSumoSave : MonoBehaviour
{
    public RobotAgent _robot;
    private CSVParser _csvParser;
    string[] data = new string[7];
    void Start()
    {
        List<string> dataName = new List<string>();
        dataName.Add("Date");
        dataName.Add("V1");
        dataName.Add("V2");
        dataName.Add("S1");
        dataName.Add("S2");
        dataName.Add("S3");
        dataName.Add("S4");
        dataName.Add("S5");
        _csvParser = new CSVParser(dataName);
        StartCoroutine(SaveData());
    }

    private IEnumerator SaveData()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            data[0] = _robot.motors[0].powerMotor.ToString();
            data[1] = _robot.motors[1].powerMotor.ToString();
            data[2] = _robot.sensors[0].distance.ToString();
            data[3] = _robot.sensors[1].distance.ToString();
            data[4] = _robot.sensors[2].distance.ToString();
            data[5] = _robot.sensors[3].distance.ToString();
            data[6] = _robot.sensors[4].distance.ToString();
            _csvParser.addData(data.ToList());
        }
    }
}
