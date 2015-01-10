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

        _invincibilityTime = 1.2f; // 3 Seconds of invincibility for now
        _elapsedInvincibility = 0f;
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (_isInvincible) return;

        if (otherCollider.gameObject.tag == "Enemy" || otherCollider.gameObject.tag == "Trap")
        {
            _currentHealth -= 1f;
            GoInvincible();
        }
        
    }

    private void FixedUpdate()
    {
        if (isDead())
        {
            Destroy(transform.root.gameObject);
            transform.root.gameObject.renderer.material.color = Color.red;
        }

        UpdateInvincibility();

        if (_isInvincible && transform.root.gameObject.renderer.material.color != Color.yellow)
        {
            transform.root.gameObject.renderer.material.color = Color.yellow; // Debug way of showing invincibility duration
        }
        else if (!_isInvincible && transform.root.gameObject.renderer.material.color == Color.yellow)
        {
            transform.root.gameObject.renderer.material.color = Color.white;
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