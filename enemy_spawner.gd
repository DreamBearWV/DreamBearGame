class_name EnemySpawner
extends Node2D

# 改為 Array，可以在 Inspector 中自由設定數量並拉入多個怪物場景
@export var enemy_scenes: Array[PackedScene] = []
@export var path_2d: Path2D
@export var spawn_interval: float = 1.5

var _spawn_timer: Timer

func _ready() -> void:
	# 建立計時器來控制生成節奏
	_spawn_timer = Timer.new()
	_spawn_timer.wait_time = spawn_interval
	_spawn_timer.timeout.connect(_spawn_enemy)
	add_child(_spawn_timer)
	_spawn_timer.start()

func _spawn_enemy() -> void:
	# 檢查陣列是否為空或 Path2D 未綁定
	if enemy_scenes.is_empty() or not path_2d:
		return

	# 從陣列中隨機抽取一種怪物場景
	var selected_scene = enemy_scenes.pick_random()
	if not selected_scene:
		return

	# 1. 動態建立 PathFollow2D 節點並掛在 Path2D 底下
	var path_follow = PathFollow2D.new()
	path_follow.loop = false
	path_2d.add_child(path_follow)

	# 2. 將選中的 Enemy 場景實例化並掛在 PathFollow2D 底下
	var enemy = selected_scene.instantiate()
	path_follow.add_child(enemy)
