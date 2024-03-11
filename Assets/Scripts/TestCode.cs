using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestCode : MonoBehaviour
{

    private Transform startPos, endPos;
    public Node startNode { get; set; }
    public Node goalNode { get; set; }


    GameObject objStartNodeCube, objEndNodeCube;
    public ArrayList pathArray;
    //设置时间范围   隔多久查找一次
    public float intervalTime = 1.0f;
    private float elapsedTime = 0.0f;








    // Start is called before the first frame update
    void Start()
    {
        objStartNodeCube = GameObject.FindGameObjectWithTag("Start");
        objEndNodeCube = GameObject.FindGameObjectWithTag("End");

        FindPath();

    }

    private void FindPath()
    {

        //变位置
        startPos = objStartNodeCube.transform;
        endPos = objEndNodeCube.transform;

        //变节点
        startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos.position)));
        goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos.position)));
        pathArray = AStar.FindPath(startNode, goalNode);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= intervalTime)
        {
            elapsedTime = 0.0f;
            FindPath();
        }
    }
    void OnDrawGizmos()
    {
        if (pathArray == null) return;
        if(pathArray .Count > 0) 
        {
            int index = 1;
            foreach(Node node in pathArray) 
            {
                if(index<pathArray.Count)
                {
                    Node nextNode = (Node)pathArray[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.black);
                    index++;
                }
            }
        }
    }
}
