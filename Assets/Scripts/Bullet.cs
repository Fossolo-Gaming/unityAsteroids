using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    public LayerMask bounds;
    public LayerMask asteroids;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    bool IsLayerInMask(LayerMask mask, int layer)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        int colLayer = col.gameObject.layer;
        if(IsLayerInMask(bounds, colLayer))
        {
            Destroy(gameObject);
        }

        if(IsLayerInMask(asteroids, colLayer))
        {
            // Destroy(col.gameObject);
            GameObject parent = col.gameObject.transform.parent.gameObject;
            parent.GetComponent<AsteroidsController>().destroyAsteroid(col.gameObject, gameObject);
            Destroy(gameObject);
            GameController.GetGameController().Score += 1;
        }        
    }
 }
