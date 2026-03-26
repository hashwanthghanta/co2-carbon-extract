using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFollower : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3.0f;
    private int currentTargetIndex = 0;

    void Update()
    {
        if (waypoints == null || currentTargetIndex >= waypoints.Length) return;

        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentTargetIndex].position, speed * Time.deltaTime);

        Vector3 direction = waypoints[currentTargetIndex].position - transform.position;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }

        if (Vector3.Distance(transform.position, waypoints[currentTargetIndex].position) < 0.1f)
        {
            currentTargetIndex++;
            if (currentTargetIndex >= waypoints.Length)
            {
                Destroy(gameObject);
            }
        }
    }
}
