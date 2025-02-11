﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace ScienceCenter.Entities.Weapons
{
	[Title("Base Weapon Class"), Icon("sports_martial_arts")]
	public partial class BaseResearchWeapon : BaseCarriable
	{
		public virtual float PrimaryRate => 5.0f;
		public virtual float SecondaryRate => 15.0f;

		public override void Spawn()
		{
			base.Spawn();
			Tags.Add( "pickup" );
			Tags.Add( "equipable" );
		}

		[Net, Predicted]
		public TimeSince TimeSincePrimaryAttack { get; set; }

		[Net, Predicted]
		public TimeSince TimeSinceSecondaryAttack { get; set; }

		public override void Simulate( Client cl )
		{
			if ( CanReload() )
			{
				Reload();
			}

			if ( !Owner.IsValid() )
				return;

			if ( CanPrimaryAttack() )
			{
				using ( LagCompensation() )
				{
					TimeSincePrimaryAttack = 0;
					AttackPrimary();
				}
			}

			if ( !Owner.IsValid() )
				return;
			if ( CanSecondaryAttack() )
			{
				using ( LagCompensation() )
				{
					TimeSinceSecondaryAttack = 0;
					AttackSecondary();
				}
			}
		}

		public virtual bool CanPrimaryAttack()
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.PrimaryAttack ) ) return false;

			var rate = PrimaryRate;
			if ( rate <= 0 ) return true;

			return TimeSincePrimaryAttack > (1 / rate);
		}

		public virtual void AttackPrimary()
		{
			
		}


		public virtual void Reload()
		{
			
		}

		public virtual bool CanReload()
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.Reload ) ) return false;

			return true;
		}
		public virtual bool CanSecondaryAttack()
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.SecondaryAttack ) ) return false;

			var rate = SecondaryRate;
			if ( rate <= 0 ) return true;

			return TimeSinceSecondaryAttack > (1 / rate);
		}
		public virtual void AttackSecondary()
		{

		}

		public virtual IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f )
		{
			bool underWater = Trace.TestPoint( start, "water" );

			var trace = Trace.Ray( start, end )
					.UseHitboxes()
					.WithAnyTags( "solid", "player", "npc" )
					.Ignore( this )
					.Size( radius );

			//
			// If we're not underwater then we can hit water
			//
			if ( !underWater )
				trace = trace.WithAnyTags( "water" );

			var tr = trace.Run();

			if ( tr.Hit )
				yield return tr;

			//
			// Another trace, bullet going through thin material, penetrating water surface?
			//
		}
		public override Sound PlaySound( string soundName, string attachment )
		{
			if ( Owner.IsValid() )
				return Owner.PlaySound( soundName, attachment );

			return base.PlaySound( soundName, attachment );
		}
	}
}
