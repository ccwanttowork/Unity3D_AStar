using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //��ʵ��ģʽ
    private static GridManager s_Instance=null;

    public static GridManager instance
    {
        get
        {
            if(s_Instance == null)
            {
                s_Instance=FindObjectOfType(typeof(GridManager)) as GridManager;
                if(s_Instance == null) //û�ҵ�
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
    public bool showGrid = true;    //������
    private GameObject[] obstacleList;
    private Node[,] nodes;    //��ά�ڵ�����
    private Vector3 origin = new Vector3();

    public Vector3 Origin
    {
        get{ return origin; }
    }

    void Awake()     //������Ҳִ�У���¼�ϰ���
    {   

        origin=this.transform.position;

        //���Ҵ��б�ǩ�ĺ�ɫ����λ�ò���Ϊ����
        obstacleList = GameObject.FindGameObjectsWithTag("Obstacle");
        CalculateObstacles();

    }

    private void CalculateObstacles()
    {
        nodes = new Node[numOfRows, numOfColumns];   //������������

        int index = 0;   //��ż������ĵ�λ��
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
    // void Start()     //�ű�������Ͳ�ִ��
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
        Gizmos.DrawSphere(transform.position, 0.5f);    //��һ��С����
    }

    private void DebugDrawGrid(Vector3 origin, int numRows, int numColumns, float CellSize, Color color)
    {
        float width = numColumns * CellSize;
        float height= numRows * CellSize;

        for(int i=0;i<numRows+1;i++)    //��ˮƽ��
        {
            Vector3 startPos = origin + i * CellSize * new Vector3(0.0f, 0.0f, 1.0f);   //Z����
            Vector3 endPos=startPos+width*new Vector3(1.0f, 0.0f, 0.0f);   //x����
            Debug.DrawLine(startPos, endPos,color);
        }
        for(int i=0; i<numRows+1;i++) 
        {
            Vector3 startPos = origin + i * CellSize * new Vector3(1.0f, 0.0f, 0.0f);   //X����
            Vector3 endPos = startPos + height * new Vector3(0.0f, 0.0f, 1.0f);   //Z����
            Debug.DrawLine(startPos, endPos, color);
        }
    }
    public void GetNeighbours(Node node,ArrayList neighbors)
    {
        Vector3 neighborPos = node.position;
        int neighborIndex = GetGridIndex(neighborPos);

        int row = GetRow(neighborIndex);
        int column=GetColumn(neighborIndex);

        //��
        int leftNodeRow = row - 1;
        int leftNodeColumn = column;
        AssigbNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //��
        leftNodeRow = row + 1;
        leftNodeColumn = column;
        AssigbNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //��
        leftNodeRow = row;
        leftNodeColumn = column+1;
        AssigbNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //��
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
