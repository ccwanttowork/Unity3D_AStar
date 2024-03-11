using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path 
{
    public bool bDebug = false;
    public Vector3[] pointA=new Vector3[0];
    public float Radis = 1.0f;

    public float Length
    {
        get
        {
            return pointA.Length;
        }
    }
    public void SetPoints(ArrayList path)
    {
        int index = 0;
        pointA = new Vector3[path.Count];
        foreach(Node node in path)
        {
            pointA[index] = node.position;
            index++;
        }
    }
    public Vector3 GetPoint(int index)
    {
        return pointA[index];
    }
    
}
