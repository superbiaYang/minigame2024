using UnityEngine;
using System;
using System.Collections.Generic;
using Mirror;

public class SafeZoneController : NetworkBehaviour
{
    public float m_ShrinkingTime;
    public int m_Damage = 10;
    public int m_DamageIntervalSecs = 2;
    private TimeSpan m_DamageInterval;
    private DateTime m_LastDamageTime = DateTime.MinValue;
    private Material m_Material;
    private float m_Radius = 1f;
    [SyncVar]private bool m_IsShrinking;
    
    void Awake()
    {
        m_Material = GetComponent<SpriteRenderer>().material;
        m_DamageInterval = new TimeSpan(0, 0, m_DamageIntervalSecs);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Init();
    }

    public void Init()
    {
        m_Radius = 1f;
        m_Material.SetFloat("_Radius", m_Radius);
        m_IsShrinking = false;
    }

    void Update()
    {
        if (!m_IsShrinking)
        {
            return;
        }
        if (m_Radius <= 0)
        {
            return;
        }
        m_Radius = m_Radius - Time.deltaTime / m_ShrinkingTime;
        if (m_Radius < 0) m_Radius = 0;
        m_Material.SetFloat("_Radius", m_Radius);
    }

    public void StartShrinking()
    {
        m_IsShrinking = true;
    }

    public void CheckDamage(List<CarController> cars)
    {
        DateTime currentTime = DateTime.Now;
        if (currentTime - m_LastDamageTime < m_DamageInterval)
        {
            return;
        }
        m_LastDamageTime = currentTime;
        foreach (CarController car in cars)
        {
            var distance = Vector2.Distance(car.transform.position, transform.position);
            if (distance > m_Radius * transform.localScale.x)
            {
                car.TakeDamage(m_Damage);
            }
        }
    }
}
