using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [SerializeField] float fadespeed; //��������Ʈ ���� ������ �ֱ����Ѱ�

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
        Sprite t_Sprite = Resources.Load("Characters/" + p_SpriteName, typeof(Sprite)) as Sprite; //��������Ʈ ������ ���� ��� ���� ���ҽ� ���� > ĳ������

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
