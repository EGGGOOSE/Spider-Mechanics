using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public Transform anchorForScale;

    Vector2 from;
    Vector2 to;
    public Transform[] borderPoints;
    public float minX, maxX, minY, maxY;
    public List<float> xPositions = null;
    public List<float> yPositions = null;

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
    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        from = new Vector2(anchor.position.x, 0);
        Vector2 anchorPos = new Vector2(anchor.position.x, anchor.position.y);
        circle2.transform.position = mousePos;
        direction = new Vector2(circle2.transform.position.x, circle2.transform.position.y) - anchorPos;
        to = direction; 
        
        muscle1.restRotation = Vector2.SignedAngle(from, to);
        muscle2.restRotation = Vector2.SignedAngle(from, to) + RotationScale(circle2.transform.position, anchorForScale.position);

        //Debug.Log(Vector2.SignedAngle(from, to));
        
        square.transform.position = new Vector2(anchor.position.x, transform.position.y);
        //Debug.Log(RotationScale(mousePos,new Vector2(anchor.position.x,anchor.position.y)));
        foreach (Muscle muscle in Muscle.muscles)
            muscle.ActivateMuscle();

        foreach (Transform borderPoint in borderPoints)
        {         
            float borderPosX = borderPoint.position.x;
            float borderPosY = borderPoint.position.y;
            xPositions.Add(borderPosX);
            yPositions.Add(borderPosY);
        }
        minX = xPositions.Min();
        maxX = xPositions.Max();
        minY = yPositions.Min();
        maxY = yPositions.Max();

        if (circle2.transform.position.x > maxX ^ circle2.transform.position.x < minX)
            circle2.transform.position = circle2.transform.position.x > maxX ? new Vector2(maxX, circle2.transform.position.y) : new Vector2(minX, circle2.transform.position.y);
        if (circle2.transform.position.y > maxY ^ circle2.transform.position.y < minY)
            circle2.transform.position = circle2.transform.position.y > maxY ? new Vector2(circle2.transform.position.x, maxY) : new Vector2(circle2.transform.position.x, minY);

        xPositions.Clear();
        yPositions.Clear();
    }
     
    private float RotationScale(Vector2 mousePos,Vector2 anchorPos)
    {
        float distance = Vector2.Distance(mousePos, anchorPos);
        if (distance >= 2.6f)
        {
            return muscleScaleRot;
        }
        return muscleScaleRot * distance / 2.6f;
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