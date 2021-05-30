using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAgent : MonoBehaviour
{

    public Transform m_target;

    [Header("Agent Stats")]
    public float m_moveSpeed;
    public float m_stoppingDistance;
    public float m_damage;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        LookAt(m_target.position);
    }


	public void Move()
	{
        // The basic enemy for now will just go straight from wherever they spawn to the player.

        if (Vector3.Distance(transform.position, m_target.position) >= m_stoppingDistance)
        { 
            // If we aren't at the stopping location, keep moving towards the player.

            Vector3 direction = m_target.position - transform.position;
            direction.Normalize();

            transform.position += direction * m_moveSpeed * Time.deltaTime;
        }
	}

    private void LookAt(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        direction.Normalize();
        Quaternion newRot = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = newRot;
    }
}
