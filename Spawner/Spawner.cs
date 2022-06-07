using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _wall;
    [SerializeField] private Transform[] _spawnPoints;
    private int randomize;
    private IEnumerator coroutine;


    private void Start()
    {
        float value = 5f;
        coroutine = WaitAndSpawn(value);
        StartCoroutine(coroutine);
        QuizGameUI.ButtonClick.AddListener(goSpawn);
        QuizGameUI.AwakeTimer.AddListener(stopSpawn);
    }
    private void goSpawn()
    {
        StartCoroutine(coroutine);
    }
    private void stopSpawn()
    {
        StopCoroutine(coroutine);
    }

    private IEnumerator WaitAndSpawn(float waitTime)
    {
        while (waitTime != 0)
        {
            yield return new WaitForSeconds(waitTime);
            SpawnElements(_wall, _spawnPoints);
        }
    }

    private void SpawnElements(GameObject wall, Transform[] spawnPoints)
    {
        randomize = Random.Range(0, _spawnPoints.Length);
        Instantiate(wall, spawnPoints[randomize].position, Quaternion.identity);
    }
}
