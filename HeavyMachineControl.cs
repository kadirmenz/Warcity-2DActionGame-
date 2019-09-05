using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeavyMachineControl : MonoBehaviour {

    [HideInInspector] public bool fireControl=false;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject emptyBarrel;
    [SerializeField] private Sprite[] machineFireSprite;
    [SerializeField] private Sprite[] machineMagazineSprite;
    private SpriteRenderer spriteRenderer;
    private int fireAnimCounter = 0;
    private int magazineAnimCounter = 0;
    private float animTime = 0;

    private GameObject character;
    private Vector3 enemyPos;
    private Vector3 characterPos;
    private Vector3 differencesBetwEnemyAndCharacterRotation;
    private Vector3 enemyDirectionAngleVector;
    private float directionAngle = 0;
    private Quaternion enemyRotation;
    private Quaternion startingRotation;
    private GameObject sight;
    private GameObject emptyBarrelPos;
    private GameObject sightDirection;
    private GameObject canvas;
    private Image healthBar;
    private GameObject explosion;
    [SerializeField] [Range(0, 100)] int lifeLength = 100;
    private float health ;
    [SerializeField] private GameObject wallHeavyMachine;
    [SerializeField] private GameObject blood;
    [SerializeField] private GameObject[] dieSprite;


    private float fireTime = 0;
    private bool magazineControl = true;




      void Start ()
    {
        spriteRenderer =GetComponent<SpriteRenderer>();
        canvas = transform.Find("Canvas").gameObject;
        healthBar = canvas.transform.Find("canbarı").GetComponent<Image>();
        health = lifeLength;

        explosion = transform.Find("explosion").gameObject;
        character = GameObject.FindGameObjectWithTag("Player");
        enemyDirectionAngleVector = new Vector3();
        startingRotation = Quaternion.Euler(transform.position);
        sight = transform.Find("nişangah").gameObject;
        sightDirection = sight.transform.GetChild(0).gameObject;
        emptyBarrelPos = transform.Find("boşkovanPos").gameObject;


    }
	
	// Update is called once per frame
	void FixedUpdate () {
        EnemyRotation();
        EnemyFire();
        Animation();
        healthBar.fillAmount = health / lifeLength;


	}
    void EnemyRotation()
    {
        if (fireControl)
        {
            enemyPos = transform.position;
            characterPos = character.transform.position;

            differencesBetwEnemyAndCharacterRotation = (enemyPos - characterPos).normalized;
            directionAngle = Mathf.Atan2(differencesBetwEnemyAndCharacterRotation.y, differencesBetwEnemyAndCharacterRotation.x) * Mathf.Rad2Deg;
            enemyDirectionAngleVector.Set(0, 0, directionAngle + 180);
            enemyRotation = Quaternion.Euler(enemyDirectionAngleVector);
            //enemyRotation *= Quaternion.Euler(0,0,90);
            transform.rotation = Quaternion.Lerp(transform.rotation, enemyRotation, 0.07f);
        }
        else
        {
            transform.rotation = startingRotation;

        }
        

    }
    void EnemyFire()
    {
        fireTime += Time.fixedDeltaTime;


        if (fireControl && fireTime>0.16 && magazineControl)
        {
            Instantiate(bullet, sight.transform.position, transform.rotation*Quaternion.Euler(0,0,90));
            GameObject boşkovanoluşan=Instantiate(emptyBarrel, emptyBarrelPos.transform.position, transform.rotation * Quaternion.Euler(0, 0, 90));
            boşkovanoluşan.GetComponent<Rigidbody2D>().AddForce(getKursunPos() * -100);
            Destroy(boşkovanoluşan, Random.Range(2, 10));
            fireTime = 0;

        }


    }
    public Vector2 getKursunPos()
    {
        return (sightDirection.transform.position - sight.transform.position).normalized ;
    }
    void Animation()
    {
        if (magazineControl)
        {
            animTime += Time.fixedDeltaTime;
            if (fireControl && animTime > 0.05f)
            {
                spriteRenderer.sprite = machineFireSprite[fireAnimCounter++];
                if (fireAnimCounter == machineFireSprite.Length)
                {
                    fireAnimCounter = 0;
                    animTime = 0;
                   

                    magazineControl = false;


                }
                animTime = 0;

            }
        }
        else
        {
            animTime += Time.fixedDeltaTime;
            if (fireControl && animTime > 0.05f)
            {
                spriteRenderer.sprite = machineFireSprite[magazineAnimCounter++];
                if (magazineAnimCounter == machineFireSprite.Length)
                {
                    magazineAnimCounter = 0;
                    animTime = 0;

                    magazineControl = true;


                }
                animTime = 0;

            }
        }
        
        

    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "kurşunTag")
        {
            health-- ;
            if (health <= 0)
            {
                explosion.SetActive(true);
                spriteRenderer.enabled = false;
                wallHeavyMachine.SetActive(false);
                
                for (int i = 0; i <Random.Range(10,20); i++)
                {
                    Vector3 bloodVec = new Vector3(transform.position.x + Random.Range(-2, 2), transform.position.y + Random.Range(-2, 2), 0);
                    Instantiate(blood, bloodVec, Quaternion.Euler(0, 0, Random.Range(0, 360)));
                }
                Vector3 dieVec= new Vector3(transform.position.x + Random.Range(-2, 2), transform.position.y + Random.Range(-2, 2), 0);
                GameObject ölüsprite=Instantiate(dieSprite[Random.Range(0,dieSprite.Length)], dieVec, Quaternion.Euler(0, 0, Random.Range(0, 360)));
                Destroy(ölüsprite, 6);

                canvas.SetActive(false);
                Destroy(this);
                Destroy(gameObject, 3);

            }
        }
    }
}
