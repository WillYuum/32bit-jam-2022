using System.Collections.Generic;
using UnityEngine;

namespace DG.Tweening
{
    public static class TweenerExtensions
    {
        public static void OnCompleteAll(this List<Tweener> tweens, TweenCallback onCompleteAll)
        {
            int numTweens = tweens.Count;
            int tweensComplete = 0;

            for (int i = 0; i < numTweens; i++)
            {
                foreach (var tween in tweens)
                {
                    tween.OnComplete(() =>
                    {
                        tweensComplete++;

                        if (tweensComplete == numTweens)
                        {
                            onCompleteAll?.Invoke();
                        }
                    });
                }
            }
        }

        public static void OnCompleteAll(this Tweener[] tweens, TweenCallback onCompleteAll)
        {
            int numTweens = tweens.Length;
            int tweensComplete = 0;

            for (int i = 0; i < numTweens; i++)
            {
                foreach (var tween in tweens)
                {
                    tween.OnComplete(() =>
                    {
                        Debug.Log("tween complete here");
                        tweensComplete++;

                        if (tweensComplete == numTweens)
                        {
                            onCompleteAll?.Invoke();
                        }
                    });
                }
            }
        }
    }
}