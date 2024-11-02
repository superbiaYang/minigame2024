using UnityEngine;
using UnityEngine.UI;

public class BloodHud : MonoBehaviour
{
    public Slider m_BloodBar;

    public void Start()
    {
        if (m_BloodBar)
        {
            m_BloodBar.maxValue = 1;
            m_BloodBar.value = 1;
        }
    }

    public void UpdateBloodBar(int curHp, int maxHp)
    {
        if (maxHp == 0)
        {
            m_BloodBar.value = 1;
            return;
        }
        if (m_BloodBar != null)
        {
            m_BloodBar.value = (float)curHp / maxHp;
        }
    }
}
