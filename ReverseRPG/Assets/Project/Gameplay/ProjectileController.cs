﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileController : MonoBehaviour 
{
	public OrbitProjectile[] projectiles;

	public float targettingDistance = 5.0f;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( projectiles == null || projectiles.Length == 0 )
		{
			projectiles = this.transform.GetComponentsInChildren<OrbitProjectile>();
		}
	}


	protected IEnumerator TargetingRoutine()
	{

		while( true )
		{
			
			EnemyTarget[] enemies = GameObject.FindObjectsOfType<EnemyTarget>();

			foreach( EnemyTarget enemy in enemies )
			{
				if( enemy.markedForDestruction )
					continue;

				if( enemy.damageType != RPG.DamageType.Melee )
					continue;

				OrbitProjectile projectile = null;

				float distance = Vector3.Distance( enemy.transform.position, this.transform.position + (this.transform.forward * targettingDistance/2.0f) );
				if( distance < targettingDistance )
				{
					foreach( OrbitProjectile candidate in projectiles )
					{
						if( candidate == null )
							continue;

						// find first projectile that's not attacking already
						if( candidate.OrbitEnabled )
						{
							projectile = candidate;
							break;
						}
					}
					if( projectile != null )
					{
						enemy.markedForDestruction = true;

						projectile.Attack(enemy, 0.8f);
						//projectile.gameObject.MoveTo( projectile.GetOrbitStartPosition() ).Time( 0.5f ).Delay(0.5f).Execute();
					}

				}
			}

			yield return new WaitForSeconds(0.3f);
		}
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script

		if( !LugusConfig.use.User.GetBool("downgrade.support.weapons", false) )
		{
			LugusCoroutines.use.StartRoutine( TargetingRoutine() );
		}
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
	
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red.a (1.0f);
		Gizmos.DrawWireSphere( this.transform.position + (this.transform.forward * (targettingDistance/1.5f)), targettingDistance );
	}

}
