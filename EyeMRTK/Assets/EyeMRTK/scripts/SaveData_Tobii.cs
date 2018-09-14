using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using Tobii.Research.Unity;
using Tobii.Research;

public class SaveData_Tobii: MonoBehaviour {
	
	AdditionalInfoToSave _additionalInfoToSave;

	private const string FORMAT_FLOAT = "0.00000000";



	private  Vector3 Tobii_Point3D_to_Vector3D (Point3D point3d)
	{
		return new Vector3 (point3d.X, point3d.Y, point3d.Z);
	}

	private  Vector3 Tobii_Point2D_to_Vector2D (NormalizedPoint2D point2d)
	{
		return new Vector2 (point2d.X, point2d.Y);
	}


	// Use this for initialization
	void Start () {


		_additionalInfoToSave = this.GetComponent<AdditionalInfoToSave> ();


	}
	
	// Update is called once per frame
	void Update () {



		Tobii.Research.Unity.VREyeTracker _tobiiEyeTracker = Tobii.Research.Unity.VREyeTracker.Instance;
		if (_tobiiEyeTracker != null && _tobiiEyeTracker.Connected) {
			
				IVRGazeData last_ivrGazeData = _tobiiEyeTracker.LatestGazeData;



				// set SaveInfo dic
				_additionalInfoToSave.SaveInfo = new Dictionary<string, Dictionary<string, string>> { };


				// Timestamp
			if (last_ivrGazeData.OriginalGaze != null)
			{
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("Time", "DeviceTimeStamp", last_ivrGazeData.OriginalGaze.DeviceTimeStamp.ToString ());
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("Time", "SystemTimeStamp", last_ivrGazeData.OriginalGaze.SystemTimeStamp.ToString ());
			}


				// Head 
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("Head", "Position_X", last_ivrGazeData.Pose.Position.x.ToString (FORMAT_FLOAT));
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("Head", "Position_Y", last_ivrGazeData.Pose.Position.y.ToString (FORMAT_FLOAT));
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("Head", "Position_Z", last_ivrGazeData.Pose.Position.z.ToString (FORMAT_FLOAT));
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("Head", "Rotation_X", last_ivrGazeData.Pose.Rotation.eulerAngles.x.ToString (FORMAT_FLOAT));
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("Head", "Rotation_Y", last_ivrGazeData.Pose.Rotation.eulerAngles.y.ToString (FORMAT_FLOAT));
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("Head", "Rotation_Z", last_ivrGazeData.Pose.Rotation.eulerAngles.z.ToString (FORMAT_FLOAT));





				//		combined gaze
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("CombinedGaze", "RayOrigin_X", last_ivrGazeData.CombinedGazeRayWorld.origin.x.ToString (FORMAT_FLOAT));
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("CombinedGaze", "RayOrigin_Y", last_ivrGazeData.CombinedGazeRayWorld.origin.y.ToString (FORMAT_FLOAT));
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("CombinedGaze", "RayOrigin_Z", last_ivrGazeData.CombinedGazeRayWorld.origin.z.ToString (FORMAT_FLOAT));
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("CombinedGaze", "RayDirection_X", last_ivrGazeData.CombinedGazeRayWorld.direction.x.ToString (FORMAT_FLOAT));
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("CombinedGaze", "RayDirection_Y", last_ivrGazeData.CombinedGazeRayWorld.direction.y.ToString (FORMAT_FLOAT));
				_additionalInfoToSave.SaveInfo.AddToNestedDictionary ("CombinedGaze", "RayDirection_Z", last_ivrGazeData.CombinedGazeRayWorld.direction.z.ToString (FORMAT_FLOAT));


				// Eye 
				// 		left
				WriteTobiiEyeData ("LeftEye", last_ivrGazeData.Left);
				//		right
				WriteTobiiEyeData ("RightEye", last_ivrGazeData.Right);



		}


	}


	private  void WriteTobiiEyeData (string header, IVRGazeDataEye eye)
	{
		_additionalInfoToSave.SaveInfo.AddToNestedDictionary(header, "PupilDiameter" , eye.PupilDiameter.ToString (FORMAT_FLOAT) );
		_additionalInfoToSave.SaveInfo.AddToNestedDictionary(header, "PupilPosition_X", eye.PupilPosiitionInTrackingArea.x.ToString (FORMAT_FLOAT) );
		_additionalInfoToSave.SaveInfo.AddToNestedDictionary(header, "PupilPosition_Y", eye.PupilPosiitionInTrackingArea.y.ToString (FORMAT_FLOAT));

		_additionalInfoToSave.SaveInfo.AddToNestedDictionary(header, "RayOrigin_X", eye.GazeRayWorld.origin.x.ToString (FORMAT_FLOAT));
		_additionalInfoToSave.SaveInfo.AddToNestedDictionary(header, "RayOrigin_Y", eye.GazeRayWorld.origin.y.ToString (FORMAT_FLOAT));
		_additionalInfoToSave.SaveInfo.AddToNestedDictionary(header, "RayOrigin_Z",eye.GazeRayWorld.origin.z.ToString (FORMAT_FLOAT) );

		_additionalInfoToSave.SaveInfo.AddToNestedDictionary(header, "RayDirection_X",eye.GazeRayWorld.direction.x.ToString (FORMAT_FLOAT) );
		_additionalInfoToSave.SaveInfo.AddToNestedDictionary(header, "RayDirection_Y",eye.GazeRayWorld.direction.y.ToString (FORMAT_FLOAT) );
		_additionalInfoToSave.SaveInfo.AddToNestedDictionary(header, "RayDirection_Z", eye.GazeRayWorld.direction.z.ToString (FORMAT_FLOAT));



	}


}
