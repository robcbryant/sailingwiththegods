using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Globals
{
	static Registry Registry = new Registry();

	public static GameVars GameVars => Get<GameVars>();
	public static GameUISystem UI => Get<GameUISystem>();
	public static QuestSystem Quests => Get<QuestSystem>();

	public static void Register<T>(T obj)
	{
		Registry.Register(obj);
	}

	public static void Unregister<T>(T obj)
	{
		Registry.Unregister(obj);
	}

	public static bool Has<T>() => Registry.Has<T>();
	public static T Get<T>()
	{
		return Registry.Get<T>();
	}
}
