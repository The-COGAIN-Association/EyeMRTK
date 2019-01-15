using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData_FakeExperiment_sub : MonoBehaviour {
	public IDictionary<string, Dictionary<string, string>> SaveInfo =
		new Dictionary<string, Dictionary<string, string>> {};

	private const string FORMAT_FLOAT = "0.00000000";



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (SaveData.Instance.saveData == true) {
			// set SaveInfo dic
			SaveInfo=new Dictionary<string, Dictionary<string, string>> {};
			SaveInfo.AddToNestedDictionary("Heading", "Value_X", Input.mousePosition.x.ToString(FORMAT_FLOAT));
			SaveInfo.AddToNestedDictionary("Heading", "Value_Y", Input.mousePosition.y.ToString(FORMAT_FLOAT));

			SaveData.Instance.AddToList (SaveInfo);

		}
	}
}
