using Godot;

public partial class SpeedButton : Button
{
    private bool _isFastSpeed = false;

    public override void _Ready()
    {
        Text = "⏩ 1x 速度";
        Pressed += OnButtonPressed;
    }

    private void OnButtonPressed()
    {
        _isFastSpeed = !_isFastSpeed;

        if (_isFastSpeed)
        {
            Engine.TimeScale = 2.0f; // 2 倍速運行全遊戲！
            Text = "⚡ 2x 速度";
            GD.Print("⏩ 切換至 2 倍速！");
        }
        else
        {
            Engine.TimeScale = 1.0f; // 恢復正常速度
            Text = "⏩ 1x 速度";
            GD.Print("▶️ 切換至 1 倍速！");
        }
    }
}