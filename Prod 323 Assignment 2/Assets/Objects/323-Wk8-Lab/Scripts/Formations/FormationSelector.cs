using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FormationSelector : MonoBehaviour
{
    private enum FormationType { Arc, Circle, Column, Diamond, Echelon, Grid, Line, Row, Square, Triangle, V, Wedge }
    private FormationType fType = FormationType.Column;
    public UnityEvent FormationChanged;

    [Header("Arc")]
    [SerializeField] float radius = 5;
    private float theta;

    [Header("Circle")]
    [SerializeField] float radius2 = 5;
    private float theta2;

    [Header("Column")]
    [SerializeField] Vector2 separation = new Vector2(2, 2);
    [SerializeField] int columns = 3;

    [Header("Diamond")]
    [SerializeField] Vector2 separation2 = new Vector2(2, 2);
    [SerializeField] bool backPositionOffset = false;

    [Header("Echelon")]
    [SerializeField] Vector2 separation3 = new Vector2(2, 2);
    [SerializeField] bool right = false;

    [Header("Grid")]
    [SerializeField] Vector2 separation4 = new Vector2(2, 2);
    [SerializeField] int rows = 3;

    [Header("Line")]
    [SerializeField] float separation5 = 2;
    [SerializeField] bool right2 = false;

    [Header("Row")]
    [SerializeField] Vector2 separation6 = new Vector2(2, 2);
    [SerializeField] int rows2 = 2;

    [Header("Square")]
    [SerializeField] float length = 5;
    private int[] agentsPerSide = new int[4];

    [Header("Triangle")]
    [SerializeField] float length2 = 5;
    private int[] agentPerSide2 = new int[3];

    [Header("V")]
    [SerializeField] Vector2 separation7 = new Vector2(2, 2);
    [SerializeField] bool fill2;
    private int currentRow2 = 1;
    private int currentAgentsPerRow2 = 0;
    private int lastIndex2;


    [Header("Wedge")]
    [SerializeField] Vector2 separation8 = new Vector2(2, 2);
    [SerializeField] bool fill;
    private int currentRow = 1;
    private int currentAgentsPerRow = 0;
    private int lastIndex;

    // Start is called before the first frame update
    void Start()
    {
        if (FormationChanged == null)
            FormationChanged = new UnityEvent();
    }

    public void SetFormation(int type)
    {
        fType = (FormationType) type;
        FormationChanged.Invoke();
    }

    public Vector3 GetFormation(Transform leader, int index, float zOffset, int size)
    {
        Vector3 pos = Vector3.zero;
        switch (fType)
        {
            case FormationType.Arc:
                pos = Arc(leader, index, zOffset, size);
                break;
            case FormationType.Circle:
                pos = Circle(leader, index, zOffset, size);
                break;
            case FormationType.Column:
                pos = Column(leader, index, zOffset);
                break;
            case FormationType.Diamond:
                pos = Diamond(leader, index, zOffset);
                break;
            case FormationType.Echelon:
                pos = Echelon(leader, index, zOffset);
                break;
            case FormationType.Grid:
                pos = Grid(leader, index, zOffset);
                break;
            case FormationType.Line:
                pos = Line(leader, index, zOffset);
                break;
            case FormationType.Row:
                pos = Row(leader, index, zOffset);
                break;
            case FormationType.Square:
                pos = Square(leader, index, zOffset, size);
                break;
            case FormationType.Triangle:
                pos = Triangle(leader, index, zOffset, size);
                break;
            case FormationType.V:
                pos = V(leader, index, zOffset);
                break;
            case FormationType.Wedge:
                pos = Wedge(leader, index, zOffset);
                break;
        }
        return pos;
    }

    private Vector3 Arc(Transform leader, int index, float zOffset, int size)
    {
        theta = Mathf.PI / size;
        return leader.TransformPoint(radius * Mathf.Sin(theta * index), 0, radius * Mathf.Cos(theta * index) - radius + zOffset);
    }

    private Vector3 Circle(Transform leader, int index, float zOffset, int size)
    {
        theta2 =  2 * Mathf.PI / size;
        return leader.TransformPoint(radius2 * Mathf.Sin(theta2 * index), 0, radius2 * Mathf.Cos(theta2 * index) - radius2 + zOffset);
    }   

    private Vector3 Column(Transform leader, int index, float zOffset)
    {
        var column = index % columns;
        var row = index / columns;

        Vector3 targetPos;
        if (column == 0)
        {
            // Position directly behind the leader
            targetPos = leader.TransformPoint(0, 0, -separation.y * row + zOffset);
        }
        else
        {
            // Alternate between the right and the left side of the center column
            targetPos = leader.TransformPoint(separation.x * (column % 2 == 0 ? -1 : 1) * (((column - 1) / 2) + 1), 0, -separation.y * row + zOffset);
        }

        return targetPos;
    }   

    private Vector3 Diamond(Transform leader, int index, float zOffset)
    {
        if (index == 0)
            return leader.TransformPoint(0, 0, zOffset);

        Vector3 targetPos;
        if (index < 3)
        { // form the arrow tip
            targetPos = leader.TransformPoint(separation2.x * (index % 2 == 0 ? -1 : 1), 0, -separation2.y + zOffset);
        }
        else
        { // form the line
            if (backPositionOffset)
            {
                targetPos = leader.TransformPoint(separation2.x * (index % 2 == 0 ? -0.5f : 0.5f), 0, -separation2.y * (((index - 1) / 2) + 1) + zOffset);
            }
            else
            {
                targetPos = leader.TransformPoint(0, 0, -separation2.y * (index - 1) + zOffset);
            }
        }
        return targetPos;
    }   

    private Vector3 Echelon(Transform leader, int index, float zOffset)
    {
        // Position at a diagonal relative to the leader
        return leader.TransformPoint(separation3.x * index * (right ? 1 : -1), 0, -separation3.y * index + zOffset);
    }   

    private Vector3 Grid(Transform leader, int index, float zOffset)
    {
        
        var row = index % rows;
        var column = index / rows;
        return leader.TransformPoint(separation4.x * column, 0, -separation4.y * row + zOffset);
    }    

    private Vector3 Line(Transform leader, int index, float zOffset)
    {
        return leader.TransformPoint(separation5 * index * (right2 ? 1 : -1), 0, zOffset);
    }     
    
    private Vector3 Row(Transform leader, int index, float zOffset)
    {
        var row = index % rows2;
        var column = index / rows2;

        Vector3 targetPos;
        if (column == 0)
        {
            // Position directly behind the leader
            targetPos = leader.TransformPoint(0, 0, -separation6.y * row + zOffset);
        }
        else
        {
            // Alternate between the right and the left side of the center column
            targetPos = leader.TransformPoint(separation6.x * (column % 2 == 0 ? -1 : 1) * (((column - 1) / 2) + 1), 0, -separation6.y * row + zOffset);
        }

        return targetPos;
    }

    private Vector3 Square(Transform leader, int index, float zOffset, int size)
    {
        if (index == 0)
            return leader.transform.position;


        for (int i = 0; i < 4; i++)
        {
            agentsPerSide[i] = size / 4 + (size % 4 > i ? 1 : 0);
        }

        var side = index % 4;

        var lengthMultiplier = (index / 4) / (float)agentsPerSide[side];
        
        lengthMultiplier = 1 - (lengthMultiplier - (int)lengthMultiplier);


        if (index == 1)
        {
            return leader.TransformPoint(length, 0, 0);
        }
        else if (index == 2)
        {
            return leader.TransformPoint(0, 0, length);
        }
        else if (index == 3)
        {
            return leader.TransformPoint(length, 0, length);
        }
        else if (index == 4)
        {
            return leader.TransformPoint((float)length / 2, 0, 0);
        }
        else if (index == 5)
        {
            return leader.TransformPoint(0, 0, (float)length / 2);
        }
        else if (index == 6)
        {
            return leader.TransformPoint(length, 0, (float)length / 2);
        }
        else
        {
            return leader.TransformPoint((float)length / 2, 0, length);
        }

        /*

        if (side == 0) //Right
        {
            return leader.TransformPoint(length / 2 * lengthMultiplier, 0, 0);
        }
        else if (side == 1) // Bottom
        {
            return leader.TransformPoint(0, 0, zOffset);
        }
        else if (side == 2)//Left
        {
            return leader.TransformPoint(-length / 2 * lengthMultiplier, 0, 0);
        } 
        else // Top
        {
            return leader.TransformPoint(0, 0, -zOffset);
        }
        */

    }  

    private Vector3 Triangle(Transform leader, int index, float zOffset, int size)
    {
        if (index == 0)
            return leader.transform.position;

        for(int i = 0; i < 3; i++)
        {
            agentPerSide2[i] = size / 3 + (size % 3 > i ? 1 : 0);
        }

        var side = index % 3;
        var lengthMultiplier = (index / 3) / (float)agentPerSide2[side];
        lengthMultiplier = 1 - (lengthMultiplier - (int)lengthMultiplier);
        var height = length2 / 2 * Mathf.Sqrt(3); // Equilateral triangle height

        

        if (side == 0) //Right
        {
            return leader.TransformPoint(length2 / 2 * lengthMultiplier, 0, -height * lengthMultiplier + zOffset);
        } 
        else if(side == 1) // Bottom
        {
            return leader.TransformPoint(Mathf.Lerp(-length2 / 2, length2 / 2, lengthMultiplier), 0, -height + zOffset);
        }
        else //Left
        {
            return leader.TransformPoint(-length2 / 2 * lengthMultiplier, 0, -height * lengthMultiplier + zOffset);
        }
    }

    private Vector3 V(Transform leader, int index, float zOffset)
    {

        if (index == 0)
            return leader.TransformPoint(0, 0, zOffset);

        if (fill)
        {
            if (index <= lastIndex2)
            {
                currentRow2 = 1;
                currentAgentsPerRow2 = 0;
            }
            lastIndex2 = index;

            var targetPosition = leader.TransformPoint(Mathf.Lerp(currentRow2 * separation7.x, currentRow2 * separation7.x, currentAgentsPerRow2 / (float)currentRow2), 0, separation7.y * currentRow2 + zOffset);

            currentAgentsPerRow2++;
            if(currentAgentsPerRow2 > currentRow2)
            {
                currentAgentsPerRow2 = 0;
                currentRow2++;
            }

            return targetPosition;


        }
        else
        {

            return leader.TransformPoint(separation7.x * (index % 2 == 0 ? -1 : 1) * (((index - 1) / 2) + 1), 0, separation7.y * (((index - 1) / 2) + 1) + zOffset);

        }

    }

    private Vector3 Wedge(Transform leader, int index, float zOffset)
    {
        if (index == 0)
            return leader.TransformPoint(0, 0, zOffset);

        if (fill)
        {
            // The wedge can optionally be filled in. I don't know of a nice formula which computes which row the agent should be in relative to its index so use the number of agents
            // already placed to determine the next position. If anybody knows of an easy formula to compute a filled in wedge please send an email to support@opsive.com.
            if (index <= lastIndex)
            {
                currentRow = 1;
                currentAgentsPerRow = 0;
            }
            lastIndex = index;

            var targetPosition = leader.TransformPoint(Mathf.Lerp(-currentRow * separation8.x, currentRow * separation8.x, currentAgentsPerRow / (float)currentRow), 0, -separation8.y * currentRow + zOffset);

            currentAgentsPerRow++;
            if (currentAgentsPerRow > currentRow)
            {
                currentAgentsPerRow = 0;
                currentRow++;
            }

            return targetPosition;
        }
        else
        {
            // The wedge is not filled in so the math is much easier.
            return leader.TransformPoint(separation8.x * (index % 2 == 0 ? -1 : 1) * (((index - 1) / 2) + 1), 0, -separation8.y * (((index - 1) / 2) + 1) + zOffset);
        }
    }


}
