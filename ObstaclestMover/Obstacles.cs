using UnityEngine;
using UnityEngine.Events;

public class Obstacles : MonoBehaviour
{
    [SerializeField] private float _speed;
    public static UnityEvent SpeedEventObstacles = new UnityEvent();

    private void Start()
    {
        LiftCollision.QuestEvent.AddListener(Stop);
        QuizGameUI.ButtonClick.AddListener(Go);
    }
    private void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }
    private void Stop()
    {
        _speed = 0;
    }
    private void Go()
    {
        int value = Random.Range(2, 8);
        _speed = value;
    }
}
