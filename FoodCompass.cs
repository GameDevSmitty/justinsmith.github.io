using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCompass : MonoBehaviour
{
    [SerializeField] Transform snakeToGetFoodFrom;
    public Transform SnakeToGetFoodFrom { get { return snakeToGetFoodFrom; } set { snakeToGetFoodFrom = value; } }

    Vector3 lookAtTransform;
    Snake snake;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (snakeToGetFoodFrom != null && snake == null)
        {
            snake = snakeToGetFoodFrom.GetComponent<Snake>();
        }

        if (snake.AcquiredTarget != null)
        {
            lookAtTransform = snake.AcquiredTarget.transform.position - transform.position;
        }

        transform.up = lookAtTransform;
    }
}
