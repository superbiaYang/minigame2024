using UnityEngine;
using Mirror;
public class Resource : NetworkBehaviour
{
    public int m_Type;
    private int m_DestoryEnergy;
    private int m_Coin;
    private int m_Mass;

    void Awake()
    {
        var config = Config.Instance.m_Resource.GetConfig(m_Type);
        if (config == null)
        {
            Debug.LogError("Invalid type " + m_Type);
            return;
        }
        m_DestoryEnergy = config.m_DestoryEnergy;
        m_Coin = config.m_Coin;
        m_Mass = config.m_Mass;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!isServer)
        {
            return;
        }
        GameObject colliderGo = other.gameObject;
        switch(colliderGo.tag)
        {
            case "Car":
                OnCollided(colliderGo.GetComponent<CarController>());
                break;
            default:
                break;
        }
    }

    void OnCollided(CarController car)
    {
        if (car.KineticEnergy < m_DestoryEnergy)
        {
            return;
        }
        car.AddMass(m_Mass);
        Disapear();
    }

    private void Disapear()
    {
        gameObject.SetActive(false);
        RpcDisapear();
    }

    [ClientRpc]
    private void RpcDisapear()
    {
        gameObject.SetActive(false);
    }
}
