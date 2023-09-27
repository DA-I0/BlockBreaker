using Godot;

public delegate void Notification();

public partial class LevelManager : Node
{
	private Node uiNode;
	private Node currentScene;
	private Node loadingScreen;
	public event Notification ResetSession;
	public event Notification SceneChanged;

	public override void _Ready()
	{
		uiNode = GetNode("../UI");
		currentScene = GetNode("../CurrentScene");
		loadingScreen = ResourceLoader.Load<PackedScene>("res://prefabs/ui/loading_screen.tscn").Instantiate();
		((SessionController)GetParent().GetChild(0)).health.GameOver += LoadMenuScene;
		LoadMenuScene();
	}

	public void LoadMenuScene()
	{
		string scenePath = $"res://scenes/menu.tscn";
		AddChild(loadingScreen);
		ClearCurrentScene(uiNode);
		ClearCurrentScene(currentScene);
		SetupNewScene(scenePath);
		ResetSession?.Invoke();
	}

	public void LoadGameScene(string sceneName)
	{
		string scenePath = $"res://scenes/levels/{sceneName}";
		AddChild(loadingScreen);
		ClearCurrentScene(uiNode);
		ClearCurrentScene(currentScene);
		SetupNewScene(scenePath);
		SceneChanged?.Invoke();
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
		ResourceLoader.LoadThreadedRequest(scenePath);
		Node newScene = (ResourceLoader.LoadThreadedGet(scenePath) as PackedScene).Instantiate();

		if (newScene.Name == "Menu")
		{
			uiNode.CallDeferred("add_child", newScene);
		}
		else
		{
			currentScene.CallDeferred("add_child", newScene);
			// ((Control)newScene).Position = new Vector2(-128, -112);
		}

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