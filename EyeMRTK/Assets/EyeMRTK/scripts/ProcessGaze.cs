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




public class ProcessGaze : MonoBehaviour
{


	public GameObject RaySource;

	// smoothing parameters
	const int MAXHISTORY = 150; // 90 would be about one second
	[SerializeField]
	[Tooltip("smooting winodw")]
	[Range(1, MAXHISTORY)]
	public int windowSize = 10;
	[SerializeField]
	[Tooltip("saccade minimum velocity")]
	[Range(0.0f, 5.0f)]
	public float SACCADE_VEL_THRESHOLD = 1.5f; // visual angle to detect saccade

//	[SerializeField]
//	[Tooltip("saccade minimum accelaration")]
//	[Range(0.0f, 3.0f)]
//	public float SACCADE_ACC_THRESHOLD = 0.5f; // visual angle to detect saccade
//


	// dont use from outside
	List<Vector3> history_tmp = new List<Vector3>();
	// dont use from outside
	List<Vector3> history_2_tmp = new List<Vector3>();

	List<Vector3> history_smooth_fixations = new List<Vector3>();
	List<Vector3> history_smooth_all = new List<Vector3>();
	List<Vector3> history_origin = new List<Vector3>();


	List<float> history_vel = new List<float>();

	bool _is_in_saccade=false;
	int _ray_saccade_onset_index=0;

//	Vector3 _latest_origin=Vector3.zero;



	private void Update()
	{
		if (RaySource!=null && RaySource.GetComponent<OutputRay>()!=null)
		{
			
			ProcessData(RaySource.GetComponent<OutputRay> ()._ray_raw);
		}

	}



	private void UpdateOutputRay()
	{
		RaySource.GetComponent<OutputRay> ()._ray_smooth_fixations = new Ray(history_origin [history_origin.Count - 1], history_smooth_fixations[history_smooth_fixations.Count-1]);
		RaySource.GetComponent<OutputRay> ()._ray_smooth_all = new Ray(history_origin [history_origin.Count - 1],history_smooth_all[history_smooth_all.Count-1] );
		RaySource.GetComponent<OutputRay> ()._is_in_saccade = _is_in_saccade;

		if (_ray_saccade_onset_index == 0 && history_smooth_fixations.Count > 2)
		{
			RaySource.GetComponent<OutputRay> ()._ray_saccade_onset = new Ray (history_origin [history_origin.Count - 3], history_smooth_fixations [history_smooth_fixations.Count - 3]);
		}
		else if (_ray_saccade_onset_index != 0 && history_smooth_fixations.Count > _ray_saccade_onset_index && history_origin.Count>_ray_saccade_onset_index )
		{
			
		
			RaySource.GetComponent<OutputRay> ()._ray_saccade_onset = new Ray (history_origin [history_origin.Count - _ray_saccade_onset_index], history_smooth_fixations [history_smooth_fixations.Count - _ray_saccade_onset_index]);
		}
		if (history_2_tmp.Count > 1 && history_smooth_all.Count>1 && history_smooth_fixations.Count>1)
		{
			RaySource.GetComponent<OutputRay> ()._diff_quaternion_raw = Quaternion.FromToRotation (history_2_tmp [history_2_tmp.Count - 1], history_2_tmp [history_2_tmp.Count - 2]);
			RaySource.GetComponent<OutputRay> ()._diff_quaternion_smooth_all = Quaternion.FromToRotation (history_smooth_all [history_smooth_all.Count - 1], history_smooth_all [history_smooth_all.Count - 2]);
			RaySource.GetComponent<OutputRay> ()._diff_quaternion_smooth_fixations = Quaternion.FromToRotation (history_smooth_fixations [history_smooth_fixations.Count - 1], history_smooth_fixations [history_smooth_fixations.Count - 2]);

		}


		RaySource.GetComponent<OutputRay> ()._ray_vel= history_vel[history_vel.Count-1] ;

	}

	private void ProcessData(Ray ray)
	{
		AddToList(history_origin,ray.origin,false);


		AddToList(history_tmp,ray.direction,true);
		AddToList(history_2_tmp,ray.direction,false);

		// we don't need to smooth the origin
		AddToList(history_smooth_fixations	,ComputeAverage(history_tmp),false);


		AddToList(history_smooth_all	,ComputeAverage(history_2_tmp),false);

	


		AddToList(history_vel,CalculateVelocity5(history_2_tmp));

		UpdateOutputRay ();
	}

