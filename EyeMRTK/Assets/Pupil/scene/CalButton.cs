using UnityEngine;
using System.Collections;

public class CalButton : MonoBehaviour {
public PupilGazeTracker pgt;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.C)){
            print("c key was pressed");
			
			pgt.StartCalibration();
		}
	}
}
