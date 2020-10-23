using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledTimeParticle: MonoBehaviour
{

	private ParticleSystem particle;
	private double lastTime;

	private void Awake() {
		particle = GetComponent<ParticleSystem>();
	}
    // Start is called before the first frame update
    void Start()
    {
		lastTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
		float deltaTime = Time.realtimeSinceStartup - (float)lastTime;
		particle.Simulate(deltaTime, true, false);
		lastTime = Time.realtimeSinceStartup;
    }
}
