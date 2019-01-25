using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GangaGame;

public delegate void TargetsVisibilityChange(List<Transform> newTargets);

[ExecuteInEditMode]
public class FieldOfView : MonoBehaviour
{
    public float viewRadius;

    [Range(0, 360)]
    public float viewAngle;

    public float viewDepth;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public int meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;


    public MeshFilter viewMeshFilter;
    public bool debug;
    Mesh viewMesh;

    public static event TargetsVisibilityChange OnTargetsVisibilityChange;

    public FogProjector fogProjector;
    public float updateDistance = 1;
    Vector3 lastUpdatePos;

    void OnEnable()
    {
        viewMesh = new Mesh {name = "View Mesh"};
        viewMeshFilter.mesh = viewMesh;

        fogProjector = fogProjector ?? FindObjectOfType<FogProjector>();
    }

    private float timerToFindTargets = 0.0f;
    void LateUpdate()
    {
        timerToFindTargets += Time.fixedDeltaTime;
        if(timerToFindTargets >= 2.0f)
        {
            FindVisibleTargets();
            timerToFindTargets = 0.0f;
        }
        DrawFieldOfView();
        if (Vector3.Distance(transform.position, lastUpdatePos) > updateDistance || Time.time<.5f)
        {
            lastUpdatePos = transform.position;
        }
    }

    List<Transform> oldVisibleTargets = new List<Transform>();
    Collider[] targetsInViewRadius;
    BaseBehavior baseBehaviorComponent;
    Vector3 offset = new Vector3(0, 1, 0);
    public void FindVisibleTargets()
    {
        visibleTargets.Clear();
        oldVisibleTargets.Clear();
        oldVisibleTargets.AddRange(visibleTargets);

        baseBehaviorComponent = gameObject.GetComponentInParent<BaseBehavior>();
        if (baseBehaviorComponent.live)
        {
            targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius - 1.5f, targetMask);
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position + offset - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        visibleTargets.Add(target);
                    }
                    else if(target.gameObject.layer == LayerMask.NameToLayer("Ambient") || target.gameObject.layer == LayerMask.NameToLayer("Building"))
                    {
                        if (Physics.Raycast(transform.position, dirToTarget))
                            visibleTargets.Add(target);
                    }
                }
            }
        }

        // if (OnTargetsVisibilityChange != null) OnTargetsVisibilityChange(visibleTargets);
        
        foreach(Transform newElement in visibleTargets.Where(p => !oldVisibleTargets.Any(l => p.GetHashCode() == l.GetHashCode())).ToArray())
            ElementStartVisible(newElement.gameObject);

        foreach (Transform oldElement in oldVisibleTargets.Where(p => !visibleTargets.Any(l => p.GetHashCode() == l.GetHashCode())).ToArray())
            if (oldElement != null)
                ElementStopVisible(oldElement.gameObject);
    }

    public void ElementStartVisible(GameObject target)
    {
        BaseBehavior baseBehaviorComponent = target.GetComponent<BaseBehavior>();
        BaseBehavior senderBaseBehavior = gameObject.GetComponentInParent<BaseBehavior>();
        baseBehaviorComponent.StartVisible(senderBaseBehavior);
        // Debug.Log("ElementStartVisible: " + target.name);
    }

    public void ElementStopVisible(GameObject target)
    {
        BaseBehavior baseBehaviorComponent = target.GetComponent<BaseBehavior>();
        BaseBehavior senderBaseBehavior = gameObject.GetComponentInParent<BaseBehavior>();
        baseBehaviorComponent.StopVisible(senderBaseBehavior);
        // Debug.Log("ElementStopVisible: " + target.name);
    }

    void DrawFieldOfView()
    {
        float stepAngleSize = viewAngle / meshResolution;
        List<Vector3> viewPoints = new List<Vector3>();
        ObstacleInfo oldObstacle = new ObstacleInfo();
        for (int i = 0; i <= meshResolution; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ObstacleInfo newObstacle = FindObstacles(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldObstacle.dst - newObstacle.dst) > edgeDstThreshold;
                if (oldObstacle.hit != newObstacle.hit ||
                    oldObstacle.hit && edgeDstThresholdExceeded)
                {
                    EdgeInfo edge = FindEdge(oldObstacle, newObstacle);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }


            viewPoints.Add(newObstacle.point);
            oldObstacle = newObstacle;
        }

        int vertexCount = viewPoints.Count + 1;
        var vertices = new Vector3[vertexCount];
        var triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }


    EdgeInfo FindEdge(ObstacleInfo minObstacle, ObstacleInfo maxObstacle)
    {
        float minAngle = minObstacle.angle;
        float maxAngle = maxObstacle.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ObstacleInfo newObstacle = FindObstacles(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minObstacle.dst - newObstacle.dst) > edgeDstThreshold;
            if (newObstacle.hit == minObstacle.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newObstacle.point;
            } else
            {
                maxAngle = angle;
                maxPoint = newObstacle.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


    ObstacleInfo FindObstacles(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (DebugRayCast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ObstacleInfo(true, hit.point + hit.normal * -viewDepth, hit.distance, globalAngle);
        }
        return new ObstacleInfo(false, transform.position + dir * (viewRadius - viewDepth), viewRadius, globalAngle);
    }

    bool DebugRayCast(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int mask)
    {
        if (Physics.Raycast(origin, direction, out hit, maxDistance, mask))
        {
            if (debug)
                Debug.DrawLine(origin, hit.point);
            return true;
        }
        if (debug)
            Debug.DrawLine(origin, origin + direction * maxDistance);
        return false;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool isGlobal)
    {
        if (!isGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ObstacleInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ObstacleInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}