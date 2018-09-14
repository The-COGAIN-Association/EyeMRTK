
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

namespace SMI
{

    /// <summary>
    /// Example MonoBehaviour class for drawing gaze cursor in game scene.
    /// </summary>
    public class SMI_Example_DrawGazePosition : MonoBehaviour
    {
        //GameObject for gaze cursor
        GameObject gazeVis = null;
        Vector3 initialeScale = Vector3.zero;

        SMI.SMIEyeTrackingUnity smiInstance = null;

        void Start()
        {
            smiInstance = SMIEyeTrackingUnity.Instance;

            gazeVis = (GameObject)Resources.Load("SMI_GazePoint");
            if (gazeVis != null)
            {
                gazeVis = Instantiate(gazeVis, Vector3.zero, Quaternion.identity) as GameObject;
                gazeVis.name = "SMI_GazePoint";
                initialeScale = gazeVis.transform.localScale;
            }
            else
            {
                Debug.LogError("Unity Prefab missing: SMI_GazePoint");
            }
        }

        void Update()
        {
            if(gazeVis != null)
                UpdateThePosition();
        }

        private void UpdateThePosition()
        {

            gazeVis.SetActive(true);

            RaycastHit hitInformation;

            //Get raycast from gaze
            smiInstance.smi_GetRaycastHitFromGaze(out hitInformation);
            if (hitInformation.collider != null)
            {
                gazeVis.transform.position = hitInformation.point;
                gazeVis.transform.localRotation = smiInstance.transform.rotation;
                gazeVis.transform.localScale = initialeScale * hitInformation.distance;
                gazeVis.transform.LookAt(Camera.main.transform);
                gazeVis.transform.transform.rotation *= Quaternion.Euler(0, 180, 0);
            }
            else
            {
                //If the raycast does not collide with any object, put it far away.
                float distance = 100;
                Vector3 scale = initialeScale * distance;
                Ray gazeRay = smiInstance.smi_GetRayFromGaze();
                
                gazeVis.transform.position = gazeRay.origin + Vector3.Normalize(gazeRay.direction) * distance;
                gazeVis.transform.rotation = smiInstance.transform.rotation;
                if (gazeRay.direction != Vector3.zero)
                    gazeVis.transform.localScale = scale;
                else
                    gazeVis.transform.localScale = Vector3.zero;
            }

            //Toggle the gaze cursor by Key "g"
            if (Input.GetKeyDown(KeyCode.G))
            {
                gazeVis.GetComponent<MeshRenderer>().enabled = !(gazeVis.GetComponent<MeshRenderer>().enabled);
            }

        }
    }
}