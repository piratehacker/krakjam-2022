using System.Collections;
using UnityEngine;
namespace ParticleAttractor
{
	[RequireComponent(typeof(ParticleSystem))]
	public class particleAttractorSelf : MonoBehaviour
	{
		ParticleSystem ps;
		ParticleSystem.Particle[] m_Particles;
		public float speed = 5f;
		int numParticlesAlive;
		void Start()
		{
			ps = GetComponent<ParticleSystem>();
			if (!GetComponent<Transform>())
			{
				GetComponent<Transform>();
			}
		}
		void Update()
		{
			m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
			numParticlesAlive = ps.GetParticles(m_Particles);
			float step = speed * Time.deltaTime;
			for (int i = 0; i < numParticlesAlive; i++)
			{
				m_Particles[i].position = Vector3.SlerpUnclamped(m_Particles[i].position, m_Particles[i + 1].position, step);
			}
			ps.SetParticles(m_Particles, numParticlesAlive);
		}
	}
}