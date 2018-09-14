// Pupil Gaze Tracker service
//
// Originally written by Yamen Saraiji <mrayyamen@gmail.com> at May 17, 2017
// https://github.com/mrayy/PupilHMDCalibration
//
// Modified by Diako Mardanbegi <dmardanbeigi@gmail.com> at June 01, 2018
//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.IO;
using MsgPack.Serialization;

namespace Pupil
{
    //Pupil data types based on Yuta Itoh sample hosted in https://github.com/pupil-labs/hmd-eyes
    [Serializable]
    public class ProjectedSphere
    {
        public double[] axes = new double[] { 0, 0 };
        public double angle;
        public double[] center = new double[] { 0, 0 };
    }
    [Serializable]
    public class Sphere
    {
        public double radius;
        public double[] center = new double[] { 0, 0, 0 };
    }
    [Serializable]
    public class Circle3d
    {
        public double radius;
        public double[] center = new double[] { 0, 0, 0 };
        public double[] normal = new double[] { 0, 0, 0 };
    }
    [Serializable]
    public class Ellipse
    {
        public double[] axes = new double[] { 0, 0 };
        public double angle;
        public double[] center = new double[] { 0, 0 };
    }
    [Serializable]
    public class PupilData3D
    {
        public string topic;
        public Circle3d circle_3d = new Circle3d();
        public double confidence;
        public double timestamp;
        public double diameter_3d;

        public Ellipse ellipse = new Ellipse();
        public double[] norm_pos = new double[] { 0, 0 };
        public double diameter;
        public Sphere sphere = new Sphere();
        public ProjectedSphere projected_sphere = new ProjectedSphere();
        public double model_confidence;
        public int model_id;
        public double model_birth_timestamp;
        public double theta;

        public double phi;
        public string method;

        public int id;


    }
    public class GazeMSG
    {
        public string topic;
        public double[] norm_pos = new double[] { 0, 0 };

        public double confidence;
        public int id;
        public double timestamp;
        public PupilData3D[] base_data;

    }
}

