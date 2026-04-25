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
        // Mesafe kontrolünü yine Vector2 üzerinden yapıyoruz (Z farkı önemli değil)
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

        // Hedef pozisyonu belirle
        Vector3 targetPos = Switch ? OriginSpot.position : DestinationSpot.position;

        // Hareketi hesapla
        Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPos, Speed * Time.fixedDeltaTime);

        // Z'Yİ KORUMA NOKTASI: 
        // Yeni pozisyonu atarken objenin kendi mevcut Z değerini koruyoruz
        transform.position = new Vector3(nextPos.x, nextPos.y, transform.position.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Eğer platforma çarpan objenin tag'i "Character" ise
        if (collision.gameObject.CompareTag("Character"))
        {
            // Çarpışmanın açısını (Normal vektörünü) kontrol et.
            // Bu script platformun üzerinde olduğu için, karakter yukarıdan çarptığında
            // temas normalinin Y değeri aşağıyı (-1) gösterir.
            if (collision.contacts[0].normal.y < -0.5f)
            {
                // Karakteri platformun çocuğu yap
                collision.transform.SetParent(transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Character"))
        {
            // İlişkiyi kes (bağımsız yap)
            collision.transform.SetParent(null);
        }
    }
}