using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBulletControl : MonoBehaviour {

    CharacterControl character;

    void Start ()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterControl>();

        GetComponent<Rigidbody2D>().AddForce(character.getKursunPos()*1000);
        Destroy(transform.gameObject, 3);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "fareTag" && collision.gameObject.name !="Alan" )
        {
            Destroy(gameObject);
        }
        

    }
}
