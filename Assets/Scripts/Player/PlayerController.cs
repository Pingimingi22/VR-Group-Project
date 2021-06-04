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

        Vector3 m_crosshairPos = Vector3.zero; // This is what moving the joysticks will be changing. We will then apply this to the image and other stuff.


        float m_xDelta = 0;
        float m_yDelta = 0;

        private LineRenderer m_lineRenderer;



        private bool m_inEditor = false;


        private bool m_swappingToGameInput = false;

        // Start is called before the first frame update
        void Start()
        {
            InitCrosshair();

            m_lineRenderer = gameObject.GetComponent<LineRenderer>();

            m_inEditor = Application.isEditor;
        }
    
        // Update is called once per frame
        void Update()
        {

            if (m_inEditor)
            {
                CrosshairInput(); // Use normal mouse/controller controls.
            }
            else
            { 
                VRPointerUpdate(); // Use VR controller input.
            }
            UpdateCrosshairImage();

            //PrototypeMovement();


            

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


            }
            else if (Input.GetAxis("Fire2") != 0)
            {
                // Shoot.
                m_combatManager.FireTorpedo(m_crosshair.transform.position);
                Debug.Log("Fired torpedo.");
            }


            else if (GameManager.m_isGameOver)
            {
                GameOverInput();
            }
            else if (!GameManager.m_hasGameStarted) // If the game hasn't started.
            {
                MainMenuInput();
            }
            

            // ------------ Line renderer stuff to help with debugging ------------ //

            //Ray ray = new Ray(pointer.position, pointer.forward);
            //m_lineRenderer.SetPosition(0, ray.origin);
            //m_lineRenderer.SetPosition(1, ray.origin + 100 * ray.direction);
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
            Ray ray = new Ray(pointer.position, pointer.forward);
            RaycastHit hit;

            bool hasHit = false;

            Plane testPlane = new Plane(new Vector3(0, 0, -1), Vector3.Distance(m_canvas.transform.position, Vector3.zero));


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

                // The reason for these if statements is so that when the cursor is moved outside of the submarines viewport, it doesn't just stop moving completely. It will keep moving but be confined to
                // the minimum and maximum extents of the view port.

                if (ray.GetPoint(enter).x > m_canvas.transform.position.x + (canvasWidth / 2))
                {
                    m_crosshairPos.x = m_canvas.transform.position.x + (canvasWidth / 2);
                    isXSet = true;
                }
                else if (ray.GetPoint(enter).x < m_canvas.transform.position.x - (canvasWidth / 2))
                {
                    m_crosshairPos.x = m_canvas.transform.position.x - (canvasWidth / 2);
                    isXSet = true;
                }

                if (ray.GetPoint(enter).y > m_canvas.transform.position.y + (canvasHeight / 2))
                {
                    m_crosshairPos.y = m_canvas.transform.position.y + (canvasHeight / 2);
                    isYSet = true;
                }
                else if (ray.GetPoint(enter).y < m_canvas.transform.position.y - (canvasHeight / 2))
                {
                    m_crosshairPos.y = m_canvas.transform.position.y - (canvasHeight / 2);
                    isYSet = true;
                }


                // Making sure the crosshair has been set to something.
                if (!isYSet)
                    m_crosshairPos.y = ray.GetPoint(enter).y;
                if (!isXSet)
                    m_crosshairPos.x = ray.GetPoint(enter).x;


                m_crosshairPos.z = m_canvas.transform.position.z;


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
    }
}
