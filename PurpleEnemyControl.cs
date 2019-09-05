using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PurpleEnemyControl : MonoBehaviour {

    private GameObject firstZone;
    private GameObject secondZone;
    private GameObject thirdZone;
    private GameObject switchBetweenZones;
    private GameObject purpleEnemy;
    private GameObject mainCharacter;

    private int goRandomZone = 0;
    private int howManyRounds;
    private int howManyRoundsCounter = 0;

    private int distanceCounter = 0;



    private bool isZoneSelected = true;
    private bool rayTouchCharacter = false;



    private Lap lap = Lap.OnLap;

    private Vector3 newPos;
    private Vector3 rot;
    private float distance = 0;

    private RaycastHit2D drawRay;
    [SerializeField] private LayerMask purpleEnemyMask;
    [SerializeField] [Range(1,20)]private float purpleEnemyVelocity;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject bulletOutPlace;
    [SerializeField] private GameObject bulletOutPlace2;
    private float bulletOutTime = 1;
    private bool whereBulletGoBool= true;

    void Start ()
    {
        
        mainCharacter = GameObject.FindGameObjectWithTag("Player");
        firstZone = transform.Find("1.Bölge").gameObject;
        secondZone = transform.Find("2.Bölge").gameObject;
        thirdZone = transform.Find("3.Bölge").gameObject;
        switchBetweenZones = transform.Find("GecisBolgesi").gameObject;
        purpleEnemy = transform.Find("MorDusmanAna").gameObject;
        bulletOutPlace = transform.Find("MorDusmanAna").transform.Find("kursuncıkısyeri").gameObject;
        bulletOutPlace2 = transform.Find("MorDusmanAna").transform.Find("kursuncıkısyeri2").gameObject;


    }
	
	void FixedUpdate ()
    {
        DrawRay();
        
        Fire();

        if (isZoneSelected)
        {
            goRandomZone = Random.Range(1, 4);
            howManyRounds = Random.Range(1, 3);

            isZoneSelected = false;
        }
        

        if (goRandomZone.Equals(1))
        {
            TourBetweenPoints(firstZone);
        }
        else if (goRandomZone.Equals(2))
        {
            TourBetweenPoints(secondZone);
        }
        else if (goRandomZone.Equals(3))
        {
            TourBetweenPoints(thirdZone);
        }
        


    }
    private void TourBetweenPoints(GameObject bölge)
    {
        if (lap == Lap.OnLap)
        {
            DistancePosRot(purpleEnemy.transform.position, bölge.transform.GetChild(distanceCounter).transform.position,out newPos,out distance,out rot);
            purpleEnemy.transform.position += newPos;
            
            if (distance < 0.2f)
            {
                distanceCounter++;
                if (distanceCounter >= bölge.transform.childCount)
                {
                    distanceCounter = 0;
                    howManyRoundsCounter++;
                    if (howManyRoundsCounter==howManyRounds)
                    {
                        

                        lap = Lap.GoZoneStartingPoins;
                        howManyRoundsCounter = 0;
                    }
                }
            }
            
        }
        else if (lap == Lap.GoZoneStartingPoins)
        {
            DistancePosRot(purpleEnemy.transform.position, bölge.transform.GetChild(0).transform.position, out newPos, out distance, out rot);
            purpleEnemy.transform.position += newPos;
          
            if (distance < 0.2f)
            {
                
               
                    
                    lap = Lap.GoMainPoint;
                
                
            }
            
        }
        
        else if (lap == Lap.GoMainPoint)
        {
            DistancePosRot(purpleEnemy.transform.position, switchBetweenZones.transform.position, out newPos, out distance,out rot);
            purpleEnemy.transform.position += newPos;
            
            if (distance < 0.2f)
            {
                lap = Lap.OnLap;
                isZoneSelected = true;


            }
            
        }
        Rotation(rot);

    }
    private void DistancePosRot(Vector3 firstPos,Vector3 lastPos,out Vector3 newPos,out float mesafe,out Vector3 rot)
    {
        newPos = (lastPos - firstPos).normalized*Time.fixedDeltaTime*purpleEnemyVelocity;
        mesafe = Vector3.Distance(firstPos, lastPos);
        Vector3 EnemyPos = purpleEnemy.transform.position;
        if (rayTouchCharacter)
        {
            
            Vector3 characterPos = mainCharacter.transform.position;

             rot = (EnemyPos - characterPos).normalized;
        }
        else
        {
            
            Vector3 pointPos = lastPos;

             rot = (EnemyPos - lastPos).normalized;
        }

    }
    private void Rotation(Vector3 newPos)
    {
        //Vector3 dusmanPos = morDusman.transform.position;
        //Vector3 karakterPos = anaKarakter.transform.position;

        //Vector3 düşmanVekarakterfarkDonme = (dusmanPos - karakterPos).normalized;

        float directionAngle = Mathf.Atan2(newPos.y, newPos.x) * Mathf.Rad2Deg;
            Vector3 enemyDirectionAngleVec = new Vector3();
            enemyDirectionAngleVec.Set(0, 0, directionAngle + 90);
            Quaternion enemyRotation = Quaternion.Euler(enemyDirectionAngleVec);
            //dusmanRotation *= Quaternion.Euler(0,0,90);
            purpleEnemy.transform.rotation = Quaternion.Lerp(purpleEnemy.transform.rotation, enemyRotation, 0.07f);
        
        

    }
    private void DrawRay()
    {
        
        drawRay = Physics2D.Raycast(purpleEnemy.transform.position,mainCharacter.transform.position - purpleEnemy.transform.position,1000, purpleEnemyMask);
        
        Debug.DrawLine(purpleEnemy.transform.position,drawRay.point,Color.black);

        rayTouchCharacter = (drawRay.collider.tag == "Player") ? true : false;

        //if (rayCizdir.collider.tag == "Player")
        //{
        //    rayKaraktereDegdi = true;
        //}
        //else
        //{
        //    rayKaraktereDegdi = false;

        //}
    }
    private void Fire()
    {
        
        if (rayTouchCharacter)
        {
            bulletOutTime += Time.fixedDeltaTime;
            if (bulletOutTime >= 0.3)
            {
                Instantiate(bullet, bulletOutPlace.transform.position, Quaternion.identity);
                bulletOutTime = 0;
            }
            
            

        }
        
    }
    public Vector3 GetBulletForce()
    {
        if (drawRay.distance > 4)
        {
            return (drawRay.point - (Vector2)bulletOutPlace.transform.position).normalized;
        }
        else
        {
            return (bulletOutPlace2.transform.position - bulletOutPlace.transform.position);
        }
            
        
        
        

    }






#if UNITY_EDITOR
    void OnDrawGizmos()
    {
       
        List<GameObject> myObject1 = new List<GameObject>();
        List <GameObject> myObject2 = new List<GameObject>();
        List<GameObject> myObject3 = new List<GameObject>();
        GameObject firstZone = transform.Find("1.Bölge").gameObject;
        GameObject secondZone = transform.Find("2.Bölge").gameObject;
        GameObject thirdZone = transform.Find("3.Bölge").gameObject;
        GameObject switchBetweenZones = transform.Find("GecisBolgesi").gameObject;
        GameObject purpleEnemyMain = transform.Find("MorDusmanAna").gameObject;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(switchBetweenZones.transform.position,1);

        if (!Application.isPlaying)
        {
            purpleEnemyMain.transform.position = switchBetweenZones.transform.position;

        }
        //-------------------------------------------------------------------
        //---------------------------------------------------------------------
        //BİRİNCİ BÖLGE
        for (int i = 0; i < firstZone.transform.childCount; i++)
        {
            myObject1.Add(firstZone.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < myObject1.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(myObject1[i].transform.position,1);

        }
        for (int i = 0; i < myObject1.Count-1; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(myObject1[i].transform.position,objelerimBir[i+1].transform.position);

        }
        if (myObject1.Count!=0)
        {
            Gizmos.DrawLine(myObject1[myObject1.Count-1].transform.position, myObject1[0].transform.position);
            Gizmos.DrawLine(switchBetweenZones.transform.position, myObject1[0].transform.position);
        }
        //-------------------------------------------------------------------
        //---------------------------------------------------------------------
        //İKİNCİ BÖLGE
        for (int i = 0; i < secondZone.transform.childCount; i++)
        {
            myObject2.Add(secondZone.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < myObject2.Count; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(myObject2[i].transform.position, 1);

        }
        for (int i = 0; i < myObject2.Count - 1; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(myObject2[i].transform.position, myObject2[i + 1].transform.position);

        }
        if (myObject2.Count != 0)
        {
            Gizmos.DrawLine(myObject2[myObject2.Count - 1].transform.position, myObject2[0].transform.position);
            Gizmos.DrawLine(switchBetweenZones.transform.position, myObject2[0].transform.position);
        }
        //-------------------------------------------------------------------
        //---------------------------------------------------------------------
        //ÜÇÜNCÜ BÖLGE
        for (int i = 0; i < thirdZone.transform.childCount; i++)
        {
            myObject3.Add(thirdZone.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < myObject3.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(myObject3[i].transform.position, 1);

        }
        for (int i = 0; i < myObject3.Count - 1; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(myObject3[i].transform.position, myObject3[i + 1].transform.position);

        }
        if (myObject3.Count != 0)
        {
            Gizmos.DrawLine(myObject3[myObject3.Count - 1].transform.position, myObject3[0].transform.position);
            Gizmos.DrawLine(switchBetweenZones.transform.position, myObject3[0].transform.position);
        }
    }
#endif

}
enum Lap
{
    OnLap=1,GoZoneStartingPoins=2,GoMainPoint=3
}


#if UNITY_EDITOR
[CustomEditor(typeof(MorDusmanKontrol))]
[System.Serializable]
class dusmanKontrolEdıtor : Editor
{
    public override void OnInspectorGUI()
    {
        
        MorDusmanKontrol script =(MorDusmanKontrol) target;
        if (GUILayout.Button("Nokta Üret Birinci Bölge",GUILayout.MinWidth(100),GUILayout.Width(200)))
        {
            GameObject firstZone = script.transform.Find("1.Bölge").gameObject;
            GameObject newObject = new GameObject();
            newObject.transform.parent = firstZone.transform;
            newObject.transform.position = firstZone.transform.position + new Vector3(Random.Range(-2,2), Random.Range(-2, 2),0);
            newObject.name = firstZone.transform.childCount.ToString();

        }
        else if (GUILayout.Button("Nokta Üret İkinci Bölge", GUILayout.MinWidth(100), GUILayout.Width(200)))
        {
            GameObject willOccurZone = script.transform.Find("2.Bölge").gameObject;
            GameObject yeniobje = new GameObject();
            yeniobje.transform.parent = willOccurZone.transform;
            yeniobje.transform.position = willOccurZone.transform.position + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);
            yeniobje.name = willOccurZone.transform.childCount.ToString();

        }
        else if (GUILayout.Button("Nokta Üret Üçüncü Bölge", GUILayout.MinWidth(100), GUILayout.Width(200)))
        {
            GameObject firstZone = script.transform.Find("3.Bölge").gameObject;
            GameObject newObject = new GameObject();
            newObject.transform.parent = firstZone.transform;
            newObject.transform.position = firstZone.transform.position + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);
            newObject.name = firstZone.transform.childCount.ToString();

        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MorDusmanMask"));
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("morDusmanHiz"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("kursun"));
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();

    }
    
}
#endif
