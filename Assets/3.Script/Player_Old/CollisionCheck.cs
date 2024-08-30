using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    private PlayerManager_Old playerManager;

    // 장애물이 앞쪽에 있을 경우
    private bool isObstacleFrontPlayer;
    public bool GetIsObstacleFrontPlayer() { return isObstacleFrontPlayer; }


    // 바닥이 있는지 없는지 확인
    private bool isFloorExist;
    public bool GetIsFloorExist() { return isFloorExist; }



    //TODO: 장애물 색상 표시

    public Vector3 Playerpos { get; private set; }
    private Vector3 obstaclepos;

    private void Awake() {
        playerManager = GetComponentInParent<PlayerManager_Old>();
        Playerpos = transform.position;
    }
    private void Update() {
        Playerpos = transform.position;
    }

    private void OnCollisionEnter(Collision collision) {

        obstaclepos = collision.transform.parent != null ? collision.transform.parent.position : collision.transform.position;

        Vector3 playerToObstacle = obstaclepos - Playerpos;

        Vector3 directionFront = transform.InverseTransformDirection(playerToObstacle);
        float d = Vector3.Dot(directionFront, Vector3.forward);

        if (d >= 0) {   // 장애물이 플레이어 앞쪽에 있을 경우 

            //TODO: 장애물 bool 넘겨야함
            isObstacleFrontPlayer = true;

            float midpointY = transform.position.y;

            float collisionY = collision.transform.position.y;

            //if (collisionY > midpointY) {
            //    TopcolObject = collision.transform.parent != null ? collision.transform.parent.gameObject : collision.gameObject;
            //    Debug.Log("Object hit from above: " + TopcolObject.name);
            //}
            //else {
            //    BottomcolObject = collision.transform.parent != null ? collision.transform.parent.gameObject : collision.gameObject;
            //    Debug.Log("Object hit from below: " + BottomcolObject.name);
            //}
        }
    }

    // 바닥 확인
    private void OnCollisionStay(Collision collision) {
        obstaclepos = collision.transform.parent != null ? collision.transform.parent.position : collision.transform.position;

        Vector3 playerToObstacle = obstaclepos - Playerpos;

        Vector3 directionFront = transform.InverseTransformDirection(playerToObstacle);
        float d = Vector3.Dot(directionFront, Vector3.forward);

        if (d >= 0) {   // 장애물이 플레이어 앞쪽에 있을 경우 

            //TODO: 장애물 bool 넘겨야함
            isObstacleFrontPlayer = true;



            float midpointY = transform.position.y;

            float collisionY = collision.transform.position.y;

            //if (collisionY > midpointY) {
            //    TopcolObject = collision.transform.parent != null ? collision.transform.parent.gameObject : collision.gameObject;
            //    Debug.Log("Object hit from above: " + TopcolObject.name);
            //}
            //else {
            //    BottomcolObject = collision.transform.parent != null ? collision.transform.parent.gameObject : collision.gameObject;
            //    Debug.Log("Object hit from below: " + BottomcolObject.name);
            //}
        }


        Vector3 directionBottom = transform.InverseTransformDirection(playerToObstacle);
        float b = Vector3.Dot(directionBottom, Vector3.down);
        if (b >= 0) {       // 바닥 오브젝트가 있다면
            Debug.Log("Object stay from bottom: " + collision.transform.parent.position);
            isFloorExist = true;
        }
    }


    private void OnCollisionExit(Collision collision) {

        obstaclepos = collision.transform.parent != null ? collision.transform.parent.position : collision.transform.position;

        Vector3 playerToObstacle = obstaclepos - Playerpos;

        Vector3 directionFront = transform.InverseTransformDirection(playerToObstacle);
        float d = Vector3.Dot(directionFront, Vector3.forward);

        if (d >= 0) {   // 장애물이 플레이어 앞쪽에 있을 경우 
            isObstacleFrontPlayer = false;

            float midpointY = transform.position.y;

            float collisionY = collision.transform.position.y;

            //if (collisionY > midpointY) {
            //    Debug.Log("Object out from above: " + TopcolObject.name);
            //    TopcolObject = null;
            //}
            //else {
            //    Debug.Log("Object out from below: " + BottomcolObject.name);
            //    BottomcolObject = null;
            //}
        }


        Vector3 directionBottom = transform.InverseTransformDirection(playerToObstacle);
        float b = Vector3.Dot(directionBottom, Vector3.down);
        if (b >= 0) {       // 바닥 오브젝트가 없다면?
            Debug.Log("Object out from bottom: " + collision.transform.parent.position);
            isFloorExist = false;
        }
    }

}

/*
 1. 위 아래 오브젝트 있을 경우 
각자 담음
 
 
 */