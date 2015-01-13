using UnityEngine;
using System.Collections;

public class PerstistanceScript : MonoBehaviour
{
    public static PerstistanceScript Persistance;

    private bool isActive;
    public bool IsActive
    {
        get { return isActive; }
        set
        {
            isActive = value;
            Debug.Log("isActive has been set to " + value);
        }
    }

	void Awake()
	{
	    isActive = true;
	    if (Persistance == null)
	    {
	        DontDestroyOnLoad(gameObject);
	    }
        else if (Persistance != this)
        {
            Destroy(gameObject);
        }

        if (!IsActive)
        {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        Debug.Log("Started up PersistanceScript! isActive: " + isActive);

        if (!isActive)
        {
            gameObject.SetActive(false);
        }
    }
}
