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
            otherCollider.gameObject.SetActive(false);
            otherCollider.gameObject.GetComponent<PerstistanceScript>().IsActive = false;
        }

    }
}
