using Skahal.Effects;
using UnityEngine;
using Buildron.ClassicMods.EasterEggMod;

/// <summary>
/// Matrix easter egg controller.
/// </summary>
public class MatrixEasterEggController : EasterEggControllerBase
{
	#region Fields
	private bool m_actived;
	private MeshFilter[] m_currentAppliedTo;
	#endregion

	#region Life cycle
	void Start ()
	{
		EasterEggsNames.Add("Matrix");
	}
	
	void OnMatrix ()
	{
		if (m_actived) {
			WireframeEffectService.Unapply (m_currentAppliedTo);
			m_actived = false;
		} else {
			var cam = Mod.Context.Camera.MainCamera;
			m_actived = true;
			m_currentAppliedTo = WireframeEffectService.ApplyToAllSceneMeshFilters ((controller) => {
				controller.TargetCamera = cam;
			});
		}
	}
	#endregion
}

