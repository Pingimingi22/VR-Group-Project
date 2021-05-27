using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheck : MonoBehaviour
{

    public GameObject m_testCube;

    private bool m_pressed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && !m_pressed)
        {
            m_pressed = true;
            ToggleCube();
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            m_pressed = false;
        }
    }

    void ToggleCube()
    {
        if (m_testCube.activeSelf)
        {
            m_testCube.SetActive(false);
        }
        else
        {
            m_testCube.SetActive(true);
        }
    }



}
