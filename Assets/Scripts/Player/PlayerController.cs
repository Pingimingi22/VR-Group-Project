using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Player
{ 
    public class PlayerController : MonoBehaviour
    {
        // -------------------------- Public Inspector Variables -------------------------- //
        [Header("Combat Manager")]
        public CombatManager m_combatManager;

        [Header("UI Camera")]
        public Camera m_uiCamera;

        [Header("Maybe useful stuff one day")]
        public Transform m_rightControllerAnchor;
        public Transform m_rightHandAnchor;

        [Header("Crosshair things")]
        public float m_crosshairSpeed = 0.75f; // idk sounds kinda normal?
        
        public Image m_crosshair;
        public Canvas m_canvas;

        [Header("Track movement stuff")]
        public float m_movementSpeed = 5;

        [Header("VR Stuff")]
        public Transform pointer;
        // -------------------------------------------------------------------------------- //


        // --------------- Input Things --------------- // 
        Vector3 m_crosshairPos = Vector3.zero; // This is what moving the joysticks will be changing. We will then apply this to the image and other stuff.

        float m_xDelta = 0;
        float m_yDelta = 0;

        private bool m_isPrimaryPressed = false;  // I can't seem to get OVRInput.GetUp() working so I'm going to make my own bool to check if it's pressed or not.
        // -------------------------------------------- //



        private LineRenderer m_lineRenderer;

        // ------------- Other Stuff ------------- // 

        private bool m_inEditor = false;

        // --------------------------------------- //

        private bool m_swappingToGameInput = false;

        private float testCounter = 0;// just for testing delete this later.

        // more test stuff delete this later
        public float xTest;
        public float yTest;
        public float zTest;

        // --------- Audio --------- // 
        [HideInInspector]
        public AudioSource m_basicGunAudioSource;

        [HideInInspector]
        public AudioSource m_torpedoAudioSource;

        [HideInInspector]
        public AudioSource m_torpedoReloadAudioSource;
        // ------------------------- //




        // Start is called before the first frame update
        void Start()
        {
            InitCrosshair();

            m_lineRenderer = gameObject.GetComponent<LineRenderer>();

            m_inEditor = Application.isEditor;


            m_basicGunAudioSource = GameObject.Find("TorpedoBasicGunAudio").GetComponent<AudioSource>();

            m_torpedoAudioSource = GameObject.Find("TorpedoGunAudio").GetComponent<AudioSource>();

            m_torpedoReloadAudioSource = GameObject.Find("TorpedoReloadAudio").GetComponent<AudioSource>();
        }
    
        // Update is called once per frame
        void Update()
        {
            OVRInput.Update();
            OVRInput.FixedUpdate();

            m_isPrimaryPressed = false;

            if (m_inEditor)
            {
                CrosshairInput(); // Use normal mouse/controller controls.
            }
            else
            { 
                VRPointerUpdate(); // Use VR controller input.
            }


            UpdateCrosshairImage();


            // This check prevents the shot of selecting a UI element from shooting bullets.
            if (m_swappingToGameInput)
            {
                if (Input.GetAxis("Fire1") == 0 || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
                {
                    // Doing this little extra check prevents the fire from selecting a UI element from shooting in game.
                    m_swappingToGameInput = false;
                }
            }


            if ((Input.GetAxis("Fire1") != 0 || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) && GameManager.m_hasGameStarted && !GameManager.m_isGameOver && !m_swappingToGameInput)
            {

                // Shoot.
                m_combatManager.Shoot(m_crosshair.transform.position);


                if(m_basicGunAudioSource.isPlaying == false)
                    m_basicGunAudioSource.Play();

                m_isPrimaryPressed = true;

            }
            else if ((Input.GetAxis("Fire2") != 0 || OVRInput.Get(OVRInput.Button.PrimaryTouchpad)) && GameManager.m_hasGameStarted && !GameManager.m_isGameOver && !m_swappingToGameInput && !m_combatManager.m_isTorpedoCooldown)
            {
                // Shoot.
                m_combatManager.FireTorpedo(m_crosshair.transform.position);
                Debug.Log("Fired torpedo.");

                //if (m_torpedoAudioSource.isPlaying == false)    // Not neccessary since the clip is longer than the reload so it's actually helpful to reset the play sound for the torpedo.
                    m_torpedoAudioSource.Play();
                m_torpedoReloadAudioSource.Play();
            }

            
            else if (GameManager.m_isGameOver)
            {
                GameOverInput();
            }
            else if (!GameManager.m_hasGameStarted) // If the game hasn't started.
            {
                MainMenuInput();
            }



            if (m_inEditor)
            {
                if (Input.GetAxis("Fire1") == 0 || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || GameManager.m_isGameOver)
                {
                    m_basicGunAudioSource.Stop();
                }
            }
            else
            {
                if(!m_isPrimaryPressed || GameManager.m_isGameOver)
                {
                    m_basicGunAudioSource.Stop();
                }
            }

            if (((Input.GetAxis("Fire2") == 0) || OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad)) || GameManager.m_isGameOver)
            {
                // Actually we don't want to stop the torpedo sound when they lift the trigger up. We want it to play out completely.

                //m_torpedoAudioSource.Stop();
            }

            // ------------ Line renderer stuff to help with debugging ------------ //

            //Ray ray = new Ray(pointer.position, pointer.forward);
            //m_lineRenderer.SetPosition(0, ray.origin);
            //m_lineRenderer.SetPosition(1, ray.origin + 100 * ray.direction);



            
            // blend shape testing stuff. delete this later.

            //if (Input.GetKey(KeyCode.R))
            //{
            //    blendCounter += Time.deltaTime;
            //    if (blendCounter >= 0.05f)
            //    {
            //        currentBlendWeight += 1;
            //        blendCounter = 0;
            //    }
            //    m_testBlendshapes.SetBlendShapeWeight(0, currentBlendWeight);
            //}
            //if (Input.GetKey(KeyCode.T))
            //{
            //    blendCounter += Time.deltaTime;
            //    if (blendCounter >= 0.05f)
            //    {
            //        currentBlendWeight -= 1;
            //        blendCounter = 0;
            //    }
            //    m_testBlendshapes.SetBlendShapeWeight(0, currentBlendWeight);
            //}
        }

        /// <summary>
        /// CrosshairInput() handles crosshair movement with keyboard/controller input. 
        /// </summary>
        void CrosshairInput()
        {
            m_xDelta = Input.GetAxis("Horizontal");
            m_yDelta = Input.GetAxis("Vertical");

            if (m_xDelta != 0 || m_yDelta != 0)
            {
                Debug.Log("Moving crosshair");
                // Player is moving joystick.

                float canvasWidth = m_canvas.GetComponent<RectTransform>().rect.width;
                float canvasHeight = m_canvas.GetComponent<RectTransform>().rect.height;

                Vector3 direction = new Vector3(m_xDelta, m_yDelta, 0);
                direction.Normalize();

                

                
                m_crosshairPos += direction * m_crosshairSpeed * Time.deltaTime;


                // Making it so they can't take the crosshair off screen.
                if (m_crosshairPos.x > (m_canvas.transform.localPosition.x + canvasWidth / 2))
                {
                    m_crosshairPos.x = m_canvas.transform.localPosition.x + canvasWidth / 2;
                }
                if (m_crosshairPos.x < (m_canvas.transform.localPosition.x - canvasWidth / 2))
                {
                    m_crosshairPos.x = m_canvas.transform.localPosition.x - canvasWidth / 2;
                }
                if (m_crosshairPos.y > (m_canvas.transform.localPosition.y + canvasHeight / 2))
                {
                    m_crosshairPos.y = m_canvas.transform.localPosition.y + canvasHeight / 2;
                }
                if (m_crosshairPos.y < (m_canvas.transform.localPosition.y - canvasHeight / 2))
                {
                    m_crosshairPos.y = m_canvas.transform.localPosition.y - canvasHeight / 2;
                }

            }
        }

        void UpdateCrosshairImage()
        {
            if (m_inEditor)
            {
                m_crosshair.transform.localPosition = m_crosshairPos;
            }
            else
            {
                m_crosshair.transform.position = m_crosshairPos;
            }
        }

        /// <summary>
        /// InitCrosshair() will just make sure that the crosshair starts in the centre of the screen.
        /// </summary>
        void InitCrosshair()
        {
            m_crosshair.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// PrototypeMovement() is just going to move the player forward constantly. I'm using this just to see how movement will look like with the agents coming towards us at the same itme.
        /// </summary>
        void PrototypeMovement()
        {
            transform.position += Vector3.forward * Time.deltaTime * m_movementSpeed;
        }



        /// <summary>
        /// VRPointerUpdate() will be used to move the crosshair with the Go controller. Just for testing since I can't connect my xbox controller to the headset right now.
        /// </summary>
        void VRPointerUpdate()
        {

            Matrix4x4 subSpace = transform.root.localToWorldMatrix.inverse;
            Matrix4x4 worldSpace = transform.root.localToWorldMatrix;

            Vector3 pointerSubSpace = subSpace * pointer.position;
            Vector3 canvasSubSpace = subSpace * m_canvas.transform.position;

            Ray ray = new Ray(pointer.position, pointer.forward);
            RaycastHit hit;

            bool hasHit = false;

            //Plane testPlane = new Plane(-transform.forward, m_originalCanvasDistance);
            Plane testPlane = new Plane(-transform.forward, transform.position + (transform.forward * Vector3.Distance(m_canvas.transform.position, transform.root.position)));


            
             

            float enter;
            if (testPlane.Raycast(ray, out enter))
            {
                //if (hit.transform.tag == "SubmarineViewport")
                //{
                //    // Means the user is targetting a location on the viewport, so we should move the crosshair there.
                float canvasWidth = m_canvas.GetComponent<RectTransform>().rect.width;
                float canvasHeight = m_canvas.GetComponent<RectTransform>().rect.height;
                Debug.Log(canvasWidth);

                bool isXSet = false;
                bool isYSet = false;
                bool isZSet = false;

                // The reason for these if statements is so that when the cursor is moved outside of the submarines viewport, it doesn't just stop moving completely. It will keep moving but be confined to
                // the minimum and maximum extents of the view port.

                Vector3 point = subSpace * ray.GetPoint(enter);
                Vector3 worldPoint = worldSpace * point;
                Vector3 worldCanvas = worldSpace * canvasSubSpace;

                if (point.x > canvasSubSpace.x + (canvasWidth / 2))
                {
                    Vector3 edgePoint = new Vector3(canvasSubSpace.x + (canvasWidth / 2), canvasSubSpace.y, canvasSubSpace.z);
                    //edgePoint = worldSpace * edgePoint;
                    //m_crosshairPos.x = m_canvas.transform.position.x + (canvasWidth / 2);
                    m_crosshairPos.x = edgePoint.x;
                    isXSet = true;

                    //m_crosshairPos.z = edgePoint.z;
                    //isZSet = true;
                }
                else if (point.x < canvasSubSpace.x - (canvasWidth / 2))
                {
                    Vector3 edgePoint = new Vector3(canvasSubSpace.x - (canvasWidth / 2), canvasSubSpace.y, canvasSubSpace.z);
                    //edgePoint = worldSpace * edgePoint;
                    //m_crosshairPos.x = m_canvas.transform.position.x - (canvasWidth / 2);
                    m_crosshairPos.x = edgePoint.x;
                    isXSet = true;

                    //m_crosshairPos.z = edgePoint.z;
                    //isZSet = true;
                }
                
                if (point.y > canvasSubSpace.y + (canvasHeight / 2))
                {
                    Vector3 edgePoint = new Vector3(canvasSubSpace.x, canvasSubSpace.y + (canvasHeight / 2), canvasSubSpace.z);
                    //edgePoint = worldSpace * edgePoint;
                
                    //m_crosshairPos.y = m_canvas.transform.position.y + (canvasHeight / 2);
                    m_crosshairPos.y = edgePoint.y;
                    isYSet = true;

                    //m_crosshairPos.z = edgePoint.z;
                    //isZSet = true;
                }
                else if (point.y < canvasSubSpace.y - (canvasHeight / 2))
                {
                
                    Vector3 edgePoint = new Vector3(canvasSubSpace.x, canvasSubSpace.y - (canvasHeight / 2), canvasSubSpace.z);
                    //edgePoint = worldSpace * edgePoint;
                
                    //m_crosshairPos.y = m_canvas.transform.position.y + (canvasHeight / 2);
                    m_crosshairPos.y = edgePoint.y;
                    isYSet = true;

                    //m_crosshairPos.z = edgePoint.z;
                    //isZSet = true;
                }


                Vector3 cursorPos = subSpace * ray.GetPoint(enter);

                // Making sure the crosshair has been set to something.
                if (!isYSet)
                {
                    m_crosshairPos.y = cursorPos.y;
                }
                if (!isXSet)
                {
                    m_crosshairPos.x = cursorPos.x;
                }
                      
                m_crosshairPos.z = cursorPos.z;

                m_crosshairPos = worldSpace * m_crosshairPos;

   
               
                //m_crosshairPos = ray.GetPoint(enter);


  

                //hasHit = true;
                //}
            }
            else
            {
                //m_crosshairPos += Vector3.forward * Time.deltaTime * m_movementSpeed;
            }

            //if (!hasHit)
            //{
            //    // If we havn't hit anything, that means we haven't updated the crosshair position which will make the crosshair go backwards.
            //    // This is a really dodgy fix but whatever.
            //    m_crosshairPos += Vector3.forward * Time.deltaTime * m_movementSpeed; // Making it keep up with the player's forward movement.
            //}
        }

        private void MainMenuInput()
        {
            // Just for testing I'm going to handle the start game user interface stuff here.
            Vector3 buttonPos;
            if(m_inEditor)
                buttonPos = GameManager.m_startGameButtonS.transform.localPosition;                     // ==================================== NOTE ====================================== // 
            else                                                                                        // Really dodgy but we have to use different position for different input.
                buttonPos = GameManager.m_startGameButtonS.transform.position;


            Rect buttonRect = GameManager.m_startGameButtonS.GetComponent<RectTransform>().rect;
            float buttonWidth = buttonRect.width;
            float buttonHeight = buttonRect.height;

            if (m_crosshairPos.x <= buttonPos.x + (buttonWidth / 2) && m_crosshairPos.x >= buttonPos.x - (buttonWidth / 2))
            {
                // We are inside the horizontal plane of the button.
                if (m_crosshairPos.y <= buttonPos.y + (buttonHeight / 2) && m_crosshairPos.y >= buttonPos.y - (buttonHeight / 2))
                {
                    // We are inside both the horizontal and vertical plane of the button. AKA the crosshair is overlapping the button.
                    GameManager.m_startGameButtonS.Select();
                    GameManager.m_startGameButtonS.OnSelect(null);
                    //GameManager.m_startGameButtonS.OnPointerEnter(null);

                    if (Input.GetAxis("Fire1") != 0 || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                    {
                        m_swappingToGameInput = true;

                        GameManager.m_startGameButtonS.onClick.Invoke();
                        Debug.Log("Button click registered.");
                    }
                }

                else
                {
                    GameManager.m_startGameButtonS.OnDeselect(null);
                }

            }


            else
            {
                GameManager.m_startGameButtonS.OnDeselect(null);
            }
        }

        private void GameOverInput()
        {
            Vector3 buttonPos;
            if (m_inEditor)
                buttonPos = GameManager.m_retryButtonS.transform.localPosition;
            else
                buttonPos = GameManager.m_retryButtonS.transform.position;

            Rect buttonRect = GameManager.m_retryButtonS.GetComponent<RectTransform>().rect;
            float buttonWidth = buttonRect.width;
            float buttonHeight = buttonRect.height;

            if (m_crosshairPos.x <= buttonPos.x + (buttonWidth / 2) && m_crosshairPos.x >= buttonPos.x - (buttonWidth / 2))
            {
                // We are inside the horizontal plane of the button.
                if (m_crosshairPos.y <= buttonPos.y + (buttonHeight / 2) && m_crosshairPos.y >= buttonPos.y - (buttonHeight / 2))
                {
                    // We are inside both the horizontal and vertical plane of the button. AKA the crosshair is overlapping the button.
                    GameManager.m_retryButtonS.Select();
                    GameManager.m_retryButtonS.OnSelect(null);
                    //GameManager.m_startGameButtonS.OnPointerEnter(null);

                    if (Input.GetAxis("Fire1") != 0 || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                    {
                        m_swappingToGameInput = true;
    
                        GameManager.m_retryButtonS.onClick.Invoke();
                        Debug.Log("Button click registered.");
                    }
                }

                else
                {
                    GameManager.m_retryButtonS.OnDeselect(null);
                }

            }


            else
            {
                GameManager.m_retryButtonS.OnDeselect(null);
            }
        }




		private void OnDrawGizmos()
		{
            float canvasWidth = m_canvas.GetComponent<RectTransform>().rect.width;
            float canvasHeight = m_canvas.GetComponent<RectTransform>().rect.height;

            Matrix4x4 subSpace = transform.localToWorldMatrix.inverse;
            Matrix4x4 worldSpace = transform.root.localToWorldMatrix;

            Vector3 canvasSubSpace = subSpace * m_canvas.transform.position;

            Vector3 xRight = new Vector3(canvasSubSpace.x + (canvasWidth / 2), canvasSubSpace.y, canvasSubSpace.z);
            xRight = worldSpace * xRight;

            Gizmos.DrawSphere(xRight, 0.1f);

            Vector3 centre = worldSpace * canvasSubSpace;
            Gizmos.DrawSphere(centre, 0.1f);

            Vector3 edgePoint1 = new Vector3(canvasSubSpace.x, canvasSubSpace.y - (canvasHeight / 2), canvasSubSpace.z);
            edgePoint1 = worldSpace * edgePoint1;

            Gizmos.DrawSphere(edgePoint1, 0.1f);

            Vector3 edgePoint2 = new Vector3(canvasSubSpace.x - (canvasWidth / 2), canvasSubSpace.y, canvasSubSpace.z);
            edgePoint2 = worldSpace * edgePoint2;

            Gizmos.DrawSphere(edgePoint2, 0.1f);


            Ray ray = new Ray(pointer.position, pointer.forward);
            //Ray ray = new Ray(pointer.position, new Vector3(pointer.forward.x + xTest, pointer.forward.y + yTest, pointer.forward.z + zTest));

            Plane testPlane = new Plane(-transform.forward, transform.position + (transform.forward * Vector3.Distance(m_canvas.transform.position, transform.root.position)));
            
            float enter;
            testPlane.Raycast(ray, out enter);

            
            Gizmos.DrawLine(transform.position, transform.position + (transform.forward * Vector3.Distance(m_canvas.transform.position, transform.root.position)));
            

            Color colourCache = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.05f);





            bool isXSet = false;
            bool isYSet = false;
            Vector3 point = subSpace * ray.GetPoint(enter);
            Vector3 modifiedGizmoPos = Vector3.zero;

            if (point.x > canvasSubSpace.x + (canvasWidth / 2))
            {
                Vector3 edgePoint = new Vector3(canvasSubSpace.x + (canvasWidth / 2), canvasSubSpace.y, canvasSubSpace.z);
                //edgePoint = worldSpace * edgePoint;
                //m_crosshairPos.x = m_canvas.transform.position.x + (canvasWidth / 2);
                modifiedGizmoPos.x = edgePoint.x;
                isXSet = true;
            
                //m_crosshairPos.z = edgePoint.z;
                //isZSet = true;
            }
            else if (point.x < canvasSubSpace.x - (canvasWidth / 2))
            {
                Vector3 edgePoint = new Vector3(canvasSubSpace.x - (canvasWidth / 2), canvasSubSpace.y, canvasSubSpace.z);
                //edgePoint = worldSpace * edgePoint;
                //m_crosshairPos.x = m_canvas.transform.position.x - (canvasWidth / 2);
                modifiedGizmoPos.x = edgePoint.x;
                isXSet = true;
            
                //m_crosshairPos.z = edgePoint.z;
                //isZSet = true;
            }
            
            if (point.y > canvasSubSpace.y + (canvasHeight / 2))
            {
                Vector3 edgePoint = new Vector3(canvasSubSpace.x, canvasSubSpace.y + (canvasHeight / 2), canvasSubSpace.z);
                //edgePoint = worldSpace * edgePoint;
            
                //m_crosshairPos.y = m_canvas.transform.position.y + (canvasHeight / 2);
                modifiedGizmoPos.y = edgePoint.y;
                isYSet = true;
            
                //m_crosshairPos.z = edgePoint.z;
                //isZSet = true;
            }
            else if (point.y < canvasSubSpace.y - (canvasHeight / 2))
            {
            
                Vector3 edgePoint = new Vector3(canvasSubSpace.x, canvasSubSpace.y - (canvasHeight / 2), canvasSubSpace.z);
                //edgePoint = worldSpace * edgePoint;
            
                //m_crosshairPos.y = m_canvas.transform.position.y + (canvasHeight / 2);
                modifiedGizmoPos.y = edgePoint.y;
                isYSet = true;
            
                //m_crosshairPos.z = edgePoint.z;
                //isZSet = true;
            }

            Vector3 testRayPoint = subSpace * ray.GetPoint(enter);

            if (!isXSet)
                modifiedGizmoPos.x = testRayPoint.x;
            if (!isYSet)
                modifiedGizmoPos.y = testRayPoint.y;

            modifiedGizmoPos.z = testRayPoint.z;

            Gizmos.DrawSphere(worldSpace * modifiedGizmoPos, 0.1f);

            //Gizmos.DrawSphere(ray.GetPoint(enter), 0.05f);




            Gizmos.color = colourCache;
        }

        void TestRotate()
        {
            testCounter += Time.deltaTime;

            if (testCounter >= 0.01f)
            { 
                Quaternion rotationEuler = Quaternion.Euler(transform.rotation.x, transform.rotation.y + 1, transform.rotation.z);
                transform.Rotate(0, 0.25f, 0);
                testCounter = 0;
            }
        }
	}
}
