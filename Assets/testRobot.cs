using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRobot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Debug.Log("Vertical: " + vertical);
        Debug.Log("Horizontal: " + horizontal);
    }
}
