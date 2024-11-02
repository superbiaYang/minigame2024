using UnityEngine;
using TMPro;

public class CarHudController : MonoBehaviour
{
    public TMP_Text m_Text;

    private int m_MaxHp;
    public int MaxHp
    {
        get => m_MaxHp;
        set
        {
            m_MaxHp = value;
            UpdateHud();
        }
    }

    private int m_Mass;
    public int Mass
    {
        get => m_Mass;
        set
        {
            m_Mass = value;
            UpdateHud();
        }
    }

    private int m_Acceleration;
    public int Acceleration
    {
        get => m_Acceleration;
        set
        {
            m_Acceleration = value;
            UpdateHud();
        }
    }

    private int m_MaxSpeed;
    public int MaxSpeed
    {
        get => m_MaxSpeed;
        set
        {
            m_MaxSpeed = value;
            UpdateHud();
        }
    }

    private int m_Hp;
    public int Hp
    {
        get => m_Hp;
        set
        {
            m_Hp = value;
            UpdateHud();
        }
    }

    private int m_Level;
    public int Level
    {
        get => m_Level;
        set
        {
            m_Level = value;
            UpdateHud();
        }
    }

    private int m_Speed;
    public int Speed
    {
        get => m_Speed;
        set
        {
            m_Speed = value;
            UpdateHud();
        }
    }

    private float m_Drag;
    public float Drag
    {
        get => m_Drag;
        set
        {
            m_Drag = value;
            UpdateHud();
        }
    }

    private int m_TurnSpeed;
    public int TurnSpeed
    {
        get => m_TurnSpeed;
        set
        {
            m_TurnSpeed = value;
            UpdateHud();
        }
    }

    private int m_AutoRecover;
    public int AutoRecover
    {
        get => m_AutoRecover;
        set
        {
            m_AutoRecover = value;
            UpdateHud();
        }
    }

    private void UpdateHud()
    {
        m_Text.text = 
        $"当前等级：{Level}\n" +
        $"当前质量：{Mass}\n" +
        $"当前时速：{Speed} / {MaxSpeed}\n" +
        $"当前血量：{Hp} / {MaxHp}\n" +
        $"加速度：{Acceleration}\n" +
        $"摩擦系数：{Drag}\n" +
        $"转向速度：{TurnSpeed}\n" +
        $"自动恢复：{AutoRecover}\n";
    }
}
