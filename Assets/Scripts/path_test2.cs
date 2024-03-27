using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class path_test2 : MonoBehaviour
{
    public Path path = new Path();
    public float speed = 5.0f;
    public float mass = 5.0f;
    public bool isLooping = false;

    private float curspeed;
    private int curPathIndex;
    private float pathLength;
    private Vector3 targetPoint;

    Vector3 velocity;

    private Transform startPos1, endPos1;
    public Node startNode1 { get; set; }
    public Node goalNode1 { get; set; }

    GameObject objStartNodeCube1, objEndNodeCube1;
    public ArrayList pathArray1;
    //设置时间范围   隔多久查找一次
    public float intervalTime = 1.0f;
    private float elapsedTime = 0.0f;



    // Start is called before the first frame update

    void Start()
    {
        objStartNodeCube1 = GameObject.FindGameObjectWithTag(this.gameObject.tag);
        objEndNodeCube1 = GameObject.FindGameObjectWithTag("End3");
        pathArray1 = new ArrayList();
        FindPath();

        path.SetPoints(pathArray1);

        pathLength = path.Length;
        curPathIndex = 0;

        //先转向 
        velocity += Steer(targetPoint, false);
    }
    private void FindPath()
    {

        //变位置
        startPos1 = objStartNodeCube1.transform;
        endPos1 = objEndNodeCube1.transform;

        //变节点
        startNode1 = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos1.position)));
        goalNode1 = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos1.position)));
        pathArray1 = AStar.FindPath(startNode1, goalNode1);
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
            pathLength = pathArray1.Count;
            path.SetPoints(pathArray1);
        }


        curspeed = speed * Time.deltaTime;
        targetPoint = path.GetPoint(curPathIndex);

        if (Vector3.Distance(transform.position, targetPoint) < path.Radis)
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
            velocity += Steer(targetPoint, false);

        transform.position += velocity;
        transform.rotation = Quaternion.LookRotation(velocity);

    }
    public Vector3 Steer(Vector3 target, bool bFinalPoint)  //朝向
    {
        Vector3 desiredVelocity = (target - transform.position);
        float dist = desiredVelocity.magnitude;
        desiredVelocity.Normalize();   //单位化
        if (bFinalPoint && dist < 0.48f)
            Destroy(this.gameObject);
        else if (bFinalPoint && dist < 10.0f)
            desiredVelocity *= (curspeed * (dist / 10.0f));   //减速
        else desiredVelocity *= curspeed;

        //调整转向
        Vector3 steeringForce = desiredVelocity - velocity;
        Vector3 accelerating = steeringForce / mass;

        return accelerating;
    }

    void OnDrawGizmos()
    {
        if (pathArray1 == null) return;
        if (pathArray1.Count > 0)
        {
            int index = 1;
            foreach (Node node in pathArray1)
            {
                if (index < pathArray1.Count)
                {
                    Node nextNode = (Node)pathArray1[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.green);
                    index++;
                }
            }
        }
    }
}

