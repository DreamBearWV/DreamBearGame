class_name GameManager
extends Node

# 靜態單例實體
static var instance: GameManager

@export var health: int = 10
@export var money: int = 100

@export var health_label: Label
@export var money_label: Label
@export var game_over_label: Label
@export var win_label: Label

var _is_game_over: bool = false

func _ready() -> void:
	instance = self  # 註冊實體 (GDScript 請使用 self)
	process_mode = PROCESS_MODE_ALWAYS
	_update_ui()

	if game_over_label:
		game_over_label.visible = false
	if win_label:
		win_label.visible = false

func add_money(amount: int) -> void:
	money += amount
	_update_ui()

func spend_money(amount: int) -> bool:
	if money >= amount:
		money -= amount
		_update_ui()
		return true
	return false

func take_damage(amount: int) -> void:
	if _is_game_over:
		return

	health -= amount
	if health < 0:
		health = 0
	_update_ui()

	if health <= 0:
		_is_game_over = true
		print("💀 遊戲結束！玩家基地被摧毀！")

		if game_over_label:
			game_over_label.text = "💀 GAME OVER 💀\n按下 [ R ] 重新開始遊戲"
			game_over_label.visible = true

		get_tree().paused = true

func win_game() -> void:
	if _is_game_over:
		return

	_is_game_over = true
	print("🎉 恭喜通關！成功防守所有敵人波次！")

	if win_label:
		win_label.text = "🎉 YOU WIN! 勝利通關！ 🎉\n按下 [ R ] 重新玩一次"
		win_label.visible = true
	elif game_over_label:
		game_over_label.text = "🎉 YOU WIN! 勝利通關！ 🎉\n按下 [ R ] 重新玩一次"
		game_over_label.visible = true

	get_tree().paused = true

func _unhandled_input(event: InputEvent) -> void:
	if _is_game_over and event is InputEventKey:
		if event.pressed and event.keycode == KEY_R:
			get_tree().paused = false
			get_tree().reload_current_scene()

func _update_ui() -> void:
	if health_label:
		health_label.text = "❤️ 血量: %d" % health
	if money_label:
		money_label.text = "💰 金錢: %d" % money
