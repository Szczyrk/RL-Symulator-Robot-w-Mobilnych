using UnityEngine;

public interface IRobot
{
    public string name { set; get; }
    public Vector3 position { set; get; }
    public Bounds bounds { get; }
    public void StartRobot();
    public void Stop();
    public void Reset();
    public GameObject gameObject { get; }
    public Transform transform { get; }
}
