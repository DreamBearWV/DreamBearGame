class_name TowerSpot
extends Button

@export var tower_scene: PackedScene
@export var tower_cost: int = 50

var _built_tower: Node = null

func _ready() -> void:
	pressed.connect(_on_button_pressed)

func _process(_delta: float) -> void:
	# 每幀檢查滑鼠是否懸停在按鈕上，顯示或隱藏範圍圈
	if is_instance_valid(_built_tower):
		_built_tower.set_range_visible(is_hovered())

func _on_button_pressed() -> void:
	# 已有塔：嘗試升級
	if is_instance_valid(_built_tower):
		if _built_tower.level < _built_tower.max_level:
			# 改用 GameManager.instance 呼叫
			if GameManager.instance and GameManager.instance.spend_money(_built_tower.upgrade_cost):
				_built_tower.upgrade()
			else:
				print("❌ 金錢不足，無法升級防禦塔！")
		else:
			print("⭐ 此防禦塔已達到最高等級！")
		return

	# 空平台：建造 LV1 塔
	if not tower_scene:
		print("❌ 報錯：TowerScene 是空的！")
		return

	# 改用 GameManager.instance 呼叫
	if GameManager.instance and GameManager.instance.spend_money(tower_cost):
		_built_tower = tower_scene.instantiate()
		_built_tower.position = size / 2
		add_child(_built_tower)
		flat = true # 隱藏預設的灰色背景框
		print("🏗️ 花費 %d 金錢成功建造 LV1 防禦塔！" % tower_cost)
	else:
		print("❌ 金錢不足，無法建造防禦塔！")
