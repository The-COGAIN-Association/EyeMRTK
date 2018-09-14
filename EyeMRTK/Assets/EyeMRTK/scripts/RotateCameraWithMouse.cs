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

using UnityEngine;
using System.Collections;

public class RotateCameraWithMouse: MonoBehaviour 
{
	//
	// VARIABLES
	//
	public Camera cam;
	public float turnSpeed = 4.0f;		// Speed of camera turning when mouse moves in along an axis
	public float panSpeed = 4.0f;		// Speed of the camera when being panned
	public float zoomSpeed = 4.0f;		// Speed of the camera going back and forth

	private Vector3 mouseOrigin;	// Position of cursor when mouse dragging starts
	private bool isPanning;		// Is the camera being panned?
	private bool isRotating;	// Is the camera being rotated?
	private bool isZooming;		// Is the camera zooming?

	//
	// UPDATE
	//

	void Update () 
	{
		// Get the left mouse button
		if(Input.GetMouseButtonDown(0))
		{
			// Get mouse origin
			mouseOrigin = Input.mousePosition;
			isRotating = true;
		}

		// Get the right mouse button
		if(Input.GetMouseButtonDown(1))
		{
			// Get mouse origin
			mouseOrigin = Input.mousePosition;
			isPanning = true;
		}

		// Get the middle mouse button
		if(Input.GetMouseButtonDown(2))
		{
			// Get mouse origin
			mouseOrigin = Input.mousePosition;
			isZooming = true;
		}

		// Disable movements on button release
		if (!Input.GetMouseButton(0)) isRotating=false;
		if (!Input.GetMouseButton(1)) isPanning=false;
		if (!Input.GetMouseButton(2)) isZooming=false;

		// Rotate camera along X and Y axis
		if (isRotating)
		{
			Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

			transform.RotateAround(transform.position, transform.right, -pos.y * turnSpeed);
			transform.RotateAround(transform.position, Vector3.up, pos.x * turnSpeed);
		}

		// Move the camera on it's XY plane
		if (isPanning)
		{
			Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

			Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
			transform.Translate(move, Space.Self);
		}

		// Move the camera linearly along Z axis
		if (isZooming)
		{
			Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

			Vector3 move = pos.y * zoomSpeed * transform.forward; 
			transform.Translate(move, Space.World);
		}
	}
}