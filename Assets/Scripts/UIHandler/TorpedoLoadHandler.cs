using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TorpedoLoadHandler : MonoBehaviour
{

    public Image m_loadingImage;
    public CombatManager m_combatManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_loadingImage.fillAmount = m_combatManager.m_torpedoGunCounter / m_combatManager.m_torpedoGunFireRate;
    }
}
