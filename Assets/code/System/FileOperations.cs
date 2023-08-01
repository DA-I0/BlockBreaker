using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

public class FileOperations
{
	private Settings _settings;

	public FileOperations(Settings settings)
	{
		_settings = settings;
		_settings.gameData.patchNotes = LoadTextFile(_settings.gameData.ChangelogFilePath);
	}

	private string LoadTextFile(string filePath)
	{
		string fileContent = "<file_not_found>";

		if (File.Exists(filePath))
		{
			fileContent = File.ReadAllText(filePath);
		}

		return fileContent;
	}
}
