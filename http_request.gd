extends Node

@onready var http_request: HTTPRequest = $HTTPRequest
@export var status_label: Label  # 可綁定 UI Label 顯示連線狀態

func _ready() -> void:
	# 1. 綁定請求完成訊號
	http_request.request_completed.connect(_on_request_completed)
	
	# 2. 發送 GET 請求 (使用相對路徑即可貫穿 Nginx -> 3001 端口)
	var err = http_request.request("/api/health")
	if err != OK:
		print("HTTP 請求發送失敗，錯誤碼: ", err)

func _on_request_completed(result: int, response_code: int, headers: PackedStringArray, body: PackedByteArray) -> void:
	if response_code == 200:
		var json = JSON.new()
		var parse_err = json.parse(body.get_string_from_utf8())
		if parse_err == OK:
			var data = json.data
			print("後端資料成功存取: ", data)
			if status_label:
				status_label.text = "後端狀態: " + data.message
		else:
			print("JSON 解析錯誤")
	else:
		print("API 回應異常，狀態碼: ", response_code)
