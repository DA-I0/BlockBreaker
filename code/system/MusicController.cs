using Godot;

public partial class MusicController : AudioController
{
	[Export] private AudioStream _menuSong;

	public override void _Ready()
	{
		SessionController tempRef = (SessionController)GetParent().GetChild(0);
		tempRef.levelManager.ResetSession += PlayMenuMusic;
		tempRef.levelManager.SceneChanged += PlayRandomAudio;
	}

	public void PlayRandomAudio()
	{
		int index = GD.RandRange(0, _clips.Length - 1);
		PlayAudio(index);
	}

	public void PlayMenuMusic()
	{
		PlayAudio(_menuSong);
	}

	public void PlayAudio(AudioStream clip)
	{
		Stream = clip;
		Play();
	}
}
