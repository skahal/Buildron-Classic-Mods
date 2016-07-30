using UnityEngine;
using Buildron.Domain.Builds;
using Buildron.Domain.Users;
using Buildron.ClassicMods.UserMod;

/// <summary>
/// Manages the UserController creations.
/// </summary>
public class UsersManager : MonoBehaviour
{
	#region Fields
	private Vector3 m_currentSpawnPosition;
	private int m_currentRowUserCount;
	private int m_rowsCount = 1;
	#endregion
	
	#region Properties
	public Vector3 FirstSpawnPosition = new Vector3(-10, -3, -10);
	public Vector3 DistanceBetweenUsers = new Vector3(2, 0, 0);
	public int NumberUserPerRows = 5;
	public Vector3 DistanceBetweenUsersRows = new Vector3(0, 0, 2);
	#endregion
	
	#region Methods
	private void Awake ()
	{
		Mod.Context.BuildFound += (sender, e) => {
            CreateUserGameObject(e.Build);
            e.Build.StatusChanged += (sender1, e1) => {
				CreateUserGameObject(e.Build);
			};
		};

		m_currentSpawnPosition = FirstSpawnPosition;
	}

	void CreateUserGameObject (IBuild build)
	{
        if (build.TriggeredBy == null)
        {
            build.TriggeredByChanged += delegate {
                CreateUserGameObject(build);
            };

            return;
        }

        var go = UserController.GetGameObject (build.TriggeredBy);
		
		if (go != null) {
			go.GetComponent<UserController> ().Data = build.TriggeredBy;
		} else {
			go = UserController.CreateGameObject (build.TriggeredBy);
			go.transform.position = m_currentSpawnPosition;
			go.transform.parent = transform;			
			
			m_currentSpawnPosition += DistanceBetweenUsers;
			m_currentRowUserCount++;
			
			if (m_currentRowUserCount >= NumberUserPerRows) {
				m_currentRowUserCount = 0;
				m_currentSpawnPosition = FirstSpawnPosition;
				m_currentSpawnPosition += DistanceBetweenUsersRows * m_rowsCount;
				m_rowsCount++;
			}
		}
	}
	#endregion
}