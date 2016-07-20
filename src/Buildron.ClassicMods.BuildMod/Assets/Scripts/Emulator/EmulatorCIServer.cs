using UnityEngine;
using System.Collections;
using Buildron.Domain.CIServers;

public class EmulatorCIServer : ICIServer {
	#region ICIServer implementation
	public CIServerType ServerType { get; set; }
		
	public string Title  { get; set; }
	public string IP  { get; set; }
	public float RefreshSeconds  { get; set; }
	public bool FxSoundsEnabled  { get; set; }
	public bool HistoryTotemEnabled  { get; set; }
	public int BuildsTotemsNumber  { get; set; }
	public CIServerStatus Status  { get; set; }
	#endregion
	#region IAuthUser implementation
	public string Domain { get; set; }
	public string DomainAndUserName { get; private set; }
	public string UserName  { get; set; }
	public string Password { get; set; }
	#endregion

	#region IEntity implementation
	public long Id  { get; set; }
	public bool IsNew {
		get {
			return Id == 0;
		}
	}
	#endregion
}
