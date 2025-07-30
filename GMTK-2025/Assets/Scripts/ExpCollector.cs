using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpCollector : MonoBehaviour
{
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
    }

    private void OnParticleTrigger()
    {
        int triggeredParticles = potency.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

        // Deletes individual particles once entering collector's collider
        for (int i = 0; i < triggeredParticles; i++)
        {
            // player.potencyAmount++; REPLACE WITH EXP OR UPGRADE SYSTEM
            ParticleSystem.Particle oneParticle = particles[i];
            oneParticle.remainingLifetime = 0f;
            particles[i] = oneParticle;
        }
        potency.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
        // Destroys particle system after collecting at least 1 particle (possible issue in the future but works for now)
        StartCoroutine(DestroyParticleObject(1.5f));
    }

    private IEnumerator DestroyParticleObject(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        Destroy(gameObject);
    }
}
