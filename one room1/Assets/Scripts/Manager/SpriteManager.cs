using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [SerializeField] float fadespeed; //스프라이트 변경 딜레이 주기위한거

    bool CheckSameSprite(SpriteRenderer p_SpriteRenderer, Sprite p_Sprite)
    {
        if (p_SpriteRenderer.sprite == p_Sprite) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public IEnumerator SpriteChangeCoroutine(Transform p_Target, string p_SpriteName)
    {
        SpriteRenderer[] t_SpriteRenderer = p_Target.GetComponentsInChildren<SpriteRenderer>();
        Sprite t_Sprite = Resources.Load("Characters/" + p_SpriteName, typeof(Sprite)) as Sprite; //스프라이트 가져올 파일 경로 지정 리소스 폴더 > 캐릭터즈

        if (!CheckSameSprite(t_SpriteRenderer[0], t_Sprite)) 
        {
            Color t_color = t_SpriteRenderer[0].color;
            Color t_ShadowColor = t_SpriteRenderer[1].color;
            t_color.a = 0;
            t_ShadowColor.a = 0;
            t_SpriteRenderer[0].color = t_color;
            t_SpriteRenderer[1].color = t_ShadowColor;

            t_SpriteRenderer[0].sprite = t_Sprite;
            t_SpriteRenderer[1].sprite = t_Sprite;


            while (t_color.a < 1)
            {
                t_color.a += fadespeed;
                t_ShadowColor.a += fadespeed;
                t_SpriteRenderer[0].color = t_color;
                t_SpriteRenderer[1].color = t_ShadowColor;
                yield return null;
            }
        }

    }
}
