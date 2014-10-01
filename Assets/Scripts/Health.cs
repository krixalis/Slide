﻿using System.Timers;
using UnityEngine;

public class Health : MonoBehaviour
{

    private float _maxHealth;
    private float _currentHealth;

    private float _invincibilityTime;
    public float _elapsedInvincibility;
    private bool  _isInvincible;
    public float _timer;

    // Use this for initialization
    private void Start()
    {
        _maxHealth = 3f; // 3 health for now, insta-death with insta-respawns instead?
        _currentHealth = _maxHealth;

        _invincibilityTime = 3f; // 3 Seconds of invincibility for now
        _elapsedInvincibility = 0f;
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (_isInvincible) return;
        
        if (otherCollider.gameObject.tag == "Enemy")
        {
            _currentHealth -= 1f;
            GoInvincible();
        }
        
    }

    private void FixedUpdate()
    {
        if (isDead())
        {
            transform.root.gameObject.renderer.material.color = Color.red;
        }

        UpdateInvincibility();
    }

    private bool isDead()
    {
        return _currentHealth <= 0f;
    }

    private void GoInvincible()
    {
        _timer = Time.time;
        _isInvincible = true;
        Debug.Log("Now went invincible.");
    }

    private void UpdateTimer()
    {
        _elapsedInvincibility = Time.time - _timer;
    }

    private void UpdateInvincibility()
    {
        UpdateTimer();
        if (_elapsedInvincibility >= _invincibilityTime)
        {
            _isInvincible = false;
            _elapsedInvincibility = 0f;
            Debug.Log("No longer invincible.");
        }
        Debug.Log("Invincibility updated.");
    }
}