using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reproduction : MonoBehaviour
{
   [Header("Parent1")]
    public float fov1;
    public float height1;
    public float moveSpeed1;

    [Header("Parent2")]
    public float fov2;
    public float height2;
    public float moveSpeed2;

    [Header("Child")]
    public float fov;
    public float height;
    public float moveSpeed;

    public void Reproduce(GameObject parent1, Movement parent2,GameObject child)
    {
        Movement parent1Script = parent1.GetComponent<Movement>();
        Movement childScript = child.GetComponent<Movement>();
        fov1 = parent1Script.fov;
        height1 = parent2.height;
        moveSpeed1 = parent2.moveSpeed;
        fov2 = parent2.fov;
        height2 = parent2.height;
        moveSpeed2 = parent2.moveSpeed;
        childScript.fov = (fov1 + fov2) /2;
        childScript.moveSpeed = (moveSpeed1 + moveSpeed2)/2;
        childScript.height = (height1 + height2) /2;
        childScript.isReproducing = false;
        childScript.waiting = false;
        childScript.endOfPath = false;
    }
}
