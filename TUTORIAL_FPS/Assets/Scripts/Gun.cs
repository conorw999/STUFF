using UnityEngine;

using System.Collections;



public class Gun : MonoBehaviour {
	
	
	
	public WeaponType typeOfWeapon;             // type of weapon, used to determine how the trigger acts
	
	public bool usePooling = false;             // do we want to use object pooling or instantiation
	
	public GameObject projectile = null;        // projectile prefab... whatever this gun shoots
	
	public Damage damage = new Damage();        // the damage and type of damage this gun does
	
	public float projectileSpeed = 100.0f;      // speed that projectile flies at
	
	public float projectileForce = 10.0f;       // force applied to any rigidbodies the projectile hits
	
	public float projectileLifeTime = 5.0f;     // how long before the projectile is considered gone and recycleable
	
	
	
	public Transform muzzlePoint = null;        // the muzzle point for this gun, where you want bullets to be spawned
	
	
	
	public int maxPenetration = 1;              // maximum amount of hits detected before the bullet is destroyed
	
	public float fireRate = 0.5f;               // time betwen shots
	
	
	
	public bool infinteAmmo = false;            // gun can have infinite ammo if thats what you wish
	
	public int roundsPerClip = 50;              // number of bullets in each clip
	
	public int numberOfClips = 5;               // number of clips you start with
	
	public int maxNumberOfClips = 10;           // maximum number of clips you can hold
	
	public int roundsLeft;                      // bullets in the gun-- current clip
	
	
	
	public float reloadTime = 2.5f;             // how long it takes to reload in seconds
	
	protected bool isReloading = false;         // are we currently reloading
	
	
	
	public float baseSpread = 0.2f;             // how accurate the weapon starts out... smaller the number the more accurate
	
	public float maxSpread = 4.0f;              // maximum inaccuracy for the weapon
	
	public float spreadPerShot = 0.05f;         // increase the inaccuracy of bullets for every shot
	
	public float spread = 0.0f;                 // current spread of the gun
	
	public float decreaseSpreadPerTick = 0.5f;  // amount of accuracy regained per frame when the gun isn't being fired 
	
	public float spreadDecreaseTicker = 1.0f;   // time in seconds to decrease inaccuracy
	
	
	
	protected float nextFireTime = 0.0f;        // able to fire again on this frame
	
	protected bool spreadDecreasing = false;    // is the gun currently decrasing the spread
	
	
	
	protected ProjectileInfo bulletInfo = new ProjectileInfo(); // all info about gun thats sent to each projectile
	
	
	
	
	
	void Start()
		
	{
		
		roundsLeft = roundsPerClip; // load gun on startup
		
		SetupBulletInfo(); // set a majority of the projectile info
		
	}
	
	
	
	// all guns handle firing a bit different so give it a blank function that each gun can override
	
	public virtual void Fire()
		
	{
		
		
		
	}
	
	
	
	
	
	// everything fires a single round the same
	
	protected virtual void FireOneShot() {
		
		if (roundsLeft > 0)
			
		{
			
			Vector3 pos = muzzlePoint.position; // position to spawn bullet is at the muzzle point of the gun       
			
			Quaternion rot = muzzlePoint.rotation; // spawn bullet with the muzzle's rotation
			
			
			
			bulletInfo.spread = spread; // set this bullet's info to the gun's current spread
			
			GameObject newBullet;
			
			
			
			if (usePooling)
				
			{
				
				newBullet = ObjectPool.pool.GetObjectForType(projectile.name, true);
				
				newBullet.transform.position = pos;
				
				newBullet.transform.rotation = rot;
				
			}
			
			else
				
			{
				
				newBullet = Instantiate(projectile, pos, rot) as GameObject; // create a bullet
				
			}
			
			
			
			newBullet.GetComponent<Projectile>().SetUp(bulletInfo); // send bullet info to spawned projectile
			
			
			
			spread += spreadPerShot;  // we fired so increase spread
			
			
			
			// if the current spread is greater then the weapons max spread, set it to max
			
			if (spread >= maxSpread)
				
			{
				
				spread = maxSpread;
				
			}
			
			
			
			// if the spread is not currently decreasing, start it up cause we just fired
			
			if (!spreadDecreasing)
				
			{
				
				InvokeRepeating("DecreaseSpread", spreadDecreaseTicker, spreadDecreaseTicker);
				
				spreadDecreasing = true;
				
			}            
			
			
			
			// if this gun doesn't have infinite ammo, subtract a round from our clip
			
			if (!infinteAmmo)
				
			{
				
				roundsLeft--;
				
				
				
				// if our clip is empty, start to reload
				
				if (roundsLeft <= 0)
					
				{
					
					StartCoroutine(Reload());
					
				}
				
			}
			
		}
		
	}
	
	
	
	// reload your weapon
	
	protected virtual IEnumerator Reload()
		
	{
		
		if (isReloading)
			
		{
			
			yield break; // if already reloading... exit and wait till reload is finished
			
		}
		
		
		
		if (numberOfClips > 0)
			
		{
			
			isReloading = true; // we are now reloading
			
			numberOfClips--; // take away a clip
			
			yield return new WaitForSeconds(reloadTime); // wait for set reload time
			
			roundsLeft = roundsPerClip; // fill up the gun
			
		}
		
		
		
		isReloading = false; // done reloading
		
	}
	
	
	
	
	
	void DecreaseSpread()
		
	{
		
		// decrease the current spread per tick
		
		spread -= decreaseSpreadPerTick;
		
		
		
		
		
		// if the current spread is less then the base spread value, set it to the base
		
		if (spread <= baseSpread)
			
		{
			
			spread = baseSpread;
			
			
			
			// stop the decrease spread function until we need it again
			
			spreadDecreasing = false;
			
			CancelInvoke("DecreaseSpread");
			
		}
		
	}
	
	
	
	
	
	// set all bullet info from the gun's info
	
	protected void SetupBulletInfo()
		
	{
		
		bulletInfo.damage.amount = damage.amount;       // amount of damage
		
		bulletInfo.damage.type = damage.type;           // type of damage
		
		bulletInfo.force = projectileForce;             // weapon force
		
		bulletInfo.maxPenetration = maxPenetration;     // max hits
		
		bulletInfo.maxSpread = maxSpread;               // max weapon spread
		
		bulletInfo.spread = spread;                     // current weapon spread value
		
		bulletInfo.speed = projectileSpeed;             // projectile speed
		
		bulletInfo.owner = transform.root.gameObject;   // this projectile's owner gameobject, useful if you want to know whose killing what for kills/assists or whatever
		
		bulletInfo.usePool = usePooling;                // do we use object pooling
		
		bulletInfo.projectileLifeTime = projectileLifeTime;
		
	}
	
}