public class PupilGazeTracker : MonoBehaviour
{
    static PupilGazeTracker _Instance;
    public static PupilGazeTracker Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("PupilGazeTracker").AddComponent<PupilGazeTracker>();
            }
            return _Instance;
        }
    }

    public class EyeData
    {


        public Vector2 gaze = new Vector2();

        public Vector2 pupil_center = new Vector2();


    }



    EyeData leftEye;
    EyeData rightEye;

    Vector2 gaze_average;


    Thread _serviceThread;
    bool _isDone = false;
    Pupil.PupilData3D _pupilData;

    Pupil.GazeMSG _gazeMSG;


    public delegate void OnCalibrationStartedDeleg(PupilGazeTracker manager);
    public delegate void OnCalibrationDoneDeleg(PupilGazeTracker manager);
    public delegate void OnEyeGazeDeleg(PupilGazeTracker manager);
    public delegate void OnCalibDataDeleg(PupilGazeTracker manager, float x, float y);

    public event OnCalibrationStartedDeleg OnCalibrationStarted;
    public event OnCalibrationDoneDeleg OnCalibrationDone;
    public event OnEyeGazeDeleg OnEyeGaze;
    public event OnCalibDataDeleg OnCalibData;


    bool _isconnected = false;
    RequestSocket _requestSocket;

    List<Dictionary<string, object>> _calibrationData = new List<Dictionary<string, object>>();

    [SerializeField]
    Dictionary<string, object>[] _CalibrationPoints
    {
        get { return _calibrationData.ToArray(); }
    }


    Vector2[] _calibPoints;
    int _calibSamples;
    int _currCalibPoint = 0;
    int _currCalibSamples = 0;

    public string ServerIP = "";
    public int ServicePort = 50020;
    public int DefaultCalibrationCount = 120;


    public float Calib_grid_radius1 = 0.15f;
    public float Calib_grid_radius2 = 0.15f;
    //calibrationTargets_offset
    public Vector2 Calib_grid_offet = new Vector2(0.00f, 0.0f);

    int _gazeFPS = 0;
    int _currentFps = 0;
    DateTime _lastT;

    object _dataLock;

    public int FPS
    {
        get { return _currentFps; }
    }

    enum EStatus
    {
        Idle,
        ProcessingGaze,
        Calibration
    }

    EStatus m_status = EStatus.Idle;

    public enum GazeSource
    {
        LeftEye,
        RightEye,
        BothEyes
    }
    public enum EyeSource
    {
        LeftEye,
        RightEye
    }
    public Vector2 NormalizedGazeAve
    {
        get { return gaze_average; }
    }

    public Vector2 LeftEyeGaze
    {
        get { return leftEye.gaze; }
    }
    public Vector2 RightEyeGaze
    {
        get { return rightEye.gaze; }
    }

    public EyeData LeftEyeData
    {
        get { return leftEye; }
    }
    public EyeData rightEyeData
    {
        get { return rightEye; }
    }

    public Vector2 GetEyeGaze(GazeSource s)
    {
        if (s == GazeSource.RightEye)
            return RightEyeGaze;
        if (s == GazeSource.LeftEye)
            return LeftEyeGaze;
        return NormalizedGazeAve;
    }

    public EyeData GetEyeData(EyeSource s)
    {
        if (s == EyeSource.RightEye)
            return rightEyeData;
        if (s == EyeSource.LeftEye)
            return LeftEyeData;
        return null;
    }



    public PupilGazeTracker()
    {
        _Instance = this;
    }
    void Start()
    {
        if (PupilGazeTracker._Instance == null)
            PupilGazeTracker._Instance = this;
        leftEye = new EyeData();
        rightEye = new EyeData();
        _dataLock = new object();
        _serviceThread = new Thread(NetMQClient);
        _serviceThread.Start();

    }
    void OnDestroy()
    {
        if (m_status == EStatus.Calibration)
            StopCalibration();
        _isDone = true;
        _serviceThread.Join();
    }

    NetMQMessage _sendRequestMessage(Dictionary<string, object> data)
    {
        NetMQMessage m = new NetMQMessage();
        m.Append("notify." + data["subject"]);

        using (var byteStream = new MemoryStream())
        {
            var ctx = new SerializationContext();
            ctx.CompatibilityOptions.PackerCompatibilityOptions = MsgPack.PackerCompatibilityOptions.None;
            var ser = MessagePackSerializer.Get<object>(ctx);
            ser.Pack(byteStream, data);
            m.Append(byteStream.ToArray());
        }

        _requestSocket.SendMultipartMessage(m);

        NetMQMessage recievedMsg;
        recievedMsg = _requestSocket.ReceiveMultipartMessage();

        return recievedMsg;
    }

    float GetPupilTimestamp()
    {
        _requestSocket.SendFrame("t");
        NetMQMessage recievedMsg = _requestSocket.ReceiveMultipartMessage();
        return float.Parse(recievedMsg[0].ConvertToString());
    }

    void NetMQClient()
    {
        //thanks for Yuta Itoh sample code to connect via NetMQ with Pupil Service
        string IPHeader = ">tcp://" + ServerIP + ":";
        var timeout = new System.TimeSpan(0, 0, 1); //1sec

        // Necessary to handle this NetMQ issue on Unity editor
        // https://github.com/zeromq/netmq/issues/526
        AsyncIO.ForceDotNet.Force();
        NetMQConfig.ManualTerminationTakeOver();
        NetMQConfig.ContextCreate(true);

        string subport = "";
        Debug.Log("Connect to the server: " + IPHeader + ServicePort + ".");
        _requestSocket = new RequestSocket(IPHeader + ServicePort);

        _requestSocket.SendFrame("SUB_PORT");
        _isconnected = _requestSocket.TryReceiveFrameString(timeout, out subport);

        _lastT = DateTime.Now;

        if (_isconnected)
        {
            StartProcess();
            var subscriberSocket = new SubscriberSocket(IPHeader + subport);
            subscriberSocket.Subscribe("gaze"); //subscribe for gaze data
            subscriberSocket.Subscribe("notify."); //subscribe for all notifications
            _setStatus(EStatus.ProcessingGaze);
            var msg = new NetMQMessage();
            while (_isDone == false)
            {
                _isconnected = subscriberSocket.TryReceiveMultipartMessage(timeout, ref (msg));
                if (_isconnected)
                {
                    try
                    {
                        string msgType = msg[0].ConvertToString();
                        // Debug.Log(msgType);
                        if (msgType == "gaze")
                        {
                            var message = MsgPack.Unpacking.UnpackObject(msg[1].ToByteArray());
                            MsgPack.MessagePackObject mmap = message.Value;

                            lock (_dataLock)
                            {
                                _gazeMSG = JsonUtility.FromJson<Pupil.GazeMSG>(mmap.ToString());

                                // Debug.Log("----------------------");
                                // Debug.Log(mmap.ToString());
                                // Debug.Log(_gazeMSG.base_data[0].ellipse.center[0] + "," + _gazeMSG.base_data[0].ellipse.center[1]);


                                if (_gazeMSG.confidence > 0.5f)
                                {
                                    OnPacket(_gazeMSG);
                                }
                            }
                        }
                        else
                        {
                           // Debug.Log(msgType);
                            if (msgType == "notify.eye_process.started")
                            {
                                _sendRequestMessage(new Dictionary<string, object> { { "subject", "set_detection_mapping_mode" }, { "mode", "3d" } });
                            }
                        }

                    }
                    catch
                    {
                        //	Debug.Log("Failed to unpack.");
                    }
                }
                else
                {
                    //	Debug.Log("Failed to receive a message.");
                    Thread.Sleep(500);
                }
            }
            StopProcess();
            subscriberSocket.Close();
        }
        else
        {
            Debug.Log("Failed to connect the server.");
        }
        _requestSocket.Close();
        // Necessary to handle this NetMQ issue on Unity editor
        // https://github.com/zeromq/netmq/issues/526
        Debug.Log("ContextTerminate: no eye gaze connection.");
        NetMQConfig.ContextTerminate();
    }

    void _setStatus(EStatus st)
    {
        if (st == EStatus.Calibration)
        {
            _calibrationData.Clear();
            _currCalibPoint = 0;
            _currCalibSamples = 0;
        }

        m_status = st;
    }

    public void StartProcess()
    {
        _sendRequestMessage(new Dictionary<string, object> { { "subject", "eye_process.should_start.0" }, { "eye_id", 0 } });
        _sendRequestMessage(new Dictionary<string, object> { { "subject", "eye_process.should_start.1" }, { "eye_id", 1 } });




    }
    public void StopProcess()
    {
        _sendRequestMessage(new Dictionary<string, object> { { "subject", "eye_process.should_stop" }, { "eye_id", 0 } });
        _sendRequestMessage(new Dictionary<string, object> { { "subject", "eye_process.should_stop" }, { "eye_id", 1 } });
    }

    public void StartCalibration()
    {




        //calibrate using default 9 points and 120 samples for each target
        StartCalibration(new Vector2[] {new Vector2 (0.5f, 0.5f)+Calib_grid_offet,
            new Vector2 (0.5f+Calib_grid_radius2*Mathf.Cos(45*Mathf.Deg2Rad), 0.5f+Calib_grid_radius2*Mathf.Sin(45*Mathf.Deg2Rad))+Calib_grid_offet,
             new Vector2 (0.5f+Calib_grid_radius2*Mathf.Cos(45*Mathf.Deg2Rad), 0.5f-Calib_grid_radius2*Mathf.Sin(45*Mathf.Deg2Rad))+Calib_grid_offet,
             new Vector2 (0.5f-Calib_grid_radius2*Mathf.Cos(45*Mathf.Deg2Rad), 0.5f+Calib_grid_radius2*Mathf.Sin(45*Mathf.Deg2Rad))+Calib_grid_offet,
             new Vector2 (0.5f-Calib_grid_radius2*Mathf.Cos(45*Mathf.Deg2Rad), 0.5f-Calib_grid_radius2*Mathf.Sin(45*Mathf.Deg2Rad))+Calib_grid_offet,
            new Vector2 (0.5f+Calib_grid_radius1, 0.5f)+Calib_grid_offet,
            new Vector2 (0.5f-Calib_grid_radius1, 0.5f)+Calib_grid_offet,
             new Vector2 (0.5f, 0.5f+Calib_grid_radius1)+Calib_grid_offet,
             new Vector2 (0.5f, 0.5f-Calib_grid_radius1)+Calib_grid_offet
            }, DefaultCalibrationCount);



    }
    public void StartCalibration(Vector2[] calibPoints, int samples)
    {
        _calibPoints = calibPoints;
        _calibSamples = samples;

        _sendRequestMessage(new Dictionary<string, object> { { "subject", "start_plugin" }, { "name", "HMD_Calibration" } });
        _sendRequestMessage(new Dictionary<string, object> { { "subject", "calibration.should_start" }, { "hmd_video_frame_size", new float[] { 1000, 1000 } }, { "outlier_threshold", 35 } });
        _setStatus(EStatus.Calibration);

        if (OnCalibrationStarted != null)
            OnCalibrationStarted(this);
    }
    public void StopCalibration()
    {
        _sendRequestMessage(new Dictionary<string, object> { { "subject", "calibration.should_stop" } });
        if (OnCalibrationDone != null)
            OnCalibrationDone(this);
        _setStatus(EStatus.ProcessingGaze);
    }


    void _CalibData(float x, float y)
    {
        if (OnCalibData != null)
            OnCalibData(this, x, y);
    }


    void OnPacket(Pupil.GazeMSG data)
    {
        //add new frame
        _gazeFPS++;
        var ct = DateTime.Now;
        if ((ct - _lastT).TotalSeconds > 1)
        {
            _lastT = ct;
            _currentFps = _gazeFPS;
            _gazeFPS = 0;
        }

        if (m_status == EStatus.ProcessingGaze)
        { //gaze processing stage


            float x, y;
            x = (float)data.norm_pos[0];
            y = (float)data.norm_pos[1];
            gaze_average.x = (leftEye.gaze.x + rightEye.gaze.x) * 0.5f;
            gaze_average.y = (leftEye.gaze.y + rightEye.gaze.y) * 0.5f;



            if (data.id == 0)
            {

                leftEye.pupil_center = new Vector2((float)data.base_data[0].ellipse.center[0], (float)data.base_data[0].ellipse.center[1]);
                leftEye.gaze = new Vector2(x, y);
                if (OnEyeGaze != null)
                    OnEyeGaze(this);
            }
            else if (data.id == 1)
            {
                rightEye.pupil_center = new Vector2((float)data.base_data[0].ellipse.center[0], (float)data.base_data[0].ellipse.center[1]);
                rightEye.gaze = new Vector2(x, y);
                if (OnEyeGaze != null)
                    OnEyeGaze(this);
            }


        }
        else if (m_status == EStatus.Calibration)
        {//gaze calibration stage
            float t = GetPupilTimestamp();
            var ref0 = new Dictionary<string, object>() { { "norm_pos", new float[] { _calibPoints[_currCalibPoint].x, _calibPoints[_currCalibPoint].y } }, { "timestamp", t }, { "id", 0 } };
            var ref1 = new Dictionary<string, object>() { { "norm_pos", new float[] { _calibPoints[_currCalibPoint].x, _calibPoints[_currCalibPoint].y } }, { "timestamp", t }, { "id", 1 } };

            _CalibData(_calibPoints[_currCalibPoint].x, _calibPoints[_currCalibPoint].y);

            _calibrationData.Add(ref0);
            _calibrationData.Add(ref1);
            _currCalibSamples++;
            Thread.Sleep(1000 / 60);

            if (_currCalibSamples >= _calibSamples)
            {
                _currCalibSamples = 0;
                _currCalibPoint++;

                string pointsData = "[";
                int index = 0;
                foreach (var v in _calibrationData)
                {
                    pointsData += JsonUtility.ToJson(v);//String.Format("{'norm_pos':({0},{1}),'timestamp':{2},'id':{3}}",v.norm_pos[0],v.norm_pos[1],v.timestamp,v.id);
                    ++index;
                    if (index != _calibrationData.Count)
                    {
                        pointsData += ",";
                    }
                }
                pointsData += "]";
                //	pointsData = JsonUtility.ToJson (_CalibrationPoints);
                //Debug.Log (pointsData);

                _sendRequestMessage(new Dictionary<string, object> { { "subject", "calibration.add_ref_data" }, { "ref_data", _CalibrationPoints } });
                _calibrationData.Clear();
                if (_currCalibPoint >= _calibPoints.Length)
                {

                    StopCalibration();
                }
            }
        }
    }

    void OnGUI()
    {
        string str = "Capture Rate=" + FPS;
        str += "\nLeft Eye:" + LeftEyeGaze.ToString();
        str += "\nRight Eye:" + RightEyeGaze.ToString();
        GUI.TextArea(new Rect(0, 0, 200, 50), str);
    }
}
