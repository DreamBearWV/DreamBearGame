using Godot;

public partial class EnemySpawner : Node2D
{
    [Export] public PackedScene EnemyScene;      // 敵人場景 (Enemy.tscn)
    [Export] public NodePath Path2DNodePath;     // 指向 Main 裡的 Path2D 節點
    [Export] public float SpawnInterval = 1.5f;  // 生成間隔（每幾秒生一隻）

    private Timer _spawnTimer;
    private Path2D _path2D;

    public override void _Ready()
    {
        // 取得 Main 場景裡的 Path2D 節點
        if (Path2DNodePath != null)
        {
            _path2D = GetNode<Path2D>(Path2DNodePath);
        }

        // 建立計時器來控制生成節奏
        _spawnTimer = new Timer();
        _spawnTimer.WaitTime = SpawnInterval;
        _spawnTimer.Timeout += SpawnEnemy;
        AddChild(_spawnTimer);
        _spawnTimer.Start();
    }

    private void SpawnEnemy()
    {
        if (EnemyScene == null || _path2D == null) return;

        // 1. 動態建立 PathFollow2D 節點並掛在 Path2D 底下
        var pathFollow = new PathFollow2D();
        pathFollow.Loop = false; // 確保不會循環
        _path2D.AddChild(pathFollow);

        // 2. 將 Enemy 場景實例化並掛在 PathFollow2D 底下
        var enemy = EnemyScene.Instantiate<Node2D>();
        pathFollow.AddChild(enemy);
    }
}