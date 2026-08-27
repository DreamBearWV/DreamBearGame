using Godot;

public partial class Bullet : Node2D
{
    [Export] public float Speed = 500.0f; // 子彈飛行速度

    private Enemy _target;
    private int _damage;

    // 設置攻擊目標與傷害
    public void Seek(Enemy target, int damage)
    {
        _target = target;
        _damage = damage;
    }

    public override void _Process(double delta)
    {
        // 如果目標已經死亡或消失，子彈自動自我銷毀
        if (!GodotObject.IsInstanceValid(_target))
        {
            QueueFree();
            return;
        }

        // 朝著目標位置飛行
        Vector2 direction = (_target.GlobalPosition - GlobalPosition).Normalized();
        GlobalPosition += direction * Speed * (float)delta;

        // 當距離目標小於 10 像素，視為命中！
        if (GlobalPosition.DistanceTo(_target.GlobalPosition) < 10.0f)
        {
            _target.TakeDamage(_damage);
            QueueFree(); // 擊中後銷毀子彈
        }
    }
}