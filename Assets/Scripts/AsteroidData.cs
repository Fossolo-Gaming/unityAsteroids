using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidData : MonoBehaviour
{
    public int size;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Shield")
        {
            GameObject player = collision.gameObject.transform.parent.parent.gameObject;
            player.GetComponent<PlayerController>().TakeDamage();            
        }
        else if(collision.gameObject.tag == "Player"){
            collision.gameObject.GetComponent<PlayerController>().TakeDamage();
        }
        
    }

}
