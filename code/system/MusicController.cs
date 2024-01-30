using System;
using Godot;

public partial class MusicController : AudioController
{
	[Export] private AudioStream _menuSong;

	private int _playlistPosition = 0;

	public override void _Ready()
	{
		SessionController tempRef = (SessionController)GetParent();
		tempRef.levelManager.ResetSession += PlayMenuMusic;
		tempRef.levelManager.ResetSession += ResetPlaylist;
		tempRef.levelManager.SceneChanged += PlayNextTrack;
	}

	public void PlayNextTrack()
	{
		if (_playlistPosition >= _clips.Length - 1)
		{
			ResetPlaylist();
		}

		_playlistPosition += 1;
		PlayAudio(_playlistPosition);
	}

	public void PlayRandomTrack()
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

	private void ResetPlaylist()
	{
		ShuffleStageMusic(_clips);
		_playlistPosition = -1;
	}

	private void ShuffleStageMusic(AudioStream[] targetArray)
	{
		Random randomizer = new Random();

		for (int index = targetArray.Length - 1; index > 0; index--)
		{
			int randomIndex = randomizer.Next(index + 1);
			AudioStream value = targetArray[randomIndex];
			targetArray[randomIndex] = targetArray[index];
			targetArray[index] = value;
		}

		_clips = targetArray;
	}
}
