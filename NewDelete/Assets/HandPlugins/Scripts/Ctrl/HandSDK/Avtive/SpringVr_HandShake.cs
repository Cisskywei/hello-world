using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpringVr_HandShake
{
    private static SpringVr_HandShake instance;
    public static SpringVr_HandShake Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SpringVr_HandShake();
            }
            return instance;
        }
    }
    private SpringVr_ReadDataBase   readData;
    private List<byte[]>            allWriteDataByte;
    private string[]                writeDatas;
    private byte[]                  allByte;
    private byte                    resultByte;
    private SpringVr_HandShake()
    {
        CharactersStructure();
    }
    private void CharactersStructure()
    {
        allWriteDataByte = new List<byte[]>();
        writeDatas = new string[2];
        writeDatas[0] = "FE80000160E1FD";
        writeDatas[1] = "FE8000010081FD";
        for (int i = 0; i < writeDatas.Length; i++)
        {
            allWriteDataByte.Add(new byte[7]);
            for (int j = 0; j < 7; j++)
            {
                allWriteDataByte[i][j] = Convert.ToByte(writeDatas[i].Substring(j * 2, 2), 16);
            }
        }
    }
    private SpringVr_ReadDataBase HandChooseFunction(SpringVr_Base.WitchHand hand)
    {
        switch (hand)
        {
            case SpringVr_Base.WitchHand.LeftHand:
                readData = SpringVr_ReadLeftHandData.Instance;
                break;
            case SpringVr_Base.WitchHand.RightHand:
                readData = SpringVr_ReadRightHandData.Instance;
                break;
            default:
                break;
        }
        return readData;
    }
    /// <summary>计算震动强度</summary>
    private void CalculateWriteData_0(byte m_byteIntensityOfVibration)
    {
        allByte = new byte[3];
        for (int i = 0; i < 3; i++)
        {
            allByte[i] = Convert.ToByte(writeDatas[0].Substring(2 + i * 2, 2), 16);
        }
        resultByte = allByte[0];
        for (int i = 1; i < allByte.Length; i++)
        {
            resultByte ^= allByte[i];
        }
        resultByte ^= m_byteIntensityOfVibration;
        //byte[] canUseBytes = new byte[7];
        allWriteDataByte[0][4] = m_byteIntensityOfVibration;
        allWriteDataByte[0][5] = resultByte;
    }
    /// <summary>震动接口</summary>
    /// <param name="hand">HandChoose的枚举类型，表示触发左手还是右手震动</param>
    /// <param name="intensityOfVibration">0-100的震动强度，当强度为0时震动停止</param>
    public void HandShakeInterface(SpringVr_Base.WitchHand hand,int intensityOfVibration)
    {
        Mathf.Clamp(intensityOfVibration,25, 50);
        byte shakeValue = (byte)intensityOfVibration;
        CalculateWriteData_0(shakeValue);
        if (HandChooseFunction(hand).m_bolBeginReadData)
        {
            HandChooseFunction(hand).WriteData(allWriteDataByte[0]);
        }
    }
}  