using System;
using Terraria.Localization;
using Newtonsoft.Json;

namespace BetterChests;

[Serializable]
public enum SortOptions
{
	Default,
	ID,
	Alphabetically,
	Rarity,
	Stack,
	Value,
	Damage,
	Defense,
	Mod,
	Random
}

// A enum wrapper to separate serialization from ToString
[JsonConverter(typeof(SortOptionToStringConverter))]
public readonly struct SortOption
{
	private readonly SortOptions data;
	private SortOption(SortOptions o)
	{
		data = o;
	}
	
	public static implicit operator SortOptions(SortOption o) => o.data;
	public static implicit operator SortOption(SortOptions o) => new(o);

	public override string ToString()
	{
		return Language.GetTextValue($"Mods.{nameof(BetterChests)}.{nameof(SortOptions)}.{data}.Label");
	}
	
	// custom Json converter, to convert the enum wrapper object into a string
	private sealed class SortOptionToStringConverter : JsonConverter<SortOption>
	{
		public override void WriteJson(JsonWriter writer, SortOption value, JsonSerializer serializer)
		{
			writer.WriteValue(Enum.GetName((SortOptions)value));
		}

		public override SortOption ReadJson(JsonReader reader, Type objectType, SortOption existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			string str = reader.Value?.ToString();
			if (!Enum.TryParse(str, out SortOptions opt)) {
				throw new JsonException();
			}

			return opt;
		}
	}
}

