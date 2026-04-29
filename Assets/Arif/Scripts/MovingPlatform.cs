using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    public Transform DestinationSpot;
    public Transform OriginSpot;
    public float Speed = 2f;
    public bool Switch = false;

    private float precision = 0.01f;

    void FixedUpdate()
    {
        float distToDest = Vector2.Distance(transform.position, DestinationSpot.position);
        float distToOrigin = Vector2.Distance(transform.position, OriginSpot.position);

        if (distToDest < precision)
        {
            Switch = true;
        }
        else if (distToOrigin < precision)
        {
            Switch = false;
        }
        Vector3 targetPos = Switch ? OriginSpot.position : DestinationSpot.position;

        Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPos, Speed * Time.fixedDeltaTime);

        transform.position = new Vector3(nextPos.x, nextPos.y, transform.position.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Character"))
        {
            if (collision.contacts[0].normal.y < -0.5f)
            {
                collision.transform.SetParent(transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Character"))
        {
            collision.transform.SetParent(null);
        }
    }
}