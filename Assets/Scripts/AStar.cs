using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    public static PriorityQueue closedList, openList;
    private static float NodeCost(Node a,Node b)
    {
        Vector3 vecCost = a.position - b.position;
        return vecCost.magnitude;    //����ģ
    }
    public static ArrayList FindPath(Node start,Node goal)
    {

        //A*�㷨��������
        openList = new PriorityQueue();
        openList.Push(start);
        start.nodeTotalCost = 0.0f;
        start.estimatedCost=NodeCost(start, goal);

        closedList=new PriorityQueue();
        Node node = null;
        while(openList.Length!=0)
        {
            node = openList.First();
            if(node.position==goal.position)
            {
                return CalculatePath(node);
            }
            ArrayList neighbours= new ArrayList();
            GridManager.instance.GetNeighbours(node, neighbours);  //�ڽӵ������������
            for(int i=0;i<neighbours.Count; i++)
            {
                Node neighbourNode = (Node)neighbours[i];
                if(!closedList.Contains(neighbourNode))
                {
                    float cost = NodeCost(node, neighbourNode);  //ԭ����
                    float totalCost = node.nodeTotalCost + cost;

                    float neighbourNodeEstCost = NodeCost(neighbourNode, goal);


                    neighbourNode.nodeTotalCost = totalCost;
                    neighbourNode.estimatedCost = totalCost+neighbourNodeEstCost;
                    neighbourNode.parent = node;

                    if(!openList.Contains(neighbourNode))
                    {
                        openList.Push(neighbourNode);
                    }
                }
                
            }
            closedList.Push(node);
            openList.Remove(node);
        }
        if (node.position != goal.position)
        {
            Debug.LogError("Goal Not Found");
            return null;
        }
         
        return CalculatePath(node);   //����·��
    }

    private static ArrayList CalculatePath(Node node)
    {
        ArrayList list=new ArrayList();
        while(node != null) 
        {
            list.Add(node);
            node = node.parent;
        }
        list.Reverse();
        return list;
    }
}
