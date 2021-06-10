using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyUIHandler : MonoBehaviour
{
    [Header("UI Stuff")]
    public Image m_healthbarImage;
    public Image m_healthbarBackground;

    private BasicAgent m_agent;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = gameObject.GetComponent<BasicAgent>();
        Debug.Assert(m_agent); // Just making sure we found the agent script.

        m_healthbarImage.enabled = false;
        m_healthbarBackground.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (m_healthbarImage.enabled == false && m_agent.m_health < m_agent.m_maxHealth)
        {
            m_healthbarImage.enabled = true;
            m_healthbarBackground.enabled = true;
        }

        float fillAmount = (float)m_agent.m_health / (float)m_agent.m_maxHealth;


        m_healthbarImage.fillAmount = fillAmount;
    }
}
