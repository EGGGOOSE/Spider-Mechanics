using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RagdollController : MonoBehaviour
{
    public GameObject cursorCircle;//Курсор

    public Muscle muscle1;
    public Muscle muscle2;
    public float muscleScaleRot;

    public Transform anchor; //Для расчета угла
    public Transform anchorForScale; //расчет дистанции, для сгибания крайней фаланги

    Vector2 from;
    Vector2 to;

    public Transform[] borderPoints; //точки,образующие 4х-угольник, в пределах которых может двигаться курсор
    public float minX, maxX, minY, maxY; // Набор крайних точек для ограничения курсора
    //Тут списки X/Y точек из borderPoints
    public List<float> xPositions = null;
    public List<float> yPositions = null;

    private void Start()
    {
        Cursor.visible = false; //Вырубить курсор
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 anchorPos = new Vector2(anchor.position.x, anchor.position.y);
        from = anchorPos;
        cursorCircle.transform.position = mousePos;
        to = new Vector2(cursorCircle.transform.position.x, cursorCircle.transform.position.y) - anchorPos; 
        
        muscle1.restRotation = Vector2.SignedAngle(from, to);
        muscle2.restRotation = Vector2.SignedAngle(from, to) + RotationScale(cursorCircle.transform.position, anchorForScale.position);
        
        foreach (Muscle muscle in Muscle.muscles)
            muscle.ActivateMuscle();
        //Итератор для добавления точек в списки, чтобы из них вычленить мин макс точки
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

        //Проверка на выход игрового курсора за карйние точки, чтобы вернуть его 
        if (cursorCircle.transform.position.x > maxX ^ cursorCircle.transform.position.x < minX)
            cursorCircle.transform.position = cursorCircle.transform.position.x > maxX ? new Vector2(maxX, cursorCircle.transform.position.y) : new Vector2(minX, cursorCircle.transform.position.y);
        if (cursorCircle.transform.position.y > maxY ^ cursorCircle.transform.position.y < minY)
            cursorCircle.transform.position = cursorCircle.transform.position.y > maxY ? new Vector2(cursorCircle.transform.position.x, maxY) : new Vector2(cursorCircle.transform.position.x, minY);

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