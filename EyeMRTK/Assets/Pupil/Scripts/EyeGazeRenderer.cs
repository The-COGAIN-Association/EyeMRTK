// Pupil Gaze Tracker service
//
// Originally written by Yamen Saraiji <mrayyamen@gmail.com> at May 17, 2017
// https://github.com/mrayy/PupilHMDCalibration
//
// Modified by Diako Mardanbegi <dmardanbeigi@gmail.com> at June 01, 2018
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class EyeGazeRenderer : MonoBehaviour
{
	public Canvas canvas;

    public PupilGazeTracker.GazeSource eye;
	private PupilGazeTracker _eyeTracker;

	private OutputRay output_ray;


	protected virtual bool HasEyeTracker
	{
		get
		{
			return _eyeTracker != null;
		}
	}
    // Script initialization
    void Start()
    {
		output_ray = this.GetComponentInParent<OutputRay> ();


		// Get EyeTracker unity object
		_eyeTracker = PupilGazeTracker.Instance;
		if (HasEyeTracker) {
			Debug.Log ("Failed to find pupil eye tracker, has it been added to scene?");
		}
    }

    void Update()
    {
      
        Vector2 g = PupilGazeTracker.Instance.GetEyeGaze(eye);

		//g = new Vector2 (0.5f, 0.5f);

		var rectGaze = new Vector2((g.x ) * canvas.pixelRect.width, (g.y ) * canvas.pixelRect.height);

		Ray ray = Camera.main.ScreenPointToRay (rectGaze);

	
		output_ray._ray = ray;


    }
}