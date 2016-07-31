using UnityEngine;
using System.Collections;
using Buildron.Domain;
using Skahal.Common;
using Buildron.Domain.RemoteControls;
using Buildron.ClassicMods.EasterEggMod;
using System.Linq;

/// <summary>
/// Kick easter egg controller.
/// </summary>
public class KickEasterEggController : EasterEggControllerBase 
{
	#region Fields
	#endregion

    #region Life cycle
    void Start ()
	{
		EasterEggsNames.Add ("Kick");
		EasterEggsNames.Add ("KickAll");
	}
	 	
	private void OnKick ()
	{		
		var avatar = Mod.Context.UserGameObjects.GetAll().GetByUsername(Mod.ConnectedRC.UserName);
		Kick (avatar);
	}

	private void OnKickAll ()
	{
		var avatars = Mod.Context.UserGameObjects.GetAll ().Select (c => c.gameObject).ToArray ();
		StartCoroutine (KickAll (avatars));
	}
	
	private void Kick (GameObject avatar)
	{
		// Check for null because the connected remote control could be from a user without a current build.
		if (avatar != null) {
			avatar.SendMessage("PlayKick", SendMessageOptions.DontRequireReceiver);
			BallController.CreateGameObject(avatar.transform.position);
		}
	}
	
	private IEnumerator KickAll (GameObject[] avatars)
	{
		foreach (var avatar in avatars) {
			Kick (avatar);
			yield return new WaitForSeconds(0.2f);
		}
	}
	#endregion
}