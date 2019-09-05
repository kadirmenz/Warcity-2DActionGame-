using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterControl : MonoBehaviour
{
    FootAnimation foots;

    private Camera camera;

    private Animator anim;

    public Text ammoNumberText;


    private GameObject mouse;
    private GameObject sight;
    private GameObject foot;
    //private GameObject canvas;
    //private GameObject kursunText;


    [SerializeField] private GameObject bullet;

    

    private float horizontal = 0;
    private float vertical = 0;
    private float characterDirectionalAngle;
    private float differencesBetwMouseAndCharacter = 0;
    private float fireTime=2;
    private float magazineAnimationTime = 0;
    private float footAnimTimeLeftRight = 0;
    private float footAnimTimeBackForth = 0;

    private int bulletMagazineCounter = 40;
    private int ammo = 40;
    private int footAnimCounterBackForth = 0;
    private int footAnimCounterLeftRight = 0;
    
    private Vector3 mousePosition;
    private Vector3 characterPosition;
    private Vector3 movementVec;
    private Vector3 differencesBetwCharacterAndMouse;
    private Vector3 characterDirectionalVec;
    private Vector3 cameraPos;
    private Vector3 footHeadPos;

    private RaycastHit2D characterRay;
    private RaycastHit2D sightRay;



    private Quaternion Rotation;


    Rigidbody2D physic;

    [SerializeField] private LayerMask layerMask;
    

    private bool fireControl=false;
    private bool magazineAnim = true;

    void Start()

    {
        
        
        ammoNumberText.text = bulletMagazineCounter + "/" + ammo;

        foot = GameObject.FindGameObjectWithTag("ayaklar");
        footHeadPos = foot.transform.position;
        foots = GameObject.FindGameObjectWithTag("ayaklar").GetComponent<FootAnimation>();
        
        foot.transform.SetParent(transform.parent.transform.parent);
        
        anim = GetComponent<Animator>();

        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mouse = GameObject.FindGameObjectWithTag("fareTag");
        sight = GameObject.FindGameObjectWithTag("nişangahTag");

        physic = GetComponent<Rigidbody2D>();

        movementVec = new Vector3();
        

        
    }

    void Update()

    {

        bulletDisplay();

        Debug.Log("cephane= " + ammo + "şarjör= " + bulletMagazineCounter);
        if (Input.GetMouseButtonDown(0) && bulletMagazineCounter!=0)
        {
            
            
                fireControl = true;
            
            

        }else if (Input.GetMouseButtonUp(0))
        {
            fireControl = false;
            fireTime = 0.2f;

        }
        if (Input.GetKeyDown(KeyCode.R) && ammo>0 && bulletMagazineCounter!=40)
        {

            magazineAnim = false;
            sarjorKontrol();


        }


    }

    void FixedUpdate()
    {
        
        Fire();
        characterMove();
        mouseMovement();
        DrawRay();
        mouseDisplay();
        cameraMovement();
        characterAnim();


    }
    /// <summary>
    /// Karakter hareketinin yapıldığı metod..
    /// </summary>
    private void characterMove()
    {
        //a tuşuna -1 b tuşuna 1 değeri verir

        horizontal = Input.GetAxisRaw("Horizontal");
        //w tuşuna 1 s tuşuna -1 verir
        vertical = Input.GetAxisRaw("Vertical");
        //bu değerleri bir vectorde tutuyoruz.
        movementVec.Set(horizontal*2, vertical*2, 0);
       //çapraz gidince daha hızlı gidiyor diye normalini alıyoruz.
        movementVec.Normalize();
        //velocity ye verıyoruz vectoru
        physic.velocity = movementVec*5;

        //ayak takıp
        foots.transform.position = transform.position + footHeadPos;


    }
    private void mouseMovement()
    {
        mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.Set(mousePosition.x, mousePosition.y, transform.position.z);
        characterPosition = transform.position ;
        differencesBetwCharacterAndMouse = mousePosition - transform.position;
        characterDirectionalAngle = Mathf.Atan2(differencesBetwCharacterAndMouse.y, differencesBetwCharacterAndMouse.x) * Mathf.Rad2Deg;
        characterDirectionalVec.Set(0, 0, characterDirectionalAngle);
        Rotation = Quaternion.Euler(characterDirectionalVec);
        Rotation*= Quaternion.Euler(0,0,270);
        physic.transform.rotation = Rotation;


    }
    /// <summary>
    /// bu metod farenın hareketını takıp eden bır obje bunun sayesınde ray vs cızdırebılırız...
    /// </summary>
    private void mouseDisplay()
    {
        mouse.transform.position = mousePosition;

    }
    private void cameraMovement()
    {
        cameraPos.Set(transform.position.x, transform.position.y, camera.transform.position.z);
        camera.transform.position = Vector3.Lerp(camera.transform.position, cameraPos,0.1f);
        physic.angularVelocity = 0;

    }
    private void DrawRay()
    {

        characterRay = Physics2D.Raycast(transform.position, differencesBetwCharacterAndMouse,1000,layerMask);
        
      

        differencesBetwMouseAndCharacter = Vector3.Distance(transform.position, characterRay.point);
        if (differencesBetwMouseAndCharacter > 5)
        {
            sightRay = Physics2D.Raycast(sight.transform.position, (mousePosition - sight.transform.position), 1000, layerMask);
        }else 
        {
            sightRay = Physics2D.Raycast(sight.transform.position, sight.transform.up, 1000, layerMask);
        }
       
        Debug.DrawLine(transform.position, characterRay.point, Color.magenta);
        Debug.DrawLine(sight.transform.position, sightRay.point, Color.yellow);

    }
    private void Fire()
    {
        fireTime += Time.deltaTime;
        if (fireTime >= 0.1)
        {
            fireTime = 0;

            if (fireControl == true && magazineAnim==true)
            {
                
                bulletMagazineCounter--;
                Instantiate(bullet, sight.transform.position, Rotation);
                

                if (bulletMagazineCounter == 0)
                {
                    if (ammo != 0)
                    {
                        sarjorKontrol();

                        magazineAnim = false;



                    }
                    else
                    {
                        fireControl = false;
                        
                    }

                }
            }
        }
        

        
        
    }
    public Vector2 getBulletPos()
    {
        return (sightRay.point - (Vector2)sight.transform.position).normalized;
    }
    private void sarjorKontrol()
    {
        int requiredBullet = 40 - bulletMagazineCounter;

        
            if (ammo > 40)
            {
                ammo -= requiredBullet;
                bulletMagazineCounter = 40;


            }
            else
            {
                int differences = 40 - bulletMagazineCounter;
                if (differences > ammo)
                {
                    bulletMagazineCounter += ammo;
                    ammo =0;
                }
                else
                {
                    bulletMagazineCounter += differences;
                    ammo -= requiredBullet;
                }
                
                

            }


        
        

    }
    private void characterAnim()
    {
        if (magazineAnim)
        {
           
            if (fireControl)
            {
                anim.SetBool("ateşGeç", true);
                anim.SetBool("yürümeGeç", false);

            }
            else
            {
                anim.SetBool("ateşGeç", false);
                if (horizontal != 0 || vertical != 0)
                {
                    anim.SetBool("yürümeGeç", true);

                }
                else
                {
                    anim.SetBool("yürümeGeç", false);

                }

            }
        }
        else
        {
            
            anim.SetBool("şarjörGeç", true);
            magazineAnimationTime += Time.fixedDeltaTime;
            if (magazineAnimationTime >= 1)
            {
                anim.SetBool("şarjörGeç", false);
                magazineAnim = true;
                magazineAnimationTime = 0;

            }
        }
        //********************************AYAKANİM********************************************
        footAnimTimeLeftRight += Time.fixedDeltaTime;
        footAnimTimeBackForth += Time.fixedDeltaTime;


        if(vertical==0 && horizontal == 0)
        {
            foots.GetSpriteRenderer().sprite = foots.başlangıçsprite;

        }
        else
        {
            if (vertical > 0 || vertical < 0)
            {
                if (footAnimTimeBackForth >= 0.03f)
                {
                    foots.GetSpriteRenderer().sprite = foots.ilerigerianimasyonlar[footAnimCounterBackForth++];
                    footAnimTimeBackForth = 0;

                }
                
                if (footAnimCounterBackForth >= 11)
                {
                    footAnimCounterBackForth = 0;

                }
            }
            else if (horizontal > 0 || horizontal < 0)
            {
                if(footAnimTimeLeftRight>= 0.03f)
                {
                    foots.GetSpriteRenderer().sprite = foots.sagasolaanimasyonlar[footAnimCounterLeftRight++];
                    footAnimTimeLeftRight = 0;

                }
                
                if (footAnimCounterLeftRight >= 8)
                {
                    footAnimCounterLeftRight = 0;

                }
            }
        }
        
        
        
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "bombacıTag")
        {

            

            collision.gameObject.GetComponent<BomberControl>().dusmanOldu();


        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.gameObject.tag == "cephaneTag" && ammo!=120)
        {
            Debug.Log("GİRDİ");

            ammo += collision.GetComponent<AmmoControl>().ammoNumber;
            if (bulletMagazineCounter == 0)
            {
                sarjorKontrol();
                magazineAnim = false;

            }
            Destroy(collision.gameObject);
            if (ammo > 120)
            {
                ammo = 120;

            }
        }
        

    }
    private void bulletDisplay()
    {
        ammoNumberText.text = bulletMagazineCounter + "/" + ammo;
    }
}
