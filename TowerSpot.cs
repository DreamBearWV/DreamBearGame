using Godot;

public partial class TowerSpot : Button
{
    [Export] public PackedScene TowerScene;
    [Export] public int TowerCost = 50;

    private Tower _builtTower = null;

    public override void _Ready()
    {
        Pressed += OnButtonPressed;
    }

    public override void _Process(double delta)
    {
        // 每幀檢查滑鼠是否懸停在按鈕上，100% 穩定顯示或隱藏範圍圈
        if (_builtTower != null && GodotObject.IsInstanceValid(_builtTower))
        {
            _builtTower.SetRangeVisible(IsHovered());
        }
    }

    private void OnButtonPressed()
    {
        // 已有塔：嘗試升級
        if (_builtTower != null && GodotObject.IsInstanceValid(_builtTower))
        {
            if (_builtTower.Level < _builtTower.MaxLevel)
            {
                if (GameManager.Instance != null && GameManager.Instance.SpendMoney(_builtTower.UpgradeCost))
                {
                    _builtTower.Upgrade();
                }
                else
                {
                    GD.Print("❌ 金錢不足，無法升級防禦塔！");
                }
            }
            else
            {
                GD.Print("⭐ 此防禦塔已達到最高等級 (LV2)！");
            }
            return;
        }

        // 空平台：建造 LV1 塔
        if (TowerScene == null)
        {
            GD.Print("❌ 報錯：TowerScene 是空的！");
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.SpendMoney(TowerCost))
        {
            _builtTower = TowerScene.Instantiate<Tower>();
            _builtTower.Position = Size / 2;
            AddChild(_builtTower);
			Flat = true; // 隱藏預設的灰色背景框
            GD.Print($"🏗️ 花費 {TowerCost} 金錢成功建造 LV1 防禦塔！");
        }
        else
        {
            GD.Print("❌ 金錢不足，無法建造防禦塔！");
        }
    }
}