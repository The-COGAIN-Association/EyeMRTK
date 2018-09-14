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
using System;

[Serializable]
public class PointingStatus {

	public bool _GazeIn;
	public bool _LaserIn;
	public bool _ReticleIn;

}

[Serializable]
public class ConfirmationStatus {



	public float _Gaze_DwellTime = 500.0f;
	public float _Gaze_DwellProgress = 0; 
	public float _Laser_DwellTime = 500.0f;
	public float _Laser_DwellProgress = 0; 
	public float _Reticle_DwellTime = 500.0f;
	public float _Reticle_DwellProgress = 0; 


	public bool _MouseLeftClick;
	public bool _MouseRightClick;
	public bool _MouseMiddleClick;

	public bool _TriggerPressed;

	public bool _HeadDown;
	public bool _HeadUp;
	public bool _HeadRight;
	public bool _HeadLeft;


}



public class GazeInteractionGeneric : MonoBehaviour {

	 int _Gaze_Starttime;
	 int _Gaze_FrameLossCount;
	 int _Laser_Starttime;
	 int _Laser_FrameLossCount;
	 int _Reticle_Starttime;
	 int _Reticle_FrameLossCount;


	public int AcceptableFrameLossCount=5;


	public PointingStatus pointingStatus;
	public ConfirmationStatus confirmationStatus;

	// Use this for initialization
	void Start () {


		GazeInteractionEventManager.instance.RayEnter += Handle_RayEnter;
		GazeInteractionEventManager.instance.RayExit += Handle_RayExit;
		GazeInteractionEventManager.instance.RayIn += Handle_RayIn;
		GazeInteractionEventManager.instance.HeadGesture += Handle_HeadGesture;




	}
	void OnDestroy()
	{
		GazeInteractionEventManager.instance.RayEnter -= Handle_RayEnter;
		GazeInteractionEventManager.instance.RayExit -= Handle_RayExit;
		GazeInteractionEventManager.instance.RayIn -= Handle_RayIn;
		GazeInteractionEventManager.instance.HeadGesture -= Handle_HeadGesture;

	}


	// Update is called once per frame
	void Update () {


		// catching confirmations commands from mouse and controller
		confirmationStatus._MouseLeftClick = Input.GetMouseButton (0);
		confirmationStatus._MouseRightClick = Input.GetMouseButton (1);
		confirmationStatus._MouseMiddleClick = Input.GetMouseButton (2);


		try{
			// We don't care about right and left here
			bool leftTrigger = SteamVR_Controller.Input (SteamVR_Controller.GetDeviceIndex (SteamVR_Controller.DeviceRelation.Leftmost)).GetHairTrigger ();
			bool rightTrigger = SteamVR_Controller.Input (SteamVR_Controller.GetDeviceIndex (SteamVR_Controller.DeviceRelation.Rightmost)).GetHairTrigger ();
			confirmationStatus._TriggerPressed = leftTrigger | rightTrigger;
		}
		catch(Exception e)
		{}

		// Dwell
		if (pointingStatus._GazeIn)
		{

			_Gaze_FrameLossCount = 0;
			//progress dwell timer
			float prog = (Environment.TickCount - _Gaze_Starttime) / confirmationStatus._Gaze_DwellTime;
			confirmationStatus._Gaze_DwellProgress = Mathf.Clamp (prog,0,1);

		}
		else
		{
			

			if (_Gaze_FrameLossCount < AcceptableFrameLossCount)
			{
				//progress dwell timer
				float prog = (Environment.TickCount - _Gaze_Starttime) / confirmationStatus._Gaze_DwellTime;
				confirmationStatus._Gaze_DwellProgress = Mathf.Clamp (prog,0,1);
				_Gaze_FrameLossCount ++;
			}
			else
			{

				_Gaze_Starttime = Environment.TickCount;
				confirmationStatus._Gaze_DwellProgress = 0;

			}
		}

		if (pointingStatus._LaserIn)
		{
			_Laser_FrameLossCount = 0;
			//progress dwell timer
			float prog = (Environment.TickCount - _Laser_Starttime) / confirmationStatus._Laser_DwellTime;
			confirmationStatus._Laser_DwellProgress = Mathf.Clamp (prog,0,1);
		}
		else
		{
			

			if (_Laser_FrameLossCount < AcceptableFrameLossCount)
			{
				//progress dwell timer
				float prog = (Environment.TickCount - _Laser_Starttime) / confirmationStatus._Laser_DwellTime;
				confirmationStatus._Laser_DwellProgress = Mathf.Clamp (prog,0,1);
				_Laser_FrameLossCount ++;
			}
			else
			{

				_Laser_Starttime = Environment.TickCount;
				confirmationStatus._Laser_DwellProgress = 0;
			}
		}

		if (pointingStatus._ReticleIn)
		{
			_Reticle_FrameLossCount = 0;
			//progress dwell timer
			float prog = (Environment.TickCount - _Reticle_Starttime) / confirmationStatus._Reticle_DwellTime;
			confirmationStatus._Reticle_DwellProgress = Mathf.Clamp (prog,0,1);
		}
		else
		{
			

			if (_Reticle_FrameLossCount < AcceptableFrameLossCount)
			{
				//progress dwell timer
				float prog = (Environment.TickCount - _Reticle_Starttime) / confirmationStatus._Reticle_DwellTime;
				confirmationStatus._Reticle_DwellProgress = Mathf.Clamp (prog,0,1);
				_Reticle_FrameLossCount ++;
			}
			else
			{

				_Reticle_Starttime = Environment.TickCount;
				confirmationStatus._Reticle_DwellProgress = 0;
			}
		}

	}



