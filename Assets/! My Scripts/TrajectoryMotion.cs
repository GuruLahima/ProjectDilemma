using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TrajectoryMotion
{
  private static float gravity = Mathf.Abs(Physics.gravity.y);
  /// <summary>
  /// Creates a line using <paramref name="lineRenderer"/>, from a <paramref name="startingPoint"/> by calculating the trajectory using <paramref name="throwAngle"/> and <paramref name="velocity"/>
  /// <para><paramref name="resolution"/> is the amount of rays that the Physics.Linecast() should use in the trajectory to check for collisions </para>
  /// <para><paramref name="LRRM"/> short for lineRendererResolutionMultiplier is the amount of points on the <paramref name="lineRenderer"/> multiple of the <paramref name="resolution"/>, so that each two points of a ray contain a fixed amount of lines in between</para>
  /// <para><paramref name="hitIndicator"/> is the object you want to be placed on the very first hit of the raycast, to return the position, simply get the position of the last point in the <paramref name="lineRenderer"/> if <paramref name="hitIndicator"/> is unused</para>
  /// <para>For <paramref name="startingPoint"/> you can simply use <paramref name="lineRenderer"/>'s Transform</para>
  /// </summary>
  /// <param name="lineRenderer"></param>
  /// <param name="startingPoint"></param>
  /// <param name="throwAngle"></param>
  /// <param name="velocity"></param>
  /// <param name="resolution"></param>
  /// <param name="LRRM"></param>
  /// <param name="rayLayerMask"></param>
  /// <param name="hitIndicator"></param>
  /// <param name="customGravity"></param>
  public static void AIM(LineRenderer lineRenderer, Transform startingPoint, float throwAngle, float velocity, int resolution, int LRRM,
    int rayLayerMask = Physics.DefaultRaycastLayers, GameObject hitIndicator = null, float? customGravity = null, GameObject decal = null)
  {
    // init custom gravity calculations otherwise use default physics gravity
    if (customGravity != null)
      gravity = (float)customGravity;

    // for the calculations we will be using radians
    float radianAngle = Mathf.Deg2Rad * throwAngle;

    //
    float Vx = velocity * Mathf.Cos(radianAngle);
    float Vy = velocity * Mathf.Sin(radianAngle);
    float distanceRaycast = Vx * (Vy + Mathf.Sqrt(Vy * Vy + 2 * gravity * startingPoint.transform.position.y)) / gravity;


    Vector3[] _arcArray = CalculateArcArray(resolution, distanceRaycast, velocity, radianAngle);

    RaycastHit hit;
    Vector3? hitPosition = null; int index = resolution;
    for (int i = 0; i < _arcArray.Length - 1; i++)
    {
      Debug.DrawLine(startingPoint.transform.TransformPoint(_arcArray[i]), startingPoint.transform.TransformPoint(_arcArray[i + 1]), Color.red, 0.5f);
      if (Physics.Linecast(startingPoint.transform.TransformPoint(_arcArray[i]), startingPoint.transform.TransformPoint(_arcArray[i + 1]), out hit, rayLayerMask, QueryTriggerInteraction.Ignore))
      {
        // show where the projectile is going to hit (if indicator is assigned)
        if (hitIndicator)
        {
          hitIndicator.transform.position = hit.point;
          hitIndicator.SetActive(true);
        }
        if (decal)
        {
          decal.transform.forward = -hit.normal;
          decal.transform.position = hit.point - decal.transform.forward * 0.01f;
          decal.SetActive(true);
        }

        // we store the hit values
        index = i;
        hitPosition = startingPoint.transform.InverseTransformPoint(hit.point);
        break;
      }
      else
      {
        // hide the indicator in case the projectile is not going to hit anything in its trajectory
        if (hitIndicator)
          hitIndicator.SetActive(false);
        if (decal)
          decal.SetActive(false);
      }
    }

    int lineRendererResolution = resolution * LRRM;

    List<Vector3> _indicatorList = CalculateArcArray(lineRendererResolution, distanceRaycast, velocity, radianAngle).ToList();
    int trimIndex = index * LRRM;
    if (trimIndex <= 0) trimIndex = 1;
    _indicatorList.RemoveRange(trimIndex, lineRendererResolution - trimIndex);

    // set the last point to match the hit point
    if (hitPosition != null)
      _indicatorList[trimIndex] = (Vector3)hitPosition;

    // set up the line renderer
    lineRenderer.positionCount = trimIndex + 1;
    lineRenderer.SetPositions(_indicatorList.ToArray());
  }

  static Vector3[] CalculateArcArray(int resolution, float maxDistance, float velocity, float radianAngle)
  {
    Vector3[] arcArray = new Vector3[resolution + 1];

    for (int i = 0; i <= resolution; i++)
    {
      float t = (float)i / (float)resolution;
      arcArray[i] = CalculateArcPoint(t, maxDistance, velocity, radianAngle);
    }

    return arcArray;
  }

  static Vector3 CalculateArcPoint(float t, float maxDistance, float velocity, float radianAngle)
  {
    float x = t * maxDistance;
    float y = x * Mathf.Tan(radianAngle) - ((gravity * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
    float z = 0f;
    return new Vector3(z, y, x);
  }
}
