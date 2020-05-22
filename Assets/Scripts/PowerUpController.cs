using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public int bonusType = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player") {
            PlayerController pm = collision.gameObject.GetComponent<PlayerController>();
            pm.ActivatePowerUp(bonusType);
            Destroy(gameObject);
        }
    }
}
