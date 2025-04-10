using UnityEngine;

public class BallThrower : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float upwardForce = 5f;
    [SerializeField] private float resetDelay = 3f;
    [SerializeField] private Camera mainCamera;

    [Header("Spawn Point")]
    [SerializeField] private Transform resetSpawnPoint;

    private Vector3 startPos;
    private Vector3 endPos;
    private Quaternion initialRotation;
    private Rigidbody rb;
    private bool isThrown = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialRotation = transform.rotation;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (resetSpawnPoint == null)
            Debug.LogWarning("⚠️ BallThrower: Reset spawn point not assigned!");
    }

    private void Update()
    {
        if (isThrown) return;

        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endPos = Input.mousePosition;
            ThrowBall();
        }
    }

    private void ThrowBall()
    {
        Vector3 swipeDelta = endPos - startPos;

        Vector3 direction = new Vector3(swipeDelta.x, 0f, swipeDelta.y).normalized;
        Vector3 worldDirection = mainCamera.transform.TransformDirection(direction);
        worldDirection.y = upwardForce;

        rb.useGravity = true;
        rb.AddForce(worldDirection * throwForce, ForceMode.Impulse);

        Debug.DrawRay(transform.position, worldDirection * throwForce, Color.red, 2f);

        isThrown = true;
        Invoke(nameof(ResetBall), resetDelay);
    }

    private void ResetBall()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.Sleep(); // Pause physics calculations

        if (resetSpawnPoint != null)
        {
            float randomY = Random.Range(2f, 6f);
            Vector3 resetPosition = resetSpawnPoint.position;
            resetPosition.y = randomY;

            rb.position = resetPosition; // Use Rigidbody's position directly
            rb.rotation = initialRotation; // Optional: use this instead of transform.rotation
        }
        else
        {
            Debug.LogWarning("⚠️ Cannot reset ball position: Reset spawn point not assigned.");
        }

        isThrown = false;
        rb.WakeUp(); // Reactivate physics
    }


}
