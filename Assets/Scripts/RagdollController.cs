using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public GameObject circle;
    public GameObject circle2;
    public GameObject square;

    public Muscle muscle1;
    public Muscle muscle2;
    public float muscleScaleRot;

    public Transform anchor;
    private Vector2 direction;

    Vector2 from;
    Vector2 to;
    /*private void Update()
    {
        foreach(Muscle muscle in muscles)
        {
            if (muscle.bone.name != "Body")
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //muscle.restRotation = Vector2.SignedAngle(transform.Find("Body").position, mousePos)+180;
                muscle.restRotation = Vector2.SignedAngle(transform.Find("Body").position, mousePos);
                Debug.Log(muscle.restRotation);
                circle.GetComponent<Transform>().position = transform.Find("Body").position;
            }
            muscle.ActivateMuscle();
        }   
    }*/
    //центр у якоря
    //сгибание

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        from = new Vector2(anchor.position.x, 0);
        Vector2 anchorPos = new Vector2(anchor.position.x, anchor.position.y);
        direction = mousePos - anchorPos;
        to = direction; 
        
        muscle1.restRotation = Vector2.SignedAngle(from, to);
        muscle2.restRotation = Vector2.SignedAngle(from, to) + RotationScale(mousePos,anchorPos);

        //Debug.Log(Vector2.SignedAngle(from, to));
        circle2.transform.position = mousePos;
        square.transform.position = new Vector2(anchor.position.x, transform.position.y);
        //Debug.Log(RotationScale(mousePos,new Vector2(anchor.position.x,anchor.position.y)));
        foreach (Muscle muscle in Muscle.muscles)
            muscle.ActivateMuscle();
    }
     
    private float RotationScale(Vector2 mousePos,Vector2 anchorPos)
    {
        float distance = Vector2.Distance(mousePos, anchorPos);
        if (distance >= 4.5f)
        {
            return muscleScaleRot;
        }
        return muscleScaleRot * distance / 6;
    }

}

[System.Serializable]

public class Muscle
{
    public Rigidbody2D bone;
    public float restRotation;
    public float force;
    public static List<Muscle> muscles = new List<Muscle>();

    public Muscle()
    {
        muscles.Add(this);
    }

    public void ActivateMuscle()
    {
        force = 105;
        bone.MoveRotation(Mathf.LerpAngle(bone.rotation, restRotation, force * Time.deltaTime));
    }

}