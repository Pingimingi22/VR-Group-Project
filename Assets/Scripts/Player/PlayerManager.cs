﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int m_maxPlayerHealth = 100;

    public int m_playerHealth;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveHealth(int damage)
    {
        m_playerHealth -= damage;
        if (m_playerHealth < 0)
            m_playerHealth = 0;
    }

    public void GainHealth(int health)
    {
        m_playerHealth += health;
        if (m_playerHealth > m_maxPlayerHealth)
            m_playerHealth = m_maxPlayerHealth;
    }
}
