//	MIT License
//
//	Copyright (c) 2018 Diako Mardanbegi
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//	of this software and associated documentation files (the "Software"), to deal
//	in the Software without restriction, including without limitation the rights
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//	copies of the Software, and to permit persons to whom the Software is
//	furnished to do so, subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all
//	copies or substantial portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//	SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RayCastTrail : MonoBehaviour {
	[SerializeField]

	[Tooltip("leave empty if you want to use the raw combined gaze ray")]
	public ProcessGaze _processGaze;

	[SerializeField]
	[Tooltip("The color of the particle")]
	private Color _color = Color.green;

	[SerializeField]
	[Range(0, 1000)]
	[Tooltip("The number of particles to allocate. Use zero to use only the last hit object.")]
	private int _particleCount = 100;

	[SerializeField]
	[Range(0.005f, 0.2f)]
	[Tooltip("The size of the particle.")]
	public float _particleSize = 0.05f;

	[SerializeField]
	[Tooltip("Turn gaze trail on or off.")]
	public bool _on = true;


	public enum DepthType
	{
		constant_always_visible,
		constant_only_when_hit ,
		hit_depth_only_when_hit
	}

	public DepthType depthType;

	[SerializeField]
	[Tooltip("particle distance relative to head")]
	public float _particleDepth = 1.00f;



	/// <summary>
	/// Turn gaze trail on or off.
	/// </summary>
	public bool On
	{
		get
		{
			return _on;
		}

		set
		{
			_on = value;
			OnSwitch();
		}
	}

	/// <summary>
	/// Set particle count between 1 and 1000.
	/// </summary>
	public int ParticleCount
	{
		get
		{
			return _particleCount;
		}

		set
		{
			if (value < 0 || value > 1000)
			{
				return;
			}

			_particleCount = value;
			CheckCount();
		}
	}

	public Color ParticleColor
	{
		get
		{
			return _color;
		}

		set
		{
			_color = value;
		}
	}

	/// <summary>
	/// Get the latest hit object.
	/// </summary>
	public Transform LatestHitObject
	{
		get
		{
			return _latestHitObject;
		}
	}

	private bool _lastOn;
	private int _lastParticleCount;
	private int _particleIndex;
	private ParticleSystem.Particle[] _particles;
	private ParticleSystem _particleSystem;
	private bool _particlesDirty;
	private Transform _latestHitObject;
	private bool _removeParticlesWhileCalibrating;






	private void Awake()
	{
		OnAwake();
	}

	private void Start()
	{
		OnStart();
	}

	protected virtual void OnAwake()
	{
	}

	protected virtual void OnStart()
	{
		_lastParticleCount = _particleCount;
		_particles = new ParticleSystem.Particle[_particleCount];
		_particleSystem = GetComponent<ParticleSystem>();


	
	}

	private void Update()
	{
		if (_particlesDirty)
		{
			_particleSystem.SetParticles(_particles, _particles.Length);
			_particlesDirty = false;
		}

		CheckCount();

		OnSwitch();


		// Reset the flag when no longer calibrating.
		_removeParticlesWhileCalibrating = true;

		if (_on)
		{
			Ray ray= GetRay();


			RaycastHit hit;
			Vector3 pos=Vector3.zero;
			if (depthType == DepthType.constant_always_visible) {

				pos = ray.GetPoint (_particleDepth);
				PlaceParticle (pos, _color, _particleSize);
			}
			else if ( depthType == DepthType.hit_depth_only_when_hit  && Physics.Raycast (ray, out hit)) {
				pos = hit.point;


				PlaceParticle (pos, _color, _particleSize);
				_latestHitObject = hit.transform;

			}			
			else if ( depthType == DepthType.constant_only_when_hit  && Physics.Raycast (ray, out hit)) {

				pos = ray.GetPoint (_particleDepth);

				PlaceParticle (pos, _color, _particleSize);
				_latestHitObject = hit.transform;

			}
			else {

				RemoveParticles();
				_latestHitObject = null;


			}
			
		}
	}

	private void CheckCount()
	{
		if (_lastParticleCount != _particleCount)
		{
			RemoveParticles();
			_particleIndex = 0;
			_particles = new ParticleSystem.Particle[_particleCount];
			_lastParticleCount = _particleCount;
		}
	}

	private void OnSwitch()
	{
		if (_lastOn && !_on)
		{
			// Switch off.
			RemoveParticles();
			_lastOn = false;
		}
		else if (!_lastOn && _on)
		{
			// Switch on.
			_lastOn = true;
		}
	}
	public Color GetColor()
	{
		return _color;
	}
	private void RemoveParticles()
	{
		for (int i = 0; i < _particles.Length; i++)
		{
			PlaceParticle(Vector3.zero, Color.white, 0);
		}
	}

	private void PlaceParticle(Vector3 pos, Color color, float size)
	{
		if (_particleCount < 1)
		{
			return;
		}




		var particle = _particles[_particleIndex];
		particle.position = pos;
		particle.startColor = color;
		particle.startSize = size;
		_particles[_particleIndex] = particle;
		_particleIndex = (_particleIndex + 1) % _particles.Length;
		_particlesDirty = true;
	}

	private  Ray GetRay()
	{



		return (_processGaze!=null)? _processGaze._ray_processed: default(Ray) ; 

	
	}
}
