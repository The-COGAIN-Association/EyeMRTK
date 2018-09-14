
// -----------------------------------------------------------------------
//
// (c) Copyright 1997-2017, SensoMotoric Instruments GmbH
// 
// Permission  is  hereby granted,  free  of  charge,  to any  person  or
// organization  obtaining  a  copy  of  the  software  and  accompanying
// documentation  covered  by  this  license  (the  "Software")  to  use,
// reproduce,  display, distribute, execute,  and transmit  the Software,
// and  to  prepare derivative  works  of  the  Software, and  to  permit
// third-parties to whom the Software  is furnished to do so, all subject
// to the following:
// 
// The  copyright notices  in  the Software  and  this entire  statement,
// including the above license  grant, this restriction and the following
// disclaimer, must be  included in all copies of  the Software, in whole
// or  in part, and  all derivative  works of  the Software,  unless such
// copies   or   derivative   works   are   solely   in   the   form   of
// machine-executable  object   code  generated  by   a  source  language
// processor.
// 
// THE  SOFTWARE IS  PROVIDED  "AS  IS", WITHOUT  WARRANTY  OF ANY  KIND,
// EXPRESS OR  IMPLIED, INCLUDING  BUT NOT LIMITED  TO THE  WARRANTIES OF
// MERCHANTABILITY,   FITNESS  FOR  A   PARTICULAR  PURPOSE,   TITLE  AND
// NON-INFRINGEMENT. IN  NO EVENT SHALL  THE COPYRIGHT HOLDERS  OR ANYONE
// DISTRIBUTING  THE  SOFTWARE  BE   LIABLE  FOR  ANY  DAMAGES  OR  OTHER
// LIABILITY, WHETHER  IN CONTRACT, TORT OR OTHERWISE,  ARISING FROM, OUT
// OF OR IN CONNECTION WITH THE  SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// -----------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace SMI
{
    /// <summary>
    /// Example for gaze triggered UI. Inherited from GazeAction
    /// </summary>
    public class GazeUI : GazeAction
    {
        [Tooltip("GameObject which contains target behaviour. The method to be invoked must be implemented there.")]
        [SerializeField]
        GameObject targetGameObject = null;
        public GameObject TargetGameObject
        {
            get { return targetGameObject; }
        }

        [HideInInspector]
        [SerializeField]
        Component targetComponent = null;
        public Component TargetComponent
        {
            get { return targetComponent; }
            set { targetComponent = value; }
        }

        [Tooltip("The method to be invoked. Target Monobehaviour must be set.")]
        [HideInInspector]
        [SerializeField]
        MethodCaller method;
        public MethodCaller Method
        {
            get { return method; }
            set { method = value; }
        }
        

        [System.Serializable]
        public class MethodCaller
        {
            public string methodToCall;
            public List<InvokeParameters> Parameters;
        };

        [System.Serializable]
        public class InvokeParameters
        {
            public string type;
            public UnityEngine.Object unityObject;
            public Vector2 vector2;
            public Vector3 vector3;
            public Vector4 vector4;
            public Quaternion quaternion;
            public Color color;
            public Rect rect;
            public LayerMask layerMask;
            public int Int;
            public float Float;
            public string String;
            public bool Bool;
            public Vector2[] vector2s;
            public Vector3[] vector3s;
            public Vector4[] vector4s;
            public Quaternion[] quaternions;
            public Color[] colors;
            public Rect[] rects;
            public LayerMask[] layerMasks;
            public int[] Ints;
            public float[] Floats;
            public string[] Strings;
            public bool[] Bools;
        }

        // Use this for initialization
        public override void Start()
        {
            base.Start();
            UnityEngine.UI.RawImage image = GetComponentInChildren<UnityEngine.UI.RawImage>();
            if (image != null)
            {
                defaultMaterial = image.material;
                meshrenderer = null;
            }
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }
        public override void OnGazeEnter(RaycastHit hitInformation)
        {
            //ToDo: Stuff
            base.OnGazeEnter(hitInformation);
            if (GetComponentInChildren<UnityEngine.UI.RawImage>() != null)
                GetComponentInChildren<UnityEngine.UI.RawImage>().material = gazeMaterial;

        }
        public override void OnGazeExit()
        {
            //ToDo: Stuff
            base.OnGazeExit();
            if (GetComponentInChildren<UnityEngine.UI.RawImage>() != null)
                GetComponentInChildren<UnityEngine.UI.RawImage>().material = defaultMaterial;
        }
        protected override void DoAction()
        {
            base.DoAction();

            List<object> paramlist = new List<object>();
            foreach(InvokeParameters p in method.Parameters)
            {
                if (p.GetType().GetMembers().Where(pf => pf.Name != "type" && GetMemberUnderlyingTypeToString(pf) == p.type).Count() > 0)
                {
                    FieldInfo field = typeof(InvokeParameters).GetField(p.GetType().GetMembers().Where
                        (pf => pf.Name != "type" && GetMemberUnderlyingTypeToString(pf) == p.type).First().Name);
                    paramlist.Add(field.GetValue(p));
                    
                }
                else
                {
                    if (p.type.Contains("[]"))
                    {
                        
                        FieldInfo field = typeof(InvokeParameters).GetField("unityObjects");
                        paramlist.Add(field.GetValue(p));
                    }
                    else
                    {
                        FieldInfo field = typeof(InvokeParameters).GetField("unityObject");

                        if (field.GetValue(p).ToString() == "null")
                        {
                            Debug.LogError("GazeUI: Parameter Unity Object Field Cannot be null!: " + p.type);
                            return;
                        }
                        paramlist.Add(field.GetValue(p));
                    }
                }
            }
            Type[] argTypes = paramlist.Select(p => p.GetType()).ToArray();

            targetComponent.GetType().GetMethod(method.methodToCall, argTypes).Invoke(targetComponent, paramlist.ToArray());

        }
        public static string GetMemberUnderlyingTypeToString(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType.ToString();
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType.ToString();
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType.ToString();
            }
            return "";
        }
        
    }
}