using System.Collections;
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
    public GameObject sphere;

    [Tooltip("Only have to set for torpedo bullets.")]
    public GameObject m_explosionAudioObject;

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
                    GameObject newSphere = Instantiate(sphere, this.gameObject.transform);

                    ParticleSystem particleClone = Instantiate(m_explosionParticles);
                    particleClone.transform.position = transform.position;

                    //newSphere.transform.parent = m_explosionParticles.transform;
                    newSphere.transform.parent = particleClone.transform;
                    //m_explosionParticles.transform.parent = null;
                    particleClone.transform.parent = null;

                    //m_explosionParticles.Play();
                    particleClone.Play();

                    //CreateAudioObject(m_explosionParticles.transform.position);
                    CreateAudioObject(particleClone.transform.position);


                    //Destroy(m_explosionParticles.gameObject, m_explosionParticles.main.duration);
                    Destroy(particleClone.gameObject, particleClone.main.duration);
                    
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

                ParticleSystem particleClone = Instantiate(m_explosionParticles);
                particleClone.transform.position = transform.position;

                if (targetRigidbody != null)
                {
                    BasicAgent fish = targetRigidbody.GetComponent<BasicAgent>();
                    GameObject newSphere = Instantiate(sphere, targetRigidbody.transform);


                    newSphere.transform.parent = particleClone.transform;

                    //newSphere.transform.parent = m_explosionParticles.transform;

                    //sphere.transform.localScale = new Vector3(10, 10, 10);
                    //sphere.transform.position = this.gameObject.transform.position;
                    //sphere.transform.parent = null;
                    //if (fish.m_health != 0)
                    //{
                    //    int damage = CalculateDamage(targetRigidbody.position);
                    //    Debug.Log(damage + "damage");
                    //
                    //    fish.GetComponent<BasicAgent>().TakeDamage(damage);
                    //}
                }

                //m_explosionParticles.transform.parent = null;

                //m_explosionParticles.Play();
                particleClone.Play();

                //CreateAudioObject(m_explosionParticles.transform.position);
                CreateAudioObject(particleClone.transform.position);

                //Destroy(m_explosionParticles.gameObject, m_explosionParticles.main.duration);
                Destroy(particleClone.gameObject, particleClone.main.duration);
            }

            Destroy(gameObject); // Removing the bullet after it hit's the enemy.

        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("CritHitBox"))
        {
            other.GetComponentInParent<BasicAgent>().TakeDamage(m_bulletDamage * m_damageMultiplier);

            if (this.gameObject.CompareTag("Shell"))
            {
                Rigidbody targetRigidbody = other.gameObject.GetComponentInParent<Rigidbody>();

                ParticleSystem particleClone = Instantiate(m_explosionParticles);
                particleClone.transform.position = transform.position;

                if (targetRigidbody != null)
                {
                    BasicAgent fish = targetRigidbody.GetComponent<BasicAgent>();
                    GameObject newSphere = Instantiate(sphere, targetRigidbody.transform);

                    //newSphere.transform.parent = m_explosionParticles.transform;
                    newSphere.transform.parent = particleClone.transform;

                    //if (fish.m_health != 0)
                    //{
                    //    int damage = CalculateDamage(targetRigidbody.position);
                    //    Debug.Log(damage + "damage");
                    //
                    //    fish.GetComponent<BasicAgent>().TakeDamage(damage);
                    //}
                }

                //m_explosionParticles.transform.parent = null;
                particleClone.transform.parent = null;

                //m_explosionParticles.Play();
                particleClone.Play();

                //CreateAudioObject(m_explosionParticles.transform.position);
                CreateAudioObject(particleClone.transform.position);

                //Destroy(m_explosionParticles.gameObject, m_explosionParticles.main.duration);
                Destroy(particleClone.gameObject, particleClone.main.duration);
            }

            Destroy(gameObject); // Removing the bullet after it hit's the enemy.
        }
    }

    /// <summary>
    /// CreateAudioObject() will make the explosion audio game object that plays the underwater explosion sound.
    /// </summary>
    private void CreateAudioObject(Vector3 pos)
    {
        m_explosionAudioObject = GameObject.Instantiate(m_explosionAudioObject);
        m_explosionAudioObject.transform.position = pos;
    }
}
