#region Using Namespaces

using System;
using System.Text;

#endregion

public class RecordReader
{
	#region Internal Members

	private byte[] m_Data;
	private int m_Position;

	#endregion

	#region Constructors

	public RecordReader(byte[] data)
	{
		m_Position	= 0;
		m_Data		= data;
	}

	public RecordReader(byte[] data, int Position)
	{
		m_Data		= data;
		m_Position	= Position;
	}

	#endregion

	#region Fields

	public int Length
	{
		get
		{
			if (m_Data == null)
				return 0;
			else
				return m_Data.Length;
		}
	}

	public int Position
	{
		get
		{
			return m_Position;
		}
		set
		{
			m_Position = value;
		}
	}

    #endregion

    #region Public Methods

    public byte ReadByte()
	{
		if (m_Position >= m_Data.Length)
			return 0;
		else
			return m_Data[m_Position++];
	}

	// Changed 28 August 2008
	public byte[] ReadBytes(int intLength)
	{
		byte[] list = new byte[intLength];
		for (int intI = 0; intI < intLength; intI++)
			list[intI] = ReadByte();
		return list;
	}

	public char ReadChar()
	{
		return (char)ReadByte();
	}

	public string ReadDomainName()
	{
		int length			= 0;
		StringBuilder name	= new StringBuilder();

		// Get  the length of the first label
		while ((length = ReadByte()) != 0)
		{
			// Top 2 bits set denotes domain name compression and to reference elsewhere
			if ((length & 0xc0) == 0xc0)
			{
				// Work out the existing domain name, copy this pointer
				RecordReader newRecordReader = new RecordReader(m_Data, (length & 0x3f) << 8 | ReadByte());

				name.Append(newRecordReader.ReadDomainName());
				return name.ToString();
			}

			// If not using compression, copy a char at a time to the domain name
			while (length > 0)
			{
				name.Append(ReadChar());
				length--;
			}
			name.Append('.');
		}
		if (name.Length == 0)
			return ".";
		else
			return name.ToString();
	}

	public Record ReadRecord(Type type, int Length)
	{
		switch (type)
		{
			case Type.A:
				return new RecordA(this);
			case Type.NS:
				return new RecordNS(this);
			case Type.MD:
				return new RecordMD(this);
			case Type.MF:
				return new RecordMF(this);
			case Type.CNAME:
				return new RecordCNAME(this);
			case Type.SOA:
				return new RecordSOA(this);
			case Type.MB:
				return new RecordMB(this);
			case Type.MG:
				return new RecordMG(this);
			case Type.MR:
				return new RecordMR(this);
			case Type.NULL:
				return new RecordNULL(this);
			case Type.WKS:
				return new RecordWKS(this);
			case Type.PTR:
				return new RecordPTR(this);
			case Type.HINFO:
				return new RecordHINFO(this);
			case Type.MINFO:
				return new RecordMINFO(this);
			case Type.MX:
				return new RecordMX(this);
			case Type.TXT:
				return new RecordTXT(this, Length);
			case Type.RP:
				return new RecordRP(this);
			case Type.AFSDB:
				return new RecordAFSDB(this);
			case Type.X25:
				return new RecordX25(this);
			case Type.ISDN:
				return new RecordISDN(this);
			case Type.RT:
				return new RecordRT(this);
			case Type.NSAP:
				return new RecordNSAP(this);
			case Type.NSAPPTR:
				return new RecordNSAPPTR(this);
			case Type.SIG:
				return new RecordSIG(this);
			case Type.KEY:
				return new RecordKEY(this);
			case Type.PX:
				return new RecordPX(this);
			case Type.GPOS:
				return new RecordGPOS(this);
			case Type.AAAA:
				return new RecordAAAA(this);
			case Type.LOC:
				return new RecordLOC(this);
			case Type.NXT:
				return new RecordNXT(this);
			case Type.EID:
				return new RecordEID(this);
			case Type.NIMLOC:
				return new RecordNIMLOC(this);
			case Type.SRV:
				return new RecordSRV(this);
			case Type.ATMA:
				return new RecordATMA(this);
			case Type.NAPTR:
				return new RecordNAPTR(this);
			case Type.KX:
				return new RecordKX(this);
			case Type.CERT:
				return new RecordCERT(this);
			case Type.A6:
				return new RecordA6(this);
			case Type.DNAME:
				return new RecordDNAME(this);
			case Type.SINK:
				return new RecordSINK(this);
			case Type.OPT:
				return new RecordOPT(this);
			case Type.APL:
				return new RecordAPL(this);
			case Type.DS:
				return new RecordDS(this);
			case Type.SSHFP:
				return new RecordSSHFP(this);
			case Type.IPSECKEY:
				return new RecordIPSECKEY(this);
			case Type.RRSIG:
				return new RecordRRSIG(this);
			case Type.NSEC:
				return new RecordNSEC(this);
			case Type.DNSKEY:
				return new RecordDNSKEY(this);
			case Type.DHCID:
				return new RecordDHCID(this);
			case Type.NSEC3:
				return new RecordNSEC3(this);
			case Type.NSEC3PARAM:
				return new RecordNSEC3PARAM(this);
			case Type.HIP:
				return new RecordHIP(this);
			case Type.SPF:
				return new RecordSPF(this);
			case Type.UINFO:
				return new RecordUINFO(this);
			case Type.UID:
				return new RecordUID(this);
			case Type.GID:
				return new RecordGID(this);
			case Type.UNSPEC:
				return new RecordUNSPEC(this);
			case Type.TKEY:
				return new RecordTKEY(this);
			case Type.TSIG:
				return new RecordTSIG(this);
			default:
				return new RecordUnknown(this);
		}
	}

	public string ReadString()
	{
		short length = ReadByte();
		StringBuilder str = new StringBuilder();
		for (int intI = 0; intI < length; intI++)
			str.Append(ReadChar());
		return str.ToString();
	}

	public ushort Readushort()
	{
		return (ushort)(ReadByte() << 8 | ReadByte());
	}

	public ushort Readushort(int offset)
	{
		m_Position += offset;
		return Readushort();
	}

	public uint Readuint()
	{
		return (uint)(Readushort() << 16 | Readushort());
	}

    #endregion
}