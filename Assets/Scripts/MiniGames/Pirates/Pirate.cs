using UnityEngine;

public class Pirate : CrewCard
{
	public int clout;
	public string pirateName;
	public Sprite pirateImage;

    public Pirate(int clout, string pirateName, Sprite pirateImage) {
		this.clout = clout;
		this.pirateName = pirateName;
		this.pirateImage = pirateImage;
	}
}
