using UnityEngine;
using Mirror;

public class CarController : NetworkBehaviour, IWeaponTargetable
{
    public Weapon m_Weapon;
    public CircularBulletController m_CircularBullet;
    public delegate void LocalPlayerSpawnedHandler(GameObject player);
    public static event LocalPlayerSpawnedHandler OnLocalPlayerSpawned;
    public int m_Type = 1;
    private int Mass
    {
        get {return (int)m_Rigidbody.mass;}
        set
        {
            m_Rigidbody.mass = value;
            m_CarModel.Mass = value;
        }
    }
    private int Speed
    { 
        get {return (int)m_Rigidbody.velocity.magnitude;}
    }
    private int m_CarDirection;
    public float KineticEnergy { get { return m_Rigidbody.velocity.sqrMagnitude * Mass / 2; } }
    
    private bool m_HasInited;

    private Rigidbody2D m_Rigidbody;
    private CarModel m_CarModel;
    private CarHudController m_HudController;
    private BloodHud m_BloodHud;
    private GameResult m_GameResult;
    private GameManager m_GameManager;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_CarModel = new CarModel();
        m_GameManager = FindObjectOfType<GameManager>();
        m_HasInited = false;
        if (!isServer)
        {
            m_HudController = FindObjectOfType<CarHudController>();
            m_BloodHud = FindObjectOfType<BloodHud>();
            m_GameResult = FindObjectOfType<GameResult>();
            m_GameResult.HideResult();
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        OnLocalPlayerSpawned?.Invoke(gameObject);
    }

    public void Init()
    {
        var config = Config.Instance.m_Vehicle.GetConfig(m_Type, m_CarModel.Level);
        if (config == null)
        {
            return;
        }
        m_CarModel.AutoRecover = config.m_AutoRecover;
        m_CarModel.Hp = config.m_Hp;
        m_CarModel.MaxHp = config.m_Hp;
        Mass = config.m_Mass;
        m_CarModel.Drag = config.m_Drag;
        m_CarModel.TurnSpeed = config.m_TurnSpeed;
        m_CarModel.Acceleration = config.m_Acceleration;
        m_CarModel.MaxSpeed = config.m_MaxSpeed;
        m_CarModel.DmgReduction = config.m_DmgReduction;
        
        m_HasInited = true;
        RpcInit();
    }

    [ClientRpc]
    private void RpcInit()
    {
        m_HasInited = true;
        UpdateHud();
    }

    private void UpdateProperty()
    {
        var config = Config.Instance.m_Vehicle.GetConfig(m_Type, m_CarModel.Level);
        if (config == null)
        {
            return;
        }
        m_CarModel.AutoRecover = config.m_AutoRecover;
        m_CarModel.MaxHp = config.m_Hp;
        m_CarModel.Drag = config.m_Drag;
        m_CarModel.TurnSpeed = config.m_TurnSpeed;
        m_CarModel.Acceleration = config.m_Acceleration;
        m_CarModel.MaxSpeed = config.m_MaxSpeed;
        m_CarModel.DmgReduction = config.m_DmgReduction;
        RpcUpdateCarModel(m_CarModel, Speed);
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (!m_HasInited)
        {
            return;
        }

        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        CmdMoveCar(moveInput, turnInput);
    }

    public Transform Transform()
    {
        return transform;
    }

