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

public class InteractionRay : MonoBehaviour {

	// Use this for initialization

	public Ray _ray;





	private RaycastHit[] hits;
	public List<GameObject> hit_objects_list;//=new List<GameObject>();
	private List<GameObject> pre_hit_objects_list;//=new List<GameObject>();




	private GazeInteractionEventArgs e;


	private void Start()
	{
		pre_hit_objects_list=new List<GameObject>();


	}


	private void Update()
	{


		// RayCasting
		hits = Physics.RaycastAll(_ray, Mathf.Infinity);

		List<GameObject> temp = new List<GameObject>();

		for (int i = 0; i < hits.Length; i++)
		{
			RaycastHit hit_i = hits[i];
			temp.Add(hit_i.collider.gameObject);

			e=new GazeInteractionEventArgs();
			e.Raycast_RaycastHit= hit_i;
			e.Raycast_GameObject = hit_i.collider.gameObject;
			e.Raycast_RayName = this.name;
			GazeInteractionEventManager.instance. FireRayIn(e);



		}

		hit_objects_list = temp;


		// trigger RayEnter for objects that are inside hit_objects_list but not in pre_hit_objects_list
		foreach (GameObject obj in hit_objects_list) {
			if (pre_hit_objects_list.Contains (obj) != true){
				e = new GazeInteractionEventArgs ();
				e.Raycast_GameObject = obj.gameObject;
				e.Raycast_RayName = this.name;
				GazeInteractionEventManager.instance. FireRayEnter (e);
			}
		}

		// trigger RayExit for objects that are inside pre_hit_objects_list but not in hit_objects_list
		foreach (GameObject obj in pre_hit_objects_list) {
			if (hit_objects_list.Contains (obj) != true) {
				e = new GazeInteractionEventArgs ();
				e.Raycast_GameObject = obj.gameObject;
				e.Raycast_RayName = this.name;
				GazeInteractionEventManager.instance. FireRayExit (e);
			}
		}



		pre_hit_objects_list = hit_objects_list;
	}


}
