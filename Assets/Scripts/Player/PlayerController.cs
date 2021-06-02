using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

            PrototypeMovement();

            if (Input.GetAxis("Fire1") != 0 || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                // Shoot.
                m_combatManager.Shoot(m_crosshair.transform.position);
            }
            else if (Input.GetAxis("Fire2") != 0)
            {
                // Shoot.
                m_combatManager.FireTorpedo(m_crosshair.transform.position);
            }



            // ------------ Line renderer stuff to help with debugging ------------ //

            //Ray ray = new Ray(pointer.position, pointer.forward);
            //m_lineRenderer.SetPosition(0, ray.origin);
            //m_lineRenderer.SetPosition(1, ray.origin + 100 * ray.direction);
        }

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
                if (m_crosshairPos.x > (m_canvas.transform.position.x + canvasWidth / 2))
                {
                    m_crosshairPos.x = m_canvas.transform.position.x + canvasWidth / 2;
                }
                if (m_crosshairPos.x < (m_canvas.transform.position.x - canvasWidth / 2))
                {
                    m_crosshairPos.x = m_canvas.transform.position.x - canvasWidth / 2;
                }
                if (m_crosshairPos.y > (m_canvas.transform.position.y + canvasHeight / 2))
                {
                    m_crosshairPos.y = m_canvas.transform.position.y + canvasHeight / 2;
                }
                if (m_crosshairPos.y < (m_canvas.transform.position.y - canvasHeight / 2))
                {
                    m_crosshairPos.y = m_canvas.transform.position.y - canvasHeight / 2;
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


                m_crosshairPos.z = ray.GetPoint(enter).z;


                //m_crosshairPos = ray.GetPoint(enter);

                //hasHit = true;
                //}
            }
            else
            {
                m_crosshairPos += Vector3.forward * Time.deltaTime * m_movementSpeed;
            }

            //if (!hasHit)
            //{
            //    // If we havn't hit anything, that means we haven't updated the crosshair position which will make the crosshair go backwards.
            //    // This is a really dodgy fix but whatever.
            //    m_crosshairPos += Vector3.forward * Time.deltaTime * m_movementSpeed; // Making it keep up with the player's forward movement.
            //}
        }
    }
}
