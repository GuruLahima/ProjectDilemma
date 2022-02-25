using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GuruLaghima;

public class SimpleCameraController : MonoBehaviour
{
  Vector2 rotation = Vector2.zero;
  public float speed = 3;
  public float yMin = 3;
  public float yMax = 3;
  public float xMin = 3;
  public float xMax = 3;

  private void Start()
  {
    rotation = transform.eulerAngles;
    MyDebug.Log(rotation.ToString());
    yMin = rotation.y + yMin;
    yMax = rotation.y + yMax;
    xMin = rotation.x + xMin;
    xMax = rotation.x + xMax;
  }

  void Update()
  {
    rotation.y += Input.GetAxis("Mouse X");
    rotation.x += -Input.GetAxis("Mouse Y");

    rotation.y = Mathf.Clamp(rotation.y, yMin, yMax);
    rotation.x = Mathf.Clamp(rotation.x, xMin, xMax);

    // transform.eulerAngles = (Vector2)rotation * speed;
    transform.rotation = Quaternion.AngleAxis(rotation.y, Vector3.up) * Quaternion.AngleAxis(rotation.x, Vector3.right);
  }
}