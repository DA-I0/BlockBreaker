using System.Collections.Generic;
using Godot;

public delegate void Notification();
public delegate void SceneInfo(string sceneName);

namespace BoGK.GameSystem
{
	public partial class LevelManager : Node
	{
		private Node _currentScene;
		private Node _loadingScreen;

		private List<string> _sessionLevels = new List<string>();

		public event Notification ResetSession;
		public event Notification SceneChanged;
		public event SceneInfo SceneChangedWithInfo;

		private SessionController refs;

		public override void _Ready()
		{
			SetupReferences();
			_loadingScreen = ResourceLoader.Load<PackedScene>("res://prefabs/ui/loading_screen.tscn").Instantiate();
		}

		public void LoadMenuScene()
		{
			string scenePath = $"res://scenes/menus/menu.tscn";
			AddChild(_loadingScreen);
			ClearCurrentScene(_currentScene);
			SetupNewScene(scenePath);
			ResetSession?.Invoke();
		}

		public void LoadGameScene(string sceneName)
		{
			string scenePath = $"res://scenes/levels/{sceneName}";
			AddChild(_loadingScreen);
			ClearCurrentScene(_currentScene);
			SetupNewScene(scenePath);
		}

		public void SetupSessionLevels(int startingIndex, int sessionLength)
		{
			_sessionLevels.Clear();

			int remainingLength = refs.gameData.Levels.Count - (startingIndex + sessionLength);

			if (sessionLength < 0)
			{
				sessionLength = refs.gameData.Levels.Count - startingIndex;
			}

			if (remainingLength >= 0)
			{
				_sessionLevels = refs.gameData.Levels.GetRange(startingIndex, sessionLength);
			}
			else
			{
				_sessionLevels = refs.gameData.Levels.GetRange(startingIndex, refs.gameData.Levels.Count - startingIndex);
				_sessionLevels.AddRange(refs.gameData.Levels.GetRange(0, -remainingLength));
			}

			ShuffleStageList();
		}

		public bool SelectSessionLevel(int index)
		{
			if (index >= _sessionLevels.Count || index < 0)
			{
				return false;
			}

			LoadGameScene(_sessionLevels[index]);
			return true;
		}

		private void SetupReferences()
		{
			_currentScene = GetNode("../CurrentScene");
			_currentScene.ChildEnteredTree += (Node scene) => CallDeferred("FinalizeStageSetup", scene);

			refs = (SessionController)GetParent();
		}

		private void ClearCurrentScene(Node parentNode)
		{
			if (parentNode.GetChildCount() > 0)
			{
				foreach (Node scene in parentNode.GetChildren())
				{
					scene.QueueFree();
				}
			}
		}

		private void SetupNewScene(string scenePath)
		{
			scenePath = scenePath.TrimSuffix(".remap");

			ResourceLoader.LoadThreadedRequest(scenePath);
			Node newScene = (ResourceLoader.LoadThreadedGet(scenePath) as PackedScene).Instantiate();

			_currentScene.CallDeferred("add_child", newScene);
		}

		private void FinalizeStageSetup(Node targetScene)
		{
			if (targetScene.IsInGroup("levels"))
			{
				ApplyTilesetSettings(targetScene.GetNode<TileMap>("TileMap"));
				ApplySpritePropSettings(targetScene.GetNode<Node2D>("MapVisuals"));
				ApplyStaticPropSettings(targetScene);
				SceneChanged?.Invoke();
				SceneChangedWithInfo?.Invoke(targetScene.SceneFilePath.Split("/")[^1]);
			}
		}

		private void ApplyTilesetSettings(TileMap mapBackground)
		{
			string tileSetPath = mapBackground.TileSet.ResourcePath;
			tileSetPath = tileSetPath.Replace(".tres", $"_{refs.settings.BackgroundColorPalette}.tres");

			if (FileAccess.FileExists(tileSetPath))
			{
				mapBackground.TileSet = ResourceLoader.Load<TileSet>(tileSetPath);
			}

			mapBackground.Modulate = new Color(refs.settings.BackgroundBrightness, refs.settings.BackgroundBrightness, refs.settings.BackgroundBrightness, 1f);
		}

		private void ApplySpritePropSettings(Node2D mapDetails)
		{
			if (mapDetails == null)
			{
				return;
			}

			mapDetails.Modulate = new Color(refs.settings.BackgroundBrightness, refs.settings.BackgroundBrightness, refs.settings.BackgroundBrightness, 1f);

			foreach (Sprite2D sprite in mapDetails.GetChildren())
			{
				sprite.Texture = ResourceLoader.Load<Texture2D>(HelperMethods.ReplaceSpritePalettePath(sprite.Texture.ResourcePath, refs.settings.BackgroundColorPalette));
			}
		}

		private void ApplyStaticPropSettings(Node targetScene)
		{
			Godot.Collections.Array<Node> staticProps = targetScene.GetChild(0).GetTree().GetNodesInGroup("static_props");
			Color backgroundBrightness = new Color(refs.settings.BackgroundBrightness, refs.settings.BackgroundBrightness, refs.settings.BackgroundBrightness, 1f);

			foreach (Sprite2D prop in staticProps)
			{
				prop.Texture = ResourceLoader.Load<Texture2D>(HelperMethods.ReplaceSpritePalettePath(prop.Texture.ResourcePath, refs.settings.BackgroundColorPalette));
				prop.Modulate = backgroundBrightness;
			}
		}

		private void OnCurrentSceneChildEnteredTree(Node node)
		{
			if (GetChildCount() < 1)
			{
				return;
			}

			RemoveChild(_loadingScreen);
		}

		private void ShuffleStageList()
		{
			if (refs.ShuffleStages)
			{
				System.Random randomizer = new System.Random();

				for (int index = _sessionLevels.Count - 1; index > 0; index--)
				{
					int randomIndex = randomizer.Next(index + 1);
					string value = _sessionLevels[randomIndex];
					_sessionLevels[randomIndex] = _sessionLevels[index];
					_sessionLevels[index] = value;
				}
			}
		}
	}
}