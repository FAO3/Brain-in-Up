using UnityEngine;
using UnityEngine.Events;

public class MoveBack : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private SpriteRenderer _sprite;
    
    private float _positionMinY;
    private Vector2 _restartPosition;

    private void Awake()
    {
        _restartPosition = transform.position;
        _positionMinY = _sprite.bounds.size.y * 2 - _restartPosition.y;
    }
    private void Start()
    {
        LiftCollision.QuestEvent.AddListener(Stop);
        QuizGameUI.ButtonClick.AddListener(Go);
    }
    private void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if(transform.position.y <= -_positionMinY)
        {
            transform.position = _restartPosition;
        }
    }
    private void Stop()
    {
        _speed = 0;
    }
    private void Go()
    {
        _speed = 2;
    }
}
