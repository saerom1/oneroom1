using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{

    [SerializeField] float spinSpeed;
    [SerializeField] Vector3 spinDir; //ȸ������ ��, ���� ���̷���
   



    // Update is called once per frame
    void Update()
    {
        transform.Rotate(spinDir * spinSpeed * Time.deltaTime);
    }

    // undate�Լ��� 60�������̴� 1�ʿ� �� 60�� ����
    // Time.deltaTime�� 60���� 1�� ����
    // spinSpeed�� 2��? 2*(1/60) * 60 >>> 1�ʿ� 2�� ȸ��
}
