using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyMachineZone : MonoBehaviour {

    HeavyMachineControl characterControl;
    void Start ()
    {

        characterControl=transform.parent.GetComponent<HeavyMachineControl>();
        transform.SetParent(transform.parent.parent);
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            characterControl.fireControl = true;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            characterControl.fireControl = false;
            
        }
    }


}
