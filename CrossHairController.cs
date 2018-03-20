using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class CrossHairController : MonoBehaviour
{
    [SerializeField]
    int playerNumber;
    [SerializeField]
    RectTransform crossHair;
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    Camera playerCamera;

    public Transform playerInCrossHairs;
    Vector2 offSetReset = new Vector2( 0f, 0f );
	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    void FixedUpdate()
    {
        if (gameManager.GameState == "Shooting")
        {
            HandleInput();
        }
        else
        {
            crossHair.offsetMin = offSetReset;
            crossHair.offsetMax = offSetReset;
        }
            
    }

    void HandleInput()
    {
        float horizontalInput = CrossPlatformInputManager.GetAxis("Xaxis" + playerNumber);
        float verticalInput = CrossPlatformInputManager.GetAxis("Yaxis" + playerNumber);

        float horizontalPosition = crossHair.position.x;
        float verticalPosition = crossHair.position.y;

        float speed = 7f;
        Debug.Log(crossHair.position.x);

        crossHair.position = new Vector3(horizontalPosition + (horizontalInput * speed), verticalPosition + (verticalInput * speed), 0f);
        

        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(crossHair.position);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Player")
            {
                playerInCrossHairs = hit.transform;
                Debug.Log(playerInCrossHairs.name);
                //Debug.Log("Parent " + playerInCrossHairs.Health);
                //Debug.Log("Parent " + playerInCrossHairs.PlayerController);
            }
        }
    }
}
