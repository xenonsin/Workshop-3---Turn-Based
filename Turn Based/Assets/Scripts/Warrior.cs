using UnityEngine;
using System.Collections;

public class Warrior : Entity
{

    public string name;
    public float maxhp = 1f;
    public float speed = 1f;
    public float damage = 1f;
    public int team = 1;

    private bool canEnd = true;

    // Use this for initialization
    public override void Awake()
    {
        Name = name;
        MaxHealth = maxhp;
        Damage = damage;
        Speed = speed;
        Team = team;

        base.Awake();

    }


    // Use this for initialization
    // Update is called once per frame
    public override void Update()
    {

        base.Update();
    }
}
