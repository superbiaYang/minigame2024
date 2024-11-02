using UnityEngine;
using UnityEngine.UI;

public class BloodHud : MonoBehaviour
{
    public Slider m_BloodBar;

    void Update()
    {
        UpdateBloodBar(Random.Range(0,100), 100);
    }

    public void UpdateBloodBar(int curHp, int maxHp)
    {
        if (m_BloodBar != null)
        {
            m_BloodBar.value = (float)curHp / maxHp;
        }
    }
}
