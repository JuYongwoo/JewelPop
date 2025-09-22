using Unity.VisualScripting;
using UnityEngine;

public class JokerFX : MonoBehaviour
{
    private Transform goalTranform;

    [SerializeField]
    private float moveSpeed = 3f; //±âº» °ª 3

    void Start()
    {
        goalTranform = ManagerObject.instance.actionManager.getJokerGoalTranform();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveSpeed * Time.deltaTime * (new Vector3(goalTranform.position.x, goalTranform.position.y, transform.position.z) - transform.position).normalized;

        if ((new Vector2(transform.position.x, transform.position.y) -  new Vector2(goalTranform.position.x, goalTranform.position.y)).sqrMagnitude <= 1f)
        {
            Destroy(gameObject);

        }
    }
}
