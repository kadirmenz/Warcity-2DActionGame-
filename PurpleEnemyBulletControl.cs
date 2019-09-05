using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleEnemyBulletControl : MonoBehaviour {

    PurpleEnemyControl purpleEnemy;
    Rigidbody2D fizik;
    [SerializeField][Range(100,1000)] private float kursunHız=100;

	void Start ()
    {
        purpleEnemy = GameObject.FindGameObjectWithTag("mordusmanTag").GetComponent<PurpleEnemyControl>();
        fizik = GetComponent<Rigidbody2D>();
        fizik.AddForce(purpleEnemy.GetBulletForce() * kursunHız);
        Destroy(gameObject, 4);
	}
	
	
	void Update ()
    {
		
	}
}
