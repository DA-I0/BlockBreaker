public struct FontVariant
{
	public string FontName;
	public int DefaultSize;

	public FontVariant(string name, int size)
	{
		FontName = name;
		DefaultSize = size;
	}
}