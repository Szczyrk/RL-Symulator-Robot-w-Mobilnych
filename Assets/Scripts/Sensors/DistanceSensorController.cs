using UnityEngine;
using System.Collections;

public class DistanceSensorController : Sensor
{
    public float FOV = 20.1f;
    private float proximity, check;

    public void CreatStartLight()
    {
        if (startLight == null)
        {
            startLight = new GameObject().transform;
            startLight.name = "Start Light";
            startLight.SetParent(this.transform);
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            startLight.localPosition = Rotation((transform.position - meshRenderer.bounds.center), meshRenderer) /
                                       transform.lossyScale.x
                                       + new Vector3(0, (meshRenderer.bounds.size.y + 0.001f) / transform.lossyScale.y,
                                           0);

            //startLight.localPosition = new Vector3(0, 0.0015f, 0);
            startLight.localRotation = Quaternion.Euler(-90, 90, 0);
        }
    }

    private Vector3 Rotation(Vector3 vector3, MeshRenderer newGameObject)
    {
        Matrix4x4 M = Matrix4x4.TRS(Vector3.one, newGameObject.transform.rotation, Vector3.one);
        return M.inverse * vector3 * -1;
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
            proximity = maxRange;
            for (float a = -FOV; a < FOV; a += 0.5f)
            {
                Quaternion rotation = Quaternion.Euler(new Vector3(0, a, 0));
                Vector3 direction = rotation * startLight.forward;
                check = Cast(direction);
                if (check < proximity) proximity = check;
            }

            distance = proximity;
            direction = startLight.forward * proximity;
            if (proximity < maxRange ) detected = true;
            else detected = false;
            yield return new WaitForSeconds(updateDt);
        }
    }

    private float Cast(Vector3 direction)
    {
        Ray ray = new Ray(startLight.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxRange) && hit.transform != null &&
            !hit.transform.TryGetComponent(out ResetTrigger resetTrigger) &&
            !hit.transform.TryGetComponent(out Environment environment))
        {
            if (drawDebug)
                Draw.Instance.Line(
                    transform.position,
                    hit.point,
                    Color.red);
            return hit.distance;
        }
        else
        {
            if (drawDebug)
                Draw.Instance.Line(
                    transform.position,
                    transform.position + direction * maxRange,
                    Color.green);
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