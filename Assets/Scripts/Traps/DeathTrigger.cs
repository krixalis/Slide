using UnityEngine;

public class DeathTrigger : MonoBehaviour {

    private GameObject _player;

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.gameObject.tag == "Player")
        {
            if (_player == null)
            {
                _player = otherCollider.gameObject;
            }
            Destroy(_player);
        }
    }
}
