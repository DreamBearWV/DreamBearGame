using Godot;

public partial class Enemy : Node2D
{
    [Export] public int MaxHealth = 10;
    [Export] public float Speed = 100.0f;
    [Export] public int RewardMoney = 15;
    [Export] public int DamageToPlayer = 1;

    [Export] public ProgressBar HealthBar; // 綁定頭頂血條

    private int _currentHealth;
    private PathFollow2D _pathFollow;

    public override void _Ready()
    {
        AddToGroup("Enemies");

        _currentHealth = MaxHealth;
        _pathFollow = GetParent<PathFollow2D>();

        if (_pathFollow != null)
        {
            _pathFollow.Loop = false;
        }

        // 初始化血條最大值與當前值
        if (HealthBar != null)
        {
            HealthBar.MaxValue = MaxHealth;
            HealthBar.Value = _currentHealth;
        }
    }

    public override void _Process(double delta)
    {
        if (_pathFollow != null)
        {
            _pathFollow.Progress += Speed * (float)delta;

            if (_pathFollow.ProgressRatio >= 0.99f)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.TakeDamage(DamageToPlayer);
                }
                _pathFollow.QueueFree();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        // 即時扣除血條長度
        if (HealthBar != null)
        {
            HealthBar.Value = _currentHealth;
        }

        if (_currentHealth <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddMoney(RewardMoney);
            }

            if (_pathFollow != null)
            {
                _pathFollow.QueueFree();
            }
            else
            {
                QueueFree();
            }
        }
    }
}