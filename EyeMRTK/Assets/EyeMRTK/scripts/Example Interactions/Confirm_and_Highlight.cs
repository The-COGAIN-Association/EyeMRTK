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

public class Confirm_and_Highlight : MonoBehaviour {


	public enum PointingWith
	{
		Gaze,
		Reticle,
		Laser,
		CustomRay
	}
	public PointingWith pointingWith;

	public enum ConfirmWith
	{
		Dwell,
		HeadUp,
		HeadDown,
		HeadRight,
		HeadLeft,
		Trigger,
		MouseLeftClick
	}


	public ConfirmWith confirmWith;




	// The material to use for active objects.
	public Material _highlightMaterial;

	// The previous material.
	private Material OriginalObjectMaterial=null;

	private GazeInteractionGeneric _gazeInteractionGeneric;
	// Use this for initialization
	void Start () {
		_gazeInteractionGeneric = this.GetComponentInParent<GazeInteractionGeneric> ();
		if (_gazeInteractionGeneric == null)
			Debug.LogError ("Hightlight feature requires the object to have a GazeInteractionGeneric componenet");

		MeshRenderer renderer = gameObject.GetComponentInParent<MeshRenderer>();
		if (renderer != null)
			OriginalObjectMaterial = renderer.material;

	}

	// Update is called once per frame
	void Update () {


		bool fixation =
			(pointingWith == PointingWith.Gaze && _gazeInteractionGeneric.pointingStatus._GazeIn) |
			(pointingWith == PointingWith.Reticle && _gazeInteractionGeneric.pointingStatus._ReticleIn) |
			(pointingWith == PointingWith.Laser && _gazeInteractionGeneric.pointingStatus._LaserIn)|
			(pointingWith == PointingWith.CustomRay && _gazeInteractionGeneric.pointingStatus._CustomRayIn) ;

		bool confirmation = false;
		if (confirmWith == ConfirmWith.Dwell) {
			if(
			(pointingWith == PointingWith.Gaze && _gazeInteractionGeneric.confirmationStatus._Gaze_DwellProgress==1) |
			(pointingWith == PointingWith.Reticle && _gazeInteractionGeneric.confirmationStatus._Reticle_DwellProgress==1) |
			(pointingWith == PointingWith.Laser && _gazeInteractionGeneric.confirmationStatus._Laser_DwellProgress==1) |
				(pointingWith == PointingWith.CustomRay && _gazeInteractionGeneric.confirmationStatus._CustomRay_DwellProgress==1)
			)
			confirmation=true;
		}else if ( (confirmWith == ConfirmWith.HeadDown && _gazeInteractionGeneric.confirmationStatus._HeadDown) |
			(confirmWith == ConfirmWith.HeadUp && _gazeInteractionGeneric.confirmationStatus._HeadUp) |
			(confirmWith == ConfirmWith.HeadRight && _gazeInteractionGeneric.confirmationStatus._HeadRight) |
			(confirmWith == ConfirmWith.HeadLeft && _gazeInteractionGeneric.confirmationStatus._HeadLeft) |
			(confirmWith == ConfirmWith.MouseLeftClick && _gazeInteractionGeneric.confirmationStatus._MouseLeftClick) |
			(confirmWith == ConfirmWith.Trigger && _gazeInteractionGeneric.confirmationStatus._TriggerPressed) )

			confirmation=true;



		if (fixation  && confirmation)
			OnGazeHighlight ();
		else 
			ResetOnGazeHighlight ();



	}

	void ResetOnGazeHighlight()
	{

		var renderer = this.GetComponent<MeshRenderer>();
		if (renderer != null)

			renderer.material = OriginalObjectMaterial;




	}
	void OnGazeHighlight()
	{

		MeshRenderer renderer = this.GetComponent<MeshRenderer>();
		if (renderer != null)

			renderer.material = _highlightMaterial;




	}
}
