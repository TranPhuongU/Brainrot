using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private bool canMoveRight = true;
    private bool canMoveLeft = true;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("DeadZoneRight"))
            canMoveRight = false;
        else if (col.CompareTag("DeadZoneLeft"))
            canMoveLeft = false;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("DeadZoneRight"))
            canMoveRight = true;
        else if (col.CompareTag("DeadZoneLeft"))
            canMoveLeft = true;
    }

}
