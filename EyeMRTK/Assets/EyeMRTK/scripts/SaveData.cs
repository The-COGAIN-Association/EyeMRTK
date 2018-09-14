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


using System.Xml;
using UnityEngine;
using  System.Collections.Generic;

public class SaveData : MonoBehaviour
{
	/// <summary>
	/// Instance of <see cref="SaveData"/> for easy access.
	/// Assigned in Awake() so use earliest in Start().
	/// </summary>
	public static SaveData Instance { get; private set; }

	[SerializeField]
	[Tooltip ("If true, data is saved.")]
	private bool _saveData;

	[SerializeField]
	[Tooltip ("Folder in the application root directory where data is saved.")]
	private string _folder = "Data";

	[SerializeField]
	[Tooltip ("This key will start or stop saving data.")]
	private KeyCode _toggleSaveData = KeyCode.S;

	[SerializeField]
	public string _fileNamePrefix="vr_data";
	[SerializeField]
	public string fileName="";
	/// <summary>
	/// If true, data is saved.
	/// </summary>
	public bool saveData {
		get {
			return _saveData;
		}

		set {
			_saveData = value;

			if (!value) {

				CloseDataFile ();
			}
		}
	}

	[SerializeField]
	[Tooltip("Additional info to save. Drag and drop a game object that contains assest with public nested dictionary named SaveInfo.")]
	private GameObject[] AdditionalInfo;



	private XmlWriterSettings _fileSettings;
	private XmlWriter _file;

	private void Awake ()
	{
		Instance = this;
	}


	private void Update ()
	{
		if (Input.GetKeyDown (_toggleSaveData)) {
			saveData = !saveData;
		}

		if (!_saveData) {
			if (_file != null) {
				
				// Closes _file and sets it to null.
				CloseDataFile ();
			}

			return;
		}

		if (_file == null) {
			// Opens data file. It becomes non-null.
			OpenDataFile ();
		}


		WriteData ();
	}

	private void OnDestroy ()
	{

		CloseDataFile ();
	}

	private void OpenDataFile ()
	{
		if (_file != null) {
			Debug.Log ("Already saving data.");
			return;
		}

		if (!System.IO.Directory.Exists (_folder)) {
			System.IO.Directory.CreateDirectory (_folder);
		}

		_fileSettings = new XmlWriterSettings ();
		_fileSettings.Indent = true;
		 fileName = string.Format ("{0}_{1}.xml", _fileNamePrefix ,System.DateTime.Now.ToString ("yyyyMMddTHHmmss"));
		_file = XmlWriter.Create (System.IO.Path.Combine (_folder, fileName), _fileSettings);
		_file.WriteStartDocument ();
		_file.WriteStartElement ("VRData");
	}

	private void CloseDataFile ()
	{

		Debug.Log ("closing file" );

		if (_file == null) {
			Debug.Log ("No ongoing recording.");
			return;
		}

		_file.WriteEndElement ();
		_file.WriteEndDocument ();
		_file.Flush ();
		_file.Close ();
		_file = null;
		_fileSettings = null;
	}

	private void WriteData ()
	{
		_file.WriteStartElement ("frame");
		_file = WriteAllData (_file);
		_file.WriteEndElement ();

	}




	private XmlWriter  WriteAllData (XmlWriter file)
	{
		
		// Additional Info
		file= WriteAnyAdditionalInfo(file);
		return file;
	}


	private XmlWriter WriteAnyAdditionalInfo(XmlWriter file)
	{

		// search for SaveInfo in every component of childrens of every AdditionalInfo object

		foreach(GameObject obj in AdditionalInfo)
		{
			AdditionalInfoToSave[] Additions=obj.GetComponentsInChildren<AdditionalInfoToSave> ();
			foreach(AdditionalInfoToSave adds in Additions)
			{
			  
				foreach (KeyValuePair<string,  Dictionary<string, string>> kvp in adds.SaveInfo)
				{
					file.WriteStartElement (kvp.Key);

					foreach (KeyValuePair<string, string> kvp2 in kvp.Value) {

						file.WriteAttributeString (kvp2.Key, kvp2.Value);

					}
					file.WriteEndElement ();
				}


			}

		}
		return file;

	}








}
