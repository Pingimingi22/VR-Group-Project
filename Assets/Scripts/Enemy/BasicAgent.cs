using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAgent : MonoBehaviour
{

    public Transform m_target;
    public PlayerManager m_playerManager;
    public CombatManager m_combatManager;

    [Header("Agent Stats")]
    public int m_health = 100;
    public int m_maxHealth = 100;
    public float m_moveSpeed;
    public float m_stoppingDistance;
    public int m_damage;
    public int m_pointsReward = 5;


    [Header("Agent Effects")]
    public GameObject m_deathExplosion;


    private bool m_hasAttacked = false; // Just for testing purposes to prevent getting hit a million times in one frame.

    // Start is called before the first frame update
    void Start()
    {
        if (m_target == null)
        {
            // We have to manually find it.
            GameObject playerObj = GameObject.Find("Player");
            m_target = playerObj.transform;
            m_playerManager = playerObj.GetComponent<PlayerManager>();
        }
        if (m_combatManager == null)
        { 
            m_combatManager = GameObject.Find("Managers").GetComponent<CombatManager>(); 
        }
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
        else if(!m_hasAttacked)
        {
            // Otherwise, we are close enough to the player to start attacking.
            m_playerManager.RemoveHealth(m_damage);

            m_hasAttacked = true;

            

            Destroy(gameObject);
        }
	}

    private void LookAt(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        direction.Normalize();
        Quaternion newRot = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = newRot;
    }

	private void OnDestroy()
	{
        if (m_deathExplosion != null) // If we have a death explosion, use it.
        {
            GameObject particleEffect = Instantiate(m_deathExplosion);
            particleEffect.transform.position = transform.position;
        }
	}

    public void TakeDamage(int damage)
    {
        m_health -= damage;
        if (m_health <= 0)
        {
            Destroy(gameObject); // I know we shouldn't be destroying but hey it's just a prototype right? ... 
            GameManager.AddPoints(m_pointsReward);
        }

        Debug.Log("Enemy took damage");
    }

	private void OnTriggerEnter (Collider collision)
	{
        if (collision.gameObject.tag == "Bullet")
        {
            TakeDamage(m_combatManager.m_basicGunDamage);
            Destroy(collision.gameObject); // Removing the bullet after it hit's the enemy.
        }
	}


}
