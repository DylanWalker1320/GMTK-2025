using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpCollector : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    private ParticleSystem potency;
    private List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
    private Transform collector;
    private TopDownPlayerMovement player;


    private void Start()
    {
        player = FindFirstObjectByType<TopDownPlayerMovement>();
        potency = GetComponent<ParticleSystem>();
        collector = GameObject.FindGameObjectWithTag("Collector").transform;
        potency.trigger.AddCollider(collector);
        StartCoroutine(DestroyParticleObject(destroyTime));
    }

    private void OnParticleTrigger()
    {
        int triggeredParticles = potency.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

        // Deletes individual particles once entering collector's collider
        for (int i = 0; i < triggeredParticles; i++)
        {
            player.experience += 1; // Increment player's experience by 1 for each collected particle
            ParticleSystem.Particle oneParticle = particles[i];
            oneParticle.remainingLifetime = 0f;
            particles[i] = oneParticle;
        }
        potency.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
    }

    private IEnumerator DestroyParticleObject(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        Destroy(gameObject);
    }
}
