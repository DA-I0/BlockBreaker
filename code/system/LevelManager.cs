using Godot;

public delegate void Notification();

public partial class LevelManager : Node
{
	private Node currentScene;
	private Node loadingScreen;
	public event Notification ResetSession;
	public event Notification SceneChanged;

	public override void _Ready()
	{
		currentScene = GetNode("../CurrentScene");
		loadingScreen = ResourceLoader.Load<PackedScene>("res://prefabs/ui/loading_screen.tscn").Instantiate();
		((SessionController)GetParent().GetChild(0)).health.GameOver += LoadMenuScene;
		LoadMenuScene();
	}

	public void LoadMenuScene()
	{
		string scenePath = $"res://scenes/menu.tscn";
		AddChild(loadingScreen);
		ClearCurrentScene();
		SetupNewScene(scenePath);
		ResetSession?.Invoke();
	}

	public void LoadGameScene(string sceneName)
	{
		string scenePath = $"res://scenes/levels/{sceneName}";
		AddChild(loadingScreen);
		ClearCurrentScene();
		SetupNewScene(scenePath);
		SceneChanged?.Invoke();
	}

	public void LoadNextLevel()
	{
		// if (refsgameData.Levels.Count - 1 < _currentLevel)
		// {
		// 	levelManager.LoadNextLevel();
		// }
		// check if there are any further levels available and if not load the main menu
		// LoadMenuScene();
		LoadGameScene("level_02");
	}

	private void ClearCurrentScene()
	{
		if (currentScene.GetChildCount() > 0)
		{
			foreach (Node scene in currentScene.GetChildren())
			{
				scene.QueueFree();
			}
		}
	}

	private void SetupNewScene(string scenePath)
	{
		ResourceLoader.LoadThreadedRequest(scenePath);
		Node newScene = (ResourceLoader.LoadThreadedGet(scenePath) as PackedScene).Instantiate();
		currentScene.CallDeferred("add_child", newScene);

		Input.MouseMode = (newScene.Name == "Menu") ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
	}

	private void OnCurrentSceneChildEnteredTree(Node node)
	{
		if (GetChildCount() < 1)
		{
			return;
		}

		RemoveChild(loadingScreen);
	}
}
