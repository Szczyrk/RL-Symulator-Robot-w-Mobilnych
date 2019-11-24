﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotLoader {


	public static List<GameObject> robotsFound = new List<GameObject>();
	
	
	public static void SearchForRobots() {
		robotsFound.Clear();
		GameObject[] robots = Resources.LoadAll<GameObject>("Robots/");
		foreach (GameObject r in robots) {
			if (r.GetComponent<Robot>()) {
				robotsFound.Add(r);
			}
		}
	}

	public static Robot LoadRobot(int index) {
		Transform robot = robotsFound[index].transform.Spawn();
		return robot.GetComponent<Robot>();
	}
	
	public static Robot LoadRobot(string name) {
		foreach (GameObject r in robotsFound) {
			if (r.name == name) {
				Transform robot = r.transform.Spawn();
                robot.name = robot.name.Replace("(Clone)", "");
                return robot.GetComponent<Robot>();
			}
		}
		Debug.LogError(name + " robot not found.");
		return null;
	}

    public static GameObject LoadRobotGameObject(string name)
    {
        return LoadRobot(name).gameObject;
    }
}
