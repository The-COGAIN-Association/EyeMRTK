﻿// Pupil Gaze Tracker service
//
// Originally written by Yamen Saraiji <mrayyamen@gmail.com> at May 17, 2017
// https://github.com/mrayy/PupilHMDCalibration
//
// Modified by Diako Mardanbegi <dmardanbeigi@gmail.com> at June 01, 2018
//

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PupilCalibMarker : MonoBehaviour
{

    RectTransform _transform;
    Image _image;
    bool _started = false;
    float x, y;

    // Use this for initialization
    void Start()
    {

        _transform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _image.enabled = false;

        PupilGazeTracker.Instance.OnCalibrationStarted += OnCalibrationStarted;
        PupilGazeTracker.Instance.OnCalibrationDone += OnCalibrationDone;
        PupilGazeTracker.Instance.OnCalibData += OnCalibData;
    }

    void OnCalibrationStarted(PupilGazeTracker m)
    {
        _started = true;
    }

    void OnCalibrationDone(PupilGazeTracker m)
    {
        _started = false;
    }

    void OnCalibData(PupilGazeTracker m, float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    void _SetLocation(float x, float y)
    {
        Canvas c = _transform.GetComponentInParent<Canvas>();
        if (c == null)
            return;
		Vector3 pos = new Vector3((x -0.5f) * c.pixelRect.width, (y-0.5f ) * c.pixelRect.height, 0);
        _transform.localPosition = pos;
    }
    // Update is called once per frame
    void Update()

    {




        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_started)
            {
                PupilGazeTracker.Instance.StopCalibration();/**/

            }
            else
            {
                PupilGazeTracker.Instance.StartCalibration();
                
            }
        }
        _image.enabled = _started;
        if (_started)
            _SetLocation(x, y);



    }
}
