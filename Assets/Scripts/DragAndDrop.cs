using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private InputAction mouseClick;
    [SerializeField] private float mousePhysicsSpeed;
    [SerializeField] private float rayDistance;

    private Camera _mainCamera;
    private WaitForFixedUpdate _waitForfixedupdate = new();

    private Coroutine _dragCoroutine;
    private Coroutine _spawnCoroutine;
    private bool _spawned = false;
    private bool _stopDrag = false;


    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        mouseClick.Enable();
        mouseClick.performed += MousePressed;
        mouseClick.canceled += MouseReleased;
        IngredientSpawner.OnIngredientSpawned += OnIngredientSpawned;
    }

    private void OnDisable()
    {
        mouseClick.Disable();
        mouseClick.performed -= MousePressed;
        mouseClick.canceled -= MouseReleased;
        IngredientSpawner.OnIngredientSpawned -= OnIngredientSpawned;
    }

    // Function that is called when the player clicks the left mouse button
    private void MousePressed(InputAction.CallbackContext context)
    {
        Debug.Log("Mouse pressed");
        _stopDrag = false;
        // Create a ray from the camera to the mouse position
        var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward * rayDistance, Color.red);
        //  If the raycast hits an object in the Interactable layer and the object has a rigidbody, start the drag coroutine
        if (Physics.Raycast(ray, out var hit, rayDistance, LayerMask.GetMask("Interactable")) && hit.collider != null)
            _dragCoroutine = StartCoroutine(DragUpdate(hit.collider.gameObject));
        // Otherwise, if the raycast hits an object in the Spawnable layer, start the spawn coroutine
        else if (!_spawned && Physics.Raycast(ray, out hit, rayDistance, LayerMask.GetMask("Spawnable")) &&
                 hit.collider != null)
            _spawnCoroutine = StartCoroutine(SpawnUpdate(hit.collider.gameObject));
        // If the raycast hits a Rat, kill it
        else if (Physics.Raycast(ray, out hit, rayDistance, LayerMask.GetMask("Rat")) && hit.collider != null)
            hit.collider.GetComponent<Ratatouille>().Kill();
        else
            Debug.Log("No object hit");
    }

    private void MouseReleased(InputAction.CallbackContext context)
    {
        Debug.Log("Mouse released");
        StopDrag();
    }


    private IEnumerator DragUpdate(GameObject clickedObject)
    {
        if (clickedObject == null) yield break;
        Debug.Log("Dragging: " + clickedObject.name);
        // Get the initial distance between the object and the camera
        var initialDistance = Vector3.Distance(clickedObject.transform.position, _mainCamera.transform.position);
        // Get the rigidbody of the object
        clickedObject.TryGetComponent<Rigidbody>(out var rb);
        // While the mouse is held down, move the object to the mouse position
        // Check if the button was pressed or released
        while (mouseClick.ReadValue<float>() != 0 && !_stopDrag)
        {
            var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            // If the object has a rigidbody, move it to the mouse position
            if (rb == null) continue;
            // Get the direction from the object to the mouse position
            var direction = ray.GetPoint(initialDistance) - clickedObject.transform.position;
            // Set the velocity of the rigidbody to the direction multiplied by the speed
            rb.velocity = direction * mousePhysicsSpeed;
            // Wait for the next fixed update
            yield return _waitForfixedupdate;
        }

        // Reset the stop drag bool
        _stopDrag = false;
    }

    private IEnumerator SpawnUpdate(GameObject spawner)
    {
        while (mouseClick.ReadValue<float>() != 0)
        {
            // Only spawn the ingredient once per click
            if (!_spawned)
            {
                Debug.Log("Spawned object from " + spawner.name);
                _spawned = true;
                // Calculate the position where the ingredient will be spawned in front of the player
                var spawnPosition = _mainCamera.transform.position + _mainCamera.transform.forward * 100f;
                // Spawn the ingredient on the server
                spawner.GetComponent<IngredientSpawner>().SpawnIngredient(spawnPosition);
            }

            yield return _waitForfixedupdate;
        }

        // On mouse release reset the spawned bool
        _spawned = false;
    }

    public void StopDrag()
    {
        Debug.Log("Stop drag called");
        // If the mouse is released, stop the drag coroutine
        if (_dragCoroutine != null) StopCoroutine(_dragCoroutine);
        // If the mouse is released, stop the spawn coroutine
        if (_spawnCoroutine != null) StopCoroutine(_spawnCoroutine);
        // Reset the spawned bool
        _spawned = false;
        // Set the stop drag bool
        _stopDrag = true;
    }

    private void OnIngredientSpawned(GameObject ingredient)
    {
        Debug.Log("On Ingredient spawned event triggered: " + ingredient.name);
        // Start the drag coroutine
        _dragCoroutine = StartCoroutine(DragUpdate(ingredient));
    }
}