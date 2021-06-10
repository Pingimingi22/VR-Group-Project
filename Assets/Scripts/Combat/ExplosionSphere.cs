using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSphere : MonoBehaviour
{
    public float m_explosionRadius = 5;
    public int m_bulletDamage = 10;
    public float m_explosionDamageFalloff = 1.0f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        { 
            if (other.GetComponent<BasicAgent>().m_health != 0)
            {
                int damage = CalculateDamage(other.gameObject.transform.position);
                Debug.Log(damage + "damage");

                other.GetComponent<BasicAgent>().TakeDamage(damage);
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
        float relativeDistance = ((m_explosionRadius - explosionDistance) / m_explosionRadius) * m_explosionDamageFalloff;
        Debug.Log(relativeDistance);

        // Calculate damage as this proportion of the maximum possible damage
        float damage = relativeDistance * m_bulletDamage;
        Debug.Log(damage);

        // Make sure that the minimum damage is always 0
        damage = Mathf.Max(0f, (int)damage);
        Debug.Log(damage);

        return (int)damage;
    }
}
