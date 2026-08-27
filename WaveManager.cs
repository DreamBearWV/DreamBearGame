using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class WaveManager : Node
{
    [System.Serializable]
    public partial class WaveData : GodotObject
    {
        [Export] public PackedScene EnemyScene;
        [Export] public int Count = 5;
        [Export] public float SpawnInterval = 1.0f;
    }

    [Export] public Path2D EnemyPath;
    [Export] public Godot.Collections.Array<WaveData> Waves;
    [Export] public float TimeBetweenWaves = 5.0f;
    [Export] public Label WaveLabel; // 綁定波次 UI 顯示

    // 新增：快怪與 Boss 場景 (用於無盡模式動態生成)
    [Export] public PackedScene NormalEnemyScene;
    [Export] public PackedScene FastEnemyScene;
    [Export] public PackedScene BossEnemyScene;

    private int _currentWaveIndex = 0;
    private bool _isSpawningWave = false;

    public override void _Ready()
    {
        StartNextWave();
    }

    public override void _Process(double delta)
    {
        if (_isSpawningWave) return;

        // 當場上沒有任何活著的敵人時，觸發下一波
        var enemies = GetTree().GetNodesInGroup("Enemies");
        if (enemies.Count == 0)
        {
            _isSpawningWave = true;
            _currentWaveIndex++;

            // 倒數計時進入下一波
            GetTree().CreateTimer(TimeBetweenWaves).Timeout += () => StartNextWave();
        }
    }

    private void StartNextWave()
    {
        _isSpawningWave = true;

        // 判斷是否為「前 3 波標準波次」
        if (_currentWaveIndex < Waves.Count)
        {
            UpdateWaveUI($"🌊 波次: {_currentWaveIndex + 1} / {Waves.Count}");
            StartCoroutine(SpawnWaveRoutine(Waves[_currentWaveIndex]));
        }
        else
        {
            // 🔥 解鎖無限波次模式！
            int endlessWaveNum = _currentWaveIndex + 1;
            UpdateWaveUI($"🔥 無限模式: 第 {endlessWaveNum} 波");
            GD.Print($"🔥 進入無盡模式！第 {endlessWaveNum} 波開始！");

            StartCoroutine(SpawnEndlessWaveRoutine(endlessWaveNum));
        }
    }

    // 🌊 傳統波次生成
    private IEnumerator SpawnWaveRoutine(WaveData wave)
    {
        for (int i = 0; i < wave.Count; i++)
        {
            SpawnEnemy(wave.EnemyScene, 1.0f); // 1.0 倍血量
            yield return ToSignal(GetTree().CreateTimer(wave.SpawnInterval), SceneTreeTimer.SignalName.Timeout);
        }
        _isSpawningWave = false;
    }

    // 🔥 動態無盡波次生成邏輯
    private IEnumerator SpawnEndlessWaveRoutine(int waveNumber)
    {
        int endlessLevel = waveNumber - Waves.Count; // 無盡模式層數 (1, 2, 3...)
        
        // 1. 數量隨波次增加 (每波 +3 隻小怪)
        int enemyCount = 8 + endlessLevel * 3;
        // 2. 血量倍率隨波次成長 (每波血量增加 25%)
        float hpMultiplier = 1.0f + (endlessLevel * 0.25f);
        // 3. 生成間隔越來越緊湊 (最低 0.3 秒一隻)
        float spawnInterval = Mathf.Max(0.3f, 1.0f - (endlessLevel * 0.05f));

        for (int i = 0; i < enemyCount; i++)
        {
            // 隨機組合敵人：50% 普通怪、35% 快怪、15% Boss
            PackedScene selectedScene = NormalEnemyScene ?? Waves[0].EnemyScene;
            float roll = GD.Randf();

            if (roll > 0.85f && BossEnemyScene != null)
            {
                selectedScene = BossEnemyScene;
            }
            else if (roll > 0.5f && FastEnemyScene != null)
            {
                selectedScene = FastEnemyScene;
            }

            SpawnEnemy(selectedScene, hpMultiplier);
            yield return ToSignal(GetTree().CreateTimer(spawnInterval), SceneTreeTimer.SignalName.Timeout);
        }

        _isSpawningWave = false;
    }

    private void SpawnEnemy(PackedScene enemyScene, float hpMultiplier)
    {
        if (enemyScene == null || EnemyPath == null) return;

        var pathFollow = new PathFollow2D();
        pathFollow.Loop = false;
        EnemyPath.AddChild(pathFollow);

        var enemyNode = enemyScene.Instantiate<Node2D>();
        pathFollow.AddChild(enemyNode);

        // 無限模式血量加成！
        if (enemyNode is Enemy enemy && hpMultiplier > 1.0f)
        {
            enemy.MaxHealth = (int)(enemy.MaxHealth * hpMultiplier);
        }
    }

    private void UpdateWaveUI(string text)
    {
        if (WaveLabel != null)
        {
            WaveLabel.Text = text;
        }
    }

    // 簡化協程輔助方法
    private void StartCoroutine(IEnumerator routine)
    {
        System.Action step = null;
        step = () =>
        {
            if (routine.MoveNext())
            {
                if (routine.Current is SignalAwaiter awaiter)
                {
                    awaiter.OnCompleted(step);
                }
            }
        };
        step();
    }
}