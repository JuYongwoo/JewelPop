using Unity.VisualScripting;
using UnityEngine;

public class JokerFX : MonoBehaviour
{
    private Transform goalTranform;

    [SerializeField]
    private float moveSpeed = 5f; // �⺻ �� 5

    // ������ ������ �̵���
    private Vector3 startPos;
    private Vector3 controlPos;
    private Vector3 goalPos;
    private float t = 0f;
    private float approxLen = 1f;
    private bool isLeft;

    void Start()
    {
        goalTranform = ManagerObject.instance.actionManager.getJokerGoalTranform();

        // ����/��ǥ (z�� ���� ������Ʈ z ����)
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        goalPos = new Vector3(goalTranform.position.x, goalTranform.position.y, transform.position.z);

        // ��/�� ����
        isLeft = Random.value < 0.5f;
        float sideSign = isLeft ? -1f : 1f;

        // ������: ��/��� ������ ���� ���÷� ������ ����
        Vector3 dir = goalPos - startPos;
        float dist = dir.magnitude;
        Vector3 mid = (startPos + goalPos) * 0.5f;
        Vector3 perp = new Vector3(-dir.y, dir.x, 0f).normalized;

        controlPos = mid
                   + perp * (sideSign * 0.45f * dist)
                   + Vector3.up * (0.25f * dist);
        controlPos.z = transform.position.z;

        // � ���� �ٻ�(�ӵ� �� t �� ��ȯ)
        approxLen = (startPos - controlPos).magnitude + (controlPos - goalPos).magnitude;
        if (approxLen < 0.001f) approxLen = Mathf.Max(dist, 1f);
    }

    void Update()
    {
        // t(0��1)�� � ���̿� ���� �������� �뷫 ���� �ӵ��� �̵�
        float dt = (moveSpeed * Time.deltaTime) / approxLen;
        t = Mathf.Clamp01(t + dt);

        // ���� ������ ����: B(t) = (1-t)^2 P0 + 2(1-t)t P1 + t^2 P2
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
