class_name Enemy
extends Node2D

@export var max_health: int = 10:
	set(value):
		max_health = value
		_current_health = max_health
		if health_bar:
			health_bar.max_value = max_health
			health_bar.value = _current_health

@export var speed: float = 100.0
@export var reward_money: int = 15
@export var damage_to_player: int = 1

@export var health_bar: ProgressBar

var _current_health: int
var _path_follow: PathFollow2D

func _ready() -> void:
	add_to_group("Enemies")

	_current_health = max_health
	_path_follow = get_parent() as PathFollow2D

	if _path_follow:
		_path_follow.loop = false

	if health_bar:
		health_bar.max_value = max_health
		health_bar.value = _current_health

func _process(delta: float) -> void:
	if _path_follow:
		_path_follow.progress += speed * delta

		# 移動到終點 (99%) 時扣除玩家血量並銷毀
		if _path_follow.progress_ratio >= 0.99:
			if GameManager.instance:
				GameManager.instance.take_damage(damage_to_player)
			_path_follow.queue_free()

func take_damage(damage: int) -> void:
	_current_health -= damage

	if health_bar:
		health_bar.value = _current_health

	if _current_health <= 0:
		if GameManager.instance:
			GameManager.instance.add_money(reward_money)

		if _path_follow:
			_path_follow.queue_free()
		else:
			queue_free()
