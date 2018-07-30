
#version 400

layout(location = 0) in vec2 in_Position;
layout(location = 1) in vec4 in_Color;

uniform mat3 worldViewMatrix;
uniform mat3 worldMatrix;

varying vec4 ex_Color;
varying vec4 ex_Position;

void main(void)
{
	ex_Color = in_Color;
	ex_Position = vec4(worldMatrix * vec3(in_Position.xy, 1.0f), 1.0f);
	gl_Position = vec4(worldViewMatrix * vec3(in_Position.xy, 1.0f), 1.0f);
}

