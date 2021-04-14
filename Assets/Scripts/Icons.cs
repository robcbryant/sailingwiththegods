using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Icons
{
	private const string _resourcePath = "resource_icons";
	private const string _crewPath = "crew_portraits";
	private const string _settlementPath = "settlement_portraits";
	private const string _coinPath = "settlement_coins";

	private const string _defaultSettlementPortrait = "settlement_portraits/gui_port_portrait_default";
	private const string _defaultCoinIcon = "settlement_coins/default_coin_texture";
	private const string _defaultCrewPortrait = "crew_portraits/phoenician_sailor";

	// just a readonly reference to the one in GameVars
	IEnumerable<MetaResource> _masterResourceList;

	public Icons(IEnumerable<MetaResource> masterResourceList) {
		_masterResourceList = masterResourceList;
	}

	public Sprite DefaultCrewPortrait => Resources.Load<Sprite>(_defaultCrewPortrait);
	public Sprite GetCrewPortrait(CrewMember member) => Resources.Load<Sprite>(_crewPath + "/" + member.ID) ?? DefaultCrewPortrait;

	public Sprite GetCargoIcon(MetaResource resource) => Resources.Load<Sprite>(_resourcePath + "/" + resource.icon);
	public Sprite GetCargoIcon(Resource resource) => GetCargoIcon(_masterResourceList.FirstOrDefault(r => r.name == resource.name));

	public Sprite DefaultPortIcon => Resources.Load<Sprite>(_defaultSettlementPortrait);
	public Sprite GetPortIcon(Settlement settlement) => Resources.Load<Sprite>(_settlementPath + "/" + settlement.settlementID) ?? DefaultPortIcon;

	public Sprite DefaultCoinIcon => Resources.Load<Sprite>(_defaultCoinIcon);
	public Sprite GetPortCoinIcon(Settlement settlement) => Resources.Load<Sprite>(_coinPath + "/" + settlement.settlementID) ?? DefaultCoinIcon;
}

public static class IconExtensions
{
	public static Sprite IconSprite(this MetaResource self) => Globals.GameVars.Icons.GetCargoIcon(self);
	public static Sprite IconSprite(this Resource self) => Globals.GameVars.Icons.GetCargoIcon(self);

	public static Sprite PortraitSprite(this CrewMember self) => Globals.GameVars.Icons.GetCrewPortrait(self);

	public static Sprite PortIcon(this Settlement self) => Globals.GameVars.Icons.GetPortIcon(self);
	public static Sprite PortCoinIcon(this Settlement self) => Globals.GameVars.Icons.GetPortCoinIcon(self);
}