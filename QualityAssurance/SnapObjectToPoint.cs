/*****************************************************************************
// File Name :         SnapObjectToPoint.cs
// Author :            Lucas Johnson
// Creation Date :     September 18, 2022
//
// Brief Description : A C# script that makes thrown objects snap to a
                       a point on an object.
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapObjectToPoint : MonoBehaviour
{
    public Vector3 snapPos;
    public string objectMask;
    public bool deliverCoffeeObjective = false;
    public bool decorateOfficeObjective = false;
    public bool addPrinterObjective = false;

    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag == "CanPickup" || other.gameObject.GetComponent<ObjectTypeStats>())
        {
            if (objectMask == "" || other.gameObject.name.Contains(objectMask))
            {
                other.transform.GetComponent<Rigidbody>().isKinematic = true;
                other.transform.position = transform.position + snapPos;
                other.transform.rotation = transform.rotation;

                if (deliverCoffeeObjective)
                {
                    ObjectTypeStats ots = other.transform.GetComponent<ObjectTypeStats>();
                    int currentState = -1;
                    if (ots.isBurned)
                    {
                        currentState = 0;
                    }
                    else if(ots.isFrozen)
                    {
                        currentState = 1;
                    }

                    if (currentState == DeliverCoffee.hotOrIced)
                    {
                        DeliverCoffee.complete = true;
                    }
                }

                if(decorateOfficeObjective)
                {
                    if(objectMask == "Plant")
                    {
                        DecorateOffice.instance.plantsCompleted++;
                        DecorateOffice.instance.UpdateGUIText();
                        ObjectiveController.instance.ChangeObjectivePage(0);
                    }
                    else if(objectMask == "Radio")
                    {
                        DecorateOffice.instance.radiosCompleted++;
                        DecorateOffice.instance.UpdateGUIText();
                        ObjectiveController.instance.ChangeObjectivePage(0);
                    }
                    Destroy(gameObject);
                }

                if(addPrinterObjective)
                {
                    AddPrinter.printerAdded = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawSphere(transform.position + snapPos, 0.01f);
        Gizmos.DrawCube(transform.position + snapPos, new Vector3(0.01f, 0.001f, 0.01f));
    }
}
