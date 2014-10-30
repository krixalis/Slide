using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{

    private bool _triggerSpikes;
    private bool _triggerRevert;
    private bool _spikesUp;

    public GameObject Spikes;

    // Use this for initialization
    void Start()
    {
        _triggerSpikes = false;
        _triggerRevert = false;
        _spikesUp = false;

        //TODO: Add if(Spikes == null) GetChild
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_triggerSpikes && !_spikesUp)
        {
            ActivateSpikes();
        }
        else if (_triggerRevert && _spikesUp)
        {
            RevertSpikes();
        }

        if (IsIdling())
        {
            Spikes.collider.enabled = false; //do not damage the player unless active
        }
    }

    private void ActivateSpikes()
    {
        animation.Play("SpikesTrigger");

        _spikesUp = true;
        _triggerSpikes = false;
    }

    private void RevertSpikes()
    {
        animation.PlayQueued("SpikesRevert");

        _spikesUp = false;
        _triggerRevert = false;

        Spikes.collider.enabled = true; // this trap is to punish the player for mindlessly changing direction on a spike platform
    }

    private bool IsIdling()
    {
        return !_spikesUp && !animation["SpikesRevert"].enabled && !animation["SpikesTrigger"].enabled;
    }

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (_triggerSpikes) return;

        if (otherCollider.gameObject.tag == "Player")
        {
            _triggerSpikes = true;
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        if (_triggerRevert) return;

        if (otherCollider.gameObject.tag == "Player")
        {
            _triggerRevert = true;
        }
    }
}
