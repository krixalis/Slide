using UnityEngine;

public class AimWeapon : MonoBehaviour
{

    private float maxRange;
    private RaycastHit hit;

	// Use this for initialization
	void Start ()
	{
	    maxRange = 25;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    var allObjectsInRange = Physics.OverlapSphere(transform.position, 25f);

	    foreach (Collider enemyCollider in allObjectsInRange)
	    {
	        if (enemyCollider != null && enemyCollider.tag == "Enemy")
	        {
                Physics.Raycast(transform.position, (enemyCollider.transform.position - transform.position), out hit, maxRange);
                Debug.DrawRay(transform.position, (hit.point - transform.position), Color.green);
	        }
	    }
	}
}
