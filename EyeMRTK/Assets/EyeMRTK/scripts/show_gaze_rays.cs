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


public class show_gaze_rays : MonoBehaviour {
	public ProcessGaze _processGaze;
	public float duration= 0.25f;
	public  bool ShowVisualAngle;
	public  float AngleToShowInDeg=1;


	// Use this for initialization
	void Start () {

	}
	Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}
	// Update is called once per frame
	void Update () {

		if ( _processGaze!=null) {

			Ray ray = _processGaze._ray_processed;
		
			Debug.DrawRay (ray.origin, ray.direction.normalized * 100, this.GetComponent<RayCastTrail> ().GetColor (), duration);

			// Showing 4 rays indicating the given angle in degrees
			if (ShowVisualAngle) {
				
//				Vector3 temp_direction=RotatePointAroundPivot(
//					ray.direction.normalized,ray.origin,
//					new Vector3(0, AngleToShowInDeg, 0));
//				Debug.DrawRay (ray.origin, temp_direction.normalized * 100, this.GetComponent<RayCastTrail> ().GetColor (), duration);
//
//				temp_direction=RotatePointAroundPivot(
//					ray.direction.normalized,ray.origin,
//					new Vector3(0, -AngleToShowInDeg, 0));
//				Debug.DrawRay (ray.origin, temp_direction.normalized * 100, this.GetComponent<RayCastTrail> ().GetColor (), duration);
//
//				temp_direction=RotatePointAroundPivot(
//					ray.direction.normalized,ray.origin,
//					new Vector3(AngleToShowInDeg, 0, 0));
//				Debug.DrawRay (ray.origin, temp_direction.normalized * 100, this.GetComponent<RayCastTrail> ().GetColor (), duration);
//
//				temp_direction=RotatePointAroundPivot(
//					ray.direction.normalized,ray.origin,
//					new Vector3( -AngleToShowInDeg,0, 0));
//				Debug.DrawRay (ray.origin, temp_direction.normalized * 100, this.GetComponent<RayCastTrail> ().GetColor (), duration);
//


				// intersect the angle ray with a colider and get the point
				Vector3 temp_direction=RotatePointAroundPivot(
										ray.direction.normalized,ray.origin,
					     				new Vector3(AngleToShowInDeg/2, 0, 0));
			    Debug.DrawRay (ray.origin, temp_direction.normalized * 100, this.GetComponent<RayCastTrail> ().GetColor (), duration);
					

			
			}


		}



	}
}
