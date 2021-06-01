using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DistanceSensorControllerTestCL : Sensor
{
    public float FOV = 20.1f;
    private float proximity, check;
    private ClassicalLogic _classicalLogic;
    float[] arrayDistance = new float[5];
    private float _maxRange;
    private Color[] _colors = new[] {Color.blue, Color.cyan, Color.green, Color.yellow, Color.magenta};
    private Color[] _colors2 = new[] {Color.magenta, Color.yellow, Color.green, Color.cyan, Color.blue};

    public void Start()
    {
        _classicalLogic = new ClassicalLogic(0f, maxRange, 3);
        _maxRange = maxRange;
        arrayDistance[0] = 0;
        int j = 1;
        for (float i = 0; i < maxRange; i = i + 0.001f)
        {
            int k = _classicalLogic.Fuzzy(i);
            if (k == j)
            {
                arrayDistance[j++] = i;
            }
        }

        arrayDistance[4] = maxRange;
        Debug.Log(String.Join(", ", arrayDistance));

        for (int i = 4; i > 0; i--)
        {
            maxRange = arrayDistance[i];
            proximity = maxRange;
            for (float a = -FOV; a < FOV; a += 0.5f)
            {
                Quaternion rotation = Quaternion.Euler(new Vector3(0, a, 0));
                Vector3 direction = rotation * startLight.forward;
                check = Cast(direction, _colors2[i]);
                if (check < proximity) proximity = check;
            }

            distance = proximity;
            direction = startLight.forward * proximity;
            if (proximity < maxRange) detected = true;
            else detected = false;
        }
    }

    public override void Enable()
    {
        scanning = true;
        StartCoroutine(UpdateData());
    }

    public override void Disable()
    {
        scanning = false;
    }


    private IEnumerator UpdateData()
    {
        while (scanning)
        {
            for (int i = 4; i > 1; i--)
            {
                maxRange = arrayDistance[i];
                proximity = maxRange;
                for (float a = -FOV; a < FOV; a += 0.5f)
                {
                    Quaternion rotation = Quaternion.Euler(new Vector3(0, a, 0));
                    Vector3 direction = rotation * startLight.forward;
                    check = Cast(direction, _colors2[i]);
                    if (check < proximity) proximity = check;
                }

                distance = proximity;
                direction = startLight.forward * proximity;
                if (proximity < maxRange) detected = true;
                else detected = false;
                yield return new WaitForSeconds(updateDt);
            }
        }
    }

    private float Cast(Vector3 direction, Color color)
    {
        Ray ray = new Ray(startLight.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxRange) && hit.transform != null &&
            !hit.transform.TryGetComponent(out ResetTrigger resetTrigger) &&
            !hit.transform.TryGetComponent(out Environment environment))
        {
            Draw.Instance.Line(
                transform.position,
                hit.point,
                Color.red);
            return hit.distance;
        }
        else
        {
            Draw.Instance.Line(
                transform.position,
                transform.position + direction * maxRange,
                color);
            return maxRange;
        }
    }

    void OnDrawGizmosSelected()
    {
        for (float a = -FOV; a < FOV; a += 2f)
        {
            Quaternion rotation = Quaternion.Euler(new Vector3(0, a, 0));
            Vector3 direction = rotation * startLight.forward;
            Ray ray = new Ray(startLight.position, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxRange) && hit.transform != null &&
                !hit.transform.TryGetComponent(out ResetTrigger resetTrigger) &&
                !hit.transform.TryGetComponent(out Environment environment))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, hit.point);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + direction * maxRange);
            }
        }


        if (Simulation.state == Simulation.State.edit)
            Gizmos.DrawRay(startLight.position,
                startLight.position +
                Quaternion.Euler(new Vector3(maxRange / Mathf.Sqrt(2), 0, maxRange / Mathf.Sqrt(2))) *
                startLight.forward);
    }
}