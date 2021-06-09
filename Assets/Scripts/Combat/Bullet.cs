﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    public float m_bulletLifeTime = 0;

    private bool m_isActive = false;
    private float m_counter = 0.0f;

    public float m_explosionRadius = 5;//need to play around with this value
    public ParticleSystem m_explosionParticles;

    public int m_bulletDamage = 10;
    public int m_damageMultiplier = 2;

    // Start is called before the first frame update
    void Start()
    {
        m_isActive = true; // Setting to true so the timer can start.
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isActive)
        {
            m_counter += Time.deltaTime;

            if (m_counter >= m_bulletLifeTime)
            {
                // The bullet has reached it's max life time.
                if (this.gameObject.CompareTag("Shell"))
                {
                    Debug.Log("destroy torpedo");
                    m_explosionParticles.transform.parent = null;

                    m_explosionParticles.Play();

                    Destroy(m_explosionParticles.gameObject, m_explosionParticles.main.duration);
                }

                Destroy(gameObject);
            }
        }
    }

    private int CalculateDamage(Vector3 targetPosition)
    {
        // Create a vector from the shell to the target
        Vector3 explosionToTarget = targetPosition - transform.position;
        Debug.Log(explosionToTarget);

        // Calculate the distance from the shell to the target
        float explosionDistance = explosionToTarget.magnitude;
        Debug.Log(explosionDistance);

        // Calculate the proportion of the maximum distance (the explosionRadius)
        // the target is away
        float relativeDistance = (m_explosionRadius - explosionDistance) / m_explosionRadius;
        Debug.Log(relativeDistance);

        // Calculate damage as this proportion of the maximum possible damage
        float damage = relativeDistance * m_bulletDamage;
        Debug.Log(damage);

        // Make sure that the minimum damage is always 0
        damage = Mathf.Max(0f, (int)damage);
        Debug.Log(damage);

        return (int)damage;
    }

    private void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.layer == LayerMask.NameToLayer("DamageHitBox"))
        {
            other.GetComponentInParent<BasicAgent>().TakeDamage(m_bulletDamage);

            if (this.gameObject.CompareTag("Shell"))
            {
                Rigidbody targetRigidbody = other.gameObject.GetComponentInParent<Rigidbody>();

                if (targetRigidbody != null)
                {
                    BasicAgent fish = targetRigidbody.GetComponent<BasicAgent>();

                    if (fish.m_health != 0)
                    {
                        int damage = CalculateDamage(targetRigidbody.position);
                        Debug.Log(damage + "damage");

                        fish.GetComponent<BasicAgent>().TakeDamage(damage);
                    }
                }

                m_explosionParticles.transform.parent = null;

                m_explosionParticles.Play();

                Destroy(m_explosionParticles.gameObject, m_explosionParticles.main.duration);
            }

            Destroy(gameObject); // Removing the bullet after it hit's the enemy.

        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("CritHitBox"))
        {
            other.GetComponentInParent<BasicAgent>().TakeDamage(m_bulletDamage * m_damageMultiplier);

            if (this.gameObject.CompareTag("Shell"))
            {
                Rigidbody targetRigidbody = other.gameObject.GetComponentInParent<Rigidbody>();

                if (targetRigidbody != null)
                {
                    BasicAgent fish = targetRigidbody.GetComponent<BasicAgent>();

                    if (fish.m_health != 0)
                    {
                        int damage = CalculateDamage(targetRigidbody.position);
                        Debug.Log(damage + "damage");

                        fish.GetComponent<BasicAgent>().TakeDamage(damage);
                    }
                }

                m_explosionParticles.transform.parent = null;

                m_explosionParticles.Play();

                Destroy(m_explosionParticles.gameObject, m_explosionParticles.main.duration);
            }

            Destroy(gameObject); // Removing the bullet after it hit's the enemy.
        }
    }
}
