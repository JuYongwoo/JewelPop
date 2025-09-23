using Unity.VisualScripting;
using UnityEngine;

public class JokerFX : MonoBehaviour
{
    private Transform goalTranform;

    [SerializeField]
    private float moveSpeed = 5f; // 기본 값 5

    // 베지어 포물선 이동용
    private Vector3 startPos;
    private Vector3 controlPos;
    private Vector3 goalPos;
    private float t = 0f;
    private float approxLen = 1f;
    private bool isLeft;

    void Start()
    {
        goalTranform = ManagerObject.instance.actionManager.getJokerGoalTranform();

        // 시작/목표 (z는 현재 오브젝트 z 유지)
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        goalPos = new Vector3(goalTranform.position.x, goalTranform.position.y, transform.position.z);

        // 좌/우 랜덤
        isLeft = Random.value < 0.5f;
        float sideSign = isLeft ? -1f : 1f;

        // 제어점: 좌/우로 벌리고 위로 들어올려 포물선 느낌
        Vector3 dir = goalPos - startPos;
        float dist = dir.magnitude;
        Vector3 mid = (startPos + goalPos) * 0.5f;
        Vector3 perp = new Vector3(-dir.y, dir.x, 0f).normalized;

        controlPos = mid
                   + perp * (sideSign * 0.45f * dist)
                   + Vector3.up * (0.25f * dist);
        controlPos.z = transform.position.z;

        // 곡선 길이 근사(속도 → t 로 변환)
        approxLen = (startPos - controlPos).magnitude + (controlPos - goalPos).magnitude;
        if (approxLen < 0.001f) approxLen = Mathf.Max(dist, 1f);
    }

    void Update()
    {
        // t(0→1)을 곡선 길이에 따라 증가시켜 대략 일정 속도로 이동
        float dt = (moveSpeed * Time.deltaTime) / approxLen;
        t = Mathf.Clamp01(t + dt);

        // 이차 베지어 보간: B(t) = (1-t)^2 P0 + 2(1-t)t P1 + t^2 P2
        float omt = 1f - t;
        Vector3 pos = (omt * omt) * startPos + 2f * omt * t * controlPos + (t * t) * goalPos;
        transform.position = pos;

        if (t >= 1f)
        {
            AudioSource.PlayClipAtPoint(ManagerObject.instance.resourceManager.gamsSFXPrefabs[SFX.ScoreGetSFX], transform.position);
            Destroy(gameObject);
        }
    }
}
