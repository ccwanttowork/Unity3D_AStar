using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowing : MonoBehaviour
{
    public Path path=new Path();
    public float speed = 5.0f;
    public float mass = 5.0f;
    public bool isLooping = false;

    private float curspeed;
    private int curPathIndex;
    private float pathLength;
    private Vector3 targetPoint;

    Vector3 velocity;

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
        pathArray = new ArrayList();
        FindPath();

        path.SetPoints(pathArray);

        pathLength = path.Length;
        curPathIndex = 0;

        //先转向 
        velocity += Steer(targetPoint, false);
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
        if (elapsedTime >= intervalTime)
        {
            elapsedTime = 0.0f;
            FindPath();
            curPathIndex = 0;
            pathLength=pathArray.Count;
            path.SetPoints(pathArray);
        }


        curspeed = speed * Time.deltaTime;
        targetPoint = path.GetPoint(curPathIndex);

        if(Vector3.Distance(transform.position,targetPoint)<path.Radis)
        {
            if (curPathIndex < pathLength - 1)
                curPathIndex++;
            else if (isLooping)
            {
                curPathIndex = 0;
            }
            else return;
        }

        if (curPathIndex >= pathLength) return;

        if (curPathIndex >= pathLength - 1 && !isLooping)
            velocity += Steer(targetPoint, true);
        else
            velocity += Steer(targetPoint,false);

        transform.position += velocity;
        transform.rotation = Quaternion.LookRotation(velocity);

    }
    public Vector3 Steer(Vector3 target,bool bFinalPoint)  //朝向
    {
        Vector3 desiredVelocity=(target - transform.position);
        float dist = desiredVelocity.magnitude;
        desiredVelocity.Normalize();   //单位化
        if (bFinalPoint && dist < 10.0f)
            desiredVelocity *= (curspeed * (dist / 10.0f));   //减速
        else desiredVelocity *= curspeed;

        //调整转向
        Vector3 steeringForce = desiredVelocity - velocity;
        Vector3 accelerating = steeringForce / mass;

        return accelerating;
    }

    void OnDrawGizmos()
    {
        if (pathArray == null) return;
        if (pathArray.Count > 0)
        {
            int index = 1;
            foreach (Node node in pathArray)
            {
                if (index < pathArray.Count)
                {
                    Node nextNode = (Node)pathArray[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.blue);
                    index++;
                }
            }
        }
    }
}
