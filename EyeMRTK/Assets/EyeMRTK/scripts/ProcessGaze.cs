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
	[Range(0, MAXHISTORY)]
	public int windowSize = 10;
	[SerializeField]
	[Tooltip("saccade minimum amplitude in degrees")]
	[Range(0.0f, 2.0f)]
	public float SACCADETHRESHOLD = 1.5f; // visual angle to detect saccade
	List<Vector3> history = new List<Vector3>();


	public Ray _ray_raw;
	public Ray _ray_processed;




	private void Start()
	{


	}




	private void Update()
	{
		if (RaySource!=null && RaySource.GetComponent<OutputRay>()!=null)
		{
			_ray_raw = RaySource.GetComponent<OutputRay> ()._ray;
			ProcessData(_ray_raw);
		}

	}




	private void ProcessData(Ray ray)
	{


			Vector3 _origin = new Vector3(0, 0, 0);
		SmoothData(ray.direction);
		// we don't need to smooth the origin
		_origin = ray.origin;


		Vector3 vec = ComputeAverage(history);

			_ray_processed = new Ray(_origin, vec);


	}

	private void SmoothData(Vector3 value)
	{

		history.Add(value);
		if (history.Count > MAXHISTORY) history.RemoveAt(0);
		if (CheckSaccade(history))
		{
			history.Clear();
			history.Add(value);
		}
			

	}



	private bool CheckSaccade(List<Vector3> history)
	{
		if (history.Count >= 2)
		{
			// check angle of current and previous gaze sample. if bigger, its a saccade, and then reset history
			Vector3 currentgaze = history[history.Count - 1];
			Vector3 lastgaze = history[history.Count - 2];

			Vector3 origin = Vector3.zero;
			Vector3 currentGazeDirection = currentgaze - origin;
			Vector3 lastGazeDirection = lastgaze - origin;

			float currentangle = Vector3.Angle(currentGazeDirection, lastGazeDirection);

			if (currentangle > SACCADETHRESHOLD)
			{
				return true;
			}

		}
		return false;
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


