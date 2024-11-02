using UnityEngine;
using System;
using System.Collections;
using Mirror;
public class Weapon : NetworkBehaviour
{
    public BulletController m_Bullet;
    public int m_Damage = 10;
    public int m_FireIntervalSecs = 2;
    public int m_Level = 1;
    private TimeSpan m_FireInterval;
    private DateTime m_LastFireTime = DateTime.MinValue;

    private GameObject m_TargetGo;
    private IWeaponTargetable m_Target;

    void Awake()
    {
        m_FireInterval = new TimeSpan(0, 0, m_FireIntervalSecs);
    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }
        Fire();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isServer)
        {
            return;
        }
        if (m_TargetGo != null)
        {
            return;
        }
        GameObject colliderGo = collider.gameObject;
        switch(colliderGo.tag)
        {
            case "Car":
                m_TargetGo = colliderGo;
                m_Target = m_TargetGo.GetComponent<CarController>();
                break;
            default:
                break;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (!isServer)
        {
            return;
        }
        GameObject colliderGo = collider.gameObject;
        if (m_TargetGo == colliderGo)
        {
            m_TargetGo = null;
            m_Target = null;
        }
    }

    private void Fire()
    {
        if (m_Target == null)
        {
            return;
        }
        DateTime currentTime = DateTime.Now;
        if (currentTime - m_LastFireTime >= m_FireInterval)
        {
            m_LastFireTime = currentTime;
            for (int i = 0; i < m_Level; i++) 
            {
                StartCoroutine(FireBullet(0.1f * i, m_Target));
            }
        }

        IEnumerator FireBullet(float secs, IWeaponTargetable target)
        {
            yield return new WaitForSeconds(secs);
            var bullet = Instantiate(m_Bullet, transform.position, transform.rotation);
            bullet.Target = target;
            bullet.Damage = m_Damage;
            NetworkServer.Spawn(bullet.gameObject);
        }
    }
}