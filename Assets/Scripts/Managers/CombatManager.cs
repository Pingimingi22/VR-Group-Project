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

    [Header("Bullet Assets")]
    public GameObject m_basicBullet;

    [Header("Bullet Spawn Locations")]
    public Transform m_spawnLeft;
    public Transform m_spawnRight;


    // test delete this.
    public Vector3 testPos;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Shoot(Vector3 crosshairPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(crosshairPos);

        // Maybe we can make two bullets per shot for the pew pew effect?

        GameObject newBullet1 = Instantiate(m_basicBullet);
        newBullet1.transform.position = Vector3.zero;

        Plane testPlane = new Plane(new Vector3(0, -1, 0), 10);
        Ray testRay = new Ray(Vector3.zero, testPos - m_uiCamera.transform.position);



        Rigidbody bullet1Rigidbody = newBullet1.GetComponent<Rigidbody>();
        bullet1Rigidbody.AddForce(testRay.direction * m_bulletSpeed, ForceMode.Impulse);

    }

	private void OnDrawGizmos()
	{
        Ray testRay = new Ray(m_spawnLeft.position, testPos);
        Gizmos.DrawRay(testRay);
	}
}
