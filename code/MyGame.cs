﻿using Sandbox;
using Sandbox.UI.Construct;
using ScienceCenter;
using Sandbox.Entities;
using ScienceCenter.Entities.Interfaces;
using ScienceCenter.UI;
using Sandbox.UI;
using System.Linq;
using System.Collections.Generic;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace ScienceCenter;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players. 
/// </summary>
public partial class MyGame : Sandbox.Game
{
	public static new MyGame Current { get; private set; }


	public MyGame()
	{
		Current = this;

		if ( IsClient )
		{
			var hud = new RootPanel();
			hud.AddChild<MainRazorHud>();
		}

	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		// Create a pawn for this client to play with
		var pawn = new Researcher( client );
		client.Pawn = pawn;

		// Get all of the spawnpoints
		var spawnpoints = Entity.All.OfType<SpawnPoint>();

		// chose a random one
		var randomSpawnPoint = spawnpoints.FirstOrDefault();

		// if it exists, place the pawn there
		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			tx.Position = tx.Position + Vector3.Up * 50.0f; // raise it up
			pawn.Transform = tx;
		}

		pawn.Respawn();
	}


	[ConCmd.Server( "kill" )]
	static void KillCommand()
	{
		Log.Info( "Hello I am kill" );
		var target = ConsoleSystem.Caller;
		if ( target == null ) return;

		(Current as Game)?.DoPlayerSuicide( target );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		//Map.Entity.Position += new Vector3( 0, 1, 1 );
	}
}
