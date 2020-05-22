using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode fireKey;
    public KeyCode bombKey;

    [SerializeField]
    float rotSpeed = 0.1f;
    [SerializeField]
    float thrustForce = 0.05f;
    [SerializeField]
    float fireForce = 1.0f;
    [SerializeField]
    float shieldRotSpeed = 10.0f;
    [SerializeField]
    float invulnerabileTime = 3.0f;

    public float defaultShotRatio = 0.5f;
    public float rapidShotRatio = 0.2f;
    private float minShotRatio;

    private float shotTimer = 0.0f;
    private float invulnerableTimer = 0.0f;
    private float bombTimer = 0.0f;

    public GameObject bulletsGroup;
    public GameObject bulletPrefab;
    public AudioSource fireSound;
    public AudioSource shieldDamageSound;
    public AudioSource shipExplosionSound;
    public AudioSource powerUpActivationSound;
    public AudioSource bombSound;

    private Rigidbody2D rb2d;

    public GameObject shield;
    public List<GameObject> shieldSprites;

    private int shieldLevel;   
    private float flashingCycles = 6f;
    private bool tripleShot = false;

    private UnityEngine.Vector3 pos0;
    private UnityEngine.Quaternion rot0;

    Color flashingFunction()
    {        
        float x = invulnerableTimer / invulnerabileTime * math.PI * flashingCycles;
        float v = math.abs(math.cos(x));
        Color c = new Color(1.0f, 1.0f, 1.0f, v);
        return c;
    }

    public void activateInvulnerability()
    {
        invulnerableTimer = invulnerabileTime;
    }

    // Start is called before the first frame update
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        transform.Rotate(Vector3.forward * 0.1f); // small rotation to initialize the rot vector
        pos0 = transform.position;
        rot0 = transform.rotation;
    }

    public void Restart()
    {
        ShieldLevel = 1;
        minShotRatio = defaultShotRatio;
        shotTimer = minShotRatio;
        tripleShot = false;
        rb2d.velocity = Vector2.zero;
        transform.position = pos0;
        transform.rotation = rot0;
        activateInvulnerability();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        shotTimer += Time.deltaTime;
        bombTimer += Time.deltaTime;

        if(invulnerableTimer>0.0f)
        {
            invulnerableTimer -= Time.deltaTime;
            if (invulnerableTimer < 0.0f) invulnerableTimer = 0.0f;
            gameObject.GetComponent<SpriteRenderer>().color = flashingFunction();
        }

        if (Input.GetKey(leftKey)) {
            UnityEngine.Vector3 rotVec = Vector3.forward * rotSpeed * Time.deltaTime;
            transform.Rotate(rotVec);
            shield.transform.Rotate(-rotVec);
            //rb2d.AddTorque(rotSpeed * Time.deltaTime, ForceMode2D.Impulse);
        }
        if (Input.GetKey(rightKey)) {
            UnityEngine.Vector3 rotVec = Vector3.back * rotSpeed * Time.deltaTime;
            transform.Rotate(rotVec);
            shield.transform.Rotate(-rotVec);
            //rb2d.AddTorque(-rotSpeed * Time.deltaTime, ForceMode2D.Impulse);
        }
        if(Input.GetKey(upKey)) {
            Vector2 force = transform.up * thrustForce;
            rb2d.AddForce(force * Time.deltaTime, ForceMode2D.Impulse);
        }
        if (Input.GetKey(downKey)) {
            Vector2 force = - transform.up * thrustForce;
            rb2d.AddForce(force * Time.deltaTime, ForceMode2D.Impulse);
        }
        if(Input.GetKey(fireKey))
        {
            Fire();
        }

        if(Input.GetKey(bombKey))
        {
            FireBomb();
        }
        
        if (shield.activeSelf)
        {
            shield.transform.Rotate(UnityEngine.Vector3.forward, shieldRotSpeed * Time.deltaTime);
        }
        
    }

    void Fire()
    {
        if (shotTimer < minShotRatio) {
            return;
        }
        shotTimer = 0.0f;

        FireBullet(0.0f);
        if(tripleShot)
        {
            FireBullet(20.0f);
            FireBullet(-20.0f);
        }
        fireSound.Play();
    }


    void FireBomb()
    {
        if (GameController.GetGameController().Bombs == 0 || shotTimer < minShotRatio)
            return;

        shotTimer = 0.0f;
        GameController.GetGameController().Bombs--;

        for (int i=0; i<12; ++i)
        {
            FireBullet((float)i * 30.0f);
        }
        bombSound.Play();
    }

    void FireBullet(float rotAngle)
    { 
        //Clone of the bullet
        GameObject bullet;

        Quaternion rot;
        if(rotAngle == 0.0)
        {
            rot = transform.rotation;
        }
        else
        {
            UnityEngine.Vector3 axis;
            float angle;
            transform.rotation.ToAngleAxis(out angle, out axis);
            angle += rotAngle;
            rot = Quaternion.AngleAxis(angle, axis);
        }

        //spawning the bullet at position
        bullet = (Instantiate(bulletPrefab, transform.position, rot, bulletsGroup.transform)) as GameObject;

        //add force to the spawned objected
        Vector2 force = bullet.transform.up * fireForce;
        bullet.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
    }

    public void TakeDamage()
    {
        if(invulnerableTimer>0.0f)
        {
            return;
        }
        if(shieldLevel>0)
        {
            ShieldLevel = shieldLevel - 1;
            activateInvulnerability();
            shieldDamageSound.Play();
        }
        else
        {
            StartCoroutine(Die());
        }
    }

    public int ShieldLevel
    {
        get { return shieldLevel;  }
        set { 
            shieldLevel = value;
            shield.SetActive(shieldLevel > 0);
            for(int i=0; i<shieldSprites.Count; ++i)
            {
                shieldSprites[i].SetActive(shieldLevel> i);
            }
        }
    }

    public void ActivatePowerUp(int bonusType)
    {
        switch(bonusType)
        {
            case 0:
                tripleShot = true;
                break;

            case 1:
                ShieldLevel = 3;
                break;

            case 2:
                minShotRatio = rapidShotRatio;
                break;

            case 3:
                GameController.GetGameController().Bombs += 1;
                break;

            case 4:
                GameController.GetGameController().Lives++;
                break;

            case 5:
                GameController.GetGameController().Score += 5;
                break;
        }
        powerUpActivationSound.Play();
    }

    //And function itself
    IEnumerator Die()
    {
        shipExplosionSound.Play();
        yield return new WaitForSeconds(1);
        GameController.GetGameController().PlayerDeath();
    }

}

