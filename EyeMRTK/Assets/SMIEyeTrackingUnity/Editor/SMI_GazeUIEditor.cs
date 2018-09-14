
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
using System.Collections;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SMI
{
    /// <summary>
    /// Custom inspector for GazeUI
    /// </summary>
    [CustomEditor(typeof(GazeUI))]
    public class SMI_GazeUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            

            GazeUI myTarget = (GazeUI)target;
            DrawDefaultInspector();
            if(myTarget.TargetGameObject != null) {
                int selectedObj = 0;
                if (myTarget.TargetComponent != null)
                    selectedObj = myTarget.TargetGameObject.GetComponents<Component>().ToList().FindIndex(s => s == myTarget.TargetComponent);

                int newselectedObj = EditorGUILayout.Popup("Target MonoBehaviour", selectedObj, myTarget.TargetGameObject.GetComponents<Component>().Select(s => s.ToString()).ToArray());
                if (newselectedObj > -1)
                {
                    myTarget.TargetComponent = myTarget.TargetGameObject.GetComponents<Component>()[newselectedObj];
                }

                if (myTarget.TargetComponent != null)
                {
                    MethodInfo[] methodInfos;
                    methodInfos = myTarget.TargetComponent.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    if (methodInfos.Length > 0)
                    {
                        int selected = 0;
                        if (myTarget.Method.methodToCall != null)
                            selected = methodInfos.ToList().FindIndex(s => CheckGazeUIFunction(s, myTarget.Method));

                        int newselected = EditorGUILayout.Popup("On DoAction Call", selected, methodInfos.Select(s => s.ToString()).ToArray());
                        if (newselected != -1)
                        {
                            myTarget.Method.methodToCall = methodInfos[newselected].Name;
                            if (selected != newselected)
                            {
                                myTarget.Method.Parameters = new System.Collections.Generic.List<GazeUI.InvokeParameters>();
                                foreach (ParameterInfo pInfo in methodInfos[newselected].GetParameters())
                                {
                                    GazeUI.InvokeParameters IP = new GazeUI.InvokeParameters();
                                    IP.type = pInfo.ParameterType.ToString();
                                    myTarget.Method.Parameters.Add(IP);
                                }
                            }
                            for (int i = 0; i < myTarget.Method.Parameters.Count; i++)
                            {
                                myTarget.Method.Parameters[i] = AddInputField(myTarget.Method.Parameters[i], methodInfos[newselected].GetParameters()[i].Name,
                                     methodInfos[newselected].GetParameters()[i].ParameterType);
                            }
                        }
                        else
                        {
                            myTarget.Method.methodToCall = "";
                        }
                    }
                }
            }
        }

        bool CheckGazeUIFunction(MethodInfo mInfo, GazeUI.MethodCaller target)
        {
            bool isSo = false;
            if(mInfo.Name == target.methodToCall)
            {
                for(int i = 0; i < Mathf.Min(mInfo.GetParameters().Length, target.Parameters.Count); i++)
                {
                    if (mInfo.GetParameters()[i].ParameterType.ToString() == target.Parameters[i].type)
                        isSo = true;
                    else
                        isSo = false;
                }
                if (mInfo.GetParameters().Length == 0 && target.Parameters.Count == 0)
                    isSo = true;
                else if (mInfo.GetParameters().Length != target.Parameters.Count)
                    isSo = false;
            }
            return isSo;
        }

        GazeUI.InvokeParameters AddInputField(GazeUI.InvokeParameters p, string argName, Type type)
        {
            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                p.unityObject = EditorGUILayout.ObjectField("  " + argName, p.unityObject, type, true);
            }
            else if (p.type == typeof(int).ToString())
            {
                p.Int = EditorGUILayout.IntField("  " + argName, (int)p.Int);
            }
            else if (p.type == typeof(float).ToString())
            {
                p.Float = EditorGUILayout.FloatField("  " + argName, p.Float);
            }
            else if (p.type == typeof(bool).ToString())
            {
                p.Bool = EditorGUILayout.Toggle(p.Bool);
            }
            else if (p.type == typeof(string).ToString())
            {
                p.String = EditorGUILayout.TextField("  " + argName, (string)p.String);
            }
            else if (p.type == typeof(Vector2).ToString())
            {
                p.vector2 = EditorGUILayout.Vector2Field(" " + argName, p.vector2);
            }
            else if (p.type == typeof(Vector3).ToString())
            {
                p.vector3 = EditorGUILayout.Vector3Field(" " + argName, p.vector3);
            }
            else if (p.type == typeof(Vector4).ToString())
            {
                p.vector4 = EditorGUILayout.Vector4Field(" " + argName, p.vector4);
            }
            else if (p.type == typeof(Quaternion).ToString())
            {
                p.quaternion = Quaternion.Euler(EditorGUILayout.Vector3Field(" " + argName, p.quaternion.eulerAngles));
            }
            else if (p.type == typeof(Color).ToString())
            {
                p.color = EditorGUILayout.ColorField(" " + argName, p.color);
            }
            else if (p.type == typeof(Rect).ToString())
            {
                p.rect = EditorGUILayout.RectField(" " + argName, p.rect);
            }
            
            else if (p.type == typeof(int[]).ToString())
            {
                int[] array = AddArrayInputField<int>(p.Ints, argName);
                for (int i = 0; i < array.Length; i++)
                    array[i] = EditorGUILayout.IntField(" ", array[i]);
                p.Ints = array;
            }
            else if (p.type == typeof(float[]).ToString())
            {
                float[] array = AddArrayInputField<float>(p.Floats, argName);
                for (int i = 0; i < array.Length; i++)
                    array[i] = EditorGUILayout.FloatField(" ", array[i]);
                p.Floats = array;
            }
            else if (p.type == typeof(bool[]).ToString())
            {
                bool[] array = AddArrayInputField<bool>(p.Bools, argName);
                for (int i = 0; i < array.Length; i++)
                    array[i] = EditorGUILayout.Toggle(" ", array[i]);
                p.Bools = array;
            }
            else if (p.type == typeof(string[]).ToString())
            {
                string[] array = AddArrayInputField<string>(p.Strings, argName);
                for (int i = 0; i < array.Length; i++)
                    array[i] = EditorGUILayout.TextField(" ", array[i]);
                p.Strings = array;
            }
            else if (p.type == typeof(Vector2[]).ToString())
            {
                Vector2[] array = AddArrayInputField<Vector2>(p.vector2s, argName);
                for (int i = 0; i < array.Length; i++)
                    array[i] = EditorGUILayout.Vector2Field(" ", array[i]);
                p.vector2s = array;
            }
            else if (p.type == typeof(Vector3[]).ToString())
            {
                Vector3[] array = AddArrayInputField<Vector3>(p.vector3s, argName);
                for (int i = 0; i < array.Length; i++)
                    array[i] = EditorGUILayout.Vector3Field(" ", array[i]);
                p.vector3s = array;
            }
            else if (p.type == typeof(Quaternion[]).ToString())
            {
                Quaternion[] array = AddArrayInputField<Quaternion>(p.quaternions, argName);
                for (int i = 0; i < array.Length; i++)
                    array[i] = Quaternion.Euler(EditorGUILayout.Vector3Field(" ", array[i].eulerAngles));
                p.quaternions = array;
            }
            else if (p.type == typeof(Color[]).ToString())
            {
                Color[] array = AddArrayInputField<Color>(p.colors, argName);
                for (int i = 0; i < array.Length; i++)
                    array[i] = EditorGUILayout.ColorField(" ", array[i]);
                p.colors = array;
            }
            else if (p.type == typeof(Rect[]).ToString())
            {
                Rect[] array = AddArrayInputField<Rect>(p.rects, argName);
                for (int i = 0; i<array.Length; i++)
                    array[i] = EditorGUILayout.RectField(" ", array[i]);
                p.rects = array;
            }
            else
            {
                Debug.LogError("The selected function has an unserializable parameter!");
            }


            return p;
        }


        T[] AddArrayInputField<T>(IList<T> array, string argName)
        {
            int length = 0;
            if (array == null)
            {
                length = EditorGUILayout.IntField(" " + argName + " size ", 0);
                array = new T[length];
            }
            else
            {
                length = EditorGUILayout.IntField(" " + argName + " size ", array.Count);
                if (array.Count != length)
                {
                    T[] t = new T[length];
                    for (int i = 0; i < Mathf.Min(array.Count, length); i++)
                    {
                        t[i] = array[i];
                    }
                    array = t;
                }

            }
            return array.ToArray();
        }
    }
}