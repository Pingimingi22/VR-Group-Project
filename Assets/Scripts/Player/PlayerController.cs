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

        // -------------------------------------------------------------------------------- //

        Vector3 m_crosshairPos = Vector3.zero; // This is what moving the joysticks will be changing. We will then apply this to the image and other stuff.


        float m_xDelta = 0;
        float m_yDelta = 0;


        // Start is called before the first frame update
        void Start()
        {
            InitCrosshair();

            
        }
    
        // Update is called once per frame
        void Update()
        {
            CrosshairInput();
            UpdateCrosshairImage();

            if (Input.GetAxis("Fire1") != 0)
            {
                // Shoot.
                m_combatManager.Shoot(m_crosshairPos);
            }



            // test delete this
            m_combatManager.testPos = m_crosshair.transform.position;
        }

        void CrosshairInput()
        {
            m_xDelta = Input.GetAxis("Horizontal");
            m_yDelta = Input.GetAxis("Vertical");

            if (m_xDelta != 0 || m_yDelta != 0)
            {
                // Player is moving joystick.
                Vector3 direction = new Vector3(m_xDelta, m_yDelta, 0);
                direction.Normalize();
                m_crosshairPos += direction * m_crosshairSpeed * Time.deltaTime;
            }
        }

        void UpdateCrosshairImage()
        {
            m_crosshair.transform.localPosition = m_crosshairPos;
        }

        /// <summary>
        /// InitCrosshair() will just make sure that the crosshair starts in the centre of the screen.
        /// </summary>
        void InitCrosshair()
        {
            m_crosshair.transform.localPosition = Vector3.zero;
        }
    }
}
