using Assets.Scripts.Player.Powerups;
using UnityEngine;

public class ReceivePowerup : MonoBehaviour {
    private void Start()
    {
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.gameObject.tag == "PowerUp")
        {
            PowerUpManager.Evaluate(otherCollider.name);
            Destroy(otherCollider.gameObject);
        }

    }
}
