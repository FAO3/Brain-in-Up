using UnityEngine;
using UnityEngine.Events;

public class LiftCollision : MonoBehaviour
{
    public static UnityEvent QuestEvent = new UnityEvent();
    private void OnTriggerEnter(Collider wall)
    {
        if(wall.tag == "Wall")
        {
            QuestEvent?.Invoke();
        }
    }
    private void OnTriggerExit(Collider wall)
    {
        if(wall.gameObject.TryGetComponent(out Obstacles obstacles))
        {
            Destroy(obstacles.gameObject);
        }
    }
}
