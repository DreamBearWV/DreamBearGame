shader_type canvas_item;
render_mode unshaded;

uniform vec4 outline_color : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform float outline_width : hint_range(0.0, 12.0, 0.5) = 2.0;

// CanvasGroup 的合成畫面
uniform sampler2D screen_texture :
    hint_screen_texture, repeat_disable, filter_nearest;

void fragment() {
    vec4 base = textureLod(screen_texture, SCREEN_UV, 0.0);

    // 8-direction sampling：圓滑感比四方向較好
    vec2 px = SCREEN_PIXEL_SIZE * outline_width;
    float nearby_alpha = 0.0;

    nearby_alpha = max(nearby_alpha, texture(screen_texture, SCREEN_UV + vec2(-px.x,  0.0)).a);
    nearby_alpha = max(nearby_alpha, texture(screen_texture, SCREEN_UV + vec2( px.x,  0.0)).a);
    nearby_alpha = max(nearby_alpha, texture(screen_texture, SCREEN_UV + vec2( 0.0, -px.y)).a);
    nearby_alpha = max(nearby_alpha, texture(screen_texture, SCREEN_UV + vec2( 0.0,  px.y)).a);

    nearby_alpha = max(nearby_alpha, texture(screen_texture, SCREEN_UV + vec2(-px.x, -px.y)).a);
    nearby_alpha = max(nearby_alpha, texture(screen_texture, SCREEN_UV + vec2( px.x, -px.y)).a);
    nearby_alpha = max(nearby_alpha, texture(screen_texture, SCREEN_UV + vec2(-px.x,  px.y)).a);
    nearby_alpha = max(nearby_alpha, texture(screen_texture, SCREEN_UV + vec2( px.x,  px.y)).a);

    // 本身透明、附近有角色像素，才畫黑邊
    float outline_alpha = nearby_alpha * (1.0 - base.a);

    // CanvasGroup 原本的 premultiplied-alpha 行為還原
    if (base.a > 0.0001) {
        base.rgb /= base.a;
    }

    COLOR = mix(
        vec4(outline_color.rgb, outline_color.a * outline_alpha),
        base,
        base.a
    );
}