using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed,
        _maxOffset,
        _destroyTime
        ;

    [SerializeField] private List<Vector3> _spawnPos;

    private Vector3 currentSpawnPos;
    private int spawnIndex;


    private bool hasGameFinished;
    private bool isLeft;
    private bool canSwitch;

    private void Start()
    {
        hasGameFinished = false;

        spawnIndex = Random.Range(0, _spawnPos.Count);
        currentSpawnPos = _spawnPos[spawnIndex];
        isLeft = Random.Range(0,2) == 0;
        currentSpawnPos.x *= isLeft ? -1f : 1f;
        transform.position = currentSpawnPos;
        canSwitch = Random.Range(0, 4) == 0 && spawnIndex != 0;
        
        
        if(canSwitch)
        {
            StartCoroutine(MoveToOppositeDirection());
        }
        
    }

    private void OnEnable()
    {
        GameManager.Instance.GameEnded += OnGameEnded;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameEnded -= OnGameEnded;
    }


    private void FixedUpdate()
    {
        if (hasGameFinished) return;

        transform.position += _moveSpeed * Time.fixedDeltaTime * Vector3.up;
        
        if(transform.position.y > _maxOffset)
        {
            Destroy(gameObject);
        }
    }

    public void OnGameEnded()
    {
        GetComponent<Collider2D>().enabled = false;
        hasGameFinished = true;
        StartCoroutine(Rescale());
    }

    private IEnumerator Rescale()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        Vector3 scaleOffset = endScale - startScale;
        float timeElapsed = 0f;
        float speed = 1 / _destroyTime;
        var updateTime = new WaitForFixedUpdate();
        while(timeElapsed < 1f)
        {
            timeElapsed += speed * Time.fixedDeltaTime;
            transform.localScale = startScale + timeElapsed * scaleOffset;
            yield return updateTime;
        }

        Destroy(gameObject);
    }

    [SerializeField] private float _startSwitchPosY, _endSwitchPosY;

    private IEnumerator MoveToOppositeDirection()
    {
        float currentSwitchPosY = _startSwitchPosY + (_endSwitchPosY - _startSwitchPosY) * 0.25f * Random.Range(0, 5);

        float currentPosY = transform.position.y;
        float offsetY = currentSwitchPosY - currentPosY;
        float waitTime = offsetY / _moveSpeed;
        yield return new WaitForSeconds(waitTime);

        float currentSwitchPosX = transform.position.x;
        float targetSwitchPosX = -currentSwitchPosX;
        float offsetX = targetSwitchPosX - currentSwitchPosX;
        float timeToSwitch = Mathf.Abs(offsetX / _moveSpeed);
        float speedMagnitude = offsetX > 0f ? 1f : -1f;

        float timeElapsed = 0f;
        while(timeElapsed < timeToSwitch)
        {
            timeElapsed += Time.fixedDeltaTime;
            transform.position += speedMagnitude * _moveSpeed * Time.fixedDeltaTime * Vector3.right;
            yield return new WaitForFixedUpdate();
        }

        Vector3 temp = transform.position;
        temp.x = targetSwitchPosX;
        transform.position = temp;

    }
}
