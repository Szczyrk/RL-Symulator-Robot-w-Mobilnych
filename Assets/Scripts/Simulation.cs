using UnityEngine;
using System.Collections.Generic;

public class Simulation : MonoBehaviour {


    public enum State {
        menu,
		starting,
		simulating,
		stopped,
        edit,
		end
	}

	private class Observer : IObjectDimensions {
		public Observer() {}
		
		public string name {
			get { return "Simulation"; }
		}
		
		public Bounds bounds {
			get {
				if (isRunning) {
					Bounds b = new Bounds();
					b.Encapsulate(robotSelected.position);
					return b;
				} 
				else {
					return Simulation.Instance.bounds;
				}
			}
		}
	}


	static Simulation() {
		testArea = new Observer();
	}


	public static Simulation Instance;
	

	public static State state {get; set;}


	public static IRobot robotSelected {
		get { return _robotSelected; }
		private set {
		//	if(_robotSelected) _robotSelected.transform.Recycle();
			_robotSelected = value;
		}
	}
    public static List<IRobot> robots 
    {
        get { return _robots; }
        private set
        {
            _robots = value;
        }
    }
    public static List<string> namesRobotInSimulation;

    public static GameObject environment {
		get {
			return _environment.gameObject;
		}
		set {
			if (_environment) _environment.transform.Recycle();
			_environment = value.GetComponent<Environment>();
			SetBounds();
		}
	}
	
	

	public static IObjectDimensions testArea {
		get; private set;
	}
	
	public static bool isRunning {
		get { return state == State.simulating; }
	}


	

	public static bool paused {
		get { return _paused; }
		set {
			_paused = value;
			if (_paused) Time.timeScale = 0f;
			else Time.timeScale = timeScale;
		}
	}

	public static float timeScale {
		get { return _timeScale; }
		set {
			_timeScale = value;
			if (!paused) {
				Time.timeScale = value;
			}
		}
	}
	
	private static IRobot _robotSelected;
    private static List<IRobot> _robots = new List<IRobot>();
    private static Environment _environment;
	private static bool _paused;
	private static float _timeScale = 1f;


	public static void Enter() {
		CamController.AddViewMode(CamController.ViewMode.Birdseye);
		CamController.AddViewMode(CamController.ViewMode.FreeMovement);
		CamController.AddViewMode(CamController.ViewMode.Orbit);
		CamController.AddAreaOfInterest(testArea);
	}
	

	public static void Run() {
		Debug.Log("Simulation run.");
		if (state == State.stopped || state == State.starting || state == State.menu) {
			Time.timeScale = _timeScale;
			state = State.simulating;
		}
	}

    public static void Starting()
    {
        Debug.Log("Simulation starting.");
        if (state == State.stopped || state == State.menu)
        {
            state = State.starting;
        }
    }


    public static void Pause() {
		Debug.Log("Simulation Pause.");
		if (state == State.simulating || state == State.starting) {
            Time.timeScale = 0f;
			state = State.stopped;
		}
	}
	
	

	
	private static void SetBounds() {
		Bounds b = new Bounds();
		foreach(Renderer r in environment.GetComponentsInChildren<Renderer>())
			b.Encapsulate(r.bounds);
			
		Instance.bounds = b;
	}

	public Bounds bounds {
		get; private set;
	}
	
	

	void Awake() {
		if (Instance) {
			Destroy(this.gameObject);
		}
		else {
			Instance = this;
		}
	}
	

	void Start()
    {
        state = State.menu;
        namesRobotInSimulation = new List<string>();
        DontDestroyOnLoad(this);
        robots.AddRange(FindObjectsOfType<RobotAgent>());
        foreach (IRobot robot in robots)
        {

	        robot.StartRobot();
        }
        Run();
    }
	
	

    void Update() {
        if (Input.GetMouseButtonDown(0) && state != State.edit)
        {

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity);
            if (hit)
            {
                if (hitInfo.transform.GetComponentInParent<IRobot>() != null)
                {
	                IRobot robot = hitInfo.transform.GetComponentInParent<IRobot>();
                    if (robot != robotSelected)
                    {
                        robotSelected = robot;
                        Debug.Log("Robot selected: " + robotSelected.name);
                    }
                }
            }
        }
    }
    

    void OnDestroy()
    {
        state = State.end;
    }
}
