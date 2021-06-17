// given the vertex of a quad, billboard it towards the camera in such a way that it will
// contain the given radius, and return the transformed position in world space
float3 billboard(float4 vertex, float radius)
{
    // "world space pivot", aka the center of our quad in world space
    float3 worldSpacePivot = unity_ObjectToWorld._m03_m13_m23;

    // vector from the quad center to the camera
    float3 worldSpacePivotToCamera = _WorldSpaceCameraPos.xyz - worldSpacePivot;

    // get object scale
    float3 scale = float3(
        length(unity_ObjectToWorld._m00_m10_m20),
        length(unity_ObjectToWorld._m01_m11_m21),
        length(unity_ObjectToWorld._m02_m12_m22)
    );

    // find the largest scale axis
    float maxScale = max(abs(scale.x), max(abs(scale.y), abs(scale.z)));

    // radius is half the largest scale
    float maxRadius = maxScale * radius;

    // sine of the right triangle formed by the camera to quad center to sphere edge
    float sinAngle = maxRadius / length(worldSpacePivotToCamera);

    // convert to cosine and then tangent
    float cosAngle = sqrt(1.0 - sinAngle * sinAngle);
    float tanAngle = sinAngle / cosAngle;

    // use the tangent to get the quad scale needed to contain the whole sphere
    // (it's some trigonometric witchcraft)
    float quadScale = tanAngle * length(worldSpacePivotToCamera) * 2.0;

    // camera up vector
    float3 up = UNITY_MATRIX_I_V._m01_m11_m21;

    // desired forward vector for our quad: towards the camera position
    float3 forward = normalize(worldSpacePivotToCamera);

    // "right" vector really ties the rotation together
    float3 right = normalize(cross(forward, up));

    // lock it down tight (is this really nessecary? idk i'm just copying code)
    up = cross(right, forward);

    // birth our beautiful little rotation matrix into the world
    float3x3 rotationMatrix = float3x3(right, up, forward);

    // project vertex into world space using our hand-crafted rotation matrix
    // (note that the rotation matrix as defined above is actually transposed, but multiplying
    //  vertex-first is a cheap workaround for this)
    return mul(float3(vertex.xy, 0.0) * quadScale, rotationMatrix) + worldSpacePivot;
}

float sphereIntersect(float3 origin, float3 direction, float4 sphere)
{
    float3 oc = origin - sphere.xyz;
    float b = dot(oc, direction);
    float c = dot(oc, oc) - sphere.w * sphere.w;
    float h = b * b - c;
    if (h < 0.0) return -1.0;
    h = sqrt(h);
    return -b - h;
}
