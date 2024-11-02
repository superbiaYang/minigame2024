using UnityEngine;
using UnityEngine.UI;

public class GameResult : MonoBehaviour
{
    public GameObject m_Canvas;
    public Text m_Text;

    public void HideResult()
    {
        m_Canvas.SetActive(false);
    }
    public void ShowResult(bool win)
    {
        m_Canvas.SetActive(true);
        m_Text.text = win ? "You Win" : "You Lose";
    }
}