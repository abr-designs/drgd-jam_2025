using UnityEngine;
using Utilities.Debugging;
using Utilities.WaitForAnimations.Base;

namespace Utilities
{
    /// <summary>
    /// Sets the position of a transform over time
    /// </summary>
    public class WaitForMoveAnimationsv2 : WaitForAnimationBase<Transform, Vector3>
    {
        [SerializeField]
        private AnimationCurve heightAddCurve;
        [SerializeField]
        private float heightMult;
        //============================================================================================================//
        
        public override Coroutine DoAnimation(float time, ANIM_DIR animDir)
        {
            return StartCoroutine(DoAnimationCoroutine(time, animDir));
        }

        protected override Vector3 Lerp(Vector3 start, Vector3 end, float t)
        {
            return Vector3.Lerp(start, end, t) + (Vector3.up * heightAddCurve.Evaluate(t) * heightMult);
        }

        protected override void SetValue(AnimationData data, Vector3 value)
        {
            data.transform.position = value;
        }

        //============================================================================================================//

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            const float RESOLUTION = 20;
            if (objectsToAnimate == null || objectsToAnimate.Length == 0)
                return;
            
            for (int i = 0; i < objectsToAnimate.Length; i++)
            {
                var objectToAnimate = objectsToAnimate[i];

                Gizmos.color = Color.yellow;
                for (int ii = 1; ii < RESOLUTION; ii++)
                {
                    var start = Lerp(objectToAnimate.start, objectToAnimate.end, (float)(ii - 1) / RESOLUTION);
                    var end = Lerp(objectToAnimate.start, objectToAnimate.end, (float)ii / RESOLUTION);
                    
                    Gizmos.DrawLine(start, end);
                }

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(objectToAnimate.start, 0.5f);
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(objectToAnimate.end, 0.5f);
                
                if(objectToAnimate.transform == null)
                    continue;
                
                var midPoint = (objectToAnimate.start + objectToAnimate.end) / 2f;
                Draw.Label(midPoint, objectToAnimate.transform.name);
            }
        }
#endif
    }
}