using UnityEngine;

public struct Parabola
{
    public float Angle;
    public float Velocity;
    public float Height;
    public float Range;
    public float Time;

    static public Parabola CalculateSpeedAndAngleByTime(float time, float range, float altitude, out float speed, out float angle, float gravityMultiplier = 1f)
    {
        float gravity = Physics.gravity.magnitude * gravityMultiplier;
        float vx = range / time;
        float vy = (altitude + 0.5f * gravity * time * time) / time;
        Vector2 velocity = new Vector2(vx, vy);
        angle = Vector2.Angle(Vector2.right, velocity);
        if (Vector2.Angle(Vector2.down, velocity) < 90) angle = -angle;
        speed = velocity.magnitude;

        Parabola parabola;
        parabola.Angle = angle;
        parabola.Velocity = speed;
        parabola.Height = 0.5f * vy * vy / gravity;
        parabola.Range = range;
        parabola.Time = time;

        return parabola;
    }

    static public Parabola ABANDONED_CalculateSpeedByAngle(float angle, float range, float altitude, out float speed, float gravityMultiplier = 1f)
    {
        angle *= Mathf.Deg2Rad;
        float gravity = Physics.gravity.magnitude * gravityMultiplier;
        float tanOfAngle = Mathf.Tan(angle);

        float upside = Mathf.Sqrt(((tanOfAngle * tanOfAngle) + 1) * gravity * range);
        float downside = Mathf.Sqrt(2 * (tanOfAngle - gravity * altitude / range));
        speed = upside / downside;

        Parabola parabola;
        float vy = speed * Mathf.Sin(angle);
        parabola.Angle = angle * Mathf.Rad2Deg;
        parabola.Velocity = speed;
        parabola.Height = 0.5f * vy * vy / gravity;
        parabola.Range = range;
        parabola.Time = (vy / gravity) + Mathf.Sqrt(2 * (parabola.Height - altitude) / gravity);

        return parabola;
    }

    /// <summary>
    /// NOT Accurate enough, because something about Tangent and curve.
    /// </summary>
    /// <returns>The speed by angle.</returns>
    /// <param name="angle">Angle.</param>
    /// <param name="range">Range.</param>
    /// <param name="altitude">Altitude.</param>
    /// <param name="speed">Speed.</param>
    /// <param name="gravityMultiplier">Gravity multiplier.</param>
    static public Parabola CalculateSpeedByAngle(float angle, float range, float altitude, out float speed, float gravityMultiplier = 1f)
    {
        float gravity = Physics.gravity.magnitude * gravityMultiplier;
        angle *= Mathf.Deg2Rad;
        range += altitude / Mathf.Tan(angle);
        Debug.Log(altitude / Mathf.Tan(angle));
        speed = Mathf.Sqrt(range * gravity / Mathf.Sin(2 * angle));

        Parabola parabola;
        float vy = speed * Mathf.Sin(angle);
        parabola.Angle = angle * Mathf.Rad2Deg;
        parabola.Velocity = speed;
        parabola.Height = 0.5f * vy * vy / gravity;
        parabola.Range = range;
        parabola.Time = (vy / gravity) + Mathf.Sqrt(2 * (parabola.Height - altitude) / gravity);

        return parabola;
    }

    static public Parabola CalculateSpeedAndAngleByHeight(float height, float range, float altitude, out float angle, out float speed, float gravityMultiplier = 1f)
    {
        float gravity = Physics.gravity.magnitude * gravityMultiplier;
        float t1 = Mathf.Sqrt(2 * height / gravity);
        float t2 = Mathf.Sqrt(2 * (height - altitude) / gravity);
        float vx = range / (t1 + t2);
        float vy = gravity * t1;
        Vector2 launch = new Vector2(vx, vy);
        angle = Vector2.Angle(Vector2.right, launch);
        //if (Vector2.Angle(Vector2.down, launch) < 90) angle = -angle;
        speed = launch.magnitude;

        Parabola parabola;
        parabola.Angle = angle;
        parabola.Velocity = speed;
        parabola.Height = height;
        parabola.Range = range;
        parabola.Time = t1 + t2;

        return parabola;
    }

    /// <summary>
    /// Calculates the angle by speed. If there's no way can reach the target, the answers will be Infinity.
    /// There will always be two answer, the first will be higher, and fly longer, the second will be lower and fly shorter.
    /// However, the first one can easily bypass barriers, and second one will be fast.
    /// </summary>
    /// <param name="speed">Speed.</param>
    /// <param name="range">Range.</param>
    /// <param name="altitude">Altitude.</param>
    /// <param name="answer1">Answer1.</param>
    /// <param name="answer2">Answer2.</param>
    /// <param name="gravityMultiplier">Gravity multiplier.</param>
    static public Parabola[] CalculateAngleBySpeed(float speed, float range, float altitude, out float answer1, out float answer2, float gravityMultiplier = 1f)
    {
        Parabola[] parabola = new Parabola[2];

        float gravity = Physics.gravity.magnitude * gravityMultiplier;
        float _2PowerOfRange = Mathf.Pow(range, 2);
        float _2PowerOfSpeed = Mathf.Pow(speed, 2);
        float _4PowerOfSpeed = Mathf.Pow(_2PowerOfSpeed, 2);

        float equation = gravity * _2PowerOfRange + 2 * altitude * _2PowerOfSpeed;
        equation = _4PowerOfSpeed - gravity * equation;

        if (equation < 0)
        {
            answer1 = answer2 = Mathf.Infinity;
            return parabola;
        }
        equation = Mathf.Sqrt(equation);
        answer1 = _2PowerOfSpeed + equation;
        answer2 = _2PowerOfSpeed - equation;

        answer1 /= gravity * range;
        answer2 /= gravity * range;

        answer1 = Mathf.Atan(answer1);
        answer2 = Mathf.Atan(answer2);

        float vy = speed * Mathf.Sin(answer1);
        parabola[0].Angle = answer1 * Mathf.Rad2Deg;
        parabola[0].Velocity = speed;
        parabola[0].Height = 0.5f * vy * vy / gravity;
        parabola[0].Range = range;
        parabola[0].Time = (vy / gravity) + Mathf.Sqrt(2 * (parabola[0].Height - altitude) / gravity);

        vy = speed * Mathf.Sin(answer2);
        parabola[1].Angle = answer2 * Mathf.Rad2Deg;
        parabola[1].Velocity = speed;
        parabola[1].Height = 0.5f * vy * vy / gravity;
        parabola[1].Range = range;
        parabola[1].Time = (vy / gravity) + Mathf.Sqrt(2 * (parabola[1].Height - altitude) / gravity);

        answer1 = answer1 * Mathf.Rad2Deg;
        answer2 = answer2 * Mathf.Rad2Deg;

        return parabola;
    }
}
