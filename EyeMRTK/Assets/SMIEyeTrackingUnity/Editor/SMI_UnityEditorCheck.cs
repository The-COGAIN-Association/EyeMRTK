
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

using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SMI
{
    /// <summary>
    /// Check Unity Editor platform and version
    /// </summary>
    [InitializeOnLoad]
    class UnityEditorCheck
    {
        // Warning popup message can be disabled if you set this value false.
        static bool SMIWarningForEditor = true;
        #region main
        static string SMITargetPlatform = "Windows64";
        static string SMISupportedUnityEditor = "64bitOnly";
        static string HelpMessage = "[SMI Unity] You can disable Editor warning messages from /SMIEyeTrackingUnity/Editor/SMI_UnityEditorCheck.cs.";
        static UnityEditorCheck()
        {
            if (SMIWarningForEditor)
            {
                if (SMISupportedUnityEditor == "64bitOnly")
                {
#if UNITY_EDITOR_32
                showWarningWindowWrongUnityVersion();
#endif
                }

                if (UnityVersionCheck(Application.unityVersion))
                    showWarningWindowOldUnityVersion();

                if (SMITargetPlatform == "Android")
                {
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                        showWarningWindowWrongPlatform("Android");
                }
                if (SMITargetPlatform == "Windows")
                {
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows)
                        showWarningWindowWrongPlatform("Windows");
                }

                if (SMITargetPlatform == "Windows64")
                {
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
                        showWarningWindowWrongPlatform("Windows64");
                }
            }
        }
        static void showWarningWindowWrongPlatform(string correctPlatform)
        {
            if (correctPlatform == "Android")
            {
                bool switchPlatform = EditorUtility.DisplayDialog("Wrong Platform", "Please switch to Android Platform", "Switch platform", "Not now");
                if (switchPlatform)
                {
#if UNITY_5_6
                    bool success = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
#else
                    bool success = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
#endif
                    if (!success)
                    {
                        Debug.LogError("Unity Android module is not installed. Please install Unity Android module.");
                    }
                }
                Debug.LogWarning(HelpMessage);
            }
            if (correctPlatform == "Windows")
            {
                bool switchPlatform = EditorUtility.DisplayDialog("Wrong Platform", "Please switch to Windows Platform", "Switch platform", "Not now");
                if (switchPlatform)
                {
#if UNITY_5_6
                    bool success = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
#else
                    bool success = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
#endif
                    if (!success)
                    {
                        Debug.LogError("Unity Windows module is not installed. Please install Unity Windows module.");
                    }
                }
                Debug.LogWarning(HelpMessage);

            }
//            if (correctPlatform == "Windows64")
//            {
//                bool switchPlatform = EditorUtility.DisplayDialog("Wrong Platform", "Please switch to 64 bit version Windows Platform", "Switch platform", "Not now");
//                if (switchPlatform)
//                {
//#if UNITY_5_6
//                    bool success = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
//#else
//                    bool success = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);
//#endif
//                    if (!success)
//                    {
//                        Debug.LogError("Unity Windows module is not installed. Please install Unity Windows module.");
//                    }
//                }
//                Debug.LogWarning(HelpMessage);
//
//            }
        }
        private static void showWarningWindowOldUnityVersion()
        {

            bool openWebpage = EditorUtility.DisplayDialog("Older Version of Unity detected!", 
                "The SMI-Eye Tracking SDK only supports Unity 5_2 or higher. Do you want to download the newest Version of the Unity Engine?", 
                "Download the newest Version", "Skip Download");

            if (openWebpage)
            {
                System.Diagnostics.Process.Start("http://unity3d.com/get-unity/download?ref=personal");
                EditorApplication.Exit(0);
            }
            Debug.LogWarning(HelpMessage);

        }
        private static void showWarningWindowWrongUnityVersion()
        {

            bool closeUnity = EditorUtility.DisplayDialog("Wrong Unity Editor!", 
                "The SMI-Eye Tracking SDK only supports 64 bit Unity.", 
                "Exit Unity Editor", "Ignore");
            if (closeUnity)
            {
                EditorApplication.Exit(0);
            }
            Debug.LogWarning(HelpMessage);

        }
        static bool UnityVersionCheck(string v)
        {
            string [] s = v.Split('.');
            if (int.Parse(s[0]) > 4) // Unity version higher than 4
            {
                return false;
            }
            else
            return true;
        }
#endregion
    }
}
