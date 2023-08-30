using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Firehose : SelectedWeapon
{
    [SerializeField] private float yOffset;
    private GameObject _objectToRotate;
    public float rotSpeed = 0.36f;

    Vector3 _pointA;
    Vector3 _pointB;
    bool _rotating = false;


    protected override void Start()
    {
        _pointA = transform.eulerAngles + new Vector3(0f, 0, 90);

        _pointB = transform.eulerAngles + new Vector3(0f, 0, -90);
        transform.eulerAngles = new Vector3(0, 0, 45);

        _objectToRotate = gameObject;
        base.Start();
    }

    private void FixedUpdate()
    {
        var playerPos = PlayerController.Instance.CurrentPlayerTransform().position;
        transform.position = new Vector3(playerPos.x, playerPos.y + yOffset, playerPos.z);
    }

    protected override void Activate()
    {
        StartCoroutine(Rotate());
        SpawnObject();
        base.Activate();
    }

    private void SpawnObject()
    {
        var go = Instantiate(instantiatedObject);
        go.TryGetComponent(out FireHoseBullet fireHoseBullet);
        fireHoseBullet.Damage = damage;
        go.transform.position = transform.position;
        go.transform.eulerAngles = transform.eulerAngles;
        go.transform.localScale = Vector3.one * Random.Range(1f, 2f);
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            //Rotate 90
            yield return RotateObject(_objectToRotate, _pointA, 3f);
            //Rotate -90
            yield return RotateObject(_objectToRotate, _pointB, 3f);
        }
    }

    private IEnumerator RotateObject(GameObject gameObjectToMove, Vector3 eulerAngles, float duration)
    {
        if (_rotating)
        {
            yield break;
        }
        
        _rotating = true;

        var newRot = gameObjectToMove.transform.eulerAngles + eulerAngles;

        var currentRot = gameObjectToMove.transform.eulerAngles;

        var counter = 0.0f;
        while (counter < duration)
        {
            counter += Time.deltaTime * rotSpeed;
            gameObjectToMove.transform.eulerAngles = Vector3.Lerp(currentRot, newRot, counter / duration);
            yield return null;
        }
        
        _rotating = false;
    }
}
