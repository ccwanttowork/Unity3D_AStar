using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //单实例模式
    private static GridManager s_Instance=null;

    public static GridManager instance
    {
        get
        {
            if(s_Instance == null)
            {
                s_Instance=FindObjectOfType(typeof(GridManager)) as GridManager;
                if(s_Instance == null) //没找到
                {
                    Debug.Log("Could not locate an GridManager object.\nYou have to have exactly one GridManager in the scene");
                }
            }
            return s_Instance;
        }
    }
    void OnAppcationQuit()
    {
        s_Instance = null;
    }




    public int numOfRows;
    public int numOfColumns;
    public float gridCellSize;
    public bool showGrid = true;    //画出来
    private GameObject[] obstacleList;
    private Node[,] nodes;    //二维节点数组
    private Vector3 origin = new Vector3();

    public Vector3 Origin
    {
        get{ return origin; }
    }

    void Awake()     //不激活也执行，记录障碍物
    {   

        origin=this.transform.position;

        //查找带有标签的红色方块位置并记为数组
        obstacleList = GameObject.FindGameObjectsWithTag("Obstacle");
        CalculateObstacles();

    }

    private void CalculateObstacles()
    {
        nodes = new Node[numOfRows, numOfColumns];   //计算网格数组

        int index = 0;   //序号计算中心点位置
        for(int i=0;i<numOfColumns;i++)
        {
            for(int j=0;j<numOfRows;j++)
            {
                Node node = new Node(GetGridCellCenter(index));
                nodes[i, j] = node;
                index++;
            }
        }
        if(obstacleList != null&&obstacleList.Length>0) 
        {
            foreach(GameObject data in obstacleList)
            {
                int indexCell = GetGridIndex(data.transform.position);
                int col = GetColumn(indexCell);
                int row= GetRow(indexCell);
                nodes[row, col].MartAsObstacle();
            }

        }


    }

    public int GetGridIndex(Vector3 pos)
    {
        pos -= Origin;
        int col = (int)(pos.x/gridCellSize);
        int row = (int)(pos.z/gridCellSize);

        return row * numOfColumns + col;                         
    }

    public Vector3 GetGridCellCenter(int index)
    {
        Vector3 cellposition = GetGridCellPosition(index);
        cellposition.x += (gridCellSize / 2.0f);
        cellposition.z += (gridCellSize / 2.0f);
        return cellposition;
    }

    public Vector3 GetGridCellPosition(int index)
    {
        int row = GetRow(index);
        int col = GetColumn(index);
        float xPosIngrid = col * gridCellSize;
        float zPosIngrid = row * gridCellSize;

        return Origin + new Vector3(xPosIngrid, 0.0f, zPosIngrid);

    }

    public int GetColumn(int index)
    {
        int col = index % numOfColumns;
        return col;
    }

    public int GetRow(int index)
    {
        int row = index / numOfColumns;
        return row;
    }


    // Start is called before the first frame update
    // void Start()     //脚本不激活就不执行
    //{

    // }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDrawGizmos()
    {
       if(showGrid)
        {
            DebugDrawGrid(transform.position,numOfRows,numOfColumns,gridCellSize,Color.blue);

        }
        Gizmos.DrawSphere(transform.position, 0.5f);    //画一个小球体
    }

    private void DebugDrawGrid(Vector3 origin, int numRows, int numColumns, float CellSize, Color color)
    {
        float width = numColumns * CellSize;
        float height= numRows * CellSize;

        for(int i=0;i<numRows+1;i++)    //画水平线
        {
            Vector3 startPos = origin + i * CellSize * new Vector3(0.0f, 0.0f, 1.0f);   //Z坐标
            Vector3 endPos=startPos+width*new Vector3(1.0f, 0.0f, 0.0f);   //x坐标
            Debug.DrawLine(startPos, endPos,color);
        }
        for(int i=0; i<numRows+1;i++) 
        {
            Vector3 startPos = origin + i * CellSize * new Vector3(1.0f, 0.0f, 0.0f);   //X坐标
            Vector3 endPos = startPos + height * new Vector3(0.0f, 0.0f, 1.0f);   //Z坐标
            Debug.DrawLine(startPos, endPos, color);
        }
    }
    public void GetNeighbours(Node node,ArrayList neighbors)
    {
        Vector3 neighborPos = node.position;
        int neighborIndex = GetGridIndex(neighborPos);

        int row = GetRow(neighborIndex);
        int column=GetColumn(neighborIndex);

        //下
        int leftNodeRow = row - 1;
        int leftNodeColumn = column;
        AssigbNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //上
        leftNodeRow = row + 1;
        leftNodeColumn = column;
        AssigbNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //右
        leftNodeRow = row;
        leftNodeColumn = column+1;
        AssigbNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //左
        leftNodeRow = row ;
        leftNodeColumn = column-1;
        AssigbNeighbour(leftNodeRow, leftNodeColumn, neighbors);
    }

    private void AssigbNeighbour(int row, int column, ArrayList neighbors)
    {
        if(row!=-1&&column!=-1&&row<numOfRows&&column<numOfColumns)
        {
            Node nodeToAdd = nodes[row, column];
            if (!nodeToAdd.bObstacle)
            {
                neighbors.Add(nodeToAdd);
            }
        }


    }
}
