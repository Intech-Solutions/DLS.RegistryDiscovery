#region Using Namespaces

using System;
using System.Net;
using System.Collections.Generic;

#endregion

#region RFC Info

/*
4.1.1. Header section format

The header contains the following fields:

									1  1  1  1  1  1
	  0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|                      ID                       |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|                    QDCOUNT                    |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|                    ANCOUNT                    |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|                    NSCOUNT                    |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|                    ARCOUNT                    |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

	where:

	ID              A 16 bit identifier assigned by the program that
					generates any kind of query.  This identifier is copied
					the corresponding reply and can be used by the requester
					to match up replies to outstanding queries.

	QR              A one bit field that specifies whether this message is a
					query (0), or a response (1).

	OPCODE          A four bit field that specifies kind of query in this
					message.  This value is set by the originator of a query
					and copied into the response.  The values are:

					0               a standard query (QUERY)

					1               an inverse query (IQUERY)

					2               a server status request (STATUS)

					3-15            reserved for future use

	AA              Authoritative Answer - this bit is valid in responses,
					and specifies that the responding name server is an
					authority for the domain name in question section.

					Note that the contents of the answer section may have
					multiple owner names because of aliases.  The AA bit
					corresponds to the name which matches the query name, or
					the first owner name in the answer section.

	TC              TrunCation - specifies that this message was truncated
					due to length greater than that permitted on the
					transmission channel.

	RD              Recursion Desired - this bit may be set in a query and
					is copied into the response.  If RD is set, it directs
					the name server to pursue the query recursively.
					Recursive query support is optional.

	RA              Recursion Available - this be is set or cleared in a
					response, and denotes whether recursive query support is
					available in the name server.

	Z               Reserved for future use.  Must be zero in all queries
					and responses.

	RCODE           Response code - this 4 bit field is set as part of
					responses.  The values have the following
					interpretation:

					0               No error condition

					1               Format error - The name server was
									unable to interpret the query.

					2               Server failure - The name server was
									unable to process this query due to a
									problem with the name server.

					3               Name Error - Meaningful only for
									responses from an authoritative name
									server, this code signifies that the
									domain name referenced in the query does
									not exist.

					4               Not Implemented - The name server does
									not support the requested kind of query.

					5               Refused - The name server refuses to
									perform the specified operation for
									policy reasons.  For example, a name
									server may not wish to provide the
									information to the particular requester,
									or a name server may not wish to perform
									a particular operation (e.g., zone
									transfer) for particular data.

					6-15            Reserved for future use.

	QDCOUNT         an unsigned 16 bit integer specifying the number of
					entries in the question section.

	ANCOUNT         an unsigned 16 bit integer specifying the number of
					resource records in the answer section.

	NSCOUNT         an unsigned 16 bit integer specifying the number of name
					server resource records in the authority records
					section.

	ARCOUNT         an unsigned 16 bit integer specifying the number of
					resource records in the additional records section.

	*/
#endregion

public class Header
{
	#region Public Members

	/// <summary>
	/// The number of resource records in the answer section
	/// </summary>
	public ushort ANCOUNT;

	/// <summary>
	/// The number of resource records in the additional records section
	/// </summary>
	public ushort ARCOUNT;

	/// <summary>
	/// Internal flag
	/// </summary>
	private ushort Flags;

	/// <summary>
	/// An identifier assigned by the program
	/// </summary>
	public ushort ID;

	/// <summary>
	/// The number of name server resource records in the authority records section
	/// </summary>
	public ushort NSCOUNT;

	/// <summary>
	/// The number of entries in the question section
	/// </summary>
	public ushort QDCOUNT;

    #endregion

    #region Constructors

    public Header()
	{
	}

	public Header(RecordReader rr)
	{
		ID		= rr.Readushort();
		Flags	= rr.Readushort();
		QDCOUNT = rr.Readushort();
		ANCOUNT = rr.Readushort();
		NSCOUNT = rr.Readushort();
		ARCOUNT = rr.Readushort();
	}

