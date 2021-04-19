using UnityEngine;

namespace FranoW
{
    public static class Gravity
    {

        public static float defaultValue = -9f;
        public static float defaultScaler = 7f;
        public static Vector3 Apply(Vector3 velocity, float value, float scaler, CharacterController cc)
        {
            velocity.y += value * scaler * Time.deltaTime;
            
            cc.Move(velocity * Time.deltaTime);
            return velocity;
        }
    }
}