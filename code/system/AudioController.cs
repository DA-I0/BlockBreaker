using Godot;

namespace BoGK.GameSystem
{
	public partial class AudioController : AudioStreamPlayer
	{
		[Export] protected AudioStream[] _clips;

		public void PlayAudio(int index)
		{
			Stream = _clips[index];
			Play();
		}
	}
}