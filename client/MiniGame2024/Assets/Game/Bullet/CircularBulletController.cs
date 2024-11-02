using UnityEngine;
using Mirror;

public class CircularBulletController : NetworkBehaviour
{
    public float m_Speed = 5f;
    public float m_Radius = 2f;
    public int m_Damage = 10;
    private float m_Angle = 0f;
    public Transform m_CenterPoint;

    void Update()
    {
        if (!isServer)
        {
            return;
        }
        m_Angle += m_Speed * Time.deltaTime;
        float x = Mathf.Cos(m_Angle) * m_Radius;
        float y = Mathf.Sin(m_Angle) * m_Radius;
        transform.position = new Vector3(m_CenterPoint.position.x + x, m_CenterPoint.position.y + y, m_CenterPoint.position.z);
        RpcUpdate(transform.position);
    }

    [ClientRpc]
    void RpcUpdate(Vector3 position)
    {
        transform.position = position;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer)
        {
            return;
        }
        GameObject colliderGo = other.gameObject;
        switch(colliderGo.tag)
        {
            case "Car":
                colliderGo.GetComponent<CarController>().TakeDamage(m_Damage);
                break;
            default:
                break;
        }
    }
}
