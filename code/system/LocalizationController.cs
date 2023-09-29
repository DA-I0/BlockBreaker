using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class LocalizationController : Node
{
	private InputType _controller = InputType.keyboard;

	Dictionary<string, string> _customValues = new Dictionary<string, string>()
	{
		{ "Mouse", "⑴" },
		{ "Left Mouse Button", "⑵" },
		{ "Right Mouse Button", "⑶" },
		{ "1", "①" },
		{ "2", "②" },
		{ "3", "③" },
		{ "4", "④" },
		{ "5", "⑤" },
		{ "6", "⑥" },
		{ "7", "⑦" },
		{ "8", "⑧" },
		{ "9", "⑨" },
		{ "0", "⓪" },
		{ "A", "Ⓐ" },
		{ "B", "Ⓑ" },
		{ "C", "Ⓒ" },
		{ "D", "Ⓓ" },
		{ "E", "Ⓔ" },
		{ "F", "Ⓕ" },
		{ "G", "Ⓖ" },
		{ "H", "Ⓗ" },
		{ "I", "Ⓘ" },
		{ "J", "Ⓙ" },
		{ "K", "Ⓚ" },
		{ "L", "Ⓛ" },
		{ "M", "Ⓜ" },
		{ "N", "Ⓝ" },
		{ "O", "Ⓞ" },
		{ "P", "Ⓟ" },
		{ "Q", "Ⓠ" },
		{ "R", "Ⓡ" },
		{ "S", "Ⓢ" },
		{ "T", "Ⓣ" },
		{ "U", "Ⓤ" },
		{ "V", "Ⓥ" },
		{ "W", "Ⓦ" },
		{ "X", "Ⓧ" },
		{ "Y", "Ⓨ" },
		{ "Z", "Ⓩ" },
		{ "Alt", "⒃" },
		{ "Ctrl", "⒄" },
		{ "Enter", "⒅" },
		{ "Escape", "⒆" },
		{ "Shift", "⒂" },
		{ "Space", "⒁" },
		{ "Tab", "⒇" },
		{ "Down", "⒞" },
		{ "Left", "⒟" },
		{ "Right", "⒠" },
		{ "Up", "⒝" },
		{ "Joypad Axis 0 -", "L⒯" },
		{ "Joypad Motion on Axis 0", "⒯⒰" },
		{ "Joypad Axis 0 +", "L⒰" },
		{ "Joypad Button 0", "⒨" }, // x, Xa, Nb
		{ "Joypad Button 1", "⒪" }, // o, xB, Na
		{ "Joypad Button 2", "⒩" }, // [], Xx, Ny
		{ "Joypad Button 3", "⒧" }, // ^, Xy, Nx
		{ "Joypad Button 4", "⒱" }, // select, Xback, N-
		{ "Joypad Button 5", "⓪" }, // PS, Xhome
		{ "Joypad Button 6", "⒲" }, // start, N+
		{ "Joypad Button 7", "L⒬" }, // left stick
		{ "Joypad Button 8", "R⒬" }, // right stick
		{ "Joypad Button 9", "⒒" }, // left shoulder
		{ "Joypad Button 10", "⒓" }, // right shoulder
		{ "Joypad Button 11", "⒢" }, // d-pad up
		{ "Joypad Button 12", "⒣" }, // d-pad down
		{ "Joypad Button 13", "⒤" }, // d-pad left
		{ "Joypad Button 14", "⒥" }, // d-pad right
		{ "block_health", "1" }, //
		{ "sturdy_block_health", "3" }, //
		{ "sturdy_barrier_health", "3" }, //
		{ "multiplier_combo_step", "10" }, //
		{ "multiplier_max", "9" }, //
		{ "multiplier_max_combo", "5" }, //
	};

	private SessionController refs;

	public override void _Ready()
	{
		refs = GetNode("/root/GameController/SessionController") as SessionController;
		((UIController)GetParent()).RefreshUI += FindLabelsToTranslate;
		FindLabelsToTranslate();
	}

	private void FindLabelsToTranslate()
	{
		List<Node> nodes = GetTree().GetNodesInGroup("manual_translation").ToList();

		foreach (Node node in nodes)
		{
			TranslateContent((RichTextLabel)node);
		}
	}

	private void TranslateContent(RichTextLabel label)
	{
		string rawLocalization = Tr(label.Name);
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

	public string GetInputValues(string inputs)
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

	public string GetCustomValue(string key)
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
