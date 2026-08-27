class_name Bullet
extends Node2D

@export var speed: float = 500.0 # 子彈飛行速度

var _target: Node2D = null
var _damage: int = 0

# 設置攻擊目標與傷害
func seek(target: Node2D, damage: int) -> void:
	_target = target
	_damage = damage

func _process(delta: float) -> void:
	# 如果目標已經死亡或消失，子彈自我銷毀
	if not is_instance_valid(_target):
		queue_free()
		return

	# 朝著目標位置飛行
	var direction = global_position.direction_to(_target.global_position)
	global_position += direction * speed * delta
	
	# （選用）讓子彈轉向飛行方向
	# rotation = direction.angle()

	# 當距離目標小於 10 像素，視為命中！
	if global_position.distance_to(_target.global_position) < 10.0:
		if _target.has_method("take_damage"):
			_target.take_damage(_damage)
		queue_free() # 擊中後銷毀子彈
