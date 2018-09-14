
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
using SMI;
namespace SMI
{

    /// <summary>
    /// GazeAction class. To implement gaze action, override DoAction method. (see Example_GazeInteractionDoAction.cs) 
    /// </summary>
    public class GazeAction : GazeMonoBehaviour
    {
        [Tooltip("Trigger for gaze action. Action is executed if gaze stays on the object.")]
        [SerializeField]
        KeyCode triggerA = KeyCode.Space;

        [Tooltip("Trigger for gaze action. Action is executed if gaze stays on the object.")]
        [SerializeField]
        KeyCode triggerB = KeyCode.RightShift;

        [Tooltip("Trigger defined in Input Manager.")]
        [SerializeField]
        string triggerDefinedInInputManager = "Fire1";
        

        [Tooltip("Switch object material while gaze stays on it.")]
        [SerializeField]
        protected Material gazeMaterial = null;

        [Tooltip("When action is executed, instantiate a particle object")]
        [SerializeField]
        protected GameObject particle = null;

        [Tooltip("When gaze enters the object, AudioSource.Play is called.")]
        [SerializeField]
        protected AudioSource selectSound = null;


        protected MeshRenderer meshrenderer = null;
        protected Material defaultMaterial = null;

        [Tooltip("If you want to activate gaze action of another GameObject, use this. When this object is being gazed at, the gazeParent also receives OnGazeStay().")]
        [SerializeField]
        protected GazeAction gazeParent = null;

        [Tooltip("Child gaze objects which also change materials and activate a gaze action.")]
        [SerializeField]
        protected GazeAction [] gazeChildren;


        protected bool isGazed = false;

        
        bool userDefinedTrigger = false;
        //Trigger defined by user
        public bool UserDefinedTrigger
        {
            get { return userDefinedTrigger; }
            set { userDefinedTrigger = value; }
        }

        // Use this for initialization
        public virtual void Start()
        {
            //Default material is stored to change the object material in OnGazeEnter.
            meshrenderer = GetComponent<MeshRenderer>();
            if (meshrenderer != null)
            {
                defaultMaterial = meshrenderer.material;
            }
        }

        // Update is called once per frame
        public virtual void Update()
        {

        }

        public override void OnGazeEnter(RaycastHit hitInformation)
        {
            //If this game object has a parent GazeAction object, its OnGazeEnter must be called.
            if (gazeParent != null)
            {
                gazeParent.OnGazeEnter(hitInformation);
            }
            
            isGazed = true;

            //Change material while the player gazes at the object.
            if (gazeMaterial != null && meshrenderer != null)
            {
                meshrenderer.material = gazeMaterial;
            }
            
            //If this game object has children GazeAction objects, their OnGazeEnter must be called.
            foreach (GazeAction child in gazeChildren)
            {
                child.GazeEnterChildren();
            }

            //Sound emission for OnGazeEnter
            if (selectSound != null)
            {
                selectSound.Play();
            }
        }

        public void GazeEnterChildren()
        {
            meshrenderer.material = gazeMaterial;
        }

        public override void OnGazeStay(RaycastHit hitInformation)
        {
            //If this game object has a parent GazeAction object, its OnGazeStay must be called.
            if (gazeParent != null)
            {
                gazeParent.OnGazeStay(hitInformation);
            }
            else
            {
                //When the trigger is pulled, do action
                if (Input.GetKeyDown(triggerA) || Input.GetKeyDown(triggerB) || Input.GetButtonDown(triggerDefinedInInputManager) || userDefinedTrigger)
                {
                    DoAction();
                    userDefinedTrigger = false;
                    //If this game object has children GazeAction objects, their DoAction must be called.
                    foreach (GazeAction child in gazeChildren)
                    {
                        child.DoAction();
                    }
                }
            }
        }

        public override void OnGazeExit()
        {
            //If this game object has a parent GazeAction object, its OnGazeExit must be called.
            if (gazeParent != null)
            {
                gazeParent.OnGazeExit();
            }
            isGazed = false;

            //When gaze exits from the object, revert the material to default.
            if (gazeMaterial != null && meshrenderer != null)
            {
                meshrenderer.material = defaultMaterial;
            }

            //If this game object has children GazeAction objects, their OnGazeExit must be called.
            foreach (GazeAction child in gazeChildren)
            {
                child.GazeExitChildren();
            }
        }
        public void GazeExitChildren()
        {
            meshrenderer.material = defaultMaterial;
        }

        protected virtual void DoAction()
        {
            if (particle != null)
            {
                //When the particle object is given, emit the particles by instantiating the particle object
                RaycastHit rayHit;                
                bool hasAHitpoint = SMI.SMIEyeTrackingUnity.Instance.smi_GetRaycastHitFromGaze(out rayHit);
                if (hasAHitpoint)
                {
                    Instantiate(particle, rayHit.point, Quaternion.identity);
                }
            }

        }

    }
}