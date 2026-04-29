using System.Collections.Generic;
using UnityEngine;

public class SmartWind : MonoBehaviour
{
    [Header("Rüzgar Ayarları")]
    public float windForce = 50f; 

    [Header("Engeller (Siperler)")]
    [Tooltip("Rüzgarı hangi katmanlar kesebilir? (Player ve Ground kesinlikle seçili olmalı)")]
    public LayerMask obstacleLayer;
    private List<Rigidbody2D> objectsInWind = new List<Rigidbody2D>();

    void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            objectsInWind.Add(rb);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null && objectsInWind.Contains(rb))
        {
            objectsInWind.Remove(rb);
        }
    }

    void FixedUpdate()
    {
        for (int i = objectsInWind.Count - 1; i >= 0; i--)
        {
            Rigidbody2D rb = objectsInWind[i];

            if (rb == null) 
            {
                objectsInWind.RemoveAt(i);
                continue;
            }

            Vector2 directionToObject = rb.transform.position - transform.position;
            float distance = directionToObject.magnitude;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToObject.normalized, distance, obstacleLayer);

            if (hit.collider == null || hit.collider.gameObject == rb.gameObject)
            {
                rb.AddForce(transform.right * windForce);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Vector3 startPos = transform.position;
        Vector3 windDirection = transform.right;

        float arrowLength = 2.5f; 
        Vector3 endPos = startPos + (windDirection * arrowLength);

        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(startPos, endPos);

        Vector3 arrowTip1 = endPos - (windDirection * 0.5f) + (transform.up * 0.5f);
        Vector3 arrowTip2 = endPos - (windDirection * 0.5f) - (transform.up * 0.5f);

        Gizmos.DrawLine(endPos, arrowTip1);
        Gizmos.DrawLine(endPos, arrowTip2);
    }
}