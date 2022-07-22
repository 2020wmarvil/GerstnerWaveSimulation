using UnityEngine;

public class GerstnerWave 
{
    /** Function for generating a Trochoidal wave */
    public static Vector3 Gerstner(Vector3 position, Vector2 direction, float steepness, float wavelength, float speed, float timeSinceStart)
    {
        float k = 2 * Mathf.PI / wavelength;

        Vector2 normalizedDirection = direction.normalized;

        float f = k * Vector2.Dot(normalizedDirection, new Vector2(position.x, position.z)) - (speed * timeSinceStart);
        float a = steepness / k;

        return new Vector3(normalizedDirection.x * a * Mathf.Cos(f), a * Mathf.Sin(f), normalizedDirection.y * a * Mathf.Cos(f));
    }
}
