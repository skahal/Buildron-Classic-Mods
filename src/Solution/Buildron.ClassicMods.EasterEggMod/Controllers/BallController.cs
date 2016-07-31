#region Usings
using UnityEngine;
using System.Collections;
using Skahal.Logging;
using Buildron.ClassicMods.EasterEggMod;


#endregion

/// <summary>
/// Ball controller.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class BallController : MonoBehaviour {
	
	#region Editor properties
	public Vector2 XForceRandomRange = new Vector2(-100, 100);	
	public float YForce = 1000;
	public float ZForce = 1000;
	#endregion
	
	#region Methods
	private void Initialize (Vector3 position)
	{
		transform.position = position + Vector3.up;
		var rb = GetComponent<Rigidbody> ();
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		rb.AddForce (new Vector3 (Random.Range (XForceRandomRange.x, XForceRandomRange.y), YForce, ZForce));
	}
	
	public static GameObject CreateGameObject (Vector3 position)
	{
		var ctx = Mod.Context;

		ctx.Log.Debug ("Requesting ball from poll.");
		var ball =  ctx.GameObjectsPool.GetGameObject ("Ball", 5);
		ball.SendMessage("Initialize", position);
		ctx.Log.Debug ("Ball received from poll.");
		
		return ball;
	}
	#endregion
}