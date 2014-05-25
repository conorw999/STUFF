using UnityEngine;

using System.Collections;



[System.Serializable]

public class ProjectileInfo
	
{
	
	public GameObject owner;
	
	public Damage damage = new Damage();
	
	public float force;
	
	public int maxPenetration;
	
	public float maxSpread;
	
	public float spread;
	
	public float speed;
	
	public bool usePool;
	
	public float projectileLifeTime;
	
}



[System.Serializable]

public class DamageType
	
{
	
	public int amount; // how much damage
	
	public DamageType type; //what type of damage
	
}



[System.Serializable]

public class Resistance
	
{
	
	public float normal;
	
	public float fire;
	
	public float ice;
	
	public float acid;
	
	public float electric;
	
	public float poison;
	
}