    [Command]
    void CmdMoveCar(float moveInput, float turnInput)
    {
        if (m_GameManager.m_Status != GameStatus.Matching)
        {
            return;
        }
        
        if (moveInput != 0)
        {
            float currentSpeed = m_Rigidbody.velocity.magnitude;
            int direction = moveInput > 0 ? 1 : -1;
            
            if (m_CarDirection == 0 || m_CarDirection == direction)
            {
                currentSpeed += m_CarModel.Acceleration * Time.deltaTime;
            }
            else
            {
                currentSpeed -= m_CarModel.Acceleration * Time.deltaTime;
            }
            if (currentSpeed <= 0 || m_CarDirection == 0)
            {
                m_CarDirection = direction;
                m_Rigidbody.angularVelocity = 0;
            }
            currentSpeed = Mathf.Clamp(currentSpeed, 0, m_CarModel.MaxSpeed);

            Vector2 velocity = transform.up * currentSpeed * m_CarDirection;
            m_Rigidbody.velocity = velocity;
        }

        if (turnInput != 0)
        {
            if (moveInput < 0)
            {
                turnInput = -turnInput;
            }
            int turnDirection = turnInput > 0 ? 1 : -1;
            float torque = -turnDirection * m_CarModel.TurnSpeed;
            if (torque * m_Rigidbody.angularVelocity < 0)
            {
                m_Rigidbody.angularVelocity = 0;
            }
            m_Rigidbody.AddTorque(torque);
        }
        else
        {
            m_Rigidbody.angularVelocity = 0;
        }

        m_Rigidbody.AddForce(-m_CarModel.Drag * m_Rigidbody.velocity);
        Vector3 localVelocity = transform.InverseTransformDirection(m_Rigidbody.velocity);
        if (Mathf.Abs(localVelocity.x) > 0.1f)
        {
            Vector3 counterForce = transform.TransformDirection(new Vector3(-localVelocity.x, 0, 0));
            m_Rigidbody.AddForce(counterForce);
        }
        RpcUpdateCarModel(m_CarModel, Speed);
        RpcUpdateState(transform.position, transform.rotation);
    }

    [ClientRpc]
    void RpcUpdateState(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public void AddMass(int mass)
    {
        m_CarModel.Mass += mass;
        bool levelup = false;
        while (m_CarModel.Mass > Config.Instance.m_Vehicle.GetNextLevelRequiredMass(m_Type, m_CarModel.Level))
        {
            m_CarModel.Level++;
            var bullet = Instantiate(m_CircularBullet, transform.position, transform.rotation);
            bullet.m_Radius = GetComponent<Renderer>().bounds.size.y / 2 + 100;
            bullet.m_CenterPoint = transform;
            NetworkServer.Spawn(bullet.gameObject);
            levelup = true;
        }
        if (levelup)
        {
            m_Weapon.m_Level = m_CarModel.Level;
            UpdateProperty();
        }
    }

    public void TakeDamage(int damage)
    {
        m_CarModel.Hp -= damage;
        if (m_CarModel.Hp < 0)
        {
            m_CarModel.Hp = 0;
        }
        RpcUpdateCarModel(m_CarModel, Speed);
        if (m_CarModel.Hp == 0)
        {
            CarDestroyed();
        }
    }

    public bool IsAlive()
    {
        return m_CarModel.Hp > 0;
    }

    private void CarDestroyed()
    {
        gameObject.SetActive(false);
        RpcCarDestoryed();
    }

    [ClientRpc]
    private void RpcCarDestoryed()
    {
        gameObject.SetActive(false);
        m_GameResult.ShowResult(false);
    }

    [ClientRpc]
    public void RpcWin()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        gameObject.SetActive(false);
        m_GameResult.ShowResult(true);
    }

    [ClientRpc]
    private void RpcUpdateCarModel(CarModel car, int speed)
    {
        m_CarModel = car;
        m_CarModel.Speed = speed;
        UpdateHud();
    }

    private void UpdateHud()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (m_BloodHud)
        {
            m_BloodHud.UpdateBloodBar(m_CarModel.Hp, m_CarModel.MaxHp);
        }
        if (!m_HudController)
        {
            return;
        }
        m_HudController.Level = m_CarModel.Level;
        m_HudController.AutoRecover = m_CarModel.AutoRecover;
        m_HudController.Hp = m_CarModel.Hp;
        m_HudController.MaxHp = m_CarModel.MaxHp;
        m_HudController.Mass = m_CarModel.Mass;
        m_HudController.Drag = m_CarModel.Drag;
        m_HudController.TurnSpeed = m_CarModel.TurnSpeed;
        m_HudController.Acceleration = m_CarModel.Acceleration;
        m_HudController.Speed = m_CarModel.Speed;
        m_HudController.MaxSpeed = m_CarModel.MaxSpeed;
    }
}
