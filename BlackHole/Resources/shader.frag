#version 330 core

const float PI = 3.14159265358979323846;

in vec2 TexCoord;

out vec4 FragColor;

uniform mat4 invView;
uniform vec2 resolution;
uniform samplerCube sampler;

struct Ray {
    vec3 origin;
    vec3 dir;
};

const float near = 1;
Ray GenerateCameraRay() {
    Ray ray;

    vec2 xy = 2.0*(gl_FragCoord.xy)/resolution - vec2(1.0);

    vec4 from = invView * vec4(xy, -near, 1);
    vec4 to = invView * vec4(xy, near, 1);
    from /= from.w;
    to /= to.w;

    ray.origin = from.xyz;
    ray.dir = normalize(to.xyz - from.xyz);

    return ray;
}

void main() {
    Ray ray = GenerateCameraRay();
    FragColor = texture(sampler, ray.dir);
}