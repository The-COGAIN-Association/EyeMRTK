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

	public enum Ray_To_Show
	{
		Raw,
		_ray_smooth_all,
		_ray_smooth_fixations,
	}

	[SerializeField]
	public Ray_To_Show _ray_To_Show= Ray_To_Show.Raw;


	public float duration= 0.25f;
	public  bool ShowVisualAngle;
	public  float AngleToShowInDeg=1;

	private static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
	{
		float angle = 0f;
		Quaternion rot = Quaternion.LookRotation(forward, up);
		Vector3 lastPoint = Vector3.zero;
		Vector3 thisPoint = Vector3.zero;

		for (int i = 0; i < segments + 1; i++)
		{
			thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
			thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

			if (i > 0)
			{
				Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
			}

			lastPoint = thisPoint;
			angle += 360f / segments;
		}
	}

	// Use this for initialization
	void Start () {

	}
	Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		return Quaternion.Euler(angles) * (point - pivot) + pivot;
	}


	private  Ray GetRay()
	{

		OutputRay _outputRay = this.transform.GetComponentInParent<OutputRay> ();
		if( _ray_To_Show==Ray_To_Show.Raw)
			return _outputRay._ray_raw ; 
		if( _ray_To_Show==Ray_To_Show._ray_smooth_all)
			return _outputRay._ray_smooth_all ; 
		if( _ray_To_Show==Ray_To_Show._ray_smooth_fixations)
			return _outputRay._ray_smooth_fixations ; 


		return default(Ray);
	}

	// Update is called once per frame
	void Update () {


		Ray ray = GetRay ();
		
			Debug.DrawRay (ray.origin, ray.direction.normalized * 100, this.GetComponent<RayCastTrail> ().GetColor (), duration);

			// Showing 4 rays indicating the given angle in degrees
			if (ShowVisualAngle) {


				float dist = 0.7f;
				Vector3 temp_direction=RotatePointAroundPivot(
					ray.direction,ray.origin,
					new Vector3(0, AngleToShowInDeg/2.0f, 0));
				
//				Debug.DrawRay (ray.origin, temp_direction.normalized * dist, this.GetComponent<RayCastTrail> ().GetColor (), duration);

				Ray _tmp_ray = new Ray (ray.origin,temp_direction);
				float _r = Vector3.Distance (ray.GetPoint(dist), _tmp_ray.GetPoint(dist));


				// I don't know why this is needed?!!
				float _correction=1.44f;
				_r = _r / _correction;
				DrawEllipse (ray.GetPoint(dist),ray.direction,Vector3.up,_r,_r,100,Color.yellow,0.01f  );


			


		}



	}
}
