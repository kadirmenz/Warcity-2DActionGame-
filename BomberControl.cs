using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberControl : MonoBehaviour {
    Vector3 pointProduced;
    Vector3 position;
    Vector3 differencesBetwPointAndEnemy;
    Vector3 differencesBetwEnemyAndCharacter;
    Vector3 differencesBetwEnemyAndCharacterRotation;
    private Vector3 directionPos, enemyPos;
    private Vector3 enemyDirectionalAngleVec;

    private Quaternion enemyRotation;

    private int animCounter = 0;


    EnemyCase enemycase = EnemyCase.CantFindPoint;

    RaycastHit2D rayFromPoint , rayFromCharacter;

    [SerializeField] LayerMask layermask;
    [SerializeField] private Sprite[] enemySprite;
    [SerializeField] private GameObject[] limbs_sprite
;
    [SerializeField] private GameObject blood;

    private SpriteRenderer spriteRenderer;
    
    private float distance = 0;
    private float waitingTime = 0;
    private float waitingRandom = 0;
    private float enemyDirectionAngle = 0;
    private float animTime = 0;


    float randomX = 0, randomY = 0;


    private bool sawTheCharacterControl = false;

     
    GameObject pointObject;
    private GameObject character;
    private GameObject explosion,flare;
    
   
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        explosion = transform.Find("Explosion").gameObject;
        flare = transform.Find("Flare").gameObject;
        pointProduced = new Vector3();
        pointObject = new GameObject("BombacınınGidecegiYer");
        enemyDirectionalAngleVec = new Vector3();

        CircleCollider2D noktaObjesiCol = pointObject.AddComponent<CircleCollider2D>();
        noktaObjesiCol.isTrigger = true;
        noktaObjesiCol.radius = 0.15f;
       
        character = GameObject.FindGameObjectWithTag("Player");


    }


    void FixedUpdate()
    {
        if (enemycase!=EnemyCase.SawTheCharacter)
        {
            findPoint();
            
        }
        enemyMove();
        drawRayCharacter();
        enemyRotate();
        Animation();



    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(pointProduced, 0.5f);


    }
    void findPoint()
    {
        if (enemycase == EnemyCase.CantFindPoint)
        {
            position = transform.position;
            randomX = Random.Range(2, 7);
            randomY = Random.Range(2, 7);
            pointProduced.Set(position.x + Random.Range(randomX * -1, randomX), position.y + Random.Range(randomY * -1, randomY), 0);
            pointObject.transform.position = pointProduced;
            drawRayFromPoint();

        }
       

    }
    void drawRayFromPoint()
    {
        differencesBetwPointAndEnemy = (pointProduced - transform.position).normalized;

        rayFromPoint = Physics2D.Raycast(transform.position,differencesBetwPointAndEnemy,1000,layermask);
        Debug.DrawLine(transform.position, rayFromPoint.point);
        if (rayFromPoint && rayFromPoint.collider.name== "BombacınınGidecegiYer")
        {
            enemycase = EnemyCase.FindPoint;

        }

    }
    private void enemyMove()
    {
        if (enemycase == EnemyCase.SawTheCharacter)
        {
            transform.position += (differencesBetwEnemyAndCharacter).normalized * Time.fixedDeltaTime*5;

            
        }
        else
        {

            if (enemycase == EnemyCase.FindPoint)
            {
                transform.position += (pointProduced - transform.position).normalized*Time.deltaTime*3;
                distance = Vector3.Distance(transform.position, rayFromPoint.point);
                if (distance < 0.5f)
                {
                    enemycase = EnemyCase.Wait;
                    waitingRandom = Random.Range(0.2f, 2.0f);


                }
            }else if (enemycase==EnemyCase.Wait)
            {
                waitingTime += Time.fixedDeltaTime;
                if (waitingTime > waitingRandom)
                {
                    enemycase = EnemyCase.CantFindPoint;
                    waitingTime = 0;

                }
            }
            
        }


    }
    
    private void drawRayCharacter()
    {
        differencesBetwEnemyAndCharacter = (character.transform.position - transform.position);

        rayFromCharacter = Physics2D.Raycast(transform.position, differencesBetwEnemyAndCharacter , 1000, layermask);

        Debug.DrawLine(transform.position, rayFromCharacter.point, Color.black);
        if (rayFromCharacter && rayFromCharacter.collider.tag == "Player")
        {
            enemycase = EnemyCase.SawTheCharacter;
            sawTheCharacterControl = true;

        }
        else
        {
            if (sawTheCharacterControl)
            {
                enemycase = EnemyCase.CantFindPoint;
                sawTheCharacterControl = false;

            }
        }

    }
    private void enemyRotate()
    {
        if (enemycase == EnemyCase.SawTheCharacter)
        {
            directionPos = character.transform.position;
        }
        else
        {
            directionPos = pointObject.transform.position;
        }
        enemyPos = transform.position;
       

        differencesBetwEnemyAndCharacterRotation = (enemyPos - directionPos).normalized;
        enemyDirectionAngle = Mathf.Atan2(differencesBetwEnemyAndCharacterRotation.y, differencesBetwEnemyAndCharacterRotation.x)*Mathf.Rad2Deg;
        enemyDirectionalAngleVec.Set(0,0,enemyDirectionAngle+90);
        enemyRotation = Quaternion.Euler(enemyDirectionalAngleVec);
        //dusmanRotation *= Quaternion.Euler(0,0,90);
        transform.rotation =Quaternion.Lerp(transform.rotation,enemyRotation,0.07f);



    }
    private void Animation()
    {
        if (enemycase == EnemyCase.Wait)
        {
            spriteRenderer.sprite = enemySprite[0];
            animCounter = 0;
            animTime = 0;
        }
        else
        {
            animTime += Time.fixedDeltaTime;
            if (animTime > 0.1f)
            {
                spriteRenderer.sprite = enemySprite[animCounter++];
                if (animCounter == enemySprite.Length)
                {
                    animCounter = 0;

                }
                animTime = 0;

            }
            
        }
        
    }
    public void enemyDie()
    {
        for(int i=0; i < Random.Range(5, 20); i++)
        {
           GameObject uzuvlar = Instantiate(limbs_sprite[Random.Range(0, limbs_sprite.Length)], transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            Vector2 uzuvVec = new Vector2(Random.Range(-800, 800), Random.Range(-800, 800));
            uzuvlar.GetComponent<Rigidbody2D>().AddForce(uzuvVec);
            Destroy(uzuvlar, Random.Range(3, 5));

        }
        for (int i = 0; i < Random.Range(5, 20); i++)
        {
            Vector3 kanvec = new Vector3(transform.position.x + Random.Range(-2, 2), transform.position.y + Random.Range(-2, 2),0);
            Instantiate(blood, kanvec, Quaternion.Euler(0, 0, Random.Range(0, 360)));
          

        }
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        explosion.SetActive(true);
        flare.SetActive(false);
        
        Destroy(this);
        
        spriteRenderer.enabled = false;
        

        Destroy(gameObject, 4);


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "kurşunTag")
        {
            enemyDie();
            Destroy(collision.gameObject);


        }


    }
}

public enum EnemyCase
{

    FindPoint=1,
    CantFindPoint=2,
    Wait=3,
    SawTheCharacter=4,



}
    
