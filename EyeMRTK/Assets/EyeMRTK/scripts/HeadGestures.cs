//	MIT License
//
//	Copyright (c) 2018 Diako Mardanbegi
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//	of this software and associated documentation files (the "Software"), to deal
//	in the Software without restriction, including without limitation the rights
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//	copies of the Software, and to permit persons to whom the Software is
//	furnished to do so, subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all
//	copies or substantial portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//	SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HeadGestureType
{
	HeadUp ,
	HeadDown,
	HeadRight,
	HeadLeft
}


public class HeadGestures : MonoBehaviour {


	public int MinimulTimeBetweenGestures = 1;// in sec
	private int timeRemaining;
    bool DetectionDisabled = false;

	public void TimerBegin()
	{
		if (!DetectionDisabled) {
			DetectionDisabled = true;
			timeRemaining = MinimulTimeBetweenGestures;
			Invoke ( "TimerTick",1 );
		}
	}

	private void TimerTick() {
		timeRemaining--;
		if(timeRemaining > 0) {
			Invoke ( "TimerTick",1);
		} else {
			DetectionDisabled = false;
		}
	}


	public Vector3 FrameVelocity;
	private Vector3 PrevPosition { get; set; }

	public int threshold = 25;

	public bool HeadUp = false;
	public bool HeadDown = false;
	public bool HeadRight = false;
	public bool HeadLeft = false;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		// Keep an average velocity due to fixed update irregularity, else we will occassionally get 0 velocity
		Vector3 currFrameVelocity = (transform.rotation.eulerAngles - PrevPosition) / Time.deltaTime;
		FrameVelocity = Vector3.Lerp (FrameVelocity, currFrameVelocity, 0.3f);
		PrevPosition = transform.rotation.eulerAngles;

		if (!DetectionDisabled) {

			if ((FrameVelocity.y < -threshold)) {
				FireGestureStatus(HeadGestureType.HeadLeft,  true);
				FireGestureStatus(HeadGestureType.HeadRight,  false);
				TimerBegin ();

			} else if ((FrameVelocity.y > threshold)) {
				FireGestureStatus(HeadGestureType.HeadLeft,  false);
				FireGestureStatus(HeadGestureType.HeadRight,  true);
				TimerBegin ();
			} else {
				FireGestureStatus(HeadGestureType.HeadLeft,  false);
				FireGestureStatus(HeadGestureType.HeadRight,  false);
			}


			if ((FrameVelocity.x > threshold)) {
				FireGestureStatus(HeadGestureType.HeadDown,  true);
				FireGestureStatus(HeadGestureType.HeadUp,  false);
				TimerBegin ();
			} else if ((FrameVelocity.x < -threshold)) {
				FireGestureStatus(HeadGestureType.HeadDown,  false);
				FireGestureStatus(HeadGestureType.HeadUp,  true);
				TimerBegin ();
			} else {
				FireGestureStatus(HeadGestureType.HeadDown,  false);
				FireGestureStatus(HeadGestureType.HeadUp,  false);
			}
		}
	}

	void FireGestureStatus(HeadGestureType gesture, bool status)
	{
		
		if (gesture == HeadGestureType.HeadDown) {
			if (status == HeadDown)
				return;// Do not fire event again if the status has not changed
		else
				HeadDown = status;
		}
		if (gesture == HeadGestureType.HeadUp) {
			if (status == HeadUp)
				return;// Do not fire event again if the status has not changed
			else
				HeadUp = status;
		}
		if (gesture == HeadGestureType.HeadRight) {
			if (status == HeadRight)
				return;// Do not fire event again if the status has not changed
			else
				HeadRight = status;
		}
		if (gesture == HeadGestureType.HeadLeft) {
			if (status == HeadLeft)
				return;// Do not fire event again if the status has not changed
			else
				HeadLeft = status;
		}

		// Fire
		GazeInteractionEventArgs e = new GazeInteractionEventArgs ();
		e.HeadGestureName = gesture;
		e.HeadGestureStatus = status;
		GazeInteractionEventManager.instance.FireHeadGesture (e);

	}

}
