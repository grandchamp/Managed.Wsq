using System;

namespace Wsq.Managed
{
    internal class WsqHelper
    {
        /*used to "mask out" n number of bits from data stream*/
        public static int[] BITMASK = { 0x00, 0x01, 0x03, 0x07, 0x0f, 0x1f, 0x3f, 0x7f, 0xff };

        public const int MAX_DHT_TABLES = 8;
        public const int MAX_HUFFBITS = 16;
        public const int MAX_HUFFCOUNTS_WSQ = 256;

        public const int W_TREELEN = 20;
        public const int Q_TREELEN = 64;

        /* WSQ Marker Definitions */
        public const int SOI_WSQ = 0xffa0;
        public const int EOI_WSQ = 0xffa1;
        public const int SOF_WSQ = 0xffa2;
        public const int SOB_WSQ = 0xffa3;
        public const int DTT_WSQ = 0xffa4;
        public const int DQT_WSQ = 0xffa5;
        public const int DHT_WSQ = 0xffa6;
        public const int DRT_WSQ = 0xffa7;
        public const int COM_WSQ = 0xffa8;

        public const int STRT_SUBBAND_2 = 19;
        public const int STRT_SUBBAND_3 = 52;
        public const int MAX_SUBBANDS = 64;
        public const int NUM_SUBBANDS = 60;
        public const int STRT_SUBBAND_DEL = NUM_SUBBANDS;
        public const int STRT_SIZE_REGION_2 = 4;
        public const int STRT_SIZE_REGION_3 = 51;

        /* Case for getting ANY marker. */
        public const int ANY_WSQ = 0xffff;
        public const int TBLS_N_SOF = 2;
        public const int TBLS_N_SOB = TBLS_N_SOF + 2;
    }

    public class WavletTree
    {
        public int x;
        public int y;
        public int lenx;
        public int leny;
        public int invrw;
        public int invcl;
    }

    public class TableDTT
    {
        public float[] lofilt;
        public float[] hifilt;
        public int losz;
        public int hisz;
        public int lodef;
        public int hidef;
    }

    public class HuffCode
    {
        public int size;
        public int code;
    }

    public class HeaderFrm
    {
        public int black;
        public int white;
        public int width;
        public int height;
        public float mShift;
        public float rScale;
        public int wsqEncoder;
        public int software;
    }

    public class HuffmanTable
    {
        public int tableLen;
        public int bytesLeft;
        public int tableId;
        public int[] huffbits;
        public int[] huffvalues;
    }

    public class TableDHT
    {
        private const int MAX_HUFFBITS = 16; /*DO NOT CHANGE THIS CONSTANT!! */
        private const int MAX_HUFFCOUNTS_WSQ = 256; /* Length of code table: change as needed */

        public byte tabdef;
        public int[] huffbits = new int[MAX_HUFFBITS];
        public int[] huffvalues = new int[MAX_HUFFCOUNTS_WSQ + 1];
    }

    public class Table_DQT
    {
        public const int MAX_SUBBANDS = 64;
        public float binCenter;
        public float[] qBin = new float[MAX_SUBBANDS];
        public float[] zBin = new float[MAX_SUBBANDS];
        public int dqtDef;
    }

    public class QuantTree
    {
        public int x;    /* UL corner of block */
        public int y;
        public int lenx;  /* block size */
        public int leny;  /* block size */
    }

    public class IntRef
    {
        public int value;

        public IntRef() { }

        public IntRef(int value)
        {
            this.value = value;
        }
    }

    public class Token
    {
        public TableDHT[] tableDHT;
        public TableDTT tableDTT;
        public Table_DQT tableDQT;

        public WavletTree[] wtree;
        public QuantTree[] qtree;


        public byte[] buffer;
        public int pointer;

        public Token(Span<byte> buffer)
        {
            this.buffer = buffer.ToArray();
            pointer = 0;
        }

        public void Initialize()
        {
            tableDTT = new TableDTT();
            tableDQT = new Table_DQT();

            /* Init DHT Tables to 0. */
            tableDHT = new TableDHT[WsqHelper.MAX_DHT_TABLES];
            for (int i = 0; i < WsqHelper.MAX_DHT_TABLES; i++)
            {
                tableDHT[i] = new TableDHT
                {
                    tabdef = 0
                };
            }
        }

        public long ReadInt()
        {
            var byte1 = buffer[pointer++];
            var byte2 = buffer[pointer++];
            var byte3 = buffer[pointer++];
            var byte4 = buffer[pointer++];

            return (0xffL & byte1) << 24 | (0xffL & byte2) << 16 | (0xffL & byte3) << 8 | (0xffL & byte4);
        }

        public int ReadShort()
        {
            var byte1 = buffer[pointer++];
            var byte2 = buffer[pointer++];

            return (0xff & byte1) << 8 | (0xff & byte2);
        }

        public int ReadByte() => 0xff & buffer[pointer++];

        public Span<byte> ReadBytes(int size)
        {
            var bytes = new byte[size];

            for (int i = 0; i < size; i++)
                bytes[i] = buffer[pointer++];

            return bytes;
        }
    }
}
