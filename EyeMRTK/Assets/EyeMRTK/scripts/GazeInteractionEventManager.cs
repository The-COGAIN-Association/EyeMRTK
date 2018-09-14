
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



public struct GazeInteractionEventArgs
{
	public RaycastHit Raycast_RaycastHit;
	public GameObject Raycast_GameObject; 
	public string Raycast_RayName; 
	public HeadGestureType HeadGestureName;
	public bool HeadGestureStatus;

}

public delegate void GazeInteractionEventHandler(object sender, GazeInteractionEventArgs e);




public class GazeInteractionEventManager : MonoBehaviour {


	private static GazeInteractionEventManager gazeInteractionEventManager;

	public  event GazeInteractionEventHandler RayEnter;
	public  event GazeInteractionEventHandler RayExit;
	public  event GazeInteractionEventHandler RayIn;
	public  event GazeInteractionEventHandler HeadGesture;


	public static GazeInteractionEventManager instance
	{
		get
		{
			if (!gazeInteractionEventManager)
			{
				gazeInteractionEventManager = FindObjectOfType (typeof (GazeInteractionEventManager)) as GazeInteractionEventManager;

				if (!gazeInteractionEventManager)
				{
					Debug.LogError ("There needs to be one active GazeInteractionEventManager script on a GameObject in your scene.");
				}
			
			}

			return gazeInteractionEventManager;
		}
	}

	public  void FireRayIn(GazeInteractionEventArgs e)
	{
		if (RayIn != null)
			RayIn(this, e);
	}

	public  void FireRayEnter(GazeInteractionEventArgs e)
	{
		if (RayEnter != null)
			RayEnter(this, e);
	}
	public  void FireRayExit(GazeInteractionEventArgs e)
	{
		if (RayExit != null)
			RayExit(this, e);
	}

	public  void FireHeadGesture(GazeInteractionEventArgs e)
	{
		if (HeadGesture != null)
			HeadGesture(this, e);
	}

}