	private void AddToList(List<float> list,float value)
	{

		list.Add(value);
		if (list.Count > MAXHISTORY) list.RemoveAt(0);

	}
	private void AddToList(List<Vector3> list,Vector3 value,bool OnlyFixations)
	{

		list.Add(value);
		if (list.Count > MAXHISTORY) list.RemoveAt(0);



		if (OnlyFixations) {
			if (CheckSaccade ()) {
				list.Clear ();
				list.Add (value);
			}

		}

	}

	private float CalculateVelocity5(List<Vector3> list)
	{


		if (list.Count >= 5) {
			// check angle of current and previous gaze sample. if bigger, its a saccade, and then reset history

			Vector3 vec = list [list.Count - 1].normalized;
			float H_ang_x_0 = Mathf.Acos (vec.x) * Mathf.Rad2Deg;
			float H_ang_y_0 = Mathf.Acos (vec.y) * Mathf.Rad2Deg;
			float H_ang_z_0 = Mathf.Acos (vec.z) * Mathf.Rad2Deg;

			vec = list [list.Count - 2].normalized;
			float H_ang_x_1 = Mathf.Acos (vec.x) * Mathf.Rad2Deg;
			float H_ang_y_1 = Mathf.Acos (vec.y) * Mathf.Rad2Deg;
			float H_ang_z_1 = Mathf.Acos (vec.z) * Mathf.Rad2Deg;

			vec = list [list.Count - 4].normalized;
			float H_ang_x_3 = Mathf.Acos (vec.x) * Mathf.Rad2Deg;
			float H_ang_y_3 = Mathf.Acos (vec.y) * Mathf.Rad2Deg;
			float H_ang_z_3 = Mathf.Acos (vec.z) * Mathf.Rad2Deg;

			vec = list [list.Count - 5].normalized;
			float H_ang_x_4 = Mathf.Acos (vec.x) * Mathf.Rad2Deg;
			float H_ang_y_4 = Mathf.Acos (vec.y) * Mathf.Rad2Deg;
			float H_ang_z_4 = Mathf.Acos (vec.z) * Mathf.Rad2Deg;

			// weighted moving average over five data samples to suppress noise 
			float vel_x = (H_ang_x_0 + H_ang_x_1 - H_ang_x_3 - H_ang_x_4) / 6;
			float vel_y = (H_ang_y_0 + H_ang_y_1 - H_ang_y_3 - H_ang_y_4) / 6;
			float vel_z = (H_ang_z_0 + H_ang_z_1 - H_ang_z_3 - H_ang_z_4) / 6;

			// vel
			return (new Vector3 (vel_x, vel_y, vel_z)).magnitude;
		} else {
			return 0.0f;
		
		}

	}
		

	private bool CheckSaccade()
	{
		_is_in_saccade = false;

	
			
		if ( history_vel.Count >2  &&  history_vel [history_vel.Count - 1] > SACCADE_VEL_THRESHOLD)
		{


			_is_in_saccade = true;


			// --- get the ray of the begining of the saccade --- 
			// searh back through velocity list and find the last occasion of vel < epsilon
			// we then return the smooth value from history_smooth_fixations

			float vel_thresh_2 = 0.5f;
			_ray_saccade_onset_index = 0;
			for (int i = 1; i < history_vel.Count; i++)
			{
				if (history_vel [history_vel.Count - i] < vel_thresh_2)
				{

					if ((i -6) < history_origin.Count   &&  (i-6)>0)
					{
						_ray_saccade_onset_index = i - 6;// lag compensation
					}
					break;
				}
			}

		}
		

		return _is_in_saccade;

	}




	private Vector3 ComputeAverage(List<Vector3> history)
	{
		Vector3 avg = Vector3.zero;
		int window = (windowSize < history.Count) ? windowSize : history.Count;

		for (int i = 1; i <= window; i = i + 1)
		{

			avg += history[history.Count - i];

		}
		// print ("average of " + window + "frames");
		return new Vector3(avg.x / window, avg.y / window, avg.z / window);
	}



}


