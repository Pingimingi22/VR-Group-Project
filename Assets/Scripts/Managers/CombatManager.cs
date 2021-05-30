using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CombatManager maybe should be a singleton since we only want one. Probably implment this eventually.
/// </summary>
public class CombatManager : MonoBehaviour
{

    [Header("Cameras")]
    public Camera m_mainCamera;
    public Camera m_uiCamera;

    [Header("Gun/Bullet Stats:")]
    public float m_basicGunDamage = 10.0f;
    public float m_bulletSpeed = 100.0f; // Maybe it will be hitscan?? So if it is hitscan I guess we wouldn't need a bullet speed. But for now I'll make it like this
                                         // So we can see bullets moving across the screen.

    [Tooltip("Time between shots.")]
    public float m_basicGunFireRate = 0.25f;

    [Header("Bullet Assets")]
    public GameObject m_basicBullet;

    [Header("Bullet Spawn Locations")]
    public Transform m_spawnLeft;
    public Transform m_spawnRight;


    // Timer stuff.
    private float m_basicGunCounter = 0.0f;
    private bool m_isBasicCooldown = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isBasicCooldown)
        {
            // The basic gun has been fired and now has to cooldown.
            m_basicGunCounter += Time.deltaTime;
            if (m_basicGunCounter >= m_basicGunFireRate)
            {
                // The cooldown duration has elapsed.
                m_isBasicCooldown = false;
                m_basicGunCounter = 0;
            }
        }
    }


    public void Shoot(Vector3 crosshairPos)
    {
        if (!m_isBasicCooldown) // Can only shoot if the gun is ready.
        { 
            Ray dirRay = new Ray(Vector3.zero, crosshairPos - m_uiCamera.transform.position);

            // Maybe we can make two bullets per shot for the pew pew effect?

            GameObject newBullet1 = Instantiate(m_basicBullet);
            newBullet1.transform.position = Vector3.zero;

            Rigidbody bullet1Rigidbody = newBullet1.GetComponent<Rigidbody>();
            bullet1Rigidbody.AddForce(dirRay.direction * m_bulletSpeed, ForceMode.Impulse);

            // ------------------- //
            m_isBasicCooldown = true; // Setting to true to start the cooldown.
        }

    }

	private void OnDrawGizmos()
	{
        //Ray testRay = new Ray(Vector3.zero, testPos - m_uiCamera.transform.position);
    }
}
