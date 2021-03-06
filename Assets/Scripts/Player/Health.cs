﻿using System.Runtime.InteropServices;
using Assets.Scripts.Player;
using UnityEngine;

public class Health : MonoBehaviour
{

    private float _maxHealth;
    private float _currentHealth;

    private float _invincibilityTime;
    public float _elapsedInvincibility;
    private bool  _isInvincible;
    public float _timer;
    public CameraShake _cameraShake;

    // Use this for initialization
    private void Start()
    {
        _maxHealth = 3f; // 3 health for now, insta-death with insta-respawns instead?
        _currentHealth = _maxHealth;

        _invincibilityTime = 1.2f; // 1.2 Seconds of invincibility for now
        _elapsedInvincibility = 0f;

        _cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (_isInvincible) return;

        if (otherCollider.gameObject.tag == "Enemy" || otherCollider.gameObject.tag == "Trap")
        {
            _currentHealth -= 1f;
            _cameraShake.Shake(1f, 0.5f, 0.05f);
            GoInvincible();
        }
        
    }

    private void FixedUpdate()
    {
        if (isDead())
        {
            //Destroy(transform.root.gameObject); //Bad thing to do?
            transform.root.gameObject.SetActive(false);
            transform.root.gameObject.GetComponent<Renderer>().material.color = Color.red;
        }

        UpdateInvincibility();

        if (_isInvincible && transform.root.gameObject.GetComponent<Renderer>().material.color != Color.yellow)
        {
            transform.root.gameObject.GetComponent<Renderer>().material.color = Color.yellow; // Debug way of showing invincibility duration
        }
        else if (!_isInvincible && transform.root.gameObject.GetComponent<Renderer>().material.color == Color.yellow)
        {
            transform.root.gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    private bool isDead()
    {
        return _currentHealth <= 0f;
    }

    private void GoInvincible()
    {
        _timer = Time.time;
        _isInvincible = true;
    }

    private void UpdateTimer()
    {
        _elapsedInvincibility = Time.time - _timer;
    }

    private void UpdateInvincibility()
    {
        if (!_isInvincible) return;

        UpdateTimer();
        if (_elapsedInvincibility >= _invincibilityTime)
        {
            _isInvincible = false;
            _elapsedInvincibility = 0f;
        }
    }
}