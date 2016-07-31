using UnityEngine;
using System.Collections;
using Buildron.Domain.Mods;
using Buildron.Domain.Users;

public class TestUserController : MonoBehaviour, IUserController {

	public string UserName;

	public IUser Model
	{
		get 
		{
			return new EmulatorUser(UserName);
		} 
	}

	public Rigidbody Rigidbody {
		get {
			throw new System.NotImplementedException ();
		}
	}

	public Collider CenterCollider {
		get {
			throw new System.NotImplementedException ();
		}
	}

	public Collider TopCollider {
		get {
			throw new System.NotImplementedException ();
		}
	}

	public Collider LeftCollider {
		get {
			throw new System.NotImplementedException ();
		}
	}

	public Collider RightCollider {
		get {
			throw new System.NotImplementedException ();
		}
	}

	public Collider BottomCollider {
		get {
			throw new System.NotImplementedException ();
		}
	}
}
