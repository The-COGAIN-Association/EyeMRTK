using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class SaveData_FakeExperiment : MonoBehaviour {

	AdditionalInfoToSave _additionalInfoToSave;

	private const string FORMAT_FLOAT = "0.00000000";


	[SerializeField]
	[Tooltip ("If true, experiment is in progress.")]
	private bool _inProgress;

	[SerializeField]
	[Tooltip ("Participant Name.")]
	private string _participantName= "P";
	[SerializeField]
	[Tooltip ("Participant Gender.")]
	private string _participantGender= "";
	[SerializeField]
	[Tooltip ("Participant Age.")]
	private string _participantAge= "";



	[SerializeField]
	[Tooltip ("This key will start or stop the session")]
	private KeyCode _toggle = KeyCode.G;


	public bool inProgress {
		get {
			return _inProgress;
		}

		set {
			_inProgress = value;

			if (!value) {
				EndExperiment();
			}
		}
	}

	// Use this for initialization
	void Start () {


		_additionalInfoToSave = GetComponent<AdditionalInfoToSave> ();
	



	}

	// Update is called once per frame
	void Update () {


		if (Input.GetKeyDown (_toggle)) {
			inProgress = !inProgress;

			if (inProgress) {
				InitializeTarget ();

			} else {
				EndExperiment ();
			}
		}

		if (!_inProgress) {


			return;
		} else {

			RunExp();

		}






	}


	private void InitializeTarget ()
	{

		//hide gaze point
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("gaze_point"))
		{
			RayCastTrail rayCastTrail = obj.GetComponent<RayCastTrail> ();
			if (rayCastTrail!=null)
				rayCastTrail.On = false;
		}

		// start save data
		SaveData.Instance.saveData = true;


	}
	private void EndExperiment()
	{


		// stop save data
		SaveData.Instance.saveData = false;

		//show gaze point
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("gaze_point"))
		{
			RayCastTrail rayCastTrail = obj.GetComponent<RayCastTrail> ();
			if (rayCastTrail!=null)
				rayCastTrail.On = true;
		}

	}


	private void RunExp()
	{

		// set SaveInfo dic
		_additionalInfoToSave.SaveInfo=new Dictionary<string, Dictionary<string, string>> {};
		_additionalInfoToSave.SaveInfo.AddToNestedDictionary("Participant", "Name", _participantName);
		_additionalInfoToSave.SaveInfo.AddToNestedDictionary("Participant", "Gender", _participantGender);
		_additionalInfoToSave.SaveInfo.AddToNestedDictionary("Participant", "Age", _participantAge);

		// save more info if needed


		// Do things in the experiment




	}

}
