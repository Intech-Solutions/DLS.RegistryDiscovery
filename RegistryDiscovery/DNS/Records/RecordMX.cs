#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
3.3.9. MX RDATA format

	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	|                  PREFERENCE                   |
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
	/                   EXCHANGE                    /
	/                                               /
	+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

PREFERENCE      A 16 bit integer which specifies the preference given to
				this RR among others at the same owner.  Lower values
				are preferred.

EXCHANGE        A <domain-name> which specifies a host willing to act as
				a mail exchange for the owner name.

MX records cause type A additional section processing for the host
specified by EXCHANGE.  The use of MX RRs is explained in detail in
[RFC-974].
*/

#endregion

public class RecordMX : Record, IComparable
{
    #region Public Members

    public string EXCHANGE;
	public ushort PREFERENCE;

    #endregion

    #region Constructors

    public RecordMX(RecordReader rr)
	{
		PREFERENCE	= rr.Readushort();
		EXCHANGE	= rr.ReadDomainName();
	}

	#endregion

	#region Public Methods

	public int CompareTo(object objA)
	{
		RecordMX recordMX = objA as RecordMX;

		if (recordMX == null)
			return -1;
		else if (PREFERENCE > recordMX.PREFERENCE)
			return 1;
		else if (PREFERENCE < recordMX.PREFERENCE)
			return -1;
		else // They are the same, now compare case insensitive names
			return string.Compare(EXCHANGE, recordMX.EXCHANGE, true);
	}

	public override string ToString()
	{
		return $"{PREFERENCE} {EXCHANGE}";
	}

    #endregion
}