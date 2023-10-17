using Godot;

public delegate void Notification();

public partial class LevelManager : Node
{
	private Node uiNode;
	private Node currentScene;
	private Node loadingScreen;
	public event Notification ResetSession;
	public event Notification SceneChanged;

	SessionController refs;

	public override void _Ready()
	{
		SetupReferences();
		loadingScreen = ResourceLoader.Load<PackedScene>("res://prefabs/ui/loading_screen.tscn").Instantiate();
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

	private void SetupReferences()
	{
		uiNode = GetNode("../UI");
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
		ResourceLoader.LoadThreadedRequest(scenePath);
		Node newScene = (ResourceLoader.LoadThreadedGet(scenePath) as PackedScene).Instantiate();

		if (!scenePath.Contains("menu"))
		{
			Node2D mapBackground = (Node2D)newScene.GetChild(0);
			mapBackground.Modulate = new Color(mapBackground.Modulate.R, mapBackground.Modulate.G, mapBackground.Modulate.B, refs.settings.BackgroundBrightness);
			Node2D deathPitAnimation = (Node2D)newScene.GetChild(2);
			deathPitAnimation.Modulate = new Color(deathPitAnimation.Modulate.R, deathPitAnimation.Modulate.G, deathPitAnimation.Modulate.B, refs.settings.HelperTransparency);
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
