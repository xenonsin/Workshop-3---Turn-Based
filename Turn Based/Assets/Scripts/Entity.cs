using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Entity : MonoBehaviour
{
    public delegate void Action();
    public static event Action EndedTurn;
    public static event Action HpChange;
    public static event Action Dead;

    public string Name { get; set; }
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public float Damage{ get; set; }
    public float Speed { get; set; }
    public bool IsTurn { get; set; }
    public int Team { get; set; }

    public bool IsDead { get; set; }

    private BattleManager _battleManager;

    public virtual void Hit(float damage)
    {
        Health -= damage;
        Debug.Log(Name + " was hit for " + damage + " damage!");
        Debug.Log(Name + " Health: " + Health.ToString());

        if (HpChange != null)
            HpChange();
    }

    public virtual void Heal(float heal)
    {
        Health += heal;
        Debug.Log(Name + " was healed for " + heal + " hp!");
        Debug.Log(Name + " Health: " + Health.ToString());

        if (HpChange != null)
            HpChange();
    }

    public virtual void Death()
    {
        if (!IsDead)
        {
            Debug.Log(Name + " has died!");
            IsDead = true;
            if (Dead != null)
                Dead();

            if(IsTurn)
                EndTurn();
        }
        //Destroy(gameObject);
    }

    public virtual void EndTurn()
    {
        Debug.Log(Name + " has ended the turn.");
        IsTurn = false;
        if (EndedTurn!= null)
            EndedTurn();
     
        
    }

    public virtual void Awake()
    {
        IsDead = false;
        Health = MaxHealth;
        IsTurn = false;
        _battleManager = GameObject.FindGameObjectWithTag("Battle Manager").GetComponent<BattleManager>();
    }

    public virtual void Update()
    {
        if (Health <= 0)
            Death();

        //if (IsTurn)
        //{
        //    if (Input.GetKeyDown(KeyCode.UpArrow))
        //        EndTurn();

        //    if (Input.GetKeyDown(KeyCode.DownArrow))
        //        Hit(1f);
        //}
    }

    public virtual void OnEnable()
    {
        BattleManager.NewTurn += OnTurn;

    }

    public virtual void OnDisable()
    {
        BattleManager.NewTurn -= OnTurn;
    }

    public virtual void OnTurn()
    {
        if (_battleManager.CurrentTurn() == this)
        {
            if(IsDead)
                EndTurn();
            else                        
                Debug.Log(Name + "'s turn.");
        }

    }



}
	
