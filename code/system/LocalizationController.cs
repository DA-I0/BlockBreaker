using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class LocalizationController
{
	Dictionary<string, string> _customValues = new Dictionary<string, string>();

	// NOTE: change to dynamic loading if I'll decide to include all breakables in the info panel
	private Dictionary<string, Breakable> _breakables = new Dictionary<string, Breakable>
	{
		{"block_health", (Breakable)ResourceLoader.Load<PackedScene>("res://prefabs/blocks/block_basic.tscn").Instantiate()},
		{"block_sturdy_health", (Breakable)ResourceLoader.Load<PackedScene>("res://prefabs/blocks/block_sturdy.tscn").Instantiate()},
		{"barrier_sturdy_health", (Breakable)ResourceLoader.Load<PackedScene>("res://prefabs/blocks/barrier_sturdy.tscn").Instantiate()}
	};

	private readonly SessionController refs;

	public LocalizationController(SessionController sessionController)
	{
		refs = sessionController;
		LoadCustomValues();
	}

	public void UpdateUILocalization()
	{
		List<Node> nodes = refs.GetTree().GetNodesInGroup("manual_translation").ToList();

		foreach (Node node in nodes)
		{
			TranslateContent((RichTextLabel)node);
		}
	}

	private void LoadCustomValues()
	{
		_customValues.Clear();
		string content = FileOperations.LoadTextFile("res://assets/text/custom_text_values.txt");
		ParseCustomValues(content);
	}

	private void ParseCustomValues(string content)
	{
		string[] splitContent = content.Split(";");

		foreach (string line in splitContent)
		{
			string[] keyValuePair = line.Split(":");

			if (keyValuePair.Count() == 2)
			{
				_customValues.Add(keyValuePair[0].Trim(), keyValuePair[1].Trim());
			}
		}
	}

	private void TranslateContent(RichTextLabel label)
	{
		string rawLocalization = TranslationServer.Translate(label.Name);
		label.Text = InsertCustomValues(rawLocalization);
	}

	public string InsertCustomValues(string sourceString)
	{
		string modifiedString = sourceString;
		int placeholderStart = modifiedString.Find("{");
		int placeholderEnd = modifiedString.Find("}") + 1;

		while (placeholderStart > -1 && placeholderEnd > -1)
		{
			string placeholder = modifiedString.Substring(placeholderStart, placeholderEnd - placeholderStart);

			if (placeholder.Contains("game_"))
			{
				modifiedString = modifiedString.Replace(placeholder, GetInputSymbol(placeholder.Replace("{", "").Replace("}", ""), refs.settings.ActiveInputType.ToString())[0]);//GetInputValues(placeholder.Replace("{", "").Replace("}", "")));
			}
			else
			{
				modifiedString = modifiedString.Replace(placeholder, GetDynamicValue(placeholder.Replace("{", "").Replace("}", "")));
			}

			placeholderStart = modifiedString.Find("{");
			placeholderEnd = modifiedString.Find("}") + 1;
		}

		return modifiedString;
	}

	public string[] GetInputSymbol(string inputEvent, string device)
	{
		var inputEvents = device.Contains("Keyboard") ? InputMap.ActionGetEvents(inputEvent).Where(a => !a.AsText().Contains("Joypad") && !a.AsText().Contains("Mouse")) : InputMap.ActionGetEvents(inputEvent).Where(a => a.AsText().Contains(device));
		List<string> inputSymbols = new List<string>();

		if (device.Contains("Mouse") && (inputEvent == "game_left" || inputEvent == "game_right"))
		{
			inputSymbols.Add(GetCustomValue("Mouse"));
		}

		for (int index = 0; index < inputEvents.Count(); index++)
		{
			string targetAction = inputEvents.ElementAt(index).AsText();

			if (targetAction.Find(" (") != -1)
			{
				targetAction = targetAction.Substring(0, targetAction.Find(" ("));
				targetAction = SetJoypadType(targetAction);
			}

			inputSymbols.Add(GetCustomValue(targetAction));
		}

		return (inputSymbols.Count < 1 && device.Contains("Mouse")) ? GetInputSymbol(inputEvent, "Keyboard") : inputSymbols.ToArray<string>();// : new string[] { string.Empty };
	}

	private string GetCustomValue(string key)
	{
		try
		{
			return _customValues[key];
		}
		catch
		{
			// GD.PrintErr("Replacement value not found");
		}

		return key.Contains("_") ? key[3..] : key;
	}

	private string GetDynamicValue(string key)
	{
		switch (key)
		{
			case "multiplier_combo_step":
				return refs.gameScore.ComboMultiplierStep.ToString();

			case "multiplier_max":
				return refs.gameScore.MaxScoreMultiplier.ToString();

			case "multiplier_max_combo":
				return refs.gameScore.MaxComboMultiplier.ToString();

			default:
				return _breakables[key].MaxHealth.ToString();
		}
	}

	public string GetInputKey(string targetValue)
	{
		foreach (var entry in _customValues)
		{
			if (entry.Value == targetValue)
			{
				return entry.Key;
			}
		}

		return targetValue;
	}

	private string SetJoypadType(string targetAction)
	{
		switch (refs.settings.ControlerPrompts)
		{
			case "nintendo":
				return $"ni_{targetAction}";

			case "playstation":
				return $"ps_{targetAction}";

			case "steam deck":
				return $"sd_{targetAction}";

			case "xbox":
				return $"xb_{targetAction}";

			default:
				return targetAction;
		}
	}
}
