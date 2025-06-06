using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashManager : MonoBehaviour
{
    [SerializeField] Image image;

    [SerializeField] Color colorWhite;
    [SerializeField] Color colorBlack;

    [SerializeField] float fadeSpeed;
    [SerializeField] float fadeSlowSpeed;

    public static bool isfinished = true;

    public IEnumerator Splash()
    {   
        isfinished = false;
        StartCoroutine(FadeOut(true, false));
        yield return new WaitUntil(() => isfinished);
        isfinished = false;
        StartCoroutine(FadeIn(true, false));
    }

    public IEnumerator FadeOut(bool _isWhite, bool _isSlow)
    {
        Color t_Color = (_isWhite == true) ? colorWhite : colorBlack;
        t_Color.a = 0;

        image.color = t_Color;

        while (t_Color.a < 1)
        {
            t_Color.a += (_isSlow == true) ? fadeSlowSpeed : fadeSpeed;
            image.color = t_Color;
            yield return null;
        }

        isfinished = true;
    }


    public IEnumerator FadeIn(bool _isWhite, bool _isSlow)
    {
        Color t_Color = (_isWhite == true) ? colorWhite : colorBlack;
        t_Color.a = 1;

        image.color = t_Color;

        while (t_Color.a > 0)
        {
            t_Color.a -= (_isSlow == true) ? fadeSlowSpeed : fadeSpeed;
            image.color = t_Color;
            yield return null;
        }

        isfinished = true;
    }


}
