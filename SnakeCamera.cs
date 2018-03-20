using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeCamera : MonoBehaviour
{
    [SerializeField] Transform snake;
    public Transform Snake { get { return snake; } set { snake = value; } }
    [SerializeField] float distanceFromPlayerToSnake;
    Snake snakeInfo;
    Transform player;
    Vector3 snakeHalfwayPoint;
    Vector3 snakeLastBodyPosition;
    float cameraOffsetDifferential = .05f;

	// Use this for initialization
	void Start ()
    {
        player = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(snake != null && snakeInfo == null)
        {
            snakeInfo = snake.GetComponent<Snake>();
        }

        if (snakeInfo != null)
        {
            if (snakeInfo.SnakeBodiesCount > 0)
            {
                snakeLastBodyPosition = snakeInfo.GetLastSnakeBodyPosition();
                snakeHalfwayPoint = ((snake.position - snakeLastBodyPosition) * .6f) + snakeLastBodyPosition;

                distanceFromPlayerToSnake = (snakeHalfwayPoint - player.position).magnitude;
                cameraOffsetDifferential = Mathf.Lerp(.15f, .05f, (Mathf.Abs(distanceFromPlayerToSnake / 30)));

                transform.position = (snakeHalfwayPoint - player.position) * (1 - ((snakeInfo.SnakeBodiesCount + 1) * cameraOffsetDifferential));
                transform.LookAt(snakeHalfwayPoint);
            }
        }
	}
}
