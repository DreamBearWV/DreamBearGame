using Godot;

public partial class Tower : Node2D
{
    [Export] public float AttackRange = 150.0f;
    [Export] public int Damage = 10;
    [Export] public float FireRate = 1.0f;

    [Export] public int Level = 1;
    [Export] public int MaxLevel = 2;
    [Export] public int UpgradeCost = 75;

    [Export] public PackedScene BulletScene;

    private double _timeSinceLastAttack = 0.0;
    private bool _isRangeVisible = false;

    public override void _Process(double delta)
    {
        _timeSinceLastAttack += delta;

        if (_timeSinceLastAttack >= FireRate)
        {
            Enemy target = FindClosestEnemy();
            if (target != null)
            {
                Attack(target);
                _timeSinceLastAttack = 0.0;
            }
        }
    }

    public void SetRangeVisible(bool visible)
    {
        if (_isRangeVisible != visible)
        {
            _isRangeVisible = visible;
            QueueRedraw(); // 觸發 _Draw() 重新繪製
        }
    }

    public override void _Draw()
    {
        if (_isRangeVisible)
        {
            // 1. 畫半透明天藍色填滿圓圈
            DrawCircle(Vector2.Zero, AttackRange, new Color(0.2f, 0.6f, 1.0f, 0.2f));
            // 2. 畫天藍色外框
            DrawArc(Vector2.Zero, AttackRange, 0, Mathf.Tau, 64, new Color(0.2f, 0.6f, 1.0f, 0.8f), 2.0f);
        }
    }

    public bool Upgrade()
    {
        if (Level >= MaxLevel) return false;

        Level++;
        Damage += 15;
        AttackRange += 50.0f;
        FireRate *= 0.7f;

        Scale = new Vector2(1.3f, 1.3f);
        QueueRedraw();

        GD.Print($"⚡ 防禦塔升級成功！當前等級: LV{Level}");
        return true;
    }

    private Enemy FindClosestEnemy()
    {
        var enemies = GetTree().GetNodesInGroup("Enemies");
        Enemy closest = null;
        float minDistance = AttackRange;

        foreach (Node node in enemies)
        {
            if (node is Enemy enemy && GodotObject.IsInstanceValid(enemy))
            {
                float dist = GlobalPosition.DistanceTo(enemy.GlobalPosition);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = enemy;
                }
            }
        }
        return closest;
    }

    private void Attack(Enemy enemy)
    {
        if (BulletScene == null)
        {
            enemy.TakeDamage(Damage);
            return;
        }

        var bullet = BulletScene.Instantiate<Bullet>();
        GetTree().CurrentScene.AddChild(bullet);
        bullet.GlobalPosition = GlobalPosition;
        bullet.Seek(enemy, Damage);
    }
}