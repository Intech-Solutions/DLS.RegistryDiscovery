#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
3.2. RR definitions

3.2.1. Format

All RRs have the same top level format shown below:

									1  1  1  1  1  1
	  0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|                                               |
	/                                               /
	/                      NAME                     /
	|                                               |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|                      TYPE                     |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|                     CLASS                     |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|                      TTL                      |
	|                                               |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|                   RDLENGTH                    |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
	/                     RDATA                     /
	/                                               /
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+


where:

NAME            an owner name, i.e., the name of the node to which this
				resource record pertains.

TYPE            two octets containing one of the RR TYPE codes.

CLASS           two octets containing one of the RR CLASS codes.

TTL             a 32 bit signed integer that specifies the time interval
				that the resource record may be cached before the source
				of the information should again be consulted.  Zero
				values are interpreted to mean that the RR can only be
				used for the transaction in progress, and should not be
				cached.  For example, SOA records are always distributed
				with a zero TTL to prohibit caching.  Zero values can
				also be used for extremely volatile data.

RDLENGTH        an unsigned 16 bit integer that specifies the length in
				octets of the RDATA field.

RDATA           a variable length string of octets that describes the
				resource.  The format of this information varies
				according to the TYPE and CLASS of the resource record.
*/
#endregion

/// <summary>
/// Resource Record (rfc1034 3.6.)
/// </summary>
public class RR
{
	#region Public Members

	/// <summary>
	/// Specifies type class of resource record, mostly IN but can be CS, CH or HS 
	/// </summary>
	public Class Class;

	/// <summary>
	/// The name of the node to which this resource record pertains
	/// </summary>
	public string NAME;

	/// <summary>
	/// 
	/// </summary>
	public ushort RDLENGTH;

	/// <summary>
	/// One of the Record* classes
	/// </summary>
	public Record RECORD;

	public int TimeLived;

	/// <summary>
	/// Specifies type of resource record
	/// </summary>
	public Type Type;

	#endregion

	#region Internal Members

	private uint m_TTL;

	#endregion

	#region Constructors

	public RR(RecordReader rr)
	{
		TimeLived	= 0;
		NAME		= rr.ReadDomainName();
		Type		= (Type)rr.Readushort();
		Class		= (Class)rr.Readushort();
		TTL			= rr.Readuint();
		RDLENGTH	= rr.Readushort();
		RECORD		= rr.ReadRecord(Type, RDLENGTH);
		RECORD.RR	= this;
	}

	#endregion

	#region Fields

	/// <summary>
	/// Time to live, the time interval that the resource record may be cached
	/// </summary>
	public uint TTL
	{
		get
		{
			return (uint)Math.Max(0, m_TTL - TimeLived);
		}
		set
		{
			m_TTL = value;
		}
	}

	#endregion

	#region Public Methods

	public override string ToString()
	{
		return $"{NAME,-32} {TTL}\t{Class}\t{Type}\t{RECORD}";
	}

	#endregion
}

public class AnswerRR : RR
{
	public AnswerRR(RecordReader br)
		: base(br)
	{
	}
}

public class AuthorityRR : RR
{
	public AuthorityRR(RecordReader br)
		: base(br)
	{
	}
}

public class AdditionalRR : RR
{
	public AdditionalRR(RecordReader br)
		: base(br)
	{
	}
}