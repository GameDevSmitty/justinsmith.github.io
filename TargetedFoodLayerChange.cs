using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedFoodLayerChange : MonoBehaviour, ISubscriber
{
    [SerializeField] Material redMat;
    [SerializeField] Material blueMat;
    [SerializeField] Material greenMat;
    [SerializeField] Material ultraMat;
    [SerializeField] Material redTargetedMat;
    [SerializeField] Material blueTargetedMat;
    [SerializeField] Material greenTargetedMat;
    [SerializeField] Material ultraTargetedMat;

    Snake snake;
    List<Transform> gameObjectsAttachedToThis = new List<Transform>();

    GameObject childGameObject;
    MeshRenderer[] meshRenderers;

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    // Update is called once per frame
    void Update ()
    {
	    if(GameObject.FindGameObjectWithTag("Predators") != null)
        {
            snake = GameObject.FindGameObjectWithTag("Predators").GetComponent<Snake>();
        }

        if(snake != null)
        {
            if(snake.AcquiredTarget == gameObject && gameObject.layer != 12)
            {
                gameObject.layer = 12;

                Transform[] allChildren = GetComponentsInChildren<Transform>(true);

                for (int childIndex = 0; childIndex < allChildren.Length; ++childIndex)
                {
                    if(allChildren[childIndex].gameObject.activeSelf)
                        gameObjectsAttachedToThis.Add( allChildren[childIndex] );
                }
                
                foreach (Transform t in gameObjectsAttachedToThis)
                {
                    t.gameObject.layer = 12;
                }

                if (childGameObject != null)
                {
                    meshRenderers = childGameObject.GetComponentsInChildren<MeshRenderer>();
                }

                switch (childGameObject.name)
                {
                    case "Red":
                        foreach (MeshRenderer mr in meshRenderers)
                        {
                            if(mr.transform.name != "Googley Eye")
                                mr.material = redTargetedMat;
                        }
                        break;
                    case "Blue":
                        foreach (MeshRenderer mr in meshRenderers)
                        {
                            if (mr.transform.name != "Googley Eye")
                                mr.material = blueTargetedMat;
                        }
                        break;
                    case "Green":
                        foreach (MeshRenderer mr in meshRenderers)
                        {
                            if (mr.transform.name != "Googley Eye")
                                mr.material = greenTargetedMat;
                        }
                        break;
                    case "Ultra":
                        foreach (MeshRenderer mr in meshRenderers)
                        {
                            if (mr.transform.name != "Googley Eye")
                                mr.material = ultraTargetedMat;
                        }
                        break;
                    default:
                        Debug.Log("Wrong name given to switch");
                        break;
                }

            }

            if (snake.AcquiredTarget != gameObject && gameObject.layer == 12)
            {
                gameObject.layer = 9;

                Transform[] allChildren = GetComponentsInChildren<Transform>(true);

                for (int childIndex = 0; childIndex < allChildren.Length; ++childIndex)
                {
                    gameObjectsAttachedToThis.Add(allChildren[childIndex]);
                }

                foreach (Transform t in gameObjectsAttachedToThis)
                {
                    t.gameObject.layer = 9;
                }

                switch (childGameObject.name)
                {
                    case "Red":
                        foreach (MeshRenderer mr in meshRenderers)
                        {
                            mr.material = redMat;
                        }
                        break;
                    case "Blue":
                        foreach (MeshRenderer mr in meshRenderers)
                        {
                            mr.material = blueMat;
                        }
                        break;
                    case "Green":
                        foreach (MeshRenderer mr in meshRenderers)
                        {
                            mr.material = greenMat;
                        }
                        break;
                    case "Ultra":
                        foreach (MeshRenderer mr in meshRenderers)
                        {
                            mr.material = ultraMat;
                        }
                        break;
                    default:
                        Debug.Log("Wrong name given to switch");
                        break;
                }

            }
        }
	}

    public void Initialize()
    {
        Subscribe();
    }

    private void GetActivatedFoodChild(TargetedFoodLayerChange t, GameObject go)
    {
        if(t == this)
            childGameObject = go;
    }

    public void Subscribe()
    {
        EventManager.OnTypeOfFoodChoosen += GetActivatedFoodChild;
    }

    public void Unsubscribe()
    {
        EventManager.OnTypeOfFoodChoosen -= GetActivatedFoodChild;
    }
}
