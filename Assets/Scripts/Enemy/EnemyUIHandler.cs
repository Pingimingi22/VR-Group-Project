using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyUIHandler : MonoBehaviour
{
    [Header("UI Stuff")]
    public Image m_healthbarImage;

    private BasicAgent m_agent;

    // Start is called before the first frame update
    void Start()
    {
        m_agent = gameObject.GetComponent<BasicAgent>();
        Debug.Assert(m_agent); // Just making sure we found the agent script.
    }

    // Update is called once per frame
    void Update()
    {
        float fillAmount = m_agent.m_health / m_agent.m_maxHealth;

        m_healthbarImage.fillAmount = fillAmount;
    }
}
