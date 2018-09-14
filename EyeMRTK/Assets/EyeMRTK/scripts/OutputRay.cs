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


public class  OutputRay: MonoBehaviour {

	public Ray _ray;

	//!!!  The first (default value) must be None otherwise it couses a infinite loop. 
	public enum InteractionRayType
	{
		None ,
		GazeRayForInteraction,
		ReticleRayForInteraction,
		LaserRayForInteraction
	}


	[SerializeField]
	[Tooltip("Make sure that each option is chosen not more than once in the entire scene")]
	public  InteractionRayType UseThisAs;
	private  InteractionRayType original_UseAs;



	private GazeInteractionEventArgs e;



	private void CreateInteractionRayObject()
	{
		if (GameObject.Find (UseThisAs.ToString ()) != null)
			Debug.LogError (UseThisAs.ToString () + " is set by multiple gaze sources in the scene");
		else {
			GameObject newObj = new GameObject (UseThisAs.ToString ());
			//Add Components
			newObj.AddComponent<InteractionRay> ();

		}
		original_UseAs = UseThisAs;
	}
	private void UpdateInteractionRayObject()
	{

		Ray ray_used_for_interaction = _ray;
		ProcessGaze _processedRay = this.GetComponentInParent<ProcessGaze> ();
		if (_processedRay!=null)
			ray_used_for_interaction = _processedRay._ray_processed;


		GameObject interactionRayObject = GameObject.Find(UseThisAs.ToString());
		if (interactionRayObject != null)
			interactionRayObject.GetComponent<InteractionRay> ()._ray = ray_used_for_interaction;
	}

	private void Start()
	{
		if (UseThisAs != InteractionRayType.None) {

				CreateInteractionRayObject();
			
		}

		 original_UseAs = UseThisAs;


	}

	private void Update()
	{
		if ((original_UseAs!=UseThisAs)  && (UseThisAs != InteractionRayType.None))
			CreateInteractionRayObject();
		
		UpdateInteractionRayObject ();
			


	}




}