	void Handle_RayEnter(object sender, GazeInteractionEventArgs e){

		if (e.Raycast_GameObject.gameObject == this.gameObject) {
			if (e.Raycast_RayName == "GazeRayForInteraction") {
				pointingStatus._GazeIn = true;

			} else if (e.Raycast_RayName == "LaserRayForInteraction") {
				pointingStatus._LaserIn = true;

			} else if (e.Raycast_RayName == "ReticleRayForInteraction") {
				pointingStatus._ReticleIn = true;
			}

		}
	}
	void Handle_RayExit(object sender, GazeInteractionEventArgs e){
		if (e.Raycast_GameObject.gameObject == this.gameObject) {
			if (e.Raycast_RayName == "GazeRayForInteraction") {


				pointingStatus._GazeIn = false;

			} else if (e.Raycast_RayName == "LaserRayForInteraction") {
				pointingStatus._LaserIn = false;

			} else if (e.Raycast_RayName == "ReticleRayForInteraction") {
				pointingStatus._ReticleIn = false;
			}
		}
	}
	void Handle_RayIn(object sender, GazeInteractionEventArgs e){
		if (e.Raycast_GameObject.gameObject == this.gameObject) {
			if (e.Raycast_RayName == "GazeRayForInteraction") {

				pointingStatus._GazeIn = true;

	
			} else if (e.Raycast_RayName == "LaserRayForInteraction") {
				pointingStatus._LaserIn = true;

			} else if (e.Raycast_RayName == "ReticleRayForInteraction") {
				pointingStatus._ReticleIn = true;
		
			}
		}
	}

	void Handle_HeadGesture(object sender, GazeInteractionEventArgs e){
		
		if (e.HeadGestureName == HeadGestureType.HeadDown)
			confirmationStatus._HeadDown = e.HeadGestureStatus;


		if (e.HeadGestureName == HeadGestureType.HeadUp)
			confirmationStatus._HeadUp = e.HeadGestureStatus;
	


		if (e.HeadGestureName == HeadGestureType.HeadRight)
			confirmationStatus._HeadRight = e.HeadGestureStatus;
	

		if (e.HeadGestureName == HeadGestureType.HeadLeft)
			confirmationStatus._HeadLeft = e.HeadGestureStatus;
		



	}
}
