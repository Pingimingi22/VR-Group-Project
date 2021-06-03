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
    //public int m_basicGunDamage = 10; // We no longer use the damage here. It's in the bullet script.
    public float m_bulletSpeed = 100.0f; // Maybe it will be hitscan?? So if it is hitscan I guess we wouldn't need a bullet speed. But for now I'll make it like this
                                         // So we can see bullets moving across the screen.

    [Tooltip("Time between shots.")]
    public float m_basicGunFireRate = 0.25f;

    [Header("Bullet Assets")]
    public GameObject m_basicBullet;
    public GameObject m_torpedo;

    [Header("Bullet Spawn Locations")]
    public Transform m_spawnLeft;
    public Transform m_spawnRight;

    [Header("Player references")]
    public Player.PlayerController m_playerController;
    public Transform m_shootTransform;


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

            GameObject newBullet1 = Instantiate(m_basicBullet, m_shootTransform.position, Quaternion.LookRotation(dirRay.direction));
            Debug.Log(dirRay.direction);
            //newBullet1.transform.position = m_shootTransform.position;

            Rigidbody bullet1Rigidbody = newBullet1.GetComponent<Rigidbody>();
            //bullet1Rigidbody.velocity = m_bulletSpeed * dirRay.direction;
            bullet1Rigidbody.AddForce(dirRay.direction * m_bulletSpeed, ForceMode.Impulse);

            // ------------------- //
            m_isBasicCooldown = true; // Setting to true to start the cooldown.
        }

    }

    public void FireTorpedo(Vector3 crosshairPos)
    {
        if (!m_isBasicCooldown) // Can only shoot if the gun is ready.
        {
            Ray dirRay = new Ray(Vector3.zero, crosshairPos - m_uiCamera.transform.position);

            // Maybe we can make two bullets per shot for the pew pew effect?

            GameObject newTorpedo1 = Instantiate(m_torpedo, m_shootTransform.position, Quaternion.LookRotation(dirRay.direction));
            Debug.Log(dirRay.direction);
            //m_torpedo.transform.position = m_shootTransform.position;

            Rigidbody torpedo1Rigidbody = newTorpedo1.GetComponent<Rigidbody>();
            //bullet1Rigidbody.velocity = m_bulletSpeed * dirRay.direction;
            torpedo1Rigidbody.AddForce(dirRay.direction * m_bulletSpeed, ForceMode.Impulse);

            // ------------------- //
            m_isBasicCooldown = true; // Setting to true to start the cooldown.
        }

    }

    private void OnDrawGizmos()
	{
        //Ray testRay = new Ray(Vector3.zero, testPos - m_uiCamera.transform.position);
    }
}
