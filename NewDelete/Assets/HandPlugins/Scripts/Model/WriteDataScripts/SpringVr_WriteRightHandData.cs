using UnityEngine;
using System.Collections.Generic;
using System;

public class SpringVr_WriteRightHandData : SpringVr_WriteDataBase
{
    public byte[] writeData;
    public byte[] fingerShake;
    private static SpringVr_WriteRightHandData instance;
    public static SpringVr_WriteRightHandData Instance
    {
        get
        {
            return instance;
        }
    }
    protected override void OnStart()
    {
        instance = this;
        Init();
    }
    private void Init()
    {
        fingerShake = new byte[5];
        for (int i = 0; i < fingerShake.Length; i++)
        {
            fingerShake[i] = 0x0;
        }
        writeData = new byte[7];
        writeData[0] = 0xFE;
        writeData[1] = 0x80;
        writeData[6] = 0xFD;
        writeData[4] = 0x01;
    }
    protected override void OnUpdate()
    {
        DataDeal();
    }

    private void DataDeal()
    {
        writeData[2] = isShake;
        isShake = 0x0;
        byte tempByte = 0x0;
        for (int i = 0; i < fingerShake.Length; i++)
        {
            tempByte += fingerShake[i];
        }
        writeData[3] = tempByte;
        writeData[5] = writeData[1];
        for (int i = 2; i < 5; i++)
        {
            writeData[5] ^= writeData[i];
        }
    }
}
