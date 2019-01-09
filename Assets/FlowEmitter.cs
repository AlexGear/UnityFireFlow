using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FlowEmitter : MonoBehaviour {
    [SerializeField, Range(1, 25)] int n = 5;
    [SerializeField] float horSweep = 30;
    [SerializeField] float vertSweep = 30;
    [SerializeField] float flowDistance = 3.5f;
    [SerializeField] float directionVariance = 0.2f;
    [SerializeField, Range(0 ,1)] float distanceReductionFactorOnHit = 0.5f;
    [SerializeField] int maxHops = 100;
    [SerializeField] Color color = new Color32(253, 106, 2, 255);

    private void Update() {
        for(int i = 0; i < n; i++) {
            for(int j = 0; j < n; j++) {
                EmitRay(i, j);
            }
        }
    }

    private void EmitRay(int i, int j) {
        float horAngle = ((float)j / n - 0.5f) * horSweep;
        float vertAngle = ((float)i / n - 0.5f) * vertSweep;
        Vector3 origin = transform.position;
        Vector3 direction = Quaternion.Euler(vertAngle, horAngle, 0) * transform.forward;

        float remainingDistance = flowDistance;
        int hops = 0;
        do {
            Vector3 end;
            if(Physics.Raycast(origin, direction, out RaycastHit hit, remainingDistance)) {
                end = hit.point;
                Debug.DrawLine(origin, end, color);

                Vector3 normal = hit.normal;
                float distance = hit.distance;

                remainingDistance -= distance;
                remainingDistance *= (1f - distanceReductionFactorOnHit * Vector3.Dot(direction, -normal));

                origin = end;
                direction = Vector3.Cross(normal, Vector3.Cross(direction, normal));
                direction.Normalize();

                if(distance > 0.4f)
                    direction += normal * Random.Range(0.01f, directionVariance);
                else 
                    direction += normal * (directionVariance + Random.Range(0.01f, directionVariance));
            }
            else {
                end = origin + direction * remainingDistance;
                Debug.DrawLine(origin, end, color);

                remainingDistance = 0;
            }
        } while(remainingDistance > 0 && hops++ < maxHops);
    }
}
