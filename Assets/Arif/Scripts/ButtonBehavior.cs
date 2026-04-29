using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    public GameObject openDoor;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Character"))
        {
            openDoor.SetActive(true);
        }
        else
        {
            openDoor.SetActive(false);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Character"))
        {
            openDoor.SetActive(false);
        }
        else
        {
            openDoor.SetActive(false);
        }
    }  
    
}
