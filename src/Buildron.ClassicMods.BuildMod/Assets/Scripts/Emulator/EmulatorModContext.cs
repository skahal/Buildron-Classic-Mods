using UnityEngine;
using Buildron.Domain.Mods;
using System;
using Buildron.Domain.Builds;
using Buildron.Domain.CIServers;
using Buildron.Domain.Users;
using System.Collections.Generic;
using Buildron.Domain.RemoteControls;
using Skahal.Logging;
using Buildron.Infrastructure.AssetsProxies;
using Buildron.Infrastructure.GameObjectsProxies;
using Skahal.Common;
using Buildron.ClassicMods.BuildMod; 

public class EmulatorModContext : MonoBehaviour, IModContext {

	#region Events
	public event EventHandler<BuildFoundEventArgs> BuildFound;

	public event EventHandler<BuildRemovedEventArgs> BuildRemoved;

	public event EventHandler<BuildUpdatedEventArgs> BuildUpdated;

	public event EventHandler<BuildStatusChangedEventArgs> BuildStatusChanged;

	public event EventHandler<BuildTriggeredByChangedEventArgs> BuildTriggeredByChanged;

	public event EventHandler<BuildsRefreshedEventArgs> BuildsRefreshed;

	public event EventHandler<CIServerConnectedEventArgs> CIServerConnected;

	public event EventHandler<CIServerStatusChangedEventArgs> CIServerStatusChanged;

	public event EventHandler<UserFoundEventArgs> UserFound;

	public event EventHandler<UserUpdatedEventArgs> UserUpdated;

	public event EventHandler<UserTriggeredBuildEventArgs> UserTriggeredBuild;

	public event EventHandler<UserRemovedEventArgs> UserRemoved;

	public event EventHandler<UserAuthenticationCompletedEventArgs> UserAuthenticationCompleted;

	public event EventHandler<RemoteControlChangedEventArgs> RemoteControlChanged;
	#endregion

	#region Properties
	public IList<IBuild> Builds { get; private set; }

	public IList<IUser> Users { get; private set; }

	public ICIServer CIServer { get; private set; }

	public ISHLogStrategy Log { get; private set; }

	public IAssetsProxy Assets  { get; private set; }

	public IGameObjectsProxy GameObjects { get; private set; }

	public IUIProxy UI { get; private set; } 
	#endregion

	#region Methods
	private void Awake()
	{
		Builds = new List<IBuild> ();
		Users = new List<IUser> ();
		CIServer = new EmulatorCIServer ();
		Log = new SHDebugLogStrategy ();
		Assets = new ResourcesFolderAssetsProxy ();
		GameObjects = new ModGameObjectsProxy ();

		var mod = new Mod ();
		mod.Initialize (this);
	}

	private void OnGUI()
	{
		if(GUILayout.Button("CIServerConnected"))
		{
			CIServerConnected.Raise (this, new CIServerConnectedEventArgs (CIServer));
		}

		if(GUILayout.Button("BuildFound"))
		{
			var build = new EmulatorBuild ();
			Builds.Add (build);
			Log.Debug ("BuildFound: {0}; {1}", build.Id, build.Status);
			BuildFound.Raise (this, new BuildFoundEventArgs (build));
		}
	}
	#endregion
}
