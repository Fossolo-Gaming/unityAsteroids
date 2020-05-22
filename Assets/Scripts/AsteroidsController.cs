using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;

public class AsteroidsController : MonoBehaviour
{
    public List<int> numAsteroids;
    public List<int> minSize;
    public List<int> maxSize;

    public List<GameObject> asteroidPrefabs;
    public AudioSource explosionSound;
    public float initialSpeed = 10.0f;
    public float explosionForce = 2.0f;

    private bool started = false;

    void Start()
    {

    }

    public void CreateAsteroids()
    {
        int index = GameController.GetGameController().Level - 1;
        int num = numAsteroids[index];
        for (int i = 0; i < num; ++i)
        {
            float angle = Random.Range(0.0f, 360.0f);
            int size = Random.Range(minSize[index], maxSize[index]);
            UnityEngine.Vector3 worldPos = GameController.GetGameController().GetRandomPosition();


            createAsteroid(worldPos, angle, size, initialSpeed);
        }
        started = true;
    }

    public void DestroyAsteroids()
    {
        started = false;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void createAsteroid(UnityEngine.Vector3 worldPos, float angle, int size, float speed)
    {
        GameObject asteroid = (Instantiate(asteroidPrefabs[size-1], worldPos, UnityEngine.Quaternion.AngleAxis(angle, UnityEngine.Vector3.forward), transform)) as GameObject;
        Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();
        float mass = rb.mass;
        rb.AddForce(asteroid.transform.up * speed * mass, ForceMode2D.Impulse);
    }

    public void destroyAsteroid(GameObject asteroid, GameObject bullet)
    {
        int size = asteroid.GetComponent<AsteroidData>().size;

        if (size > 1)
        {
            --size;
            UnityEngine.Vector3 axis;
            float angle;
            bullet.transform.rotation.ToAngleAxis(out angle, out axis);

            for (int i = 0; i < 2; ++i)
            {
                float newAngle = angle + ((i == 0) ? 90.0f : -90.0f);
                createAsteroid(asteroid.transform.position, newAngle, size, explosionForce);
            }
        }

        Destroy(asteroid);
        explosionSound.Play();
        Debug.Log("Remaining asteroids: " + transform.childCount);
    }


    // Update is called once per frame
    void Update()
    {
        if (started && transform.childCount == 0)
        {
            GameController.GetGameController().LevelCompleted();
        }
    }
}
