class_name Tower
extends Node2D

@export var attack_range: float = 150.0
@export var damage: int = 10
@export var fire_rate: float = 1.0 # 攻擊間隔 (秒)

@export var level: int = 1
@export var max_level: int = 2
@export var upgrade_cost: int = 75

@export var bullet_scene: PackedScene

var _time_since_last_attack: float = 0.0
var _is_range_visible: bool = false

func _process(delta: float) -> void:
	_time_since_last_attack += delta

	if _time_since_last_attack >= fire_rate:
		var target = _find_closest_enemy()
		if target:
			_attack(target)
			_time_since_last_attack = 0.0

func set_range_visible(visible: bool) -> void:
	if _is_range_visible != visible:
		_is_range_visible = visible
		queue_redraw() # 觸發 _draw() 重新繪製

func _draw() -> void:
	if _is_range_visible:
		# 若整體 Node2D 被縮放 (scale)，繪製時需除以 scale 避免視覺範圍圈放大變形
		var draw_radius = attack_range / scale.x
		
		# 1. 畫半透明天藍色填滿圓圈
		draw_circle(Vector2.ZERO, draw_radius, Color(0.2, 0.6, 1.0, 0.2))
		# 2. 畫天藍色外框
		draw_arc(Vector2.ZERO, draw_radius, 0, TAU, 64, Color(0.2, 0.6, 1.0, 0.8), 2.0)

func upgrade() -> bool:
	if level >= max_level:
		return false

	level += 1
	damage += 15
	attack_range += 50.0
	fire_rate *= 0.7

	scale = Vector2(1.3, 1.3)
	queue_redraw()

	print("⚡ 防禦塔升級成功！當前等級: LV%d" % level)
	return true

func _find_closest_enemy() -> Node2D:
	var enemies = get_tree().get_nodes_in_group("Enemies")
	var closest: Node2D = null
	var min_distance: float = attack_range

	for node in enemies:
		if is_instance_valid(node):
			var dist = global_position.distance_to(node.global_position)
			if dist < min_distance:
				min_distance = dist
				closest = node
	return closest

func _attack(enemy: Node2D) -> void:
	if not bullet_scene:
		if "take_damage" in enemy:
			enemy.take_damage(damage)
		return

	var bullet = bullet_scene.instantiate()
	get_tree().current_scene.add_child(bullet)
	bullet.global_position = global_position

	if bullet.has_method("seek"):
		bullet.seek(enemy, damage)
