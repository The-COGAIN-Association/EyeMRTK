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

public class RotateCameraWithKeyboard: MonoBehaviour 
{
	//
	// VARIABLES
	//
	public Camera cam;

	public float turnSpeed = 4.0f;		// Speed of camera turning when mouse moves in along an axis

	//
	// UPDATE
	//

	void Update () 

	{



		// left key only

		bool Left = (Input.GetKey (KeyCode.LeftArrow) && !Input.GetKey (KeyCode.RightArrow) && !Input.GetKey (KeyCode.UpArrow) && !Input.GetKey (KeyCode.DownArrow))
		          | (Input.GetKey (KeyCode.A)         && !Input.GetKey (KeyCode.D)          && !Input.GetKey (KeyCode.W)        && !Input.GetKey (KeyCode.S));


		bool Right = (!Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.RightArrow) && !Input.GetKey (KeyCode.UpArrow) && !Input.GetKey (KeyCode.DownArrow))
			       | (!Input.GetKey (KeyCode.A)         && Input.GetKey (KeyCode.D)          && !Input.GetKey (KeyCode.W)        && !Input.GetKey (KeyCode.S));

		bool Up = (!Input.GetKey (KeyCode.LeftArrow) && !Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.UpArrow) && !Input.GetKey (KeyCode.DownArrow))
			    | (!Input.GetKey (KeyCode.A)         && !Input.GetKey (KeyCode.D)          && Input.GetKey (KeyCode.W)       && !Input.GetKey (KeyCode.S));

		bool Down = (!Input.GetKey (KeyCode.LeftArrow) && !Input.GetKey (KeyCode.RightArrow) && !Input.GetKey (KeyCode.UpArrow) && Input.GetKey (KeyCode.DownArrow))
		          | (!Input.GetKey (KeyCode.A)         && !Input.GetKey (KeyCode.D)          && !Input.GetKey (KeyCode.W)       && Input.GetKey (KeyCode.S));


		bool LeftUp = (Input.GetKey (KeyCode.LeftArrow) && !Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.UpArrow) && !Input.GetKey (KeyCode.DownArrow))
			        | (Input.GetKey (KeyCode.A)         && !Input.GetKey (KeyCode.D)          && Input.GetKey (KeyCode.W)        && !Input.GetKey (KeyCode.S));


		bool LeftDown = (Input.GetKey (KeyCode.LeftArrow) && !Input.GetKey (KeyCode.RightArrow) && !Input.GetKey (KeyCode.UpArrow) && Input.GetKey (KeyCode.DownArrow))
			          | (Input.GetKey (KeyCode.A)         && !Input.GetKey (KeyCode.D)          && !Input.GetKey (KeyCode.W)        && Input.GetKey (KeyCode.S));

		bool RigthUp = (!Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.UpArrow) && !Input.GetKey (KeyCode.DownArrow))
		         	| (!Input.GetKey (KeyCode.A)         && Input.GetKey (KeyCode.D)          && Input.GetKey (KeyCode.W)       && !Input.GetKey (KeyCode.S));

		bool RightDown = (!Input.GetKey (KeyCode.LeftArrow) && Input.GetKey (KeyCode.RightArrow) && !Input.GetKey (KeyCode.UpArrow) && Input.GetKey (KeyCode.DownArrow))
		            	| (!Input.GetKey (KeyCode.A)         && Input.GetKey (KeyCode.D)          && !Input.GetKey (KeyCode.W)       && Input.GetKey (KeyCode.S));



		if (Left) {

			transform.RotateAround(transform.position, Vector3.up, - turnSpeed);

		} else if (Right) {
			
			transform.RotateAround(transform.position, Vector3.up,  turnSpeed);

		} else if (Up) {
			
			transform.RotateAround(transform.position, transform.right, - turnSpeed);

		} else if (Down) {

			transform.RotateAround(transform.position, transform.right,  turnSpeed);
		} 

		else if (LeftUp) {

			transform.RotateAround(transform.position, Vector3.up, - turnSpeed);
			transform.RotateAround(transform.position, transform.right, - turnSpeed);

		} else if (LeftDown) {

			transform.RotateAround(transform.position, Vector3.up, - turnSpeed);
			transform.RotateAround(transform.position, transform.right,  turnSpeed);
		} else if (RigthUp) {

			transform.RotateAround(transform.position, Vector3.up,  turnSpeed);
			transform.RotateAround(transform.position, transform.right, - turnSpeed);

		} else if (RightDown) {

			transform.RotateAround(transform.position, Vector3.up,  turnSpeed);
			transform.RotateAround(transform.position, transform.right,  turnSpeed);
		} 






	}
}