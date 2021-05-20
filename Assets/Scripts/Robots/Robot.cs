using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Robot : MonoBehaviour, IObjectDimensions, IRobot {


	public bool 		manualControl;
    public float		stopDistance;	
	public Transform 	destination;	
	public Vector3 		centerOfMassOffset;

    Rigidbody rigidbody;
    public MotorController[] 	motors;
    public Sensor[] sensors;
	private Vector3 	_size;
    private Vector3 startPostion;
    private Quaternion startRotation;




    public Vector3 position {
		get{ return transform.position; }
		set{ transform.position = value; }
	}
	

	public Bounds bounds {
		get {
			return new Bounds(transform.position, _size);
		}
	}
	
	public bool atDestination {
		get {
			if (!destination) return false;
			float distance = Vector3.Distance(transform.position, destination.position);
			if (distance < stopDistance) return true;
			return false;
		}
	}

	
	public void Reset() {
        transform.localPosition = new Vector3(startPostion.x + UnityEngine.Random.Range(-0.25f,0.25f),startPostion.y, startPostion.z + UnityEngine.Random.Range(-0.25f,0.25f));
        transform.localRotation = startRotation;
        
        if (rigidbody != null)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

    public void UpdateEquipment()
    {
        motors = GetComponentsInChildren<MotorController>();
        sensors = GetComponentsInChildren<Sensor>();
    }


    private void Start()
    {
        startPostion = transform.localPosition;
        startRotation = transform.rotation;
        motors = GetComponentsInChildren<MotorController>();
        for(int i = 0; i< motors.Length; i++)
        {
            motors[i].powerMotor = 0f;
        }
        sensors = GetComponentsInChildren<Sensor>();
        rigidbody = GetComponent<Rigidbody>();
        if(!Simulation.robots.Contains(this))
            Simulation.robots.Add(this);
    }
    
    

    public void StartRobot()
    {
        StartSensor();

    }

    public void Stop()
    {
        for (int i = 0; i < motors.Length; i++)
        {
            motors[i].powerMotor = 0;
        }
        StopSensor();
    }

    public void StartSensor()
    {
        foreach (Sensor sensor in sensors)
            sensor.Enable();
    }

    public void StopSensor()
    {
        foreach (Sensor s in sensors)
        {
            s.Disable();
        }
    }

	
	private void Awake() {


		Bounds b = new Bounds(Vector3.zero, Vector3.zero);
		foreach(Renderer r in GetComponentsInChildren<Renderer>())
			b.Encapsulate(r.bounds);
		_size = b.size;

		GetComponent<Rigidbody>().centerOfMass += centerOfMassOffset;
	}

    private void ManualControl()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Debug.Log("Vertical" + vertical);
        Debug.Log("Horizontal: " + horizontal);
        if (vertical == 0)
        {
            if (horizontal > 0)
            {
                motors[1].powerMotor = -20f;
                motors[0].powerMotor = 20f;
            }
            else if (horizontal < 0)
            {
                motors[1].powerMotor = 20f;
                motors[0].powerMotor = -20f;
            }
            else
            {
                motors[1].powerMotor = 0f;
                motors[0].powerMotor = 0f;
            }
        }else if (vertical > 0)
        {
            if (horizontal > 0)
            {
                motors[1].powerMotor = 20f;
                motors[0].powerMotor = 10f;
            }
            else if (horizontal < 0)
            {
                motors[1].powerMotor = 10f;
                motors[0].powerMotor = 20f;
            }
            else
            {
                motors[1].powerMotor = 20f;
                motors[0].powerMotor = 20f;
            }
        }
        else
        {
            if (horizontal > 0)
            {
                motors[1].powerMotor = -10f;
                motors[0].powerMotor = -20f;
            }
            else if (horizontal < 0)
            {
                motors[1].powerMotor = -20f;
                motors[0].powerMotor = -10f;
            }
            else
            {
                motors[1].powerMotor = -20f;
                motors[0].powerMotor = -20f;
            } 
        }


    }

	protected void Update() {

        if (manualControl)
        {
            ManualControl();
        }

        OnUpdate();
		 if (Simulation.isRunning){
            foreach (MotorController motor in motors)
            {
                motor.Run();
            }
        }
    }
    
    public virtual void OnUpdate(){}
    private void OnDrawGizmos() {

		Gizmos.color = Color.Lerp(Color.yellow, Color.clear, 0.25f);
		if (Application.isPlaying) {
			Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass, 0.05f);
		} else {
			Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass + centerOfMassOffset, 0.05f);
		}
		
	}
}
