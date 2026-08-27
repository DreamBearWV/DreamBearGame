class_name SpeedButton
extends Button

# 可自訂的倍速列表
var _speeds: Array[float] = [1.0, 2.0, 4.0]
var _speed_index: int = 0

func _ready() -> void:
	pressed.connect(_on_button_pressed)
	_update_speed_ui()

func _on_button_pressed() -> void:
	# 在 0 -> 1 -> 2 -> 0 之間循環切換
	_speed_index = (_speed_index + 1) % _speeds.size()
	Engine.time_scale = _speeds[_speed_index]
	
	_update_speed_ui()
	print("⚡ 切換至 %dx 速度！" % int(_speeds[_speed_index]))

func _update_speed_ui() -> void:
	var current_speed = int(_speeds[_speed_index])
	if current_speed == 1:
		text = "⏩ 1x 速度"
	else:
		text = "⚡ %dx 速度" % current_speed

func _exit_tree() -> void:
	# 重開遊戲或切換關卡時，自動恢復正常 1x 速度，避免影響新場景
	Engine.time_scale = 1.0
