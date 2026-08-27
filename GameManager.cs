using Godot;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }

    [Export] public int Health = 10;
    [Export] public int Money = 100;

    [Export] public Label HealthLabel;
    [Export] public Label MoneyLabel;
    [Export] public Label GameOverLabel;
    [Export] public Label WinLabel; // 新增：勝利宣告文字

    private bool _isGameOver = false;

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always; // 確保暫停時依然能接收 R 鍵重開

        UpdateUI();

        if (GameOverLabel != null) GameOverLabel.Visible = false;
        if (WinLabel != null) WinLabel.Visible = false;
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void TakeDamage(int amount)
    {
        if (_isGameOver) return;

        Health -= amount;
        if (Health < 0) Health = 0;
        UpdateUI();

        if (Health <= 0)
        {
            _isGameOver = true;
            GD.Print("💀 遊戲結束！玩家基地被摧毀！");

            if (GameOverLabel != null)
            {
                GameOverLabel.Text = "💀 GAME OVER 💀\n按下 [ R ] 重新開始遊戲";
                GameOverLabel.Visible = true;
            }

            GetTree().Paused = true; // 凍結遊戲
        }
    }

    // 🏆 新增：全波次清除後的勝利方法
    public void WinGame()
    {
        if (_isGameOver) return;

        _isGameOver = true;
        GD.Print("🎉 恭喜通關！成功防守所有敵人波次！");

        if (WinLabel != null)
        {
            WinLabel.Text = "🎉 YOU WIN! 勝利通關！ 🎉\n按下 [ R ] 重新玩一次";
            WinLabel.Visible = true;
        }
        else if (GameOverLabel != null) // 如果沒綁定 WinLabel，借用 GameOverLabel 顯示
        {
            GameOverLabel.Text = "🎉 YOU WIN! 勝利通关！ 🎉\n按下 [ R ] 重新玩一次";
            GameOverLabel.Visible = true;
        }

        GetTree().Paused = true; // 凍結遊戲，歡慶勝利
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_isGameOver && @event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.R)
        {
            GetTree().Paused = false;
            GetTree().ReloadCurrentScene();
        }
    }

    private void UpdateUI()
    {
        if (HealthLabel != null) HealthLabel.Text = $"❤️ 血量: {Health}";
        if (MoneyLabel != null) MoneyLabel.Text = $"💰 金錢: {Money}";
    }
}