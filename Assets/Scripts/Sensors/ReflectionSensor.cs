using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReflectionSensor : Sensor
{
    private float proximity, check;
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
            proximity = maxRange;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 1, 0));
            Vector3 direction = rotation * startLight.forward;
            check = Cast(direction);
            if (check < proximity) proximity = check;
            direction = startLight.forward * proximity;
            distance = proximity;
            if (proximity < maxRange) detected = true;
            else detected = false;
            yield return new WaitForSeconds(updateDt);
        }
    }

    private float Cast(Vector3 direction)
    {
        Ray ray = new Ray(startLight.position, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, maxRange);

        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag("SPRITE"))
            {
                if (drawDebug)
                    Draw.Instance.Line(
                        transform.position,
                        hit.point,
                        Color.green);
                return hit.distance;
            }
            else
            {
                if (drawDebug)
                    Draw.Instance.Line(
                        transform.position,
                        transform.position + direction * maxRange,
                        Color.red);
            }
        }

        return maxRange;
    }

    void OnDrawGizmosSelected()
    {
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 1, 0));
        Vector3 direction = rotation * startLight.forward;
        Ray ray = new Ray(startLight.position, direction);
        
        RaycastHit[] hits = Physics.RaycastAll(ray, maxRange);
        foreach (var hit in hits)
        {
            if (hit.transform.CompareTag("SPRITE"))
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