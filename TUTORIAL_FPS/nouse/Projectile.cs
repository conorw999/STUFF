using UnityEngine;

using System.Collections;



public class Projectile : MonoBehaviour
	
{
	
	private ProjectileInfo myInfo = new ProjectileInfo();
	
	private Rigidbody myRigid;
	
	private Vector3 velocity;
	
	private int hitCount = 0;
	
	
	
	public void SetUp(ProjectileInfo info)
		
	{
		
		myInfo = info;
		
		myRigid = rigidbody;
		
		hitCount = 0;
		
		velocity = myInfo.speed * transform.forward + transform.TransformDirection(Random.Range(-myInfo.maxSpread, myInfo.maxSpread) * myInfo.spread, Random.Range(-myInfo.maxSpread, myInfo.maxSpread) * myInfo.spread, 1);
		
		
		
		
		
		myRigid.velocity = velocity;
		
		
		
		Invoke("Recycle", myInfo.projectileLifeTime);
		
	}
	
	
	
	void FixedUpdate()
		
	{
		
		Debug.DrawLine(transform.position, transform.position + myRigid.velocity / 60, Color.red);
		
		Debug.DrawLine(transform.position, transform.position - myRigid.velocity / 30, Color.magenta);
		
		
		
		RaycastHit hit;  // forward hit
		
		RaycastHit hit2; // rear hit
		
		
		
		
		
		if (Physics.Raycast(transform.position, myRigid.velocity, out hit, 10.0f, ~LayerMask.NameToLayer("Projectiles")))
			
		{
			
			// probably shouldn't do this but best way i can think of to avoid
			
			// multiple hits from same bullet
			
			myRigid.MovePosition(hit.point); // move the bullet to the impact point
			
			
			
			
			
			if (hit.transform.CompareTag("Ground"))
				
			{// if we hit dirt... kill the bullet since most weapons don't pierce the earth
				
				CancelInvoke("Recycle");
				
				Recycle(); 
				
			}
			
			
			
			if(hit.transform.CompareTag("Player"))
				
			{
				
				// try to grab a health component from a player Object
				
				Health hitObject = hit.transform.GetComponent<Health>();           
				
				
				
				// if the hit object has a Health component... let it know it was hit
				
				if (hitObject)
					
				{                
					
					hitObject.Hit(myInfo); // send bullet info to hit object's health component
					
				}
				
			}
			
			else
				
			{
				
				MakeAHole(hit);
				
			}
			
			
			
			hitCount++; // add a hit
			
			
			
			if (hitCount > myInfo.maxPenetration)
				
			{
				
				CancelInvoke("Recycle");
				
				Recycle(); // if hit count exceeds max hits.... kill the bullet
				
			}
			
		}
		
		
		
		// this shoots a ray out behind the bullet.
		
		// use this to add a bullet hole to the back side of a penetrated wall or whatever
		
		if (Physics.Raycast(transform.position, -myRigid.velocity, out hit2, 2.0f , ~LayerMask.NameToLayer("Projectiles")))
			
		{
			
			if (hit2.transform.CompareTag("Player"))
				
			{
				
				// do nothing since we probably already penetrated the player
				
			}
			
			else
				
			{
				
				MakeAHole(hit2);
				
			}
			
			
			
		}
		
		
		
	}
	
	
	
	void MakeAHole(RaycastHit hit)
		
	{
		
		BulletHoleManager.bulletHole.SpawnHole(hit);
		
	}
	
	
	
	void Recycle()
		
	{
		
		if (myInfo.usePool)
			
		{
			
			ObjectPool.pool.PoolObject(gameObject);
			
		}
		
		else
			
		{
			
			Destroy(gameObject);
			
		}
		
	}
	
}

