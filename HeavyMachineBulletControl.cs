using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyMachineBulletControl : MonoBehaviour {

    HeavyMachineControl heavyMachineControl;

    void Start()
    {
        heavyMachineControl = GameObject.FindGameObjectWithTag("agırmakınalıTag").GetComponent<HeavyMachineControl>();

        GetComponent<Rigidbody2D>().AddForce(heavyMachineControl.getKursunPos() * 1000);
        Destroy(transform.gameObject, 3);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "fareTag" && collision.gameObject.name!="Alan")
        {
            Destroy(gameObject);
        }


    }
}
