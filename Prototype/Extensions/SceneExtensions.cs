namespace Prototype.Extensions;

using Stride.Engine;

public static class SceneExtensions
{
	public static Entity? FirstOrDefault(this IEnumerable<Entity> entities, string name)
	{
		return entities.FirstOrDefault(entity => entity.Name == name);
	}

	public static IEnumerable<Entity> OfType(this IEnumerable<Entity> entities, string name)
	{
		return entities.Where(entity => entity.Name == name);
	}

	public static T? FirstOrDefault<T>(this IEnumerable<EntityComponent> components)
		where T : EntityComponent
	{
		return components.FirstOrDefault(component => component is T) as T;
	}
}
