using UnityEngine;
using Mirror;

public class BulletController : NetworkBehaviour
{
    private IWeaponTargetable m_Target;
    public IWeaponTargetable Target { set {m_Target = value;} }
    private int m_Damage;
    public int Damage { set {m_Damage = value;} }
    public float m_Speed;

    private Rigidbody2D m_Rigidbody;
    private bool m_IsStopped = false;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (m_IsStopped || m_Target == null) return;
        
        var targetPosition = m_Target.Transform().position;
        Vector2 direction = (targetPosition - transform.position).normalized;
        m_Rigidbody.velocity = direction * m_Speed;
        RpcUpdate(targetPosition);
    }

    [ClientRpc]
    void RpcUpdate(Vector3 targetPosition)
    {
        if (m_Rigidbody == null)
        {
            return;
        }
        Vector2 direction = (targetPosition - transform.position).normalized;
        m_Rigidbody.velocity = direction * m_Speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer)
        {
            return;
        }
        if (other.transform == m_Target.Transform())
        {
            m_IsStopped = true;
            m_Rigidbody.velocity = Vector2.zero;
            m_Rigidbody.angularVelocity = 0f;
            m_Target.TakeDamage(m_Damage);
            Destroy(gameObject);
        }
    }
}
