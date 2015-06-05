using UnityEditor;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _origin;
    private bool _shake;
    private float _intensity;
    private float _interval;
    private float _elapsedInterval;
    private float _decrease;

    private Vector2 _randomPosition;

	// Use this for initialization
    void Start () {
        _origin = new Vector3(0, 0, 0);
        _shake = false;
        _elapsedInterval = 0f;
    }
	
	// Update is called once per frame
    void Update ()
    {
        if(_shake) Shake();
    }

    private void Shake()
    {
        if (_intensity > 0)
        {
            _randomPosition = Random.insideUnitCircle*_intensity;
            _intensity -= _decrease;
        }
        else
        {
            _shake = false;
            transform.localPosition = _origin;
            return;
        }
        var newPosition = new Vector3(_randomPosition.x, _randomPosition.y, 0);
        transform.localPosition = newPosition;
    }

    public void Shake(float intensity, float interval, float decrease)
    {
        _intensity = intensity;
        _interval = interval;
        _decrease = decrease;
        _shake = true;
    }
}
