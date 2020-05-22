using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsController : MonoBehaviour
{
    public List<GameObject> PowerUpsPrefabs;
    public float powerUpTimer = 3.0f;
    public AudioSource spawnSound;

    private bool Activated = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()    
    {
        Activated = true;
        StartCoroutine(SpawnPowerUp());
    }

    public void Deactivate()
    {
        Activated = false;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    IEnumerator SpawnPowerUp()
    {
        if (Activated)
        {
            yield return new WaitForSeconds(powerUpTimer);
        }

        if(Activated) { 
            Vector3 pos = GameController.GetGameController().GetRandomPosition();
            int bonusType = Random.Range(0, PowerUpsPrefabs.Count);
            GameObject powerUp = (Instantiate(PowerUpsPrefabs[bonusType], pos, UnityEngine.Quaternion.identity, transform)) as GameObject;
            spawnSound.Play();
            StartCoroutine(SpawnPowerUp());
        }
    }

}
