using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycasterSnake : MonoBehaviour, ISubscriber
{
    [SerializeField] float raycastDistanceMax = 50f;
    [SerializeField] float raycastDistanceMin = 20f;
    [TooltipAttribute("Layer for Food")]
    [SerializeField]
    int layerForFood;
    [TooltipAttribute("Layer for Snake")]
    [SerializeField]
    int layerForSnake;
    [TooltipAttribute("Layer for Boundary")]
    [SerializeField]
    int layerForBoundary;
    [SerializeField] Transform rotator;
    [SerializeField] Transform directionFromHitToSnake;
    [SerializeField] bool isEasyMode;

    float xRandom;
    float yRandom;
    float zRandom;
    float distanceRandom;
    Vector3 rayDirection;
    RaycastHit hit;

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    void Start()
    {
        layerForFood = 1 << layerForFood;
        layerForBoundary = 1 << layerForBoundary;
        layerForSnake = 1 << layerForSnake;
    }

    void RaycastToSpawnFood(Vector3 v)
    {
        bool isOverLapping = false;

        do
        {

            isOverLapping = SnakeMode();

        } while (isOverLapping);
    }

    bool SnakeMode()
    {
        Vector3 spawnPoint;
        RaycastHit hit;

        xRandom = Random.Range(0f, 359.9f);
        yRandom = Random.Range(0f, 359.9f);
        zRandom = Random.Range(0f, 359.9f);
        distanceRandom = Random.Range(raycastDistanceMin, raycastDistanceMax);

        rayDirection = new Vector3(xRandom, yRandom, zRandom);

        rotator.eulerAngles = rayDirection;

        if (Physics.Raycast(rotator.position, rotator.forward, out hit, distanceRandom, layerForBoundary) == false)
        {
            if (!Physics.CheckSphere(rotator.position + rotator.forward * distanceRandom, .7f, layerForFood) &&
                !Physics.CheckSphere(rotator.position + rotator.forward * distanceRandom, .7f, layerForSnake))
            {
                spawnPoint = rotator.position + rotator.forward * distanceRandom;

                EventManager.CallRaycastAction(spawnPoint);
                return false;
            }
            else
                return true;
        }
        else
        {
            directionFromHitToSnake.position = transform.position - hit.point;
            Vector3 hitPointAdjusted = directionFromHitToSnake.position + directionFromHitToSnake.forward * 2f;

            Debug.Log((transform.position - hitPointAdjusted).magnitude);

            if ((transform.position - hitPointAdjusted).magnitude >= raycastDistanceMin)
            {
                if (!Physics.CheckSphere(hitPointAdjusted, .7f, layerForFood) &&
                    !Physics.CheckSphere(hitPointAdjusted, .7f, layerForSnake))
                {
                    spawnPoint = hitPointAdjusted;

                    EventManager.CallRaycastAction(spawnPoint);
                    return false;
                }
                else
                    return true;
            }
            else
                return true;
        }
    }

    public void Subscribe()
    {
        EventManager.OnTimerAction += RaycastToSpawnFood;
    }

    public void Unsubscribe()
    {
        EventManager.OnTimerAction -= RaycastToSpawnFood;
    }
}
