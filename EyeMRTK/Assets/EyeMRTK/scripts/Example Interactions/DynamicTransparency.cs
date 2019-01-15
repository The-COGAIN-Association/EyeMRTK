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

public class DynamicTransparency : MonoBehaviour {

	public float AlphaAtClose = 0.0f;
	public float AlphaAtFar = 1.0f;
	public float DistCutoffFar = 25f;
	public float DistCutoffClose = 5;

	public enum InteractionRayType
	{
		GazeRayForInteraction,ReticleRayForInteraction,ControllerRayForInteraction
	}


	[SerializeField]
	[Tooltip("Select the Ray type that controls the transparency")]
	public  InteractionRayType RelativeTo;

	public float CurrentDistIs;

	// Use this for initialization
	void Start () {


	}

	// Update is called once per frame
	void Update () {
		
		GameObject rayObj = GameObject.Find (RelativeTo.ToString ());
		if (rayObj != null) {
			
			CurrentDistIs = Vector3.Angle (rayObj.GetComponent<InteractionRay>()._ray_smooth_fixations .direction, this.transform.position - rayObj.GetComponent<InteractionRay>()._ray_smooth_fixations.origin);

			Color color = GetComponent<Renderer> ().material.color;
			float dist_clamped = Mathf.Clamp (CurrentDistIs, DistCutoffClose, DistCutoffFar);

			color.a =( (AlphaAtFar - AlphaAtClose) / (DistCutoffFar - DistCutoffClose) ) *(dist_clamped - DistCutoffFar) + AlphaAtFar;
				

			this.GetComponentInParent<MeshRenderer> ().material.SetColor ("_Color", color);

		}

	}
}
