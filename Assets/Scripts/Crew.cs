using System.Collections.Generic;
using System.Linq;

public class Crew
{
	// just a readonly reference to the one in GameVars
	IEnumerable<CrewMember> _masterCrewList;
	IEnumerable<PirateType> _masterPirateTypeList;
	Ship _ship;

	public Crew(IEnumerable<CrewMember> masterCrewList, IEnumerable<PirateType> masterPirateTypeList, Ship ship) {
		_masterCrewList = masterCrewList;
		_masterPirateTypeList = masterPirateTypeList;
		_ship = ship;
	}

	public CrewMember Jason => _masterCrewList.FirstOrDefault(c => c.isJason);
	public IEnumerable<CrewMember> StandardCrew => _masterCrewList.Where(c => !c.isPirate);
	public IEnumerable<CrewMember> Pirates => _masterCrewList.Where(c => c.isPirate);
	public IEnumerable<PirateType> PirateTypes => _masterPirateTypeList;
	public IEnumerable<CrewMember> AllNonCrew => StandardCrew.Where(c => !_ship.crewRoster.Contains(c));
}
