using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class LocalizationController
{
	Dictionary<string, string> _customValues = new Dictionary<string, string>();
	// 	Joypad Button 0 -> PS x,      X a,    N b
	// 	Joypad Button 1 -> PS o,      X b,    N a
	// 	Joypad Button 2 -> PS [],     X x,    N y
	// 	Joypad Button 3 -> PS ^,      X y,    N x
	// 	Joypad Button 4 -> PS select, X back, N -
	// 	Joypad Button 5 -> PS PS,     X home, N home?
	// 	Joypad Button 6 -> PS start,  X start, N+
	// 	Joypad Button 7 -> left stick click
	// 	Joypad Button 8 -> right stick click
	// 	Joypad Button 9 -> left shoulder
	// 	Joypad Button 10 -> right shoulder
	// 	Joypad Button 11 -> d-pad up
	// 	Joypad Button 12 -> d-pad down
	// 	Joypad Button 13 -> d-pad left
	// 	Joypad Button 14 -> d-pad right

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
		string content = refs.fileOperations.LoadTextFile("res://assets/text/custom_text_values.txt");
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

	private string InsertCustomValues(string sourceString)
	{
		string modifiedString = sourceString;
		int placeholderStart = modifiedString.Find("{");
		int placeholderEnd = modifiedString.Find("}") + 1;

		while (placeholderStart > -1 && placeholderEnd > -1)
		{
			string placeholder = modifiedString.Substring(placeholderStart, placeholderEnd - placeholderStart);
			modifiedString = placeholder.Contains("game_") ? modifiedString.Replace(placeholder, GetInputValues(placeholder.Replace("{", "").Replace("}", ""))) : modifiedString.Replace(placeholder, GetCustomValue(placeholder.Replace("{", "").Replace("}", "")));
			placeholderStart = modifiedString.Find("{");
			placeholderEnd = modifiedString.Find("}") + 1;
		}

		return modifiedString;
	}

	private string GetInputValues(string inputs)
	{
		var inputEvents = InputMap.ActionGetEvents(inputs);
		string inputValue = string.Empty;

		foreach (InputEvent inputEvent in inputEvents)
		{
			if (refs.settings.ActiveController != InputType.gamepad && !inputEvent.AsText().Contains("Joypad"))
			{
				inputValue += inputValue.Contains(_customValues[inputEvent.AsText()]) ? string.Empty : $"{_customValues[inputEvent.AsText()]} ";
				continue;
			}

			if (refs.settings.ActiveController == InputType.gamepad && inputEvent.AsText().Contains("Joypad"))
			{
				string joystickInput = inputEvent.AsText().Substring(0, inputEvent.AsText().Find(" ("));
				inputValue += inputValue.Contains(_customValues[joystickInput]) ? string.Empty : $"{_customValues[joystickInput]} ";
				continue;
			}
		}

		inputValue.Trim();

		if (inputs == "game_right")
		{
			inputValue += (refs.settings.ActiveController == InputType.keyboard) ? $"/ {_customValues["Mouse"]}" : $"/ {_customValues["Joypad Motion on Axis 0"]}";
		}

		return inputValue;
	}

	private string GetCustomValue(string key)
	{
		try
		{
			return _customValues[key];
		}
		catch
		{
			GD.PrintErr("Replacement value not found");
		}

		return "<value not found>";
	}
}
