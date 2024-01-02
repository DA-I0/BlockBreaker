using Godot;

namespace BoGK.UI
{
	public partial class UIPaginatorPanel : UIPanel
	{
		[Export] private Node _paginatorParent;
		[Export] private Texture2D _inactiveItemSprite;
		[Export] private Texture2D _activeItemSprite;

		protected void CreateItemIndicators(int itemCount)
		{
			ClearItemIndicators();

			for (int i = 0; i < itemCount; i++)
			{
				TextureRect itemIndicator = new TextureRect
				{
					Texture = _inactiveItemSprite,
					StretchMode = TextureRect.StretchModeEnum.KeepAspect
				};
				_paginatorParent.AddChild(itemIndicator);
			}
		}

		protected void UpdatePaginatorStatus(int activeIndex)
		{
			for (int index = 0; index < _paginatorParent.GetChildCount(); index++)
			{
				if (index == activeIndex)
				{
					((TextureRect)_paginatorParent.GetChild(index)).Texture = _activeItemSprite;
				}
				else
				{
					((TextureRect)_paginatorParent.GetChild(index)).Texture = _inactiveItemSprite;
				}
			}
		}

		private void ClearItemIndicators()
		{
			if (_paginatorParent.GetChildCount() < 1)
			{
				return;
			}

			for (int index = _paginatorParent.GetChildCount() - 1; index >= 0; index--)
			{
				Node child = _paginatorParent.GetChild(index);
				_paginatorParent.RemoveChild(child);
				child.QueueFree();
			}
		}
	}
}