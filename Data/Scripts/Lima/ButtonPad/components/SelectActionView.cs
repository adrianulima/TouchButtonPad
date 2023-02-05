using Lima.API;
using Lima.ButtonPad;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI;
using VRageMath;

namespace Lima
{
  public class SelectActionView : TouchScrollView
  {
    private ButtonPadApp _padApp;
    private List<ITerminalAction> _terminalActions = new List<ITerminalAction>();
    private List<TouchButton> _buttons = new List<TouchButton>();

    public SelectActionView(ButtonPadApp pad)
    {
      _padApp = pad;

      ScrollBar.Pixels = new Vector2(24, 0);
      Gap = 2;
    }

    public void Dispose()
    {
      _terminalActions.Clear();
      _buttons.Clear();
      _terminalActions = null;
      _buttons = null;
    }

    public void UpdateItemsForButton(ActionButton actionBt, IMyBlockGroup blockGroup)
    {
      _terminalActions.Clear();
      Utils.GetBlockGroupActions(blockGroup, _terminalActions);

      RemoveAllChildren();
      _buttons.Clear();
      foreach (var act in _terminalActions)
      {
        var bt = new TouchButton(act.Name.ToString(), () => SelectGroupAction(blockGroup, actionBt, act));
        AddButton(bt);
      }
    }

    public void UpdateItemsForButton(ActionButton actionBt, IMyTerminalBlock block)
    {
      _terminalActions.Clear();
      block.GetActions(_terminalActions, (a) => a.IsEnabled(block));

      RemoveAllChildren();
      _buttons.Clear();
      foreach (var act in _terminalActions)
      {
        var bt = new TouchButton(act.Name.ToString(), () => SelectAction(block, actionBt, act));
        AddButton(bt);
      }
    }

    private void AddButton(TouchButton button)
    {
      var height = _padApp.Screen.Surface.SurfaceSize.Y;
      var smallHeight = height < 128;
      var smallWidth = _padApp.Screen.Surface.SurfaceSize.X <= 128;

      var h = smallWidth ? 68f : 36f;
      if (smallHeight)
        h = ((height / 2) - 2) / _padApp.Theme.Scale;

      ScrollBar.Pixels = new Vector2(smallWidth && !smallHeight ? 8 : 24, 0);
      button.Padding = new Vector4(4);
      button.Label.Alignment = smallWidth ? TextAlignment.CENTER : TextAlignment.LEFT;
      button.Label.FontSize = smallWidth ? 0.5f : 0.8f;
      button.Label.AutoBreakLine = true;
      button.Pixels = new Vector2(0, h);
      button.Scale = new Vector2(1, 0);
      AddChild(button);
      _buttons.Add(button);

      ScrollWheelStep = (button.Pixels.Y + Gap) * _padApp.Theme.Scale;
    }

    public void RemoveAllChildren()
    {
      Scroll = 0;
      // TODO: Pool
      foreach (var bt in _buttons)
        RemoveChild(bt);
      _buttons.Clear();
    }

    private void SelectGroupAction(IMyBlockGroup blockGroup, ActionButton actionBt, ITerminalAction action)
    {
      actionBt.SetAction(blockGroup, action);
      _padApp.SelectActionConfirm();
    }

    private void SelectAction(IMyCubeBlock block, ActionButton actionBt, ITerminalAction action)
    {
      actionBt.SetAction(block, action);
      _padApp.SelectActionConfirm();
    }
  }
}