	#endregion

	#region Fields

	/// <summary>
	/// Authoritative Answer
	/// </summary>
	public bool AA
	{
		get
		{
			return GetBits(Flags, 10, 1) == 1;
		}
		set
		{
			Flags = SetBits(Flags, 10, 1, value);
		}
	}

	/// <summary>
	/// Represents the header as a byte array
	/// </summary>
	public byte[] Data
	{
		get
		{
			List<byte> data = new List<byte>();
			data.AddRange(WriteShort(ID));
			data.AddRange(WriteShort(Flags));
			data.AddRange(WriteShort(QDCOUNT));
			data.AddRange(WriteShort(ANCOUNT));
			data.AddRange(WriteShort(NSCOUNT));
			data.AddRange(WriteShort(ARCOUNT));
			return data.ToArray();
		}
	}

	/// <summary>
	/// Specifies kind of query
	/// </summary>
	public OPCode OPCODE
	{
		get
		{
			return (OPCode)GetBits(Flags, 11, 4);
		}
		set
		{
			Flags = SetBits(Flags, 11, 4, (ushort)value);
		}
	}

	/// <summary>
	/// Query (false), or a Response (true)
	/// </summary>
	public bool QR
	{
		get
		{
			return GetBits(Flags, 15, 1) == 1;
		}
		set
		{
			Flags = SetBits(Flags, 15, 1, value);
		}
	}

	/// <summary>
	/// Recursion Available
	/// </summary>
	public bool RA
	{
		get
		{
			return GetBits(Flags, 7, 1) == 1;
		}
		set
		{
			Flags = SetBits(Flags, 7, 1, value);
		}
	}

	/// <summary>
	/// Response code
	/// </summary>
	public RCode RCODE
	{
		get
		{
			return (RCode)GetBits(Flags, 0, 4);
		}
		set
		{
			Flags = SetBits(Flags, 0, 4, (ushort)value);
		}
	}

	/// <summary>
	/// Recursion Desired
	/// </summary>
	public bool RD
	{
		get
		{
			return GetBits(Flags, 8, 1) == 1;
		}
		set
		{
			Flags = SetBits(Flags, 8, 1, value);
		}
	}

	/// <summary>
	/// TrunCation
	/// </summary>
	public bool TC
	{
		get
		{
			return GetBits(Flags, 9, 1) == 1;
		}
		set
		{
			Flags = SetBits(Flags, 9, 1, value);
		}
	}

	/// <summary>
	/// Reserved for future use
	/// </summary>
	public ushort Z
	{
		get
		{
			return GetBits(Flags, 4, 3);
		}
		set
		{
			Flags = SetBits(Flags, 4, 3, value);
		}
	}

	#endregion

	#region Internal Methods

	private ushort GetBits(ushort oldValue, int position, int length)
	{
		// Sanity check
		if (length <= 0 || position >= 16)
			return 0;

		// Get some mask to put on
		int mask = (2 << (length - 1)) - 1;

		// Shift down to get some value and mask it
		return (ushort)((oldValue >> position) & mask);
	}

	private ushort SetBits(ushort oldValue, int position, int length, bool blnValue)
	{
		return SetBits(oldValue, position, length, blnValue ? (ushort)1 : (ushort)0);
	}

	private ushort SetBits(ushort oldValue, int position, int length, ushort newValue)
	{
		// Sanity check
		if (length <= 0 || position >= 16)
			return oldValue;

		// Get some mask to put on
		int mask = (2 << (length - 1)) - 1;

		// Clear out value
		oldValue &= (ushort)~(mask << position);

		// Set new value
		oldValue |= (ushort)((newValue & mask) << position);
		return oldValue;
	}

	private byte[] WriteShort(ushort sValue)
	{
		return BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)sValue));
	}

	#endregion
}