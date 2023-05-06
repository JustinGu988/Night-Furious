using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private ParticleSystem destroyParticles;
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private AudioSource hornSound;
    [SerializeField] private AudioSource crashSound;

    [SerializeField] private AudioSource collectPowerUpSound;

    // Start is called before the first frame update
    void Start()
    {
        initEngineSound();
    }

    // Update is called once per frame
    void Update()
    {
        playEngineSound();

        // Play horn
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!hornSound.isPlaying) hornSound.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If health is zero destroy car and create destroyParticle effect at car location
        // Else show collision particles at collision location

        if (collision.gameObject.tag == "PowerUp")
        {
            collectPowerUpSound.Play();
        }
        else
        {
            //var location = collision.collider.ClosestPointOnBounds(transform.position);
            //var location = collision.collider.ClosestPointOnBounds(collision.transform.position);
            var location = collision.GetContact(0).point;
            var particles = Instantiate(collisionParticles);

            particles.transform.position = location;

            // Play Crash sound
            if (!crashSound.isPlaying)
            {
                var force = collision.impulse.magnitude;
                crashSound.volume = Mathf.Max(force / 20000.0f, 0.1f);

                //Debug.Log(force);
                crashSound.Play();
            }
        }
    }

    private void initEngineSound()
    {
        engineSound.playOnAwake = true;
        engineSound.loop = true;
    }

    private void playEngineSound()
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        float maxPitch = 3.0f;
        float pitch = Mathf.Min(maxPitch, 0.5f + rigidBody.velocity.magnitude * 0.1f);
        engineSound.pitch = pitch;
    }
}
