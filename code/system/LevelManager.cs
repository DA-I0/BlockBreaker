using Godot;

public delegate void Notification();

public partial class LevelManager : Node
{
	private Node currentScene;
	private Node loadingScreen;
	public event Notification ResetSession;
	public event Notification SceneChanged;

	private SessionController refs;

	public override void _Ready()
	{
		SetupReferences();
		loadingScreen = ResourceLoader.Load<PackedScene>("res://prefabs/ui/loading_screen.tscn").Instantiate();
	}

	public void LoadMenuScene()
	{
		string scenePath = $"res://scenes/menus/menu.tscn";
		AddChild(loadingScreen);
		ClearCurrentScene(currentScene);
		SetupNewScene(scenePath);
		ResetSession?.Invoke();
	}

	public void LoadGameScene(string sceneName)
	{
		string scenePath = $"res://scenes/levels/{sceneName}";
		AddChild(loadingScreen);
		ClearCurrentScene(currentScene);
		SetupNewScene(scenePath);
		SceneChanged?.Invoke();
	}

	private void SetupReferences()
	{
		currentScene = GetNode("../CurrentScene");

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

		if (!scenePath.Contains("menu"))
		{
			Node2D mapBackground = (Node2D)newScene.GetChild(0);
			mapBackground.Modulate = new Color(mapBackground.Modulate.R, mapBackground.Modulate.G, mapBackground.Modulate.B, refs.settings.BackgroundBrightness);
		}

		currentScene.CallDeferred("add_child", newScene);
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
