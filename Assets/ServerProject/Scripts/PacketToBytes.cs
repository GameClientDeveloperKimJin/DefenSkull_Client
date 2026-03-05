using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketToBytes 
{
    public static byte[] Make(PACKETID packetID, byte[] bodyData)
    {
        byte type = 0;
        var pktID = (UInt16)packetID;
        UInt16 bodyDataSize = 0;
        if (bodyData != null)
        {
            bodyDataSize = (UInt16)bodyData.Length;
        }
        var packetSize = (UInt16)(bodyDataSize + PacketDef.PACKET_HEADER_SIZE);

        var dataSource = new byte[packetSize];
        Buffer.BlockCopy(BitConverter.GetBytes(packetSize), 0, dataSource, 0, 2);
        Buffer.BlockCopy(BitConverter.GetBytes(pktID), 0, dataSource, 2, 2);
        dataSource[4] = type;

        if (bodyData != null)
        {
            Buffer.BlockCopy(bodyData, 0, dataSource, 5, bodyDataSize);
        }

        return dataSource;
    }

    public static Tuple<int, byte[]> ClientReceiveData(int recvLength, byte[] recvData)
    {
        var packetSize = BitConverter.ToUInt16(recvData, 0);
        var packetID = BitConverter.ToUInt16(recvData, 2);
        var bodySize = packetSize - PacketDef.PACKET_HEADER_SIZE;

        var packetBody = new byte[bodySize];
        Buffer.BlockCopy(recvData, PacketDef.PACKET_HEADER_SIZE, packetBody, 0, bodySize);

        return new Tuple<int, byte[]>(packetID, packetBody);
    }
}
