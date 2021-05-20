using UnityEngine;


abstract public class Sensor : MonoBehaviour {

	public bool drawDebug;	
    public bool detected;
    public Vector3 direction;
    public float distance;
    public float maxRange = 20.1f;
    public float updateDt = 0.2f;
    public Transform startLight;
    public bool scanning { get; set; }
    abstract public void Enable();

	abstract public void Disable();
}
