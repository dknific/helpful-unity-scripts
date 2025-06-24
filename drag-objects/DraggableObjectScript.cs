using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class DraggableObjectScript : MonoBehaviour
{
  public AudioClip[] gemHitSounds;
  private bool isBeingDragged = false;
  private Rigidbody rigidBody;
  private AudioSource hitSound;

  public void OnEnable()
  {
    EnhancedTouchSupport.Enable();
    UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += HandleObjectTouch;
    UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerMove += HandleObjectDrag;
    UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp += HandleObjectRelease;
  }

  public void Start()
  {
    rigidBody = GetComponent<Rigidbody>();
    hitSound = GetComponent<AudioSource>();
  }

  public void OnDisable()
  {
    UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= HandleObjectTouch;
    UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerMove -= HandleObjectDrag;
    UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerUp -= HandleObjectRelease;
    EnhancedTouchSupport.Disable();
  }

  public void OnCollisionEnter(Collision collision)
  {
    if (gemHitSounds.Length > 0)
    {
      int index = Random.Range(0, gemHitSounds.Length);
      hitSound.clip = gemHitSounds[index];
      hitSound.Play();
    }
  }

  public void HandleObjectTouch(Finger finger)
  {
    Ray ray = Camera.main.ScreenPointToRay(finger.currentTouch.screenPosition);

    if (Physics.Raycast(ray, out RaycastHit raycastHit))
    {
      if (raycastHit.transform.gameObject == gameObject)
      {
        isBeingDragged = true;
        rigidBody.linearVelocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
      }
    }
  }

  public void HandleObjectDrag(Finger finger)
  {
    if (isBeingDragged)
    {
      float zDepth = Mathf.Abs(Camera.main.transform.position.z);
      Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
        new Vector3(
          finger.currentTouch.screenPosition.x,
          finger.currentTouch.screenPosition.y,
          zDepth
        )
      );
      transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z);
    }
  }

  public void HandleObjectRelease(Finger finger)
  {
    if (isBeingDragged)
    {
      Vector2 myVector = finger.currentTouch.history[0].screenPosition - finger.currentTouch.history[2].screenPosition;
      Debug.Log(myVector);
      rigidBody.AddForce(new Vector3(myVector.x, myVector.y, 0), ForceMode.Impulse);
      isBeingDragged = false;
    }
  }
}
