using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

public class FileOperations
{
	private Settings _settings;

	public FileOperations(Settings settings)
	{
		_settings = settings;
	}

	public void LoadSettings()
	{
		if (File.Exists(_settings.ConfigFilePath))
		{
			string configFile = File.ReadAllText(_settings.ConfigFilePath);
			StringReader reader = new StringReader(configFile);

			System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SettingsFile));
			SettingsFile data = (SettingsFile)serializer.Deserialize(reader);
			reader.Close();
			_settings.ImportSettings(data);
		}
	}

	public void SaveSettings()
	{
		FileStream fileStream;
		fileStream = new FileStream(_settings.ConfigFilePath, FileMode.Create);

		System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SettingsFile));
		serializer.Serialize(fileStream, _settings.ExportSettings());

		fileStream.Close();
	}
}
