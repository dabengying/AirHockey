
#version 400

layout(location = 0) in vec2 in_Position;

varying vec2 worldPosition;

uniform mat3 viewMatrix;
uniform mat3 projectionViewMatrix;

void main(void)
{
	worldPosition = vec2(viewMatrix * vec3(in_Position.xy, 1.0f));
	gl_Position = vec4(projectionViewMatrix * vec3(in_Position.xy, 1.0f), 1.0f);
}