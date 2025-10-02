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
        AppManager.instance.soundManager.PlaySound(Sounds.ScoreGetSFX, 0.2f, false);

        goalTranform = AppManager.instance.actionManager.getJokerGoalTranformM();

        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        goalPos = new Vector3(goalTranform.position.x, goalTranform.position.y, transform.position.z);

        isLeft = Random.value < 0.5f;
        float sideSign = isLeft ? -1f : 1f;

        Vector3 dir = goalPos - startPos;
        float dist = dir.magnitude;
        Vector3 mid = (startPos + goalPos) * 0.5f;
        Vector3 perp = new Vector3(-dir.y, dir.x, 0f).normalized;

        controlPos = mid
                   + perp * (sideSign * 0.45f * dist)
                   + Vector3.up * (0.25f * dist);
        controlPos.z = transform.position.z;

        approxLen = (startPos - controlPos).magnitude + (controlPos - goalPos).magnitude;
        if (approxLen < 0.001f) approxLen = Mathf.Max(dist, 1f);
    }

    void Update()
    {
        float dt = (moveSpeed * Time.deltaTime) / approxLen;
        t = Mathf.Clamp01(t + dt);

        float omt = 1f - t;
        Vector3 pos = (omt * omt) * startPos + 2f * omt * t * controlPos + (t * t) * goalPos;
        transform.position = pos;

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
