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

public class Point_and_Highlight : MonoBehaviour {

	public bool Gaze=true;
	public bool Reticle=true;
	public bool Laser=true;

	public bool GazeAndReticle=false;


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


		if (
			( Gaze && _gazeInteractionGeneric.pointingStatus._GazeIn) |
			( Reticle && _gazeInteractionGeneric.pointingStatus._ReticleIn) |
			( Laser && _gazeInteractionGeneric.pointingStatus._LaserIn) |
			(GazeAndReticle &&  _gazeInteractionGeneric.pointingStatus._GazeIn &&_gazeInteractionGeneric.pointingStatus._ReticleIn)
		)
			
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
