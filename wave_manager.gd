class_name WaveManager
extends Node

@export var enemy_path: Path2D
@export var waves: Array[Resource] = []
@export var time_between_waves: float = 5.0
@export var wave_label: Label
@export var is_endless_enabled: bool = true  # 是否在標準波次後開啟無限模式

@export_group("Endless Mode Enemies")
@export var normal_enemy_scene: PackedScene
@export var fast_enemy_scene: PackedScene
@export var boss_enemy_scene: PackedScene

var _current_wave_index: int = 0
var _is_spawning_wave: bool = false

func _ready() -> void:
	start_next_wave()

func _process(_delta: float) -> void:
	if _is_spawning_wave:
		return

	# 檢查場上是否還有活著且未被銷毀的敵人
	if not _has_active_enemies():
		_is_spawning_wave = true
		_current_wave_index += 1

		# 倒數計時進入下一波
		await get_tree().create_timer(time_between_waves).timeout
		start_next_wave()

func start_next_wave() -> void:
	_is_spawning_wave = true

	# 1. 標準波次
	if _current_wave_index < waves.size():
		update_wave_ui("🌊 波次: %d / %d" % [_current_wave_index + 1, waves.size()])
		spawn_wave_routine(waves[_current_wave_index])
	
	# 2. 無限模式
	elif is_endless_enabled:
		var endless_wave_num = _current_wave_index + 1
		update_wave_ui("🔥 無限模式: 第 %d 波" % endless_wave_num)
		print("🔥 進入無盡模式！第 %d 波開始！" % endless_wave_num)
		spawn_endless_wave_routine(endless_wave_num)
	
	# 3. 通關勝利 (無開啟無限模式時)
	else:
		if GameManager.instance:
			GameManager.instance.win_game()

# 🌊 傳統波次生成
func spawn_wave_routine(wave: WaveData) -> void:
	if not wave:
		_is_spawning_wave = false
		return

	for i in range(wave.count):
		spawn_enemy(wave.enemy_scene, 1.0) # 1.0 倍血量
		await get_tree().create_timer(wave.spawn_interval).timeout

	_is_spawning_wave = false

# 🔥 動態無盡波次生成邏輯
func spawn_endless_wave_routine(wave_number: int) -> void:
	var endless_level = wave_number - waves.size() # 無盡模式層數 (1, 2, 3...)

	var enemy_count = 8 + endless_level * 3
	var hp_multiplier = 1.0 + (endless_level * 0.25)
	var spawn_interval = maxf(0.3, 1.0 - (endless_level * 0.05))

	for i in range(enemy_count):
		var default_scene = normal_enemy_scene if normal_enemy_scene else (waves[0].enemy_scene if not waves.is_empty() else null)
		var selected_scene = default_scene
		var roll = randf()

		if roll > 0.85 and boss_enemy_scene:
			selected_scene = boss_enemy_scene
		elif roll > 0.5 and fast_enemy_scene:
			selected_scene = fast_enemy_scene

		spawn_enemy(selected_scene, hp_multiplier)
		await get_tree().create_timer(spawn_interval).timeout

	_is_spawning_wave = false

func spawn_enemy(enemy_scene: PackedScene, hp_multiplier: float) -> void:
	if not enemy_scene or not enemy_path:
		return

	var path_follow = PathFollow2D.new()
	path_follow.loop = false
	enemy_path.add_child(path_follow)

	var enemy_node = enemy_scene.instantiate()
	path_follow.add_child(enemy_node)

	# 無限模式血量加成
	if "max_health" in enemy_node and hp_multiplier > 1.0:
		enemy_node.max_health = int(enemy_node.max_health * hp_multiplier)

func _has_active_enemies() -> bool:
	var enemies = get_tree().get_nodes_in_group("Enemies")
	for enemy in enemies:
		if is_instance_valid(enemy) and not enemy.is_queued_for_deletion():
			return true
	return false

func update_wave_ui(text: String) -> void:
	if wave_label:
		wave_label.text = text
