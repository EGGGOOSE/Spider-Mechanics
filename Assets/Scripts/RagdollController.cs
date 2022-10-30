using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RagdollController : MonoBehaviour
{
    public GameObject cursorCircle;// урсор

    public Muscle muscle1;
    public Muscle muscle2;
    public float muscleScaleRot;

    public Transform anchor; //ƒл€ расчета угла
    public Transform anchorForScale; //расчет дистанции, дл€ сгибани€ крайней фаланги

    Vector2 toVector;

    public Transform[] borderPoints; //точки,образующие 4х-угольник, в пределах которых может двигатьс€ курсор
    public float minX, maxX, minY, maxY; // Ќабор крайних точек дл€ ограничени€ курсора
    //“ут списки X/Y точек из borderPoints
    public List<float> xPositions = null;
    public List<float> yPositions = null;

    private LineRenderer _lineRenderer;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 5;
        foreach (Muscle muscle in Muscle.muscles)
            muscle.length = Vector3.Distance(muscle.startPos.position, muscle.endPos.position);//бл€дь, вообще эту хрень надо делать в конструкторе Muscle, но посто€нно кака€-то поебень происходит, поэтому пока так.

        //Cursor.visible = false; //¬ырубить курсор
        //Debug.Log( muscle1.bone.GetComponent<SpriteRenderer>().bounds.size.x);
    }
    //нужно будет спрайты переделать чтоб идеально ровно они были повернуты относительно двух точек которые считаютс€ костью
    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 anchorPos = new Vector2(anchor.position.x, anchor.position.y);
        cursorCircle.transform.position = mousePos;
        toVector = (mousePos - anchorPos).normalized;
        float angle = Mathf.Atan2(toVector.y, toVector.x) * Mathf.Rad2Deg;

        muscle1.restRotation = angle;
        muscle2.restRotation = angle;

        float distance = Vector2.Distance(anchorPos, mousePos);

        float a = muscle1.length;
        float b = muscle2.length;
        float c = distance;

        
        if (0 < distance && distance < muscle1.length + muscle2.length)
        {
            muscle1.restRotation += Mathf.Acos((a * a + c * c - b * b) / (2 * a * c)) * Mathf.Rad2Deg;
            muscle2.restRotation += Mathf.Acos((a * a + b * b - c * c) / (2 * a * b)) * Mathf.Rad2Deg - 180f;
            Debug.Log(muscle1.restRotation + " " + muscle2.restRotation + "\n"
                + distance + " " + (muscle1.length + muscle2.length));
        }
        

        _lineRenderer.SetPosition(0, muscle1.startPos.position);
        _lineRenderer.SetPosition(1, muscle1.endPos.position);
        _lineRenderer.SetPosition(2, muscle2.endPos.position);
        _lineRenderer.SetPosition(3, muscle1.startPos.position);
        _lineRenderer.SetPosition(4, mousePos);

        foreach (Muscle muscle in Muscle.muscles)
            muscle.ActivateMuscle();
        //»тератор дл€ добавлени€ точек в списки, чтобы из них вычленить мин макс точки
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

        //ѕроверка на выход игрового курсора за карйние точки, чтобы вернуть его 
        if (cursorCircle.transform.position.x > maxX ^ cursorCircle.transform.position.x < minX)
            cursorCircle.transform.position = cursorCircle.transform.position.x > maxX ? new Vector2(maxX, cursorCircle.transform.position.y) : new Vector2(minX, cursorCircle.transform.position.y);
        if (cursorCircle.transform.position.y > maxY ^ cursorCircle.transform.position.y < minY)
            cursorCircle.transform.position = cursorCircle.transform.position.y > maxY ? new Vector2(cursorCircle.transform.position.x, maxY) : new Vector2(cursorCircle.transform.position.x, minY);

        xPositions.Clear();
        yPositions.Clear();
    }

    private void OnDestroy()
    {
        Muscle.muscles = new List<Muscle>();
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
    
    public Transform startPos, endPos;
    [HideInInspector] public float length;
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