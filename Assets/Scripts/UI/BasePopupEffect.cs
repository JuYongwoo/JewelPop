using UnityEngine;

public class BasePopupEffect : MonoBehaviour
{
    [Header("Popup Effect Settings")]
    public float speed = 10f;        // ������ ��ȭ �ӵ�
    public float overshoot = 1.2f;   // Ƣ��� �� ����
    protected Vector3 targetScale = Vector3.one;
    protected bool playing = false;
    protected bool overshootPhase = true;

    protected virtual void OnEnable()
    {
        transform.localScale = Vector3.zero;
        playing = true;
        overshootPhase = true;
    }

    protected virtual void Update()
    {
        if (!playing) return;

        if (overshootPhase)
        {
            // Overshoot �ܰ� (1.2�����)
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                Vector3.one * overshoot,
                Time.deltaTime * speed
            );

            if (transform.localScale.x >= overshoot - 0.01f)
            {
                overshootPhase = false;
            }
        }
        else
        {
            // ���� ũ��(1��)�� ����
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * speed
            );

            if (Vector3.Distance(transform.localScale, targetScale) < 0.01f)
            {
                transform.localScale = targetScale;
                playing = false;
            }
        }
    }
}
