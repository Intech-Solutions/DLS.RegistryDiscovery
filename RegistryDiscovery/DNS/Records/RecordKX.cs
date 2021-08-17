#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
*http://tools.ietf.org/rfc/rfc2230.txt
 * 
 * 3.1 KX RDATA format

   The KX DNS record has the following RDATA format:

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                  PREFERENCE                   |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   EXCHANGER                   /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

   where:

   PREFERENCE      A 16 bit non-negative integer which specifies the
                   preference given to this RR among other KX records
                   at the same owner.  Lower values are preferred.

   EXCHANGER       A <domain-name> which specifies a host willing to
                   act as a mail exchange for the owner name.

   KX records MUST cause type A additional section processing for the
   host specified by EXCHANGER.  In the event that the host processing
   the DNS transaction supports IPv6, KX records MUST also cause type
   AAAA additional section processing.

   The KX RDATA field MUST NOT be compressed.

 */

#endregion

public class RecordKX : Record, IComparable
{
	#region Public Members

	public string EXCHANGER;
	public ushort PREFERENCE;

    #endregion

    #region Constructors

    public RecordKX(RecordReader rr)
	{
		PREFERENCE	= rr.Readushort();
		EXCHANGER	= rr.ReadDomainName();
	}

	#endregion

	#region Public Methods

	public int CompareTo(object objA)
	{
		RecordKX recordKX = objA as RecordKX;
		if (recordKX == null)
			return -1;
		else if (PREFERENCE > recordKX.PREFERENCE)
			return 1;
		else if (PREFERENCE < recordKX.PREFERENCE)
			return -1;
		else // They are the same, now compare case insensitive names
			return string.Compare(EXCHANGER, recordKX.EXCHANGER, true);
	}

	public override string ToString()
	{
		return $"{PREFERENCE} {EXCHANGER}";
	}

    #endregion
}