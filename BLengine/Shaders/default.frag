uniform vec4 colour;
uniform vec3 viewPos;
uniform float FogEndDistance;
in vec2 texCoord;
in vec3 worldPosition;
in float depth;
uniform sampler2D diffuse;
uniform sampler2D normal;

out vec4 FragColor;

void main()
{
	//FragColor = vec4(worldPosition.rgb, 1.0f);
	vec4 diff = vec4(0.5f, 0.5f, 0.5f, 1.0f);
	vec3 norm = vec3(0.0f, 0.0f, 1.0f);
	vec3 lightDir = normalize(vec3(0.0f, 0f, 10.0f) - worldPosition);
	vec3 viewDir = normalize((viewPos) - worldPosition);
	vec3 halfwayDir = normalize(lightDir + viewPos);
	float shininess = 16;

	vec4 result = vec4(1.0f, 0.0f, 0.0f, 1.0f);
	#ifdef USE_DIFFUSE
		diff = vec4(texture(diffuse, texCoord).rgb, 1.0f);
		result = vec4(diff.rgb, 1.0f);
	#endif

	#ifdef USE_NORMAL
		norm = vec3(texture(normal, texCoord).rgb);
		result = vec4(norm.rgb * 2 - 1, 1.0f);
	#endif

	float lighting = max(dot(norm * 2 - 1, lightDir), 0.0f);
	vec3 lightcolour = vec3(0.3, 0.5, 0.9);
    vec4 colouredlight = vec4(lighting * lightcolour.r, lighting * lightcolour.g, lighting * lightcolour.b, 1.0f);


    float spec = pow(max(dot(norm, halfwayDir), 0.0), shininess);
    vec3 specular = lightcolour * spec;
	
    #ifdef LIT
	    result = vec4(vec3(diff.rgb + specular.rgb) * colouredlight.rgb, 1.0f);
	#endif

	#ifdef DEBUG_WORLDPOSITION
	    result = vec4(worldPosition.rgb, 1.0);// / 50, 0.0f, 0.0f, 1.0f);
	#endif

	#ifdef DEBUG_VIEWPOS
	    result = vec4(viewPos.rgb, 1.0f);
	#endif

    #ifdef DEBUG_FOG
		float fog = distance(viewPos.rgb, worldPosition.rgb) / FogEndDistance;
        result = vec4(fog, fog, fog, 1.0f);
    #endif


    #ifdef DEBUG_LIGHTING
        result = vec4(colouredlight.rgb + specular, 1.0f);
        result = vec4(viewDir.rgb, 1.0f);
        result = vec4(specular.rgb, 1.0f);
    #endif

	FragColor = result;
	//FragColor = vec4(depth, 0.0f, 0.0f, 1.0f);

}