﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvLoader : MonoBehaviour  {

	public static List<GameObject> environmentsFound = new List<GameObject>();

	public static void SearchForEnvironments() {
		environmentsFound.Clear();
		GameObject[] envs = Resources.LoadAll<GameObject>("Environments/");
		foreach (GameObject e in envs) {
			environmentsFound.Add(e);
		}
	}
	
	public static GameObject LoadEnvironment(string name) {
		foreach (GameObject e in environmentsFound) {
			if (e.name == name) {
				Transform env = e.transform.Spawn();
				env.position = e.transform.position;
				return env.gameObject;
			}
		}
		Debug.LogError(name + " environment not found.");
		return null;
	}

}
