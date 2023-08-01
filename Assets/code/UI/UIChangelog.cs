using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChangelog : MonoBehaviour
{
	[SerializeField] private const string NewLineMarker = "---";
	[SerializeField] private TMP_Text _patchnotes;
	[SerializeField] private Scrollbar _scrollbar;

	public void UpdateUI()
	{
		_patchnotes.text = ConvertPatchnotes(GameObject.Find("_system").GetComponent<Settings>().gameData.patchNotes);
		_scrollbar.value = 1f;
	}

	private string ConvertPatchnotes(string content)
	{
		string modifiedContent;

		using (StringReader reader = new StringReader(content))
		{
			modifiedContent = $"<b><u>{reader.ReadLine()}</u></b>";
			string nextLine = reader.ReadLine();

			while (!string.IsNullOrEmpty(nextLine))
			{
				modifiedContent += $"<br>{nextLine.Replace(NewLineMarker, "")}";
				nextLine = reader.ReadLine();
			}
		}

		return modifiedContent;
	}
}
